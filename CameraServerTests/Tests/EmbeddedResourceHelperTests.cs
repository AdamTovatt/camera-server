using CameraServer.Helpers;

namespace CameraServerTests.Tests
{
    [TestClass]
    public class EmbeddedResourceHelperTests
    {
        [TestMethod]
        public async Task ReadsMockedCameraImage()
        {
            byte[] imageBytes = await EmbeddedResourceHelper.ReadResource("MockedCameraImage");

            Assert.IsNotNull(imageBytes);
            Assert.AreEqual(239172, imageBytes.Length); // check that the amount of bytes that were read is correct
        }
    }
}