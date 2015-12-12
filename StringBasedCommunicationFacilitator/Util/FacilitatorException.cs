using StringBasedCommunicationFacilitator.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StringBasedCommunicationFacilitator.Util
{
    public class FacilitatorException : Exception
    {
        private int resultCode = 0;
        public int ResultCode { get { return this.resultCode; } }

        public string ResultDesc
        {
            get
            {
                return Resources.ResourceManager.GetString("ERR_" + resultCode.ToString().PadLeft(3, '0'));
            }
        }

        public FacilitatorException(int resultCode)
        {
            this.resultCode = resultCode;
        }

        public FacilitatorException(int resultCode, string message)
            : base(message)
        {
            this.resultCode = resultCode;
        }

        public FacilitatorException(int resultCode, string message, Exception inner)
            : base(message, inner)
        {
            this.resultCode = resultCode;
        }

        public FacilitatorException(int resultCode, string fmt, params object[] args)
            : base(string.Format(fmt, args))
        {
            this.resultCode = resultCode;
        }

        public FacilitatorException(int resultCode, Exception inner, string fmt, params object[] args)
            : base(string.Format(fmt, args), inner)
        {
            this.resultCode = resultCode;
        }
    }
}