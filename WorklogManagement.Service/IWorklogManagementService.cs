using WorklogManagement.Service.Models;

namespace WorklogManagement.Service;

public interface IWorklogManagementService
{
    Task<OvertimeInfo> GetOvertimeAsync();

    Task<List<WorkTime>> GetWorkTimesOfYearAsync(int year);

    Task<List<Absence>> GetAbsencesOfYearAsyncAsync(int year);

    // TODO: Tickets

    // TODO: TicketAttachments

    // TODO: TicketsStatusLogs

    // TODO: Worklogs

    // TODO: WorklogAttachments

    Task<TDataModel> SaveAsync<TDataModel>(TDataModel item) where TDataModel : IDataModel;

    Task DeleteAsync<TDataModel>(int id) where TDataModel : IDataModel;
}
