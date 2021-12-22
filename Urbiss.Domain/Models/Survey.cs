using NetTopologySuite.Geometries;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Urbiss.Domain.Models
{
    public class Survey : BaseEntity
    {
        [Column(TypeName = "geometry(polygon)")]
        [Required]
        public Polygon Geometry { get; set; }

        [Column]
        [Required]
        public int UserId { get; set; }

        [Column]
        [Required]
        public DateTime SurveyDate { get; set; }

        [Column(TypeName = "character varying(40)")]
        [Required]
        public string Folder { get; set; }

        [Required]
        [Column]
        public int Srid { get; set; }
    }
}
