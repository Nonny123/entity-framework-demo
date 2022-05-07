using EFDemo.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDemo.Domain
{
    public class League : BaseDomainObject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Team> Teams { get; set; }

    }
}
