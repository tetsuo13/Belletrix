using Belletrix.DAL;
using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository StudentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            StudentRepository = studentRepository;
        }

        public async Task<IEnumerable<StudentModel>> GetStudents(int? id = null)
        {
            return await StudentRepository.GetStudents(id);
        }

        public async Task<StudentModel> GetStudent(int id)
        {
            return await StudentRepository.GetStudent(id);
        }
    }
}
