using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.WorklogManagement.Models
{
    [Table("Workload")]
    [Index("Name", Name = "UX_Workload_Name", IsUnique = true)]
    public partial class Workload
    {
        public Workload()
        {
            Days = new HashSet<Day>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [InverseProperty("Workload")]
        public virtual ICollection<Day> Days { get; set; }
    }
}
