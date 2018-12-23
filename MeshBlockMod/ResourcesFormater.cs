using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using UnityEngine;


public class ResourcesFormater
{
    //自带预制模型的路径
    public static string PrefabPath = @"Resources\Prefab";

    //用户自定模型路径
    public static string CustomPath = "";




    public ResourcesFormater()
    {
        ReadMeshs(PrefabPath);

    }


    void ReadMeshs(string path,bool data = false)
    {
        List<ModMesh> modMeshes = new List<ModMesh>();

        string[] vs = ModIO.GetFiles(path, data);

        Debug.Log(vs.Length);
        Debug.Log(vs[0]);
        Console.WriteLine(vs[0]);
   

        
        //ModResource.CreateMeshResource("",)

        //foreach (var str in vs)
        //{
        //    Debug.Log(str.ToString());
        //}

    }

    //List<ModTexture> ReadTextures(string path,bool data = false )
    //{


    //}


}

