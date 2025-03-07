using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendService.Core.Commands
{
    public class GettingDocumentRequset
    {
        public string? Text { get; set; }
    }
    public class GettingDocumentResponse
    {
        public List<object> Data { get; set; }
    }
}
