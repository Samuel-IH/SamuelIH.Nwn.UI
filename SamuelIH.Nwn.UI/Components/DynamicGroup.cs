using Anvil.API;

namespace SamuelIH.Nwn.UI.Components;

/// <summary>
///     A group that can have its contents replaced.
/// </summary>
public class DynamicGroup : UiComponent<NuiGroup>
{
    private UiComponent<NuiLayout> _contents;
    private readonly NuiGroup _group;
    private readonly UiEvent _groupId = new();

    private BindRegistry? _registry;

    public DynamicGroup(UiComponent<NuiLayout> contents, NuiGroup? group = null)
    {
        _contents = contents;
        _group = group ?? new NuiGroup();
    }

    public UiComponent<NuiLayout> Contents
    {
        get => _contents;
        set
        {
            var oldContents = _contents;
            _contents = value;

            if (_registry == null) return;
            _registry.Remove(oldContents);
            _registry.Add(_contents);

            if (_registry.Token is not NuiWindowToken token) return;

            token.SetGroupLayout(_group, _contents);
            _registry.SetTokenForWaitingBinds(token);
        }
    }

    public override void Register(BindRegistry registry)
    {
        _registry = registry;
        registry.Add(_groupId);
        registry.Add(_contents);
    }

    public override NuiGroup Build()
    {
        _group.Id = _groupId;
        _group.Layout = _contents;
        return _group;
    }
}