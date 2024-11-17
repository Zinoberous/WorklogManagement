using WorklogManagement.Service.Models;
using WorklogManagement.Service.Models.Queries;

namespace WorklogManagement.Service;

public interface IWorklogManagementService
{
    Task<TDataModel> SaveAsync<TDataModel>(TDataModel item) where TDataModel : IDataModel;

    Task DeleteAsync<TDataModel>(int id) where TDataModel : IDataModel;

    // TODO: Absences

    Task<Ticket> GetTicketByIdAsync(int id);

    Task<Page<Ticket>> GetTicketsAsync(TicketQuery query);

    // TODO: TicketAttachments

    // TODO: TicketsStatusLogs

    Task<Worklog> GetWorklogByIdAsync(int id);

    Task<Page<Worklog>> GetWorklogsAsync(WorklogQuery query);

    // TODO: WorklogAttachments

    // TODO: WorkTimes

    Task<OvertimeInfo> GetOvertimeAsync();

    // TODO: Statistics

    // TODO: Holidays
}
