# Handoff para la próxima conversación con Claude

**Proyecto:** Momias del Grito (arena-brawler 2D pixel art, Unity 6.6 URP)
**Para:** Claude (nueva conversación, mismo proyecto)
**De:** Claude (sesión anterior) — con Cristian E. Oropeza
**Fecha:** 23 nov 2026
**Entrega final:** 27 nov 2026 (quedan 4 días hábiles)

---

## 0. Lee ESTO primero

Hola Claude. Esta conversación continúa un proyecto grande con Cristian. Para no reaprender todo:

1. Lee este archivo completo.
2. Luego lee `BRIEF_NUEVO_CHAT.md` (el brief original del proyecto) y `ESTADO_Y_ROADMAP.md` para contexto profundo.
3. `JUEGO_Vision_Completa.md` es la "Biblia" del juego, pero **algunas decisiones cambiaron** (ver sección 5 de este doc, "Decisiones tomadas").
4. **No preguntes cosas que este doc ya responde.** Cristian valora la velocidad.

---

## 1. Qué es Momias del Grito

Arena-brawler 2D pixel art para la materia de Creación de Videojuegos.

**Pitch:** Diego, guía turístico del Museo de las Momias de Guanajuato, defiende el centro de la pantalla con una campana sónica mientras momias de 4 variantes atacan por ambos flancos, a través de 4 niveles icónicos de Guanajuato, culminando con la Momia Madre en la Parroquia de Dolores Hidalgo.

**Equipo:**
- **Cristian E. Oropeza** (con quien hablas) — programación, sistemas, integración.
- **José Antonio Balderas Melchor** (Antonio) — arte, animación, audio, level design. No está en el chat; Cristian es el intermediario.

**Repo:** https://github.com/AntonioBM28/Momias (no asumas que puedes hacer commits — Cristian los hace manual siempre).

---

## 2. Stack técnico y convenciones

- **Unity 6.6 (6000.0.f1)** con URP 2D.
- **C#** — nada de Visual Scripting.
- **Input System VIEJO** — `Input.GetKey`, `Input.GetAxisRaw`, `Input.GetKeyDown`. **NO uses el New Input System.**
- **TextMeshPro** disponible.
- **Git + Git LFS** configurados.

**Nomenclatura:**
- PascalCase para scripts (`PlayerController.cs`).
- Comentarios y docstrings en **español**, breves.
- `/// <summary>` arriba de cada clase explicando responsabilidad.
- `[Header("...")]` en inspector para claridad.
- Desacople entre sistemas vía `UnityEvent` (no `Action` directo).

**Estructura de carpetas:**
```
Assets/Scripts/{Data, Enemies, Items, Managers, Player, UI}
Assets/Prefabs/ (y subcarpetas Items, Player)
Assets/ScriptableObjects/Enemies/
Assets/Editor/ (editor scripts con menús [MenuItem])
Assets/Animations/
Assets/Enemies/[NombreMomia]/ (sprites, uno por animación)
Assets/Sprites/{Backgrounds, Diego, Items, UI, VFX}
Assets/Audio/{Music, SFX}
Assets/Scenes/
```

**Tags:** `Player`, `Enemy`, `Pickup`, `SacredBell`.

**Layers:** 6 Ground, 7 Boundary, 8 Player, 9 Enemy.

**Collision Matrix:**
- Enemy ↔ Enemy: OFF
- Player ↔ Enemy: OFF
- Enemy ↔ Boundary: OFF

---

## 3. CÓMO TRABAJAMOS (importantísimo, leer con atención)

Cristian tiene un flujo específico y **no le gusta que te desvíes de él**:

### 3.1 Flujo por tarea

1. **Siempre propones un plan corto en lenguaje natural ANTES de hacer algo.** Si tienes dudas de alcance, preguntas 1–3 cosas concretas con tu voto/recomendación.
2. Cristian confirma "dale" / "va" / "simón".
3. Tú ejecutas TODO: escribes scripts directo en el proyecto via `device_bash`, generas sprites con Python, creas editor scripts, etc. **NO le pegas código para que lo copie.** El código se escribe directo en `C:\Users\orope\OneDrive\Escritorio\Momias\Assets\...`
4. Le dices en lenguaje natural qué hiciste + cómo probar + resultado esperado.
5. Cristian prueba, te confirma o te manda screenshot con el error.
6. Siguiente tarea.

### 3.2 Reglas estrictas

- **NUNCA hagas `git commit`**, `git add` ni nada de git. Cristian lo hace manual siempre.
- **NO me digas "ejecuta este comando en PowerShell"** a menos que sea estrictamente necesario (crear archivos, instalar algo que no puede hacerse desde Unity).
- **Todo lo que puedas automatizar, hazlo con editor scripts** (`[MenuItem("Momias/Fase X/...")]`). Cristian entra a Unity y le da clic al menú.
- **NO preguntes cosas obvias ni vuelvas a preguntar lo que ya está respondido en este doc o en los .md anteriores.**
- Cuando el user haga una pregunta tipo "ok?" o "dale?", eso ES una confirmación, no pidas que "te confirme".
- Si una modificación falla a mitad de camino (ej. compile error), **fíjalo inmediatamente** sin preguntar.
- Cristian valora mensajes **cortos y directos**. Nada de preludios ni "¡Vamos!" antes de cada bloque. Al grano.

### 3.3 Estilo de respuesta esperado

**Mal:**
> "¡Excelente pregunta! Déjame pensar en las opciones que tenemos... [100 líneas de plan y explicaciones]"

**Bien:**
> "Fix. [tool call]. Hecho: X, Y, Z. Probá con Menú → Momias/Fase X → Build ALL."

### 3.4 Al arrancar cada fase

- Lee el brief, propón plan corto en lenguaje natural.
- Pregunta UNA o DOS dudas máximo, con tu voto/recomendación. Default tus votos si no responde.
- Cristian confirma, ejecutas, le dices cómo probar.

---

## 4. Estado actual del proyecto (23 nov 2026)

### 4.1 Escenas

| Escena | Estado |
|---|---|
| `Menu.unity` | ✅ Completo (fondo procedural, 4 botones, créditos, opciones con sliders). |
| `Nivel1_Museo.unity` | ✅ Pulido (oleadas con Bebés, HeartPickup, Shockwave, juice, audio). |
| `Nivel2_Alhondiga.unity` | ✅ Armado (Pípila + 4 oleadas Caminante+Bebé+Corredora). **Falta probar end-to-end.** |
| `Nivel3_Callejones.unity` | 🟡 Block-out de Antonio (solo backgrounds). Falta armar gameplay. |
| `Nivel4_Parroquia.unity` | 🟡 Block-out de Antonio. Depende de Momia Madre (no entregada). |

### 4.2 Enemigos (status completo)

| Enemigo | Sprites | Animator | Prefab | SO | Script C# | Integrado |
|---|---|---|---|---|---|---|
| Momia Caminante | ✅ | ✅ | ✅ `MomiaWalking_0.prefab` | ✅ `MomiaCaminante_Gris.asset` | ✅ `MomiaCaminante.cs` | ✅ |
| Momia Bebé | ✅ | ✅ | ✅ `MomiaBebe.prefab` | ✅ `MomiaBebe.asset` | ✅ `MomiaBebe.cs` (hereda Caminante, override `RecibirDanio` requiere crouch, override `FeedbackMuerte` sin shake) | ✅ |
| Momia Corredora | ✅ (PixelLab, 31 frames en 4 carpetas) | ✅ `MomiaCorredora.controller` (Run/Attack/Die + triggers Corriendo/Atacar/Morir) | ✅ `MomiaCorredora.prefab` | ✅ `MomiaCorredora.asset` (vida=2, vel=3.5, daño=2, escala=1.3) | ✅ `MomiaCorredora.cs` | ✅ |
| Momia Escupidora | ✅ (Antonio, 5 PNGs: Idle/Walk/Attack/Die/Hurt) | ✅ `MomiaEscupidora.controller` | ✅ `MomiaEscupidora.prefab` (SIN script propio todavía) | ✅ `MomiaEscupidora.asset` | ❌ FALTA | ❌ FALTA |
| Boss Pípila | ✅ | ✅ `MomiaPipila.controller` | ✅ `MomiaPipila.prefab` | ✅ `MomiaPipila.asset` (vida=15, daño=3) | ✅ `BossPipila.cs` (2 fases: embestidas + salto+onda) | ✅ |
| Boss Momia Madre | ❌ FALTA | ❌ | ❌ | ❌ | ❌ | ❌ |

### 4.3 Scripts actuales

**Managers (`Assets/Scripts/Managers/`):**
- `AudioManager.cs` (singleton + DontDestroyOnLoad, con slots `musicMenu`, `musicNivel1-3`, `musicBossPipila`, `musicVictoria`, `musicGameOver`, `sfxCampanazo`, `sfxImpactoMomia`, `sfxMuerteMomia`, `sfxSalto`, `sfxPickupCorazon`, `sfxCampanaSagrada`, `sfxUIClick`. Getters `MusicVolume`/`SFXVolume`).
- `GameManager.cs` (singleton, estados Menu/Playing/Paused/GameOver/Victory, `GoToFirstLevel`, `GoToNextLevel`, `GetNextLevelSceneName` con switch de niveles, `ReproducirMusicaDelNivel` con switch).
- `SceneTransition.cs` (fades negros entre escenas con Fader).
- `WaveManager.cs` (lista de oleadas serializables con prefab, data, cantidad, intervalo, pausa).
- `CameraShaker.cs` (singleton, `Shake(mag, dur)`).
- `HitPause.cs` (singleton, `Freeze(dur)`. **OJO: bug conocido ya arreglado** — SIEMPRE restaura a `Time.timeScale = 1f` al terminar, nunca a un valor guardado, para evitar slow-mo permanente si se dispara durante otro freeze).
- `ParallaxLayer.cs` (factor 0-1 por capa; actualmente NO se usa en Nivel 2, cámara es estática).
- `CameraFollow.cs` (sigue a Diego en X con zona activa clamped; actualmente NO se usa en Nivel 2).

**Enemies (`Assets/Scripts/Enemies/`):**
- `EnemyBase.cs` (abstract, vida/daño/muerte compartidos. Tiene `heartPickupPrefab` + `heartDropChance = 0.20f`. `Morir()` virtual llama a `HeartPickup.TrySpawnOnEnemyDeath`, dispara anim trigger "Morir", y arranca `RutinaMorir` que: cae 0.4s, se vuelve Kinematic, hace blink alpha 0.7s, destruye. `FeedbackMuerte()` virtual sobreescribible: default shake 0.08 + hitpause 0.06 + DustBurst + PlayMuerteMomia).
- `MomiaCaminante.cs` (IA caminar + melee con rango).
- `MomiaBebe.cs` (hereda Caminante, override `RecibirDanio` ignora si Diego no agachado, override `FeedbackMuerte` sin shake).
- `MomiaCorredora.cs` (hereda Caminante, arranca con `anim.SetBool("Corriendo", true)`).
- `BossPipila.cs` (hereda EnemyBase, 2 fases, eventos `OnBossSpawned`/`OnBossDied`/`OnBossHealthChanged`/`OnBossFaseChanged`, override `FeedbackMuerte` con shake grande 0.4/0.5).
- ❌ Falta `MomiaEscupidora.cs`.

**Player (`Assets/Scripts/Player/`):**
- `PlayerController.cs` (mover WASD+flechas, salto W/Espacio/↑, agacharse S/↓, propiedades `IsGrounded`/`IsCrouching`/`IsFacingRight`/`PuedeActuar`. En `FixedUpdate` detecta aterrizaje y spawna `DustBurst`).
- `PlayerAttack.cs` (campanazo con J o click izquierdo, hitbox, shockwave con `shockwaveFrames[]`).
- `PlayerHealth.cs` (vida por medios corazones, `Curar(int)`, `VidaActual`, `VidaMaxima`, `EstaMuerto`, `EsInvulnerable`, eventos `OnHealthChanged`/`OnDamaged`/`OnDeath`. Al recibir daño: shake 0.12/0.15 + hitpause 0.03).
- `AttackHitbox.cs` (zona de daño temporal).

**Items (`Assets/Scripts/Items/`):**
- `SacredBell.cs` (recoger con E, dispara Victory).
- `HeartPickup.cs` (bob + rotación pingpong, cura 2 al tocar, sortingOrder=5, prefab escala 0.6 para tamaño chico, método estático `TrySpawnOnEnemyDeath(prefab, pos, prob)`).

**UI (`Assets/Scripts/UI/`):**
- `Fader.cs`, `HeartsHUD.cs`, `LevelTitleCard.cs` (CUIDADO: tiene campos `tituloNivel` y `subtituloNivel` que se re-aplican al TMP en Start; para cambiarlos usa SerializedObject en los campos, no solo tmp.text).
- `MainMenu.cs`, `CreditsPanel.cs`, `OptionsPanel.cs`, `MenuBackground.cs`, `ButtonHover.cs`.
- `PauseMenu.cs` (refactor con sub-paneles mainPanel/optionsPanel/creditsPanel).
- `GameOverPanel.cs`, `VictoryPanel.cs` (jingles como SFX one-shot, NO loop. Botón "Siguiente Nivel" se habilita dinámicamente según `GameManager.GetNextLevelSceneName()`).
- `VictoryCinematic.cs` (flash+shake+slowmo+letterbox+zoom+onda dorada+fade dorado).
- `ShockwaveVFX.cs` (sprite-swap de 4 frames con lerp escala+alpha, método estático `Spawn(sprites, pos, color, sortingOrder)`).
- `DustBurst.cs` (singleton con sprite serializable, método estático `Spawn(pos, tinte)` que genera 6-10 GameObjects con Rigidbody2D + fade).
- `BossHealthBar.cs` (estilo Hollow Knight, barra dorada + white delayed, `Inicializar(nombre, vidaMax)`, `MostrarConFade()`, `OcultarConFade()`, `SetVida(int)`).

### 4.4 Editor scripts (menús)

Todos los editor scripts están en `Assets/Editor/`. Cada uno agrega menús a la barra superior de Unity bajo `Momias/...`:

- `Chunk1Setup.cs` → `Momias/Chunk 1/...` (GameOverPanel, VictorySystem, LevelTitleCard — es el patrón base, replica su estilo).
- `FaseA_Setup.cs` → `Momias/Fase A/...` (stats SOs, fix MomiaBebe prefab, HeartPickup prefab, shockwave sprites, oleadas Nivel 1).
- `FaseB_Setup.cs` → `Momias/Fase B/...` (Menú Principal completo con fondo procedural + Managers persistentes + cablea los 11 audio clips al AudioManager de Menu).
- `FaseC_Setup.cs` → `Momias/Fase C/...` (CameraShaker/HitPause/DustBurst singletons + ButtonHover a todos los botones).
- `FaseD_Corredora.cs` → `Momias/Fase D/...` (config sprites Corredora pivot Y=0.15, .anim clips, Animator Controller, SO, prefab).
- `FaseD_Nivel2.cs` → `Momias/Fase D/...` (arma Nivel 2 completo: cámara estática, Managers, GameUI, WaveManager, Boss Pípila oculto, BossHealthBar, BossBridge, PauseMenu inline, auto-cablea audios).
- `Audios_Setup.cs` → `Momias/Audios/...` (asigna 7 clips nuevos de Antonio al AudioManager de Nivel 1 por GUID).

**Patrón para nuevos editor scripts:** mira `FaseD_Nivel2.cs` o `FaseB_Setup.cs` — usan helpers `CreateImageGO`, `CreateTMP`, `CreateMenuButton`, `SetStretchFull`, `SetAnchor`. Replica ese estilo.

### 4.5 Audios integrados

Todos están en `Assets/Audio/{Music,SFX}/` y cableados al AudioManager vía GUID en los editor scripts. La lista completa con GUIDs está en `AUDIOS_Para_Cristian.md` (Antonio nos lo pasó). Resumen:

- Música: Menu, Nivel 1, Nivel 2, Nivel 3, Boss Pípila (todos loopean).
- SFX: Campanazo Diego, Impacto Momia, Recoger Campana, Game Over, Jingle Victoria, Muerte Momia.

**Faltan (pendientes de Antonio):** Música Nivel 4, Música Boss Final, sfx_salto, sfx_pickup_corazon, sfx_ui_click, sfx_dano_diego.

### 4.6 Decisiones ya tomadas (NO volver a preguntar)

- **Nivel 2 = Alhóndiga con Boss Pípila** (contradice la Biblia que dice Callejones). Boss Pípila es homenaje al héroe de la Alhóndiga, por eso va ahí.
- **Nivel 3 = Callejones de Guanajuato** (sin boss, 5 oleadas con las 4 variantes).
- **Nivel 4 = Parroquia con Momia Madre** (boss final).
- **Cámara ESTÁTICA en todos los niveles** (como Nivel 1). NO sigue a Diego. NO parallax activo (los scripts existen pero no se usan).
- **Diego agachado:** NO tiene sprite propio. Se queda como está (solo encoge). Antonio NO va a entregarlo.
- **HeartPickup:** 20% de probabilidad al matar momia, cura 2 unidades, no se recoge si vida llena, prefab escala 0.6.
- **Menú de pausa:** mismo diseño dorado que el Menú Principal, 4 botones (Continuar, Opciones, Menú Principal, Salir). **Sin Créditos** en pausa.
- **AudioManager** se auto-cablea en cualquier escena donde se crea (via editor scripts) con los 11 clips (7 nuevos por GUID + 4 originales por nombre). Esto evita el bug de "AudioManager persistente vacío".
- **Shake de cámara:** 0.12/0.15 al recibir daño (suave), 0.08/0.08 al matar normal, 0.4/0.5 al matar boss. Bebés NO disparan shake (override FeedbackMuerte).
- **HitPause:** 60ms default. Jamás restaura a `timeScaleOriginal` (restaura a 1 o 0 si pausado).
- **Créditos:** NO incluyen "Asistencia técnica / Claude".
- **Play Mode Start Scene** configurado a Menu.unity (Menú → `Momias/Fase B/5. Set Play Mode -> Menu`).

---

## 5. Lo que FALTA (prioridad descendente)

### 5.1 PROGRAMACIÓN inmediata (se puede hacer ya)

**A. Momia Escupidora** (todo listo de Antonio, solo falta script):
1. Crear `Assets/Scripts/Enemies/MomiaEscupidora.cs` — hereda `EnemyBase`. No camina (override `Mover` vacío). Mira a Diego. Cada 2-3 segundos dispara proyectil parabólico hacia la posición predicha de Diego. Al recibir daño va a estado Hurt. Vida 2, daño 2 (según brief).
2. Crear `Assets/Scripts/Enemies/EnemyProjectile.cs` — Rigidbody2D con gravedad 1.5, velocidad inicial calculada parabólica. Daña a Player al tocarlo (OnTriggerEnter2D). Se destruye al tocar Ground o después de 3s.
3. Generar sprite placeholder del proyectil con Python (3-4 frames pixel art, verde/café, 12×12) hasta que Antonio entregue el real. Guardarlo en `Assets/Sprites/VFX/Escupitajo_0..3.png`.
4. Crear `Assets/Editor/FaseE_Escupidora.cs` — menú `Momias/Fase E/🚀 Integrar Escupidora`:
   - Agregar `MomiaEscupidora.cs` al prefab `MomiaEscupidora.prefab`.
   - Asignar `MomiaEscupidora.asset` como data.
   - Asignar heartPickupPrefab.
   - Config sprites del proyectil (pivot center, ppu=12).
   - Crear prefab `Assets/Prefabs/EnemyProjectile.prefab`.
   - Ajustar escala/stats del SO si no están bien.

**B. Nivel 3 Callejones** (listo para armar, ya hay las 4 momias):
1. Crear `Assets/Editor/FaseE_Nivel3.cs` — menú `Momias/Fase E/🚀 Build ALL Nivel 3`:
   - Patrón idéntico a `FaseD_Nivel2.cs` pero sin boss.
   - Cámara estática.
   - Diego (clonar del Nivel 1 via additive scene).
   - Managers + auto-cablear audios (misma función que en FaseD).
   - GameUI (GameOver/Victory/LevelTitleCard "NIVEL 3 / CALLEJONES DE GUANAJUATO").
   - PauseMenu inline (patrón de FaseD).
   - WaveManager con 5 oleadas (según tabla del brief):
     - Oleada 1: 4C + 1B + 1Cor + 1Esc = 7 enemigos
     - Oleada 2: 5C + 1B + 2Cor + 2Esc = 10
     - Oleada 3: 5C + 2B + 2Cor + 2Esc = 11
     - Oleada 4: 6C + 2B + 3Cor + 2Esc = 13
     - Oleada 5 final: 8C + 3B + 4Cor + 3Esc = 18
     - Cada oleada se divide en sub-oleadas por tipo con `pausaAntesDeOleada = 0.5f` entre sub-oleadas y 3f entre oleadas principales.
   - Al terminar oleadas → aparece Campana Sagrada (como Nivel 1, sin boss).
   - Agregar escena a Build Settings si no está.

**C. VictorySystem adaptado a Nivel 2 y 3:**
- El `Chunk1Setup.BuildVictorySystem` ya crea el panel con texto genérico "¡NIVEL COMPLETADO!" y fondo oscuro.
- Verificar que el `FaseD_Nivel2.SpawnGameUI` y `FaseE_Nivel3` llamen a `Chunk1Setup.BuildVictorySystem(silent: true)` y queden bien.

### 5.2 PROGRAMACIÓN diferida (depende de Antonio)

**D. Momia Madre (boss final):** espera a que Antonio suba sprites + Animator. Luego:
1. `MomiaMadre.cs` — 3 fases según brief/biblia:
   - Fase 1 (100-66%): lágrimas que caen del cielo.
   - Fase 2 (66-33%): torbellino de polvo que persigue.
   - Fase 3 (33-0%): invocación de mini-momias + salto con impacto.
2. Actualizar `BossHealthBar` para reusarse con cualquier boss (ya es genérica).
3. `FaseG_Nivel4.cs` — arma Nivel 4 Parroquia (patrón idéntico a FaseD_Nivel2 pero con Momia Madre + sin oleadas previas o con oleadas cortas de intro).

**E. Audios faltantes:** cuando Antonio suba Música Nivel 4, Boss Final y SFX cortos (salto, pickup corazón, UI click, daño Diego), extender `AsignarAudios` en editor scripts para que también los cablee por GUID.

### 5.3 TESTING y pulido final

- Probar flujo end-to-end: Menu → Nivel 1 → Nivel 2 → Nivel 3 → Nivel 4 → ganar.
- Probar muerte en cada nivel → GameOver → Reintentar.
- Probar pausa en cada nivel → Menú Principal → regresar.
- Build final de Windows (.exe) para entregar al profe.

---

## 6. Pendiente de Antonio (mandarle mensaje si no lo ha hecho)

**URGENTE para Nivel 3 (bloqueante):**
- Sprite del proyectil que dispara la Momia Escupidora (3-4 frames pixel art, color verdoso o café).

**URGENTE para Nivel 4 (bloqueante para cerrar juego):**
- Momia Madre completa: sprites (Idle, Walk, Attack, Die — idealmente 2-3 variantes de attack para 3 fases) + Animator + prefab + SO.
- Música Nivel 4 Parroquia (loop 2-3 min).
- Música Boss Final Momia Madre (loop 1-2 min, intenso).

**Si da tiempo (polish, no bloqueante):**
- SFX: sfx_salto, sfx_pickup_corazon, sfx_ui_click, sfx_dano_diego.

---

## 7. Gotchas y edge cases conocidos

- **HitPause bug (ya arreglado pero evita reintroducirlo):** NUNCA guardar `Time.timeScale` para restaurar. Siempre restaurar a 1f (o 0 si Pausa).
- **AudioManager vacío al cargar desde Menu:** si creas un AudioManager en una escena nueva, SIEMPRE cablea los 11 clips vía GUID (ver método `AsignarAudiosAlAudioManager` en `FaseD_Nivel2.cs` y `AsignarClipsAlAudioManager` en `FaseB_Setup.cs`). Reusa esa función en cada editor script nuevo.
- **LevelTitleCard:** para cambiar el texto, modifica `tituloNivel` y `subtituloNivel` via SerializedObject, NO solo el TMP (porque el Start() lo re-aplica).
- **Sprite de PixelLab tiene padding transparente abajo:** usa pivot Custom Y=0.15 (no BottomCenter) para que los pies queden al ras del piso. Ver `FaseD_Corredora.Step1_ConfigureSprites`.
- **`EnemyData.danio` es `float`** (no int). El sistema de daño acumula fracciones y hace FloorToInt — ver `MomiaCaminante.RutinaAtaque`.
- **MomiaPipila prefab:** el controller usa triggers `Caminando`, `Saltar`, `Morir`. Si Antonio renombra algo, los triggers del `BossPipila.cs` fallan silenciosos.
- **`device_bash` corre en la máquina de Cristian (Windows), no en Linux de Claude.** `python3` SÍ está disponible ahí. Para edits precisos en scripts C#, usa un bloque Python con `pathlib.Path` + `assert s.count(old) == 1` + `s.replace(old, new)`.
- **La carpeta del proyecto está en OneDrive:** `C:\Users\orope\OneDrive\Escritorio\Momias\`. Accesible desde `$HOME/mnt/Momias/` en device_bash. OneDrive puede tardar en sincronizar; si algo no aparece, esperá 10s.
- **No puedes borrar archivos desde device_bash** en la carpeta del proyecto (OneDrive bloquea delete). Si necesitas borrar algo, pídele a Cristian que lo borre desde Unity.
- **Permisos de `rm` en `$HOME/mnt/Momias/`:** fallan con "Operation not permitted". Siempre que generes algo que puede existir ya (ScriptableObjects via `AssetDatabase.CreateAsset`), usa primero `AssetDatabase.DeleteAsset(path)` desde el editor script, NO `rm` desde shell.
- **Momia Escupidora prefab lo subió Antonio SIN script.** Al integrar, usa el patrón de `FaseA_Setup.Step3_FixMomiaBebePrefabScript` para agregar el componente al prefab.

---

## 8. Cómo arrancar la próxima conversación

Cristian probablemente te dirá algo como "sigamos con el nivel 3" o "ahora la escupidora" o "mete la momia madre".

**Tu primera respuesta debe:**
1. Confirmar en 1 línea que leíste el handoff.
2. Hacer 1-2 preguntas máximo si hay algo ambiguo (con tu voto).
3. Si todo claro, proponer plan corto y pedir "dale".

**NO hagas:**
- Un resumen largo del proyecto (Cristian ya lo sabe).
- Una lista exhaustiva de lo que vas a hacer antes de que confirme.
- Preguntas que este doc ya responde.

---

## 9. Archivos de contexto en el repo

- `JUEGO_Vision_Completa.md` — Biblia original del juego (tabla de oleadas, stats, lore). **OJO: algunas decisiones cambiaron.**
- `BRIEF_NUEVO_CHAT.md` — Brief del arranque de la sesión actual, con plan de 5 fases A-E.
- `ESTADO_Y_ROADMAP.md` — Estado al 11 nov con análisis de lo que subió Antonio.
- `ESTADO_23_NOV.md` — Vista general en lenguaje natural al 23 nov (para Cristian).
- `AUDIOS_Para_Cristian.md` — Nota de Antonio con los 7 audios subidos y sus GUIDs.
- **ESTE archivo (`HANDOFF_CLAUDE.md`)** — El doc que estás leyendo.

---

## 10. Checklist rápido para próxima sesión

Antes de responder al primer mensaje de Cristian:

- [ ] Leíste este archivo completo.
- [ ] Entiendes que NO hagas git commits.
- [ ] Entiendes el flujo: plan corto → confirmación → ejecutas → cómo probar.
- [ ] Sabes que los editor scripts van en `Assets/Editor/` con menú `Momias/...`.
- [ ] Sabes que las modificaciones a scripts C# existentes se hacen con Python + `str.replace` + assert, nunca pegando código al chat.
- [ ] Sabes que el AudioManager de cada escena debe auto-cablear los 11 clips.
- [ ] Sabes que la cámara es estática en todos los niveles.
- [ ] Entiendes los pendientes prioritarios: Escupidora → Nivel 3 → (esperar Momia Madre) → Nivel 4 → Testing → Build.

Buena suerte 🔔
