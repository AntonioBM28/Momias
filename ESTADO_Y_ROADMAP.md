# 📊 Estado del Proyecto + Roadmap Detallado

**Fecha:** 11 nov 2026
**Rama:** `main` (sincronizada con `origin/main`, commit `5b32677`)
**Último merge:** PR #63 - `feature/nivel2-alhondiga-granaditas` de Antonio

---

## 1. CONTEXTO DEL PROYECTO

**Momias del Grito** — arena-brawler 2D pixel art para la materia de Creación de Videojuegos.

**Equipo:**
- **Cristian E. Oropeza** (tú) — programación, sistemas, integración
- **José Antonio Balderas Melchor** (Antonio) — arte, animación, audio, level design

**Pitch:** Diego, guía turístico del Museo de las Momias de Guanajuato, debe tocar 4 Campanas Sagradas para devolver a las momias al sueño eterno después de que un turista desencadenara el caos.

**Estado de entregas (según `JUEGO_Vision_Completa.md`):**
- ✅ Avance 1 (16 oct): prototipo base
- ✅ Avance 2 (30 oct): Nivel 1 completo + menú principal
- 🟡 Avance 3 (6 nov): Niveles 2 y 3, HUD, audio, enemigos — **EN CURSO (atrasados)**
- 🔴 Avance 4 (13 nov): Nivel 4, boss final, narrativa, guardado — **próximo viernes**
- 🔴 Avance 5 (20 nov): builds multiplataforma
- 🔴 Final (27 nov): entrega completa

---

## 2. LO QUE ANTONIO ACABA DE SUBIR (análisis del último pull)

### 🎨 Arte nuevo
- **MomiaBebe completa**: sprites `Idle`, `Walk`, `Attack`, `Run`, `Die` + Animator Controller + 5 animaciones. Prefab ya existe pero **sin script propio**.
- **MomiaPipila completa** (boss grande): sprites `Walking`, `Jumping`, `TakingPunch`, `FallingBackDeath` + Animator Controller + 5 animaciones (Idle, Walk, Jump, Hurt, Die). Prefab existe **sin script**.
- **Nivel 2 backgrounds** (3 capas): `Capa1_Fondo`, `Capa2_Alhondiga` (fortaleza colonial piedra + banderas + antorchas), `Capa3_Suelo`.
- **Nivel 3 backgrounds** (3 capas): `Capa1_Fondo`, `Capa2_Callejon`, `Capa3_Suelo`.

### 📐 Escenas nuevas
- `Nivel2_Alhondiga.unity` — block-out básico: cámara, luz global, 2 paredes, 3 capas de background. **Sin Diego, sin WaveManager, sin UI, sin sistema de victoria**.
- `Nivel3_Callejones.unity` — mismo estado que Nivel 2.
- Ambas agregadas al `Build Settings` (índices 2 y 3).

### 🗂️ ScriptableObjects
- `MomiaBebe.asset` (EnemyData: vida=2, vel=2.1, daño=0.5, escala=0.85)
- `MomiaCaminante_Gris.asset` (EnemyData: vida=2, vel=1.5, daño=1, escala=1.3)
- `MomiaPipila.asset` (EnemyData: vida=20, vel=1.2, daño=1, escala=1)

### 🔧 En Nivel 1 ya funcional
- WaveManager configurado con **3 oleadas** (3, 5 y 7 Caminantes). **Sin bebés integradas.**
- Diego completo con animaciones (idle, walk, run, jump, attack, damage, death).
- HUD de corazones usando `CorazonesHUD.png`.
- Audio asignado: `Musica_Ambiental.mp3` como música del nivel, `Campanazo_Diego.mp3`, `Impacto_Momia.mp3`, `Recoger_Campana.mp3` como SFX.
- Victory Cinematic + GameOverPanel + PauseMenu + LevelTitleCard funcionando.

---

## 3. 🚨 PROBLEMAS DETECTADOS

### 3.1 Nombres de niveles INVERTIDOS respecto a la Biblia

| | Según `JUEGO_Vision_Completa.md` | Como lo armó Antonio |
|---|---|---|
| **Nivel 2** | Callejones de Guanajuato (sin boss, Momia Corredora intro) | **Alhóndiga de Granaditas** |
| **Nivel 3** | Alhóndiga de Granaditas (sin boss, 4 tipos de momia) | **Callejones de Guanajuato** |
| **Nivel 2 Boss** | ❌ No hay boss | - |
| **Boss Pípila** | Nivel 2 según la Biblia | Sin ubicar aún |

**🎯 DECISIÓN QUE NECESITAS TOMAR:**
- **Opción A:** Cambiar los nombres de las escenas de Antonio para que coincidan con la Biblia (Nivel2 = Callejones, Nivel3 = Alhóndiga).
- **Opción B:** Dejar las escenas como están y actualizar la Biblia (más fácil, más consistente con lo que ya hizo Antonio).

**Nota crítica:** El **Boss Momia-Pípila es un homenaje al héroe de la Alhóndiga**, entonces temáticamente hace más sentido que esté en el Nivel 2 (Alhóndiga según Antonio). **Recomiendo Opción B** — cambiar la Biblia.

### 3.2 Prefabs sin script propio

- `MomiaBebe.prefab` → usa `MomiaCaminante.cs` genérico. **No tiene la mecánica de "solo daño si Diego ataca agachado"** que pide la Biblia.
- `MomiaPipila.prefab` → **NO tiene ningún script** (solo sprite + Animator). No hace nada en el juego.

### 3.3 Menu.unity sigue vacía

Solo tiene `Main Camera`. Al dar clic en "Menú Principal" durante gameplay carga esta escena → pantalla azul vacía + música sigue sonando.

### 3.4 Nivel 1 — oleadas no incluyen Momia Bebé

Las 3 oleadas actuales son puro Caminante Gris (3, 5, 7 enemigos). La Biblia pide:
- Oleada 1: 3 Caminantes ✅
- Oleada 2: 4 Caminantes + 1 Bebé ❌ (actual: 5 Caminantes)
- Oleada 3: 5 Caminantes + 3 Bebés ❌ (actual: 7 Caminantes)

### 3.5 Niveles 2 y 3 son block-outs vacíos

Falta todo el gameplay: Diego, WaveManager, UI (HUD/Pausa/GameOver/Victory), SacredBell, hooks, GameManager persistente, etc.

### 3.6 Stats de ScriptableObjects no coinciden con Biblia

| Enemigo | Según Biblia | Como lo tiene Antonio |
|---|---|---|
| Momia Bebé: vida | 1 campanazo | 2 |
| Momia Bebé: daño | ½ corazón (1 unidad) | 0.5 (no es entero, EnemyData usa `int`) |
| Momia Bebé: escala | 0.5 | 0.85 |
| Boss Pípila: vida | ~15 | 20 |
| Boss Pípila: daño | 2 corazones (4 unidades) | 1 |

👉 Hay que decidir cuáles son los valores correctos. Yo sugiero **ajustar al balance jugable**, no pegarnos literal a la Biblia.

---

## 4. 📝 LO QUE FALTA POR HACER

### 4.1 PRIORIDAD ALTA — Pulido del Nivel 1 (lo que teníamos antes y se perdió)

**Scripts a crear:**
- [ ] `MomiaBebe.cs` → hereda de `MomiaCaminante`, override `RecibirDanio()` que solo acepta daño si origen Y está por debajo del centro (ataque agachado).
- [ ] `HeartPickup.cs` → ítem recolectable con bob + cura 2 unidades. Spawn con 15% de probabilidad al morir una momia.
- [ ] `ShockwaveVFX.cs` → efecto visual de onda sónica en el campanazo (opcional pero da mucho juice).

**Configuración:**
- [ ] Cambiar script del `MomiaBebe.prefab` de `MomiaCaminante` a `MomiaBebe`.
- [ ] Reconfigurar las 3 oleadas de Nivel 1 con la mezcla correcta de enemigos.
- [ ] Agregar hook en `EnemyBase.Morir()` para spawnear HeartPickup con probabilidad.
- [ ] Crear prefab `HeartPickup.prefab` usando `CorazonesHUD.png`.

### 4.2 PRIORIDAD ALTA — Menú Principal funcional

**Scripts a crear:**
- [ ] `MainMenu.cs` → lógica, hooks de botones.
- [ ] `CreditsPanel.cs` → panel de créditos con scroll automático.
- [ ] `OptionsPanel.cs` → placeholder "Próximamente Avance 4".
- [ ] `MenuBackground.cs` → animación Ken Burns del fondo.

**Contenido escena Menu.unity:**
- [ ] Canvas con fondo bello (fondo generado o reusar `Capa2_Alhondiga` escalado + vignette oscuro).
- [ ] Título "MOMIAS DEL GRITO" con animación de entrada.
- [ ] 4 botones: Jugar, Opciones, Créditos, Salir.
- [ ] Panel de Créditos con scroll animado.
- [ ] Panel de Opciones placeholder.
- [ ] GameManager + AudioManager + SceneTransition persistentes.
- [ ] Música del menú (ver sección 4.5).

### 4.3 PRIORIDAD ALTA — Juice / Game Feel

**Scripts a crear:**
- [ ] `CameraShaker.cs` (singleton) → screen shake al recibir daño y al matar enemigos.
- [ ] `HitPause.cs` → freeze-frame breve (Time.timeScale a 0.05 por 50ms) al matar enemigos.
- [ ] `DustBurst.cs` → partículas de polvo al aterrizar saltos y al morir enemigos.

**Mejoras a scripts existentes:**
- [ ] `PlayerHealth` → camera shake + hit-pause al recibir daño + flash rojo mejorado.
- [ ] `EnemyBase.Morir()` → dust burst + camera shake chico + hit-pause.
- [ ] Botones de UI con hover effect (scale 1.1 + glow + sound tick).

### 4.4 PRIORIDAD MEDIA — Armar Nivel 2 (Alhóndiga) con Boss Pípila

**Scripts a crear:**
- [ ] `BossPipila.cs` → boss con 2 fases:
  - Fase 1 (100-50% vida): embestidas horizontales rápidas.
  - Fase 2 (50-0% vida): salto + caída con onda expansiva.
- [ ] `BossHealthBar.cs` → barra de vida del jefe en pantalla.

**Montaje de escena Nivel2_Alhondiga.unity:**
- [ ] Copiar estructura completa del Nivel 1 (Diego, Managers, UI Canvas, SacredBell, WaveManager, VictorySystem).
- [ ] Configurar parallax de las 3 capas (`Nivel2_Capa1/2/3`).
- [ ] Spawners + 4 oleadas pequeñas antes del boss.
- [ ] Espacio para el Boss Pípila después de las oleadas.
- [ ] Música del Nivel 2 + música del boss (pendiente, pedirle a Antonio).

### 4.5 PRIORIDAD MEDIA — Música faltante

- [ ] Música del **Menú Principal** (diferente a la del Nivel 1).
  - **Opción A:** generar chiptune placeholder con Python.
  - **Opción B:** procesar `Musica_Ambiental.mp3` con pydub (slow + pitch down + reverb) → "versión menu" misma vibe.
  - **Opción C:** pedir a Antonio una pista nueva.
- [ ] Música del **Nivel 2** (Alhóndiga).
- [ ] Música del **Boss Pípila**.
- [ ] Música del **Nivel 3** (Callejones).
- [ ] Música de **Victoria** breve (~10s, mariachi triunfal).
- [ ] Música de **Game Over** breve (~5s, lamento).

### 4.6 PRIORIDAD BAJA — Nivel 3 (Callejones) sin boss

**Montaje:**
- [ ] Copiar estructura de Nivel 2.
- [ ] Configurar parallax Nivel 3.
- [ ] 5 oleadas con 4 tipos de momia (según Biblia). Pendiente de que Antonio haga Momia Corredora y Momia Escupidora.

### 4.7 PENDIENTE DE ANTONIO (asset request)

Para completar Nivel 3 y Nivel 4:

**Sprites necesarios:**
- [ ] Momia Corredora (palette swap rojo de Caminante o sprite propio): Idle, Walk rápido, Attack, Die.
- [ ] Momia Escupidora (verde, sprite propio): Idle, Spit (lanzar proyectil), Die.
- [ ] Proyectil de polvo (3-4 frames).
- [ ] La Momia Madre (boss final Nivel 4): sprite grande 192x192+ con 3 fases.

**Audio:**
- [ ] Música del Menú (si va por opción C).
- [ ] Música del Nivel 2 + Boss Pípila.
- [ ] Música del Nivel 3.
- [ ] Música del Nivel 4 + Boss Final.
- [ ] SFX restantes: salto, pickup corazón, UI click, campanazo cargado, daño a Diego, gemido muerte momia.

---

## 5. 🎯 RECOMENDACIÓN DE ORDEN DE TRABAJO

**Fase A — Pulir Nivel 1 (~45 min):**
1. `MomiaBebe.cs` + fix prefab + reconfigurar 3 oleadas Nivel 1
2. `HeartPickup.cs` + prefab + hook en `EnemyBase.Morir()`
3. `ShockwaveVFX.cs` (opcional pero rápido)

**Fase B — Menú Principal bello (~30 min):**
4. 4 scripts del menú (`MainMenu`, `CreditsPanel`, `OptionsPanel`, `MenuBackground`)
5. Fondo del menú generado / usar capa del Nivel 2
6. Editor script `Chunk2Setup.cs` para armar escena con 1 clic
7. Música del menú (opción B — procesar la de Antonio)

**Fase C — Juice (~30 min):**
8. `CameraShaker.cs`
9. `HitPause.cs`
10. `DustBurst.cs`
11. Hover + sound en botones

**Fase D — Nivel 2 con Boss Pípila (~60 min):**
12. `BossPipila.cs` con 2 fases
13. Fix script de MomiaPipila.prefab
14. Armar escena Nivel2_Alhondiga completa
15. 4 oleadas + hook para spawn del boss
16. `BossHealthBar.cs`
17. VictoryCinematic Nivel 2

**Fase E — Nivel 3 (depende de assets de Antonio):**
18. Esperar Momia Corredora y Momia Escupidora
19. Armar escena Nivel3_Callejones
20. 5 oleadas con 4 tipos

---

## 6. ❓ DECISIONES QUE NECESITAS TOMAR

1. **Nombres de niveles invertidos**: ¿Opción A (renombrar escenas) u **Opción B (actualizar Biblia, recomendado)**?
2. **Música del menú**: ¿Generada A, procesada B, o pedir a Antonio C?
3. **Stats de ScriptableObjects**: ¿pegarnos a la Biblia o ajustar a lo jugable?
4. **Orden**: ¿Vamos fase A→B→C→D o prefieres otro orden?
5. **Juice pack**: ¿lo incluyes (hace el juego ver mucho más pro) o lo saltamos por tiempo?

---

## 7. 📂 ARCHIVOS CLAVE DEL PROYECTO

```
Momias/
├── JUEGO_Vision_Completa.md       ← Biblia del diseño
├── ESTADO_Y_ROADMAP.md            ← este archivo
├── COMPARACION_ESTADO.md          ← comparación anterior (puede eliminarse)
├── Assets/
│   ├── Scripts/                   ← 19 scripts C#
│   ├── Editor/Chunk1Setup.cs      ← único editor script actual
│   ├── Scenes/
│   │   ├── Menu.unity             ← vacía, falta armar
│   │   ├── Nivel1_Museo.unity     ← funcional pero falta integrar bebés
│   │   ├── Nivel2_Alhondiga.unity ← block-out vacío
│   │   └── Nivel3_Callejones.unity ← block-out vacío
│   ├── Enemies/                   ← sprites + anims
│   ├── Prefabs/                   ← MomiaWalking, MomiaBebe, MomiaPipila
│   ├── ScriptableObjects/Enemies/ ← EnemyData assets
│   ├── Animations/                ← controllers y .anim
│   ├── Audio/                     ← 1 música + 3 SFX
│   └── Sprites/
│       ├── Diego/                 ← sprites + anims completas
│       ├── Backgrounds/           ← Nivel 1 + Nivel 2 + Nivel 3
│       ├── Items/                 ← CampanaSagrada
│       └── UI/                    ← CorazonesHUD
└── README_KANBAN.md               ← info del tablero kanban
```
