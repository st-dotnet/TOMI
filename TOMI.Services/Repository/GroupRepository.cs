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
        //private readonly ILogger<GroupRepository> _logger;
        private readonly TOMIDataContext _context;
        public GroupRepository(/*ILogger<GroupRepository> logger, */TOMIDataContext context, IMapper mapper)
        {
            //_logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<GroupResponse> AddGroup(GroupModel model)
        {
            try
            {
                var isNameExist = await _context.Group.FirstOrDefaultAsync(c => c.Name == model.Name);

                if (isNameExist != null)
                {
                    return new GroupResponse { Error = "Group name already exist ", Success = false };
                }
                Group existingRanges = await _context.Group.FirstOrDefaultAsync(c => c.Id == model.Id);

                if (existingRanges != null)
                {

                    existingRanges.Name = model.Name;
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
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        public async Task<GroupResponse> DeleteGroup(Guid id)
        {
            try
            {
                var existingGroup = await _context.Group.FirstOrDefaultAsync(x => x.Id == id);
                if (existingGroup != null)
                {
                    var recordexist = await _context.Ranges
                    .FirstOrDefaultAsync(e => e.GroupId == id);
                    if (recordexist != null)
                        return new GroupResponse { Error = "Mentioned group name is in used ", Success = false };
                    _context.Group.Remove(existingGroup);
                    _context.SaveChanges();
                    return new GroupResponse { group = existingGroup, Success = true };
                }
                return new GroupResponse { Error = "GroupId not found! ", Success = false };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<Group> GetGroup(Guid id)
        {
            try
            {
                return await _context.Group.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<List<Group>> GetGroupAsync()
        {
            try
            {
                return await _context.Group.Where(x => x.IsActive == false).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
