using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Oleada
    {
        public string nombre = "Oleada";
        public GameObject prefabEnemigo;
        public EnemyData dataEnemigo;
        public int cantidad = 3;
        public float intervaloSpawn = 1.5f;
        public float pausaAntesDeOleada = 2f;
        [Tooltip("Si true, pasa a la siguiente oleada SIN esperar que mueran los enemigos de esta. Permite fluir varias sub-oleadas como UNA sola oleada logica.")]
        public bool continuarSinEsperarMuertes = false;
    }

    [Header("Spawners")]
    [SerializeField] private Transform spawnIzquierdo;
    [SerializeField] private Transform spawnDerecho;

    [Header("Configuracion de oleadas")]
    [SerializeField] private List<Oleada> oleadas = new List<Oleada>();
    [SerializeField] private bool arrancarAutomatico = true;

    [Header("Campana Sagrada")]
    [SerializeField] private Sprite spriteCampanaSagrada;
    [SerializeField] private Vector3 posicionCampanaSagrada = new Vector3(0f, -1.3f, 0f);

    [Header("Eventos")]
    public UnityEvent<int, int> OnWaveChanged;
    public UnityEvent<int, int> OnEnemiesRemaining;
    public UnityEvent OnAllWavesCompleted;

    private int oleadaActual = -1;
    private int enemigosDeLaOleada;
    private int enemigosVivos;
    private readonly List<EnemyBase> vivos = new List<EnemyBase>();
    private bool corriendo;

    void Start()
    {
        if (arrancarAutomatico) IniciarOleadas();
    }

    public void IniciarOleadas()
    {
        if (corriendo || oleadas.Count == 0) return;
        corriendo = true;
        ShuffleBloquesRandom();
        StartCoroutine(LoopOleadas());
    }

    /// <summary>
    /// Dentro de cada bloque lógico (sub-oleadas consecutivas con continuarSinEsperarMuertes=true
    /// terminando en una con false), shuffle el orden de los tipos de enemigos.
    /// Los intervalos/pausas se mantienen en sus posiciones; solo rota el tipo/cantidad.
    /// </summary>
    private void ShuffleBloquesRandom()
    {
        int i = 0;
        while (i < oleadas.Count)
        {
            int fin = i;
            while (fin < oleadas.Count && oleadas[fin].continuarSinEsperarMuertes) fin++;
            int len = fin - i + 1;
            if (len > 1)
            {
                // Snapshot de tipos del bloque
                var tipos = new List<(GameObject prefab, EnemyData data, int cant)>();
                for (int k = i; k <= fin && k < oleadas.Count; k++)
                    tipos.Add((oleadas[k].prefabEnemigo, oleadas[k].dataEnemigo, oleadas[k].cantidad));

                // Fisher-Yates shuffle
                for (int k = tipos.Count - 1; k > 0; k--)
                {
                    int j = Random.Range(0, k + 1);
                    (tipos[k], tipos[j]) = (tipos[j], tipos[k]);
                }

                // Reasignar en orden shuffled
                for (int k = 0; k < len && (i + k) < oleadas.Count; k++)
                {
                    oleadas[i + k].prefabEnemigo = tipos[k].prefab;
                    oleadas[i + k].dataEnemigo = tipos[k].data;
                    oleadas[i + k].cantidad = tipos[k].cant;
                }
                Debug.Log($"[WaveManager] Bloque {i}-{fin} shuffleado ({len} sub-oleadas)");
            }
            i = fin + 1;
        }
    }

    private IEnumerator LoopOleadas()
    {
        for (int i = 0; i < oleadas.Count; i++)
        {
            oleadaActual = i;
            Oleada o = oleadas[i];
            OnWaveChanged?.Invoke(oleadaActual + 1, oleadas.Count);

            Debug.Log($"[WaveManager] Iniciando oleada {i + 1}/{oleadas.Count}: {o.nombre} ({o.cantidad} enemigos)");

            yield return new WaitForSeconds(o.pausaAntesDeOleada);

            enemigosDeLaOleada = o.cantidad;
            enemigosVivos = 0;
            for (int n = 0; n < o.cantidad; n++)
            {
                SpawnEnemigo(o, n);
                enemigosVivos++;
                OnEnemiesRemaining?.Invoke(enemigosVivos, enemigosDeLaOleada);
                yield return new WaitForSeconds(o.intervaloSpawn);
            }

            Debug.Log($"[WaveManager] Oleada {i + 1} spawneada.");

            // Si la oleada quiere continuar sin esperar, saltamos el bloqueo
            if (o.continuarSinEsperarMuertes)
            {
                Debug.Log($"[WaveManager] Oleada {i + 1} continua sin esperar muertes (fluye con siguiente).");
                continue;
            }

            // FIX: esperar a TODOS los enemigos vivos en escena, no solo los de esta sub-oleada
            // (sino al terminar la ultima sub con continuar=false, campana aparece con momias aun vivas)
            while (true)
            {
                LimpiarMuertos();
                if (vivos.Count == 0) break;
                yield return new WaitForSeconds(0.3f);
            }

            Debug.Log($"[WaveManager] Oleada {i + 1} completada (todos los vivos muertos).");
        }

        // Al cerrar el loop, asegurar que no queden vivos en escena antes de disparar Victory
        while (true)
        {
            LimpiarMuertos();
            if (vivos.Count == 0) break;
            yield return new WaitForSeconds(0.3f);
        }

        corriendo = false;
        SpawnCampanaSagrada();
        OnAllWavesCompleted?.Invoke();
        Debug.Log("[WaveManager] Todas las oleadas completadas. ¡Victoria!");
    }

    private void SpawnCampanaSagrada()
    {
        if (spriteCampanaSagrada == null)
        {
            // Null = este nivel NO spawnea Campana automatica (el boss se encarga).
            Debug.Log("[WaveManager] spriteCampanaSagrada null: no se spawnea automatica (boss-driven).");
            return;
        }

        GameObject campana = new GameObject("Campana Sagrada Nivel 1");
        campana.transform.position = posicionCampanaSagrada;

        SpriteRenderer renderer = campana.AddComponent<SpriteRenderer>();
        renderer.sprite = spriteCampanaSagrada;
        renderer.sortingOrder = 15;

        campana.AddComponent<CircleCollider2D>();
        campana.AddComponent<SacredBell>();
        Debug.Log("[WaveManager] Campana Sagrada apareció en el centro. Acércate y pulsa E.");
    }

    private void SpawnEnemigo(Oleada o, int indice)
    {
        if (o.prefabEnemigo == null)
        {
            Debug.LogError($"[WaveManager] Oleada '{o.nombre}' no tiene prefabEnemigo.");
            return;
        }

        Transform spawn = (indice % 2 == 0) ? spawnIzquierdo : spawnDerecho;
        if (spawn == null)
        {
            Debug.LogError("[WaveManager] Falta asignar spawnIzquierdo o spawnDerecho.");
            return;
        }

        GameObject instancia = Instantiate(o.prefabEnemigo, spawn.position, Quaternion.identity);
        EnemyBase eb = instancia.GetComponent<EnemyBase>();
        if (eb == null)
        {
            Debug.LogWarning($"[WaveManager] El prefab {o.prefabEnemigo.name} no tiene un script que herede de EnemyBase.");
            return;
        }

        // Asignar data (ya no por reflexion sino con metodo publico)
        if (o.dataEnemigo != null)
            eb.SetData(o.dataEnemigo);

        vivos.Add(eb);
        Debug.Log($"[WaveManager] Spawn #{indice} en {spawn.name} ({spawn.position}): {instancia.name}");
    }

    private void LimpiarMuertos()
    {
        for (int i = vivos.Count - 1; i >= 0; i--)
        {
            if (vivos[i] == null || vivos[i].EstaMuerto)
            {
                vivos.RemoveAt(i);
                enemigosVivos = Mathf.Max(0, enemigosVivos - 1);
                OnEnemiesRemaining?.Invoke(enemigosVivos, enemigosDeLaOleada);
            }
        }
    }
}
