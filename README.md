# Camera Server
## What is this?
This is a web api for getting images from cameras.

## What technologies are used?
The backend is written in C# using ASP.NET Core and the client script is written in Python.

There is also a front end written in TypeScript using React with Styled Components. The repository for the front end can be found here: https://github.com/AdamTovatt/camera-frontend

As I'm writing this the front end is also live at https://keycard.kthsalar.se/ but I don't know if it will be when you are reading this.

## How does it all work?
The cameras send their images to the server via websockets, and the camera then stores them in memory. Since there are almost constantly new images, the server doesn't write all the images to some sort of disk or database but rather just keeps them in memory. It only keeps the latest image for each camera.

The server then provides endpoints for receiving the images, most notably the stream-image endpoint, which allows the front end to use an event stream to continually read from the stream that the server keeps open and sends bytes over. This way, the server is the connection between a front end and the cameras, enabling front-end applications to view the images of different cameras.

This repository also contains a directory called "ClientScripts," which contains a Python script that can be run on a client computer to send images to the server. The script uses OpenCV to capture images from the camera and then websockets to send them to the server.

In more detail, the camera client opens a websocket connection and starts by sending 4 bytes, which represent the ID of the camera. It then sends 4 more bytes, which represent the length of the token that will be sent. The camera then sends the bytes of the token string encoded with the UTF-8 encoding. When the server receives the first 4 bytes, it converts them to a 32-bit integer and checks if a camera with that ID exists. If it doesn't exist, it closes the stream. If it does exist, it reads the next 4 bytes and converts them to a 32-bit integer, which represents the length of the token. It then reads the next bytes by using the length and then converts them to a string. If the token is correct, it associates the current camera with the current stream and starts listening for images. If the token is incorrect, it closes the stream.

A little side note on the token: The token for the camera is created by hashing the camera ID together with a secret only known by the server. This way, the token can later be verified by the server by hashing the camera ID together with the secret and comparing it to the token sent by the camera. This is not a perfectly secure way of doing it, but it is good enough for this use case.

After sending the token, the client starts sending images to the server. Like with the token, the length of the images is sent in the first 4 bytes, and then the actual data is sent. This allows the server to read the first 4 bytes and then read the specified amount of bytes as an image.

For more specific information on how this is implemented, please see the code.

## How is this used?

This is currently running the server on a Raspberry Pi 4 B and the camera client on a Raspberry Pi Zero W.

## Requirements

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Visual Studio Code](https://code.visualstudio.com/) or any IDE of your choice

## Getting Started

1. Clone this repository: `git clone https://github.com/your-username/camera-server.git`
2. Open the project in your IDE
3. Build the project: `dotnet build`
4. Run the project: `dotnet run`

## Endpoints

### `GET /hello`

This endpoint returns a simple "Hello World!" message.

### `GET /list`

This endpoint returns a list of cameras available in the CameraContainer.

### `GET /mocked-image`

This endpoint returns a mocked image.

### `GET /image`

This endpoint returns an image from a local camera.

### `POST /update-image`

This endpoint updates the image of a camera with the given ID. The image should be sent as a form data with the name "image", and the camera ID should be sent as a form data with the name "cameraId".

### `GET /get-image`

This endpoint returns an image from the camera with the given ID.

### `GET /stream-image`

This endpoint streams the image from the camera with the given ID.

## Usage

To use this Camera Server, you can send HTTP requests to the various endpoints provided by the server.

For example, to get a list of cameras, you can send a GET request to `/list`.

## Contributing

Contributions are welcome! If you find any bugs or have any suggestions for new features, feel free to open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
