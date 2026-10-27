# MOMIAS DEL GRITO — Visión Completa del Juego

**Versión:** 2.1 · **Fecha:** 2026-10-20
**Equipo:** Cristian E. Oropeza (programación) · José A. Balderas Melchor (arte, animación, audio, level design)
**Motor:** Unity 6.6 (6000.0.f1) · URP 2D
**Repositorio:** [AntonioBM28/Momias](https://github.com/AntonioBM28/Momias)

---

## 📑 Índice

1. [Pitch e historia](#1-pitch-e-historia)
2. [Personajes](#2-personajes)
3. [Mecánicas de juego](#3-mecánicas-de-juego)
4. [Estados del juego](#4-estados-del-juego)
5. [Enemigos a detalle](#5-enemigos-a-detalle)
6. [Los 4 niveles con oleadas](#6-los-4-niveles-con-oleadas)
7. [Items coleccionables](#7-items-coleccionables)
8. [Audio y música](#8-audio-y-música)
9. [Arte y estilo visual](#9-arte-y-estilo-visual)
10. [Arquitectura técnica](#10-arquitectura-técnica)
11. [Roadmap por entregas](#11-roadmap-por-entregas)
12. [División de trabajo](#12-división-de-trabajo)
13. [Convenciones del equipo](#13-convenciones-del-equipo)

---

## 1. Pitch e historia

### Pitch de una línea
Un guía turístico de Guanajuato debe tocar 4 Campanas Sagradas en los lugares emblemáticos del estado para devolver al sueño eterno a las momias que un turista despertó al tocar la Campana de Dolores al revés en la madrugada del 16 de septiembre.

### Sinopsis
Es la madrugada del 16 de septiembre. **Diego Ramírez**, un joven guía turístico del Museo de las Momias, cubre el turno nocturno. Un turista extranjero, buscando "buena suerte", toca la réplica de la Campana de Dolores **al revés** siguiendo un video viral falso. Ese sonido invertido rompe un sello colonial olvidado y las momias despiertan.

**Doña Eulalia**, curandera y bisabuela de Diego, le explica por videollamada que la única forma de revertir el ritual es tocar las **4 Campanas Sagradas** escondidas en los puntos donde el sello original fue clavado: el Museo, los Callejones, la Alhóndiga y la Parroquia de Dolores. Diego toma la campanita ceremonial de su abuela y sale a devolverle el sueño a las momias antes del amanecer.

### Estructura narrativa (3 actos)
- **Acto 1 — Detonante:** El turista toca la campana al revés; las momias despiertan; Diego escapa del Museo. **(Nivel 1)**
- **Acto 2 — Viaje:** Diego cruza el estado buscando las Campanas Sagradas; enfrenta nuevas variantes de momia y al primer jefe. **(Niveles 2 y 3)**
- **Acto 3 — Confrontación:** Llega a la Parroquia de Dolores, enfrenta a la Momia Madre y toca las 4 campanas en el altar para cerrar el sello. **(Nivel 4)**

---

## 2. Personajes

### Protagonista: Diego Ramírez
- **Edad / rol:** 19 años, guía nocturno del Museo de las Momias.
- **Descripción:** Introvertido, curioso por la historia local. Hereda de su bisabuela una campanita ceremonial que resulta ser el arma sónica clave del ritual.
- **Arma:** Campana de mano que emite ondas sónicas.
- **Vida:** 6 corazones (12 medios corazones = 12 unidades de HP).

### NPCs y aliados
- **Doña Eulalia:** Bisabuela curandera. Aparece en videollamada al inicio de cada nivel (Avance 4+). Guía a Diego sobre dónde está la próxima Campana Sagrada.
- **El Turista:** Detona el caos por accidente. Aparece como sidekick culposo en cinemáticas cortas, es alivio cómico.
- **Estudiantina fantasma** (opcional, Avance 4+): grupo de músicos fantasma amigables en los Callejones.

### Enemigos — 4 variantes de momia
(Detalle completo en [sección 5](#5-enemigos-a-detalle))
1. **Momia Caminante** (gris) — básica, melee lenta
2. **Momia Bebé** (gris pequeña) — básica, requiere agacharse para dañarla
3. **Momia Corredora** (roja) — media, rápida
4. **Momia Escupidora** (verde) — media, lanza proyectil

### Jefes
- **Momia-Pípila (Nivel 2):** Gigante con losa de piedra en espalda. 2 fases: embestidas + saltos pesados. Homenaje al héroe de la Alhóndiga.
- **La Momia Madre (Nivel 4):** Jefe final. 3 fases: proyectiles de polvo → invocación de momias menores → forma corrupta gigante.

---

## 3. Mecánicas de juego

### Controles (teclado)

| Tecla | Acción |
|---|---|
| **A / Flecha ←** | Caminar izquierda |
| **D / Flecha →** | Caminar derecha |
| **Espacio** | Saltar (solo si está en el suelo) |
| **S / Flecha ↓** | Agacharse (reduce velocidad y collider; obligatorio para atacar Momias Bebé) |
| **J / Clic izquierdo** | Campanazo direccional corto (hacia donde mira Diego) |
| **E** | Interactuar (recoger Campana Sagrada al final del nivel) |
| **Esc** | Pausa / menú |

### Movimiento
Diego se mueve libremente en el eje X dentro de una **arena fija del tamaño de la pantalla** (no hay scroll). Las paredes invisibles en los bordes lo mantienen contenido; las momias las atraviesan para entrar.

### Combate
- **Campanazo corto:** Hitbox temporal de ~1.5 unidades frente a Diego, duración ~0.15s. Cooldown 0.35s. Daño: 1 unidad por golpe (una Caminante muere con 2 golpes).
- **Campanazo cargado** (Avance 3+): Mantener J → carga → al soltar emite onda sónica que viaja toda la pantalla. Consume 50 puntos de **eco**.
- **Agacharse** no bloquea el campanazo; ajusta el hitbox a altura baja para pegarle a las Momias Bebé.

### Sistema de vida
- **Diego:** 12 unidades (6 corazones de medios).
- **i-frames:** 1 segundo tras recibir daño; Diego parpadea rojo y es invulnerable.
- **Knockback:** físico, lo empuja opuesto al atacante.
- **Daño por enemigo:**
  - Momias **básicas** (Caminante, Bebé) → **½ corazón** (1 unidad)
  - Momias **medias** (Corredora, Escupidora) → **1 corazón** (2 unidades)
  - Jefes → **2 corazones** (4 unidades) por golpe

### Progresión por nivel
Diego debe sobrevivir todas las oleadas → al final aparece la **Campana Sagrada del Nivel X** en el centro de la pantalla → la toca con **E** → cinemática breve → pantalla de **Victoria** → siguiente nivel.

### Sistema de Eco (Avance 3+)
Barra de 100 puntos en el HUD. Las **Chispas de Eco** que sueltan las momias al morir (~30% probabilidad) regeneran +10. El campanazo cargado consume 50.

---

## 4. Estados del juego

El juego es una máquina de estados simple. El `GameManager` orquesta las transiciones.

### Los 5 estados principales

#### 🏠 Menú Principal
- Escena independiente (`Menu.unity`).
- Botones: **Jugar**, **Opciones** (placeholder vacío hasta Avance 4), **Créditos**, **Salir**.
- Fondo con arte estilizado + música de misterio en loop.

#### ▶️ Playing
- Gameplay activo del nivel actual.
- HUD visible: corazones (y barra de eco desde Avance 3+).
- Música del nivel sonando.

#### ⏸ Paused
- Al presionar `Esc` durante Playing.
- Time.timeScale = 0 (congela físicas, animaciones y oleadas).
- Panel oscurecido con botones: **Continuar**, **Menú Principal**, **Salir**.
- Música baja volumen.

#### 💀 Game Over
- Dispara al llegar a 0 de vida.
- Panel rojo "GAME OVER" con botones: **Reintentar** (recarga la escena actual), **Menú Principal**.
- Música de game over breve.

#### 🏆 Victoria
- Dispara cuando se derrotan todas las oleadas del nivel Y Diego toca la Campana Sagrada.
- Panel "¡NIVEL X COMPLETADO!" con botones: **Siguiente Nivel** (deshabilitado si es el último nivel implementado), **Menú Principal**.
- Música de victoria breve (mariachi triunfal).

### Flujo entre estados

```
┌─────────────────┐
│  Menú Principal │
└────────┬────────┘
         │ Jugar
         ▼
┌─────────────────┐  Esc  ┌─────────┐
│    Playing      │◄─────►│ Paused  │
└────┬────────┬───┘       └────┬────┘
     │        │                │ Menú
     │ 0 HP   │ Campana        │
     ▼        ▼ recogida       ▼
┌─────────┐ ┌──────────┐  ┌─────────────┐
│Game Over│ │ Victoria │  │ Menú        │
└────┬────┘ └────┬─────┘  │ Principal   │
     │ Reint    │ Sig Nvl └─────────────┘
     ▼          ▼
  Playing   Playing (N+1)
```

---

## 5. Enemigos a detalle

### 🟢 Momia Caminante — BÁSICA
- **Sprite:** gris estándar.
- **Velocidad:** 1.5 unidades/seg.
- **HP:** 2 campanazos (vida 2).
- **Daño a Diego:** ½ corazón (1 unidad).
- **Comportamiento:** Camina horizontalmente hacia Diego. En rango melee se detiene y hace su animación de ataque. Aplica daño en el frame correcto.
- **Aparece en:** N1, N2, N3, N4.

### 🟢 Momia Bebé — BÁSICA
- **Sprite:** gris, escala 0.5 (más chica que la Caminante).
- **Velocidad:** 2.0 unidades/seg (más rápida que Caminante pero más chica).
- **HP:** 1 campanazo.
- **Daño a Diego:** ½ corazón.
- **Mecánica especial:** **SOLO recibe daño si Diego ataca estando agachado**. Si Diego le pega parado, el campanazo le pasa por encima.
- **Aparece en:** N1 (desde la oleada 2), N2, N3, N4.

### 🟠 Momia Corredora — MEDIA
- **Sprite:** tinte rojo (palette swap del base).
- **Velocidad:** 3.0 unidades/seg (2× la Caminante).
- **HP:** 2 campanazos.
- **Daño a Diego:** 1 corazón (2 unidades).
- **Comportamiento:** Igual a Caminante pero más rápida. Difícil de dejar atrás, obliga a reaccionar rápido o saltarla.
- **Aparece en:** N2 (desde la oleada 3), N3, N4.

### 🟠 Momia Escupidora — MEDIA
- **Sprite:** tinte verde.
- **Velocidad:** 1.0 unidad/seg (muy lenta).
- **HP:** 3 campanazos.
- **Daño a Diego:** 1 corazón por proyectil.
- **Comportamiento:** Se queda cerca del borde de la pantalla y lanza un proyectil parabólico de polvo cada 2-3 segundos. Diego debe **agacharse o saltar** para esquivarlo.
- **Debilidad:** melee corto no la alcanza fácil (está lejos). En Avance 3+, el **campanazo cargado** es ideal para ella.
- **Aparece en:** N3 (desde la oleada 1), N4.

### 👑 Momia-Pípila (Jefe Nivel 2)
- **Sprite:** gigante (192×192 px), con losa de piedra en la espalda.
- **HP:** ~15 campanazos (configurable).
- **Daño a Diego:** 2 corazones por golpe.
- **Fase 1:** Embiste de un lado al otro de la pantalla. Diego debe saltarla.
- **Fase 2 (al 50% vida):** Salta y cae con onda expansiva. Diego debe estar lejos del impacto.
- **Se mata:** rompiendo su losa con campanazo cargado (Avance 3) o campanazos acumulados (fallback).

### 👑 La Momia Madre (Jefe final Nivel 4)
- **Sprite:** gigante, forma humana al inicio.
- **HP:** ~30 campanazos total (3 fases).
- **Daño a Diego:** 2 corazones por golpe.
- **Fase 1 (100-66% vida):** Proyectiles de polvo que Diego devuelve con reflejo cronometrado.
- **Fase 2 (66-33% vida):** Invoca momias Caminantes del suelo mientras ataca.
- **Fase 3 (33-0% vida):** Se transforma en forma corrupta gigante que ocupa media pantalla.

---

## 6. Los 4 niveles con oleadas

### Nivel 1 — Museo de las Momias
- **Ambientación:** Pasillos oscuros del museo, vitrinas rotas, luces de emergencia parpadeantes. Paleta: verdes y dorados.
- **Jefe:** — (sin jefe)
- **Duración estimada:** 3-5 minutos.

| Oleada | Caminante | Bebé | Corredora | Escupidora | Total |
|---|:-:|:-:|:-:|:-:|:-:|
| 1 | 3 | - | - | - | 3 |
| 2 | 4 | 1 | - | - | 5 |
| 3 (enjambre) | 5 | 3 | - | - | 8 |

### Nivel 2 — Callejones de Guanajuato
- **Ambientación:** Callejón del Beso, casas amarillas/rojas/azules, farolitos, papel picado, luna llena. Paleta cálida (naranjas + rojos).
- **Jefe final:** Momia-Pípila.
- **Duración estimada:** 6-8 minutos.

| Oleada | Caminante | Bebé | Corredora | Escupidora | Total |
|---|:-:|:-:|:-:|:-:|:-:|
| 1 | 4 | 1 | - | - | 5 |
| 2 | 5 | 2 | - | - | 7 |
| 3 | 4 | 2 | 1 | - | 7 |
| 4 (enjambre) | 6 | 2 | 4 | - | 12 |
| 👑 Jefe | Momia-Pípila | | | | |

### Nivel 3 — Alhóndiga de Granaditas
- **Ambientación:** Fortaleza colonial, arcos de piedra, antorchas, banderas rasgadas. Paleta: piedra gris + fuego naranja + verde momia.
- **Jefe:** — (sin jefe, pero oleadas más intensas con 4 tipos)
- **Duración estimada:** 8-10 minutos.

| Oleada | Caminante | Bebé | Corredora | Escupidora | Total |
|---|:-:|:-:|:-:|:-:|:-:|
| 1 | 4 | 1 | 1 | 1 | 7 |
| 2 | 5 | 1 | 2 | 2 | 10 |
| 3 | 5 | 2 | 2 | 2 | 11 |
| 4 | 6 | 2 | 3 | 2 | 13 |
| 5 (final) | 8 | 3 | 4 | 3 | 18 |

### Nivel 4 — Parroquia de Dolores Hidalgo
- **Ambientación:** Interior de la parroquia iluminada por la luna, vitrales rotos, campana original colgando. Paleta: azules + dorado + rojo místico.
- **Jefe final:** La Momia Madre (3 fases).
- **Duración estimada:** 12-15 minutos.

| Oleada | Caminante | Bebé | Corredora | Escupidora | Total |
|---|:-:|:-:|:-:|:-:|:-:|
| 1 | 5 | 2 | 2 | 1 | 10 |
| 2 | 6 | 2 | 2 | 2 | 12 |
| 3 | 7 | 3 | 3 | 2 | 15 |
| 4 | 8 | 3 | 4 | 3 | 18 |
| 5 | 10 | 4 | 5 | 3 | 22 |
| 6 (enjambre masivo) | 12 | 5 | 6 | 4 | 27 |
| 👑 Jefe final | La Momia Madre (3 fases) | | | | |

---

## 7. Items coleccionables

### 💖 Corazón de vida
- Aparece con **~15% de probabilidad** cuando Diego mata una momia (cualquier tipo).
- Se recoge **caminando sobre él** (trigger collider).
- Recupera **1 corazón completo** (2 unidades) de vida.
- Si Diego está al máximo (12/12), no lo recoge (se queda en el suelo).

### ✨ Chispa de eco (Avance 3+)
- Aparece con **~30% de probabilidad** al matar momia.
- Se recoge igual.
- Suma **+10 al medidor de eco** (máximo 100).
- Alimenta el **Campanazo Cargado**.

### 🔔 Campana Sagrada del Nivel (fin de nivel)
- Aparece en el centro de la pantalla al terminar la oleada final.
- Diego la toca con **E** para activarla.
- Dispara cinemática breve "Campana recolectada" → pantalla de Victoria.
- Es el símbolo de progresión narrativa: 4 campanas = sello restaurado.

---

## 8. Audio y música

### Música por nivel

| Lugar | Pista | Fuente / Estado |
|---|---|---|
| Menú Principal | Instrumental misterioso, loop | OpenGameArt (libre) |
| Nivel 1 - Museo | Chelo tenso + latidos + eco de campana | OpenGameArt libre con vibra similar a *Kamikaze* de Mora |
| Nivel 2 - Callejones | Guitarra española + coro susurrado | OpenGameArt libre |
| Nivel 3 - Alhóndiga | Percusión ceremonial + trompeta lejana | OpenGameArt libre |
| Nivel 4 - Parroquia | Campanas mayores + sinfonía | OpenGameArt libre |
| Boss (Pípila / Momia Madre) | Variante rápida con percusión pesada | OpenGameArt libre |
| Victoria | Mariachi triunfal breve (~15s) | OpenGameArt libre |

### 🚨 Nota sobre "Kamikaze" (Mora)
A Cristian le gusta esta canción para el Nivel 1. **PERO tiene copyright duro:** si entra en la build o en un video público de YouTube, el Content ID la detecta en minutos y bloquea el video o reclama derechos. 

**Acuerdo del equipo:** usar *Kamikaze* **solo en el video de presentación de la historia** (uso académico, no se sube público). En el gameplay interno de la build se usa música libre con vibra similar.

### Efectos de sonido (SFX)
- Campanazo corto (campana metálica con reverb)
- Campanazo cargado (campana grave + onda expansiva grave) — Avance 3+
- Impacto a momia
- Gemido / crujido al morir momia
- Salto (whoosh breve)
- Pickup de corazón (chime positivo)
- Pickup de chispa (chispa eléctrica) — Avance 3+
- Recoger Campana Sagrada (campanada ceremonial)
- Diego recibe daño (grito contenido + hit)
- UI clicks (menús)
- Game Over (campanada grave de cierre)
- Victoria (acorde triunfal)

**Fuente de SFX:** freesound.org, OpenGameArt, Pixabay (todos CC0 o CC-BY).

**Producción interna:** José graba o edita en Audacity si es necesario.

---

## 9. Arte y estilo visual

### Estilo
- **Pixel art.**
- Sprites del protagonista: ~48×48 px (escalados 1.5× en runtime).
- Momias comunes: 32×48 px (base) con palette swap para variantes.
- Jefes: hasta 192×192 px.
- Fondos: parallax de 3 capas por nivel.

### Paleta general
Cálida colonial (naranjas, rojos, dorados) con contraste verde-momia y morado místico para elementos sobrenaturales. Alto contraste.

### Iluminación
Luces por sprite (veladoras, luna, flashazos de campana). Sin sistema 2D lights complejo — todo pintado sobre el sprite. Opcional: usar la `Global Light 2D` de URP para tintar el ambiente de cada nivel.

### Referencias visuales
- **Blasphemous** (atmósfera colonial oscura)
- **Katana Zero** (color pop en pixel)
- **Hollow Knight** (silueta clara del personaje)
- **Undertale** (accesibilidad y humor mezclado con drama)

### Herramientas
- **Aseprite** para sprites y animaciones (José).
- **Krita / GIMP** para íconos de UI y arte promocional (José).
- **Audacity** para edición de audio (José).

---

## 10. Arquitectura técnica

### Stack
- **Motor:** Unity 6.6 (6000.0.f1)
- **Pipeline:** Universal Render Pipeline 2D
- **Lenguaje:** C# (sin Bolt / Visual Scripting)
- **Control de versiones:** Git + Git LFS
- **IDE:** VS Code o Rider

### Estructura de carpetas

```
Momias/
├── Assets/
│   ├── Animations/            # Animator Controllers y clips (.anim)
│   ├── Audio/
│   │   ├── Music/             # Pistas de nivel
│   │   └── SFX/               # Efectos de sonido
│   ├── Enemies/               # Sprites de momia (PNG) + .meta
│   ├── Prefabs/
│   │   ├── Enemies/           # Prefabs de momias y jefes
│   │   ├── Items/             # Corazón, chispa, campana sagrada
│   │   ├── Player/            # AttackHitbox, proyectiles
│   │   └── UI/                # Paneles reutilizables
│   ├── Scenes/
│   │   ├── Menu.unity
│   │   ├── Nivel1_Museo.unity  (actual SampleScene.unity, renombrar)
│   │   ├── Nivel2_Callejones.unity
│   │   ├── Nivel3_Alhondiga.unity
│   │   └── Nivel4_Parroquia.unity
│   ├── ScriptableObjects/
│   │   ├── Enemies/           # EnemyData por variante
│   │   └── Waves/             # Configuración de oleadas (futuro)
│   ├── Scripts/
│   │   ├── Data/              # ScriptableObjects (EnemyData.cs)
│   │   ├── Enemies/           # EnemyBase + variantes
│   │   ├── Items/             # HeartPickup, EchoSparkPickup, SacredBell
│   │   ├── Managers/          # GameManager, WaveManager, SceneTransition, AudioManager
│   │   ├── Player/            # PlayerController, Health, Attack, AttackHitbox
│   │   └── UI/                # HeartsHUD, PauseMenu, GameOverPanel, VictoryPanel, MainMenu
│   ├── Settings/              # Config de URP
│   └── Sprites/
│       ├── Backgrounds/       # Parallax por nivel
│       ├── Diego/             # Sprites del protagonista
│       ├── UI/                # Iconos, botones custom
│       └── VFX/               # Efectos (onda sónica, flash impacto)
```

### Scripts clave y su responsabilidad

| Script | Responsabilidad |
|---|---|
| `PlayerController` | Movimiento, salto, agacharse, flip visual |
| `PlayerHealth` | Vida con eventos (OnHealthChanged, OnDeath) |
| `PlayerAttack` | Input de ataque + spawn de hitbox |
| `AttackHitbox` | Zona de daño temporal con auto-destrucción |
| `EnemyBase` (abstract) | Vida, daño, muerte compartidos |
| `MomiaCaminante` | IA de caminar + atacar en melee |
| `MomiaBebe` (futura) | Igual que Caminante pero con flag `requiereAgachado = true` |
| `MomiaCorredora` (futura) | Caminante con velocidad doble y más daño |
| `MomiaEscupidora` (futura) | Comportamiento estático + spawn de proyectil |
| `EnemyProjectile` (futura) | Proyectil parabólico de la Escupidora |
| `BossBase` (futura) | Clase base para jefes con fases |
| `MomiaPipila` (futura) | Boss Nivel 2 |
| `MomiaMadre` (futura) | Boss Nivel 4, 3 fases |
| `EnemyData` (ScriptableObject) | Stats por variante (vida, velocidad, daño, tinte, escala) |
| `WaveManager` | Gestor de oleadas con cantidades por tipo |
| `GameManager` | Máquina de estados (Menu/Playing/Paused/GameOver/Victory) |
| `SceneTransition` | Fades entre escenas |
| `AudioManager` | Música de fondo + pool de SFX |
| `HeartsHUD` | UI de corazones que escucha PlayerHealth |
| `PauseMenu` | Panel de pausa con botones |
| `GameOverPanel` | Panel al morir |
| `VictoryPanel` | Panel al terminar nivel |
| `MainMenu` | Botones del menú principal |
| `HeartPickup` | Item recolectable que recupera vida |
| `SacredBell` | Campana al final del nivel, trigger al presionar E |

### Setup de Unity

**Tags:**
- `Player`, `Enemy`, `Pickup`, `SacredBell`, `MainCamera` (default)

**Layers:**
- 6: `Ground`
- 7: `Boundary`
- 8: `Player`
- 9: `Enemy`
- 10: `EnemyProjectile`
- 11: `Pickup`

**Collision Matrix (Project Settings → Physics 2D):**
- `Enemy ↔ Enemy`: ❌ desactivado
- `Player ↔ Enemy`: ❌ desactivado
- `Enemy ↔ Boundary`: ❌ desactivado
- `Pickup ↔ Enemy`: ❌ desactivado
- `Pickup ↔ Player`: ✅ activado (para recoger)
- `EnemyProjectile ↔ Player`: ✅ activado (para hacer daño)
- `EnemyProjectile ↔ Boundary`: ❌ desactivado (que vuelen libres)

### Comunicación entre sistemas

Usamos **UnityEvents** para desacoplar sistemas:
- `PlayerHealth.OnHealthChanged(int actual, int max)` → `HeartsHUD` actualiza visual
- `PlayerHealth.OnDeath()` → `GameManager` cambia a estado `GameOver`
- `WaveManager.OnAllWavesCompleted()` → spawnea Campana Sagrada
- `SacredBell.OnPickedUp()` → `GameManager` cambia a estado `Victory`
- `GameManager.OnStateChanged(GameState)` → UI reacciona (muestra paneles)

---

## 11. Roadmap por entregas

Fechas fijadas por el profesor.

| # | Fecha | Objetivo | Estado |
|---|---|---|---|
| **Avance 1** | 16 oct ✅ | Prototipo base: personaje, controles y mecánica principal; repositorio creado | **Entregado** |
| **Avance 2** | **30 oct** | **Nivel 1 completo con los 4 estados; menú principal** | **En curso** |
| **Avance 3** | 6 nov | Niveles 2 y 3; HUD, audio y enemigos (campanazo cargado, chispas, variantes medias, Boss Pípila) | Pendiente |
| **Avance 4** | 13 nov | Nivel 4 (jefe final); narrativa, guardado y opciones | Pendiente |
| **Avance 5** | 20 nov | Builds en Web y Móvil; pruebas y corrección de errores | Pendiente |
| **Final** | 27 nov | Juego terminado en las 3 plataformas, documentación y presentación | Pendiente |

### Scope detallado por avance

**Avance 2 (30 oct) — EN CURSO:**
- GameManager con máquina de estados
- Pantallas: Pausa, Game Over, Victoria
- Menú Principal (nueva escena)
- HUD de corazones
- Música de Nivel 1 y Menú (libre)
- SFX básicos (campanazo, impacto, muerte, UI)
- Animación de campanazo real (swing + onda sónica)
- Animación de agacharse real
- Momia Bebé funcional (requiere agacharse)
- Item 💖 Corazón de vida
- 🔔 Campana Sagrada del Nivel 1
- Transiciones de escena con fades

**Avance 3 (6 nov):**
- Nivel 2 completo (Callejones) con Momia Corredora
- Nivel 3 completo (Alhóndiga) introduciendo Momia Escupidora
- Boss **Momia-Pípila** en N2
- Barra de **Eco** en el HUD
- Mecánica de **Campanazo cargado**
- Item ✨ Chispa de eco
- Música de niveles 2 y 3
- Más SFX

**Avance 4 (13 nov):**
- Nivel 4 completo (Parroquia)
- Boss final **La Momia Madre** (3 fases)
- Mecánica de **Reflejo cronometrado**
- Sistema de **Narrativa** (diálogos de Doña Eulalia al inicio de cada nivel)
- Sistema de **Guardado de progreso** (PlayerPrefs: nivel completado, vida máxima)
- Pantalla de **Opciones** funcional (volumen música, volumen SFX, rebind de controles opcional)
- Cinemática de cierre

**Avance 5 (20 nov):**
- Build para **WebGL** (jugar en navegador)
- Build para **Android** (adaptar controles a touch)
- Build para **Windows** (ya la tenemos)
- Testing con usuarios externos
- Corrección de bugs reportados
- Optimización de assets (compresión, atlas)

**Final (27 nov):**
- 3 plataformas pulidas
- Documentación técnica completa
- Presentación oral
- Trailer de 60-90 segundos
- itch.io con los 3 builds subidos

---

## 12. División de trabajo

### 🧑‍💻 Cristian — Programación e integración
- Todos los scripts de C# (gameplay, UI, managers)
- Setup técnico de Unity (tags, layers, collision matrix)
- Integración de animaciones de José al Animator Controller
- Integración de audio al proyecto (AudioManager)
- Debugging y balance de números
- Git / repo / documentación técnica (PROGRESO.md, TODO.md)
- Interfaz con mi asistente de IA (Claude)

### 🎨 José — Arte, animación, audio y level design
- Sprites y animaciones de Diego (incluyendo las nuevas: agachado, campanazo real)
- Sprites y animaciones de momias (variantes + jefes futuros)
- Sprites de VFX (onda sónica, pickups, flash de impacto)
- Fondos de parallax por nivel (los 4)
- Sprites de UI (corazones, íconos, botones custom si aplica)
- Diseño visual de pantallas (Menú, Game Over, Victoria, Pausa)
- Búsqueda y descarga de música libre (OpenGameArt, incompetech)
- Búsqueda y descarga de SFX libres (freesound)
- Edición de audio en Audacity si es necesario
- Montaje visual de las escenas de niveles (poner GameObjects, parallax, spawners)

### 🤝 Ambos
- Testing jugable
- Decisiones de game design
- Scrum ligero: standup rápido 2× por semana (mensaje por chat con qué hiciste / vas / blockers)
- Entregas: build final + commit + video + documentación

### División por Avance

| Avance | Cristian | José |
|---|---|---|
| 2 | GameManager + estados + UI + HUD + integración | Sprites agachado/campanazo + onda + corazones + música libre + SFX |
| 3 | Lógica de 2 niveles + boss Pípila + mecánica cargado + barra eco | Arte N2 y N3 + sprites variantes + música 2-3 + SFX nuevos |
| 4 | Boss final + narrativa + guardado + opciones | Arte N4 + boss sprite + música 4 + diálogos sprites |
| 5 | Builds Web/Android/Win + fixes | Testing + iconos de app + tienda itch.io |
| Final | Trailer edición + doc técnica | Trailer arte/música + presentación visual |

---

## 13. Convenciones del equipo

### Git workflow
- Rama principal: `main`. Siempre debe compilar y correr.
- Trabajo en **feature branches**: `feature/game-manager`, `feature/hud-corazones`, `art/diego-agachado`, etc.
- Al terminar, **Pull Request** con descripción breve. El otro revisa y mergea.
- Commits con prefijo tipo: `feat:`, `fix:`, `art:`, `audio:`, `docs:`, `refactor:`.
- **NO commitear al mismo tiempo cambios en la misma escena** (Unity genera conflictos horribles). Comunicar antes.

### Nombrado de archivos
- **Scripts:** PascalCase, descriptivos. Ej: `PlayerController.cs`, `MomiaCaminante.cs`.
- **Prefabs:** PascalCase con sufijo opcional. Ej: `Diego.prefab`, `MomiaWalking_0.prefab`, `AttackHitbox.prefab`.
- **Sprites:** camelCase o snake_case consistente. Ej: `diego_idle.png`, `diego_crouch.png`, `momia_corredora_walk.png`.
- **Animaciones:** `<Entidad>_<Accion>.anim`. Ej: `Diego_Idle.anim`, `Momia_Attack.anim`.
- **ScriptableObjects:** descriptivos. Ej: `MomiaCaminante_Gris.asset`, `Oleadas_Nivel1.asset`.
- **Escenas:** `NivelN_Nombre.unity`. Ej: `Nivel1_Museo.unity`, `Nivel2_Callejones.unity`.

### Comentarios en código
- En español, breves.
- Al inicio de cada script, un `/// <summary>` describiendo responsabilidad.
- Secciones con `[Header("...")]` en el inspector para claridad visual.

### Flujo de trabajo con assets de José
1. José entrega un asset (PNG, OGG, etc.) subiéndolo al repo en la carpeta correcta y haciendo PR.
2. Cristian pulls, integra en Unity (importa, crea anim, asigna en prefab).
3. Pruebas conjuntas.
4. Cierre de issue en kanban.

### Scrum ligero
- **Standup 2×/semana** (lunes y jueves por chat):
  - Qué hice desde el último standup
  - Qué voy a hacer antes del siguiente
  - Blockers
- **Retro al final de cada Avance** (sábado post-entrega): qué salió bien, qué mejorar.

### Comunicación con IA (Claude)
- Cristian lleva el chat con Claude.
- Cuando algo del juego cambia por decisión conjunta, Cristian actualiza este `.md` y lo comparte con José.
- Antes de arrancar cada Avance, Cristian abre una conversación nueva de Claude con un **prompt-brief** del Avance (ver `PROMPT_<Avance>.md`).

---

## 📌 Resumen de 1 línea

> **"Momias del Grito"** es un arena-brawler 2D pixel art donde Diego defiende el centro de pantalla con una campana sónica mientras momias de 4 variantes atacan por ambos flancos a través de 4 niveles icónicos de Guanajuato, culminando en un enfrentamiento con La Momia Madre en la Parroquia de Dolores Hidalgo.

---

**Última actualización:** 2026-10-20
**Mantenedor:** Cristian E. Oropeza
