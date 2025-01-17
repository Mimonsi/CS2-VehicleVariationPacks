using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Environment;
using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;

namespace VehicleVariationPacks
{
    [FileLocation($"ModsSettings/{nameof(VehicleVariationPacks)}/{nameof(VehicleVariationPacks)}")]
    public class Setting : ModSetting
    {
        public static Setting Instance;
        public Setting(IMod mod) : base(mod)
        {

        }

        [SettingsUIHidden]
        public bool HiddenSetting { get; set; }

        public bool OpenPacksFolder
        {
            set
            {
                var file = EnvPath.kUserDataPath + "/ModsData/VehicleVariationPacks/packs";
                var parentDir = Directory.GetParent(file).FullName;
                Process.Start(Path.Combine(parentDir, "packs"));
            }
        }

        public bool ReloadPacks
        {
            set
            {
                PackDropdownItemsVersion++;
            }
        }

        private string _packDropdown = "Realistic Global";
        private static int PackDropdownItemsVersion { get; set; }

        [SettingsUIDropdown(typeof(Setting), nameof(GetNameDropdownItems))]
        [SettingsUIValueVersion(typeof(Setting), nameof(PackDropdownItemsVersion))]
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

        public override void SetDefaults()
        {
            HiddenSetting = true;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Vehicle Variation Packs" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenPacksFolder)), "Open Packs Folder" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenPacksFolder)),
                    $"Opens the folder where the packs are stored, allowing you to add your own"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ReloadPacks)), "Reload available Packs" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ReloadPacks)),
                    $"Reloads the available packs installed in your packs folder"
                },


                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PackDropdown)), "Active Pack" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.PackDropdown)),
                    $"Choose which Variation Pack to use. If you manually installed a pack and it is not displayed here, click on 'Reload available Packs' to refresh the list"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
