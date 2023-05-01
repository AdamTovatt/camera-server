using CameraServer.Helpers.ImageProviding;
using CameraServer.Models;

namespace CameraServer.Helpers
{
    public class CameraContainer
    {
        private static Dictionary<int, ICamera>? container;

        private static CameraContainer? _instance;

        public static CameraContainer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CameraContainer();
                return _instance;
            }
        }

        public CameraContainer()
        {
            if (container == null)
                container = new Dictionary<int, ICamera>();
        }

        public async Task<ICamera> GetCamera(int id)
        {
            await Task.CompletedTask;
            return container![id];
        }

        public async Task<bool> ContainsKey(int id)
        {
            await Task.CompletedTask;
            return container!.ContainsKey(id);
        }

        public async Task SetImage(int id, CameraImage image)
        {
            if (container!.TryGetValue(id, out ICamera? camera))
            {
                await camera.SetImage(image);
            }
            else
            {
                ICamera newCamera = new Camera();
                await newCamera.SetImage(image);
                container.Add(id, newCamera);
            }
        }
    }
}
