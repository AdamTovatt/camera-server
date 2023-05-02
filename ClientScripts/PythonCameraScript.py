import cv2
import requests
import numpy as np
import signal
import sys
import time as time

# url of the endpoint to send the image to
url = 'http://localhost:5018/camera/update-image'

camera_id = 1  # id of the camera, sent to the server with the image

cap = cv2.VideoCapture(1)  # the radxa rock pi seems to need to use id 1


def signal_handler(sig, frame):  # stop the program if systemctl says so
    print('Stopping program...')
    cap.release()
    cv2.destroyAllWindows()
    sys.exit(0)


# register signal handler to stop the program if systemctl says so
signal.signal(signal.SIGTERM, signal_handler)

while True:
    ret, frame = cap.read()

    _, img_encoded = cv2.imencode('.jpg', frame)  # encode the image as a jpg

    data = {'cameraId': camera_id}  # add the camera ID to the request data

    # add the image to the request files
    files = {'image': ('image.jpg', img_encoded.tobytes(), 'image/jpeg')}

    # Send the image and camera ID to the endpoint using a POST request
    try:
        response = requests.post(url, data=data, files=files, verify=False)
        print(response.text)
    except:
        print('Error sending image to server')

    time.sleep(0.5)
