using EFDemo.Domain.Common;
using System;
using System.Collections.Generic;


namespace EFDemo.Domain
{
    public class League : BaseDomainObject
    { 
        public string Name { get; set; }

        public List<Team> Teams { get; set; }

    }
}
