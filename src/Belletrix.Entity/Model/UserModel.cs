using System;

namespace Belletrix.Entity.Model
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }

        [Obsolete("Use Password instead")]
        public string PasswordHash { get; set; }

        [Obsolete("Use PasswordHash instead")]
        public int PasswordIterations { get; set; }

        [Obsolete("Use PasswordHash instead")]
        public string PasswordSalt { get; set; }

        public string Password { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastLogin { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
    }
}
