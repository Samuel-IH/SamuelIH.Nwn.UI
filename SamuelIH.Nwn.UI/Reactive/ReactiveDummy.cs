namespace SamuelIH.Nwn.UI.Reactive;

public class ReactiveDummy<T> : IReactiveGettable<T>
{
    public ReactiveDummy(T value)
    {
        Value = value;
    }

    public event EventHandler<T>? OnChange;
    public T Value { get; }
}