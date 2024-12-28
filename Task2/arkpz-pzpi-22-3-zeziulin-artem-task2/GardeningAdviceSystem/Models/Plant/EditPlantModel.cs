namespace GardeningAdviceSystem.Models.Plant
{
    public class EditPlantModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float MinPh { get; set; }
        public float MaxPh { get; set; }
        public int MinMoisture { get; set; }
        public int MaxMoisture { get; set; }
    }
}
