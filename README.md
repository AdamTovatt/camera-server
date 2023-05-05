# camera-server
Camera Server
The Camera Server is a back-end API that allows users to interact with camera devices and get images or video streams from them. This README provides documentation for the API endpoints and their usage.

Installation and Setup
Clone the repository from Github
Install the latest version of .NET Core SDK (https://dotnet.microsoft.com/download)
Run the following command in the terminal to restore dependencies:
Copy code
dotnet restore
Build the project using the following command:
Copy code
dotnet build
Run the project using the following command:
arduino
Copy code
dotnet run
Access the API using the following base URL:
arduino
Copy code
http://localhost:5000
API Endpoints
GET /camera/hello
This endpoint returns a simple greeting message.

Request
http
Copy code
GET /camera/hello HTTP/1.1
Response
http
Copy code
HTTP/1.1 200 OK
Content-Type: application/json

{
    "message": "Hello World!"
}
GET /camera/list
This endpoint returns a list of all available cameras.

Request
http
Copy code
GET /camera/list HTTP/1.1
Response
http
Copy code
HTTP/1.1 200 OK
Content-Type: application/json

{
    "cameras": [
        {
            "id": 1,
            "name": "Camera 1"
        },
        {
            "id": 2,
            "name": "Camera 2"
        }
    ]
}
GET /camera/mocked-image
This endpoint returns a mocked image.

Request
http
Copy code
GET /camera/mocked-image HTTP/1.1
Response
http
Copy code
HTTP/1.1 200 OK
Content-Type: image/jpeg
Content-Length: {length}

{binary data}
GET /camera/image
This endpoint returns an image from the specified camera.

Request
http
Copy code
GET /camera/image HTTP/1.1
Response
http
Copy code
HTTP/1.1 200 OK
Content-Type: image/jpeg
Content-Length: {length}

{binary data}
POST /camera/update-image
This endpoint allows users to update the image for a camera.

Request
http
Copy code
POST /camera/update-image HTTP/1.1
Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryyrV7KO0BoCBuDbTL

------WebKitFormBoundaryyrV7KO0BoCBuDbTL
Content-Disposition: form-data; name="image"; filename="image.jpg"
Content-Type: image/jpeg

{binary data}
------WebKitFormBoundaryyrV7KO0BoCBuDbTL
Content-Disposition: form-data; name="cameraId"

1
------WebKitFormBoundaryyrV7KO0BoCBuDbTL--
Response
http
Copy code
HTTP/1.1 200 OK
Content-Type: application/json

{
    "message": "Image updated successfully."
}
GET /camera/get-image
This endpoint returns the image for the specified camera.

Request
http
Copy code
GET /camera/get-image?cameraId=1 HTTP/1.1
Response
http
Copy code
HTTP/1.1 200 OK
Content-Type: image/jpeg
Content-Length: {length}

{binary data}
GET /camera/stream-image
This endpoint streams the image for the specified camera.

Request
http
Copy code
GET /camera/stream-image?cameraId=1&update
