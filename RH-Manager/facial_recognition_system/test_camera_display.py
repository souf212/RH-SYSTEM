import cv2

cap = cv2.VideoCapture(0)  # Use camera index 0

if not cap.isOpened():
    print("❌ Error: Camera not detected.")
    exit()

while True:
    ret, frame = cap.read()
    if not ret:
        print("❌ Error: Unable to read frame.")
        break

    cv2.imshow("Camera Test", frame)  # Show the captured frame

    if cv2.waitKey(1) & 0xFF == ord('q'):  # Press 'q' to exit
        break

cap.release()
cv2.destroyAllWindows()

