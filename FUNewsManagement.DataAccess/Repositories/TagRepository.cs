
using FUNewsManagement.BusinessObjects.Context;
using FUNewsManagement.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public class TagRepository : ITagRepository
    {
        public async Task<List<Tag>> GetTagsAsync()
        {
            try
            {
                using var context = new FunewsManagementContext();
                return await context.Tags.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetTags: " + ex.Message);

            }
        }
    }
}
