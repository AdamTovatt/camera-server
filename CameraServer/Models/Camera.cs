using CameraServer.Helpers;
using System.Net.WebSockets;
using OpenCvSharp;
using CameraServer.Providers;

namespace CameraServer.Models
{
    public class Camera
    {
        public CameraInformation Information { get { return information; } }
        public bool IsConnected { get { return socketConnection != null && !socketConnection.CloseStatus.HasValue; } }

        private CameraImage currentImage;
        private CameraInformation information;
        private WebSocket? socketConnection;
        private byte[]? queuedData;

        private Movement queuedMovement;
        private CascadeClassifier classifier;

        private Rect[] faces = new Rect[0];
        private int counter = 0;

        public Camera(CameraInformation information)
        {
            this.information = information;
            currentImage = new CameraImage(new byte[0], DateTime.MinValue);
            queuedMovement = new Movement();

            classifier = CascadeProvider.CascadeClassifier;
        }

        public void SetImage(byte[] bytes)
        {
            currentImage.Bytes = bytes;
            DateTime time = DateTime.UtcNow;
            currentImage.TimeOfCapture = time;
            information.LastActive = time;

            using (Mat imageMat = Cv2.ImDecode(bytes, ImreadModes.Color))
            {
                using (Mat grayMat = new Mat())
                {
                    if (counter >= 25)
                    {
                        Cv2.CvtColor(imageMat, grayMat, ColorConversionCodes.BGR2GRAY);

                        // Detect faces in the grayscale image
                        faces = classifier.DetectMultiScale(grayMat);

                        // Draw rectangles around the detected faces
                        foreach (Rect faceRect in faces)
                            Cv2.Rectangle(imageMat, faceRect, Scalar.Blue, 2);

                        counter = 0;

                        // Convert the image back to byte[] format
                        byte[] processedImageData = imageMat.ToBytes(".jpg");
                        currentImage.Bytes = processedImageData;
                    }
                    else
                    {
                        foreach (Rect faceRect in faces)
                            Cv2.Rectangle(imageMat, faceRect, Scalar.Blue, 2);

                        // Convert the image back to byte[] format
                        byte[] processedImageData = imageMat.ToBytes(".jpg");
                        currentImage.Bytes = processedImageData;
                        counter++;
                    }
                }
            }
        }

        public async Task<CameraImage> GetImageAsync()
        {
            if (currentImage == null)
                throw new Exception("no image available");

            await Task.CompletedTask;
            return currentImage;
        }

        public bool IsValidToken(string token)
        {
            if (information == null || information.Token == null)
                return false;

            return TokenHelper.GetToken(information.Id) == information.Token;
        }

        public async Task SetPreviewAsync()
        {
            Information.Preview = Convert.ToBase64String((await GetImageAsync()).Bytes);
        }

        public void SetConnection(WebSocket webSocket)
        {
            socketConnection = webSocket;
        }

        public void QueueBytes(byte[] bytes)
        {
            queuedData = bytes;
            queuedMovement.Clear();
        }

        public void Move(int deltaPitch, int deltaYaw)
        {
            queuedMovement.DeltaPitch += deltaPitch;
            queuedMovement.DeltaYaw += deltaYaw;
            queuedMovement.ContainsValue = true;
        }

        private async Task<bool> SendBytesAsync(byte[] bytes)
        {
            if (!IsConnected)
                return false;

            await socketConnection!.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);

            return true;
        }

        public async Task<bool> SubmitResponse()
        {
            if (queuedData != null)
            {
                bool result = await SendBytesAsync(queuedData);
                queuedData = null;
                return result;
            }

            if (queuedMovement.ContainsValue)
            {
                queuedData = queuedMovement.GetBytes();
                queuedMovement.Clear();
                bool result = await SendBytesAsync(queuedData);
                queuedData = null;
                return result;
            }

            return await SendBytesAsync(new byte[0]);
        }
    }
}
