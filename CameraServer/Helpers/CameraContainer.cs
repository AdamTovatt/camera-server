using CameraServer.Helpers.ImageProviding;

namespace CameraServer.Helpers
{
    public class CameraContainer
    {
        private static Dictionary<int, ICamera> container = new Dictionary<int, ICamera>();

        public async Task<ICamera> GetImage(int id)
        {
            await Task.CompletedTask;
            return container[id];
        }

        public async Task<bool> ContainsKey(int id)
        {
            await Task.CompletedTask;
            return container.ContainsKey(id);
        }

        public async Task SetImage(int id, byte[] image)
        {
            if (container.TryGetValue(id, out ICamera? camera))
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
