namespace GardeningAdviceSystem.Models.Plant
{
    public class PlantModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float MinPh { get; set; }
        public float MaxPh { get; set; }
        public int MinMoisture { get; set; }
        public int MaxMoisture { get; set; }
    }
}
