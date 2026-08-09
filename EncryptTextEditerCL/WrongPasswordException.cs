using System;

namespace EncryptTextEditerCL
{
    public class WrongPasswordException : Exception
    {
        public WrongPasswordException()
            : base("The password is incorrect.") { }
    }
}
