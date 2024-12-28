namespace GardeningAdviceSystem.Models.WateringReminder
{
    public class CreateReminderModel
    {
        public bool Regular { get; set; }
        public DateTime ReminderDate { get; set; }
        public int? DayGap { get; set; }

        public int DeviceId { get; set; }
    }
}
