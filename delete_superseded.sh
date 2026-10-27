#!/usr/bin/env bash
# delete_superseded.sh — Borra permanentemente las 10 issues superseded del Avance 2 original.
# IMPORTANTE: necesitas permisos ADMIN en el repo para borrar issues.
# Si no eres admin, el script fallará y hay que pedírselo a José.

set -euo pipefail

OWNER="AntonioBM28"
REPO="Momias"
TARGET="$OWNER/$REPO"

ISSUES_TO_DELETE=(21 22 23 24 25 26 27 28 34 36)

echo "🗑️ Borrando ${#ISSUES_TO_DELETE[@]} issues superseded de $TARGET..."
echo ""

success=0
failed=0

for num in "${ISSUES_TO_DELETE[@]}"; do
    # Primero intentar con gh issue delete (requiere admin)
    if gh issue delete "$num" -R "$TARGET" --yes >/dev/null 2>&1; then
        echo "  ✅ #$num borrada"
        ((success++))
    else
        # Fallback: GraphQL (requiere mismo nivel de permisos)
        node_id=$(gh api "repos/$TARGET/issues/$num" --jq '.node_id' 2>/dev/null || echo "")
        if [[ -n "$node_id" ]]; then
            if gh api graphql -f query="mutation { deleteIssue(input: {issueId: \"$node_id\"}) { repository { name } } }" >/dev/null 2>&1; then
                echo "  ✅ #$num borrada (vía GraphQL)"
                ((success++))
            else
                echo "  ❌ #$num NO se pudo borrar (necesitas permisos admin en el repo)"
                ((failed++))
            fi
        else
            echo "  ❌ #$num no existe o no se pudo acceder"
            ((failed++))
        fi
    fi
done

echo ""
echo "📊 Resultado: $success borradas, $failed fallidas."

if [[ $failed -gt 0 ]]; then
    echo ""
    echo "⚠️  Si falló por permisos, pídele a José (dueño del repo) que las borre, o:"
    echo "    1. Que te dé rol de ADMIN en Settings → Collaborators."
    echo "    2. O que él corra este mismo script en su máquina."
fi
