using CameraServer.Helpers;
using CameraServer.Repositories;
using Microsoft.AspNet.SignalR.WebSockets;
using System.Net.WebSockets;
using System.Text;

namespace CameraServer.Controllers
{
    public class StreamInputHandler : WebSocketHandler
    {
        private readonly RequestDelegate nextDelegate;

        public StreamInputHandler(RequestDelegate nextDelegate) : base(null)
        {
            this.nextDelegate = nextDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if(!CameraContainer.Instance.IsInitialized)
                    await CameraContainer.Instance.InitializeFromRepository(CameraRepository.Instance);

                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await HandleWebSocket(webSocket);
                }
                else
                {
                    await nextDelegate(context);
                }
            }
            catch (Exception exception)
            {
                throw; // We should probably do something with the exception here...
            }
        }

        private async Task HandleWebSocket(WebSocket socket)
        {
            WebSocketReceiveResult? result = null;
            Camera? camera = null;
            string? token = null;

            do
            {
                if (camera == null) // Receive the camera id
                {
                    byte[] cameraId = new byte[4];
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(cameraId), CancellationToken.None);

                    if (!CameraContainer.Instance.TryGetCamera(BitConverter.ToInt32(cameraId, 0), out camera)) // Close the stream if we don't get a valid camera id
                        await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, $"No camera with id {BitConverter.ToInt32(cameraId, 0)} exists", CancellationToken.None);
                }
                else if(token == null) // Receive the token
                {
                    byte[] sizeBuffer = new byte[4];
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(sizeBuffer), CancellationToken.None);
                    int dataSize = BitConverter.ToInt32(sizeBuffer, 0);

                    byte[] data = new byte[dataSize];
                    int remainingBytes = dataSize;
                    while (remainingBytes > 0)
                    {
                        byte[] buffer = new byte[remainingBytes];
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        Buffer.BlockCopy(buffer, 0, data, dataSize - remainingBytes, result.Count);
                        remainingBytes -= result.Count;
                    }

                    token = Encoding.UTF8.GetString(data);

                    if (!camera.IsValidToken(token))
                        await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid token", CancellationToken.None);
                }
                else // Receive the image data
                {
                    // Receive the size of the image data
                    byte[] sizeBuffer = new byte[4];
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(sizeBuffer), CancellationToken.None);
                    int dataSize = BitConverter.ToInt32(sizeBuffer, 0);

                    // Receive the image data
                    byte[] data = new byte[dataSize];
                    int remainingBytes = dataSize;
                    while (remainingBytes > 0)
                    {
                        byte[] buffer = new byte[remainingBytes];
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        Buffer.BlockCopy(buffer, 0, data, dataSize - remainingBytes, result.Count);
                        remainingBytes -= result.Count;
                    }

                    if (data.Length > 0)
                        camera.SetImage(data);
                }
            }
            while (!result.CloseStatus.HasValue);

            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
