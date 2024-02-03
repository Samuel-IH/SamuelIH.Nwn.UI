namespace SamuelIH.Nwn.UI;

public class UiEvent
{
    public string Id { get; internal set; } = "";

    internal IUiComponent? Component { get; set; }

    public static implicit operator string(UiEvent uiEvent)
    {
        if (uiEvent.Id == "") throw new Exception("GoonEvent has not been registered yet.");
        return uiEvent.Id;
    }
}