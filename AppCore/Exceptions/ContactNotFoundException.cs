namespace AppCore.Exceptions;

public class ContactNotFoundException : Exception
{
    public ContactNotFoundException(string msg) : base(msg)
    {
    }
}