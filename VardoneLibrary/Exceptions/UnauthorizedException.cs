using System;

namespace VardoneLibrary.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() { }
        public UnauthorizedException(string name) : base("UnauthorizedException " + name) { }
    }
}