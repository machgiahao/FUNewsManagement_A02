
using FUNewsManagement.BusinessObjects.Entities;

namespace FUNewsManagementSystem.DataAccess
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTagsAsync();
    }
}
