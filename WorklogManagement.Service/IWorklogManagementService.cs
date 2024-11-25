using WorklogManagement.Service.Enums;
using WorklogManagement.Service.Models;

namespace WorklogManagement.Service;

public interface IWorklogManagementService
{
    Task<OvertimeInfo> GetOvertimeAsync();

    Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null);

    Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync();

    Task<List<WorkTime>> GetWorkTimesAsync(DateOnly date) => GetWorkTimesAsync(date, date);
    Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to);

    Task<List<Absence>> GetAbsencesAsync(DateOnly date) => GetAbsencesAsync(date, date);
    Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to);

    // TODO: Tickets

    // TODO: TicketAttachments

    // TODO: TicketsStatusLogs

    // TODO: Worklogs

    // TODO: WorklogAttachments

    Task<TDataModel> SaveAsync<TDataModel>(TDataModel item) where TDataModel : IDataModel;

    Task DeleteAsync<TDataModel>(int id) where TDataModel : IDataModel;
}
