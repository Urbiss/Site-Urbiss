using ExifLib;
using System;

namespace Urbiss.Services.Helpers
{
    public static class ImageHelper
    {
        public static ExifInformation ReadExifData(string fileName)
        {
            using var reader = new ExifReader(fileName);
            if ((reader.GetTagValue<double[]>(ExifTags.GPSLongitude, out double[] longitude)) &&
                (reader.GetTagValue<double[]>(ExifTags.GPSLatitude, out double[] latitude)) &&
                (reader.GetTagValue<string>(ExifTags.GPSLongitudeRef, out string longitudeRef)) &&
                (reader.GetTagValue<string>(ExifTags.GPSLatitudeRef, out string latitudeRef)) &&
                (reader.GetTagValue<DateTime>(ExifTags.DateTimeOriginal, out DateTime dateTime)))
            {
                var lat = NtsHelper.ConvertLatLongToDecimal(latitude);
                if (!latitudeRef.ToUpper().Equals("N"))
                    lat = 0 - lat;
                var lng = NtsHelper.ConvertLatLongToDecimal(longitude);
                if (!longitudeRef.ToUpper().Equals("E"))
                    lng = 0 - lng;
                return new ExifInformation
                {
                    Latitude = lat,
                    Longitude = lng,
                    DateTime = dateTime
                };
            }
            return null;
        }
    }

    public class ExifInformation
    {
        public double Latitude;
        public double Longitude;
        public DateTime DateTime;
    }
}
