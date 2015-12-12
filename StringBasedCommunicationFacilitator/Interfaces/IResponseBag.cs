using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringBasedCommunicationFacilitator.Interfaces
{
    public interface IResponseBag
    {
        string RequestText { get; set; }
        string ResponseText { get; set; }
        int ResultCode { get; set; }
        string ResultDesc { get; set; }
    }
}
