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

        public async Task<Group> AddGroup(GroupModel model)
        {
            Group existingRanges = await _context.Group.FirstOrDefaultAsync(c => c.Name == model.Name);

            var group = _mapper.Map<Group>(model);
            if (existingRanges == null)
            {
                Group result = _context.Group.Add(group).Entity;
                _context.SaveChanges();
                return result;
            }
            throw new ValidationException("Group not found!");
        }

       
        public async Task<Group> DeleteGroup(Guid id)
        {
            var result= await _context.Group
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                _context.Group.Remove(result);
                await _context.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<Group> GetGroup(Guid id)
        {
            return await _context.Group.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Group>> GetGroupAsync()
        {
            return await _context.Group.ToListAsync();
        }

        public async Task<Group> UpdateGroup(GroupModel model)
        {
            var result = await _context.Group
                .FirstOrDefaultAsync(e => e.Id == model.Id);

            if (result != null)
            {
                result.Id = model.Id;
                result.Name = model.Name;
               

                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }
    }
}
