using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendService.Core.Commands
{
    public class GettingRecordsRequest
    {
    }
    public class GettingRecordsResponse
    {
        public List<ApplicationDto> Data { get; set; } = new();
    }
    public class ApplicationDto
    {
        public int Id { get; set; }
        public int ServiceNumber { get; set; }
        public DateTime Created { get; set; }
        public string Body { get; set; }
        public int StatusId { get; set; }
        public bool Check { get; set; }
    }

}
