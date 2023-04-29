using CameraServer.Helpers.ImageProviding;

namespace CameraServer.Helpers
{
    public class CameraContainer
    {
        private Dictionary<int, ICamera> cache = new Dictionary<int, ICamera>();

        public async void AddImage(int id, Camera provider)
        {
            if(cache.ContainsKey(id))
            {
                cache[id] = provider;
            }

            else
                cache.Add(id, provider);
            await Task.CompletedTask;
        }

        public async Task<ICamera> GetImage(int id) 
        {   
            await Task.CompletedTask;
            return cache[id];
        }
    }
}
