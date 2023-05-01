import cv2
import requests
import numpy as np
import signal
import sys

# Define the URL of the endpoint where you want to send the image
url = 'http://example.com/upload_image'

# Define the ID of the camera
camera_id = 'camera1'

# Set up the camera
cap = cv2.VideoCapture(0)

# Define a signal handler for SIGTERM


def signal_handler(sig, frame):
    print('Stopping program...')
    cap.release()
    cv2.destroyAllWindows()
    sys.exit(0)


# Register the signal handler
signal.signal(signal.SIGTERM, signal_handler)

while True:
    # Capture an image from the camera

    ret, frame = cap.read()

    # Convert the image to a JPEG-encoded bytes object
    _, img_encoded = cv2.imencode('.jpg', frame)
    headers = {'Content-Type': 'multipart/form-data'}

    # Create a dictionary to hold the request data
    data = {'camera_id': camera_id}

    # Add the image data to the request data
    files = {'image': ('image.jpg', img_encoded.tobytes(), 'image/jpeg')}

    # Send the image and camera ID to the endpoint using a POST request
    response = requests.post(url, headers=headers, data=data, files=files)

    # Print the response from the server
    print(response.text)

    # Delay for 1 second before capturing the next image
    cv2.waitKey(1000)

cap.release()
cv2.destroyAllWindows()
