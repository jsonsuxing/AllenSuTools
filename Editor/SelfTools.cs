// ========================================================
// 描述：综合小工具编辑器管理类
// 作者：苏醒 
// 创建时间：2019-07-18 21:22:38
// 版 本：1.0
// ========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChinarX.ExtensionMethods;
using UI.ThreeDimensional;
using UnityEditor;
using UnityEngine;
using LitJson;
using Object = UnityEngine.Object;


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

    // 镜像克隆对象
    public string SetMirrorAxis = "y"; // 设置镜像克隆的对称轴
    public Vector3 MirrorPoint = Vector3.zero; // 指定对称中心

    // 查询文件夹下的同名文件
    public string FilePath = string.Empty; // 文件路径

    // 零件库相关数据显示区
    // 总数据区
    public int AllGranuleTypeCount = 0; // 目前的总颗粒类数(包含中颗粒)
    public int AllGranuleCount = 0; // 目前的总颗粒个数(包含中颗粒)
    public int AllUndoneGranuleCount = 0; // 所有未完成的颗粒个数(含有"待定颗粒"标识的个数)
    public int AllCompletedGranuleCount = 0; // 所有已完成的颗粒个数(不含有"待定颗粒"标识的个数)
    // 初级零件库数据区
    public int PrimaryTypeCount = 0; // 初级零件库颗粒大类个数
    public int AllPrimaryGranuleCount = 0; // 初级零件库总颗粒个数
    public int PrimaryUndoneGranuleCount = 0; // 初级零件库含有待定颗粒个数
    public int PrimaryCompletedGranuleCount = 0; // 初级零件库已完成颗粒个数
    // 中级零件库数据区
    public int IntermediateTypeCount = 0; // 中级零件库颗粒大类个数
    public int AllIntermediateGranuleCount = 0; // 中级零件库总颗粒个数
    public int IntermediateUndoneGranuleCount = 0; // 中级零件库含有待定颗粒个数
    public int IntermediateCompletedGranuleCount = 0; // 中级零件库已完成颗粒个数



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
        // ReadLddID();
        // return;

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
               Object.DestroyImmediate(selectObj.transform.GetComponent<MeshRenderer>());
                Object.DestroyImmediate(selectObj.transform.GetComponent<MeshFilter>());
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
                        Object.DestroyImmediate(selectObj.transform.GetChild(i).GetComponent<MeshRenderer>());
                        Object.DestroyImmediate(selectObj.transform.GetChild(i).GetComponent<MeshFilter>());
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

    #region 镜像克隆对象

    /// <summary>
    /// 镜像克隆对象
    /// </summary>
    public void MirrorObj()
    {
        var gameObjects = Selection.gameObjects;
        if (gameObjects.Length == 0)
        {
            WindowTips("至少要选中一个对象");
            return;
        }
        else
        {
            var cloneObjList = new List<GameObject>();
            var tempI = 0;
            foreach (var obj in gameObjects)
            {
                if (obj.GetComponent<BoxCollider>())
                {
                    var boxCollider = obj.GetComponent<BoxCollider>();
                    obj.transform.position = obj.transform.TransformPoint(boxCollider.center);
                    boxCollider.center     = Vector3.zero;
                }
                
                var newObject = Object.Instantiate(obj, obj.transform.parent);
                Undo.RegisterCreatedObjectUndo(newObject,"MirrorObjects");
                if (obj.name.Contains("环形碰撞盒"))
                {
                    newObject.name = newObject.name.Replace("(Clone)", "(Mirror)");
                }
                else
                {
                    tempI++;
                    var maxIndex =int.Parse(GetBuWeiMaxNameIndex(gameObjects).ToString())+tempI;
                    newObject.name = GetBuWeiChineseName(obj) + " (" + maxIndex + ")";
                }
                cloneObjList.Add(newObject);
            }
          
            switch (SetMirrorAxis)
            {
                case "x":
                    foreach (var cloneObj in cloneObjList)
                    {
                        cloneObj.transform.position = new Vector3(cloneObj.transform.position.x - 2 * (cloneObj.transform.position.x - MirrorPoint.x), cloneObj.transform.position.y, cloneObj.transform.position.z);
                        var vector3Rotation = cloneObj.transform.rotation.eulerAngles;
                        var newEulerAngles  = new Vector3(vector3Rotation.x, -vector3Rotation.y, -vector3Rotation.z);
                        cloneObj.transform.rotation = Quaternion.Euler(newEulerAngles);
                    }
                    break;
                case "y":
                    foreach (var cloneObj in cloneObjList)
                    {
                        cloneObj.transform.position = new Vector3(cloneObj.transform.position.x, cloneObj.transform.position.y - 2 * (cloneObj.transform.position.y - MirrorPoint.y), cloneObj.transform.position.z);
                        var vector3Rotation = cloneObj.transform.rotation.eulerAngles;
                        var newEulerAngles  = new Vector3(-vector3Rotation.x, -vector3Rotation.y, -vector3Rotation.z);
                        cloneObj.transform.rotation = Quaternion.Euler(newEulerAngles);
                    }
                    break;
                case "z":
                    foreach (var cloneObj in cloneObjList)
                    {
                        cloneObj.transform.position = new Vector3(cloneObj.transform.position.x, cloneObj.transform.position.y, cloneObj.transform.position.z - 2 * (cloneObj.transform.position.z - MirrorPoint.z));
                        var vector3Rotation = cloneObj.transform.rotation.eulerAngles;
                        var newEulerAngles  = new Vector3(-vector3Rotation.x, -vector3Rotation.y, vector3Rotation.z);
                        cloneObj.transform.rotation = Quaternion.Euler(newEulerAngles);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            cloneObjList.Clear();
        }
    }

    #endregion

    #region 查询文件夹下的同名文件

    /// <summary>
    /// 查询文件夹下的同名fbx文件
    /// </summary>
    public void SameNameFile()
    {
        if (Equals(FilePath, string.Empty))
        {
            WindowTips("文件路径不能为空");
            return;
        }

        var ht = new Hashtable();
        // 获取到所有 fbx 文件
        var files = Directory.GetFiles(FilePath, "*.fbx", SearchOption.AllDirectories);
        // 同名文件个数
        var sameFileNum = 0;
        for (var i = 0; i < files.Length; i++)
        {
            var fileName = Path.GetFileNameWithoutExtension(files[i]);
            if (ht.ContainsValue(fileName))
            {
                WriteToTxt(TxtDirPath,"同名 fbx 文件","第 " + (++sameFileNum) +" 个："+fileName);
            }
            else
            {
                ht.Add(i,fileName);
            }
        }

        if (sameFileNum == 0)
        {
            WindowTips("没有同名文件");
        }
        else
        {
            WindowTips("已查到 " + sameFileNum + " 个同名文件，详见 《 D:/ 编辑器生成的txt文件汇总 》 文件夹");
            System.Diagnostics.Process.Start(TxtDirPath); // 文件夹存在就直接打开
        }

        FilePath = string.Empty;
        sameFileNum = 0;
    }

    #endregion

    #region 零件库相关数据显示区

    /// <summary>
    /// 零件库相关数据显示区
    /// </summary>
    public void ShowLibraryData()
    {
        // 如果 Content 对象存在
        if (Content)
        {
            // 初级、中级颗粒分割线下标
            var splitLineIndex = 0;

            for (var i = 0; i < Content.transform.childCount; i++)
            {
                AllChildObj.Add(Content.transform.GetChild(i).gameObject);
                if (Equals(AllChildObj[i].name, "中颗粒泊类")) splitLineIndex = AllChildObj[i].transform.GetSiblingIndex();
            }
            Debug.Log("splitLineIndex="+ splitLineIndex);
           
            // 列表数据赋值区
            foreach (var child in AllChildObj)
            {
                if(child.GetComponent<PrimaryBlockType>()) PrimaryTypeList.Add(child);            // 为初级零件库大类赋值
                if (child.GetComponent<IntermediateBlockType>()) IntermediateTypeList.Add(child); // 为中级零件库大类赋值

                if (child.GetComponent<UIObject3D>())
                {
                    // 为所有的待定颗粒个数、完成修改的颗粒个数赋值
                    if (child.GetComponent<UIObject3D>().待定颗粒) AllUndoneGranuleCount++;
                    else AllCompletedGranuleCount++;
                }

                if (child.transform.GetSiblingIndex() <= splitLineIndex && child.GetComponent<UIObject3D>() && !child.transform.name.Contains("组合"))
                {
                    PrimaryGranuleList.Add(child); // 为初级零件库颗粒赋值
                }
                if (child.transform.GetSiblingIndex() > splitLineIndex && child.GetComponent<UIObject3D>())
                {
                    IntermediateGranuleList.Add(child); // 为中级零件库颗粒赋值
                }
            }

            // 初级数据处理区
            foreach (var primaryChild in PrimaryGranuleList)
            {
                if (primaryChild.GetComponent<UIObject3D>().待定颗粒) PrimaryUndoneGranuleCount++;
                else PrimaryCompletedGranuleCount++;
            }
            // 中级数据处理区
            foreach (var intermediateChild in IntermediateGranuleList)
            {
                if (intermediateChild.GetComponent<UIObject3D>().待定颗粒) IntermediateUndoneGranuleCount++;
                else IntermediateCompletedGranuleCount++;
            }

            // 初级零件库数据传值
             PrimaryTypeCount = PrimaryTypeList.Count; // 初级零件库颗粒大类个数
             AllPrimaryGranuleCount = PrimaryGranuleList.Count; // 初级零件库颗粒总个数

            // 中级零件库数据传值
            IntermediateTypeCount = IntermediateTypeList.Count; // 中级零件库颗粒大类个数
            AllIntermediateGranuleCount = IntermediateGranuleList.Count; // 中级零件库颗粒总个数

            // 总数据传值
            AllGranuleTypeCount = PrimaryTypeCount + IntermediateTypeCount;
            AllGranuleCount = Content.transform.childCount - PrimaryTypeList.Count - IntermediateTypeList.Count; // 目前的总颗粒个数(包含中颗粒)
        }
        else
        {
            WindowTips("Content 对象不存在 ！ ！ ！");
        }

    }

    #endregion


    public static SelfTools Instance()
    {
        return instance ?? (instance = new SelfTools());
    }
}