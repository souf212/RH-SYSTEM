import face_recognition
import pickle
import os
import re
import mysql.connector
from config import EMPLOYEE_FOLDER, ENCODINGS_FILE

# ✅ Fetch `IdUtilisateur → IdEmploye` mapping from DB
def get_employee_mapping():
    try:
        conn = mysql.connector.connect(
            host="localhost",
            user="root",
            password="zxcvbnm",
            database="HR_Management_System1"
        )
        cursor = conn.cursor()
        # Fetch from utilisateurs table and join with Employes
        cursor.execute("""
            SELECT e.IdEmploye, e.IdUtilisateur, LOWER(u.Nom) 
            FROM Employes e
            JOIN utilisateurs u ON e.IdUtilisateur = u.Id
        """)
        mapping = {row[1]: {"id_employe": row[0], "name": row[2]} for row in cursor.fetchall()}
        
        conn.close()
        print(f"✅ Retrieved {len(mapping)} employee mappings from database.")
        return mapping
    except mysql.connector.Error as e:
        print(f"❌ Database connection error: {e}")
        return {}

def train_model():
    known_faces = []
    known_names = []

    # ✅ Fetch valid mappings dynamically
    employee_mapping = get_employee_mapping()
    if not employee_mapping:
        print("⚠️ No employees found in the database. Exiting training.")
        return

    # ✅ Extract name & `IdUtilisateur` from filename (moe_17_0.jpg)
    name_pattern = re.compile(r'([a-zA-Z]+)_(\d+)_(\d+)\.jpg$', re.IGNORECASE)

    for filename in os.listdir(EMPLOYEE_FOLDER):
        if filename.lower().endswith((".jpg", ".png")):
            image_path = os.path.join(EMPLOYEE_FOLDER, filename)
            image = face_recognition.load_image_file(image_path)
            encodings = face_recognition.face_encodings(image)

            if encodings:
                match = name_pattern.match(filename)
                if match:
                    detected_name = match.group(1).lower()  # Extract name
                    utilisateur_id = int(match.group(2))  # Extract `IdUtilisateur`

                    # ✅ Ensure `IdUtilisateur` exists in the database
                    if utilisateur_id in employee_mapping:
                        employee_id = employee_mapping[utilisateur_id]["id_employe"]  # Get `IdEmploye`
                        stored_name = f"{detected_name}_{employee_id}"  # Store `moe_8`

                        known_faces.append(encodings[0])
                        known_names.append(stored_name)
                        print(f"✅ Face encoded for {stored_name}")  # Now correctly storing `moe_8`
                    else:
                        print(f"⚠️ Skipping {filename}: No employee found with IdUtilisateur '{utilisateur_id}'")
                else:
                    print(f"⚠️ Skipping {filename}: Could not extract employee name properly")
            else:
                print(f"⚠️ No face found in {filename}, skipping...")

    # ✅ Save face encodings
    with open(ENCODINGS_FILE, "wb") as f:
        pickle.dump({"faces": known_faces, "names": known_names}, f)

    print("🎯 Face encodings saved successfully!")

if __name__ == "__main__":
    train_model()
