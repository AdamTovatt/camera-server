using CameraServer.Models;
using CameraServer.Repositories;
using Microsoft.AspNet.SignalR.WebSockets;
using System.Net.WebSockets;
using System.Text;

namespace CameraServer.Controllers
{
    public class StreamOutputHandler : WebSocketHandler
    {
        private readonly RequestDelegate nextDelegate;

        public StreamOutputHandler(RequestDelegate nextDelegate) : base(null)
        {
            this.nextDelegate = nextDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (!CameraContainer.Instance.IsInitialized)
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
            WebSocketReceiveResult? result;
            Camera? camera = null;
            long previousTimeOfCapture = 0;

            do
            {
                if (camera == null) // Receive the camera id
                {
                    byte[] cameraId = new byte[4];
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(cameraId), CancellationToken.None);

                    if (CameraContainer.Instance.TryGetCamera(BitConverter.ToInt32(cameraId, 0), out camera))
                    {
                        if (camera != null)
                            camera.SetConnection(socket);
                    }
                    else // close the stream if we don't get a valid camera id
                    {
                        await socket.CloseOutputAsync(WebSocketCloseStatus.PolicyViolation, $"No camera with id {BitConverter.ToInt32(cameraId, 0)} exists", CancellationToken.None);
                    }
                }
                else // send the image data
                {
                    byte[] bytes = await camera.GetNextImageBytesAsync(previousTimeOfCapture);
                    previousTimeOfCapture = camera.LastTimeOfCapture;
                    await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
                }
            }
            while (!socket.CloseStatus.HasValue);

            await socket.CloseAsync(socket.CloseStatus.Value, socket.CloseStatusDescription, CancellationToken.None);
        }
    }
}
