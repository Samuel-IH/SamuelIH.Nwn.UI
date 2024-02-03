namespace SamuelIH.Nwn.UI.Reactive;

/// <summary>
///     A reactive value/wrapper that is gettable, and notifies when the value changes.
/// </summary>
public interface IReactiveGettable<T>
{
    public T Value { get; }
    public event EventHandler<T> OnChange;
}