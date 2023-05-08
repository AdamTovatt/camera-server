namespace CameraServer.Helpers
{
    public class CameraInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Token { get; set; }
        public DateTime LastActive { get; set; }

        public CameraInformation(int id, string name, string description, DateTime lastActive)
        {
            Id = id;
            Name = name;
            Description = description;
            LastActive = lastActive;
            Token = TokenHelper.GetToken(id);
        }
    }
}
