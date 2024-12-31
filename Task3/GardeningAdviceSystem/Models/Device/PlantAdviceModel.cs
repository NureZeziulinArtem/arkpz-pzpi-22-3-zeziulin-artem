namespace GardeningAdviceSystem.Models.Device
{
    public class PlantAdviceModel
    {
        public float Ph { get; set; }
        public int Moisture { get; set; }

        public float MinPh { get; set; }
        public float MaxPh { get; set; }
        public int MinMoisture { get; set; }
        public int MaxMoisture { get; set; }

        public float MinPhChange { get; set; }
        public float IdealPhChange { get; set; }

        public int MinMoistureChange { get; set; }
        public int IdealMoistureChange { get; set; }
    }
}
