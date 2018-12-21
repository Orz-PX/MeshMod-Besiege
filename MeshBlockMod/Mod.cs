using System;
using UnityEngine;
using System.Collections.Generic;
using Modding;
using Modding.Serialization;
using Modding.Levels;

namespace MeshMod
{
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
