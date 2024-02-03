using WeakEvent;

namespace SamuelIH.Nwn.UI.Reactive;

/// <summary>
///     A reactive wrapper that you can use to interact with properties. Raises OnChange events when you set the `Value`
///     property.
///     Example usage:
///     <code>
/// public class MyModel
/// {
///     private int _myProperty;
///     public readonly ReactiveProperty&lt;int&gt; MyProperty { get; private set; }
/// 
///     public MyModel()
///     {
///         MyProperty = new ReactiveProperty&lt;int&gt;(this, () => _myProperty, value => _myProperty = value);
///     }
/// }
/// 
/// ...
/// 
/// var model = new MyModel();
/// 
/// // OnChange handler does not retain reference to the delegate, so we need to store it ourselves
/// var handler = (sender, value) => Console.WriteLine($"MyProperty changed to {value}");
/// 
/// model.MyProperty.OnChange += handler;
/// model.MyProperty.Value = 5;
/// </code>
/// </summary>
/// <typeparam name="T"></typeparam>
public class ReactiveProperty<T> : IReactiveGettable<T>
{
    private readonly WeakEventSource<T> _onChange = new();

    private readonly Func<T> _getter;

    private readonly object? _sender;
    private readonly Action<T> _setter;

    public ReactiveProperty(object? owner, Func<T> getter, Action<T> setter)
    {
        _sender = owner;
        _getter = getter;
        _setter = setter;
    }

    public event EventHandler<T> OnChange
    {
        add => _onChange.Subscribe(value);
        remove => _onChange.Unsubscribe(value);
    }

    public T Value
    {
        get => _getter();
        set
        {
            _setter(value);
            _onChange.Raise(_sender, value);
        }
    }

    public static implicit operator T(ReactiveProperty<T> reactiveProperty)
    {
        return reactiveProperty.Value;
    }
}