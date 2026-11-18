# 📊 Comparación: lo que teníamos vs lo que subió José

**Fecha de análisis:** 9 nov 2026
**Rama actual:** `main` (hard reset a lo subido por José)

---

## ✅ LO QUE ESTÁ EN EL REPO AHORA (lo de José)

### Sistemas de código (14 scripts C#)
- **Managers**: GameManager, SceneTransition, AudioManager, WaveManager
- **Player**: PlayerController, PlayerHealth, PlayerAttack, AttackHitbox
- **Enemies**: EnemyBase, MomiaCaminante
- **Items**: SacredBell
- **UI**: Fader, GameOverPanel, HeartsHUD, LevelTitleCard, PauseMenu, VictoryCinematic, VictoryPanel
- **Data**: EnemyData

### Editor scripts
- **Chunk1Setup.cs** únicamente

### Assets visuales nuevos (de José, no los teníamos)
- 🎨 **Diego**: DiegoDeath.png, DiegoTakingDamage.png (sprites + anims `Diego_Death.anim`, `Diego_Damage.anim`)
- 🎨 **Momia Bebé**: `MomiaBebe_Idle.png`, `MomiaBebe_Walk.png`, `MomiaBebe_Attack.png` + Animator Controller `MomiaBebe.controller` + 3 anims
- 🎨 **CampanaSagrada.png** (sprite real del nivel)
- 🎨 **CorazonesHUD.png** (sprite real de los corazones)
- 🎨 Backgrounds Capa1/2/3 (sin cambio)

### Assets de audio (nuevos, reales)
- 🎵 `Audio/Music/Musica_Ambiental.mp3`
- 🔊 `Audio/SFX/Campanazo_Diego.mp3`
- 🔊 `Audio/SFX/Impacto_Momia.mp3`
- 🔊 `Audio/SFX/Recoger_Campana.mp3`

### Prefabs
- `MomiaWalking_0.prefab` (sin cambio)
- `MomiaBebe.prefab` (⚠️ existe pero usa script `MomiaCaminante`, no tiene mecánica bebé)
- `AttackHitbox.prefab`

### Escenas
- `Menu.unity` → ⚠️ **ESTÁ CASI VACÍA** (solo Main Camera + DontDestroyOnLoad). Por eso al dar clic en "Menú Principal" ves pantalla azul.
- `Nivel1_Museo.unity` → nivel funcional con gameplay, WaveManager, VictoryCinematic, GameOverPanel, LevelTitleCard, SacredBell.

---

## ❌ LO QUE SE PERDIÓ (que teníamos antes)

### Editor scripts (automatización)
- `Chunk2Setup.cs` → menú `Momias/Chunk 2/` para construir el menú principal con 1 clic
- `Chunk3Setup.cs` → menú `Momias/Chunk 3/` para HUD, Items, hooks, etc.
- `MomiasAssetSetup.cs` → menú `Momias/🔧` para configurar imports de sprites y audio

### Scripts C#
- **UI/MainMenu.cs** → lógica del menú principal (botones Jugar, Opciones, Créditos, Salir)
- **UI/CreditsPanel.cs** → panel de créditos con scroll automático
- **UI/OptionsPanel.cs** → placeholder opciones para Avance 4
- **UI/MenuBackground.cs** → animación Ken Burns del fondo del menú
- **UI/ShockwaveVFX.cs** → efecto visual de la onda sónica al atacar
- **Enemies/MomiaBebe.cs** → variante Momia Bebé (override RecibirDanio que requiere ataque agachado)
- **Items/HeartPickup.cs** → item recolectable corazón de vida

### Contenido en escenas
- `Menu.unity` tenía: Canvas con fondo del museo, título "MOMIAS DEL GRITO", botones Jugar/Opciones/Créditos/Salir, CreditsPanel con texto scrolleable, OptionsPanel placeholder
- `Nivel1_Museo.unity` tenía: oleada de Momia Bebé auto-agregada al WaveManager + hook SacredBell

### Documentación
- `GUIA_UNITY_Chunk2_y_3.md` → instrucciones paso a paso
- `PROGRESO_Avance2.md` → resumen del avance
- `README.md` → readme actualizado del proyecto

### Sprites/audio generados proceduralmente (los de José son MEJORES)
- Corazones (3 estados) generados → José tiene `CorazonesHUD.png` real
- Campana Sagrada generada → José tiene `CampanaSagrada.png` real
- MomiaBebe variante de Caminante → José tiene sprites propios
- SFX chiptune sintetizados → José tiene SFX reales
- Música chiptune → José tiene `Musica_Ambiental.mp3` real
- MenuBackground procedural → se perdió, no hay fondo del menú actualmente

---

## 🔀 DIFF RESUMIDO

| Categoría | Status |
|---|---|
| Sistemas base (GameManager, Audio, Scene, Wave) | ✅ Intactos |
| Gameplay Nivel 1 (Diego, Momia Caminante, Victoria) | ✅ Intacto |
| HUD de corazones | ✅ Intacto (con sprite real) |
| Audio del juego | ✅ MEJOR que antes (MP3 reales) |
| Animaciones Diego (Death, Damage) | 🆕 Nuevas |
| Sprites Momia Bebé | 🆕 Nuevos y mejores que los míos |
| Animator Momia Bebé | 🆕 Nuevo |
| **Menú Principal funcional** | ❌ **PERDIDO** (solo quedó escena vacía) |
| **Créditos** | ❌ **PERDIDO** |
| **Opciones (placeholder)** | ❌ **PERDIDO** |
| **Script MomiaBebe (mecánica agachado)** | ❌ **PERDIDO** |
| **Momia Bebé integrada en oleadas** | ❌ **PERDIDO** |
| **Onda sónica al atacar (ShockwaveVFX)** | ❌ **PERDIDO** |
| **HeartPickup (ítem recolectable)** | ❌ **PERDIDO** |
| **Editor scripts automatización** | ❌ **PERDIDO** (hay Chunk1 solo) |
| **Nivel 2 Callejones + Boss Pípila** | ❌ No existe (Avance 3) |

---

## 🎯 QUÉ HACE FALTA PARA EL AVANCE 3

### Para completar Nivel 1 (lo que faltó rellenar del Avance 2)
1. 🔧 Rearmar `Menu.unity` con botones, créditos, opciones
2. 🔧 Reescribir `MomiaBebe.cs` con mecánica de "solo daño si Diego ataca agachado"
3. 🔧 Asignar script `MomiaBebe` al prefab actual (en vez de MomiaCaminante)
4. 🔧 Agregar oleada de Momia Bebé al WaveManager de Nivel 1
5. 🔧 Rescribir `ShockwaveVFX.cs` + asignarlo al PlayerAttack
6. 🔧 Rescribir `HeartPickup.cs` + crear prefab con el sprite CorazonesHUD

### Para el Avance 3 completo (según JUEGO_Vision_Completa.md)
7. 🆕 Crear `Nivel2_Callejones.unity` (ambientación colonial)
8. 🆕 Scripts: `MomiaCorredora.cs` (velocidad 3x, daño 1 corazón) — necesita sprites de José
9. 🆕 Scripts: `BossPipila.cs` con 2 fases (embestida + salto con onda) — necesita sprite de José
10. 🆕 Sistema de **Barra de Eco** (HUD + mecánica)
11. 🆕 Mecánica de **Campanazo Cargado** (mantener J, consume 50 eco)
12. 🆕 Script `EchoSparkPickup.cs` + sprite (chispas al morir momia)
13. 🆕 Music del Nivel 2 + música del boss

### Para el Avance 3 acelerado (lo mínimo para entregar)
Si solo quieres Nivel 2 básico sin boss/eco todavía:
- Nivel 2 escena + arte reutilizado con tint diferente
- MomiaBebe + MomiaCorredora como enemigos nuevos
- Victoria al terminar oleadas
- Transición Nivel 1 → Nivel 2 → menú

---

## 📁 ARCHIVOS DE JOSÉ QUE PODEMOS APROVECHAR DIRECTO

- `Assets/Sprites/UI/CorazonesHUD.png` → asignar al `HeartsHUD` script
- `Assets/Sprites/Items/CampanaSagrada.png` → asignar al `SacredBell` en escena
- `Assets/Prefabs/MomiaBebe.prefab` → cambiar script a `MomiaBebe` (nuevo)
- `Assets/Animations/MomiaBebe.controller` → ya enlazado al prefab seguramente
- `Assets/Audio/Music/Musica_Ambiental.mp3` → asignar al `AudioManager.musicNivel1` o `musicMenu`
- `Assets/Audio/SFX/*.mp3` → asignar a los slots del AudioManager
- `Assets/Sprites/Diego/DiegoDeath.png` + `DiegoTakingDamage.png` + anims → integrar al Animator de Diego
