using Job_Portal_Project.Models;

namespace Job_Portal_Project.ViewModels
{
    public class CompanyListViewModel
    {
        public List<Company> Companies { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string NameFilter { get; set; }
        public string CountryFilter { get; set; }
        public string CityFilter { get; set; }
    }
}
