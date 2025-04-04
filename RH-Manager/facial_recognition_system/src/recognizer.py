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

# Load known faces
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

# Start webcam
video_capture = cv2.VideoCapture(0)
video_capture.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
video_capture.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

if not video_capture.isOpened():
    print("❌ Error: Cannot open webcam. Trying fallback method...")
    video_capture = cv2.VideoCapture(1)
    if not video_capture.isOpened():
        print("❌ Error: No available camera found.")
        exit()

print("✅ Camera initialized successfully.")

RECOGNITION_DELAY = 30  # seconds
recognized_today = {}
frame_skip = 3
frame_count = 0

def get_employee_id_from_name(name):
    try:
        parts = name.split("_")
        if len(parts) == 2 and parts[1].isdigit():
            return int(parts[1])
    except ValueError:
        return None
    return None

def send_pointage(employee_id):
    """Send attendance and avoid spamming within delay period."""
    now = time.time()
    if employee_id in recognized_today and (now - recognized_today[employee_id]) < RECOGNITION_DELAY:
        print(f"⚠️ Employee {employee_id} was detected recently. Skipping duplicate entry.")
        return

    timestamp = datetime.now().replace(second=0, microsecond=0).isoformat()
    data = {
        "IdEmploye": employee_id,
        "HeureEntree": timestamp
    }

    print(f"📡 Sending API Request: {data}")

    try:
        response = requests.post(API_URL, json=data)
        print(f"🛠 API Response: {response.status_code} - {response.text}")

        if response.status_code == 200:
            print(f"✅ Attendance recorded for Employee ID {employee_id}")
            recognized_today[employee_id] = now
        else:
            print(f"⚠️ API Error {response.status_code}: {response.text}")

    except requests.exceptions.RequestException as e:
        print(f"❌ API Error: {e}")

# 🧠 Main loop
while True:
    ret, frame = video_capture.read()
    if not ret:
        print("❌ Error: Unable to capture frame.")
        continue

    frame_count += 1
    if frame_count % frame_skip != 0:
        continue

    small_frame = cv2.resize(frame, (0, 0), fx=0.5, fy=0.5)
    rgb_frame = cv2.cvtColor(small_frame, cv2.COLOR_BGR2RGB)

    face_locations = face_recognition.face_locations(rgb_frame)
    face_encodings = face_recognition.face_encodings(rgb_frame, face_locations)

    for face_encoding, face_location in zip(face_encodings, face_locations):
        name = "Unknown"
        if len(known_faces) > 0:
            face_distances = face_recognition.face_distance(known_faces, face_encoding)
            best_match_index = np.argmin(face_distances)

            if face_distances[best_match_index] < 0.5:
                name = known_names[best_match_index]
                print(f"🔍 Detected: {name}")
                employee_id = get_employee_id_from_name(name)

                if employee_id:
                    send_pointage(employee_id)
                else:
                    print(f"⚠️ Could not extract ID from {name}")
            else:
                print("⚠️ No match found.")

    # 🖼 Show video
    cv2.imshow("Facial Recognition", frame)

    # ⏹ Stop manually
    if cv2.waitKey(1) & 0xFF == ord("q"):
        print("🔴 Exiting system...")
        break

    time.sleep(0.1)

video_capture.release()
cv2.destroyAllWindows()
print("✅ Program terminated.")
