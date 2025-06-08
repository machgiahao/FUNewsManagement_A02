using FUNewsManagement.BusinessObjects.Entities;
using FUNewsManagementSystem.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository iTagRepository;

        public TagService()
        {
            this.iTagRepository = new TagRepository();
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            return await iTagRepository.GetTagsAsync();
        }
    }
}
