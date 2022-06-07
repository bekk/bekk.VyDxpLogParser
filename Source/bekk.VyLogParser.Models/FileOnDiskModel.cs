using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bekk.VyLogParser.Models
{
    public class FileOnDiskModel
    {
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public DateTime LogStartDate { get; set; }
    }
}
