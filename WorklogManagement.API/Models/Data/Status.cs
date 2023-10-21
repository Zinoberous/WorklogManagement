using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class Status : IData
    {
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("name")]
        [MaxLength(255)]
        public string Name { get; }

        public Status(DB.Status status)
        {
            Id = status.Id;
            Name = status.Name;
        }
    }
}
