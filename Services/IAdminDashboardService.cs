using Job_Portal_Project.ViewModels.Admin;

namespace Job_Portal_Project.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetDashboardDataAsync();
    }
}
