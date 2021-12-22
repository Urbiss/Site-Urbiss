using NetTopologySuite.Geometries;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Urbiss.Domain.Models
{
    public class AerialPhoto : BaseEntity
    {
        [Column(TypeName = "geometry(point)")]
        [Required]
        public Point Geometry { get; set; }

        [Column(TypeName = "character varying(40)")]
        [Required]
        public string Name { get; set; }

        [Column]
        [Required]
        public DateTime Date { get; set; }

        [Column]
        [Required]
        public long SurveyId { get; set; }
    }
}
