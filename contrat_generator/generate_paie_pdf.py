import sys
import os
from jinja2 import Environment, FileSystemLoader
import cairosvg

# ✅ Lire les arguments depuis la ligne de commande
# Arguments attendus : nom, type_contrat, salaire, mois, date, heures
_, nom, type_contrat, salaire, mois, date, heures = sys.argv

# ✅ Obtenir le chemin absolu vers le dossier actuel
current_dir = os.path.dirname(os.path.abspath(__file__))
templates_dir = os.path.join(current_dir, "templates")
outputs_dir = os.path.join(current_dir, "outputs")

# ✅ Assurer que le dossier outputs existe
os.makedirs(outputs_dir, exist_ok=True)

# ✅ Préparer le moteur Jinja avec le chemin absolu
env = Environment(loader=FileSystemLoader(templates_dir))
template = env.get_template('fiche_de_paie_template.svg')

# ✅ Remplir le template SVG avec les données utilisateur
rendered_svg = template.render({
    "nom": nom,
    "type_contrat": type_contrat,
    "salaire": salaire,
    "mois": mois,
    "date": date,
    "heures": heures
})

# ✅ Génération du nom de fichier et du chemin de sortie
filename = f"fiche_paie_{nom.replace(' ', '_')}_{mois.replace(' ', '_')}.pdf"
output_path = os.path.join(outputs_dir, filename)

# ✅ Convertir le SVG en PDF
cairosvg.svg2pdf(bytestring=rendered_svg.encode('utf-8'), write_to=output_path)

# ✅ Retourner le chemin (lu par ASP.NET via stdout)
print(output_path)
