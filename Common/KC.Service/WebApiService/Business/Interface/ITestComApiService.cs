using System.Threading.Tasks;

namespace KC.Service.WebApiService.Business
{
    public interface ITestComApiService
    {
        ServiceResult<bool> Get();
        Task<ServiceResult<bool>> GetAsync();
        string GetTestHtml(string title);
    }
}