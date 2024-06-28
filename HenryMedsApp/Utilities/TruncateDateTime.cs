namespace HenryMedsApp.Utilities
{
    public static class TruncateDateTime
    {
        public static DateTime TruncateToMinute(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0);
        }
    }
}
