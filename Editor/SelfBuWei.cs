// ========================================================
// 描述：关键部位管理类
// 作者：苏醒 
// 创建时间：2019-07-18 20:13:54
// 版 本：1.0
// ========================================================

using System.Linq;
using System.Windows.Forms;
using ChinarX;
using ChinarX.ExtensionMethods;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 关键部位管理类
/// </summary>
public class SelfBuWei : CommonFun
{
    // 碰撞盒管理类
    private static SelfBuWei instance;

    #region 字段声明

    // 一：逐个克隆关键部位
    public int    CloneNumByAxisX   = 0;    //沿着 x 轴正方向生成多少个 默认不生成
    public int    CloneNumByAxisZ   = 0;    //沿着 z 轴正方向生成多少个 默认不生成
    public int    CloneNumByAxisXFu = 0;    //沿着 x 轴负方向生成多少个 默认不生成
    public int    CloneNumByAxisZFu = 0;    //沿着 z 轴负方向生成多少个 默认不生成
    public float  ModelSpace        = 0;    //间隔的值
    public float  GeneralSpace      = 0.8f; //间隔0.8(普通颗粒)
    public float  MediumSpace       = 1.6f; //间隔1.6(中颗粒)
    public bool   IsClickChangeSpace;       //是否点了改变间隔的值的按钮
    public bool   IsMoreLine   = true;      //逐行克隆还是多行克隆，默认是多行克隆
    public string ShowCloneWay = "多行克隆";    //显示哪种克隆方式

    // 二：不规则部位坐标的计算
    public bool   IsClickX;                     //是否点了x轴
    public bool   IsClickZ;                     //是否点了z轴
    public bool   AddValueNum;                  //加0.4f
    public bool   ReduceValueNum;               //减去0.4f
    public string ShowClickAxis = string.Empty; //提示哪个坐标轴
    public string ShowValueNum  = string.Empty; //提示+ - 0.4

    public static bool ISBevelBox = false;             // 是否是倾斜盒子，默认不是

    #endregion

    #region 关键部位字段声明

    //Grid 栅格类型
    public static string AoCao    = "AoCao";
    public static string TuQi     = "TuQi";
    public static string SonAoCao = "SonAoCao";

    //Axle 轴洞类型
    public static string BuChuanTouCao = "BuChuanTouCao";
    public static string BuChuanTouCha = "BuChuanTouCha";
    public static string XiaoChaCao    = "XiaoChaCao";
    public static string XiaoChaXiao   = "XiaoChaXiao";
    public static string ChaCao        = "ChaCao";
    public static string ChaXiao       = "ChaXiao";
    public static string ChaCaoShiZi   = "ChaCaoShiZi";
    public static string ChaXiaoShiZi  = "ChaXiaoShiZi";
    public static string PiLunWai      = "PiLunWai";
    public static string PiLunNei      = "PiLunNei";

    //Hinge 合页类型
    public static string ChaCaoMenChuang  = "ChaCaoMenChuang";
    public static string ChaXiaoMenChuang = "ChaXiaoMenChuang";
    public static string ZhuanYaChaCao    = "ZhuanYaChaCao";
    public static string ZhuanYaChaxiao   = "ZhuanYaChaxiao";

    //Sphere 球类型
    public static string YuanQiuDong = "YuanQiuDong";
    public static string YuanQiuZhu  = "YuanQiuZhu";

    //Fixure 固定类型
    public static string GongJuxiangDi  = "GongJuxiangDi";
    public static string GongJuxiangGai = "GongJuxiangGai";

    //Slide 滑动类型
    public static string HuaDongCao  = "HuaDongCao";
    public static string HuaDongXiao = "HuaDongXiao";

    //管道类型
    public static string GuanDaoCao  = "GuanDaoCao";
    public static string GuanDaoXiao = "GuanDaoXiao";

    #endregion

    #region 批量更改关键部位名字

    #region Grid 栅格类型

    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/----------栅格类型----------", false, INDEXNUM)]
    public static void FlagTypeGrid() { }

    //0--凹槽
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/AoCao", false, INDEXNUM + 1)]
    public static void Fun_AoCao() { ChangeKeyName(AoCao, GuanJianType.Grid, 0, 0); }

    //1--凸起
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/TuQi", false, INDEXNUM + 2)]
    public static void Fun_TuQi() { ChangeKeyName(TuQi, GuanJianType.Grid, 1, 0); }

    //2--不能和 AoCao 重名
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/SonAoCao", false, INDEXNUM + 3)]
    public static void Fun_SonTuQi() { ChangeKeyName(SonAoCao, GuanJianType.Grid, 0, 0); }

    #endregion

    #region Axle 轴洞类型

    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/----------轴洞类型----------", false, INDEXNUM + 4)]
    public static void FlagTypeAxle() { }

    //0--不穿透槽
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/BuChuanTouCao", false, INDEXNUM + 5)]
    public static void Fun_BuChuanTouCao() { ChangeKeyName(BuChuanTouCao, GuanJianType.Axle, 0, 0.17f); }

    //1--不穿透插
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/BuChuanTouCha", false, INDEXNUM + 6)]
    public static void Fun_BuChuanTouCha() { ChangeKeyName(BuChuanTouCha, GuanJianType.Axle, 1, 0.17f); }
  
    //2--小插槽
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/XiaoChaCao", false, INDEXNUM + 7)]
    public static void Fun_XiaoChaCao() { ChangeKeyName(XiaoChaCao, GuanJianType.Axle, 2, 0.17f); }

    //3--小插销
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/XiaoChaXiao", false, INDEXNUM + 8)]
    public static void Fun_XiaoChaXiao() { ChangeKeyName(XiaoChaXiao, GuanJianType.Axle, 3, 0.8f); }

    //4--插槽
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/ChaCao", false, INDEXNUM + 9)]
    public static void Fun_ChaCao() { ChangeKeyName(ChaCao, GuanJianType.Axle, 4, 0.8f); }

    //5--插销
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/ChaXiao", false, INDEXNUM + 10)]
    public static void Fun_ChaXiao() { ChangeKeyName(ChaXiao, GuanJianType.Axle, 5, 0.8f); }

    //6--插槽十字
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/ChaCaoShiZi", false, INDEXNUM + 11)]
    public static void Fun_ChaCaoShiZi() { ChangeKeyName(ChaCaoShiZi, GuanJianType.Axle, 6, 0.8f); }

    //7-插销十字
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/ChaXiaoShiZi", false, INDEXNUM + 12)]
    public static void Fun_ChaXiaoShiZi() { ChangeKeyName(ChaXiaoShiZi, GuanJianType.Axle, 7, 0.8f); }
  
    //特殊-皮轮外
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/PiLunWai", false, INDEXNUM + 15)]
    public static void Fun_PiLunWai() { ChangeKeyName(PiLunWai, GuanJianType.Axle, -2, 0); }

    //特殊-皮轮内
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/PiLunNei", false, INDEXNUM + 16)]
    public static void Fun_PiLunNei() { ChangeKeyName(PiLunNei, GuanJianType.Axle, -3, 0); }

    #endregion

    #region Hinge 合页类型

    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/----------合页类型----------", false, INDEXNUM + 17)]
    public static void FlagTypeHinge() { }

    //0--插槽门窗
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/ChaCaoMenChuang", false, INDEXNUM + 18)]
    public static void Fun_ChaCaoMenChuang() { ChangeKeyName(ChaCaoMenChuang, GuanJianType.Hinge, 0, 0); }
  
    //1--插销门窗
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/ChaXiaoMenChuang", false, INDEXNUM + 19)]
    public static void Fun_ChaXiaoMenChuang() { ChangeKeyName(ChaXiaoMenChuang, GuanJianType.Hinge, 1, 0); }
  
    //2--转牙插槽
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/ZhuanYaChaCao", false, INDEXNUM + 20)]
    public static void Fun_ZhuanYaChaCao() { ChangeKeyName(ZhuanYaChaCao, GuanJianType.Hinge, 2, 0); }

    //3--转牙插销
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/ZhuanYaChaxiao", false, INDEXNUM + 21)]
    public static void Fun_ZhuanYaChaxiao() { ChangeKeyName(ZhuanYaChaxiao, GuanJianType.Hinge, 3, 0); }

    #endregion

    #region Sphere 球类型

    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/----------球类型----------", false, INDEXNUM + 22)]
    public static void FlagTypeSphere() { }

    //0--圆球洞
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/YuanQiuDong", false, INDEXNUM + 23)]
    public static void Fun_YuanQiuDong() { ChangeKeyName(YuanQiuDong, GuanJianType.Sphere, 0, 0); }

    //1--圆球珠
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/YuanQiuZhu", false, INDEXNUM + 24)]
    public static void Fun_YuanQiuZhu() { ChangeKeyName(YuanQiuZhu, GuanJianType.Sphere, 1, 0); }

    #endregion

    #region Fixure 固定类型

    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/----------固定类型----------", false, INDEXNUM + 25)]
    public static void FlagTypeFixure() { }

    //0--工具箱盖
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/GongJuxiangGai", false, INDEXNUM + 26)]
    public static void Fun_GongJuxiangGai() { ChangeKeyName(GongJuxiangGai, GuanJianType.Fixure, 0, 0); }
    
    //1--工具箱底
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/GongJuxiangDi", false, INDEXNUM + 27)]
    public static void Fun_GongJuxiangDi() { ChangeKeyName(GongJuxiangDi, GuanJianType.Fixure, 1, 0); }

    #endregion

    #region Slide 滑动类型

    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/----------滑动类型----------", false, INDEXNUM + 28)]
    public static void FlagTypeSlide() { }

    //0--滑动槽
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/HuaDongCao", false, INDEXNUM + 29)]
    public static void Fun_HuaDongCao() { ChangeKeyName(HuaDongCao, GuanJianType.Slide, 0, 0); }
   
    //1--滑动销
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/HuaDongXiao", false, INDEXNUM + 30)]
    public static void Fun_HuaDongXiao() { ChangeKeyName(HuaDongXiao, GuanJianType.Slide, 1, 0); }

    #endregion

    #region 管道类型

    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/----------管道类型----------", false, INDEXNUM + 31)]
    public static void FlagTypeGuanDao() { }

    //0--管道槽
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/GuanDaoCao", false, INDEXNUM + 32)]
    public static void Fun_GuanDaoCao() { ChangeKeyName(GuanDaoCao, GuanJianType.Grid, 0, 0); }

    //1--管道销
    [UnityEditor.MenuItem("AllenSu/0、更改关键部位名字/GuanDaoXiao", false, INDEXNUM + 33)]
    public static void Fun_GuanDaoXiao() { ChangeKeyName(GuanDaoXiao, GuanJianType.Grid, 1, 0); }

    #endregion

    #region 更改碰撞盒名称

    [UnityEditor.MenuItem("AllenSu/1、更改碰撞盒名称/Normal Box", false, INDEXNUM)]
    public static void NormalBox()
    {
        ISBevelBox = false;
        ChangeBoxName();
        // 不知为何，将下行代码放到 ChangeBoxName() 函数内部最后却无法执行，单独写出来却可以执行，不知道为啥
        ToolPro.Instance().CheckBoxCollider(Selection.gameObjects[0].transform.parent.gameObject);
    }

    [UnityEditor.MenuItem("AllenSu/1、更改碰撞盒名称/Bevel Box", false, INDEXNUM)]
    public static void BevelBox()
    {
        ISBevelBox = true;
        ChangeBoxName();
        ToolPro.Instance().CheckBoxCollider(Selection.gameObjects[0].transform.parent.gameObject);
    }

    #endregion

    /// <summary>
    /// 批量更改关键部位名字，并为 GuanJianBuWei 脚本赋值
    /// </summary>
    /// <param name="name">名字</param>
    /// <param name="type">类型</param>
    /// <param name="size">大小</param>
    /// <param name="length">长度</param>
    public static void ChangeKeyName(string name, GuanJianType type, int size, float length)
    {
        //调用该函数，说明在切换类型，所以先让 CurrentIndexNum 恢复默认值
        CurrentIndexNum = 1;

        var i = 1;
        var siblingObjects  = Selection.gameObjects.OrderBy(_ => _.transform.GetSiblingIndex());

        siblingObjects.ForEach(obj =>
        {
            Undo.RecordObject(obj, "siblingObjects");
            obj.name= name + " " + "(" + (i++) + ")"; //名字;
            var buWei = obj.GetComponent<GuanJianBuWei>();
            buWei.type = type; //类型
            buWei.size = size; //大小
            buWei.length = length;           //长度
        });
    }

    /// <summary>
    /// 更改碰撞盒名称
    /// </summary>
    public static void ChangeBoxName()
    {
        var i = 1;
        var siblingObjects = Selection.gameObjects.OrderBy(_ => _.transform.GetSiblingIndex());
        siblingObjects.ForEach(obj =>
        {
            Undo.RecordObject(obj, "siblingBoxCollObjects");
            obj.name= ISBevelBox ? "Bevel Box " + "(" + i + ")" : "Normal Box " + "(" + i + ")";
            i++;
        });
    }

    #endregion

    #region 一：逐个克隆关键部位

    /// <summary>
    /// 逐个克隆关键部位
    /// </summary>
    public void CloneBuWeiOneByOne()
    {
        // 获得所选关键部位
        var selectObj = Selection.activeGameObject;
        if (Selection.activeGameObject == null)
        {
            WindowTips("没有选中颗粒");
            return;
        }

        // 关键部位名称(不带数字和括号)
        var buWeiName = string.Empty;

        // 括号里的数字分为 A:1~9，B:10 以上，位数不一样，获取到的关键部位名称会不同
        buWeiName = Equals(selectObj.name.Substring(selectObj.name.Length - 3)[0].ToString(), "(")
            ? selectObj.name.Substring(0, selectObj.name.Length - 4)
            : selectObj.name.Substring(0, selectObj.name.Length - 5);

        var selectPosition = selectObj.transform.localPosition;

        // 沿着 x 轴正方向生成多少个关键部位
        if (CloneNumByAxisX != 0)
        {
            for (var i = 1; i < CloneNumByAxisX + 1; i++)
            {
                CloneMethod(selectObj, buWeiName, new Vector3(selectPosition.x + i * ModelSpace, selectPosition.y, selectPosition.z));
            }
        }

        // 沿着 z 轴正方向生成多少个关键部位
        if (CloneNumByAxisZ != 0)
        {
            for (var i = 1; i < CloneNumByAxisZ + 1; i++)
            {
                CloneMethod(selectObj, buWeiName, new Vector3(selectPosition.x, selectPosition.y, selectPosition.z + i * ModelSpace));

                // 是否多行克隆
                if (!IsMoreLine) continue;
                // 沿着 x 轴正方向生成多少个关键部位
                if (CloneNumByAxisX != 0)
                {
                    for (var j = 1; j < CloneNumByAxisX + 1; j++)
                    {
                        CloneMethod(selectObj, buWeiName, new Vector3(selectPosition.x + j * ModelSpace, selectPosition.y, selectPosition.z + i * ModelSpace));
                    }
                }
                // 沿着 x 轴负方向生成多少个关键部位
                else if (CloneNumByAxisXFu != 0)
                {
                    for (var j = 1; j < CloneNumByAxisXFu + 1; j++)
                    {
                        CloneMethod(selectObj, buWeiName, new Vector3(selectPosition.x - j * ModelSpace, selectPosition.y, selectPosition.z + i * ModelSpace));
                    }
                }
            }
        }

        // 沿着 x 轴负方向生成多少个关键部位
        if (CloneNumByAxisXFu != 0)
        {
            for (var i = 1; i < CloneNumByAxisXFu + 1; i++)
            {
                CloneMethod(selectObj, buWeiName, new Vector3(selectPosition.x - i * ModelSpace, selectPosition.y, selectPosition.z));
            }
        }

        // 沿着 z 轴负方向生成多少个关键部位
        if (CloneNumByAxisZFu != 0)
        {
            for (var i = 1; i < CloneNumByAxisZFu + 1; i++)
            {
                CloneMethod(selectObj, buWeiName, new Vector3(selectPosition.x, selectPosition.y, selectPosition.z - i * ModelSpace));

                // 是否多行克隆
                if (!IsMoreLine) continue;
                if (CloneNumByAxisX != 0)
                {
                    for (var j = 1; j < CloneNumByAxisX + 1; j++)
                    {
                        CloneMethod(selectObj, buWeiName, new Vector3(selectPosition.x + j * ModelSpace, selectPosition.y, selectPosition.z - i * ModelSpace));
                    }
                }
                else if (CloneNumByAxisXFu != 0)
                {
                    for (var j = 1; j < CloneNumByAxisXFu + 1; j++)
                    {
                        CloneMethod(selectObj, buWeiName, new Vector3(selectPosition.x - j * ModelSpace, selectPosition.y, selectPosition.z - i * ModelSpace));
                    }
                }
            }
        }

        // 克隆完之后恢复默认
        CloneNumByAxisX = CloneNumByAxisZ = CloneNumByAxisXFu = CloneNumByAxisZFu = 0;
    }


    /// <summary>
    /// 克隆关键部位
    /// </summary>
    /// <param name="go">所选物体</param>
    /// <param name="buWeiName">关键部位名称</param>
    /// <param name="v">关键部位新坐标</param>
    public void CloneMethod(GameObject go, string buWeiName, Vector3 v)
    {
        CurrentIndexNum++;
        var buWei = go.GetComponent<GuanJianBuWei>();

        //克隆关键部位,设置父物体，重新命名
        var obj = new GameObject();
        Undo.RegisterCreatedObjectUndo(obj,"CloneBuWei");
        obj.transform.parent = go.transform.parent.transform;
        obj.AddComponent<GuanJianBuWei>();
        var objBuWei = obj.GetComponent<GuanJianBuWei>();
        objBuWei.type            =  buWei.type;
        objBuWei.size            =  buWei.size;
        objBuWei.director        =  buWei.director;
        objBuWei.length          =  buWei.length;
        obj.name                    =  buWeiName + " (" + (CurrentIndexNum) + ")";
        obj.transform.localPosition += new Vector3(v.x, v.y, v.z);
    }

    #endregion

    #region 二：对称克隆关键部位

    /// <summary>
    /// 对称克隆关键部位
    /// </summary>
    public void SymmetryCloneBuWei()
    {
        // 获得所选关键部位
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中关键部位");
            return;
        }

        var selectBuWei = selectObj.GetComponent<GuanJianBuWei>();
        if (!selectBuWei) return;
        // 括号里的数字分为 A:1~9，B:10 以上，位数不一样，获取到的关键部位名称会不同
        var buWeiName = Equals(selectObj.name.Substring(selectObj.name.Length - 3)[0].ToString(), "(")
            ? selectObj.name.Substring(0, selectObj.name.Length - 4)
            : selectObj.name.Substring(0, selectObj.name.Length - 5);

        var selectPosition = selectObj.transform.localPosition;
        for (var i = 1; i < 4; i++)
        {
            // 克隆关键部位,设置父物体，重新命名
            var obj = new GameObject();
            Undo.RegisterCreatedObjectUndo(obj, "SymmetryCloneBuWei");
            obj.transform.parent = selectObj.transform.parent.transform;
            var buWei = obj.AddComponent<GuanJianBuWei>();
            buWei.type     = selectBuWei.type;
            buWei.size     = selectBuWei.size;
            buWei.director = selectBuWei.director;
            buWei.length   = selectBuWei.length;
            obj.name       = buWeiName + " (" + (i + 1) + ")";
            switch (i)
            {
                case 1: // x轴取反
                    obj.transform.localPosition = new Vector3(-selectPosition.x, selectPosition.y, selectPosition.z);
                    break;
                case 2: // z轴取反
                    obj.transform.localPosition = new Vector3(selectPosition.x, selectPosition.y, -selectPosition.z);
                    break;
                case 3: // x轴，z轴都取反
                    obj.transform.localPosition = new Vector3(-selectPosition.x, selectPosition.y, -selectPosition.z);
                    break;
                default:
                    WindowTips("对称克隆关键部位出错");
                    break;
            }
        }
    }

    #endregion

    #region 三：生成默认的 GameObject

    /// <summary>
    /// 生成默认的 GameObject
    /// </summary>
    public void CreateDefaultObj()
    {
        IfSelectionIsNull("没有选中颗粒作为父物体");
        var selectObj = new GameObject("默认 GameObject");
        Undo.RegisterCreatedObjectUndo(selectObj,"CreateDefaultGameObject");
        selectObj.transform.position = Vector3.zero;
        selectObj.transform.parent = Selection.activeGameObject.transform;
    }

    #endregion

    #region 四：生成空物体 NormalBox (1)

    public void CreateDefaultNormalBox()
    {
        IfSelectionIsNull("没有选中颗粒作为父物体");
        var obj = new GameObject("Normal Box (1)");
        Undo.RegisterCreatedObjectUndo(obj, "CreateDefaultNormalBox");
        obj.transform.parent = Selection.activeGameObject.transform;
        obj.AddComponent<BoxCollider>();
        var box = obj.GetComponent<BoxCollider>();
        box.size = new Vector3(0.5f, 0.5f, 0.5f);
    }

    #endregion

    #region 五：特殊部位坐标的计算(+-0.4类型)

    /// <summary>
    /// 复制凸起的坐标+ - 0.4后改成管道槽的坐标
    /// </summary>
    public void SpecialAxis()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中关键部位");
            return;
        }

        var selectV = selectObj.transform.localPosition;

        // x轴
        if (IsClickX)
        {
            // +0.4
            if (AddValueNum) selectV = new Vector3((selectV.x + 0.4f), selectV.y, selectV.z);
            // -0.4
            else if (ReduceValueNum) selectV = new Vector3((selectV.x - 0.4f), selectV.y, selectV.z);
        }

        //z轴
        if (IsClickZ)
        {
            // +0.4
            if (AddValueNum) selectV = new Vector3(selectV.x, selectV.y, (selectV.z + 0.4f));
            // -0.4
            else if (ReduceValueNum) selectV = new Vector3(selectV.x, selectV.y, (selectV.z - 0.4f));
        }

        //v3是值类型，这里只是修改了值，需要返回给对象
        selectObj.transform.localPosition = selectV;
        if (!IsClickX              && !IsClickZ) WindowTips("请至少选择一个轴向");
        if ((IsClickX || IsClickZ) && (!AddValueNum && !ReduceValueNum)) WindowTips("请至少选择一个长度");
    }


    /// <summary>
    /// x轴，z轴的坐标都减去0.4
    /// </summary>
    public void SubtractAllValue()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中关键部位");
            return;
        }

        Vector3 selectV = selectObj.transform.localPosition;
        selectV                           = new Vector3((selectV.x - 0.4f), selectV.y, (selectV.z - 0.4f));
        selectObj.transform.localPosition = selectV;
    }

    #endregion

    public static SelfBuWei Instance()
    {
        return instance ?? (instance = new SelfBuWei());
    }
}