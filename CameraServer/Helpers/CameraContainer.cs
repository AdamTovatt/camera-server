using CameraServer.Helpers.ImageProviding;

namespace CameraServer.Helpers
{
    public class ImageCache
    {
        private Dictionary<int, IImageProvider> cache = new Dictionary<int, IImageProvider>();

        public async void AddImage(int id, ImageProvider provider)
        {
            if(cache.ContainsKey(id))
            {
                cache[id] = provider;
            }

            else
                cache.Add(id, provider);
            await Task.CompletedTask;
        }

        public async Task<IImageProvider> GetImage(int id) 
        {   
            await Task.CompletedTask;
            return cache[id];
        }
    }
}
