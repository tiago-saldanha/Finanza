namespace Finanza.Application.Exceptions
{
    public class AccountNameAppException : Exception
    {
        public AccountNameAppException() : base("O nome da conta é obrigatório")
        {
        }
    }
}
