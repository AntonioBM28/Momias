#!/usr/bin/env bash
# setup_kanban.sh — Crea labels, milestones e issues del proyecto Momias del Grito
# Correr desde Git Bash en Windows (o cualquier shell con bash + gh CLI).
#
# PREREQUISITOS:
#   1. gh CLI instalado (https://cli.github.com)
#   2. gh auth login  (ya autenticado con permisos repo + project)
#
# USO:
#   1. Edita las 3 variables de abajo con tus datos reales.
#   2. chmod +x setup_kanban.sh
#   3. ./setup_kanban.sh
#
set -euo pipefail

############################
# >>> EDITA ESTO <<<
###############Oye brooooo, ocupo hacer un proyecto kanban en github documentando todo lo que hicimos, con issues y todo bien divididos con labels, asignaciones, milestone, y eso. Como puedes ayudarme de tal forma que quede a la de YA y bien hecho, pero dime cual seria la mejor forma
#############
OWNER="AntonioBM28"              # dueño del repo
REPO="Momias"                     # nombre del repo
JOSE="AntonioBM28"                # GitHub username de José
CRISTIAN="Cristian-Oropeza"   # GitHub username de Cristian ← CÁMBIALO
############################

TARGET="$OWNER/$REPO"
echo "🚀 Setup Kanban para $TARGET"
echo ""

###########################################
# 1. LABELS
###########################################
echo "📋 Creando labels..."

create_label() {
    local name="$1" color="$2" desc="$3"
    gh label create "$name" --color "$color" --description "$desc" -R "$TARGET" 2>/dev/null \
      || gh label edit "$name" --color "$color" --description "$desc" -R "$TARGET"
}

# Tipo
create_label "type: bug"           "d73a4a" "Error que hay que corregir"
create_label "type: feature"       "0e8a16" "Nueva funcionalidad"
create_label "type: enhancement"   "a2eeef" "Mejora de algo existente"
create_label "type: documentation" "0075ca" "Documentación"

# Área
create_label "area: player"      "6f42c1" "Scripts y animaciones de Diego"
create_label "area: enemies"     "e99695" "Momias y jefes"
create_label "area: levels"      "fbca04" "Escenarios y parallax"
create_label "area: ui"          "c5def5" "Interfaz y HUD"
create_label "area: audio"       "d4c5f9" "SFX y música"
create_label "area: vfx"         "f9d0c4" "Efectos visuales"
create_label "area: tooling"     "cccccc" "Setup, Git, CI, scripts"

# Prioridad
create_label "priority: high"    "b60205" "Alta"
create_label "priority: medium"  "fbca04" "Media"
create_label "priority: low"     "0e8a16" "Baja"

# Estado
create_label "status: blocked"   "000000" "Bloqueado por algo externo"

echo "✅ Labels listos"
echo ""

###########################################
# 2. MILESTONES
###########################################
echo "🎯 Creando milestones..."

create_milestone() {
    local title="$1" state="$2" due="$3" desc="$4"
    gh api -X POST "repos/$TARGET/milestones" \
      -f title="$title" \
      -f state="$state" \
      -f description="$desc" \
      -f due_on="$due" 2>/dev/null || echo "  (milestone '$title' ya existe, continuando)"
}

create_milestone "Avance 1 - Prototipo Base" "closed" \
  "2026-10-16T23:59:59Z" \
  "Personaje + controles + mecánica principal + repositorio. Entregado el 16/oct/2026."

create_milestone "Avance 2 - Pulido y Audio" "open" \
  "2026-11-14T23:59:59Z" \
  "Animaciones propias (campanazo, agacharse), HUD, pantallas, SFX, música, primera variante extra de momia."

create_milestone "Avance 3 - Niveles y Jefes" "open" \
  "2026-12-05T23:59:59Z" \
  "Niveles 2, 3, 4 con sus fondos, jefes (Pípila y Momia Madre), variantes restantes de momia, transiciones."

echo "✅ Milestones listos"
echo ""

###########################################
# 3. ISSUES
###########################################
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
- Escala base ajustable para que se vea más grande." \
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
- ScriptableObject con stats por variante (vida, velocidad, daño, tinte, escala).
- Método SetData() público para asignar desde WaveManager." \
  "type: feature,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[ENEMIES] MomiaCaminante con IA (persecución + ataque + muerte)" \
  "\`MomiaCaminante.cs\`:
- Camina hacia Diego.
- En rango se detiene, dispara trigger 'Atacar'.
- Aplica daño en el frame correcto de la animación.
- Usa distancia 2D (jump-over seguro).
- Muere con trigger 'Morir' y se destruye tras 1s." \
  "type: feature,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[MANAGERS] WaveManager con 3 oleadas progresivas" \
  "\`WaveManager.cs\`:
- Oleadas configurables desde inspector.
- Spawn alternado entre SpawnIzquierdo y SpawnDerecho.
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
  "- GUIA_01_v2_Integracion_con_Jose.md (setup paso a paso).
- PROGRESO_Avance1.md (handoff del Avance 1).
- TODO_Despues_del_Avance1.md (pendientes técnicos).
- PROMPTS_FONDOS_Espanol.md.
- VIDEO_HISTORIA_Guion.md." \
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
  "**Causa:** Spawners posicionados dentro del BoxCollider2D de las paredes.
**Fix:** Mover spawners a X=±7, fuera de las paredes." \
  "type: bug,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[BUG] Momias no entraban a cámara (chocaban con paredes)" \
  "**Causa:** Enemy ↔ Boundary collision activa.
**Fix:** Desactivar en Collision Matrix. Diego sigue bounded (Player↔Boundary activo)." \
  "type: bug,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[BUG] Momias caían al vacío fuera del ground" \
  "**Causa:** Capa1 BoxCollider solo cubría X=±4.86, spawners estaban más afuera.
**Fix:** Extender BoxCollider2D Size X a 25." \
  "type: bug,area: enemies" \
  "Avance 1 - Prototipo Base" \
  "$CRISTIAN" "closed"

create_issue \
  "[BUG] Diego muere parado sobre momia (knockback loop)" \
  "**Causa:** Momia detectaba daño solo por distancia X; Diego caía sobre ella y recibía daño repetido.
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
- Sprite animado de Diego haciendo swing con campana (4 frames, 48x48).
- Prefab OndaSonica (anillos que se expanden como proyectil).
- Transition Any State → Diego_Attack con trigger 'Attack'.
- La onda ES el hitbox visible y viaja ~2 unidades.

**Tareas:**
1. Dibujar sprites en Aseprite.
2. Crear anim Diego_Attack.
3. Agregar transition en Animator.
4. Modificar AttackHitbox.prefab con SpriteRenderer." \
  "type: enhancement,area: player,area: vfx,priority: high" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

create_issue \
  "[PLAYER] Animación de agacharse (sprite propio)" \
  "**Estado actual:** se encoge en Y a 0.65 (placeholder).

**Ideal:**
- Sprite DiegoAgachado.png (1-2 frames).
- Animación Diego_Crouch.
- Transition Any State → Diego_Crouch con isCrouching==true.
- Remover escalaYAgachado del PlayerController." \
  "type: enhancement,area: player,priority: medium" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

create_issue \
  "[UI] HUD de 6 corazones en pantalla" \
  "- Canvas con 6 Image de corazones.
- Script HeartsUI que escuche PlayerHealth.OnHealthChanged.
- Soporta medios corazones (12 unidades)." \
  "type: feature,area: ui,priority: high" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

create_issue \
  "[UI] Pantalla de Game Over" \
  "- GameManager escucha PlayerHealth.OnDeath.
- Panel con texto 'GAME OVER' y botones Reintentar / Menú.
- Fade in oscuro de fondo." \
  "type: feature,area: ui,priority: high" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

create_issue \
  "[UI] Pantalla de Victoria" \
  "- GameManager escucha WaveManager.OnAllWavesCompleted.
- Panel con texto 'VICTORIA' y botón Siguiente Nivel / Menú.
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
  "- Buscar/descargar 5 SFX libres (freesound.org o similar).
- AudioManager con AudioSource.
- Asignar al componente PlayerAttack (sfxCampanazo) y MomiaCaminante." \
  "type: feature,area: audio,priority: high" \
  "Avance 2 - Pulido y Audio" \
  "$JOSE" "open"

create_issue \
  "[AUDIO] Música de nivel + música de boss" \
  "- OpenGameArt: tema de misterio colonial para el Nivel 1.
- Variante épica para encuentros con jefes.
- AudioManager con fade entre ellos." \
  "type: feature,area: audio,priority: medium" \
  "Avance 2 - Pulido y Audio" \
  "$JOSE" "open"

create_issue \
  "[BUG] Momias se agrandan en animación (keyframes de scale)" \
  "Las animaciones Momia_Walk/Attack/Die tienen keyframes de Scale que distorsionan la escala.

**Fix:** abrir cada clip en Window → Animation → Animation, remover propiedades Scale.x, Scale.y, Scale.z." \
  "type: bug,area: enemies,priority: low" \
  "Avance 2 - Pulido y Audio" \
  "$JOSE" "open"

create_issue \
  "[ENEMIES] Momia Corredora (variante roja, 2x velocidad)" \
  "- Crear EnemyData MomiaCorredora_Roja (velocidad 3, tinte rojo).
- Añadir oleada en WaveManager con esta variante.
- Testear que no sea injusta." \
  "type: feature,area: enemies,priority: medium" \
  "Avance 2 - Pulido y Audio" \
  "$CRISTIAN" "open"

#######################################
# AVANCE 3 - ABIERTAS (futuro)
#######################################

create_issue \
  "[NIVEL 2] Fondo de Callejones + setup" \
  "- Generar fondo con prompt de PROMPTS_FONDOS_Espanol.md (Nivel 2).
- Montar escena Nivel2_Callejones.unity con parallax, suelo, paredes y spawners.
- Oleadas específicas del nivel." \
  "type: feature,area: levels,priority: medium" \
  "Avance 3 - Niveles y Jefes" \
  "$JOSE" "open"

create_issue \
  "[NIVEL 3] Fondo de Alhóndiga + Jefe Momia-Pípila" \
  "- Fondo con prompt.
- Montar escena Nivel3_Alhondiga.unity.
- Crear Jefe Momia-Pípila con 2 fases: embestida + salto.
- Script boss distinto del regular." \
  "type: feature,area: levels,area: enemies,priority: medium" \
  "Avance 3 - Niveles y Jefes" \
  "$CRISTIAN" "open"

create_issue \
  "[NIVEL 4] Fondo de Parroquia + Jefe La Momia Madre" \
  "- Fondo con prompt.
- Montar escena Nivel4_Parroquia.unity.
- Jefe final La Momia Madre con 3 fases (proyectiles, invocación, forma corrupta).
- Cinemática de cierre." \
  "type: feature,area: levels,area: enemies,priority: medium" \
  "Avance 3 - Niveles y Jefes" \
  "$CRISTIAN" "open"

create_issue \
  "[MANAGERS] SceneManager para transiciones entre niveles" \
  "- Script LevelFlow con SceneManager.LoadScene.
- Fade negro al cambiar.
- Guardar progreso (nivel actual) en PlayerPrefs." \
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
- Solo recibe daño si Diego ataca estando agachado.
- Modificar PlayerAttack para que considere esto." \
  "type: feature,area: enemies,priority: medium" \
  "Avance 3 - Niveles y Jefes" \
  "$CRISTIAN" "open"

echo ""
echo "✅ Setup completo!"
echo ""
echo "📊 Siguiente paso: crear el Project (kanban) en GitHub:"
echo "   1. Ve a https://github.com/users/$CRISTIAN/projects (o /orgs/.../projects)"
echo "   2. Clic en 'New project' → Template: 'Board'"
echo "   3. En el project: 'Add items' → 'Add from repository' → selecciona $TARGET → marca todas las issues."
echo ""
echo "   Las issues ya están ordenadas por milestone, así que puedes agruparlas por milestone en el board."
