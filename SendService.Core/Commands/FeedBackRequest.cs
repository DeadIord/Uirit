using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendService.Core.Commands
{
    public class FeedBackRequest
    {
        public string? Text { get; set; }
    }
    public class FeedBackResponse
    {
        public List<object> Data { get; set; }
    }
}
