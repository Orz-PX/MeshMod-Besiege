using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Modding;


//    partial class MeshBlockMod
//    {

//        public Block MeshBlock = new Block()
//        #region 网格模块 基本属性  

//           //模块 ID
//           .ID(655)

//           //模块 名称
//           .BlockName("Mesh Block")

//           //模块 模型信息
//           .Obj(new System.Collections.Generic.List<Obj> { new Obj("/MeshBlockMod/Cube.obj", "/MeshBlockMod/Cube.png", new VisualOffset(Vector3.one * 0.3f, new Vector3(0, 0, 0.5f), Vector3.zero)) })

//           //模块 图标
//           .IconOffset(new Icon(0.5f, Vector3.zero, new Vector3(45, 0, 45)))

//           //模块 组件
//           .Components(new Type[] { typeof(MeshBlockScript) })

//           //模块 设置模块属性
//           .Properties(new BlockProperties().SearchKeywords(new string[] { "Mesh", "网格" }))

//           //模块 质量
//           .Mass(0.5f)

//            //模块 是否显示碰撞器
//#if DEBUG
//            .ShowCollider(true)
//#endif
//           //模块 碰撞器
//           .CompoundCollider(new System.Collections.Generic.List<ColliderComposite>
//                                {
//                                    //方块碰撞器
//                                    ColliderComposite.Mesh("/MeshBlockMod/Cube.obj",Vector3.one*0.3f, new Vector3(0,0,0.5f), Vector3.zero)

//                                    //球形碰撞器
//                                    ,ColliderComposite.Sphere(0.5f,Vector3.forward*0.5f,Vector3.zero)
//                                }
//                             )

//           //模块 载入资源
//           .NeededResources(NRF.NeedResources)

//           //模块 连接点
//           .AddingPoints(new List<AddingPoint>
//                            {

//                                new BasePoint(true,true) //底部连接点。第一个是指你能不能将其他模块安在该模块底部。第二个是指这个点是否是在开局时粘连其他链接点
//                                                .Motionable(false,false,false) //底点在X，Y，Z轴上是否是能够活动的。
//                                                .SetStickyRadius(0.25f) //粘连距离
//                                ,new AddingPoint(new Vector3(0,0,0.5f),new Vector3(0,0,90),true)

//                            });

//        #endregion;

//    }

    //网格模块脚本
    public class MeshBlockScript : BlockScript
    {
        //声明功能页菜单
        MMenu PageMenu;
        enum PageMenuList
        {
            基础设置 = 0,
            碰撞设置 = 1,
            模型设置 = 2,
            渲染设置 = 3
        };
              

        //声明基础功能组件
        //声明质量来自尺寸 质量 硬度组件和相关变量
        MToggle MassFormSizeToggle;   
        MSlider MassSlider;
        MSlider DragSlider;
        MMenu HardnessMenu;
        public bool MassFormSize = false;
        public float Mass = 0.5f;
        public float Drag = 0.2f;
        public int Hardness = 1;

        //声明碰撞设置功能组件
        //声明碰撞 碰撞 动静摩擦 弹力显示相关组件和相关变量
        //MMenu ColliderMenu;
        MToggle DisplayColliderToggle;
        MSlider DynamicFrictionSlider;
        MSlider StaticFrictionSlider;
        MSlider BouncynessSlider;

        //声明模型设置功能组件
        //声明网格 贴图 碰撞 碰撞显示 旋转相关组件和相关变量
        //MMenu MeshMenu;
        //MMenu TextureMenu;               
        MSlider RotationXSlider;
        MSlider RotationYSlider;
        MSlider RotationZSlider;
        public float RotationX = 0;
        public float RotationY = 0;
        public float RotationZ = 0;

        MSlider PositionXSlider;
        MSlider PositionYSlider;
        MSlider PositionZSlider;
        public float PositionX = 1;
        public float PositionY = 0;
        public float PositionZ = 0;

        //声明渲染设置功能组件
        //声明 着色器 RGBA 组件
        MMenu ShaderMenu;
        MSlider RedSlider;
        MSlider GreenSlider;
        MSlider BlueSlider;
        MSlider AlphaSlider;

        //声明需要使用的已存在组件
        MeshFilter MF;
        MeshRenderer MR,MC_MR;
        MeshCollider MC;
        SphereCollider SC;
        ConfigurableJoint CJ;
        Rigidbody RB;
        //NeedResourceFormat NRF = MeshBlockMod.NRF;
        

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
            SC = GetComponentsInChildren<SphereCollider>().ToList().Find(match => match.name == "SphereCollider");
            MR.material = new Material(Shader.Find("Diffuse"));
            MC_MR = MC.GetComponent<MeshRenderer>();
            CJ = GetComponent<ConfigurableJoint>();
            RB = GetComponent<Rigidbody>();


            #endregion

            #region 初始化组件

            //功能页组件
            PageMenu = AddMenu("Page", 0, EnumToStringList.Convert<PageMenuList>());
            PageMenu.ValueChanged += (int value) => { DisplayInMapper(); };

            //基础功能组件
            //硬度 尺寸决定质量 质量组件
            HardnessMenu = AddMenu("Hardness", Hardness, new List<string>() { "无碳钢", "低碳钢", "中碳钢", "高碳钢" });
            HardnessMenu.ValueChanged += (int value) => { Hardness = value; ChangedHardness(); };           
            MassFormSizeToggle = AddToggle("尺寸决定质量", "MassFormSize", MassFormSize);
            MassFormSizeToggle.Toggled += (bool value) => { MassFormSize = value; };
            MassSlider = AddSlider("质量", "Mass", Mass, 0.2f, 2f);
            MassSlider.ValueChanged += (float value) => { Mass = value; };
            DragSlider = AddSlider("阻力", "Drag", Drag, 0f, 2f);
            DragSlider.ValueChanged += (float value) => { Drag = value; };

            //碰撞设置
            //ColliderMenu = AddMenu("Collider", 0, MeshBlockMod.NRF.MeshNames);
            //ColliderMenu.ValueChanged += ChangedCollider;
            DisplayColliderToggle = AddToggle("碰撞可视", "DisplayCollider", false);
            DynamicFrictionSlider = AddSlider("滑动摩擦", "DynamicFriction", 0.5f, 0f, 1f);
            StaticFrictionSlider = AddSlider("静态摩擦", "StaticFriction", 0.5f, 0f, 1f);
            BouncynessSlider = AddSlider("表面弹性", "Bouncyness", 0f, 0f, 1f);
            DynamicFrictionSlider.ValueChanged += (float value) => { SC.material.dynamicFriction = MC.material.dynamicFriction = value; };
            StaticFrictionSlider.ValueChanged += (float value) => { SC.material.staticFriction = MC.material.staticFriction = value; };
            BouncynessSlider.ValueChanged += (float value) => { SC.material.bounciness = MC.material.bounciness = value; };

            //自定模型组件
            //旋转、位置滑条；网格、贴图、碰撞菜单；碰撞可视相关组件；
            RotationXSlider = AddSlider("旋转X轴", "RotationX", RotationX, 0f, 360f);
            RotationYSlider = AddSlider("旋转Y轴", "RotationY", RotationY, 0f, 360f);
            RotationZSlider = AddSlider("旋转Z轴", "RotationZ", RotationZ, 0f, 360f);           
            RotationXSlider.ValueChanged += (float value) => { RotationX = value; ChangedRotation(); };
            RotationYSlider.ValueChanged += (float value) => { RotationY = value; ChangedRotation(); };
            RotationZSlider.ValueChanged += (float value) => { RotationZ = value; ChangedRotation(); };

            PositionXSlider = AddSlider("移动X轴", "PositionX", PositionX = MR.transform.localPosition.x, -3f, 3f);
            PositionYSlider = AddSlider("移动Y轴", "PositionY", PositionY = MR.transform.localPosition.y, -3f, 3f);
            PositionZSlider = AddSlider("移动Z轴", "PositionZ", PositionZ = MR.transform.localPosition.z, -3f, 3f);
            PositionXSlider.ValueChanged += (float value) => { PositionX = value; ChangedPosition(); };
            PositionYSlider.ValueChanged += (float value) => { PositionY = value; ChangedPosition(); };
            PositionZSlider.ValueChanged += (float value) => { PositionZ = value; ChangedPosition(); };

            //MeshMenu = AddMenu("Mesh", 0, MeshBlockMod.NRF.MeshNames);
            //MeshMenu.ValueChanged += (int value) => { MF.mesh = resources[MeshBlockMod.NRF.MeshFullNames[value]].mesh; };
            //TextureMenu = AddMenu("Texture", 0, MeshBlockMod.NRF.TextureNames);
            //TextureMenu.ValueChanged += (int value) => { MR.material.mainTexture = resources[MeshBlockMod.NRF.TextureFullNames[value]].texture; };


            //渲染设置
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
            //DisplayInMapper();
            RefreshVisual();
            ChangedCollider();
            ChangedPoint();

            CJ.breakForce = CJ.breakTorque = 50000;

            #endregion

            if (GetComponent<DestroyJointIfNull>() == null) gameObject.AddComponent<DestroyJointIfNull>();


        }

        //组件显示事件
        void DisplayInMapper()
        {
            ////基础组件显示
            //HardnessMenu.DisplayInMapper = MassFormSizeToggle.DisplayInMapper = MassSlider.DisplayInMapper = DragSlider.DisplayInMapper = PageMenu.Value == Convert.ToInt32(PageMenuList.基础设置);

            ////碰撞组件显示
            //ColliderMenu.DisplayInMapper = DisplayColliderToggle.DisplayInMapper = PageMenu.Value == Convert.ToInt32(PageMenuList.碰撞设置);
            //DynamicFrictionSlider.DisplayInMapper = StaticFrictionSlider.DisplayInMapper = BouncynessSlider.DisplayInMapper = PageMenu.Value == Convert.ToInt32(PageMenuList.碰撞设置);

            ////模型组件显示
            //MeshMenu.DisplayInMapper = TextureMenu.DisplayInMapper = PageMenu.Value == Convert.ToInt32(PageMenuList.模型设置);
            //RotationXSlider.DisplayInMapper = RotationYSlider.DisplayInMapper = RotationZSlider.DisplayInMapper = PageMenu.Value == Convert.ToInt32(PageMenuList.模型设置);
            //PositionXSlider.DisplayInMapper = PositionYSlider.DisplayInMapper = PositionZSlider.DisplayInMapper = PageMenu.Value == Convert.ToInt32(PageMenuList.模型设置);

            ////渲染组件显示
            //ShaderMenu.DisplayInMapper = RedSlider.DisplayInMapper = GreenSlider.DisplayInMapper = BlueSlider.DisplayInMapper = AlphaSlider.DisplayInMapper = PageMenu.Value == Convert.ToInt32(PageMenuList.渲染设置);

        }

        //改变碰撞事件
        void ChangedCollider(int value = 0)
        {

            //if (MeshBlockMod.NRF.MeshNames[value] =="圆球体")
            //{
            //    MC.isTrigger = true;
            //    SC.isTrigger = false;
            //    MC.GetComponent<MeshFilter>().mesh = resources[MeshBlockMod.NRF.MeshFullNames[value]].mesh;

            //}
            //else
            //{
            //    MC.isTrigger = false;
            //    SC.isTrigger = true;
            //    MC.sharedMesh = MC.GetComponent<MeshFilter>().mesh = resources[MeshBlockMod.NRF.MeshFullNames[value]].mesh;
            //}
            
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

        //改变位置事件
        void ChangedPosition()
        {
            MR.transform.localPosition = new Vector3(PositionXSlider.Value, PositionYSlider.Value, PositionZSlider.Value);
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

        //改变刚体属性事件
        void ChangedRigibodyPropertise()
        {
            RB.mass = Mass * (MassFormSize ? transform.localScale.x * transform.localScale.y * transform.localScale.z : 1f);
            RB.drag = Drag;
        }

        //改变安装点事件
        //改变安装点大小  为正方体
        void ChangedPoint()
        {
            BoxCollider BC = GetComponentsInChildren<BoxCollider>().ToList().Find(match => match.name == "Adding Point");
            BC.center = Vector3.zero;
            BC.size = Vector3.one * 1.1f;          
        }

        //刷新可视组件
        void RefreshVisual()
        {
            //更新网格和贴图
            //MF.mesh = resources[MeshBlockMod.NRF.MeshFullNames[MeshMenu.Value]].mesh;
            //MR.material.mainTexture = resources[MeshBlockMod.NRF.TextureFullNames[TextureMenu.Value]].texture;
            ChangedShader();ChangedColor();

            //添加碰撞箱可视组件
            if (MC.GetComponent<MeshFilter>() == null)
            {
                //MC.gameObject.AddComponent<MeshFilter>().mesh = resources[MeshBlockMod.NRF.MeshFullNames[MeshMenu.Value]].mesh;
                MC.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                MeshRenderer mr = MC.gameObject.AddComponent<MeshRenderer>();
                mr.material.shader = Shader.Find("Transparent/Diffuse");
                mr.material.color = new Color(1, 1, 1, 0.25f);
                mr.enabled = DisplayColliderToggle.IsActive;
            }
            else
            {
                MC.GetComponent<MeshFilter>();
            }
            
        }

        public override void BuildingUpdate()
        { 
            //改变质量
            ChangedRigibodyPropertise();
            
            //没被选中时就刷新显示
            if (!(GetComponent<BlockVisualController>().Highlighted || GetComponent<BlockVisualController>().Selected ))
            {
                //RefreshVisual();
            }

            //碰撞箱在建造模式下总是显示
            if (MC_MR.enabled == false)
            {
                MC_MR.enabled = true;
            }

        }

        public override void OnSimulateStart()
        {

            //模拟模式下碰撞显示生效就显示碰撞箱 否则 自动隐藏
            MC_MR.enabled = DisplayColliderToggle.IsActive;
        }

    }

    //枚举转list
    public class EnumToStringList
    {

        /// <summary>  
        /// 枚举名称  
        /// </summary>  
        public static List<string> EnumName { set; get; } 

        public static List<string> Convert<T>()
        {
            EnumName = new List<string>();

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                EnumName.Add(e.ToString());

            }
            return EnumName;
        }

    }

   



