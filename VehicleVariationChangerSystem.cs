using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game;
using Game.Common;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using PersonalCar = Game.Vehicles.PersonalCar;
using PublicTransport = Game.Prefabs.PublicTransport;

namespace VehicleVariationPacks
{
    public partial class VehicleVariationChangerSystem : GameSystemBase
    {
        private static ILog Logger;

        private EntityQuery query;

        //private UIUpdateState uiUpdateState;
        private PrefabSystem prefabSystem;
        public static VehicleVariationChangerSystem Instance { get; private set; }
        private VariationPack _currentVariationPack;

        protected override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
            Enabled = true;
            Logger = Mod.log;

            EntityQueryDesc desc = new EntityQueryDesc
            {
                Any =
                [
                    ComponentType.ReadOnly<CarData>(),
                ]
            };
            query = GetEntityQuery(desc);
            //uiUpdateState = UIUpdateState.Create(World, 1024);
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            SaveDefaultVariations();
            if (_currentVariationPack == null)
            {
                string pack = "Realistic Global";
                if (Setting.Instance != null)
                {
                    pack = Setting.Instance.PackDropdown;
                }
                _currentVariationPack = VariationPack.Load(pack);
            }
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            if (mode == GameMode.MainMenu)
            {
                //UpdatePrefabs();
                SaveDefaultVariations();
                UpdateEntities();
            }
        }

        public static void UpdateEntitiesManually()
        {
            Instance.UpdateEntities();
        }

        public void UpdateEntityInstances()
        {
            EntityQueryDesc desc = new EntityQueryDesc
            {
                Any =
                [
                    ComponentType.ReadOnly<Car>(),
                ],
            };
            var updateQuery = GetEntityQuery(desc);
            foreach (var entity in updateQuery.ToEntityArray(Allocator.Temp))
            {
                EntityManager.AddComponent<BatchesUpdated>(entity);
            }
        }

        private void SaveDefaultVariations()
        {
            var entities = query.ToEntityArray(Allocator.Temp);
            VariationPack pack = new VariationPack();
            foreach (var entity in entities)
            {
                if (EntityManager.HasBuffer<SubMesh>(entity))
                {
                    var subMesh = EntityManager.GetBuffer<SubMesh>(entity);
                    if (subMesh.IsEmpty)
                        continue;
                    if (subMesh[0].m_SubMesh != Entity.Null && EntityManager.HasBuffer<ColorVariation>(subMesh[0].m_SubMesh))
                    {
                        var colorVariations = EntityManager.GetBuffer<ColorVariation>(subMesh[0].m_SubMesh);
                        var prefabName = prefabSystem.GetPrefabName(entity);
                        pack.SavePrefabVariations(prefabName, colorVariations);
                    }
                }
            }
            pack.Name = "Vanilla";
            pack.Save();
        }

        private void UpdateEntities()
        {
            if (_currentVariationPack == null)
                _currentVariationPack = VariationPack.Load("Vanilla");
            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (EntityManager.HasBuffer<SubMesh>(entity))
                {
                    var subMesh = EntityManager.GetBuffer<SubMesh>(entity);
                    if (subMesh.IsEmpty)
                        continue;
                    if (subMesh[0].m_SubMesh != Entity.Null && EntityManager.HasBuffer<ColorVariation>(subMesh[0].m_SubMesh))
                    {
                        var colorVariations = EntityManager.GetBuffer<ColorVariation>(subMesh[0].m_SubMesh);
                        var prefabName = prefabSystem.GetPrefabName(entity);
                        _currentVariationPack.FillColorVariations(prefabName, ref colorVariations);
                    }
                }
            }
        }

        protected override void OnUpdate()
        {

        }

        public void LoadVariationPack(string value)
        {
            if (value.StartsWith("debug_"))
            {
                value = value.Replace("debug_", "");
                if (value == "Test")
                {
                    Instance._currentVariationPack = VariationPack.Test();
                }
                if (value == "CrazyColors")
                {
                    Instance._currentVariationPack = VariationPack.Rdm();
                }
                if (value == "Default")
                {
                    Instance._currentVariationPack = VariationPack.Default();
                }
                if (value == "ColorComponents1")
                {
                    Instance._currentVariationPack = VariationPack.ColorComponents1();
                }
                if (value == "ColorComponents2")
                {
                    Instance._currentVariationPack = VariationPack.ColorComponents2();
                }
                if (value == "ColorComponents3")
                {
                    Instance._currentVariationPack = VariationPack.ColorComponents3();
                }
            }
            else
            {
                Instance._currentVariationPack = VariationPack.Load(value);
            }
            UpdateEntitiesManually();
            UpdateEntityInstances();
        }
    }

}