﻿using CameraServer.Helpers;
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

        public static async Task<OutputClientInfo?> ReceiveAsync(WebSocket socket)
        {
            int messageLength = await socket.ReceiveIntAsync();

            if (messageLength == 0 || messageLength > 1024)
                return null;

            byte[] messageBytes = new byte[messageLength];
            await socket.ReceiveAsync(messageBytes, CancellationToken.None);

            return FromBytes(messageBytes);
        }
    }
}
