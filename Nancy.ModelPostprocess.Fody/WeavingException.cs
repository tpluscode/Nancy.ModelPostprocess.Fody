using System;

namespace Nancy.ModelPostprocess.Fody
{
    public class WeavingException : Exception
    {
        public WeavingException(string message)
            : base(message)
        {
        }
    }
}