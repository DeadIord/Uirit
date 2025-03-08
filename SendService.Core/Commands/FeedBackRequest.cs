using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendService.Core.Commands
{
    public class FeedBackRequest
    {
        public ServiceInfo Service { get; set; }
        public ContactsInfo Contacts { get; set; }
        public PropertiesInfo Properties { get; set; }

        public FeedBackRequest()
        {
            Service = new ServiceInfo();
            Contacts = new ContactsInfo();
            Properties = new PropertiesInfo();
        }
    }
    public class FeedBackResponse
    {
        public string Data { get; set; }
    }


    public class ServiceInfo
    {
        public DateTime RegDate { get; set; }
        public string ServiceType { get; set; }
    }

    public class ContactsInfo
    {
        public PrivatePersonInfo[] PrivatePerson { get; set; }

        public ContactsInfo()
        {
            PrivatePerson = new PrivatePersonInfo[1] { new PrivatePersonInfo() };
        }
    }

    public class PrivatePersonInfo
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
    }

    public class PropertiesInfo
    {
        public string Text { get; set; }
    }

    public class ApplicationPostResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

}
