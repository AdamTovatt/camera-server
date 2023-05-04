namespace CameraServer.Helpers
{
    public class CameraInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Preview { get; set; }

        public CameraInformation(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
