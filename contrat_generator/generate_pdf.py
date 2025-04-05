import sys
import os
from jinja2 import Environment, FileSystemLoader
import cairosvg

# 📦 Récupérer les arguments depuis ASP.NET
Nom = sys.argv[1]
Email = sys.argv[2]
CIN = sys.argv[3]
TypeContrat = sys.argv[4]
DateDebut = sys.argv[5]
DateFin = sys.argv[6]
Salaire = sys.argv[7]

# 🔁 Préparer les données
data = {
    "Nom": Nom,
    "Email": Email,
    "CIN": CIN,
    "TypeContrat": TypeContrat,
    "DateDebut": DateDebut,
    "DateFin": DateFin,
    "Salaire": Salaire
}

# 📂 Chargement du template
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
env = Environment(loader=FileSystemLoader(os.path.join(BASE_DIR, "templates")))
template = env.get_template("contrat_template.svg")

rendered_svg = template.render(data)

# 📄 Dossier de sortie
output_dir = os.path.join(BASE_DIR, "outputs")
os.makedirs(output_dir, exist_ok=True)

filename = f"Contrat_{Nom.replace(' ', '_')}.pdf"
output_path = os.path.join(output_dir, filename)

# 🎯 Génération PDF
cairosvg.svg2pdf(bytestring=rendered_svg.encode('utf-8'), write_to=output_path)

print(output_path)
