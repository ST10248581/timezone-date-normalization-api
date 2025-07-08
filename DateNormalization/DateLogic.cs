using DateInputNormalizer.Settings;

namespace DateInputNormalizer.DateNormalization
{
    public class DateLogic
    {
        /// <summary>
        /// Attempts to set the server's time zone used by the ZonedDateConverter
        /// based on the configuration value in AppSettings.
        /// </summary>
        /// <returns>
        /// True if the server time zone was successfully set; otherwise, false.
        /// </returns>
        public bool setServerTimeZone()
        {
            bool success = false;
            try
            {
                ZonedDateConverter.ServerTimeZone = TimeZoneInfo.FindSystemTimeZoneById(AppSettings.ServerTimeZoneId);
                success = true;
            }
            catch
            {
                success =  false;
            }

            return success;
        }

        /// <summary>
        /// Attempts to set the client's time zone used by the ZonedDateConverter
        /// using the provided time zone identifier.
        /// </summary>
        /// <param name="timeZoneID">The IANA or Windows time zone ID to apply as the client time zone.</param>
        /// <returns>
        /// True if the client time zone was successfully set; otherwise, false.
        /// </returns>
        public bool setClientTimeZone(string? timeZoneID)
        {
            bool success = false;
            try
            {
                if (timeZoneID == null) timeZoneID = AppSettings.DefaultTimeZoneId;

                ZonedDateConverter.ClientTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneID);
                success = true;
            }
            catch
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Converts a <see cref="DateOnly"/> value to a UTC <see cref="DateTime"/>
        /// using the client time zone defined in <see cref="ZonedDateConverter.ServerTimeZone"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateOnly"/> value to convert.</param>
        /// <returns>
        /// A UTC <see cref="DateTime"/> representing midnight of the given date in the server's time zone.
        /// </returns>
        public DateTime ToUtcDateTimeSafe(DateOnly date)
        {
            var localMidnight = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(localMidnight, ZonedDateConverter.ClientTimeZone);
        }


        /// <summary>
        /// Converts a UTC <see cref="DateTime"/> value to a local <see cref="DateOnly"/>
        /// using the client time zone defined in <see cref="ZonedDateConverter.ServerTimeZone"/>.
        /// </summary>
        /// <param name="utcDateTime">The <see cref="DateTime"/> value to convert.</param>
        /// <returns>
        /// A local <see cref="DateOnly"/> representing the date of the given date time in the client's time zone.
        /// </returns>
        public DateOnly ToLocalDateSafe(DateTime utcDateTime)
        {
            var correctLocalDateTime =  TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, ZonedDateConverter.ClientTimeZone);
            return new DateOnly(correctLocalDateTime.Year, correctLocalDateTime.Month, correctLocalDateTime.Day);
        }

        /// <summary>
        /// Returns the current date and time in the configured server time zone.
        /// </summary>
        /// <returns>A DateTime representing the current local server time.</returns>
        public static DateTime GetServerDateTimeNow()
        {
            var serverTimeZone = ZonedDateConverter.ServerTimeZone
                                 ?? TimeZoneInfo.Utc; // fallback if not set

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, serverTimeZone);
        }
    }
}
