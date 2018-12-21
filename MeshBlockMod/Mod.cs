using System;
using UnityEngine;
using System.Collections.Generic;
using Modding;
using Modding.Serialization;

namespace XultimateX.MeshBlockMod
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
                Debug.Log("Place..." + entity.Name);
                if (entity.Name == "MeshEntity"/*|| entity.Name == "Modded: MeshEntity"*/)
                {
                    // entity.InternalObject.GetEntityData().;
                    entity.InternalObject.EntityBehaviour.AddSlider("Red", "Red", 0, 0, 255);
                    entity.InternalObject.gameObject.AddComponent<MeshEntityScript>();
                   

                }
            };

            Events.OnLevelSave += (level) =>
            {
                foreach (var e in level.Entities)
                {
                    var sliders = e.InternalObject.EntityBehaviour.Sliders;
                    float value = 0;
                    foreach (var s in sliders)
                    {
                        if (s.Key == "Red")
                        {
                            value = s.Value;
                            break;
                        }
                    }

                    e.InternalObject.GetEntityData().Write("Red", value);

                    ///level.CustomData.Write("color", new tcolor() { color = new Color(1, 1, 1, 1), ID = e.Id }.AttributesUsed);
                }

            };

            Events.OnLevelLoaded += (level) =>
            {
                foreach (var e in level.Entities)
                {
                    Debug.Log("load..." + e.Name);

                    if (e.Name == "Modded: MeshEntity")
                    {
                        //float value = e.InternalObject.GetEntityData().ReadFloat("bmt-Red");

                        var data = e.InternalObject.GetEntityData();
                        //////Debug.Log("??.." + (float)e.InternalObject.blockBehaviour.GetLoadData("bmt-Red").RawValue);
                        
                        if (data.HasKey("bmt-Red"))
                        {
                           
                        }

                        foreach (var da in data.ReadAll())
                        {
                            
                            Debug.Log(da.Key);
                        }
                        //foreach (var k in e.InternalObject.EntityBehaviour.variables)
                        //{
                        //    Debug.Log(k.Key + " " + k.Value);
                        //}

                        //var sliders = e.InternalObject.EntityBehaviour.Sliders;
                        MSlider mSlider = e.InternalObject.EntityBehaviour.AddSlider("Red", "Red", 0, 0, 255);
                        //foreach (var s in sliders)
                        //{
                        //    if (s.Key == "Red")
                        //    {
                        //        mSlider = s;
                        //        break;
                        //    }
                        //}

                        //mSlider.Value = e.InternalObject.GetEntityData().ReadInt("Red");
                    }
                }
            };
        }

        
        public class tcolor:Element,IValidatable
        {
            public long ID { get; set; }
            public Color color { get; set; }
        }
    }

}
