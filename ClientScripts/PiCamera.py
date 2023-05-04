from picamera import PiCamera
import requests
import io
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
except FileNotFoundError:
    print('Error! No config file found.')
    print('Should be a config file called camera-config.txt in the same directory as this script.')
    print('The config file should have the url on the first line and the camera ID on the second line.')
    print("The url is the url of the endpoint that the image will be sent to.")
    print('The camera ID is the ID of the camera that will be sent with the image, it represents what camera this is, at what place it is, whose camera it is.')
    sys.exit(0)
except PermissionError:
    print('Error! No permission to read config file.')
    sys.exit(0)
except:
    print('Unknown error reading config file.')
    sys.exit(0)

camera = PiCamera()
stream = io.BytesIO()


def signal_handler(sig, frame):  # stop the program if systemctl says so
    print('Stopping program...')
    camera.close()
    sys.exit(0)


# register signal handler to stop the program if systemctl says so
signal.signal(signal.SIGTERM, signal_handler)

while True:
    # Clear the stream before capturing the next image
    stream.seek(0)
    stream.truncate()

    # Capture an image from the camera to the stream
    camera.capture(stream, format='jpeg')
    stream.seek(0)

    # Prepare the request data and files
    data = {'cameraId': camera_id}  # add the camera ID to the request data
    files = {'image': ('image.jpg', stream.getvalue(), 'image/jpeg')}

    # Send the image and camera ID to the endpoint using a POST request
    try:
        response = requests.post(url, data=data, files=files, verify=False)
        print(response.text)
    except:
        print('Error sending image to server')

    time.sleep(0.5)
