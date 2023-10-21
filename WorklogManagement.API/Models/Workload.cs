using System.ComponentModel.DataAnnotations;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models
{
    public class Workload
    {
        public int Id { get; }

        [StringLength(255)]
        public string Name { get; }

        public Workload(DB.Workload workload)
        {
            Id = workload.Id;
            Name = workload.Name;
        }
    }
}
