using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Urbiss.Domain.Models
{
    public class City : BaseEntity
    {
        [Column(TypeName = "geometry(multipolygon)")]
        [Required]
        public MultiPolygon Geometry { get; set; }

        [Column(TypeName = "character varying(100)")]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = "character varying(7)")]
        [Required]
        public string IbgeCode { get; set; }

        [Column]
        [Required]
        public int Srid { get; set; }

        [Column]
        [Required]
        public double LatitudeCenter { get; set; }
        
        [Column]
        [Required]
        public double LongitudeCenter { get; set; }

        [Column]
        [Required]
        public int Zoom { get; set; }
    }
}
