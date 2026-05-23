#!/bin/bash
# Build-Skript: Baut die VueJS Applikation und kopiert das Bundle.

VUE_DIR="Jellyfin.Plugin.Template/Configuration/Web/VueJS"
BUILD_DIR="Jellyfin.Plugin.Template/Configuration/Web"

echo "Navigiere zu $VUE_DIR..."
cd "$VUE_DIR" || exit 1

echo "Starte Build..."
npm run build

echo "Kopiere Ergebnisse in den Web-Ordner..."
cp dist/TemplateVueJS.js "../TemplateVueJS.js"

echo "Build abgeschlossen."

