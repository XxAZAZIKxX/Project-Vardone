using System;

namespace VardoneLibrary.Core.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() { }
        public UnauthorizedException(string name) : base(name) { }
    }
}