namespace GardeningAdviceSystem.Models.Fertilizer
{
    public class CreateFertilizerModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Ph { get; set; }
        public float Size { get; set; }
        public int Price { get; set; }
    }
}
