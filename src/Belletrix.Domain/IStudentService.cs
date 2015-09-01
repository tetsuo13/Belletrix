using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentModel>> GetStudents(int? id = null);
        Task<StudentModel> GetStudent(int id);
    }
}
