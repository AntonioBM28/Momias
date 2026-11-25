using UnityEngine;

/// <summary>
/// Puente generico entre WaveManager y cualquier Boss (BossPipila, MomiaMadre, ...).
/// - Al terminar las oleadas -> activa el boss, muestra barra, cambia musica.
/// - Al morir el boss (polling de EstaMuerto) -> oculta barra + spawnea Campana Sagrada.
/// - BossHealthBar hace polling directo de VidaActual cada frame (robusto).
///
/// CRITICO: vive en Assets/Scripts (runtime), nunca en Assets/Editor.
/// </summary>
public class BossBridge : MonoBehaviour
{
    [Header("Referencias")]
    public EnemyBase boss;
    public BossHealthBar bar;
    public Sprite bellSprite;

    [Header("Config")]
    public string bossName = "BOSS";
    public AudioClip musicBoss;

    private bool bossActivado;
    private bool muerteNotificada;

    void Start()
    {
        WaveManager wm = FindFirstObjectByType<WaveManager>();
        if (wm != null)
        {
            wm.OnAllWavesCompleted.AddListener(AparicionBoss);
            Debug.Log($"[BossBridge/{bossName}] ✅ Hook runtime a WaveManager.OnAllWavesCompleted");
        }
        else Debug.LogError($"[BossBridge/{bossName}] No se encontro WaveManager en escena!");

        if (boss == null) Debug.LogError($"[BossBridge/{bossName}] boss NULL en inspector");
        if (bar == null) Debug.LogError($"[BossBridge/{bossName}] bar NULL en inspector");
    }

    void Update()
    {
        // Polling de muerte del boss (evita depender de UnityEvents de clases concretas)
        if (bossActivado && !muerteNotificada && boss != null && boss.EstaMuerto)
        {
            muerteNotificada = true;
            OnBossMurio();
        }
    }

    public void AparicionBoss()
    {
        Debug.Log($"[BossBridge/{bossName}] AparicionBoss() invocado");
        if (bossActivado || boss == null) return;
        bossActivado = true;

        boss.gameObject.SetActive(true);

        int vidaMax = boss.Data != null ? boss.Data.vidaMaxima : 15;
        if (bar != null)
        {
            bar.Inicializar(bossName, vidaMax);
            bar.MostrarConFade();
            bar.SetBoss(boss);
            bar.SetVida(boss.VidaActual > 0 ? boss.VidaActual : vidaMax);
        }

        // Musica del boss (si esta asignada en inspector > fallback a AudioManager.musicBossPipila)
        AudioClip clip = musicBoss;
        if (clip == null && AudioManager.Instance != null) clip = AudioManager.Instance.musicBossPipila;
        if (clip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(clip, true);

        if (CameraShaker.Instance != null) CameraShaker.Instance.Shake(0.3f, 0.5f);
        Debug.Log($"[BossBridge/{bossName}] ⚔️ Boss aparecio, vidaMax={vidaMax}");
    }

    private void OnBossMurio()
    {
        Debug.Log($"[BossBridge/{bossName}] boss murio");
        if (bar != null) bar.OcultarConFade();
        SpawnCampanaSagrada();
    }

    private void SpawnCampanaSagrada()
    {
        if (bellSprite == null)
        {
            Debug.LogWarning($"[BossBridge/{bossName}] bellSprite null, no spawnea Campana Sagrada");
            return;
        }
        GameObject campana = new GameObject($"Campana Sagrada {bossName}");
        campana.transform.position = new Vector3(0f, -1.3f, 0f);
        SpriteRenderer sr = campana.AddComponent<SpriteRenderer>();
        sr.sprite = bellSprite;
        sr.sortingOrder = 15;
        campana.AddComponent<CircleCollider2D>();
        campana.AddComponent<SacredBell>();
        Debug.Log($"[BossBridge/{bossName}] 🔔 Campana Sagrada spawneada");
    }
}
