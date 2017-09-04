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
           .IconOffset(new Icon(0.5f, Vector3.zero, new Vector3(45, 0, 45)))

           //模块 组件
           .Components(new Type[] { typeof(MeshBlockScript) })

           //模块 设置模块属性
           .Properties(new BlockProperties().SearchKeywords(new string[] { "Mesh", "网格" }))

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
                                    ColliderComposite.Mesh("/MeshBlockMod/Cube.obj",Vector3.one*0.3f, new Vector3(0,0,0.5f), Vector3.zero)
                                }
                             )

           //模块 载入资源
           .NeededResources(NRF.NeedResources)

           //模块 连接点
           .AddingPoints(new List<AddingPoint>
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
        //声明功能页菜单
        MMenu PageMenu;


               
        //声明基础功能组件
        //声明质量来自尺寸 质量 硬度组件和相关变量
        MToggle MassFormSizeToggle;   
        MSlider MassSlider;
        MMenu HardnessMenu;
        public bool MassFormSize = false;
        public float Mass = 0.5f;
        public int Hardness = 1;

        //声明自定模型功能组件
        //声明网格 贴图 碰撞 碰撞显示 旋转相关组件和相关变量
        MMenu MeshMenu;
        MMenu TextureMenu;
        MMenu ColliderMenu;
        MToggle DisplayColliderToggle;
        MToggle RotationToggle;
        MSlider RotationXSlider;
        MSlider RotationYSlider;
        MSlider RotationZSlider;
        public float RotationX = 0;
        public float RotationY = 0;
        public float RotationZ = 0;


        //声明自定外观功能组件
        //声明 着色器 RGBA 组件
        MMenu ShaderMenu;
        MSlider RedSlider;
        MSlider GreenSlider;
        MSlider BlueSlider;
        MSlider AlphaSlider;

        //声明需要使用的已存在组件
        MeshFilter MF;
        MeshRenderer MR;
        MeshCollider MC; 
        ConfigurableJoint CJ;
        Rigidbody RB;
        NeedResourceFormat NRF = MeshBlockMod.NRF;

        //标记是否打开过mapper
        bool OpenedKeymapper = false;


        public override void SafeAwake()
        {
            base.SafeAwake();
#if DEBUG
            Debug.Log("SafeAwake");
#endif


            #region 获取组件

            MF = GetComponentsInChildren<MeshFilter>().ToList().Find(match => match.name == "Vis");
            MR = GetComponentsInChildren<MeshRenderer>().ToList().Find(match => match.name == "Vis");
            MC = GetComponentsInChildren<MeshCollider>().ToList().Find(match => match.name == "MeshCollider");
            MR.material = new Material(Shader.Find("Diffuse"));
            CJ = GetComponent<ConfigurableJoint>();
            RB = GetComponent<Rigidbody>();

            #endregion

            #region 初始化组件

            //功能页组件
            PageMenu = AddMenu("Page", 0, new List<string>() {"基础功能","自定模型","自定外观" });
            PageMenu.ValueChanged += (int value) => { DisplayInMapper(); };

            //基础功能组件
            //硬度 尺寸决定质量 质量组件
            HardnessMenu = AddMenu("Hardness", Hardness, new List<string>() { "无碳钢", "低碳钢", "中碳钢", "高碳钢" });
            HardnessMenu.ValueChanged += (int value) => { Hardness = value; ChangedHardness(); };           
            MassFormSizeToggle = AddToggle("尺寸决定质量", "MassFormSize", MassFormSize);
            MassFormSizeToggle.Toggled += (bool value) => { MassFormSize = value; };
            MassSlider = AddSlider("质量", "Mass", Mass, 0.2f, 2f);
            MassSlider.ValueChanged += (float value) => { Mass = value; };

            //自定模型组件
            //旋转滑条；网格、贴图、碰撞菜单；碰撞可视相关组件
            //RotationToggle = AddToggle("模型旋转", "Rotation", false);
            RotationXSlider = AddSlider("旋转X轴", "RotationX", RotationX, 0f, 360f);
            RotationYSlider = AddSlider("旋转Y轴", "RotationY", RotationY, 0f, 360f);
            RotationZSlider = AddSlider("旋转Z轴", "RotationZ", RotationZ, 0f, 360f);           
            //RotationToggle.Toggled += (bool value) => { DisplayInMapper(); };
            RotationXSlider.ValueChanged += (float value) => { RotationX = value; ChangedRotation(); };
            RotationYSlider.ValueChanged += (float value) => { RotationY = value; ChangedRotation(); };
            RotationZSlider.ValueChanged += (float value) => { RotationZ = value; ChangedRotation(); };

            MeshMenu = AddMenu("Mesh", 0, MeshBlockMod.NRF.MeshNames);
            MeshMenu.ValueChanged += (int value) => { MF.mesh = resources[MeshBlockMod.NRF.MeshFullNames[value]].mesh; };
            TextureMenu = AddMenu("Texture", 0, MeshBlockMod.NRF.TextureNames);
            TextureMenu.ValueChanged += (int value) => { MR.material.mainTexture = resources[MeshBlockMod.NRF.TextureFullNames[value]].texture; };
            ColliderMenu = AddMenu("Collider", 0, MeshBlockMod.NRF.MeshNames);
            ColliderMenu.ValueChanged += (int value) => { MC.sharedMesh = MC.GetComponent<MeshFilter>().mesh = resources[MeshBlockMod.NRF.MeshFullNames[value]].mesh; };

            DisplayColliderToggle = AddToggle("碰撞可视", "DisplayCollider", false);
            DisplayColliderToggle.Toggled += (bool value) => { MC.GetComponent<MeshRenderer>().enabled = value; };

            //自定外观组件
            //着色菜单；RGBA滑条相关组件
            ShaderMenu = AddMenu("Shader", 0, new List<string>() {"漫反射","透明"});
            ShaderMenu.ValueChanged += (int value) =>{ ChangedShader(); };
            RedSlider = AddSlider("红色通道", "Red", 1f, 0f, 1f);
            GreenSlider = AddSlider("绿色通道", "Green", 1f, 0f, 1f);
            BlueSlider = AddSlider("蓝色通道", "Blue", 1f, 0f, 1f);
            AlphaSlider = AddSlider("Alpha通道", "Alpha", 1f, 0f, 1f);
            RedSlider.ValueChanged += (float value) => { ChangedColor(); };
            GreenSlider.ValueChanged += (float value) => { ChangedColor(); };
            BlueSlider.ValueChanged += (float value) => { ChangedColor(); };
            AlphaSlider.ValueChanged += (float value) => { ChangedColor(); };


            #endregion

            #region 相关组件赋初值

            ChangedHardness();
            DisplayInMapper();
            RefreshVisual();

            #endregion

            if (GetComponent<DestroyJointIfNull>() == null) gameObject.AddComponent<DestroyJointIfNull>();

        }

        //组件显示事件
        void DisplayInMapper()
        {

            HardnessMenu.DisplayInMapper = MassFormSizeToggle.DisplayInMapper = MassSlider.DisplayInMapper = PageMenu.Value == 0;

            MeshMenu.DisplayInMapper = TextureMenu.DisplayInMapper = ColliderMenu.DisplayInMapper = DisplayColliderToggle.DisplayInMapper = PageMenu.Value == 1;
            RotationXSlider.DisplayInMapper = RotationYSlider.DisplayInMapper = RotationZSlider.DisplayInMapper = /*RotationToggle.IsActive &&*/ PageMenu.Value == 1;

            ShaderMenu.DisplayInMapper = RedSlider.DisplayInMapper = GreenSlider.DisplayInMapper = BlueSlider.DisplayInMapper = AlphaSlider.DisplayInMapper = PageMenu.Value == 2;

        }

        //改变旋转事件
        void ChangedRotation()
        {
            if (!OpenedKeymapper)
            {
                MR.transform.rotation = Quaternion.Euler(new Vector3(RotationX, RotationY, RotationZ));
            }
            else
            {
                OpenedKeymapper = false;
            }
            
        }

        //改变硬度事件
        void ChangedHardness()
        {

            CJ.projectionMode = JointProjectionMode.PositionAndRotation;
            if (Hardness == 0)
            {
                CJ.projectionAngle = 10;
                CJ.projectionDistance = 5;
            }
            if (Hardness == 1)
            {
                CJ.projectionAngle = 5;
                CJ.projectionDistance = 2;
            }
            if (Hardness == 2)
            {
                CJ.projectionAngle = 2;
                CJ.projectionDistance = 0.5f;
            }
            if (Hardness == 3)
            {
                CJ.projectionAngle = CJ.projectionDistance = 0;
            }
        }

        //改变着色器
        void ChangedShader()
        {
            switch (ShaderMenu.Value)
            {
                case 1: MR.material.shader = Shader.Find("Transparent/Diffuse"); break;
                case 2: MR.material.shader = Shader.Find("Self-Illuminated/Diffuse"); break;
                case 3: MR.material.shader = Shader.Find("Reflective/Diffuse"); break;
                default: MR.material.shader = Shader.Find("Diffuse"); break;

            }
        }

        //改变颜色事件
        void ChangedColor()
        {
            MR.material.color = new Color(RedSlider.Value, GreenSlider.Value, BlueSlider.Value, AlphaSlider.Value);
        }

        //改变质量事件
        void ChangedMass()
        {
            RB.mass = Mass * (MassFormSize ? transform.localScale.x * transform.localScale.y * transform.localScale.z : 1f);
        }



        //刷新可视组件
        void RefreshVisual()
        {
            //时刻更新网格和贴图
            MF.mesh = resources[MeshBlockMod.NRF.MeshFullNames[MeshMenu.Value]].mesh;
            MR.material.mainTexture = resources[MeshBlockMod.NRF.TextureFullNames[TextureMenu.Value]].texture;
            ChangedShader();ChangedColor();

            //添加碰撞箱可视组件
            if (MC.GetComponent<MeshFilter>() == null)
            {
                MC.gameObject.AddComponent<MeshFilter>().mesh = resources[MeshBlockMod.NRF.MeshFullNames[MeshMenu.Value]].mesh;
                MC.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                MeshRenderer mr = MC.gameObject.AddComponent<MeshRenderer>();              
                mr.material.shader = Shader.Find("Transparent/Diffuse");
                mr.material.color = new Color(1, 1, 1, 0.25f);
                mr.enabled = DisplayColliderToggle.IsActive;
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
            ChangedMass();
            RefreshVisual();
        }

    }

}
