// ========================================================
// 描述：综合小工具编辑器管理类
// 作者：苏醒 
// 创建时间：2019-07-18 21:22:38
// 版 本：1.0
// ========================================================

using System;
using System.IO;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 综合小工具编辑器管理类
/// </summary>
public class SelfTools : CommonFun
{
    private static SelfTools instance;

    #region 字段声明

    // 计算 Length 的值
    public int    ClickTime = 1;          // 第几次点击按钮
    public double FirstValue;             // 当前坐标(第一个坐标)
    public double SecondValue;            // 第二个坐标
    public double Length;                 // length的值(两坐标的差值)
    public double MidPoint;               // 同轴时，两个坐标x或y或z的中点值
    public string SetAxis = "y"; // 为轴向赋值，默认为y轴

    // 批量修改“-”为“&”
    public string ChangeFileNamePath = string.Empty; // 要修改的文件路径
    public int    ModifiedNum        = 0;            // 修改了多少个文件

    // 删除指定后缀名的文件
    public string DeleteFilePath  = string.Empty; // 要修改的指定后缀类型的文件路径
    public string SelectExtension = string.Empty; // 点选的文件后缀名

    // 移动模型文件(边框)
    public string OldModelPath = string.Empty;    // 原来的模型文件夹路径
    public string NewModelPath = string.Empty;    // 指定的模型文件夹路径
    public string NeedMoveFileName = string.Empty;// 待移动的文件名

    #endregion

    #region 所选物体坐标归0

    /// <summary>
    /// 所选物体坐标归0
    /// </summary>
    public void TransformToZero()
    {
        IfSelectionIsNull("没有选中关键部位");
        Selection.activeGameObject.transform.localPosition = Vector3.zero;
    }

    #endregion

    #region 计算 Length 的值

    /// <summary>
    /// 计算关键部位上 Length 的值
    /// </summary>
    public void GetLength()
    {
        IfSelectionIsNull("没有选中关键部位");
        GetLengthValue(Selection.activeGameObject, SetAxis);
    }


    /// <summary>
    /// 获取颗粒 Length 的值
    /// </summary>
    /// <param name="selectObj">所选物体</param>
    /// <param name="axle">轴向</param>
    public void GetLengthValue(GameObject selectObj, string axle)
    {
        var buWei = selectObj.GetComponent<GuanJianBuWei>();
        if (!buWei)
        {
            WindowTips("关键部位上没有 GuanJianBuWei 脚本");
            return;
        }

        var objPos = selectObj.transform.position;

        if (ClickTime == 1)
        {
            // 多次点击时清空之前获取到的长度以及中点值
            Length = MidPoint = 0;
            switch (axle)
            {
                case "x":
                    FirstValue = objPos.x;
                    break;
                case "y":
                    FirstValue = objPos.y;
                    break;
                case "z":
                    FirstValue = objPos.z;
                    break;
            }
        }
        else if (ClickTime == 2)
        {
            switch (axle)
            {
                case "x":
                    SecondValue = objPos.x;
                    selectObj.transform.position = new Vector3((float)(Math.Round((FirstValue + SecondValue) / 2, 6)), objPos.y,objPos.z );
                    break;
                case "y":
                    SecondValue = objPos.y;
                    selectObj.transform.position = new Vector3(objPos.x, (float)(Math.Round((FirstValue + SecondValue) / 2, 6)), objPos.z);
                    break;
                case "z":
                    SecondValue = objPos.z;
                    selectObj.transform.position = new Vector3(objPos.x,objPos.y, (float)(Math.Round((FirstValue + SecondValue) / 2, 6)));
                    break;
            }

            if (FirstValue >= 0 && SecondValue >= 0 || FirstValue <= 0 && SecondValue <= 0)
            {
                //保留两位小数，Math.Round是国际标注，四舍六入
                Length = Math.Round(Math.Abs(FirstValue - SecondValue), 2);
            }
            else if (FirstValue > 0 && SecondValue < 0 || FirstValue < 0 && SecondValue > 0)
            {
                Length = Math.Round(Math.Abs(FirstValue) + Math.Abs(SecondValue), 2);
            }
            // 关键部位的长度
            buWei.length = (float) Length;

            // 以下数据不管是否用到，都传值显示
            MidPoint = Math.Round((FirstValue + SecondValue) / 2, 6); // 两点的中点值
            SelfCollider.Instance().Height= (float)Length - 0.004f;   // 环形碰撞盒的高度要比实际值小
            SelfCollider.Instance().CloneSpace = (float)Length;       // 自定义平移碰撞盒的间隔
        }

        ClickTime++;
        if (ClickTime > 2) ClearValue();
    }


    //清空已有数据
    public void ClearValue()
    {
        FirstValue  = 0;
        SecondValue = 0;
        ClickTime   = 1;
    }

    #endregion

    #region 批量修改“-”为“&”

    /// <summary>
    /// 批量修改文件名中含有“-”的为 (shift+7对应的符号)
    /// </summary>
    public void ChangeFileName()
    {
        if (ChangeFileNamePath == string.Empty)
        {
            WindowTips("请输入要修改的文件路径");
            return;
        }

        var files = Directory.GetFiles(ChangeFileNamePath, "*.*", SearchOption.AllDirectories);
        foreach (string str in files)
        {
            var tempName = Path.GetFileNameWithoutExtension(str);
            if (tempName.Contains("-"))
            {
                //替换文件名
                var newName = tempName.Replace("-", "&");
                File.Move(str, Path.GetDirectoryName(str) + "\\" + newName + Path.GetExtension(str));
                ModifiedNum++;
            }
        }

        if (ModifiedNum != 0)
        {
            WindowTips("修改成功，本次共修改 " + ModifiedNum + " 个文件");
            //清空路径
            ChangeFileNamePath = string.Empty;
            ModifiedNum        = 0;
        }
    }

    #endregion

    #region 添加，删除 MeshRenderer,MeshFilter

    /// <summary>
    /// 添加MeshRenderer,MeshFilter
    /// </summary>
    public void AddMesh()
    {
        // 所选物体
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中关键部位");
            return;
        }

        selectObj.AddComponent<MeshRenderer>();
        selectObj.AddComponent<MeshFilter>();
    }


    /// <summary>
    /// 删除MeshRenderer,MeshFilter
    /// </summary>
    public void RemoveMesh()
    {
        // 所选物体
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中关键部位");
            return;
        }

        // 没有子物体，证明在对单个关键部位进行操作
        if (selectObj.transform.childCount == 0)
        {
            if (selectObj.transform.GetComponent<MeshRenderer>()
                && selectObj.transform.GetComponent<MeshFilter>())
            {
                DestroyImmediate(selectObj.transform.GetComponent<MeshRenderer>());
                DestroyImmediate(selectObj.transform.GetComponent<MeshFilter>());
            }
        }
        else
        {
            // 有子物体，说明点的是颗粒，需要把所有Mesh组件全部删除
            for (int i = 0; i < selectObj.transform.childCount; i++)
            {
                // 把物件_1先排除掉
                if (!Equals(selectObj.transform.GetChild(i).name, "物件_1"))
                {
                    // 如果子物体上有MeshRenderer和MeshFilter组件再移除
                    if (selectObj.transform.GetChild(i).GetComponent<MeshRenderer>()
                        && selectObj.transform.GetChild(i).GetComponent<MeshFilter>())
                    {
                        DestroyImmediate(selectObj.transform.GetChild(i).GetComponent<MeshRenderer>());
                        DestroyImmediate(selectObj.transform.GetChild(i).GetComponent<MeshFilter>());
                    }
                }
            }
        }
    }

    #endregion

    #region 删除指定后缀名的文件

    /// <summary>
    /// 删除指定后缀名的文件
    /// </summary>
    public void DeleteSelectExtension()
    {
        if (DeleteFilePath == string.Empty)
        {
            WindowTips("请输入要修改的文件路径");
            return;
        }

        if (SelectExtension == string.Empty)
        {
            WindowTips("请点择文件后缀名");
            return;
        }

        var files = Directory.GetFiles(DeleteFilePath, "*.*", SearchOption.AllDirectories);
        foreach (var str in files)
        {
            var extension = Path.GetExtension(str);
            if (Equals(extension, SelectExtension))
            {
                File.Delete(str);
            }
        }
    }

    #endregion

    public static SelfTools Instance()
    {
        return instance ?? (instance = new SelfTools());
    }
}