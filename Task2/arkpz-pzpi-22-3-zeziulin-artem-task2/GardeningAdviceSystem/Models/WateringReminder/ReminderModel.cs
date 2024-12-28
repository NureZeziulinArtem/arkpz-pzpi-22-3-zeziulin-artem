namespace GardeningAdviceSystem.Models.WateringReminder
{
    public class ReminderModel
    {
        public int Id { get; set; }
        public bool Regular { get; set; }
        public DateTime ReminderDate { get; set; }
        public int? DayGap { get; set; }

        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
    }
}
