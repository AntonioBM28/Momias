#!/usr/bin/env bash
# update_kanban_v2.sh — Reorganiza el kanban según el nuevo plan (chunks + 6 avances).
# NO toca issues #1-20 (Avance 1 ya entregado al profe).

set -euo pipefail

OWNER="AntonioBM28"
REPO="Momias"
JOSE="AntonioBM28"
CRISTIAN="Cristian-Oropeza"
TARGET="$OWNER/$REPO"

MS2="Avance 2 - Nivel 1 Completo + Menú Principal"
MS3="Avance 3 - Niveles 2 y 3; HUD, audio, enemigos"
MS4="Avance 4 - Nivel 4; narrativa, guardado, opciones"
MS5="Avance 5 - Builds Web y Móvil; pruebas"
MSF="Entrega Final - Juego terminado + documentación + presentación"

echo "🚀 Fix completo del kanban para $TARGET"
echo ""

# ============================
# 1. ACTUALIZAR MILESTONES EXISTENTES
# ============================
echo "📝 Actualizando milestones existentes..."

gh api -X PATCH "repos/$TARGET/milestones/2" \
  -f title="$MS2" \
  -f description="Nivel 1 completo con los 4 estados del juego (Playing, Paused, GameOver, Victoria) + Menú Principal + HUD + audio + animaciones nuevas de Diego." \
  -f due_on="2026-10-30T23:59:59Z" >/dev/null

gh api -X PATCH "repos/$TARGET/milestones/3" \
  -f title="$MS3" \
  -f description="Nivel 2 con Boss Momia-Pípila (lead Cristian). Nivel 3 (lead José). Variantes medias (Corredora, Escupidora). Mecánica Campanazo Cargado + Barra de Eco. Chispas de eco." \
  -f due_on="2026-11-06T23:59:59Z" >/dev/null

echo "✅ Milestones #2 y #3 actualizados"
echo ""

# ============================
# 2. CREAR NUEVOS MILESTONES
# ============================
echo "🎯 Creando milestones nuevos..."

gh api -X POST "repos/$TARGET/milestones" \
  -f title="$MS4" \
  -f state="open" \
  -f description="Nivel 4 (Parroquia) + Boss final Momia Madre (3 fases). Sistema de narrativa (diálogos Doña Eulalia). Sistema de guardado (PlayerPrefs). Pantalla de Opciones. Mecánica reflejo cronometrado." \
  -f due_on="2026-11-13T23:59:59Z" >/dev/null || echo "  (Avance 4 ya existe)"

gh api -X POST "repos/$TARGET/milestones" \
  -f title="$MS5" \
  -f state="open" \
  -f description="Builds para WebGL, Android (controles touch), Windows. Testing externo con 3-5 usuarios. Corrección de bugs. Iconos de app. Página itch.io." \
  -f due_on="2026-11-20T23:59:59Z" >/dev/null || echo "  (Avance 5 ya existe)"

gh api -X POST "repos/$TARGET/milestones" \
  -f title="$MSF" \
  -f state="open" \
  -f description="Juego terminado en las 3 plataformas. Documentación técnica. Trailer de 60-90 seg. Presentación oral. Subida final a itch.io." \
  -f due_on="2026-11-27T23:59:59Z" >/dev/null || echo "  (Final ya existe)"

echo "✅ Milestones #4, #5, Final creados"
echo ""

# ============================
# 3. ABRIR MILESTONE 1 TEMPORALMENTE (para que gh issue create encuentre los cerrados)
# ============================
# NOTA: no necesitamos abrir Avance 1 porque no vamos a crear issues ahí.

# ============================
# 4. CERRAR ISSUES "SUPERSEDED"
# ============================
echo "🔄 Cerrando issues reemplazadas por chunks..."

close_superseded() {
    local num="$1" reason="$2"
    gh issue comment "$num" -R "$TARGET" -b "⚠️ **Reorganización del kanban (20 oct 2026)**: $reason" >/dev/null
    gh issue close "$num" -R "$TARGET" >/dev/null
    echo "  🔒 #$num cerrada"
}

close_superseded 21 "Agrupada en el nuevo chunk 'Pack de animaciones de Diego + Momia Bebé' (José) para Avance 2."
close_superseded 22 "Agrupada en el nuevo chunk 'Pack de animaciones de Diego + Momia Bebé' (José) para Avance 2."
close_superseded 23 "Agrupada en el nuevo chunk 'Integración de assets + HUD + mecánicas nuevas' (Cristian) para Avance 2."
close_superseded 24 "Agrupada en el nuevo chunk 'Sistema de estados del juego' (Cristian) para Avance 2."
close_superseded 25 "Agrupada en el nuevo chunk 'Sistema de estados del juego' (Cristian) para Avance 2."
close_superseded 26 "Agrupada en el nuevo chunk 'Menú Principal + navegación' (Cristian) para Avance 2."
close_superseded 27 "Agrupada en el nuevo chunk 'Pack de audio del Nivel 1' (José) para Avance 2."
close_superseded 28 "Agrupada en el nuevo chunk 'Pack de audio del Nivel 1' (José) para Avance 2."
close_superseded 34 "SceneManager absorbido en el chunk 'Sistema de estados del juego' (Cristian) para Avance 2."
close_superseded 36 "Momia Bebé absorbida en el chunk 'Integración de assets + HUD + mecánicas nuevas' (Cristian) + Pack animaciones (José) para Avance 2."

echo ""

# ============================
# 5. MOVER ISSUES A NUEVOS MILESTONES
# ============================
echo "📦 Moviendo issues a nuevos milestones..."

gh issue edit 29 -R "$TARGET" --milestone "$MS3" >/dev/null && echo "  📦 #29 → Avance 3"
gh issue edit 30 -R "$TARGET" --milestone "$MS3" >/dev/null && echo "  📦 #30 → Avance 3"
gh issue edit 33 -R "$TARGET" --milestone "$MS4" >/dev/null && echo "  📦 #33 → Avance 4"

echo ""

# ============================
# 6. ACTUALIZAR ISSUES EXISTENTES
# ============================
echo "✏️ Actualizando issues existentes..."

gh issue edit 31 -R "$TARGET" \
  --title "[AVANCE 3 - JOSÉ] Nivel 2 Callejones: aporte visual completo" \
  --body "Como lead del aporte visual del Nivel 2:
- Parallax de 3 capas (Callejones de Guanajuato, Callejón del Beso, luna llena, farolitos, papel picado).
- Sprites del Boss Momia-Pípila (walking + attack + die + losa en espalda).
- Música del Nivel 2 (OpenGameArt, estilo mariachi/guitarra).
- Montaje visual de la escena Nivel2_Callejones.unity (spawners, decoración).

Lead técnico: Cristian (#32)." >/dev/null && echo "  ✏️ #31 actualizada"

gh issue edit 32 -R "$TARGET" \
  --title "[AVANCE 3 - CRISTIAN] Nivel 2 Callejones: lead técnico + Boss Momia-Pípila" \
  --body "Como lead técnico del Nivel 2:
- Montar escena Nivel2_Callejones.unity con el arte que entregue José (#31).
- Oleadas específicas del Nivel 2 (4 oleadas): Caminantes + Bebés + 1 Corredora en oleada 3 + enjambre final con Corredoras.
- Código del Boss **Momia-Pípila** con 2 fases:
  1. Embestida de un lado al otro (Diego debe saltar).
  2. Al 50% vida: salto con onda expansiva (Diego debe estar lejos).
- Script boss distinto del enemigo regular (BossBase + MomiaPipila).
- Integración de música/SFX recibidos de José." >/dev/null && echo "  ✏️ #32 actualizada"

gh issue edit 35 -R "$TARGET" \
  --body "Nueva clase MomiaEscupidora : EnemyBase.
- Se queda cerca del borde (poco movimiento, ~1.0 u/s).
- HP: 3 campanazos.
- Daño: 1 corazón por proyectil.
- Lanza proyectil parabólico cada 2-3 seg.
- Diego debe **agacharse o saltar** para esquivar.
- Scripts: MomiaEscupidora.cs + EnemyProjectile.cs (parábola + daño on trigger con Player).
- Aparece desde N3 oleada 1 (ver GDD)." >/dev/null && echo "  ✏️ #35 actualizada"

echo ""

# ============================
# 7. CREAR NUEVAS ISSUES — AVANCE 2 (chunks grandes)
# ============================
echo "➕ Creando chunks de Avance 2..."

create_issue() {
    local title="$1" body="$2" labels="$3" milestone="$4" assignee="$5"
    local url
    url=$(gh issue create --title "$title" --body "$body" --label "$labels" --milestone "$milestone" --assignee "$assignee" -R "$TARGET")
    echo "  ✓ #${url##*/}: $title"
}

create_issue \
  "[AVANCE 2 - CRISTIAN - Chunk 1] Sistema de estados del juego" \
  "**Chunk grande que incluye:**
- GameManager con máquina de estados (Menu/Playing/Paused/GameOver/Victory)
- Pantalla de **Pausa** (toggle con Esc, Time.timeScale=0, panel con botones Continuar/Menú/Salir)
- Pantalla de **Game Over** (dispara al llegar a 0 vida, panel rojo con Reintentar/Menú)
- Pantalla de **Victoria del Nivel 1** (dispara al tocar la Campana Sagrada, panel dorado con Siguiente deshabilitado/Menú)
- SceneManager con fades de transición entre escenas

**Entregable:** los 4 estados funcionando (Playing ya existe) + transiciones con fade negro.

Reemplaza issues #24, #25, #34." \
  "type: feature,area: ui,area: tooling,priority: high" \
  "$MS2" "$CRISTIAN"

create_issue \
  "[AVANCE 2 - CRISTIAN - Chunk 2] Menú Principal + navegación" \
  "**Chunk grande:**
- Nueva escena \`Menu.unity\`
- Botones: Jugar, Opciones (placeholder), Créditos, Salir
- Navegación entre escenas (Menu ↔ Nivel1_Museo) con fade
- Música de menú en loop
- Integra el arte de José (fondo + estilos)

**Entregable:** al abrir el juego arranca en el menú, clic en Jugar carga el Nivel 1.

Reemplaza issue #26." \
  "type: feature,area: ui,priority: high" \
  "$MS2" "$CRISTIAN"

create_issue \
  "[AVANCE 2 - CRISTIAN - Chunk 3] Integración assets + HUD + mecánicas nuevas" \
  "**Chunk grande:**
- **HUD de corazones** (6 corazones en esquina sup izq, soporta medios, escucha PlayerHealth.OnHealthChanged)
- Añadir las nuevas animaciones de Diego (agachado + campanazo) al Animator Controller
- **AudioManager** (música con fade + pool de SFX)
- Integrar los audios de José en eventos (campanazo, impacto, pickup, UI click)
- **Momia Bebé** (código: hereda EnemyBase, flag requiereAgachado=true)
- **Item 💖 Corazón** (prefab + script + spawn 15% al matar momia, recupera 2 unidades)
- **🔔 Campana Sagrada del Nivel 1** (prefab + aparece al terminar oleada final + trigger E → Victoria)
- Tuning de balance final

**Entregable:** gameplay del Nivel 1 completo con todas las mecánicas.

Reemplaza issues #23, #34 (parcial), #36." \
  "type: feature,area: player,area: ui,area: tooling,priority: high" \
  "$MS2" "$CRISTIAN"

create_issue \
  "[AVANCE 2 - JOSÉ - Chunk 1] Pack animaciones Diego + Momia Bebé" \
  "**Chunk grande:**
- Sprite + animación de **Diego agachado** (1-2 frames, 48x48)
- Sprite + animación de **Diego campanazo real** (3-4 frames de swing con la campana)
- Sprite + animación de **Onda sónica** (3 frames de expansión del proyectil)
- Sprite + animación de **Momia Bebé** (walk + attack + die; puede ser palette swap de Caminante escalada a 0.5)

**Entregable:** PNGs en \`Assets/Sprites/Diego/\` y \`Assets/Enemies/\` + .anim listos.

Reemplaza issues #21, #22." \
  "type: feature,area: player,area: enemies,priority: high" \
  "$MS2" "$JOSE"

create_issue \
  "[AVANCE 2 - JOSÉ - Chunk 2] Pack visual de UI y pantallas" \
  "**Chunk grande:**
- Sprite de **Corazón** (3 estados: lleno, medio, vacío)
- Sprite de **Campana Sagrada**
- Diseño visual del **Menú Principal** (fondo + estilo de botones)
- Diseño visual de **Pantalla de Pausa** (panel semitransparente)
- Diseño visual de **Game Over** (color rojo, atmósfera dark)
- Diseño visual de **Victoria** (color dorado, celebración)

**Entregable:** PNGs en \`Assets/Sprites/UI/\` + mockups de las pantallas." \
  "type: feature,area: ui,priority: high" \
  "$MS2" "$JOSE"

create_issue \
  "[AVANCE 2 - JOSÉ - Chunk 3] Pack audio Nivel 1" \
  "**Chunk grande:**
- Música de **Nivel 1** (~2-3 min loop, estilo similar a 'Kamikaze' de Mora pero LIBRE — buscar en OpenGameArt o freemusicarchive)
- Música de **Menú Principal** (~1 min loop, misteriosa colonial)
- Música breve de **Game Over** (~5 seg)
- Música breve de **Victoria** (~10 seg, mariachi triunfal)
- **SFX** (5-7 archivos libres): campanazo, impacto, muerte momia, salto, pickup corazón, campana sagrada, UI click

**Entregable:** archivos .ogg/.wav en \`Assets/Audio/Music/\` y \`Assets/Audio/SFX/\` + README.md con licencias.

Reemplaza issues #27, #28 (música de boss va en Avance 3)." \
  "type: feature,area: audio,priority: high" \
  "$MS2" "$JOSE"

create_issue \
  "[AVANCE 2 - AMBOS] Testing + Build + entrega" \
  "**Compartido:**
- Testing conjunto del Nivel 1 completo (Menú → Playing → Pausa → Victoria/GameOver → Menú)
- Fix de bugs encontrados
- Verificar que compila sin errores
- Commit final + push al repo
- Grabar video corto de gameplay (30-60 seg)
- Actualizar PROGRESO.md con el estado del Avance 2
- Entregar al profesor" \
  "type: documentation,area: tooling,priority: high" \
  "$MS2" "$CRISTIAN"

echo ""

# ============================
# 8. CREAR NUEVAS ISSUES — AVANCE 3
# ============================
echo "➕ Creando issues de Avance 3..."

create_issue \
  "[AVANCE 3 - JOSÉ] Nivel 3 Alhóndiga: lead completo" \
  "Como **lead del Nivel 3** (José):
- Diseñar el gameplay feel del Nivel 3 (ritmo, dificultad, pacing)
- Arte completo: parallax de 3 capas de la fortaleza colonial
- Montaje visual de Nivel3_Alhondiga.unity (spawners, antorchas, decoración)
- Música del Nivel 3 (libre, OpenGameArt)
- Decisiones de dificultad (ajustar cantidades de oleadas del GDD)
- Testing jugable

Cristian aporta: código de variantes nuevas (#35), integración, bugs.

**Nivel 3:** 5 oleadas sin jefe, introduce los 4 tipos de momia." \
  "type: feature,area: levels,priority: high" \
  "$MS3" "$JOSE"

create_issue \
  "[AVANCE 3 - CRISTIAN] Mecánica Campanazo Cargado + Barra de Eco" \
  "Sistema que da profundidad al combate:
- Barra de **Eco** en el HUD (100 puntos máximo)
- Pulsar J → campanazo corto (ya existe)
- Mantener J → cargando (visual: anillo o glow en Diego) → al soltar → **campanazo cargado** (hitbox grande, viaja toda la pantalla; consume 50 eco)
- Al matar momias con ~30% probabilidad spawnean **✨ Chispa de eco** (+10 al medidor)
- Si eco < 50, no se puede cargado (feedback visual)

Scripts: modificar PlayerAttack + crear EcoManager + crear EchoSparkPickup." \
  "type: feature,area: player,area: ui,priority: high" \
  "$MS3" "$CRISTIAN"

create_issue \
  "[AVANCE 3 - JOSÉ] Sprites variantes medias + Chispa de Eco + Pípila" \
  "- Sprite de **Momia Corredora** (palette swap rojo de Caminante + ajuste visual)
- Sprite de **Momia Escupidora** (palette swap verde + animación de escupir)
- Sprite del **proyectil de la Escupidora** (polvo maldito, 2-3 frames)
- Sprite de **✨ Chispa de Eco** (3-4 frames idle sparkle)
- Sprite del Boss **Momia-Pípila** (gigante con losa, walk + embestida + salto + die)

PNGs en \`Assets/Enemies/\` y \`Assets/Sprites/VFX/\`." \
  "type: feature,area: enemies,area: vfx,priority: high" \
  "$MS3" "$JOSE"

create_issue \
  "[AVANCE 3 - AMBOS] Testing + Build + entrega" \
  "Testing conjunto de niveles 2 y 3. Fix bugs. Commit + push + video + entrega." \
  "type: documentation,area: tooling,priority: high" \
  "$MS3" "$CRISTIAN"

echo ""

# ============================
# 9. CREAR NUEVAS ISSUES — AVANCE 4
# ============================
echo "➕ Creando issues de Avance 4..."

create_issue \
  "[AVANCE 4 - CRISTIAN] Lead Nivel 4: código + Boss Momia Madre" \
  "Como lead del Nivel 4:
- Montar escena Nivel4_Parroquia.unity con arte de José
- Oleadas del Nivel 4 (6 oleadas) con los 4 tipos
- Código del **Boss final La Momia Madre** con 3 fases:
  1. Proyectiles de polvo (devuélvelos con reflejo cronometrado)
  2. Invocación de momias Caminantes desde el suelo
  3. Forma corrupta gigante que ocupa media pantalla
- Mecánica de **Reflejo cronometrado** (campanazo justo cuando proyectil está en hitbox → devuelve)" \
  "type: feature,area: levels,area: enemies,priority: high" \
  "$MS4" "$CRISTIAN"

create_issue \
  "[AVANCE 4 - JOSÉ] Lead Nivel 4: arte + cinemática de cierre" \
  "Como lead visual del Nivel 4:
- Parallax de 3 capas de la Parroquia de Dolores Hidalgo
- Sprites de las 3 fases visuales de la Momia Madre
- Música del Nivel 4 + música del boss final
- Sprites UI para narrativa (textbox + retrato Doña Eulalia)
- Cinemática de cierre del juego (serie de sprites o video corto): Diego toca las 4 campanas, amanecer, momias regresan al sueño" \
  "type: feature,area: levels,area: ui,priority: high" \
  "$MS4" "$JOSE"

create_issue \
  "[AVANCE 4 - CRISTIAN] Sistema de Narrativa (diálogos Doña Eulalia)" \
  "- Pantalla de diálogo con textbox estilo RPG + retrato del hablante
- Diálogos de Doña Eulalia al inicio de cada nivel (recibe guión conjunto)
- Script para avanzar con Espacio o clic
- Pausa el gameplay durante el diálogo
- Diálogos configurables en ScriptableObject" \
  "type: feature,area: ui,priority: medium" \
  "$MS4" "$CRISTIAN"

create_issue \
  "[AVANCE 4 - CRISTIAN] Sistema de Guardado + Pantalla de Opciones" \
  "**Guardado (PlayerPrefs):**
- Nivel máximo completado
- Volumen de música y SFX
- Settings de controles

**Pantalla de Opciones (desde Menú y Pausa):**
- Sliders de volumen (música + SFX)
- Toggle fullscreen (opcional)
- Botón 'Reset progreso'" \
  "type: feature,area: ui,area: tooling,priority: medium" \
  "$MS4" "$CRISTIAN"

create_issue \
  "[AVANCE 4 - AMBOS] Guión + testing + build + entrega" \
  "- Escribir juntos el guión de los diálogos de Doña Eulalia (1 página)
- Testing conjunto del juego completo (Menú → N1 → N2 → N3 → N4 → cinemática)
- Fix bugs
- Commit + push + video + entrega" \
  "type: documentation,area: tooling,priority: high" \
  "$MS4" "$CRISTIAN"

echo ""

# ============================
# 10. CREAR NUEVAS ISSUES — AVANCE 5
# ============================
echo "➕ Creando issues de Avance 5..."

create_issue \
  "[AVANCE 5 - CRISTIAN] Build WebGL + optimización" \
  "- Configurar Player Settings para WebGL
- Build y verificar en navegador
- Optimizar: compresión de audio, texturas 2D, scripts stripping
- Testar en Chrome, Firefox, Safari si posible" \
  "type: feature,area: tooling,priority: high" \
  "$MS5" "$CRISTIAN"

create_issue \
  "[AVANCE 5 - CRISTIAN] Build Android con controles touch" \
  "- Configurar Android SDK + NDK
- Adaptar controles a touch (joystick virtual + botones de ataque/salto/agacharse)
- Build .apk
- Testar en dispositivo real o emulador" \
  "type: feature,area: tooling,priority: high" \
  "$MS5" "$CRISTIAN"

create_issue \
  "[AVANCE 5 - CRISTIAN] Build Windows refinada" \
  "- Build final de Windows
- Icono del .exe
- Loader screen (opcional)
- Instalador o zip con instrucciones" \
  "type: feature,area: tooling,priority: high" \
  "$MS5" "$CRISTIAN"

create_issue \
  "[AVANCE 5 - JOSÉ] Testing externo + iconos + página itch.io" \
  "- Testing con 3-5 personas externas al equipo
- Recabar feedback (divertido, frustrante, bugs)
- Diseñar icono de app (Android + itch.io)
- Crear página de itch.io: capturas, descripción, créditos, link de descarga
- Reportar bugs encontrados" \
  "type: documentation,area: tooling,priority: high" \
  "$MS5" "$JOSE"

create_issue \
  "[AVANCE 5 - AMBOS] Priorización + fix bugs reportados" \
  "- Revisar reportes de testing
- Priorizar bugs (bloqueantes > funcionales > visuales)
- Dividir arreglos (Cristian código, José visual)
- Testear fixes
- Commit + push + entrega" \
  "type: bug,area: tooling,priority: high" \
  "$MS5" "$CRISTIAN"

echo ""

# ============================
# 11. CREAR NUEVAS ISSUES — ENTREGA FINAL
# ============================
echo "➕ Creando issues de Entrega Final..."

create_issue \
  "[FINAL - CRISTIAN] Documentación técnica final" \
  "- README.md del repo con descripción, setup, controles, créditos
- Diagrama de arquitectura (scripts, flujo)
- Guía de deployment por plataforma
- Changelog por avance" \
  "type: documentation,area: tooling,priority: high" \
  "$MSF" "$CRISTIAN"

create_issue \
  "[FINAL - JOSÉ] Subida itch.io + material visual" \
  "- Subir los 3 builds a itch.io (WebGL embed + descarga Windows + descarga Android)
- Capturas de pantalla finales (6-8 imágenes)
- Carátula del juego
- Trailer thumbnail
- Material para presentación oral (slides con screenshots)" \
  "type: documentation,area: tooling,priority: high" \
  "$MSF" "$JOSE"

create_issue \
  "[FINAL - AMBOS] Trailer 60-90 seg + presentación oral" \
  "- Guión del trailer (gameplay cuts + música + logo + CTA)
- José dirige artísticamente, Cristian edita técnicamente
- Música del trailer: 'Kamikaze' de Mora (uso académico, no monetizado)
- Presentación oral ante el profesor
- Demo en vivo del juego" \
  "type: documentation,area: tooling,priority: high" \
  "$MSF" "$CRISTIAN"

echo ""
echo "✅ ¡Setup completo!"
echo ""
echo "📊 Resumen del nuevo kanban:"
echo "   - 6 milestones (Avance 1-5 + Final)"
echo "   - Issues #1-20 Avance 1: INTACTAS ✅"
echo "   - 10 issues cerradas como superseded"
echo "   - 3 issues movidas (#29→A3, #30→A3, #33→A4)"
echo "   - 3 issues modificadas (#31, #32, #35)"
echo "   - 24 issues nuevas creadas"
echo ""
echo "   👉 Verifica en https://github.com/$TARGET/milestones"
echo "   👉 Y en https://github.com/$TARGET/issues?q=is%3Aissue+is%3Aopen"
