using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginManager.DTO
{
    public class LoggedUserResultDTO
    {
        public Guid id { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public DateTime last_login { get; set; }
        public List<ProfileDTO> profiles { get; set; }
    }
}
