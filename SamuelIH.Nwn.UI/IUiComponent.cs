using Anvil.API.Events;

namespace SamuelIH.Nwn.UI;

public interface IUiComponent
{
    void Register(BindRegistry registry);
    void OnNuiEvent(ModuleEvents.OnNuiEvent eventData);
}