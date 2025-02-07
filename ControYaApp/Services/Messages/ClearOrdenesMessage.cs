using CommunityToolkit.Mvvm.Messaging.Messages;

public class ClearDataMessage : ValueChangedMessage<string>
{
    public ClearDataMessage(string value) : base(value)
    {
    }
}
