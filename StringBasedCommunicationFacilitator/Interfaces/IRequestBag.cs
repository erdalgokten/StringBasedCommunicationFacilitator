using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringBasedCommunicationFacilitator.Interfaces
{
    public interface IRequestBag
    {
        string ActionName { get; }
        string ActionDesc { get; }
    }
}
