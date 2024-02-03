using System.Numerics;
using Anvil.API;
using NLog;

namespace SamuelIH.Nwn.UI.Components;

public class ItemRenderer : UiComponent<NuiImage>
{
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private readonly float _height;
    private readonly UiBind<bool> _isComposite = new();
    private readonly UiBind<bool> _isVisible = new();

    private readonly UiBind<string> _layer1 = new();
    private readonly UiBind<string> _layer2 = new();
    private readonly UiBind<string> _layer3 = new();

    private readonly float _width;

    public ItemRenderer(float width, float height)
    {
        _width = width;
        _height = height;
    }

    protected void BindItem(ItemLayerData? layerData)
    {
        if (layerData == null)
        {
            _isVisible.Value = false;
            return;
        }

        _layer1.Value = layerData.layer1;
        _layer2.Value = layerData.layer2;
        _layer3.Value = layerData.layer3;
        _isComposite.Value = layerData.isComposite;
    }

    public override NuiImage Build()
    {
        var image = new NuiImage(_layer1)
        {
            ImageAspect = NuiAspect.Fit,
            HorizontalAlign = NuiHAlign.Center,
            VerticalAlign = NuiVAlign.Top,

            Width = _width,
            Height = _height
        };

        var rect = new NuiRect(0, 0, _width, _height);
        image.DrawList = new List<NuiDrawListItem>
        {
            new NuiDrawListImage(_layer2, rect)
            {
                Enabled = _isComposite,
                Aspect = NuiAspect.Fit,
                HorizontalAlign = NuiHAlign.Center,
                VerticalAlign = NuiVAlign.Top
            },
            new NuiDrawListImage(_layer3, rect)
            {
                Enabled = _isComposite,
                Aspect = NuiAspect.Fit,
                HorizontalAlign = NuiHAlign.Center,
                VerticalAlign = NuiVAlign.Top
            }
        };

        image.Visible = _isVisible;
        return image;
    }

    private Vector2 IconSizeForBaseItem(NwBaseItem baseItem)
    {
        var size = baseItem.InventorySlotSize;
        var scale = 100f / size.Y;
        if (size.X > size.Y) scale = 100f / size.X;
        return new Vector2(size.X * scale, size.Y * scale);
    }

    public override void Register(BindRegistry registry)
    {
        registry.Add(_layer1);
        registry.Add(_layer2);
        registry.Add(_layer3);
        registry.Add(_isComposite);
    }

    protected ItemLayerData GetLayerDataFor(NwBaseItem baseItem, IReadOnlyList<ushort> modelParts)
    {
        var layerData = new ItemLayerData();

        if (baseItem.ModelType == BaseItemModelType.Composite && modelParts.Count >= 3)
        {
            var defIcon = baseItem.DefaultIcon;
            layerData.isComposite = true;
            layerData.layer1 = $"{defIcon}_b_{modelParts[0]:000}";
            layerData.layer2 = $"{defIcon}_m_{modelParts[1]:000}";
            layerData.layer3 = $"{defIcon}_t_{modelParts[2]:000}";
            return layerData;
        }

        if (baseItem.ItemType == BaseItemType.Cloak)
        {
            layerData.layer1 = "iit_cloak";
            return layerData;
        }

        if (baseItem.ItemType == BaseItemType.SpellScroll || baseItem.ItemType == BaseItemType.EnchantedScroll)
        {
            // foreach (var prop in item.ItemProperties) {
            //     if (prop.PropertyType == ItemPropertyType.CastSpell) {
            //         layerData.layer1 = GoonUIPlugin.SpellProps[prop.SubType].Icon;
            //         break;
            //     }
            // }
            // return layerData;
        }

        if (baseItem.ModelType == BaseItemModelType.Simple && modelParts.Count >= 1)
        {
            var id = $"{modelParts[0]:000}";
            var defIcon = baseItem.DefaultIcon;
            if (defIcon.LastIndexOf('_') == defIcon.Length - 4) defIcon = defIcon[..^4];

            switch (baseItem.ItemType)
            {
                case BaseItemType.MiscSmall:
                case BaseItemType.CraftMaterialSmall:
                    defIcon = "iit_smlmisc";
                    break;
                case BaseItemType.MiscMedium:
                case BaseItemType.CraftMaterialMedium:
                case (BaseItemType)112:
                    defIcon = "iit_midmisc";
                    break;
                case BaseItemType.MiscLarge:
                    defIcon = "iit_talmisc";
                    break;
                case BaseItemType.MiscThin:
                    defIcon = "iit_thnmisc";
                    break;
            }

            layerData.layer1 = defIcon + "_" + id;
            return layerData;
        }

        layerData.layer1 = baseItem.DefaultIcon;
        return layerData;
    }


    protected class ItemLayerData
    {
        public bool isComposite;
        public string layer1 = "";
        public string layer2 = "";
        public string layer3 = "";
    }
}