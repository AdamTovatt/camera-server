# Camera Server
## What is this?
This is a web api for getting images from cameras. The cameras send their images to the server which stores them in memory. The server then provides endpoints for recieving the images. This way, the server is the connection between a front end and the cameras enabling front end applications to view the images of different cameras.

This repository also contains a directory called "ClientScripts" which contains scripts that can be run on the camera clients. These scripts are written in python and are used to send images to the server using OpenCV. Please note that these specific scripts are not required to use the server. Any client that can send images to the server in the correct way can be used, but these scripts are provided as an example.

## How is this made?
This back end is written in C# using the .NET 6 framework and ASP.NET core. The example camera scripts are written in python.

## Requirements

- [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
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
