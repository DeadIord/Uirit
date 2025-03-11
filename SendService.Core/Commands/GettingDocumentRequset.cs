using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SendService.Core.Commands
{
    public class GettingDocumentRequset
    {
    }
   [XmlRoot("Document")]
    public class GettingDocumentResponse
    {
        [XmlElement("Data")]
        public string Data { get; set; }
    }

}
