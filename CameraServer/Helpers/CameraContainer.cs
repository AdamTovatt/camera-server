using CameraServer.Models;
using CameraServer.Repositories;

namespace CameraServer.Helpers
{
    public class CameraContainer
    {
        private static Dictionary<int, Camera>? container;

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
                container = new Dictionary<int, Camera>();
        }

        public async Task LoadFromRepository(ICameraRepository cameraRepository)
        {
            if(container == null)
                container = new Dictionary<int, Camera>();

            foreach (CameraInformation info in await cameraRepository.GetAllCameraInformationsAsync())
            {
                container.Add(info.Id, new Camera(info));
            }
        }

        public void Clear()
        {
            if (container != null)
                container.Clear();
        }

        public bool TryGetCamera(int cameraId, out Camera? camera)
        {
            return container!.TryGetValue(cameraId, out camera);
        }

        public async Task<Camera> GetCameraAsync(int id)
        {
            await Task.CompletedTask;
            return container![id];
        }

        public async Task<bool> ContainsKey(int id)
        {
            await Task.CompletedTask;
            return container!.ContainsKey(id);
        }

        public List<CameraInformation> GetCameraList()
        {
            List<CameraInformation> result = new List<CameraInformation>();

            if (container == null)
                return result;

            foreach (KeyValuePair<int, Camera> pair in container)
            {
                result.Add(pair.Value.GetInformation());
            }

            return result;
        }
    }
}
