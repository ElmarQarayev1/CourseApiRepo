using System;
using Course.Service.Dtos.GroupDtos;

namespace Course.Service.Interfaces
{
	public interface IGroupService
	{
        int Create(GroupCreateDto createDto);
        List<GroupGetDto> GetAll(string? search = null);
        GroupDetailsDto GetById(int id);
        void Update(int id, GroupUpdateDto updateDto);
        void Delete(int id);
    }
}

