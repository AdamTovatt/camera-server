import cv2
import websocket
import struct
import time

running = True
cap = None
ws = None

while running:
    try:
        # Connect to the server using WebSocket (Important note! The port is 5000, not 5001! It doesn't work with 5001, I don't know why)
        ws = websocket.create_connection(
            "ws://localhost:5000/video-input-stream")
        # Send the camera ID to the server
        ws.send_binary(struct.pack('<I', 1337))

        # Open the camera
        cap = cv2.VideoCapture(0)

        # Capture and encode the video
        while running:
            ret, frame = cap.read()
            if not ret:
                break

            # Encode the frame as JPG (I think H.264 could yield performance improvements but I couldn't get the encoding to work)
            encoded, buffer = cv2.imencode('.jpg', frame)
            data = buffer.tobytes()

            # Send the video chunk to the server
            ws.send_binary(struct.pack('<I', len(data)) + data)

            # Wait a little bit to prevent flooding the server
            time.sleep(0.01)
    except ConnectionResetError:
        print(
            "The established connection to the server was lost, will attempt to reconnect.")
        cap.release()  # Release the camera resource since we don't know if we will be able to reconnect
    except ConnectionRefusedError:
        print("Could not establish a new connection to the server, retrying after 5 seconds...")
        time.sleep(5)

# Release the resources
if (cap is not None):
    cap.release()

# Close the WebSocket connection
if (ws is not None):
    ws.close()
