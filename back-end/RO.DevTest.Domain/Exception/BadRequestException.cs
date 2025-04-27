using RO.DevTest.Domain.Exception;

namespace RO.DevTest.Domain.Exception
{
    /// <summary>
    /// Exceção para requisições inválidas
    /// </summary>
    public class BadRequestException : System.Exception
    {
        public BadRequestException() : base() { }
        
        public BadRequestException(string message) : base(message) { }
        
        public BadRequestException(string message, System.Exception innerException) 
            : base(message, innerException) { }
    }
}
