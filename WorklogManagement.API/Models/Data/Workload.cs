using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class Workload : IData
    {
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("name")]
        [MaxLength(255)]
        public string Name { get; }

        public Workload(DB.Workload workload)
        {
            Id = workload.Id;
            Name = workload.Name;
        }
    }
}
