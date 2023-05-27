using CameraServer.Models;

namespace CameraServerTests.Tests
{
    [TestClass]
    public class CameraTests
    {
        private Camera camera = new Camera(new CameraInformation(1, "test", "test", DateTime.Now));
    }
}
