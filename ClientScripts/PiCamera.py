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

camera = PiCamera()
stream = io.BytesIO()


def signal_handler(sig, frame):  # stop the program if systemctl says so
    print('Stopping program...')
    camera.close()
    sys.exit(0)


# register signal handler to stop the program if systemctl says so
signal.signal(signal.SIGTERM, signal_handler)
hasSentImage = False

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
        if (not hasSentImage):
            print("First image sent sucessfully, will continue to send images")
            hasSentImage = True
    except:
        print('Error sending image to server')

    time.sleep(delay)
