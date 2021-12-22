using NetTopologySuite.Geometries;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Urbiss.Domain.Models
{
    public class UserSurvey : BaseEntity
    {
        [Column(TypeName = "geometry(polygon)")]
        [Required]
        public Polygon Geometry { get; set; }

        [Column]
        [Required]
        public int UserId { get; set; }

        [Column]
        [Required]
        public DateTime Date { get; set; }

        [Column(TypeName = "character varying(20)")]
        [Required]
        public string Ip { get; set; }

        [Column]
        [Required]
        public double Area { get; set; }
    }
}
