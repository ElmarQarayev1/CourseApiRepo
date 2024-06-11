using System;
using Course.Core.Entities;
using Course.Data;
using Course.Service.Interfaces;
using Course.Service.Dtos.StudentDtos;
using Course.Service.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Course.Data.Repostories.Interfaces;
using Course.Service.Dtos.GroupDtos;
using Microsoft.AspNetCore.Mvc;
using Course.Service.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace Course.Service.Implementations
{
	public class StudentService:IStudentService
	{
        private readonly IStudentRepository _studentRepository;
        private readonly IGroupRepository _groupRepository;

        private readonly IWebHostEnvironment _env;
        public StudentService(IGroupRepository groupRepository,IStudentRepository studentRepository, IWebHostEnvironment env)
        {
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
            _env = env;

        }
        public int Create([FromForm] StudentCreateDto createDto)
        {
            
            Group group = _groupRepository.Get(x => x.Id == createDto.GroupId, "Students");
            if (group == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "GroupId", "Group not found by given Id");
            }

            if (group.Limit <= group.Students.Count)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Group is full!");
            }

            if (_studentRepository.Exists(x => x.Email.ToUpper() == createDto.Email.ToUpper() && !x.IsDeleted))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email", "Student already exists by given Email");
            }

            Student student = new Student
            {
                FullName = createDto.FullName,
                Email = createDto.Email,
                BirthDate = createDto.Birthdate,
                GroupId = createDto.GroupId,
               FileName = FileManager.Save(createDto.File, _env.WebRootPath, "uploads/student")
        };
            _studentRepository.Add(student);
            _studentRepository.Save();

            return student.Id;
        }

        public void Delete(int id)
        {
            Student entity = _studentRepository.Get(x => x.Id == id);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Student not found");

            _studentRepository.Delete(entity);
            _groupRepository.Save();
        }
        public List<StudentGetDto> GetAll(string? search=null)
        {

            return _studentRepository.GetAll(x => x.FullName.Contains(search)).Select(x => new StudentGetDto
            {
                FullName = x.FullName,
                Email = x.Email,
                Birthdate = x.BirthDate,
                GroupName=x.Group.No

            }).ToList();         
        }

        public StudentDetailsDto GetById(int id)
        {
            Student student = _studentRepository.Get(x => x.Id == id && !x.IsDeleted);

            if (student == null) throw new RestException(StatusCodes.Status404NotFound, "Group not found");

            return new StudentDetailsDto
            {
                FullName = student.FullName,
                Email = student.Email,

                Birthdate = student.BirthDate,
                GroupName = student.Group.No
            };
            //return Mapper<Student, StudentDetailsDto>.Map(student);

        }
        public void Update(int id, [FromForm] StudentUpdateDto studentUpdate)
        {
            
            var existingStudent = _studentRepository.Get(x => x.Id == id);
            if (existingStudent == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Id", "Student not found by given Id");
            }

            string deletedFile = null;

            Group group = _groupRepository.Get(x => x.Id == studentUpdate.GroupId, "Students");
            if (group == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "GroupId", "Group not found by given Id");
            }

            if (group.Limit <= group.Students.Count)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Group is full!");
            }

            existingStudent.FullName = studentUpdate.FullName;
            existingStudent.Email = studentUpdate.Email;
            existingStudent.BirthDate = studentUpdate.Birthdate;
           
            existingStudent.GroupId = studentUpdate.GroupId;
            existingStudent.ModifiedAt = DateTime.Now;   
            _studentRepository.Save();

            if (deletedFile != null)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/student", deletedFile);
            }

        }
    
    }
}

