namespace GardeningAdviceSystem.Models.Device
{
    public class DeviceModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int? PlantId { get; set; }
        public string? PlantName { get; set; }
    }
}
