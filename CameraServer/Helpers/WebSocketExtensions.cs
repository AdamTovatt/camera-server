using System.Net.WebSockets;

namespace CameraServer.Helpers
{
    public static class WebSocketExtensions
    {
        public static async Task<int> ReceiveIntAsync(this WebSocket socket)
        {
            byte[] intBytes = new byte[4];
            await socket.ReceiveAsync(new ArraySegment<byte>(intBytes), CancellationToken.None);
            return BitConverter.ToInt32(intBytes, 0);
        }
    }
}
