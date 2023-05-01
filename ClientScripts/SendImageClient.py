import requests

# Set the URL of the endpoint
url = 'https://localhost:53736/Camera/updated-image'

# Replace <port> with the actual port number your server is running on

# Set the camera ID and image file path
camera_id = 1
image_path = 'image.jpeg'

# Read the image file as binary data
with open(image_path, 'rb') as f:
    image_data = f.read()

# Send the POST request with the image data and camera ID as parameters
response = requests.post(
    url, data={'image': image_data, 'cameraId': camera_id})

# Check the response status code
if response.status_code == 200:
    print('Image uploaded successfully!')
else:
    print(f'Error uploading image: {response.status_code} {response.reason}')
