import cv2
import requests
import numpy as np
import signal
import sys
import time as time
import datetime

try:
    # Open the text file in read mode
    with open('camera-config.txt', 'r') as file:
        # Read the lines from the text file
        lines = file.readlines()

        # Extract the values of url and camera_id from the lines
        url = lines[0].strip()
        camera_id = int(lines[1].strip())
        hardware_id = int(lines[2].strip())
        delay = float(lines[3].strip())
except FileNotFoundError:
    print('Error! No config file found.')
    print('Should be a config file called camera-config.txt in the same directory as this script.')
    print('The config file should contain: url, camera ID, hardware ID, delay. On seperate lines.')
    print("The url is the url of the endpoint that the image will be sent to.")
    print('The camera ID is the ID of the camera that will be sent with the image, it represents what camera this is, at what place it is, whose camera it is.')
    print('The hardware ID is what camera the program will use to capture the image, should probably be 0 for most devices, 1 for the radxa rock pi.')
    print('The delay is how long the program will wait between sending images, in seconds.')
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
hasSentImage = False

while True:
    ret, frame = cap.read()

    # Add current date and time to the bottom right corner of the image
    font = cv2.FONT_HERSHEY_SIMPLEX
    # coordinates of the bottom right corner
    bottom_right_corner = (frame.shape[1]-230, frame.shape[0]-10)
    current_time = datetime.datetime.utcnow().strftime(
        "%Y-%m-%d %H:%M:%S UTC")  # current date and time
    cv2.putText(frame, current_time, bottom_right_corner,
                font, 0.5, (255, 255, 255), 2, cv2.LINE_AA)

    _, img_encoded = cv2.imencode('.jpg', frame)  # encode the image as a jpg

    data = {'cameraId': camera_id}  # add the camera ID to the request data

    # add the image to the request files
    files = {'image': ('image.jpg', img_encoded.tobytes(), 'image/jpeg')}

    # Send the image and camera ID to the endpoint using a POST request
    try:
        response = requests.post(url, data=data, files=files, verify=False)
        if (not hasSentImage):
            print("First image sent sucessfully, will continue to send images")
            hasSentImage = True
    except:
        print('Error sending image to server')

    time.sleep(delay)
