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
            bool useBase64 = false;

            OutputClientInfo? clientInfo = await OutputClientInfo.ReceiveAsync(socket);

            do
            {
                    byte[] bytes = await camera.GetNextImageBytesAsync(previousTimeOfCapture);

                    if(useBase64)
                        bytes = UTF8Encoding.UTF8.GetBytes(Convert.ToBase64String(bytes));

                    previousTimeOfCapture = camera.LastTimeOfCapture;
                    await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
            while (!socket.CloseStatus.HasValue);

            await socket.CloseAsync(socket.CloseStatus.Value, socket.CloseStatusDescription, CancellationToken.None);
        }

        private 
    }
}
