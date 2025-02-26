using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Environment;
using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;
using VehicleVariationPacks.Data;

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
                
                { m_Setting.GetOptionTabLocaleID(Setting.kMainSection), "Settings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kSettingsGroup), "General Settings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCreateVariationPackGroup), "Create Variation Packs" },
                
                {m_Setting.GetOptionLabelLocaleID(nameof(Setting.ImportPackText)), "After creating your pack in the online tool and downloading the file, click on 'Open Variation Pack Folder' and put the downloaded json-file in that folder. Click on 'Reload available Packs' to refresh the list and select your pack in the dropdown menu."},
                
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
                
                {m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenVariationPackCreator)), "Open Variation Pack Creator"},
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenVariationPackCreator)),
                    "Open the Variation Pack Creator in your browser. This tool allows you to create your own variation packs"
                }
            };
        }

        public void Unload()
        {
        }
    }
}
