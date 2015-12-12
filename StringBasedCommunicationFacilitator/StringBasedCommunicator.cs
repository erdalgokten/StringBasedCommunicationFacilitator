using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace StringBasedCommunicationFacilitator
{
    /// <summary>
    /// This is where we make the connection to the string-based communicatior
    /// </summary>
    public static class StringBasedCommunicator
    {
        public static string SendAndReceive(string input)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("000000002"); // Total number of orders
            sb.Append("Kettle    It boils water      0005.540000100005.54"); // First order
            sb.Append("MiniRobot It chops            0004.000000200008.00"); // Second order
            sb.Append("                                                  "); // Empty
            sb.Append("                                                  "); // Empty
            sb.Append("                                                  "); // Empty
            return sb.ToString();
        }
    }
}