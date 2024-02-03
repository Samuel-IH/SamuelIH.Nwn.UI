using Anvil.API;
using NWN.Native.API;

namespace SamuelIH.Nwn.UI.Components;

public class NwItemRenderer : ItemRenderer
{
    private NwItem? _item;

    public NwItemRenderer(float width, float height) : base(width, height)
    {
    }

    public void SetItem(NwItem item)
    {
        _item = item;
        var nativeItem = (CNWSItem)item!;

        BindItem(GetLayerDataFor(item.BaseItem, nativeItem.m_nModelPart));
    }
}