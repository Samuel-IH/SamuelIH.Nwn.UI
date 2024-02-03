using WeakEvent;

namespace SamuelIH.Nwn.UI.Reactive;

/// <summary>
///     Transforms a reactive value into another reactive value, and notifies when the value changes.
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class ReactiveTransformer<TIn, TOut> : IReactiveGettable<TOut>
{
    private readonly Func<TIn, TOut> _converter;
    private readonly WeakEventSource<TOut> _onChange = new();

    private IReactiveGettable<TIn> _source; // source needs held to prevent GC

    public ReactiveTransformer(IReactiveGettable<TIn> source, Func<TIn, TOut> converter)
    {
        _source = source;
        _converter = converter;

        Value = converter(source.Value);

        source.OnChange += OnSourceChange;
    }

    public event EventHandler<TOut> OnChange
    {
        add => _onChange.Subscribe(value);
        remove => _onChange.Unsubscribe(value);
    }

    public TOut Value { get; private set; }

    private void OnSourceChange(object? sender, TIn value)
    {
        Value = _converter(value);
        _onChange.Raise(sender, Value);
    }
}

/// <summary>
///     Combines two reactive values into one, and notifies when either of the values change.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class ReactiveTransformer<T1, T2, TOut> : IReactiveGettable<TOut>
{
    private readonly WeakEventSource<TOut> _onChange = new();

    private readonly Func<T1, T2, TOut> _transformer;

    private readonly IReactiveGettable<T1> _value1;
    private readonly IReactiveGettable<T2> _value2;

    /// <summary>
    ///     Creates a new ReactiveGroup that combines two reactive values into one, and notifies when either of the values
    ///     change.
    /// </summary>
    /// <param name="r1"></param>
    /// <param name="r2"></param>
    /// <param name="transformer"></param>
    public ReactiveTransformer(IReactiveGettable<T1> r1, IReactiveGettable<T2> r2, Func<T1, T2, TOut> transformer)
    {
        _transformer = transformer;

        _value1 = r1;
        _value2 = r2;

        r1.OnChange += Update;
        r2.OnChange += Update;

        Value = _transformer(_value1.Value, _value2.Value);
    }

    public event EventHandler<TOut> OnChange
    {
        add => _onChange.Subscribe(value);
        remove => _onChange.Unsubscribe(value);
    }

    public TOut Value { get; private set; }

    private void Update(object? o, T1 t)
    {
        Value = _transformer(_value1.Value, _value2.Value);
        _onChange.Raise(this, Value);
    }

    private void Update(object? o, T2 t)
    {
        Value = _transformer(_value1.Value, _value2.Value);
        _onChange.Raise(this, Value);
    }
}