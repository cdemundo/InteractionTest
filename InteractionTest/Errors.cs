using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Interaction
{
    public class ETAMarketOrderFailedException : Exception
    {
        //constructors
        public ETAMarketOrderFailedException()
        { }

        public ETAMarketOrderFailedException(string message)
            : base(message)
        { }

        //Serializable
        protected ETAMarketOrderFailedException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        { }
    }

    public class ETAImageSearchFailedException : Exception
    {
        //constructors
        public ETAImageSearchFailedException()
        { }

        public ETAImageSearchFailedException(string message)
            : base(message)
        { }

        //Serializable
        protected ETAImageSearchFailedException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        { }
    }
    class Errors
    {
    }
}
