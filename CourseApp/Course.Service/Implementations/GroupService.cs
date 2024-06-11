using System;
using Course.Core.Entities;
using Course.Data;
using Course.Data.Repostories.Interfaces;
using Course.Service.Dtos.GroupDtos;
using Course.Service.Exceptions;
using Course.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Course.Service.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;

        }
        public int Create(GroupCreateDto createDto)
        {
            if (_groupRepository.Exists(x => x.No == createDto.No && !x.IsDeleted))
            throw new RestException(StatusCodes.Status400BadRequest, "No", "No already taken");

            Group entity = new Group
            {
                No = createDto.No,
                Limit = createDto.Limit,
            };
            _groupRepository.Add(entity);
            _groupRepository.Save();

            return entity.Id;
        }
        public List<GroupGetDto> GetAll(string? search=null)
        {
            return _groupRepository.GetAll(x => x.No.Contains(search),"Students").Select(x => new GroupGetDto
            {
                Id = x.Id,
                No = x.No,
                Limit = x.Limit,
                StudentCount=x.Students.Count
            }).ToList();
        }
        public GroupDetailsDto GetById(int id)
        {
            Group group = _groupRepository.Get(x => x.Id == id && !x.IsDeleted);

            if (group == null) throw new RestException(StatusCodes.Status404NotFound, "Group not found");

            return new GroupDetailsDto
            {
                Id = group.Id,
                No = group.No,
                Limit = group.Limit,
                StudentCount = group.Students.Count
            };

            // return Mapper<Group, GroupDetailsDto>.Map(group);
        }

        public void Update(int id, GroupUpdateDto updateDto)
        {
            Group entity = _groupRepository.Get(x => x.Id == id && !x.IsDeleted, "Students");

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Group not found");

            if (entity.No != updateDto.No && _groupRepository.Exists(x => x.No == updateDto.No && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "No", "No already taken");

            if (entity.Students.Count > updateDto.Limit)
                throw new RestException(StatusCodes.Status400BadRequest, "Limit", "limit exception");

            entity.No = updateDto.No;
            entity.Limit = updateDto.Limit;
            entity.ModifiedAt = DateTime.Now;

            _groupRepository.Save();

        }

        public void Delete(int id)
        {
            Group entity = _groupRepository.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Group not found");
            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.Now;
            _groupRepository.Save();
        }

       
    }
}

