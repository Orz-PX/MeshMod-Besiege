using spaar.ModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace XultimateX.MeshBlockMod
{
    partial class MeshBlockMod
    {

        public Block MeshBlock = new Block()
        #region 网格模块 基本属性  

           //模块 ID
           .ID(655)

           //模块 名称
           .BlockName("Mesh Block")

           //模块 模型信息
           .Obj(new System.Collections.Generic.List<Obj> { new Obj("/MeshBlockMod/Cube.obj", "/MeshBlockMod/Cube.png", new VisualOffset(Vector3.one * 0.325f, new Vector3(0, 0, 0.5f), Vector3.zero)) })

           //模块 图标
           .IconOffset(new Icon(0.5f, Vector3.zero, new Vector3(45,0,45)))

           //模块 组件
           .Components(new Type[] { typeof(MeshBlockScript) })

           //模块 设置模块属性
           .Properties( new BlockProperties().SearchKeywords(new string[] { "Mesh", "网格" }))

           //模块 质量
           .Mass(0.5f)

            //模块 是否显示碰撞器
#if DEBUG
            .ShowCollider(true)
#endif
           //模块 碰撞器
           .CompoundCollider(new System.Collections.Generic.List<ColliderComposite>
                                {
                                    //方块碰撞器
                                    ColliderComposite.Box(Vector3.one, new Vector3(0,0,0.5f), Vector3.zero)
                                }
                             )

           //模块 载入资源
           .NeededResources(new System.Collections.Generic.List<NeededResource>
                               {
                                    //new NeededResource(ResourceType.Texture,"/MordenFirearmKitMod/RocketFire.png"),
                                    //new NeededResource(ResourceType.Texture,"/MordenFirearmKitMod/RocketSmoke.png")
                                })

           //模块 连接点
           .AddingPoints(new System.Collections.Generic.List<AddingPoint>
                            {

                                new BasePoint(true,true) //底部连接点。第一个是指你能不能将其他模块安在该模块底部。第二个是指这个点是否是在开局时粘连其他链接点
                                                .Motionable(false,false,false) //底点在X，Y，Z轴上是否是能够活动的。
                                                .SetStickyRadius(0.25f) //粘连距离
               
                                ,new AddingPoint(
                                                new Vector3(  0f,  0f,  0.5f), //位置
                                                new Vector3(-90f,  0f,  0f), //旋转
                                                false                       //这个点是否是在开局时粘连其他链接点
                                                ).SetStickyRadius(0.15f)      //粘连距离
                                ,new AddingPoint(new Vector3(0,0,0.5f),new Vector3(0,0,0),false).SetStickyRadius(0.15f)
                                ,new AddingPoint(new Vector3(0,0,0.5f),new Vector3(0,0,90),false).SetStickyRadius(0.15f)
                                ,new AddingPoint(new Vector3(0,0,0.5f),new Vector3(0,0,180),false).SetStickyRadius(0.15f)
                                ,new AddingPoint(new Vector3(0,0,0.5f),new Vector3(0,0,270),false).SetStickyRadius(0.15f)

                            });

        #endregion;

        
    }

    //网格模块脚本
    public class MeshBlockScript : BlockScript
    {

        MSlider RotationXSlider;
        MSlider RotationYSlider;
        MSlider RotationZSlider;
        public float RotationX = 0;
        public float RotationY = 0;
        public float RotationZ = 0;

        MMenu HardnessMenu;
        public int Hardness = 1;

        MToggle MassFormSizeToggle;
        MSlider MassSlider;
        public bool MassFormSize = false;
        public float Mass = 0.5f;

        MVisual SkinsVisual;


        MeshRenderer MR;

        ConfigurableJoint CJ;

        Rigidbody RB;

        string PathMesh = Application.dataPath + "/Mods/Blocks/Resources/MeshBlockMod/Mesh";
        string PathTexture = Application.dataPath + "/Mods/Blocks/Resources/MeshBlockMod/Texture";

        public override void SafeAwake()
        {
            base.SafeAwake();

            MR = GetComponentsInChildren<MeshRenderer>().ToList().Find(match => match.name == "Vis");
            CJ = GetComponent<ConfigurableJoint>();
            RB = GetComponent<Rigidbody>();

            RotationXSlider = AddSlider("旋转X轴", "RotationX", RotationX, -180f, 180f);
            RotationYSlider = AddSlider("旋转Y轴", "RotationY", RotationY, -180f, 180f);
            RotationZSlider = AddSlider("旋转Z轴", "RotationZ", RotationZ, -180f, 180f);
            RotationXSlider.ValueChanged += (float value) => { RotationX = value; ChangedRotation(); };
            RotationYSlider.ValueChanged += (float value) => { RotationY = value; ChangedRotation(); };
            RotationZSlider.ValueChanged += (float value) => { RotationZ = value; ChangedRotation(); };

            HardnessMenu = AddMenu("Hardness", Hardness, new List<string>() { "无碳钢","低碳钢", "中碳钢", "高碳钢" });
            HardnessMenu.ValueChanged += (int value) => { Hardness = value;ChangedHardness(); };
            ChangedHardness();

            MassFormSizeToggle = AddToggle("尺寸决定质量", "MassFormSize", MassFormSize);
            MassFormSizeToggle.Toggled += (bool value) => { MassFormSize = value;};
            MassSlider = AddSlider("质量", "Mass", Mass, 0.2f, 2f);
            MassSlider.ValueChanged += (float value) => { Mass = value;};

            //SkinsVisual = new MVisual(GetComponent<BlockVisualControllerExtended>(), 0, new List<BlockSkinLoader.SkinPack.Skin>() {new BlockSkinLoader.SkinPack.Skin() });
            GetMeshs(PathMesh);

        }

        void ChangedRotation()
        {
            MR.transform.rotation = Quaternion.Euler(new Vector3(RotationX,RotationY,RotationZ));
        }

        void ChangedHardness()
        {
            CJ.projectionMode = JointProjectionMode.PositionAndRotation;
            if (Hardness == 0)
            {
                CJ.projectionAngle =10;
                CJ.projectionDistance = 5;
            }
            if (Hardness == 1)
            {
                CJ.projectionAngle =  5;
                CJ.projectionDistance = 2;
            }
            if (Hardness == 2)
            {
                CJ.projectionAngle =  2;
                CJ.projectionDistance = 0.5f;
            }
            if (Hardness == 3)
            {
                CJ.projectionAngle = CJ.projectionDistance = 0;
            }
        }

        void ChangedMass()
        {
            RB.mass = Mass * (MassFormSize ? transform.localScale.x * transform.localScale.y * transform.localScale.z : 1f);
        }

        void GetMeshs(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo direction = new DirectoryInfo(path);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

                Debug.Log(files.Length);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    Debug.Log("Name:" + files[i].Name);
                    Debug.Log( "FullName:" + files[i].FullName );  
                    Debug.Log( "DirectoryName:" + files[i].DirectoryName );  
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
            
        }

        protected virtual System.Collections.IEnumerator UpdateMapper()
        {
            if (BlockMapper.CurrentInstance == null)
                yield break;
            while (Input.GetMouseButton(0))
                yield return null;
            BlockMapper.CurrentInstance.Copy();
            BlockMapper.CurrentInstance.Paste();
            yield break;
        }

        public override void OnSave(XDataHolder stream)
        {
            base.OnSave(stream);
            SaveMapperValues(stream);
        }

        public override void OnLoad(XDataHolder stream)
        {
            base.OnLoad(stream);
            LoadMapperValues(stream);
        }

        protected override void BuildingUpdate()
        {
            base.BuildingUpdate();
            ChangedMass();
        }

        protected override void OnSimulateStart()
        {
            base.OnSimulateStart();
            //foreach (AddingPoint ap in )
            //{
            //    Debug.Log(ap.sticky);
            //}
            
        }


    }
}
