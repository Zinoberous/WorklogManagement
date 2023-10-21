using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models
{
    public class Status
    {
        public int Id { get; }

        [StringLength(255)]
        public string Name { get; }

        public Status(DB.Status status)
        {
            Id = status.Id;
            Name = status.Name;
        }
    }
}
