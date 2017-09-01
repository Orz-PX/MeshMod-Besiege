﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace XultimateX.MeshBlockMod
{

    /// <summary>
    /// 资源格式化
    /// </summary>
    public class NeedResourceFormat
    {

        /// <summary>
        /// 模组名
        /// </summary>
        public static string ModName = Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// 模组资源文件夹路径
        /// </summary>
        public static string ModResourcePath { get; } = "/" + ModName + "/";

        /// <summary>
        /// 模组资源文件夹全路径
        /// </summary>
        public static string ModResourceFullPath { get; private set; } = Application.dataPath + "/Mods/Blocks/Resources/" + ModName + "/";

        /// <summary>
        /// 资源列表
        /// </summary>
        public List<NeededResource> NeedResources = new List<NeededResource>();

        /// <summary>
        /// 网格名列表
        /// </summary>
        public List<string> MeshNames = new List<string>();

        /// <summary>
        /// 网格全名列表
        /// </summary>
        public List<string> MeshFullNames = new List<string>();

        /// <summary>
        /// 贴图名列表
        /// </summary>
        public List<string> TextureNames = new List<string>();

        /// <summary>
        /// 贴图全名列表
        /// </summary>
        public List<string> TextureFullNames = new List<string>();


        /// <summary>
        /// 获取资源名列表
        /// </summary>
        public NeedResourceFormat()
        {
            ResourceFormat();
        }

        public NeedResourceFormat(bool Perfabs)
        {
            PerfabsResourceFormat();
            ResourceFormat();
        }

        /// <summary>
        /// 资源格式化
        /// </summary>
        void ResourceFormat()
        {


            if (Directory.Exists(ModResourceFullPath))
            {
                FileInfo[] files = new DirectoryInfo(ModResourceFullPath).GetFiles("*", SearchOption.TopDirectoryOnly);
#if DEBUG
                Debug.Log("文件数量" + files.Length);
#endif
                for (int i = 0; i < files.Length; i++)
                {

                    string Name = files[i].Name;
                    string Fullname = ModResourcePath + Name;

                    if (Fullname.EndsWith(".obj"))
                    {                  
                        NeedResources.Add(new NeededResource(ResourceType.Mesh, Fullname));
                        MeshNames.Add(Name.Substring(0, Name.Length - 4));
                        MeshFullNames.Add(Fullname);
                        continue;
                    }

                    if (files[i].Name.EndsWith(".png"))
                    {
                        NeedResources.Add(new NeededResource(ResourceType.Texture, Fullname));
                        TextureNames.Add(Name.Substring(0, Name.Length - 4));
                        TextureFullNames.Add(Fullname);
                        continue;
                    }


                }
            }
            else
            {
                Directory.CreateDirectory(ModResourceFullPath);
            }
        }


        void PerfabsResourceFormat()
        {
            if (Directory.Exists(ModResourceFullPath + "Perfabs/"))
            {
                FileInfo[] files = new DirectoryInfo(ModResourceFullPath + "Perfabs/").GetFiles("*", SearchOption.AllDirectories);
#if DEBUG
                Debug.Log("文件数量" + files.Length);
#endif
                for (int i = 0; i < files.Length; i++)
                {

                    string Name = files[i].Name;
                    string Fullname = ModResourcePath + "Perfabs/" + Name;

                    if (Fullname.EndsWith(".obj"))
                    {
                        NeedResources.Add(new NeededResource(ResourceType.Mesh, Fullname));
                        MeshNames.Add(Name.Substring(0, Name.Length - 4));
                        MeshFullNames.Add(Fullname);
                        continue;
                    }

                    if (files[i].Name.EndsWith(".png"))
                    {
                        NeedResources.Add(new NeededResource(ResourceType.Texture, Fullname));
                        TextureNames.Add(Name.Substring(0, Name.Length - 4));
                        TextureFullNames.Add(Fullname);
                        continue;
                    }


                }
            }
            else
            {
                Directory.CreateDirectory(ModResourceFullPath);
            }
        }
    }

}
