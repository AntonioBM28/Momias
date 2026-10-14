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
    }

    [Header("Spawners")]
    [SerializeField] private Transform spawnIzquierdo;
    [SerializeField] private Transform spawnDerecho;

    [Header("Configuracion de oleadas")]
    [SerializeField] private List<Oleada> oleadas = new List<Oleada>();
    [SerializeField] private bool arrancarAutomatico = true;

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
        StartCoroutine(LoopOleadas());
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

            Debug.Log($"[WaveManager] Oleada {i + 1} spawneada. Esperando a que mueran todos.");

            while (enemigosVivos > 0)
            {
                LimpiarMuertos();
                yield return new WaitForSeconds(0.3f);
            }

            Debug.Log($"[WaveManager] Oleada {i + 1} completada.");
        }

        corriendo = false;
        OnAllWavesCompleted?.Invoke();
        Debug.Log("[WaveManager] Todas las oleadas completadas. ¡Victoria!");
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
