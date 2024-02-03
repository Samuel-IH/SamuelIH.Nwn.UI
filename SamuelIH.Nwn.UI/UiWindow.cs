using Anvil.API;
using Anvil.API.Events;
using Goon.ComponentSystem;
using NLog;

namespace SamuelIH.Nwn.UI;

public abstract class UiWindow : PlayerComponent
{
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

    private NuiWindowToken _currentToken;
    private BindRegistry _registry = new();
    protected abstract string WindowId { get; }
    protected abstract void Register(BindRegistry registry);
    protected abstract NuiWindow Build();

    protected override void OnAwake()
    {
        base.OnAwake();
        var player = Entity;
        RegisterListener(() => player.OnNuiEvent += _OnNuiEvent, () => player.OnNuiEvent -= _OnNuiEvent);
    }

    public void Show()
    {
        _registry = new BindRegistry();
        Register(_registry);
        var window = Build();
        if (Entity.TryCreateNuiWindow(window, out var token, WindowId))
        {
            _currentToken = token;
            _registry.SetTokenForWaitingBinds(token);
        }
    }

    public void Close()
    {
        _currentToken.Close();
    }

    private void _OnNuiEvent(ModuleEvents.OnNuiEvent eventData)
    {
        if (eventData.Token.Token == _currentToken.Token)
        {
            if (_registry.HandleEventData(eventData)) return;
            OnNuiEvent(eventData);
        }
    }

    /// <summary>
    ///     Handles events that are not delegated out to components.
    ///     This includes window events, of course.
    /// </summary>
    protected virtual void OnNuiEvent(ModuleEvents.OnNuiEvent eventData)
    {
    }
}