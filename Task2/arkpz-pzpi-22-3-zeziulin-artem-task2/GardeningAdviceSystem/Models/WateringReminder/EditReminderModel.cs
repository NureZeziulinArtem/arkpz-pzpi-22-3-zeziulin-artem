namespace GardeningAdviceSystem.Models.WateringReminder
{
    public class EditReminderModel
    {
        public int Id { get; set; }
        public bool Regular { get; set; }
        public DateTime ReminderDate { get; set; }
        public int? DayGap { get; set; }
    }
}
