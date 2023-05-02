import cv2
import requests
import numpy as np
import signal
import sys
import time as time

try:
    # Open the text file in read mode
    with open('camera-config.txt', 'r') as file:
        # Read the lines from the text file
        lines = file.readlines()

        # Extract the values of url and camera_id from the lines
        url = lines[0].strip()
        camera_id = int(lines[1].strip())
        hardware_id = int(lines[2].strip())
except FileNotFoundError:
    print('Error! No config file found.')
    print('Should be a config file called camera-config.txt in the same directory as this script.')
    print('The config file should have the url on the first line, the camera ID on the second line and the hardware ID on the third line.')
    print("The url is the url of the endpoint that the image will be sent to.")
    print('The camera ID is the ID of the camera that will be sent with the image, it represents what camera this is, at what place it is, whose camera it is.')
    print('The hardware ID is what camera the program will use to capture the image, should probably be 0 for most devices, 1 for the radxa rock pi.')
    sys.exit(0)
except PermissionError:
    print('Error! No permission to read config file.')
    sys.exit(0)
except:
    print('Unknown error reading config file.')
    sys.exit(0)

    # the radxa rock pi seems to need to use id 1
cap = cv2.VideoCapture(hardware_id)


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
