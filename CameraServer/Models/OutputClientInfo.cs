using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace CameraServer.Models
{
    public class OutputClientInfo
    {
        [JsonProperty("cameraId")]
        public int CameraId { get; set; }

        [JsonProperty("token")]
        public string? Token { get; set; }

        public bool IsValid => CameraId > 0 && !string.IsNullOrEmpty(Token);

        public OutputClientInfo() { }

        public OutputClientInfo(int cameraId, string token)
        {
            CameraId = cameraId;
            Token = token;
        }

        public static OutputClientInfo? FromBytes(byte[] bytes)
        {
            if(bytes == null || bytes.Length == 0) return null;

            string json = Encoding.UTF8.GetString(bytes);

            return JsonConvert.DeserializeObject<OutputClientInfo>(json);
        }

        public static async Task<OutputClientInfo> ReceiveAsync(WebSocket socket)
        {
            byte[] cameraIdBytes = new byte[4];
            result = await socket.ReceiveAsync(new ArraySegment<byte>(cameraIdBytes), CancellationToken.None);

            int cameraId = BitConverter.ToInt32(cameraIdBytes, 0);

            if (cameraId == 7)
            {
                useBase64 = true;
                cameraId = 1;
            }

            if (!CameraContainer.Instance.TryGetCamera(cameraId, out camera)) // close the stream if we don't get a valid camera id
            {
                await socket.CloseOutputAsync(WebSocketCloseStatus.PolicyViolation, $"No camera with id {cameraId} exists", CancellationToken.None);
            }
        }
    }
}
