using EFDemo.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDemo.Domain
{
    public class Team : BaseDomainObject
    {   
        public string Name { get; set; }

        public int LeagueId { get; set; }
        public virtual League League { get; set; } //navigation property

        public virtual List<Match> HomeMatches { get; set; } //navigation property

        public virtual List<Match> AwayMatches { get; set; } //navigation property
    }
}
