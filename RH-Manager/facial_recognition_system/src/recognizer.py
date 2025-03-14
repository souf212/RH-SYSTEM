import os
import cv2
import face_recognition
import pickle
import requests
import time
import numpy as np
from datetime import datetime, timedelta
from config import ENCODINGS_FILE, API_URL

# Suppress unnecessary macOS warnings
os.environ["QT_LOGGING_RULES"] = "*.debug=false"

# Load known faces from stored encodings
if not os.path.exists(ENCODINGS_FILE):
    print("❌ Error: Face encodings file not found. Train the model first.")
    exit()

with open(ENCODINGS_FILE, "rb") as f:
    data = pickle.load(f)

known_faces = np.array(data.get("faces", []))
known_names = data.get("names", [])

print(f"🔍 Loaded {len(known_faces)} known faces.")

if len(known_faces) == 0:
    print("⚠ Warning: No faces found in the encoding file. Train the model again.")
    exit()

# Start webcam with optimized resolution
video_capture = cv2.VideoCapture(0)
video_capture.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
video_capture.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

if not video_capture.isOpened():
    print("❌ Error: Cannot open webcam. Trying fallback method...")
    video_capture = cv2.VideoCapture(1)  # Try using another camera index
    if not video_capture.isOpened():
        print("❌ Error: No available camera found.")
        exit()

print("✅ Camera initialized successfully.")

EXIT_TIME_THRESHOLD = timedelta(minutes=30)
recognized_today = {}
frame_skip = 3
frame_count = 0

def get_employee_id_from_name(name):
    """Extract the correct `IdEmploye` from the detected name (moe_8 → 8)."""
    try:
        parts = name.split("_")
        if len(parts) == 2 and parts[1].isdigit():
            return int(parts[1])  # Returns `IdEmploye`
    except ValueError:
        return None
    return None

def send_pointage(employee_id):
    """Send check-in time if not already recorded."""
    if employee_id in recognized_today and (time.time() - recognized_today[employee_id]) < 30:
        print(f"⚠️ Employee {employee_id} was detected recently. Skipping duplicate entry.")
        return

    timestamp = datetime.now().replace(second=0, microsecond=0).isoformat()
    data = {"HeureEntree": timestamp, "IdEmploye": employee_id}

    print(f"📡 Sending API Request: {data}")  

    try:
        response = requests.post(API_URL, json=data)
        print(f"🛠 API Response: {response.status_code} - {response.text}")

        if response.status_code == 200:
            print(f"✅ Attendance recorded for Employee ID {employee_id}")
            recognized_today[employee_id] = time.time()
        else:
            print(f"⚠️ API Error {response.status_code}: {response.text}")

    except requests.exceptions.RequestException as e:
        print(f"❌ API Error: {e}")

while True:
    ret, frame = video_capture.read()
    if not ret:
        print("❌ Error: Unable to capture frame. Retrying...")
        continue  # Keep retrying instead of exiting

    frame_count += 1
    if frame_count % frame_skip != 0:
        continue  # Skip frames for performance

    small_frame = cv2.resize(frame, (0, 0), fx=0.5, fy=0.5)
    rgb_frame = cv2.cvtColor(small_frame, cv2.COLOR_BGR2RGB)

    face_locations = face_recognition.face_locations(rgb_frame)
    face_encodings = face_recognition.face_encodings(rgb_frame, face_locations)

    if not face_encodings:
        print("⚠️ No faces detected in this frame.")

    for face_encoding, face_location in zip(face_encodings, face_locations):
        name = "Unknown"
        if len(known_faces) > 0:
            face_distances = face_recognition.face_distance(known_faces, face_encoding)
            best_match_index = np.argmin(face_distances)

            if face_distances[best_match_index] < 0.5:  # Lower threshold to avoid false matches
                name = known_names[best_match_index]
                print(f"🔍 Detected face name: {name}")

                # ✅ Extract `IdEmploye` directly from the name (moe_8 → 8)
                employee_id = get_employee_id_from_name(name)

                if not employee_id:
                    print(f"⚠️ Warning: Could not extract valid employee ID from '{name}'")
                    continue  # Skip if no valid ID

                send_pointage(employee_id)  # ✅ Now using IdEmploye instead of IdUtilisateur
            else:
                print(f"⚠️ Warning: No face match found for detected person.")

        top, right, bottom, left = [coord * 2 for coord in face_location]
        cv2.rectangle(frame, (left, top), (right, bottom), (0, 255, 0), 2)
        cv2.putText(frame, name, (left, top - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.9, (0, 255, 0), 2)

    cv2.imshow("Facial Recognition", frame)

    if cv2.waitKey(1) & 0xFF == ord("q"):
        print("🔴 Exiting facial recognition system...")
        break

    time.sleep(0.1)

video_capture.release()
cv2.destroyAllWindows()
print("✅ Program terminated successfully.")
