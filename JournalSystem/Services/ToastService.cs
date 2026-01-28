public enum ToastType { Success, Error, Info }

public record ToastMessage(string Message, ToastType Type);

public class ToastService
{
    public event Action<ToastMessage>? OnShow;

    // show toast message
    public void Show(string message, ToastType type)
        => OnShow?.Invoke(new ToastMessage(message, type));
}
