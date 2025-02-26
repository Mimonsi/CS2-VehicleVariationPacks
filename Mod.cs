using System.IO;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Colossal.Logging;
using Colossal.PSI.Environment;
using Game;
using Game.Modding;
using Game.SceneFlow;
using VehicleVariationPacks.Systems;

namespace VehicleVariationPacks
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(VehicleVariationPacks)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        private Setting m_Setting;
        private string path;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                path = asset.path;

            CopyEmbeddedPacks();
            updateSystem.UpdateAt<VehicleVariationChangerSystem>(SystemUpdatePhase.MainLoop);

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            foreach (var item in new LocaleHelper("VehicleVariationPacks.Locale.json").GetAvailableLanguages())
            {
                GameManager.instance.localizationManager.AddSource(item.LocaleId, item);
            }

            AssetDatabase.global.LoadSettings(nameof(VehicleVariationPacks), m_Setting, new Setting(this));
            Setting.Instance = m_Setting;
        }

        private void CopyEmbeddedPacks()
        {
            var modPath = Path.GetDirectoryName(path);
            var srcPath = Path.Combine(modPath, "packs");
            var destPath = Path.Combine(EnvPath.kUserDataPath, "ModsData", nameof(VehicleVariationPacks), "packs");
            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);
            foreach(var file in Directory.GetFiles(srcPath))
            {
                var destFile = Path.Combine(destPath, Path.GetFileName(file));
                if (!File.Exists(destFile))
                    File.Copy(file, destFile);
            }
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}