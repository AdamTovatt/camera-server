using CameraServer.Helpers.ImageProviding;

namespace CameraServer.Helpers
{
    public class CameraContainer
    {
        private Dictionary<int, ICamera> Cache = new Dictionary<int, ICamera>();

        public async void AddImage(int id, Camera provider)
        {
            if(Cache.ContainsKey(id))
            {
                Cache[id] = provider;
            }

            else
                Cache.Add(id, provider);
            await Task.CompletedTask;
        }

        public async Task<ICamera> GetImage(int id) 
        {   
            await Task.CompletedTask;
            return Cache[id];
        }
    }
}
