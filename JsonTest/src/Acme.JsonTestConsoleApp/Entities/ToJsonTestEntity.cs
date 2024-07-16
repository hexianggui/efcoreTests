using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Acme.JsonTestConsoleApp.Entities
{
    public class ToJsonTestEntity:Entity<Guid>
    {
        public List<JosnItem>? JsonItems {  get; set; }
    }
    public class JosnItem
    {
        // Erro
        public DateTime? TimeTest { get; set; }
        // True?
        //public string? TimeTest { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        
    }
}
