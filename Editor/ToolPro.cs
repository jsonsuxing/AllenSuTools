// ========================================================
// 描述：工具加强版
// 作者：苏醒 
// 创建时间：2019-10-25 17:06:08
// 版 本：1.0
// ========================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ToolPro : CommonFun
{
    private static ToolPro instance;

    #region 字段声明

    // 一：复制名称到指定 txt 文件
    public static int LineNumber = 0;  // 有多少行数据(即已经有多少颗粒)
    public Dictionary<int, string> GranuleDic = new Dictionary<int, string>();

    #endregion

    #region 一：复制名称到指定 txt 文件

    public void CopyNameToTxt()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("所选物体不能为空");
            return;
        }

        var filePath = "D://所选颗粒名称汇总.txt";
        //如果文件存在先判断重复性
        if (File.Exists(filePath))
        {
            // 是否重复添加
            if (GranuleDic.ContainsValue(selectObj.name)) WindowTips("重复添加");
            return;
        }

        // 写入到本地文件
        WriteToTxt("D:/","所选颗粒名称汇总",selectObj.name);
        GranuleDic.Add(LineNumber, selectObj.name);
        LineNumber++;
        WindowTips("复制成功，当前已有 " + GranuleDic.Count + " 个颗粒");
    }

    #endregion

    public static ToolPro Instance()
    {
        return instance ?? (instance = new ToolPro());
    }
}
