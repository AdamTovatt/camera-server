namespace CameraServer.Helpers
{
    public class image_cache
    {
        private Dictionary<int, image_provider> _cache = new Dictionary<int, image_provider>();

        public void AddImage(int id, image_provider provider)
        {
            _cache.Add(id, provider);
        }

        public image_provider GetImage(int id) 
        {   
            return _cache[id];
        }
    }
}
