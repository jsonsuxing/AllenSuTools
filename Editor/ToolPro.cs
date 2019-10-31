// ========================================================
// 描述：工具加强版
// 作者：苏醒 
// 创建时间：2019-10-25 17:06:08
// 版 本：1.0
// ========================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UI.ThreeDimensional;
using UnityEditor;
using UnityEngine;
using Directory = System.IO.Directory;


public class ToolPro : CommonFun
{
    private static ToolPro instance;

    #region 字段声明

    // 一：修改完旧模型的后续操作
    public GameObject selectObj  = null;
    public string     TxtPath    = "D:/所选颗粒名称汇总.txt";                      // 存放记录做的哪些颗粒名称的txt文件路径
    public string     OutFbxPath = string.Empty;                                   // 复制fbx到哪个文件夹
    public string     ModelPath  = Application.dataPath + "/Other/InitialModels/"; // 工程fbx模型路径
    public string[]   StrContent = null;                                           // txt内容

    #endregion

    #region 一：修改完旧模型的后续操作

    #region 一：复制名称到指定 txt 文件

    public void CopyNameToTxt()
    {
        selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("所选物体不能为空");
            return;
        }
        // 文件已存在时判断加入颗粒名称的重复性
        if (File.Exists(TxtPath))
        {
            StrContent = File.ReadAllLines(TxtPath);
            if (StrContent.Contains(selectObj.name))
            {
                WindowTips("不能重复添加");
                return;
            }
        }

        // 写入到本地文件
        WriteToTxt("D:/", "所选颗粒名称汇总", selectObj.name);
        WindowTips(message: "添加成功,当前已加入 " + (StrContent?.Length + 1 ?? 1) + " 个颗粒");
    }

    #endregion

    #region 二：复制 fbx 到指定文件夹

    /// <summary>
    /// 复制 fbx 到指定文件夹
    /// </summary>
    public void CopyFbxToDirectory()
    {
        if (Equals(OutFbxPath, string.Empty))
        {
            WindowTips("外部文件夹路径不能为空");
            return;
        }

        if (!File.Exists(TxtPath))
        {
            WindowTips("不存在 D:/所选颗粒名称汇总.txt 这个文件");
            return;
        }
        else
        {
            // 文件存在时，读取 StrContent
            StrContent = File.ReadAllLines(TxtPath);
        }

        var isSameTxt = false; // 是否有相同颗粒
        var sameNumber = 0;     // 相同颗粒个数
        var successNumber = 0;     // 复制了多少个文件
        var files = Directory.GetFiles(ModelPath, "*.fbx", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            foreach (var strTxt in StrContent)
            {
                // 文件名相同
                if (Equals(Path.GetFileNameWithoutExtension(file), strTxt))
                {
                    // 如果有同名 fbx 文件
                    if (File.Exists(OutFbxPath + "\\" + strTxt + ".fbx"))
                    {
                        WriteToTxt("D:/", "出现了相同颗粒名称", strTxt);
                        isSameTxt = true;
                        sameNumber++;
                        continue;
                    }
                    // 不相同时则直接复制文件
                    else
                    {
                        successNumber++;
                        File.Copy(file, OutFbxPath + "\\" + strTxt + ".fbx");
                    }
                }
            }
        }

        if (successNumber != 0) WindowTips("成功复制 " + successNumber + "个文件");
        if (isSameTxt)
        {
            WindowTips("出现了 " + sameNumber + " 个相同颗粒名称,详见 D:/出现了相同颗粒名称.txt");
            isSameTxt = false;
        }

        // 直接 取消"待定颗粒"标识
        var content = GameObject.Find("View/Canvas Assembling/Left Tool Panel/Granule Library/Viewport/Content/");
        for (var i = 0; i < content.transform.childCount; i++)
        {
            foreach (var strTxt in StrContent)
            {
                if (Equals(content.transform.GetChild(i).name, strTxt))
                {
                    content.transform.GetChild(i).GetComponent<UIObject3D>().待定颗粒 = false;
                }
            }
        }
    }

    #endregion

    #endregion




    public static ToolPro Instance()
    {
        return instance ?? (instance = new ToolPro());
    }
}