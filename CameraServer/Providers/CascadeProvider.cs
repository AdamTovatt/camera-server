using OpenCvSharp;

namespace CameraServer.Providers
{
    public class CascadeProvider
    {
        public static CascadeClassifier CascadeClassifier
        {
            get
            {
                if (_cascadeClassifier == null)
                    _cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
                return _cascadeClassifier;
            }
        }

        private static CascadeClassifier? _cascadeClassifier;
    }
}
