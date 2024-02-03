using Anvil.API;

namespace SamuelIH.Nwn.UI;

public interface IBindable
{
    public void SetName(string bindName);
    public void SetToken(NuiWindowToken token);
}