using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Environment;
using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;
using VehicleVariationPacks.Data;
using VehicleVariationPacks.Systems;

namespace VehicleVariationPacks
{
    [FileLocation($"ModsSettings/{nameof(VehicleVariationPacks)}/{nameof(VehicleVariationPacks)}")]
    [SettingsUIGroupOrder(kSettingsGroup, kCreateVariationPackGroup)]
    [SettingsUIShowGroupName(kSettingsGroup, kCreateVariationPackGroup)]
    
    public class Setting : ModSetting
    {
        public static Setting Instance;
        public const string kMainSection = "Settings";
        public const string kSettingsGroup = "General Settings";
        public const string kCreateVariationPackGroup = "Create Variation Pack";
        public Setting(IMod mod) : base(mod)
        {

        }

        private string _packDropdown = "Realistic Global";
        private static int PackDropdownItemsVersion { get; set; }

        [SettingsUIDropdown(typeof(Setting), nameof(GetNameDropdownItems))]
        [SettingsUIValueVersion(typeof(Setting), nameof(PackDropdownItemsVersion))]
        [SettingsUISection(kMainSection, kSettingsGroup)]
        public string PackDropdown
        {
            get
            {
                return _packDropdown;
            }
            set
            {
                _packDropdown = value;
                if (value != null)
                {
                    VehicleVariationChangerSystem.Instance.LoadVariationPack(value);
                }
            }
        }

        public DropdownItem<string>[] GetNameDropdownItems()
        {
            var names =  VariationPack.GetVariationPackNames();

            List<DropdownItem<string>> items = new List<DropdownItem<string>>();
            foreach(string s in names)
            {
                items.Add(new DropdownItem<string>()
                {
                    value = s,
                    displayName = s,
                });
            }

            /*items.Add(new DropdownItem<string>()
            {
                value = "debug_ColorComponents1",
                displayName = "ColorComponents1",
            });
            items.Add(new DropdownItem<string>()
            {
                value = "debug_ColorComponents2",
                displayName = "ColorComponents2",
            });
            items.Add(new DropdownItem<string>()
            {
                value = "debug_ColorComponents3",
                displayName = "ColorComponents3",
            });
            items.Add(new DropdownItem<string>()
            {
                value = "debug_Test",
                displayName = "RGB",
            });*/

            return items.ToArray();
        }
        
        [SettingsUISection(kMainSection, kSettingsGroup)]
        public bool OpenPacksFolder
        {
            set
            {
                var file = EnvPath.kUserDataPath + "/ModsData/VehicleVariationPacks/packs";
                var parentDir = Directory.GetParent(file).FullName;
                Process.Start(Path.Combine(parentDir, "packs"));
            }
        }

        [SettingsUISection(kMainSection, kSettingsGroup)]
        public bool ReloadPacks
        {
            set
            {
                PackDropdownItemsVersion++;
            }
        }

        [SettingsUIButton]
        [SettingsUISection(kMainSection, kCreateVariationPackGroup)]
        public bool OpenVariationPackCreator
        {
            set => Process.Start("https://variationpackcreator.mimonsi.de");
        }

        [SettingsUIMultilineText]
        [SettingsUISection(kMainSection, kCreateVariationPackGroup)]
        public string ImportPackText => string.Empty;

        public override void SetDefaults()
        {
            
        }
    }
}
