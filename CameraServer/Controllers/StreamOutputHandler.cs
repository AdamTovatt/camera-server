using CameraServer.Models;
using CameraServer.Repositories;
using Microsoft.AspNet.SignalR.WebSockets;
using System;
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
            Camera? camera = null;

            Camera.ImageUpdated broadCastNewImage = async (object sender, ArraySegment<byte> bytes) =>
            {
                await socket.SendAsync(bytes, WebSocketMessageType.Binary, true, CancellationToken.None);
            };

            try
            {
                OutputClientInfo? clientInfo = await OutputClientInfo.ReceiveAsync(socket);

                if (clientInfo == null || !clientInfo.IsValid) // ensure that we got a valid client info
                {
                    await socket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid client info", CancellationToken.None);
                    return;
                }

                if (!CameraContainer.Instance.TryGetCamera(clientInfo.CameraId, out camera) || camera == null) // ensure that the camera exists
                {
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, $"No camera with id {clientInfo.CameraId} exists", CancellationToken.None);
                    return;
                }

                if (!camera.UserIsAllowed(clientInfo.Token!)) // ensure that the user is allowed for this camera
                {
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, $"No camera with id {clientInfo.CameraId} exists", CancellationToken.None);
                    return;
                }

                camera.OnImageUpdated += broadCastNewImage;

                byte[] byteBuffer = new byte[1024]; // 100 KB buffer

                while (true) // this loop is constantly waiting for messages from the front end to send to the camera
                {
                    ArraySegment<byte> bufferSegment = new ArraySegment<byte>(byteBuffer);

                    if (socket.CloseStatus.HasValue) // break immediately if the socket has been closed
                        break;

                    if (bufferSegment.Count == 0 || bufferSegment.Array == null)
                        continue; // if we didn't get any data, skip this "message" (that was empty anyway) and wait for the next one

                    MessageToCameraClient messageToCameraClient = MessageToCameraClient.FromBytes(bufferSegment.Array);

                    if (messageToCameraClient.Type != MessageToCameraClient.MessageType.Invalid)
                        await camera.SendMessageToCameraClient(messageToCameraClient);
                }

                await socket.CloseAsync(socket.CloseStatus.Value, socket.CloseStatusDescription, CancellationToken.None);
            }
            catch
            {
                throw;
            }
            finally // we want to make sure we always unsubscribe from the camera's OnImageUpdated event when we're done
            {
                if (camera != null)
                    camera.OnImageUpdated -= broadCastNewImage;
            }
        }
    }
}
