namespace WorklogManagement.API.Interfaces
{
    public interface IQuery
    {
        string Sort { get; set; }
        uint PageSize { get; set; }
        uint PageIndex { get; set; }
    }
}
