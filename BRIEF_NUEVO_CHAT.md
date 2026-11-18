Hola Claude. Soy Cristian Oropeza, programador en el proyecto "Momias
del Grito" (juego Unity 2D para la materia de Creación de Videojuegos).
Trabajo en pareja con mi compañero José Antonio Balderas Melchor (arte,
animación, audio, level design).

══════════════════════════════════════════════════════════
🎮 CONTEXTO GENERAL DEL PROYECTO
══════════════════════════════════════════════════════════

**Momias del Grito** es un arena-brawler 2D pixel art donde Diego
(protagonista, guía turístico nocturno del Museo de las Momias de
Guanajuato) defiende el centro de pantalla con una campana sónica
mientras momias de 4 variantes atacan por ambos flancos, a través de
4 niveles icónicos del estado de Guanajuato, culminando en un
enfrentamiento con La Momia Madre en la Parroquia de Dolores Hidalgo.

**Equipo:**
- **Cristian E. Oropeza** (yo) — programación, sistemas, integración
- **José Antonio Balderas Melchor** (Antonio) — arte, animación, audio,
  level design

**Repositorio:** https://github.com/AntonioBM28/Momias
**Entregas (ya hechas):** Avance 1 (prototipo), Avance 2 (Nivel 1 completo)
**Entregas pendientes:** Avance 3 (Niveles 2 y 3) — en curso ahora,
Avance 4 (Nivel 4 final), Avance 5 (builds multiplataforma), Final
(27 nov 2026).

══════════════════════════════════════════════════════════
📁 ARCHIVOS CLAVE EN MI COMPUTADORA
══════════════════════════════════════════════════════════

El proyecto completo está en: C:\Users\orope\OneDrive\Escritorio\Momias

**Archivos .md de contexto (léelos en este orden al arrancar):**
1. `JUEGO_Vision_Completa.md` ← La BIBLIA del juego. 13 secciones:
   historia, personajes, mecánicas, 4 estados del juego, 4 niveles
   con tablas de oleadas, items, audio, arquitectura, roadmap,
   división de trabajo, convenciones.
2. `ESTADO_Y_ROADMAP.md` ← Estado actual del proyecto al 11 nov 2026,
   análisis de lo que subió Antonio, problemas detectados, roadmap
   detallado.
3. Este archivo (`BRIEF_NUEVO_CHAT.md`) ← lo que tienes enfrente.

**Scripts actuales (ya funcionan, NO REESCRIBIR, solo modificar donde
diga la fase correspondiente):**
```
Assets/Scripts/
├── Data/EnemyData.cs
├── Enemies/
│   ├── EnemyBase.cs          (abstract, vida/daño/muerte compartidos)
│   └── MomiaCaminante.cs     (IA caminar + melee)
├── Items/SacredBell.cs       (Campana Sagrada al final del nivel)
├── Managers/
│   ├── AudioManager.cs       (singleton, música + SFX)
│   ├── GameManager.cs        (singleton, estado Menu/Playing/Paused/
│                              GameOver/Victory, DontDestroyOnLoad)
│   ├── SceneTransition.cs    (singleton, fades entre escenas)
│   └── WaveManager.cs        (gestor de oleadas con UnityEvents)
├── Player/
│   ├── PlayerController.cs   (mover, saltar W/Espacio/↑, agacharse S)
│   ├── PlayerHealth.cs       (vida con medios corazones)
│   ├── PlayerAttack.cs       (campanazo con hitbox)
│   └── AttackHitbox.cs       (zona de daño temporal)
└── UI/
    ├── Fader.cs              (anima alpha de un Image negro)
    ├── GameOverPanel.cs      (panel rojo + Reintentar/Menú)
    ├── HeartsHUD.cs          (6 corazones esquina sup izq)
    ├── LevelTitleCard.cs     (cartel "NIVEL 1" al cargar)
    ├── PauseMenu.cs          (Esc toggle, Time.timeScale=0)
    ├── VictoryCinematic.cs   (secuencia 3s: flash+shake+slowmo+letterbox+
                                zoom+onda dorada+fade a dorado)
    └── VictoryPanel.cs       (panel dorado con bounce)
```

**Editor scripts actuales:**
- `Assets/Editor/Chunk1Setup.cs` ← patrón de automatización con menú
  "Momias/Chunk 1/..." en la barra superior de Unity. Este es el
  PATRÓN que debes replicar para cualquier setup automatizado (ver
  sección de "Cómo trabajamos").

**Escenas:**
- `Assets/Scenes/Menu.unity` ← **VACÍA**, solo tiene Main Camera.
- `Assets/Scenes/Nivel1_Museo.unity` ← Funcional, WaveManager con 3
  oleadas (solo Caminantes), Victory/GameOver/Pausa funcionando, HUD
  completo, SacredBell.
- `Assets/Scenes/Nivel2_Alhondiga.unity` ← **Block-out**: solo backgrounds,
  paredes y luz. Sin Diego, sin WaveManager, sin UI, sin nada de
  gameplay.
- `Assets/Scenes/Nivel3_Callejones.unity` ← **Block-out**, igual que
  Nivel 2.

**Sprites y prefabs ya disponibles:**
- Diego: Idle, Walk, Run, Jump, Attack, Damage, Death (sprites + anims
  en Assets/Animations/)
- Momia Caminante: sprites + anims completos + prefab +
  `MomiaCaminante_Gris.asset` (ScriptableObject)
- **Momia Bebé**: sprites completos (Idle, Walk, Attack, Run, Die) +
  Animator + `MomiaBebe.prefab` + `MomiaBebe.asset`. ⚠️ El prefab USA
  SCRIPT `MomiaCaminante` (no tiene mecánica bebé).
- **Momia Pípila** (Boss): sprites (Walking, Jumping, TakingPunch,
  FallingBackDeath) + Animator + 5 anims + `MomiaPipila.prefab` +
  `MomiaPipila.asset`. ⚠️ El prefab **NO TIENE NINGÚN SCRIPT**.
- Backgrounds Nivel 1 (Capa1/2/3), Nivel 2 (Capa1/2/3), Nivel 3
  (Capa1/2/3) — todos listos.
- `CampanaSagrada.png`, `CorazonesHUD.png` (reales de Antonio).

**Audio ya disponible:**
- `Music/Musica_Ambiental.mp3` (música del Nivel 1, ya asignada).
- `SFX/Campanazo_Diego.mp3`, `SFX/Impacto_Momia.mp3`,
  `SFX/Recoger_Campana.mp3`.
- Pendiente de Antonio: música Menú, música Nivel 2 Alhóndiga (calma),
  música Boss Pípila, música Nivel 3, jingle Victoria, jingle Game Over,
  SFX salto, pickup corazón, UI click, daño Diego, muerte momia.

══════════════════════════════════════════════════════════
🛠️ STACK TÉCNICO
══════════════════════════════════════════════════════════

- **Motor:** Unity 6.6 (6000.0.f1) con URP 2D
- **Lenguaje:** C# (sin Bolt/Visual Scripting)
- **Input System:** VIEJO (Input.GetKey, Input.GetAxisRaw, Input.GetKeyDown).
  NO uses el New Input System.
- **Git + Git LFS** configurados.
- **TextMeshPro** importado (TMP_Text, TextMeshProUGUI).

══════════════════════════════════════════════════════════
📐 CONVENCIONES DEL PROYECTO
══════════════════════════════════════════════════════════

- **Nomenclatura scripts:** PascalCase (`PlayerController.cs`,
  `MomiaBebe.cs`).
- **Carpetas por responsabilidad:**
  `Scripts/{Data, Enemies, Items, Managers, Player, UI}`.
- **Prefabs:** `Assets/Prefabs/` con sub-carpetas (Enemies, Items,
  Player, UI).
- **ScriptableObjects:** `Assets/ScriptableObjects/Enemies/`.
- **Tags:** `Player`, `Enemy`, `Pickup` (futuro), `SacredBell`.
- **Layers:**
  - 6: Ground
  - 7: Boundary
  - 8: Player
  - 9: Enemy
- **Collision Matrix** (Project Settings → Physics 2D):
  - Enemy ↔ Enemy: DESACTIVADO
  - Player ↔ Enemy: DESACTIVADO
  - Enemy ↔ Boundary: DESACTIVADO
  - (los demás como default)
- **Comentarios en código:** en español, breves.
- **Docstrings:** `/// <summary>` arriba de cada clase explicando
  responsabilidad.
- **Secciones en inspector:** usa `[Header("...")]` para claridad.
- **Desacople entre sistemas:** usa `UnityEvent` (no `Action` directo).
  Ejemplos: `PlayerHealth.OnHealthChanged`, `WaveManager.OnAllWavesCompleted`.
- **Editor Scripts:** patrón de `Chunk1Setup.cs` con `[MenuItem]` para
  automatizar setup, helpers (`CreateImage`, `CreateTMP`, `CreateButton`,
  `StretchFull`, `SetAnchor`, `EnsureEventSystem`, `FindOrCreateGameUICanvas`).

══════════════════════════════════════════════════════════
✅ DECISIONES YA TOMADAS (NO VOLVER A PREGUNTAR)
══════════════════════════════════════════════════════════

**Nombres de niveles** (contradicen la Biblia pero así quedó):
- Nivel 1 = Museo (correcto)
- Nivel 2 = **Alhóndiga de Granaditas** (en la Biblia era Callejones)
- Nivel 3 = **Callejones de Guanajuato** (en la Biblia era Alhóndiga)
- Nivel 4 = Parroquia (correcto)
- Boss Pípila va en Nivel 2 (Alhóndiga) — homenaje al héroe.

**Flujo entre niveles:**
- Menu → Nivel 1 → Nivel 2 → Nivel 3 → Nivel 4 (directo, sin volver al
  menú entre niveles).
- Al ganar un nivel, botón "Siguiente Nivel" carga el siguiente
  directamente con fade.

**Música:**
- Menú Principal: 1 track (Antonio lo prepara).
- Nivel 1: 1 track (`Musica_Ambiental.mp3`, ya asignado).
- Nivel 2 Alhóndiga: 2 tracks (calma + versión boss para Pípila, con
  crossfade cuando aparece el boss).
- Nivel 3 Callejones: 1 track.
- Victoria: jingle breve ~10s.
- Game Over: jingle breve ~5s.
- Opcional: aceleración sutil de 5-10% del pitch en "momentos intensos"
  (última oleada, HP bajo) para dar sensación de urgencia.

**Stats de enemigos (overridean los valores actuales de los .asset):**

| Enemigo | vida | daño | escala | velocidad |
|---|---|---|---|---|
| Momia Caminante | 2 | 1 | 1.3 | 1.5 |
| Momia Bebé | 1 | 1 | 0.6 | 2.0 |
| Boss Pípila | 15 | 3 | 1.0 | 1.2 |

**Mecánica Momia Bebé:** solo recibe daño si Diego ataca estando
AGACHADO (el origen del campanazo tiene que venir desde Y bajo).
Si Diego la ataca parado, el daño es ignorado.

**HeartPickup:** 20% de probabilidad de spawnearse cuando muere una
momia. Al tocarlo, Diego recupera 2 unidades (1 corazón completo). Si
Diego está en vida máxima, el corazón NO se recoge (queda en el suelo).

**ShockwaveVFX (onda sónica del campanazo):** sprite animado pixel art
(4 frames, anillo dorado expandiéndose), generado programáticamente por
Python. Antonio lo reemplazará con una versión mejor en Avance 4.

**Barra de vida del Boss:** estilo Hollow Knight (horizontal, abajo
centrada, con nombre "MOMIA-PÍPILA" encima). Fade-in al aparecer el
boss. Al recibir daño, barra "blanca" que se queda un momento para
mostrar daño hecho (game feel).

**Juice pack:** screen shake al recibir daño y matar enemigos,
hit-pause (freeze-frame breve 50ms) al matar, dust burst al aterrizar
saltos y morir enemigos, botones con hover effect (scale 1.1 + sound).

**Momia Corredora + Momia Escupidora + La Momia Madre:**
PENDIENTES de Antonio (le pasé lista). NO los implementar hasta que
él entregue los sprites y animaciones.

══════════════════════════════════════════════════════════
🤝 CÓMO QUIERO TRABAJAR (MUY IMPORTANTE)
══════════════════════════════════════════════════════════

**Máxima automatización.** Mi carpeta del proyecto está conectada
(device_bash + mnt/Momias) así que puedes escribir archivos directo:

1. **SCRIPTS DE GAMEPLAY (C#):** los escribes DIRECTO en
   `Assets/Scripts/` mediante `device_bash`. NO me pases código en el
   chat para que yo lo pegue — escríbelo directo en el proyecto.

2. **SETUP DE UNITY** (crear GameObjects, Canvas, prefabs, asignar
   refs, botones, etc.): creas un EDITOR SCRIPT en `Assets/Editor/`
   con atributos `[MenuItem]` que me generen un menú "Momias/Fase X/..."
   en la barra superior de Unity. Yo solo le doy clic al menú y queda
   todo listo. Ejemplo del patrón: `Assets/Editor/Chunk1Setup.cs`.

3. **Lo que SÍ queda manual** (porque es destructivo, único, o Unity
   no permite scripting fácil):
   - Renombrar escenas
   - Build Settings
   - Importar paquetes
   - Mover carpetas de Project
   - Asignar Animator Controllers específicos
   Para esto me das pasos breves en el chat o en un .md.

4. **Flujo por chunk/fase:**
   a. Me propones un plan (scripts nuevos, qué hace cada uno,
      qué editor script, qué me toca manual)
   b. YO CONFIRMO primero
   c. Tú ejecutas (escribes scripts + editor script directo en el
      proyecto via device_bash)
   d. Me dices qué menú clickar en Unity + los pasos manuales
   e. Yo pruebo, te confirmo "listo fase X" o te paso el error
   f. Siguiente fase

5. **NO ESCRIBAS CÓDIGO hasta que yo confirme el plan de cada fase.**

6. **Si truena algo** te paso el screenshot o el mensaje de la Console
   y lo resolvemos antes de seguir.

══════════════════════════════════════════════════════════
📅 TIEMPO
══════════════════════════════════════════════════════════

Hoy miércoles 11 nov 2026. Próxima entrega oficial: **Avance 3 ya debió
ser el viernes 6 nov** (estamos atrasados) pero el profe suele dar
tolerancia. **Avance 4 es el viernes 13 nov** (pasado mañana). El
objetivo real: tener Niveles 1, 2 y 3 pulidos para el Avance 3
(atrasado) y entrar al Avance 4 con Nivel 4 + narrativa.

**Prioridad:** calidad sobre velocidad. Quiero que el juego se sienta
"juego de paga", no proyecto estudiantil genérico.

══════════════════════════════════════════════════════════
🎯 PLAN COMPLETO DE 5 FASES
══════════════════════════════════════════════════════════

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🟥 FASE A — Pulir Nivel 1 (~45 min) — PRIORIDAD MÁXIMA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Esto es "lo mínimo que debe funcionar" en Nivel 1.

**Scripts a crear:**

1. `Assets/Scripts/Enemies/MomiaBebe.cs`:
   - Hereda de `MomiaCaminante`.
   - Override `RecibirDanio(int cantidad, Vector2 origen)`:
     ```
     Si (origen.y - transform.position.y) > umbral: ignorar el daño
     (porque el campanazo viene de arriba = Diego parado).
     Si <= umbral: llamar base.RecibirDanio() (Diego agachado).
     ```
   - Añadir Debug.Log cuando el ataque es ignorado para feedback en
     testing.
   - `umbralAlturaDanio` default 0.1f, exposed en inspector.

2. `Assets/Scripts/Items/HeartPickup.cs`:
   - RequireComponent Collider2D (trigger).
   - Bob visual (Sin wave en Y).
   - OnTriggerEnter2D: si other es Player con PlayerHealth,
     y PlayerHealth.VidaActual < VidaMaxima, llamar
     `playerHealth.Curar(2)` + sonido + destroy.
   - Si vida llena, NO recoge (queda en el suelo).
   - Método estático `TrySpawnOnEnemyDeath(prefab, pos, prob=0.20f)`
     que spawnea con probabilidad.

3. `Assets/Scripts/UI/ShockwaveVFX.cs`:
   - RequireComponent SpriteRenderer.
   - En Awake, guarda color inicial.
   - En Update: escala de 0.4 a 2.5 durante 0.3s con AnimationCurve.
   - Alpha de inicial a 0 durante el mismo tiempo.
   - Auto-destroy al acabar.
   - Método estático `Spawn(sprite, pos, color?, sortingOrder=15)`.

**Modificaciones a scripts existentes:**

4. `Assets/Scripts/Enemies/EnemyBase.cs`:
   - Agregar campo `[SerializeField] private GameObject heartPickupPrefab`
     y `[Range(0,1)] private float heartDropChance = 0.20f`.
   - En `Morir()`, después de marcar estaMuerto, llamar
     `HeartPickup.TrySpawnOnEnemyDeath(heartPickupPrefab, transform.position, heartDropChance)`.

5. `Assets/Scripts/Player/PlayerAttack.cs`:
   - Agregar campo `[SerializeField] private Sprite shockwaveSprite`.
   - En `Atacar()`, después de instanciar el hitbox, llamar
     `ShockwaveVFX.Spawn(shockwaveSprite, pos, Color amarillo claro)`.

**Assets a generar (Python):**

6. `Assets/Sprites/VFX/Shockwave_Sheet.png`:
   - Sprite sheet 256x64 (4 frames 64x64), anillo dorado expandiéndose.
   - Pivots center.
   - Pixel art consistente con el estilo del juego.

**Editor script a crear:**

7. `Assets/Editor/Chunk2Setup.cs` (reusa el nombre, es "setup del
    siguiente chunk"):
   - Menú "Momias/Fase A/..." con items:
     - 🚀 Build ALL Fase A (ejecuta todos los steps abajo)
     - Fix MomiaBebe prefab (cambia script MomiaCaminante → MomiaBebe,
       mantiene Animator ya asignado, reasigna data al asset)
     - Fix stats en ScriptableObjects (actualiza MomiaBebe.asset y
       MomiaPipila.asset con los valores correctos de la tabla arriba)
     - Build HeartPickup prefab (crea Assets/Prefabs/Items/HeartPickup.prefab
       con SpriteRenderer + CircleCollider2D trigger + HeartPickup.cs)
     - Reconfigurar oleadas Nivel 1 (modifica el WaveManager de la
       escena actual para que las 3 oleadas sean:
         Oleada 1: 3 Caminantes
         Oleada 2: 4 Caminantes + 1 Bebé (dos oleadas serializadas
                   con pausa 0 entre ellas, O una sola oleada con
                   array mixto — ver cómo lo maneja WaveManager actual)
         Oleada 3: 5 Caminantes + 3 Bebés)
     - Assign heartPickup prefab a todos los EnemyBase de la escena
     - Assign shockwave sprite al PlayerAttack de Diego

**Checklist de verificación Fase A:**
- [ ] Al entrar en Play, Diego ataca con J → se ve una onda dorada
      expandiéndose en el punto del hitbox.
- [ ] Las momias caminantes sueltan ~1 corazón por cada 5 que mates
      aproximadamente.
- [ ] El corazón hace bob, al tocarlo Diego recupera 1 corazón (si no
      está lleno).
- [ ] En la oleada 2 aparecen bebés mezclados con caminantes.
- [ ] Las bebés parado son invencibles (Debug.Log "ignorado"), agachado
      sí les haces daño.
- [ ] Al matar todas las oleadas aparece la Campana Sagrada → Victory.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🟧 FASE B — Menú Principal funcional + bello (~30 min)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

El Menu.unity está vacío (pantalla azul). Hay que armarlo de 0 bonito.

**Scripts a crear:**

1. `Assets/Scripts/UI/MainMenu.cs`:
   - Referencias a mainPanel, creditsPanel, optionsPanel.
   - Métodos públicos: MostrarPrincipal, MostrarCreditos, MostrarOpciones,
     OnJugar (carga Nivel1 via SceneTransition), OnSalir.
   - En Start: autoPlayMenuMusic desde AudioManager.

2. `Assets/Scripts/UI/CreditsPanel.cs`:
   - ScrollingContent (RectTransform) con velocidad de scroll vertical.
   - Botón Volver → MostrarPrincipal del MainMenu.
   - Reset de posición Y al OnEnable.

3. `Assets/Scripts/UI/OptionsPanel.cs`:
   - Placeholder "Próximamente en Avance 4".
   - Botón Volver.

4. `Assets/Scripts/UI/MenuBackground.cs`:
   - Animación Ken Burns (zoom lento 1.0 → 1.08, PingPong) + paneo
     horizontal con Sin wave.

**Assets a generar:**

5. Fondo del menú:
   - Opción A: generar procedural (museo nocturno con luna, arcos,
     antorchas, partículas de polvo flotando). 1920x1080.
   - Opción B: reusar `Nivel2_Capa2_Alhondiga.png` escalado con vignette
     oscuro encima.
   - Decisión: Opción A (más original).

**Editor script:**

6. Agregar al `Chunk2Setup.cs` menú "Momias/Fase B/..." con:
   - 🚀 Build ALL Fase B
   - Setup Managers en Menu.unity (GameManager, AudioManager,
     SceneTransition con Fader, EventSystem)
   - Build MainMenu Canvas (fondo + título "MOMIAS DEL GRITO" +
     subtítulo "Un guía, cuatro campanas, una noche eterna" + 4 botones)
   - Build CreditsPanel (texto scrolleable con los créditos)
   - Build OptionsPanel (placeholder)
   - Hookup audio clips del Menu

**Créditos (contenido):**
```
MOMIAS DEL GRITO

Un juego de
Cristian E. Oropeza  &  José A. Balderas

PROGRAMACIÓN Y SISTEMAS
Cristian Eduardo Oropeza

ARTE, ANIMACIÓN, AUDIO Y LEVEL DESIGN
José Antonio Balderas Melchor

MOTOR
Unity 6.6 — URP 2D

ASISTENCIA TÉCNICA
Claude (Anthropic)

PROYECTO ACADÉMICO
Creación de Videojuegos
Universidad de Guanajuato — 2026

Gracias por jugar 🔔
```

**Checklist de verificación Fase B:**
- [ ] Al abrir el juego desde Build arranca en Menu.unity.
- [ ] Música de menú suena (o silencio si Antonio no la ha entregado).
- [ ] Fondo animado tipo Ken Burns se mueve lento.
- [ ] Clic en JUGAR → fade a negro → carga Nivel 1 → fade in.
- [ ] Clic en CRÉDITOS → scroll automático de texto → Volver cierra.
- [ ] Clic en OPCIONES → placeholder → Volver cierra.
- [ ] Clic en SALIR → cierra el juego (en Editor cierra Play mode).
- [ ] Desde pausa en Nivel 1 → Menú Principal → regresa al menú con fade.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🟨 FASE C — Juice / Game Feel (~30 min)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Esto es lo que diferencia un proyecto estudiantil de un juego indie
profesional. Técnicas estándar de Celeste, Katana Zero, Hollow Knight.

**Scripts a crear:**

1. `Assets/Scripts/Managers/CameraShaker.cs`:
   - Singleton con DontDestroyOnLoad.
   - Método público `Shake(float magnitude, float duration)`.
   - Mueve la cámara con Random.insideUnitCircle multiplied por
     magnitude durante duration.
   - Restaura posición original al terminar.

2. `Assets/Scripts/Managers/HitPause.cs`:
   - Singleton con DontDestroyOnLoad.
   - Método público `Freeze(float duration = 0.05f)`.
   - Baja Time.timeScale a 0.05 por duration (usa unscaledDeltaTime
     para medir).
   - Restaura a 1 al terminar.

3. `Assets/Scripts/UI/DustBurst.cs`:
   - Spawnea 6-12 pequeños sprites de polvo con física (Rigidbody2D)
     que se dispersan radialmente y hacen fade out.
   - Método estático `Spawn(Vector3 pos, Color tint)`.

**Modificaciones:**

4. `PlayerHealth.cs` → en `RecibirDanio`, llamar
   `CameraShaker.Instance.Shake(0.3f, 0.2f)` + `HitPause.Instance.Freeze(0.03f)`.

5. `EnemyBase.cs` → en `Morir()`, llamar
   `CameraShaker.Instance.Shake(0.15f, 0.1f)` + `HitPause.Instance.Freeze(0.05f)` +
   `DustBurst.Spawn(transform.position, Color(0.6,0.5,0.4,1))`.

6. `PlayerController.cs` → al detectar aterrizaje de salto (IsGrounded
   cambia de false a true), llamar `DustBurst.Spawn` en los pies con
   color polvo claro.

**Pulido botones UI:**

7. Script `ButtonHover.cs` → componente que agrega hover effect:
   escala 1.0 → 1.1 al entrar mouse, vuelve a 1.0 al salir, sonido
   tick al entrar.

**Editor script:**

8. Agregar al setup "Momias/Fase C/..." con items:
   - Setup CameraShaker en escena activa
   - Setup HitPause en escena activa
   - Add ButtonHover a todos los botones del Menu y de los paneles de
     pausa/gameover/victoria.

**Checklist de verificación Fase C:**
- [ ] Al recibir daño, cámara tiembla 0.2s.
- [ ] Al matar momia, mini freeze-frame + mini shake + polvo volando.
- [ ] Al aterrizar de un salto, polvo levanta de los pies.
- [ ] Botones del menú crecen 10% al pasar el mouse + suena tick.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🟩 FASE D — Nivel 2 Alhóndiga con Boss Pípila (~60 min)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

**Scripts a crear:**

1. `Assets/Scripts/Enemies/BossPipila.cs`:
   - Hereda de EnemyBase.
   - Estados: Idle, Walking, Embestida, Jumping, Falling, Hurt, Dead.
   - **Fase 1** (vida 100%-50%): Embestidas horizontales rápidas de un
     lado al otro de la pantalla. Diego debe saltarlas.
   - **Fase 2** (vida 50%-0%): Salto alto + caída con onda expansiva
     (daña si Diego está cerca del punto de impacto).
   - Transición de fase: anim de Hurt + pequeño freeze.
   - Al morir: anim Die + pausa + fade-out + spawn de Campana Sagrada.
   - Daño por golpe: 3 unidades.
   - Vida: 15.

2. `Assets/Scripts/UI/BossHealthBar.cs`:
   - Panel UI con nombre del boss arriba + barra horizontal abajo.
   - Suscripción a BossPipila.OnHealthChanged.
   - Barra "blanca" delayed que se queda un momento para mostrar el
     daño justo hecho (lerp).
   - Entrada con fade-in + slide desde abajo.

3. `Assets/Scripts/Managers/ParallaxLayer.cs`:
   - Componente que se asigna a cada capa del background.
   - En Update, mueve la Transform con offset proporcional a la cámara
     (Camera.main.transform.position.x * factor).
   - factor 0.1 para fondo lejos, 0.5 para medio, 1.0 para cerca.

**Modificaciones:**

4. `EnemyBase.cs` → agregar virtual method `OnDamaged()` para que
   derivados (como BossPipila) puedan reaccionar y cambiar fase.

**Montaje escena Nivel2_Alhondiga.unity:**

5. Editor script "Momias/Fase D/..." con:
   - 🚀 Build ALL Nivel 2
   - Add parallax to layers (Nivel2_Capa1_Fondo factor 0.1,
     Capa2_Alhondiga factor 0.5, Capa3_Suelo factor 1.0)
   - Spawn Diego (copiar prefab/config de Nivel 1)
   - Spawn Managers (GameManager, AudioManager, SceneTransition — ya
     deben existir porque son DontDestroyOnLoad desde Menu)
   - Spawn GameUI_Canvas con HeartsHUD, PauseMenu, GameOverPanel,
     VictoryCinematic, VictoryPanel, LevelTitleCard (título "NIVEL 2
     - ALHÓNDIGA DE GRANADITAS")
   - Spawn WaveManager con 4 oleadas de Caminante + Bebé (sin Corredora
     todavía por falta de sprite de Antonio)
   - Spawn SacredBell oculta (aparece después de matar al Boss)
   - Spawn BossPipila oculto (aparece después de completar las 4
     oleadas, con fade-in dramático)
   - Spawn BossHealthBar oculto
   - Hook WaveManager.OnAllWavesCompleted → spawn Boss Pípila
   - Hook BossPipila.OnDeath → spawn SacredBell

**Fix prefab MomiaPipila:**

6. Editor script: AddComponent BossPipila al MomiaPipila.prefab,
   asignar `MomiaPipila.asset` como data, verificar que el Animator
   ya tenga el `MomiaPipila.controller` asignado.

**Checklist de verificación Fase D:**
- [ ] Al ganar Nivel 1, botón "Siguiente Nivel" carga Nivel 2 con fade.
- [ ] Nivel 2 arranca con cartel "NIVEL 2 - ALHÓNDIGA DE GRANADITAS".
- [ ] Parallax se nota al moverte horizontal.
- [ ] 4 oleadas de Caminantes + Bebés funcionan como en Nivel 1.
- [ ] Al terminar oleadas, música cambia (crossfade a versión boss),
      aparece Boss Pípila con animación de entrada.
- [ ] Boss Health Bar aparece arriba abajo con "MOMIA-PÍPILA" y barra.
- [ ] Fase 1 del boss: embestidas horizontales, Diego las salta.
- [ ] A vida 50%, boss hace anim Hurt + cambia a Fase 2.
- [ ] Fase 2: salto + caída con onda, Diego debe estar lejos del impacto.
- [ ] Al morir Pípila, aparece Campana Sagrada → Victory cinemática.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🟦 FASE E — Nivel 3 Callejones (depende de assets de Antonio)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Solo arrancar cuando Antonio entregue:
- Sprites de Momia Corredora (roja, rápida)
- Sprites de Momia Escupidora (verde, lanza proyectil)
- Sprite del proyectil (3-4 frames)
- Música del Nivel 3

**Scripts a crear cuando lleguen assets:**

1. `MomiaCorredora.cs`: hereda de MomiaCaminante, solo cambia valores
   (vel doble, daño 2).

2. `MomiaEscupidora.cs`: no camina hacia Diego, se queda en su spawn,
   dispara proyectil parabólico cada 2-3s.

3. `EnemyProjectile.cs`: proyectil con física parabólica, daña a Diego
   en collision, se destruye al tocar suelo.

**Montaje Nivel3_Callejones.unity:**

4. Editor script "Momias/Fase E/..." con build similar al Nivel 2 pero
   con las 5 oleadas que mezclan las 4 variantes de momia (ver tabla
   en Biblia).

5. Sin boss.

**Checklist Fase E:**
- [ ] Nivel 3 arranca con cartel "NIVEL 3 - CALLEJONES DE GUANAJUATO".
- [ ] Corredora es visiblemente más rápida y roja.
- [ ] Escupidora lanza proyectiles que Diego debe esquivar.
- [ ] 5 oleadas terminan con Campana Sagrada.
- [ ] Al ganar, carga Menú (no hay Nivel 4 aún).

══════════════════════════════════════════════════════════
🚀 LO QUE QUIERO QUE HAGAS AHORA
══════════════════════════════════════════════════════════

1. Lee `JUEGO_Vision_Completa.md` para que tengas la visión completa.
2. Lee `ESTADO_Y_ROADMAP.md` para que sepas en qué estamos.
3. Revisa los scripts actuales en `Assets/Scripts/` (sobre todo los
   Managers y EnemyBase + MomiaCaminante + PlayerAttack porque vas a
   modificarlos en Fase A).
4. Revisa `Assets/Editor/Chunk1Setup.cs` para que entiendas el patrón
   de automatización con editor scripts.
5. **Dime si tienes dudas en algo antes de arrancar.**
6. Si todo claro, propóneme el plan **específico y detallado de la
   FASE A** (los scripts exactos que vas a escribir, los cambios
   exactos a los existentes, el editor script que vas a crear, y qué
   me toca a mí de clic manual en Unity).
7. Yo confirmo "va".
8. Entonces ejecutas: escribes TODO directo en el proyecto via
   device_bash, generas los assets necesarios (sprite shockwave) con
   Python, y me dejas el proyecto listo para probar.
9. Al final te paso screenshot o mensaje de Console y seguimos con
   Fase B.

**NO escribas código todavía. Primero plan, yo confirmo, después
ejecutas.**

Arranquemos.
