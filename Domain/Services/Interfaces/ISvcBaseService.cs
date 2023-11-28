
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface ISvcBaseService<T>
    {
        Task<T> GetAsync(string id);
    }
}
