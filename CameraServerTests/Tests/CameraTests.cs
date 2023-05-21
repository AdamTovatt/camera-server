using CameraServer.Models;

namespace CameraServerTests.Tests
{
    [TestClass]
    public class CameraTests
    {
        private Camera camera = new Camera(new CameraInformation(1, "test", "test", DateTime.Now));

        const int loopLength = 10;
        const float differenceThreshold = 0.1f;

        [TestMethod]
        public void Move()
        {
            Random random = new Random();

            DateTime start = DateTime.UtcNow;

            for (int i = 0; i < loopLength; i++)
            {
                camera.Move(0.8f, 0.2f);
                Thread.Sleep(random.Next(100));
            }

            for (int i = 0; i < loopLength; i++)
            {
                camera.Move(0.8f, 0.2f);
                Thread.Sleep(2);
            }

            for (int i = 0; i < loopLength; i++)
            {
                camera.Move(0.8f, 0.2f);
                Thread.Sleep(20);
            }

            DateTime stop = DateTime.UtcNow;
            float time = (float)(stop - start).TotalSeconds;

            float difference = (time * 0.8f - camera.QueuedMovement.Pitch);
            Assert.IsTrue(difference < differenceThreshold, $"{difference} is not withing threshold of {differenceThreshold}");
        }
    }
}
