using Newtonsoft.Json.Converters;

namespace Urbiss.Domain.Converters
{
    public class DateTimeFormatConverter : IsoDateTimeConverter
    {
        public DateTimeFormatConverter(string format) : base()
        {
            DateTimeFormat = format;
        }
    }
}
