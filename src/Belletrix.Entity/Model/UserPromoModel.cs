using System;

namespace Belletrix.Entity.Model
{
    public class UserPromoModel
    {
        public string Description { get; set; }
        public string Code { get; set; }
        public int CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public bool Active { get; set; }
        public Guid? PublicToken { get; set; }
    }
}
