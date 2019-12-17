// ========================================================
// 描述：公共函数管理类
// 作者：苏醒
// 创建时间：2019-07-18 16:42:40
// 版 本：1.0
// ========================================================

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 公共函数管理类
/// </summary>
public class CommonFun
{
    #region 通用字段声明

    public static int    CurrentIndexNum = 1;            // 当前序号
    public static string ShowGranuleType = string.Empty; // 颗粒所属类别
    public const  int    INDEXNUM        = -100;         // 暂时定右击鼠标面板的下标是100

    public string TxtDirPath = "D:/编辑器生成的txt文件汇总/"; // 编辑器生成的 txt 路径
    public string jsonPath = Application.dataPath + "/AllenSuTools/Data/初级颗粒数据.txt"; // 自己的工程json路径
    public string PrefabPath = Application.dataPath + "/Resources/Prefab/ModelPrefabs"; // 预设物路径
    public string FbxPath = Application.dataPath + "/Other/InitialModels";  // 所有的fbx路径
    public string EdgePath= Application.dataPath + "/StreamingAssets/ModelEdgeDate"; // 边框路径
    public string PrimaryJsonPath = "G:/MyGitHub/MyUsedFile/txt 文件/工作/初级颗粒数据.txt"; // 初级颗粒数据路径

    public static GameObject Content = GameObject.Find("View/Canvas Assembling/Left Tool Panel/Granule Library/Viewport/Content");

    // 小颗粒类型汇总
    public string[] AllGranuleTypeGroup =
    {
        "泊类", "窗类", "齿轮类", "船类", "车轮类", "侧斜类", "动物类", "方高类", "发型胡须类", "方高连接类",
        "功能方高类", "拱形类", "交通工具类", "结构连接类", "轮架类", "门类", "明罩类", "平板类", "墙面类",
        "人仔身体类", "人仔头部类", "手脚配件类", "食物类", "套脖类", "头饰类", "围栏楼梯类", "武器类", "斜面类",
        "圆洞泊类", "圆洞类", "异形高类", "异形泊类", "装饰类", "植物类", "轴柱链类",
    };

    // 颗粒大类首字母缩写
    public string[] FirstPinYin = {"B", "C", "D", "F", "G", "H", "J", "L", "M", "N", "P", "Q", "R", "S", "T", "W", "X", "Y", "Z",};

    // 注意：需先点击《一般工具》编辑器里的"零件库相关数据显示区"按钮为列表赋值
    // 存储 Content 所有子物体的列表
    public static List<GameObject> AllChildObj = new List<GameObject>();
    // 所有初级零件库大类（如：方高类）
    public static List<GameObject> PrimaryTypeList = new List<GameObject>();
    // 所有中级零件库大类（如：中颗粒泊类）
    public static List<GameObject> IntermediateTypeList = new List<GameObject>();
    // 所有初级零件库颗粒（如：02.01.01.01.01.12&高一粒）
    public static List<GameObject> PrimaryGranuleList = new List<GameObject>();
    // 所有中级零件库颗粒（如：02.99.04.02.02.03&中颗粒泊2*4）
    public static List<GameObject> IntermediateGranuleList = new List<GameObject>();

    #endregion


    /// <summary>
    /// 弹出提示窗口
    /// </summary>
    /// <param name="message">提示内容</param>
    public static void WindowTips(string message)
    {
        EditorUtility.DisplayDialog("提示窗口", message, "确定");
    }


    /// <summary>
    /// 如果选中的物体为空
    /// </summary>
    /// <param name="tips">提示信息</param>
    public static void IfSelectionIsNull(string tips)
    {
        if (Selection.activeGameObject != null) return;
        WindowTips(tips);
        return;
    }


    /// <summary>
    /// 判断是否输入 ShowGranuleType ，以及输入的 ShowGranuleType 的存在性
    /// </summary>
    public void JudgeGranuleType()
    {
        //颗粒名字，种类为空
        if (Equals(ShowGranuleType, string.Empty))
        {
            WindowTips("颗粒类别不能为空");
            return;
        }

        //输入的颗粒类别是否存在
        if (AllGranuleTypeGroup.Contains(ShowGranuleType)) return;
        WindowTips("不存在的颗粒类别:" + "《" + ShowGranuleType + "》" + ",请重新输入");
        ShowGranuleType = string.Empty;
        return;
    }


    /// <summary>
    /// 写入文件到本地
    /// </summary>
    /// <param name="dirPath">文件夹路径</param>
    /// <param name="txtFileName">txt文件名</param>
    /// <param name="writeContent">写入的内容</param>
    public void WriteToTxt(string dirPath, string txtFileName, string writeContent)
    {
        CreateNewDirectory(dirPath);
        CreateNewFile(dirPath + txtFileName + ".txt");

        //初始化文件流
        using (var fs = new FileStream(dirPath + txtFileName + ".txt", FileMode.Append))
        {
            //将字符串转换为字节数组
            var arr = Encoding.UTF8.GetBytes(writeContent + "\r\n");
            //将字节数组写入文件流
            fs.Write(arr, 0, arr.Length);
            fs.Close();
        }
    }


    /// <summary>
    /// 新建文件夹
    /// </summary>
    /// <param name="dirPath">文件夹路径</param>
    public void CreateNewDirectory(string dirPath)
    {
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
    }


    /// <summary>
    /// 新建文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public void CreateNewFile(string filePath)
    {
        if (!File.Exists(filePath)) File.Create(filePath).Dispose();
    }


    /// <summary>
    /// 设置GUIStyle
    /// </summary>
    /// <param name="color">字体颜色</param>
    /// <param name="fontSize">字体大小</param>
    /// <returns></returns>
    public GUIStyle SetGuiStyle(Color color, int fontSize)
    {
        var style = new GUIStyle
        {
            normal =
            {
                textColor = color
            },
            fontSize = fontSize
        };
        return style;
    }

    /// <summary>
    /// 返回关键部位的纯汉字名称
    /// </summary>
    /// <param name="selectObj"></param>
    /// <returns></returns>
    public string GetBuWeiChineseName(GameObject selectObj)
    {
        // 括号里的数字分为 A:1~9，B:10 以上，位数不一样，获取到的关键部位名称会不同
        var buWeiName = Equals(selectObj.name.Substring(selectObj.name.Length - 3)[0].ToString(), "(")
            ? selectObj.name.Substring(0, selectObj.name.Length - 4)
            : selectObj.name.Substring(0, selectObj.name.Length - 5);
        return buWeiName;
    }

    /// <summary>
    /// 返回关键部位括号内的数字
    /// </summary>
    /// <param name="selectObjects"></param>
    /// <returns></returns>
    public char GetBuWeiMaxNameIndex(GameObject[] selectObjects)
    {
        var nameIndexChar=new char[selectObjects.Length];
        // 返回所选数组对象中最大的下标
        for (var i = 0; i < selectObjects.Length; i++)
        {
            var nameIndex = selectObjects[i].name.Substring(selectObjects[i].name.Length - 2)[0];
            nameIndexChar[i] = nameIndex;
        }
        return nameIndexChar.Max();
    }

    /// <summary>
    /// 解决类似 90.00001 转 int 值的问题
    /// </summary>
    /// <returns></returns>
    public int SpecialFloatToInt(float value)
    {
        var s = int.Parse(value.ToString(CultureInfo.InvariantCulture).Split('.')[0]);
        // var returnValue = int.Parse(str.Substring(str.IndexOf('.')+1));
        return s;
    }
}