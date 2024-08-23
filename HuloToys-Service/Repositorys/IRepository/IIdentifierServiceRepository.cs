using System.Threading.Tasks;
namespace REPOSITORIES.IRepositories
{
    public interface IIdentifierServiceRepository
    {
        Task<string> buildOrderNo(long count_exists_order = 0); 
    }
}
