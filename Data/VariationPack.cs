using System.Collections.Generic;
using System.IO;
using Colossal.PSI.Environment;
using Game.Prefabs;
using Game.Rendering;
using Newtonsoft.Json;
using Unity.Entities;
using UnityEngine;

namespace VehicleVariationPacks.Data
{
    public record PrefabEntry
    {
        public string Name;
        public DynamicBuffer<ColorVariation> ColorVariations;
    }

    public record Entry
    {
        public Color color
        {
            set
            {
                color1 = value;
                color2 = value;
                color3 = value;
            }
        }

        [JsonConverter(typeof(ColorHandler))] public Color color1;
        [JsonConverter(typeof(ColorHandler))] public Color color2;
        [JsonConverter(typeof(ColorHandler))] public Color color3;
        public byte probability;
    }

    public record VariationPack
    {
        public string Name;
        public Dictionary<string, List<Entry>> Entries;

        public VariationPack()
        {

        }

        public static VariationPack Default()
        {
            VariationPack pack = new VariationPack();
            pack.Name = "Default";
            pack.Entries = new Dictionary<string, List<Entry>>();
            pack.Entries["default"] = new List<Entry>
            {
                new()
                {
                    color = Color.white,
                    probability = 26,
                },
                new()
                {
                    color = Color.black,
                    probability = 22,
                },
                new()
                {
                    color = Color.gray,
                    probability = 25,
                },
                new()
                {
                    color = Color.blue,
                    probability = 10,
                },
                new()
                {
                    color = Color.red,
                    probability = 9,
                },
                new()
                {
                    color = Color.red,
                    probability = 1,
                },
                new()
                {
                    color = Color.green,
                    probability = 1,
                },
                new()
                {
                    color = new Color(255, 165, 0),
                    probability = 1,
                },

            };
            return pack;
        }

        public static List<string> GetVariationPackNames()
        {
            var path = Path.Combine(EnvPath.kUserDataPath, "ModsData", nameof(VehicleVariationPacks), "packs");
            if (!Directory.Exists(path))
                return new List<string>();

            var files = Directory.GetFiles(path, "*.json");
            List<string> names = new List<string>();
            /*List<string> names = new List<string>()
            {
                "debug_Default",
                "debug_CrazyColors",
                "debug_Test"
            };*/
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                names.Add(name);
            }

            return names;
        }

        public static VariationPack Rdm()
        {
            VariationPack pack = new VariationPack();
            pack.Name = "Random";
            pack.Entries = new Dictionary<string, List<Entry>>();
            pack.Entries["default"] = new List<Entry>();

            for (int i = 0; i < 1000; i++)
            {

                pack.Entries["default"].Add(new Entry()
                {
                    color1 = new Color(Random.value, Random.value, Random.value),
                    color2 = new Color(Random.value, Random.value, Random.value),
                    color3 = new Color(Random.value, Random.value, Random.value),
                    probability = 1
                });
            }

            return pack;
        }

        public static VariationPack Test()
        {
            VariationPack pack = new VariationPack();
            pack.Name = "Random";
            pack.Entries = new Dictionary<string, List<Entry>>();
            pack.Entries["default"] = new List<Entry>();
            pack.Entries["default"].Add(new Entry()
            {
                color1 = Color.red,
                color2 = Color.green,
                color3 = Color.blue,
                probability = 1
            });

            return pack;
        }

        public static VariationPack ColorComponents1()
        {
            VariationPack pack = new VariationPack();
            pack.Name = "debug_ColorComponents1";
            pack.Entries = new Dictionary<string, List<Entry>>();
            pack.Entries["default"] = new List<Entry>();
            pack.Entries["default"].Add(new Entry()
            {
                color1 = Color.green,
                color2 = new Color(0.718f, 0.196f, 0.671f),
                color3 = new Color(0.718f, 0.196f, 0.671f),
                probability = 1
            });
            return pack;
        }

        public static VariationPack ColorComponents2()
        {
            VariationPack pack = new VariationPack();
            pack.Name = "debug_ColorComponents2";
            pack.Entries = new Dictionary<string, List<Entry>>();
            pack.Entries["default"] = new List<Entry>();
            pack.Entries["default"].Add(new Entry()
            {
                color1 = new Color(0.718f, 0.196f, 0.671f),
                color2 = Color.green,
                color3 = new Color(0.718f, 0.196f, 0.671f),
                probability = 1
            });
            return pack;
        }

        public static VariationPack ColorComponents3()
        {
            VariationPack pack = new VariationPack();
            pack.Name = "debug_ColorComponents3";
            pack.Entries = new Dictionary<string, List<Entry>>();
            pack.Entries["default"] = new List<Entry>();
            pack.Entries["default"].Add(new Entry()
            {
                color1 = new Color(0.718f, 0.196f, 0.671f),
                color2 = new Color(0.718f, 0.196f, 0.671f),
                color3 = Color.green,
                probability = 1
            });
            return pack;
        }

        public void FillColorVariations(string prefabName, ref DynamicBuffer<ColorVariation> buffer)
        {
            if (Entries == null)
            {
                Mod.log.Warn("Entries is null, this should never happen");
                return;
            }

            if (!Entries.ContainsKey(prefabName))
            {
                if (Entries.ContainsKey("default"))
                {
                    buffer.Clear();
                    foreach (Entry e in Entries["default"])
                    {
                        var elem = new ColorVariation
                        {
                            m_ColorSet = new ColorSet()
                            {
                                m_Channel0 = e.color1,
                                m_Channel1 = e.color2,
                                m_Channel2 = e.color3,
                            },
                            m_Probability = e.probability,
                        };
                        buffer.Add(elem);
                    }
                }

                return;
            }

            buffer.Clear();
            foreach (Entry e in Entries[prefabName])
            {
                var elem = new ColorVariation
                {
                    m_ColorSet = new ColorSet()
                    {
                        m_Channel0 = e.color1,
                        m_Channel1 = e.color2,
                        m_Channel2 = e.color3,
                    },
                    m_Probability = e.probability
                };
                buffer.Add(elem);
            }
        }

        public static VariationPack Load(string name)
        {
            Mod.log.Info("Loading pack " + name);
            var path = Path.Combine(EnvPath.kUserDataPath, "ModsData", nameof(VehicleVariationPacks), "packs",
                name + ".json");
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<VariationPack>(json);
        }

        public void Save()
        {
            var path = Path.Combine(EnvPath.kUserDataPath, "ModsData", nameof(VehicleVariationPacks), "packs",
                Name + ".json");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public void SavePrefabVariations(string prefabName, DynamicBuffer<ColorVariation> colorVariations)
        {
            if (Entries == null)
                Entries = new Dictionary<string, List<Entry>>();

            var entries = new List<Entry>();
            foreach (ColorVariation cv in colorVariations)
            {
                entries.Add(new Entry
                {
                    color1 = cv.m_ColorSet[0],
                    color2 = cv.m_ColorSet[1],
                    color3 = cv.m_ColorSet[2],
                    probability = cv.m_Probability
                });
            }

            Entries[prefabName] = entries;
        }
    }
}