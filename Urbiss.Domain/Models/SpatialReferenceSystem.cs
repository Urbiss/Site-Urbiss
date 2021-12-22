using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Urbiss.Domain.Models
{
    [Table("spatial_ref_sys")]
    [Keyless]
    public class SpatialReferenceSystem
    {
        public int Srid { get; set; }
        [Column("srtext")]
        public string Text { get; set; }
    }
}
