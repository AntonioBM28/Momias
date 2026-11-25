# Estado del proyecto — 23 nov 2026

Juego completo al día de hoy, qué falta hacer y qué falta pedirle a Antonio.

---

## 🎮 Dónde vamos

Tenemos **5 escenas** en el proyecto:

- **Menú Principal** — listo y funcional.Bro, para cerrar el juego me faltan estos assets de tu lado, por prioridad:

URGENTE para el Nivel 3:

Sprite del proyectil que dispara la Momia Escupidora (un escupitajo de polvo, 3-4 frames pixel art, color verdoso/café para que combine con ella).

URGENTE para el Nivel 4 (Boss final):

Momia Madre completa: sprites (Idle, Walk, Attack, Die) + Animator + prefab + SO. Es el jefe final, debe verse grande e intimidante. Si puedes 2-3 variantes de attack para hacer fases, mejor; si no, con un Attack básico me acomodo.
Música Nivel 4 Parroquia (loop 2-3 min, épico/final).
Música Boss Final Momia Madre (loop 1-2 min, muy intenso).

Si te da tiempo (polish, no bloqueante):

SFX cortos: sfx_salto, sfx_pickup_corazon, sfx_ui_click, sfx_dano_diego.

Todo lo demás ya está integrado en el juego. Con estos cierro Nivel 3 y Nivel 4 y queda jugable end-to-end. Gracias crack 🔔
- **Nivel 1 Museo** — pulido y jugable completo.
- **Nivel 2 Alhóndiga con Boss Pípila** — armado, falta probar end-to-end.
- **Nivel 3 Callejones** — bloque básico de Antonio, sin gameplay todavía.
- **Nivel 4 Parroquia** — bloque básico de Antonio, sin gameplay todavía.

Las 4 variantes de momia están en el proyecto (Caminante, Bebé, Corredora, Escupidora) y el Boss Pípila también.

---

## ✅ Lo que ya funciona

- Menú principal con fondo animado, música, 4 botones (Jugar, Opciones, Créditos, Salir), panel de créditos con scroll, panel de opciones con sliders de volumen.
- Nivel 1 con 3 oleadas mezcladas (Caminantes + Bebés), Campana Sagrada al final, cinemática de victoria, Game Over, Pausa, HUD de corazones, música y SFX.
- Corazones que caen al matar momias y curan a Diego.
- Efecto de shockwave dorada al atacar.
- Game feel completo: shake de cámara, hit-pause, polvo al aterrizar y al morir enemigos, hover en botones.
- Audio cableado: música por nivel, jingle de victoria y game over, muerte de momia.

---

## 🛠️ Lo que me falta programar

**Lo inmediato (puedo hacerlo esta misma sesión):**

- Script de la **Momia Escupidora** (Antonio subió sprites y prefab pero sin código propio).
- Script del **Proyectil** que dispara la Escupidora.
- Armar el **Nivel 3 Callejones** completo con 5 oleadas que mezclan los 4 tipos de momia, sin boss.

**Depende de que Antonio entregue la Momia Madre:**

- Script del **Boss Final Momia Madre** con 3 fases.
- Armar el **Nivel 4 Parroquia** completo con el Boss Final.

**Al final:**

- Probar el juego completo (Menu → Nivel 1 → 2 → 3 → 4) y pulir bugs.
- Build final de Windows (.exe) para entregar.

---

## 🎨 Lo que falta de Antonio

**Para cerrar el Nivel 3:**

- Sprite del proyectil que dispara la Momia Escupidora (puede ser un escupitajo de polvo, 3-4 frames).

**Para cerrar el Nivel 4 (Boss Final):**

- **Momia Madre** completa: sprites (Idle, Walk, Attack, Die), Animator y prefab. Es el jefe final del juego, grande e intimidante.
- **Música del Nivel 4 Parroquia** (loop épico).
- **Música del Boss Final Momia Madre** (loop intenso).

**Si da tiempo (polish):**

- SFX cortos: salto, recoger corazón, clic de botón de UI, daño a Diego.

---

## 📊 Qué tan avanzados vamos

Más o menos **70% del juego listo**. Lo que falta es todo "integración" y el Boss Final.

Si Antonio sube pronto la Momia Madre, en una sesión más el juego queda jugable end-to-end. Después solo testing y build.
