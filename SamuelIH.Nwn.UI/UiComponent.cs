using Anvil.API;
using Anvil.API.Events;

namespace SamuelIH.Nwn.UI;

public abstract class UiComponent<T> : IUiComponent where T : NuiElement
{
    public abstract void Register(BindRegistry registry);

    public virtual void OnNuiEvent(ModuleEvents.OnNuiEvent eventData)
    {
    }

    public abstract T Build();

    public static implicit operator T(UiComponent<T> component)
    {
        return component.Build();
    }
}