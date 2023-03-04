namespace Banks.Entities;

public class Notification
{
    public Notification(string message)
    {
        Message = message;
    }

    private string Message { get; }

    public string GetMessage()
    {
        return Message;
    }
}