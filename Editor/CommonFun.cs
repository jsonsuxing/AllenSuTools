// ========================================================
// 描述：公共函数管理类
// 作者：苏醒
// 创建时间：2019-07-18 16:42:40
// 版 本：1.0
// ========================================================

using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 公共函数管理类
/// </summary>
public class CommonFun : MonoBehaviour
{
    #region 通用字段声明

    public static int    currentNum      = 1;            // 当前序号
    public static string ShowGranuleType = string.Empty; // 颗粒所属类别
    public const  int    INDEXNUM        = -100;         // 暂时定右击鼠标面板的下标是100

    public string TxtDirPath = "D:/Users/suxing/Desktop/编辑器生成的txt文件汇总/";

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
        if (Selection.activeGameObject == null)
        {
            WindowTips(tips);
            return;
        }
    }


    /// <summary>
    /// 判断是否输入 ShowGranuleType ，以及输入的 ShowGranuleType 的存在性
    /// </summary>
    public void JudgeGranuleType()
    {
        //颗粒名字，种类为空
        if (string.Equals(ShowGranuleType, string.Empty))
        {
            WindowTips("颗粒类别不能为空");
            return;
        }

        //输入的颗粒类别是否存在
        if (!AllGranuleTypeGroup.Contains(ShowGranuleType))
        {
            WindowTips("不存在的颗粒类别:" + "《" + ShowGranuleType + "》" + ",请重新输入");
            ShowGranuleType = string.Empty;
            return;
        }
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
        using (FileStream fs = new FileStream(dirPath + txtFileName + ".txt", FileMode.Append))
        {
            //将字符串转换为字节数组
            byte[] arr = Encoding.UTF8.GetBytes(writeContent + "\r\n");
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
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }


    /// <summary>
    /// 新建文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public void CreateNewFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Dispose();
        }
    }


    /// <summary>
    /// 设置GUIStyle
    /// </summary>
    /// <param name="color">字体颜色</param>
    /// <param name="fontSize">字体大小</param>
    /// <returns></returns>
    public GUIStyle SetGUIStyle(Color color, int fontSize)
    {
        GUIStyle style = new GUIStyle
        {
            normal =
            {
                textColor = color
            },
            fontSize = fontSize
        };
        return style;
    }
}