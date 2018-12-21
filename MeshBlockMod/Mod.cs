using System;
using UnityEngine;
using System.Collections.Generic;
using Modding;
using Modding.Serialization;
using Modding.Levels;

namespace MeshMod
{

    // If you need documentation about any of these values or the mod loader
    // in general, take a look at https://spaar.github.io/besiege-modloader.

    //public partial class MeshBlockMod : BlockMod
    //{
    //    public override string Name { get; } = "MeshBlockMod";
    //    public override string DisplayName { get; } = "Mesh Block Mod";
    //    public override string Author { get; } = "XultimateX";
    //    public override Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

    //    // You don't need to override this, if you leavie it out it will default
    //    // to an empty string.
    //    public override string VersionExtra { get; } = "";

    //    // You don't need to override this, if you leave it out it will default
    //    // to the current version.
    //    public override string BesiegeVersion { get; } = "v0.45";

    //    // You don't need to override this, if you leave it out it will default
    //    // to false.
    //    public override bool CanBeUnloaded { get; } = false;

    //    // You don't need to override this, if you leave it out it will default
    //    // to false.
    //    public override bool Preload { get; } = false;

    //    public static NeedResourceFormat NRF = new NeedResourceFormat(true);

    //    public override void OnLoad()
    //    {
    //        // Your initialization code here

    //        new GameObject().AddComponent<Updater>();         

    //        LoadBlock(MeshBlock);

    //    }

    //    public override void OnUnload()
    //    {
    //        // Your code here
    //        // e.g. save configuration, destroy your objects if CanBeUnloaded is true etc


    //    }

    //}

    public class MeshMod : ModEntryPoint
    {

        public override void OnLoad()
        {
            Events.OnEntityPlaced += (entity) => 
            {
                if (entity.Name == "MeshEntity")
                {
                    AddScript(entity);               
                }
            };

            Events.OnLevelSave += (Level level) =>
            {
                foreach (var e in level.Entities)
                {
                    if (e.Name.Contains("MeshEntity"))
                    {                  
                        MeshEntityData meshEntityData = e.InternalObject.GetComponent<MeshEntityScript>().meshEntityData;
                        level.CustomData.Write("MeshEntityData|" + level.Entities.IndexOf(e), meshEntityData.ToString());
                    }                  
                }
            };

            Events.OnLevelLoaded += (level) =>
            {
                var data = level.CustomData.ReadAll();
                List<MeshEntityData> meshEntityDatas = new List<MeshEntityData>();

                foreach (var da in data)
                {
                    if (da.Key.Contains("MeshEntity"))
                    {
                        MeshEntityData meshEntityData = new MeshEntityData(da.RawValue.ToString());
                        meshEntityDatas.Add(meshEntityData);
                    }
                }

                foreach (var e in level.Entities)
                {
                    if (e.Name.Contains("MeshEntity"))
                    {
                        MeshEntityData meshEntityData = meshEntityDatas.Find(match => match.ID == e.Id);
                        AddScript(e, meshEntityData);
                    }
                }
            };

            void AddScript(Entity entity,MeshEntityData meshEntityData = null)
            {
                MeshEntityScript meshEntityScript = entity.InternalObject.gameObject.AddComponent<MeshEntityScript>();
                meshEntityScript.entity = entity;
                meshEntityScript.meshEntityData = meshEntityData;
            }
        }


       
    }

   

}
