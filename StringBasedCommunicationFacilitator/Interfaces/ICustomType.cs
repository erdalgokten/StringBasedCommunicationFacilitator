using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringBasedCommunicationFacilitator.Interfaces
{
    public interface ICustomType
    {
        int Length { get; }
        string ToCustomString();
        void FromCustomString(string str);
    }
}
