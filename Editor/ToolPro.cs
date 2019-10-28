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

        // 写入到本地文件
        WriteToTxt("D:/","所选颗粒名称汇总",selectObj.name);
        WindowTips("添加成功");
    }

    #endregion

    public static ToolPro Instance()
    {
        return instance ?? (instance = new ToolPro());
    }
}
