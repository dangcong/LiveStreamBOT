using BaseSouce.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Data.Entities
{
    public class ManagerGroup
    {
        public int Id { get; set; }
        public string UserIdManager { get; set; }
        public string UserId { get; set; }
        public string Note { get; set; }

        public AppUser UserManager { get; set; }
        public AppUser User { get; set; }
    }
}
