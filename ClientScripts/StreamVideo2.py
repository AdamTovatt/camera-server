import websocket
import struct
import time
import json
import os
import socket
import picamera
import io


class CameraConfig:
    def __init__(self, cameraId, cameraToken, webSocketEndpoint):
        self.cameraId = cameraId
        self.cameraToken = cameraToken
        self.webSocketEndpoint = webSocketEndpoint

    @staticmethod
    def readConfigFromFile(path):
        with open(path, 'r') as file:
            config_data = json.load(file)

        camera_id = config_data['cameraId']
        camera_token = config_data['cameraToken']
        backend_url = config_data['webSocketEndpoint']

        return CameraConfig(camera_id, camera_token, backend_url)


configPath = "camera-config.json"

running = True
cap = None
ws = None
config = None

if not os.path.exists(configPath):
    print("Error! No config file found.")
    print("Should be a config file called camera-config.json in the same directory as this script.")
    print("The config file should contain: cameraId, cameraToken, webSocketEndpoint.")
    running = False
else:
    try:
        config = CameraConfig.readConfigFromFile(configPath)
    except json.decoder.JSONDecodeError:
        print("Invalid json config file, please check the file and try again.")
        print("The config file should contain: cameraId, cameraToken, webSocketEndpoint.")
        running = False

while running:
    try:
        print("Connecting to the server at " + config.webSocketEndpoint)
        # Connect to the server using WebSocket (Important note! The port is 5000, not 5001! It doesn't work with 5001, I don't know why)
        ws = websocket.create_connection(config.webSocketEndpoint, timeout=8)

        print("Did connect to the server, sending camera ID")

        # Send the camera ID to the server
        ws.send_binary(struct.pack('<I', 1337))

        print("Did send camera ID, starting video capture")

        with picamera.PiCamera() as camera:
            camera.resolution = (640, 480)
            camera.framerate = 24

            with io.BytesIO() as stream:
                for _ in camera.capture_continuous(stream, format='jpeg', use_video_port=True):
                    # Do something with the stream
                    data = stream.getvalue()

                    # Send the video chunk to the server
                    ws.send_binary(struct.pack('<I', len(data)) + data)

                    # Reset the stream for the next capture
                    stream.seek(0)
                    stream.truncate()

                    time.sleep(0.1)

    except ConnectionResetError as error:
        print(
            "The established connection to the server was lost, will attempt to reconnect")
        print(error)
        cap.release()  # Release the camera resource since we don't know if we will be able to reconnect
    except ConnectionRefusedError as error:
        print(
            "Could not establish a new connection to the server, retrying after 5 seconds")
        print(error)
        time.sleep(5)
    except ConnectionAbortedError as error:
        print("The server closed the connection, will attempt to reconnect in 5 seconds")
        print(error)
        time.sleep(5)
    except socket.timeout:
        print("The connection timed out, will attempt to reconnect in 30 seconds")
        time.sleep(30)

# Release the resources
if (cap is not None):
    cap.release()

# Close the WebSocket connection
if (ws is not None):
    ws.close()
