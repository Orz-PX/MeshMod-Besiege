using System;
using spaar.ModLoader;
using UnityEngine;
using Blocks;
using System.Collections.Generic;

namespace XultimateX.MeshBlockMod
{

    // If you need documentation about any of these values or the mod loader
    // in general, take a look at https://spaar.github.io/besiege-modloader.

    public partial class MeshBlockMod : BlockMod
    {
        public override string Name { get; } = "MeshBlockMod";
        public override string DisplayName { get; } = "Mesh Block Mod";
        public override string Author { get; } = "XultimateX";
        public override Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        // You don't need to override this, if you leavie it out it will default
        // to an empty string.
        public override string VersionExtra { get; } = "";

        // You don't need to override this, if you leave it out it will default
        // to the current version.
        public override string BesiegeVersion { get; } = "v0.45";

        // You don't need to override this, if you leave it out it will default
        // to false.
        public override bool CanBeUnloaded { get; } = false;

        // You don't need to override this, if you leave it out it will default
        // to false.
        public override bool Preload { get; } = false;

        public static string PathMesh = Application.dataPath + "/Mods/Blocks/Resources/MeshBlockMod/Mesh";
        public static string PathTexture = Application.dataPath + "/Mods/Blocks/Resources/MeshBlockMod/Texture";

        public static List<NeededResource> LNR;

        //public static List<string> ObjName = new List<string>();
        //public static List<string> TextureName = new List<string>();

        public override void OnLoad()
        {
            // Your initialization code here

            new GameObject().AddComponent<Updater>();

            //LNR = NR(Application.dataPath + "/Mods/Blocks/Resources/MeshBlockMod/");

            LoadBlock(MeshBlock);

        }

        public override void OnUnload()
        {
            // Your code here
            // e.g. save configuration, destroy your objects if CanBeUnloaded is true etc

            
        }
      
    }
}
