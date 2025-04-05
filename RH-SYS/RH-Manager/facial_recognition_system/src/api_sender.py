import requests
from datetime import datetime
from config import API_URL

headers = {
    "Accept": "application/json",
    "Content-Type": "application/json"
}

def send_pointage(employee_id):
    timestamp = datetime.now().strftime('%Y-%m-%dT%H:%M:%S')

    # 🔹 Étape 1️⃣: Vérifier si une entrée active existe
    response = requests.get(f"{API_URL}/{employee_id}", headers=headers)

    print("🔍 GET Response Status:", response.status_code)
    print("🔍 GET Response Content:", response.text)  # ✅ Debugging

    if response.status_code == 200:
        try:
            data = response.json()
        except requests.exceptions.JSONDecodeError:
            print("🚨 ERREUR: La réponse du serveur n'est pas un JSON valide !")
            return
        
        if data and data.get("sortie") == "Pas encore sorti":
            # 🔹 Étape 2️⃣: Enregistrer l'heure de sortie
            exit_response = requests.put(f"{API_URL}/exit/{employee_id}", headers=headers)
            print("🔍 PUT Response Status:", exit_response.status_code)
            print("🔍 PUT Response Content:", exit_response.text)  # ✅ Debugging

            try:
                print(f"📡 Exit logged: {exit_response.json()}")
            except requests.exceptions.JSONDecodeError:
                print("🚨 ERREUR: La réponse du PUT n'est pas un JSON valide !")
            return

    # 🔹 Étape 3️⃣: Aucune entrée active → Créer une nouvelle entrée
    data = {
        "IdEmploye": employee_id,
        "HeureEntree": timestamp
    }

    entry_response = requests.post(API_URL, json=data, headers=headers)
    print("🔍 POST Response Status:", entry_response.status_code)
    print("🔍 POST Response Content:", entry_response.text)  # ✅ Debugging

    try:
        print(f"📡 Entry logged: {entry_response.json()}")
    except requests.exceptions.JSONDecodeError:
        print("🚨 ERREUR: La réponse du POST n'est pas un JSON valide !")

if __name__ == "__main__":
    employee_id = int(input("Enter employee ID: "))
    send_pointage(employee_id)
