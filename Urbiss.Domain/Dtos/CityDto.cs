namespace Urbiss.Domain.Dtos
{
    public class CityWithoutGeometryDto
    {
        public long Id { get; set; }
        public string IbgeCode { get; set; }
        public string Name { get; set; }
        public int Zoom { get; set; }
        public double LatitudeCenter { get; set; }
        public double LongitudeCenter { get; set; }
    }
}
