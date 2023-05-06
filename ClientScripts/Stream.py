import websocket

ws = websocket.create_connection("ws://localhost:5000/video-input-stream")

ws.send_binary(b"Hello, World!")
