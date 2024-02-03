using Anvil.API;

namespace SamuelIH.Nwn.UI;

public class UiBindList<T> : IBindable
{
    private NuiBind<T>? _bind;

    private List<T>? _initialValue;

    private NuiWindowToken? _token;

    public UiBindList()
    {
    }

    public UiBindList(List<T> initialValue) : this()
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

    public List<T>? Value
    {
        get
        {
            if (!_token.HasValue || _bind == null)
                return _initialValue;
            try
            {
                return Bind.GetBindValues(_token.Value.Player, _token.Value.Token);
            }
            catch (Exception _)
            {
                return _initialValue;
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
                    _bind.SetBindValues(_token.Value.Player, _token.Value.Token, value);
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

        if (_initialValue != null) Bind.SetBindValues(token.Player, token.Token, _initialValue);
    }

    public static implicit operator NuiBind<T>(UiBindList<T> g)
    {
        return g.Bind;
    }
}