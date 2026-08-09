using System;

namespace EncryptTextEditerCL
{
    public class VaultCorruptException : Exception
    {
        public VaultCorruptException(Exception innerException)
            : base("The options file is corrupted and cannot be read.", innerException) { }
    }
}
