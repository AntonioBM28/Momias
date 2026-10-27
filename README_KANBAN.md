# Setup del Kanban en GitHub — Momias del Grito

Este script crea en 1 ejecución **todo** lo que necesitas para el kanban:
- **17 labels** (type, area, prioridad, estado).
- **3 milestones** (Avance 1 cerrado, Avance 2 y 3 abiertos).
- **~36 issues** (20 cerradas con el trabajo del Avance 1, 16 abiertas con el roadmap).

Todas con labels, assignees y milestone correctos.

---

## 1. Prerequisitos (5 min, una sola vez)

### Instalar GitHub CLI
Si aún no lo tienes:

**Windows (recomendado winget):**
```powershell
winget install --id GitHub.cli
```

O descarga de https://cli.github.com/

### Autenticar
Abre **Git Bash** (o PowerShell) y corre:
```bash
gh auth login
```

Elige:
- GitHub.com
- HTTPS
- Login with a web browser → te abre el browser, autenticas → listo.

**Importante:** cuando pregunte sobre scopes, asegúrate de incluir `repo` y `project` (deberían venir por default).

Verifica que funciona:
```bash
gh auth status
```

Deberías ver `✓ Logged in to github.com as TuUsuario`.

---

## 2. Correr el script (2 min)

1. Guarda `setup_kanban.sh` en una carpeta cualquiera (ej. en el Escritorio).

2. Abre el archivo y **edita las 4 variables de arriba** si hace falta:
```bash
OWNER="AntonioBM28"              # ya está correcto
REPO="Momias"                     # ya está correcto
JOSE="AntonioBM28"                # confirma el username real de Jose
CRISTIAN="REPLACE_WITH_YOUR_GH"   # ← pon tu username de GitHub aquí
```

3. En Git Bash, en la carpeta del script:
```bash
chmod +x setup_kanban.sh
./setup_kanban.sh
```

El script tarda ~2-3 minutos. Verás:
```
📋 Creando labels...
✅ Labels listos
🎯 Creando milestones...
✅ Milestones listos
🐛 Creando issues...
  #1: [SETUP] Configurar repositorio Unity...
  #2: [NIVEL 1] Level blockout del Museo...
  ...
  #36: [ENEMIES] Momia Bebé...
✅ Setup completo!
```

### Si truena

| Error | Causa | Solución |
|---|---|---|
| `could not find any suitable gh` | gh no instalado | Volver al paso 1 |
| `You are not logged in` | No hiciste gh auth login | `gh auth login` |
| `HTTP 404: Not Found` | Repo wrong o sin acceso | Revisar OWNER/REPO |
| `HTTP 422: Validation Failed` | Milestone ya existía | Normal, el script continúa |
| `label already exists` | Script ya corrió antes | Normal, actualiza el label |

**El script es idempotente**: puedes correrlo múltiples veces sin problemas.

---

## 3. Crear el Project (kanban) desde la UI (1 min)

El script crea las **issues**, pero el **tablero kanban** (Project v2) se arma mejor desde la UI porque te deja escoger vista.

### Pasos:

1. En GitHub, ve a tu perfil o la organización dueña del repo.
2. Clic en **Projects** → **New project**.
3. Elige template **Board**.
4. Nómbralo: `Momias del Grito — Roadmap` (o lo que quieras).
5. Clic en **Create project**.

### Añadir las issues (1 clic):

6. En el nuevo project, arriba a la derecha, clic en **Add items**.
7. Elige **Add from repository** → busca `AntonioBM28/Momias`.
8. Selecciona todas las issues (hay un botón "Select all" después del filtro).
9. Clic **Add selected items**.

### Configurar vistas útiles:

En el Project, puedes crear múltiples vistas:
- **Vista 1 (Kanban por estado):** Board con columnas Todo / In Progress / Done. Ya viene por default.
- **Vista 2 (Por milestone):** Table view, agrupada por milestone. Útil para ver avances por sprint.
- **Vista 3 (Por área):** Board agrupado por label `area:`. Útil para dividir trabajo.

Para crear una vista:
1. Clic en el `+` al lado del tab "View 1".
2. Elige el tipo (Board/Table/Roadmap).
3. En el menú de la vista → "Group by" → elige milestone, labels o status.

---

## 4. ¿Qué queda después?

Si llegaste hasta aquí, tienes:
- ✅ Repo con el prototipo del Avance 1
- ✅ 36 issues organizadas
- ✅ 3 milestones (Avance 1 cerrado, Avance 2 y 3 abiertos)
- ✅ Project kanban con las issues

**Para la entrega al profe,** le puedes compartir el link del Project (es público si el repo lo es) o un screenshot del board. Más la URL del repo con el commit del Avance 1 y el build + video.

---

## 5. Si prefieres evitar bash

El script usa bash + gh CLI. Si prefieres PowerShell puro, los mismos comandos se traducen 1 a 1 (reemplaza `local` por `$local:` y adapta sintaxis). Pero bash en Git Bash es la ruta más rápida.

---

**Suerte bro. Con esto queda bien hecho y súper rápido. 🔔**
