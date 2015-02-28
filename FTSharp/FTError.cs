using System;

namespace FTSharp
{
    public class FTError : Exception
    {
        public string errorMessage;
        public int errorCode;
        
        public FTError (int err)
        {
            errorCode = err;
            errorMessage = FT.ErrorMessage(err);
        }

        public FTError(string msg) // custom error
        {
            errorCode = -1;
            errorMessage = msg;
        }

        public override string Message {
            get {
                return String.Format("Freetype Error: {0} (0x{1:x4})", errorMessage, errorCode);
            }
        }
    }
}

