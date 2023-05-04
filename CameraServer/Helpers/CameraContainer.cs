using CameraServer.Helpers.ImageProviding;
using CameraServer.Models;
using CameraServer.Repositories;

namespace CameraServer.Helpers
{
    public class CameraContainer
    {
        private static Dictionary<int, ICamera>? container;

        private static CameraContainer? _instance;

        public int CameraCount { get { return container?.Count ?? 0; } }

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

        public void Clear()
        {
            if (container != null)
                container.Clear();
        }

        public async Task<ICamera> GetCameraAsync(int id)
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
                ICamera newCamera = new Camera(CameraInformationRepository.Instance.GetCameraInformationById(id));
                await newCamera.SetImage(image);
                container.Add(id, newCamera);
            }
        }

        public List<CameraInformation> GetCameraList()
        {
            List<CameraInformation> result = new List<CameraInformation>();

            if (container == null)
                return result;

            foreach (KeyValuePair<int, ICamera> pair in container)
            {
                result.Add(pair.Value.GetInformation());
            }

            return result;
        }
    }
}
