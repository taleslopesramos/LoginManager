using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginManager.Model
{
    public class User
    {
        public User()
        {
            Profiles = new HashSet<Profile>();
        }

        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime LastAccess { get; set; }

        public ICollection<Profile> Profiles { get; set; }

    }
}
