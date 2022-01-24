using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TOMI.Data.Database;
using TOMI.Data.Database.Entities;
using TOMI.Services.Interfaces;
using TOMI.Services.Models;

namespace TOMI.Services.Repository
{
    public class GroupRepository : IGroupService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GroupRepository> _logger;
        private readonly TOMIDataContext _context;
        public GroupRepository(ILogger<GroupRepository> logger, TOMIDataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<GroupResponse> AddGroup(GroupModel model)
        {
            var isNameExist = await _context.Group.FirstOrDefaultAsync(c => c.Name == model.Name);

            if (isNameExist != null)
            {
                return new GroupResponse { Error = "Group name already exist ", Success = false };
            }
            Group existingRanges = await _context.Group.FirstOrDefaultAsync(c => c.Id == model.Id);
            
            if (existingRanges != null)
            {
                var group = _mapper.Map<Group>(model);
               // existingRanges.Name = model.Name;
                _context.Group.Update(existingRanges);
                _context.SaveChanges();
                return new GroupResponse { group = existingRanges, Success = true };
            }
    
            if (existingRanges == null)
            {
                var group = _mapper.Map<Group>(model);
                Group result = _context.Group.Add(group).Entity;
                _context.SaveChanges();
                return new GroupResponse { group = result, Success = true };
            }
      

            throw new ValidationException("Group not found!");
        }

        public async Task<GroupResponse> DeleteGroup(Guid id)
        {
            var existingRange= await _context.Ranges.FirstOrDefaultAsync(x => x.GroupId == id);
            if(existingRange!=null)
            {
                return new GroupResponse { Error = "Mentioned group name is in used ", Success = false };
            }
            var existingRanges = await _context.Group.FirstOrDefaultAsync(x => x.Id == id);
           
            if (existingRanges.IsDeleted == false)
            {
                existingRanges.IsDeleted = true;
                await _context.SaveChangesAsync();
                //return existingRanges;
                return new GroupResponse { group = existingRanges, Success = true };
            }
            throw new ValidationException("GroupId not found!");
        }
       

        public async Task<Group> GetGroup(Guid id)
        {
            return await _context.Group.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Group>> GetGroupAsync()
        {
            return await _context.Group.Where(x=>x.IsActive==false).ToListAsync();
        }

        //public async Task<Group> UpdateGroup(GroupModel model)
        //{
        //    var result = await _context.Group
        //        .FirstOrDefaultAsync(e => e.Id == model.Id);

        //    if (result != null)
        //    {
        //        result.Name = model.Name;
        //        await _context.SaveChangesAsync();

        //        return result;
        //    }

        //    return null;
        //}
    }
}
