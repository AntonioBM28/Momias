#!/usr/bin/env bash
# setup_kanban_fix.sh — Crea solo las issues (labels y milestones ya existen).
# Fix al problema: gh issue create no encuentra milestones cerrados.
# Solución: abre Avance 1 temporalmente, crea las issues, lo cierra de nuevo.

set -euo pipefail

OWNER="AntonioBM28"
REPO="Momias"
JOSE="AntonioBM28"
CRISTIAN="Cristian-Oropeza"
TARGET="$OWNER/$REPO"

echo "🔓 Reabriendo milestone 'Avance 1' temporalmente..."
gh api -X PATCH "repos/$TARGET/milestones/1" -f state=open >/dev/null
echo "✅ Avance 1 abierto temporalmente"
echo ""

echo "🐛 Creando issues..."

create_issue() {
    local title="$1" body="$2" labels="$3" milestone="$4" assignee="$5" state="$6"

    local url
    url=$(gh issue create \
        --title "$title" \
        --body "$body" \
        --label "$labels" \
        --milestone "$milestone" \
        --assignee "$assignee" \
        -R "$TARGET")

    local number="${url##*/}"
    echo "  #$number: $title"

    if [[ "$state" == "closed" ]]; then
        gh issue close "$number" -R "$TARGET" >/dev/null
    fi
}

#######################################
# AVANCE 1 - CERRADAS (trabajo hecho)
#######################################

create_issue \
  "[SETUP] Configurar repositorio Unity 6.6 con URP 2D y Git LFS" \
  "- Proyecto Unity 6.6 con pipeline URP 2D.
- Git inicializado con .gitignore y .gitattributes de Unity.
- Git LFS para PNG, audio, video, etc." \
  "type: documentation,area: tooling" \
  "Avance 1 - Prototipo Base" \
  "$JOSE" "closed"

create_issue \
  "[NIVEL 1] Level blockout del Museo con parallax de 3 capas" \
  "Montaje completo del Nivel 1 Museo:
- Capa1: fondo con BoxCollider2D (suelo).
- Capa2: vitrinas rotas y pilares (midground).
- Capa3: foreground decorativo.
- Spawners izquierdo y derecho posicionados fuera de cámara." \
  "type: feature,area: levels" \
  "Avance 1 - Prototipo Base" \
  "$JOSE" "closed"

create_issue \
  "[ARTE] Sprites y animaciones de la Momia Caminante" \
  "Sprites + Animator Controller con 3 estados:
- Momia_Walk (default)
- Momia_Attack (trigger 'Atacar')
- Momia_Die (trigger 'Morir')" \
  "type: feature,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$JOSE" "closed"

create_issue \
  "[ARTE] Sprites de Diego (Idle, Walk, Run, Jump, Attack)" \
  "Sprite sheets + anims básicas:
- DiegoIDLE, DiegoWalkingt, DiegoRunning, DiegoJumping, DiegoCrossPunch.
- Animator con parámetro velocidadAbsoluta." \
  "type: feature,area: player" \
  "Avance 1 - Prototipo Base" \
  "$JOSE" "closed"

create_issue \
  "[ARTE] Fondos del Museo (Capa1, Capa2, Capa3)" \
  "Parallax art pintado para el Nivel 1." \
  "type: feature,area: levels" \
  "Avance 1 - Prototipo Base" \
  "$JOSE" "closed"

create_issue \
  "[PLAYER] Controlador de Diego con movimiento, salto y agacharse" \
  "\`PlayerController.cs\`:
- A/D movimiento horizontal con límites.
- Espacio salto con detección de suelo (OverlapCircle).
- S agacharse (reduce collider y velocidad).
- Flip del sprite por dirección.
- Escala base ajustable." \
  "type: feature,area: player" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[PLAYER] Sistema de vida de 6 corazones con i-frames y knockback" \
  "\`PlayerHealth.cs\`:
- 12 unidades (6 corazones de medios).
- i-frames de 1 seg con parpadeo rojo.
- Knockback físico.
- Eventos: OnHealthChanged, OnDeath, OnDamaged." \
  "type: feature,area: player" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[PLAYER] Campanazo direccional con J/clic izquierdo" \
  "\`PlayerAttack.cs\`:
- Ataque hacia la dirección que mira Diego.
- Cooldown de 0.35s.
- Spawn de hitbox temporal.
- Flash amarillo como feedback visual (placeholder)." \
  "type: feature,area: player" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[PLAYER] AttackHitbox temporal con auto-destrucción" \
  "\`AttackHitbox.cs\`:
- BoxCollider2D isTrigger.
- Vive 0.15 seg y se autodestruye.
- HashSet para no golpear al mismo enemigo 2 veces." \
  "type: feature,area: player" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[ENEMIES] EnemyBase abstracta + ScriptableObject de datos" \
  "\`EnemyBase.cs\` y \`EnemyData.cs\`:
- Clase base con vida, muerte y daño.
- ScriptableObject con stats por variante.
- Método SetData() público para asignar desde WaveManager." \
  "type: feature,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[ENEMIES] MomiaCaminante con IA (persecución + ataque + muerte)" \
  "\`MomiaCaminante.cs\`:
- Camina hacia Diego.
- En rango se detiene, dispara trigger 'Atacar'.
- Aplica daño en el frame correcto.
- Distancia 2D (jump-over seguro).
- Muere con trigger 'Morir'." \
  "type: feature,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[MANAGERS] WaveManager con 3 oleadas progresivas" \
  "\`WaveManager.cs\`:
- Oleadas configurables desde inspector.
- Spawn alternado izquierda/derecha.
- Dificultad progresiva (3, 5, 7 momias).
- Eventos para HUD futuro." \
  "type: feature,area: tooling" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[SETUP] Configurar Layers y Collision Matrix" \
  "- Tags: Player, Enemy.
- Layers: Ground, Boundary, Player, Enemy.
- Collision Matrix: Enemy↔Enemy, Player↔Enemy, Enemy↔Boundary desactivados." \
  "type: feature,area: tooling" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[DOCS] Documentación de progreso + guías + TODO" \
  "- GUIA_01_v2_Integracion_con_Jose.md
- PROGRESO_Avance1.md
- TODO_Despues_del_Avance1.md
- PROMPTS_FONDOS_Espanol.md
- VIDEO_HISTORIA_Guion.md" \
  "type: documentation,area: tooling" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[BUG] Pantalla amarilla al Play (Main Camera en Z=0)" \
  "**Síntoma:** Game view mostraba solo amarillo.
**Causa:** Camera Z=0, sprites Z=0 (dentro del near clip plane).
**Fix:** Mover Main Camera a Z=-10." \
  "type: bug,area: levels" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[BUG] Diego tapado por muebles del fondo" \
  "**Causa:** Sprite Renderer de Diego con Order in Layer bajo.
**Fix:** Order in Layer = 10." \
  "type: bug,area: player" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[BUG] Momias trababan en el spawn dentro de las paredes" \
  "**Causa:** Spawners dentro del BoxCollider2D de las paredes.
**Fix:** Mover spawners a X=±7." \
  "type: bug,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[BUG] Momias no entraban a cámara (chocaban con paredes)" \
  "**Causa:** Enemy ↔ Boundary collision activa.
**Fix:** Desactivar en Collision Matrix." \
  "type: bug,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[BUG] Momias caían al vacío fuera del ground" \
  "**Causa:** Capa1 BoxCollider angosto, spawners más afuera.
**Fix:** Extender BoxCollider2D Size X a 25." \
  "type: bug,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[BUG] Diego muere parado sobre momia (knockback loop)" \
  "**Causa:** Daño por distancia X; Diego caía encima y recibía daño en loop.
**Fix:** Distancia 2D en MomiaCaminante + desactivar Player↔Enemy en matrix." \
  "type: bug,area: player" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

#######################################
# AVANCE 2 - ABIERTAS (pendientes)
#######################################

create_issue \
  "[PLAYER] Animación de campanazo real (swing + onda sónica)" \
  "**Estado actual:** flash amarillo como placeholder.

**Ideal:**
- Sprite animado de Diego con swing de campana (4 frames).
- Prefab OndaSonica con proyectil visible.
- Transition Any State → Diego_Attack con trigger 'Attack'.
- La onda ES el hitbox visible." \
  "type: enhancement,area: player,area: vfx,priority: high" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

create_issue \
  "[PLAYER] Animación de agacharse (sprite propio)" \
  "**Estado actual:** se encoge en Y a 0.65 (placeholder).

**Ideal:**
- Sprite DiegoAgachado.png.
- Animación Diego_Crouch.
- Transition en Animator con isCrouching==true." \
  "type: enhancement,area: player,priority: medium" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

create_issue \
  "[UI] HUD de 6 corazones en pantalla" \
  "- Canvas con 6 Image.
- Script HeartsUI escucha PlayerHealth.OnHealthChanged.
- Soporta medios corazones." \
  "type: feature,area: ui,priority: high" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

create_issue \
  "[UI] Pantalla de Game Over" \
  "- GameManager escucha PlayerHealth.OnDeath.
- Panel con texto GAME OVER y botones Reintentar/Menú.
- Fade in oscuro." \
  "type: feature,area: ui,priority: high" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

create_issue \
  "[UI] Pantalla de Victoria" \
  "- GameManager escucha WaveManager.OnAllWavesCompleted.
- Panel VICTORIA con botones.
- Música de victoria breve." \
  "type: feature,area: ui,priority: medium" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

create_issue \
  "[UI] Menú Principal" \
  "Escena Menu.unity:
- Botones Jugar, Opciones, Salir.
- Transición a Nivel1_Museo.
- Fuente pixelada." \
  "type: feature,area: ui,priority: medium" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

create_issue \
  "[AUDIO] SFX: campanazo, impacto, gemido momia, salto, muerte" \
  "- Buscar/descargar 5 SFX libres (freesound.org).
- AudioManager con AudioSource.
- Asignar al PlayerAttack y MomiaCaminante." \
  "type: feature,area: audio,priority: high" \
  "Avance 2 - Pulido y Audio" \
  "$JOSE" "open"

create_issue \
  "[AUDIO] Música de nivel + música de boss" \
  "- OpenGameArt: tema de misterio colonial.
- Variante épica para boss.
- AudioManager con fade." \
  "type: feature,area: audio,priority: medium" \
  "Avance 2 - Pulido y Audio" \
  "$JOSE" "open"

create_issue \
  "[BUG] Momias se agrandan en animación (keyframes de scale)" \
  "Las animaciones Momia_Walk/Attack/Die tienen keyframes de Scale que distorsionan la escala.

**Fix:** abrir cada clip, remover propiedades Scale.x/y/z." \
  "type: bug,area: enemies,priority: low" \
  "Avance 2 - Pulido y Audio" \
  "$JOSE" "open"

create_issue \
  "[ENEMIES] Momia Corredora (variante roja, 2x velocidad)" \
  "- Crear EnemyData MomiaCorredora_Roja.
- Añadir oleada en WaveManager.
- Testear balance." \
  "type: feature,area: enemies,priority: medium" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

#######################################
# AVANCE 3 - ABIERTAS (futuro)
#######################################

create_issue \
  "[NIVEL 2] Fondo de Callejones + setup" \
  "- Generar fondo con prompt del PROMPTS_FONDOS.
- Montar escena Nivel2_Callejones.unity.
- Oleadas específicas del nivel." \
  "type: feature,area: levels,priority: medium" \
  "Avance 3 - Niveles y Jefes" \
  "$JOSE" "open"

create_issue \
  "[NIVEL 3] Fondo de Alhóndiga + Jefe Momia-Pípila" \
  "- Fondo con prompt.
- Escena Nivel3_Alhondiga.unity.
- Jefe Momia-Pípila con 2 fases (embestida + salto).
- Script boss distinto del regular." \
  "type: feature,area: levels,area: enemies,priority: medium" \
  "Avance 3 - Niveles y Jefes" \
  "$CRISTIAN" "open"

create_issue \
  "[NIVEL 4] Fondo de Parroquia + Jefe La Momia Madre" \
  "- Fondo con prompt.
- Escena Nivel4_Parroquia.unity.
- Jefe final con 3 fases.
- Cinemática de cierre." \
  "type: feature,area: levels,area: enemies,priority: medium" \
  "Avance 3 - Niveles y Jefes" \
  "$CRISTIAN" "open"

create_issue \
  "[MANAGERS] SceneManager para transiciones entre niveles" \
  "- Script LevelFlow con SceneManager.LoadScene.
- Fade negro al cambiar.
- PlayerPrefs para progreso." \
  "type: feature,area: tooling,priority: medium" \
  "Avance 3 - Niveles y Jefes" \
  "$CRISTIAN" "open"

create_issue \
  "[ENEMIES] Momia Escupidora (verde, lanza proyectil)" \
  "- Nueva clase MomiaEscupidora : EnemyBase.
- Se queda en el borde y lanza proyectil parabólico.
- Diego debe agacharse para esquivar." \
  "type: feature,area: enemies,priority: medium" \
  "Avance 3 - Niveles y Jefes" \
  "$CRISTIAN" "open"

create_issue \
  "[ENEMIES] Momia Bebé (pequeña, requiere agacharse)" \
  "- EnemyData con escala 0.5.
- Solo recibe daño si Diego ataca agachado.
- Modificar PlayerAttack para considerar esto." \
  "type: feature,area: enemies,priority: medium" \
  "Avance 3 - Niveles y Jefes" \
  "$CRISTIAN" "open"

echo ""
echo "🔒 Cerrando milestone 'Avance 1' nuevamente..."
gh api -X PATCH "repos/$TARGET/milestones/1" -f state=closed >/dev/null
echo ""
echo "✅ Setup completo! 36 issues creadas."
echo ""
echo "📊 Siguiente paso: crear el Project (kanban) en GitHub:"
echo "   1. https://github.com/$CRISTIAN?tab=projects → New project → Template: Board"
echo "   2. En el project → Add items → Add from repository → $TARGET → Select all → Add"
