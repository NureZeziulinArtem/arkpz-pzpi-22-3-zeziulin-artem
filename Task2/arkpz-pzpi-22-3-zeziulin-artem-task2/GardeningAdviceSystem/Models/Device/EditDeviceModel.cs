namespace GardeningAdviceSystem.Models.Device
{
    public class EditDeviceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? PlantId { get; set; }
    }
}
