namespace GardeningAdviceSystem.Models.Fertilizer
{
    public class EditFertilizerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Ph { get; set; }
        public float Size { get; set; }
        public int Price { get; set; }
    }
}
