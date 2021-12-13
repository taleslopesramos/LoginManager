using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginManager.Model
{
    public class Profile
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }

        public int IdUser { get; set; }
        public User User { get; set; }

        public string DisplayName { get; set; }
    }
}
