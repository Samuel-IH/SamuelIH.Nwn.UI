using System.Diagnostics;
using Anvil.API;
using Newtonsoft.Json;
using SamuelIH.Nwn.UI.Reactive;

namespace SamuelIH.Nwn.UI;

/// <summary>
///     A default implementation of IBindable that uses a NuiBind.
/// </summary>
/// <typeparam name="T"></typeparam>
public class UiBind<T> : IBindable
{
    private NuiBind<T>? _bind;

    private T? _initialValue;

    private NuiWindowToken? _token;

    public UiBind()
    {
    }

    public UiBind(T initialValue) : this()
    {
        _initialValue = initialValue;
    }

    public NuiBind<T> Bind
    {
        get
        {
            if (_bind == null)
            {
                // log a warning
            }

            return _bind!;
        }
    }

    public T? Value
    {
        get
        {
            if (!_token.HasValue || _bind == null)
                return _initialValue;
            try
            {
                return Bind.GetBindValue(_token.Value.Player, _token.Value.Token);
            }
            catch (JsonSerializationException e)
            {
                return default;
            }
        }
        set
        {
            if (!_token.HasValue || _bind == null)
            {
                _initialValue = value;
            }
            else
            {
                if (value != null && _token.Value.Player.IsValid)
                    _bind.SetBindValue(_token.Value.Player, _token.Value.Token, value);
            }
        }
    }

    public void SetName(string bindName)
    {
        _bind = new NuiBind<T>(bindName);
    }

    public void SetToken(NuiWindowToken token)
    {
        _token = token;

        if (_initialValue != null) Bind.SetBindValue(token.Player, token.Token, _initialValue);
    }

    public static implicit operator NuiBind<T>(UiBind<T> g)
    {
        return g.Bind;
    }
}

public class ReactiveGetterBind<T> : IBindable
{
    private NuiBind<T> _bind = null!;

    private IReactiveGettable<T> _gettable;

    private NuiWindowToken? _token;

    public ReactiveGetterBind(IReactiveGettable<T> gettable)
    {
        _gettable = gettable;
    }

    public void SetName(string bindName)
    {
        _bind = new NuiBind<T>(bindName);
    }

    public void SetToken(NuiWindowToken token)
    {
        _token = token;
        _token.Value.SetBindValue(_bind, _gettable.Value);
        _gettable.OnChange += OnChange;
    }

    private void OnChange(object? sender, T value)
    {
        if (!_token!.Value.Player.IsValid) return;
        _token!.Value.SetBindValue(_bind, value);
    }

    public void ChangeGettable(IReactiveGettable<T> gettable)
    {
        _gettable.OnChange -= OnChange;
        _gettable = gettable;
        _gettable.OnChange += OnChange;
        _token?.SetBindValue(_bind, _gettable.Value);
    }

    public static implicit operator NuiBind<T>(ReactiveGetterBind<T> g)
    {
        Debug.Assert(g._bind != null,
            "Binds should not be null at this point. Did you convert it to a NuiBind before the registry phase?");
        return g._bind!;
    }
}