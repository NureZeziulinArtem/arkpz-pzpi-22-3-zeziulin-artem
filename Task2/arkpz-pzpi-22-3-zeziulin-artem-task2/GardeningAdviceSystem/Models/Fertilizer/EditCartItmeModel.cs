namespace GardeningAdviceSystem.Models.Fertilizer
{
    public class EditCartItmeModel
    {
        public int FertilizerId { get; set; }
        public bool Remind { get; set; }
        public DateTime? RemindAt { get; set; }
    }
}
