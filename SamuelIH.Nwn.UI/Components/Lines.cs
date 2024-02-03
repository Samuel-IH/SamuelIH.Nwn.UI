using Anvil.API;

namespace SamuelIH.Nwn.UI.Components;

public class HLine : UiComponent<NuiGroup>
{
    private readonly float? _width;

    public HLine()
    {
    }

    public HLine(float width)
    {
        _width = width;
    }

    public override NuiGroup Build()
    {
        var g = new NuiGroup
        {
            Scrollbars = NuiScrollbars.None,
            Height = 2f,
            Border = true,
            Layout = new NuiColumn()
        };

        if (_width != null) g.Width = _width.Value;

        return g;
    }

    public override void Register(BindRegistry registry)
    {
    }
}

public class VLine : UiComponent<NuiGroup>
{
    private readonly float? _height;

    public VLine()
    {
    }

    public VLine(float height)
    {
        _height = height;
    }

    public override NuiGroup Build()
    {
        var g = new NuiGroup
        {
            Scrollbars = NuiScrollbars.None,
            Width = 2f,
            Border = true,
            Layout = new NuiRow()
        };

        if (_height != null) g.Height = _height.Value;

        return g;
    }

    public override void Register(BindRegistry registry)
    {
    }
}