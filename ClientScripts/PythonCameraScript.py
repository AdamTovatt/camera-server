import cv2
import requests
import numpy as np
import signal
import sys
import time as time

# Define the URL of the endpoint where you want to send the image
url = 'http://localhost:5018/camera/update-image'

# Define the ID of the camera
camera_id = 1

# Set up the camera
cap = cv2.VideoCapture(1)

# Define a signal handler for SIGTERM


def signal_handler(sig, frame):
    print('Stopping program...')
    cap.release()
    cv2.destroyAllWindows()
    sys.exit(0)


# Register the signal handler
signal.signal(signal.SIGTERM, signal_handler)

while True:
    ret, frame = cap.read()

    # Convert the image to a JPEG-encoded bytes object
    _, img_encoded = cv2.imencode('.jpg', frame)

    # Create a dictionary to hold the request data
    data = {'cameraId': camera_id}

    # Add the image data to the request data
    files = {'image': ('image.jpg', img_encoded.tobytes(), 'image/jpeg')}

    # Send the image and camera ID to the endpoint using a POST request
    try:
        response = requests.post(url, data=data, files=files, verify=False)
        print(response.text)
    except:
        print('Error sending image to server')

    time.sleep(0.5)
