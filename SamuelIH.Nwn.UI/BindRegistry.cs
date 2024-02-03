using Anvil.API;
using Anvil.API.Events;

namespace SamuelIH.Nwn.UI;

/// <summary>
///     Keeps track of binds and events for a window hierarchy.
/// </summary>
public class BindRegistry
{
    // Binds that are waiting for a token to be set.
    private readonly List<IBindable> _bindsAwaitingToken = new();

    /// <summary>
    ///     Depth-stack of components that are currently being registered.
    ///     This is used to make any events that are registered inside a component automatically have that component as their
    ///     target.
    /// </summary>
    private readonly List<IUiComponent> _components = new();

    // Events are ids that are "compressed" into strings that are (almost) as short as possible.
    // They are used to identify which event handler to call when an event is received.
    private readonly Dictionary<string, UiEvent> _events = new();

    private int _currentBindId;
    private int _currentEventId;

    private bool _isDeregistering;

    internal BindRegistry()
    {
    }

    public NuiWindowToken? Token { get; private set; }

    public void Add(UiEvent uiEvent)
    {
        if (_isDeregistering)
        {
            if (_events.ContainsKey(uiEvent.Id)) _events.Remove(uiEvent.Id);
            return;
        }

        if (_components.Count > 0) uiEvent.Component = _components[_components.Count - 1];
        uiEvent.Id = _currentEventId.ToString("X");
        _currentEventId++;
        _events.Add(uiEvent.Id, uiEvent);
    }

    public void Add(IBindable bind)
    {
        if (_isDeregistering) return;

        bind.SetName(_currentBindId.ToString("X"));
        DeferToken(bind);
        _currentBindId++;
    }

    public void Add(IEnumerable<IBindable> binds)
    {
        foreach (var bind in binds) Add(bind);
    }

    public void Add(IUiComponent component)
    {
        _components.Add(component);
        component.Register(this);
        _components.RemoveAt(_components.Count - 1);
    }

    public void Add(IEnumerable<IUiComponent> components)
    {
        foreach (var component in components) Add(component);
    }

    public void Remove(IUiComponent component)
    {
        _isDeregistering = true;
        component.Register(this);
        _isDeregistering = false;
    }

    private void DeferToken(IBindable bind)
    {
        _bindsAwaitingToken.Add(bind);
    }

    /// <summary>
    ///     Sets the token for all binds that are waiting for one, and removes them from the waiting list.
    /// </summary>
    /// <param name="token"></param>
    public void SetTokenForWaitingBinds(NuiWindowToken token)
    {
        Token = token;
        foreach (var bind in _bindsAwaitingToken) bind.SetToken(token);
        _bindsAwaitingToken.Clear();
    }

    internal bool HandleEventData(ModuleEvents.OnNuiEvent eventData)
    {
        var id = eventData.ElementId.Split('-')[0];
        if (!_events.TryGetValue(id, out var goonEvent)) return false;
        if (goonEvent.Component == null) return false;
        goonEvent.Component?.OnNuiEvent(eventData);
        return true;
    }
}