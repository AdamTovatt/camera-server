using CameraServer.Helpers.ImageProviding;

namespace CameraServer.Helpers
{
    public class CameraContainer
    {
        private Dictionary<int, ICamera> container = new Dictionary<int, ICamera>();

        public async void AddImage(int id, Camera provider)
        {
            if (container.ContainsKey(id))
                container[id] = provider;

            else
                container.Add(id, provider);
            await Task.CompletedTask;
        }

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
    }
}
