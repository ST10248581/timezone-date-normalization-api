namespace DateInputNormalizer.Models
{
    public class TestDateModel
    {
        public bool skipsOtherProperty = true;
        public string ReceivedDateTimeRaw { get; set; }
        public string ReceivedDateOnlyRaw { get; set; }
        public DateTime EventTime { get; set; }
        public DateOnly EventDate { get; set; }
        public DateOnly ConvertedDateOnly { get; set; }
        public DateOnly ConvertedSafeDateOnly { get; set; }
    }
}
