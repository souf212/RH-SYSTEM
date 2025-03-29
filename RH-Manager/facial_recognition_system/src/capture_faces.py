import sys
import os
import cv2
import time
import mysql.connector

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))
from config import EMPLOYEE_FOLDER

# Ensure employees folder exists
os.makedirs(EMPLOYEE_FOLDER, exist_ok=True)

# Load OpenCV Face Detection Model
face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + "haarcascade_frontalface_default.xml")

def get_user_id(employee_name):
    """ Fetch `IdUtilisateur` for a given employee name from the database. """
    try:
        conn = mysql.connector.connect(
            host="localhost",
            user="root",
            password="zxcvbnm",  # 🔴 Update this with your actual MySQL password
            database="HR_Management_System1"
        )
        cursor = conn.cursor()

        # ✅ Fetch `IdUtilisateur`
        cursor.execute("SELECT Id FROM Utilisateurs WHERE LOWER(Nom) = %s", (employee_name.lower(),))
        result = cursor.fetchone()
        conn.close()

        if result:
            return result[0]  # Return `IdUtilisateur`
        else:
            print(f"❌ Error: User '{employee_name}' not found in database!")
            return None
    except mysql.connector.Error as e:
        print(f"❌ Database connection error: {e}")
        return None

def capture_face(employee_name, num_images=50):
    """ Capture and save multiple face images for better training. """
    utilisateur_id = get_user_id(employee_name)
    if not utilisateur_id:
        return  # Stop execution if no valid ID is found

    cap = cv2.VideoCapture(0)
    if not cap.isOpened():
        print("❌ Error: Camera not detected!")
        return

    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

    print(f"📸 Capturing {num_images} images for {employee_name} (IdUtilisateur: {utilisateur_id}). Press 'q' to cancel.")
    image_count = 0

    while image_count < num_images:
        ret, frame = cap.read()
        if not ret:
            print("❌ Error: Could not read frame from camera.")
            break

        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = face_cascade.detectMultiScale(gray, scaleFactor=1.1, minNeighbors=3, minSize=(80, 80))

        for (x, y, w, h) in faces:
            cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)

            # ✅ Save images with `IdUtilisateur` in filename (NOT `IdEmploye`)
            image_path = os.path.join(EMPLOYEE_FOLDER, f"{employee_name}_{utilisateur_id}_{image_count}.jpg")
            cv2.imwrite(image_path, frame)
            image_count += 1
            print(f"✅ Image {image_count}/{num_images} saved: {image_path}")

            if image_count >= num_images:
                break

        cv2.imshow("Face Capture", frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            print("❌ Capture aborted.")
            break

        time.sleep(0.2)

    cap.release()
    cv2.destroyAllWindows()
    print(f"✅ Successfully captured {num_images} images for {employee_name} (IdUtilisateur: {utilisateur_id})!")

if __name__ == "__main__":
    name = input("Enter employee name (as in database, or type 'exit' to quit): ").strip()
    if name.lower() != "exit":
        capture_face(name, num_images=50)
