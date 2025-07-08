using System.Text.Json.Serialization;
using System.Text.Json;

namespace DateInputNormalizer.DateNormalization
{
    /// <summary>
    /// A custom JSON converter that handles serialization and deserialization of 
    /// DateTime and DateTime? values with time zone normalization.
    /// </summary>
    /// <remarks>
    /// Converts dates to and from a specified client time zone during serialization/deserialization,
    /// and supports both DateTime and DateOnly types.
    /// </remarks>
    public class ZonedDateConverter : JsonConverter<object>
    {
        /// <summary>
        /// The time zone used by the server for normalization during serialization.
        /// Defaults to UTC.
        /// </summary>
        public static TimeZoneInfo ServerTimeZone = TimeZoneInfo.Utc;

        /// <summary>
        /// The time zone representing the client's time zone.
        /// Used to convert incoming JSON date values to UTC for backend processing.
        /// Defaults to UTC.
        /// </summary>
        public static TimeZoneInfo ClientTimeZone = TimeZoneInfo.Utc;


        private readonly JsonConverterFactory? _defaultFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateConverter"/> class.
        /// </summary>
        public ZonedDateConverter()
        {
            _defaultFactory = null;
        }

        /// <summary>
        /// Determines whether this converter can convert the specified type.
        /// </summary>
        /// <param name="typeToConvert">The target type to check for compatibility.</param>
        /// <returns>
        /// True if the type is DateTime or DateTime?; otherwise, false.
        /// </returns>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTime?);
        }

        /// <summary>
        /// Reads and deserializes a JSON string into a DateTime object,
        /// applying client time zone conversion for normalization.
        /// </summary>
        /// <param name="reader">The JSON reader.</param>
        /// <param name="typeToConvert">The expected type (DateTime or DateOnly).</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>
        /// A UTC DateTime or adjusted DateOnly object, depending on the target type.
        /// </returns>
        /// <exception cref="JsonException">Thrown if the input is invalid or unsupported.</exception>
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException();

            var str = reader.GetString();

            if (string.IsNullOrWhiteSpace(str))
                return null;

            if (typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTime?))
            {
                if (DateTime.TryParse(str, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                {
                    var unspecified = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
                    var utc = TimeZoneInfo.ConvertTimeToUtc(unspecified, ClientTimeZone);
                    return utc;
                }
            }
          
            throw new JsonException("Invalid date format or unsupported type.");
        }

        /// <summary>
        /// Writes a DateTime object to JSON,
        /// converting it from UTC or server time zone into the client time zone.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The object to write.</param>
        /// <param name="options">The serializer options.</param>
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is DateTime dt)
            {
                // From UTC to client timezone
                var adjusted = TimeZoneInfo.ConvertTimeFromUtc(dt, ClientTimeZone);
                writer.WriteStringValue(adjusted.ToString("o")); // ISO 8601
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
