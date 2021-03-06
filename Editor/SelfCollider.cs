// ========================================================
// 描述：处理碰撞盒相关的操作
// 作者：苏醒
// 创建时间：2019-07-02 11:38:08
// 版 本：1.0
// ========================================================

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditorInternal;
using Application = UnityEngine.Application;
using Object = UnityEngine.Object;


#region 枚举

/// <summary>
/// 轴向
/// </summary>
public enum PivotAxis
{
    X,
    Y,
    Z
};


/// <summary>
/// 环形碰撞盒编辑状态
/// </summary>
public enum EditorStatus
{
    准备,
    编辑,
    非均匀尺度,
    无效的选择
}


/// <summary>
/// 自定义操作碰撞盒
/// </summary>
public enum SelfPivotAxis
{
    X轴正方向,
    X轴负方向,
    Y轴正方向,
    Y轴负方向,
    Z轴正方向,
    Z轴负方向,
}

#endregion


/// <summary>
/// 碰撞盒管理类
/// </summary>
public class SelfCollider : CommonFun
{
    // 碰撞盒管理类
    private static SelfCollider instance;

    #region MenuItem

    // 1--复制粘贴当前碰撞盒
    [MenuItem("CONTEXT/BoxCollider/一键复制，粘贴当前碰撞盒")]
    static void CopyAndPaste(MenuCommand command)
    {
        ComponentUtility.CopyComponent((BoxCollider) command.context);    // 复制
        ComponentUtility.PasteComponentAsNew(Selection.activeGameObject); // 粘贴
    }

    #endregion

    #region 字段声明

    //父物体上碰撞盒的个数
    public BoxCollider[] ParentBoxNumGroup;

    // 显示，隐藏碰撞盒
    public bool   IsHideAllBoxColl;              // 是否隐藏所有碰撞盒
    public bool   IsClickHideBtn;                // 是否点了隐藏或者显示碰撞盒按钮
    public bool   IsClickRemoveBtn;              // 是否点了移除所有碰撞盒按钮
    public string HideOrShowTips = string.Empty; // 隐藏或者显示提示文本

    // 一键添加长方体类型的碰撞盒(如高一粒)
    public float TuQiHeight = 0.17f; //凸起的高度

    // 添加轮胎类的碰撞盒
    public PivotAxis    PivotAxis          = PivotAxis.Y;        // 碰撞盒以哪个轴生成，默认以Y轴
    public Vector3 SelfRoundPivotAxis = Vector3.zero; // 自定义轴心x，y，z
    public EditorStatus EditorStatus       = EditorStatus.无效的选择; // 默认编辑状态
    public int          BoxCollNumGenerate = 8;                  // 生成多少个碰撞盒，默认为8个
    public float        OuterRadius        = 0.14f;              // 碰撞盒外半径
    public float        InnerRadius        = 0.01f;              // 碰撞盒内半径
    public float        Height             = 0.79f;              // 碰撞盒高度
    public float        RotationOffset     = 0f;                 // 旋转角度
    public float        DiffValue          = 0.004f;             // 自动生成的碰撞盒与实际模型尺寸的差值(尺寸完全一致不会吸合)
    public bool         IsLockInnerRadius;                       // 是否禁用内半径，默认关闭
    public GameObject   WorkingCollider = null;                  // 接收生成的环形碰撞盒
    public string       ChoseQuickData  = "小圆棍";              // 当前所选的快捷数据

    // 自定义倾斜环形碰撞盒
    // 旋转
    public int   CustomBoxCollNum = 8; // 生成多少个碰撞盒，默认为8个
    // 平移
    public SelfPivotAxis SelfPivotAxis = SelfPivotAxis.X轴正方向; // 选择轴向
    public float         CloneSpace    = 0.8f;                   // 克隆的间隔

    // 显示模型的长宽高
    public double ModelLength = 0; // 模型长度
    public double ModelWidth  = 0; // 模型宽度
    public double ModelHeight = 0; // 模型高度
    public double AoCaoY      = 0; // 正方体类型颗粒底部凹槽Y轴坐标
    public double TuQiY       = 0; // 正方体类型颗粒凸起下方Y轴坐标
    public double MinY        = 0; // Y轴最小坐标
    public double MaxY        = 0; // Y轴最大坐标
    public double MinX        = 0; // X轴最小坐标
    public double MaxX        = 0; // X轴最大坐标
    public double MinZ        = 0; // Z轴最小坐标
    public double MaxZ        = 0; // Z轴最大坐标

    // 显示有几个碰撞盒
    public int ChildBoxCollNum = 0; // 所有子物体下有几个碰撞盒

    // 计算带角度模型
    // 1：角度
    public Vector3 FirstAnglePos = Vector3.zero;  // 点击的第一个点的坐标
    public Vector3 SecondAnglePos = Vector3.zero; // 点击的第二个点的坐标
    public Vector3 AngleResult = Vector3.zero;    // 输出结果
    public int AngleClickTime = 1;                // 第几次点击
    public string StrAngleTips = string.Empty;    // 点击了几次的提示信息

    // 2：位置
    public Vector3 FirstPos = Vector3.zero;       // 点击的第一个点的坐标
    public Vector3 SecondPos = Vector3.zero;      // 点击的第二个点的坐标
    public int PosClickTime = 1;                  // 第几次点击


    // 计算圆心
    public Vector3 FirstPoint = Vector3.zero;  // 第一个点
    public Vector3 SecondPoint = Vector3.zero; // 第二个点
    public Vector3 ThirdPoint = Vector3.zero;  // 第三个点
    public int WhichTimeClick = 0; // 点击了第几次
    public string WhichTimeTips = string.Empty; // 点击了几次的提示信息

    // 长方体的一条对角线两个顶点，确定一个 BoxCollider
    public List<Vector3> VertexList;  // 获取到的顶点列表


    #endregion

    #region 一：克隆碰撞盒

    #region 一：对角线两顶点确定碰撞盒

    public void VertexBox()
    {
        if (VertexList == null) VertexList = new List<Vector3>();
        VertexList.Add(Selection.activeGameObject.transform.position);

        if (VertexList.Count == 2)
        {
            for (var i = 0; i < VertexList.Count; i++)
            {
                // 变换位置，从世界坐标到局部坐标。和 Transform.TransformPoint 相反
                VertexList[i] = Selection.activeTransform.InverseTransformPoint(VertexList[i]);
            }
            float xMin, xMax = xMin = VertexList[0].x;
            float yMin, yMax = yMin = VertexList[0].y;
            float zMin, zMax = zMin = VertexList[0].z;

            foreach (var vector in VertexList)
            {
                // 获取用于创建向量的最小值和最大值。
                xMin = (vector.x < xMin) ? vector.x : xMin;
                xMax = (vector.x > xMax) ? vector.x : xMax;
                yMin = (vector.y < yMin) ? vector.y : yMin;
                yMax = (vector.y > yMax) ? vector.y : yMax;
                zMin = (vector.z < zMin) ? vector.z : zMin;
                zMax = (vector.z > zMax) ? vector.z : zMax;
            }
            var maxVector   = new Vector3(xMax, yMax, zMax);
            var minVector   = new Vector3(xMin, yMin, zMin);

            var size        = maxVector - minVector;
            var center      = (maxVector + minVector) / 2;
            // 如果原来物体上有碰撞盒，则先移除
            if (Selection.activeGameObject.GetComponent<BoxCollider>()) Object.DestroyImmediate(Selection.activeGameObject.GetComponent<BoxCollider>());
            var boxCollider = Undo.AddComponent<BoxCollider>(Selection.activeGameObject);
            // boxCollider.size      = size;
            boxCollider.size=new Vector3(size.x - DiffValue , size.y - DiffValue , size.z - DiffValue);
            boxCollider.center = center;
            // 生成完所需尺寸碰撞盒后，再转换其世界和局部的坐标
            boxCollider.transform.position = boxCollider.transform.TransformPoint(boxCollider.center);
            boxCollider.center=Vector3.zero;

            VertexList.Clear();
        }
    }


    #endregion

    #region 二：添加环形类的碰撞盒

    /// <summary>
    /// 环形碰撞盒的GUI总控
    /// </summary>
    public void RingBoxCollGuiControl()
    {
        DrawRingBoxCollGui();
        SetEditorStatus();
    }


    /// <summary>
    /// 绘制环形碰撞盒GUI
    /// </summary>
    public void DrawRingBoxCollGui()
    {
        var style = new GUIStyle
        {
            normal =
            {
                textColor = Color.blue
            },
            fontSize = 20
        };
        // 如果启用，将内半径与外半径的距离存储到锁定距离
        var radiusDiff = OuterRadius - InnerRadius;
        GUILayout.Label("一：克隆环形类的碰撞盒", style);

        //------------一：开始垂直画盒子------------
        GUILayout.BeginVertical("box");

        // 第一组垂直排版开始
        EditorGUILayout.BeginVertical();

        GUILayout.Label("Tips：轴心、对称中心 同步下方的《自定义操作碰撞盒》");
        GUILayout.Space(3);

        // 选择轴心
        PivotAxis = (PivotAxis) EditorGUILayout.EnumPopup("1：轴心", PivotAxis);
        GUILayout.Space(3);

        // 选择对称中心
        SelfRoundPivotAxis = EditorGUILayout.Vector3Field("2：对称中心(默认原点)", SelfRoundPivotAxis);
        GUILayout.Space(3);

        if (GUILayout.Button("选择对象，设置对称中心")) SetMyPivot();
        GUILayout.Space(3);

        // 设置边数，最小3，最大64
        BoxCollNumGenerate = EditorGUILayout.IntSlider("3：设置边数", BoxCollNumGenerate, 3, 64);
        GUILayout.Space(3);

        // 外半径（从指定外半径的值到无穷大。0.011f,无穷大）
        OuterRadius = Mathf.Max(EditorGUILayout.FloatField("4：外半径", OuterRadius), 0.011f);
        EditorGUILayout.EndVertical();
        // 第一组垂直排版结束
        GUILayout.Space(3);

        // 第二组水平排版开始
        EditorGUILayout.BeginHorizontal();
        if (IsLockInnerRadius) GUI.enabled = false;
        // 内半径（最小0.01，最大值要小于外半径-0.01）
        InnerRadius = Mathf.Clamp(EditorGUILayout.FloatField("内半径", InnerRadius), 0.01f, OuterRadius - 0.01f);
        GUI.enabled = true;
        // 锁
        IsLockInnerRadius = GUILayout.Toggle(IsLockInnerRadius, new GUIContent("锁内半径", "将内半径值锁定到距外半径的当前距离"), "Button");
        GUILayout.MaxWidth(50f);
        EditorGUILayout.EndHorizontal();
        // 第二组水平排版结束

        // 锁定内外半径间的距离
        if (IsLockInnerRadius) InnerRadius = Mathf.Max(0.1f, OuterRadius - radiusDiff);

        // 碰撞盒高度从0.01到无穷大
        Height      = Mathf.Max(EditorGUILayout.FloatField("手动调整碰撞盒高度", Height), 0.01f);
        GUI.enabled = true;
        GUILayout.EndVertical();
        //------------一：结束垂直画盒子------------

        // ------------ 一：开始垂直画盒子 ------------
        GUILayout.BeginVertical("box");
        GUILayout.Label("快捷设置数据", SetGuiStyle(Color.red, 16));
        GUILayout.Space(3);

        // 第一组水平排版开始
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("轮架"))
        {
            // 轮架数据 length=0.8，limit勾选
            // 小圆洞的 length=0.8-0.18=0.72，limit不勾选
            OuterRadius    = 0.208f; // 丰满数据：0.24f
            InnerRadius    = 0.16f;
            Height         = 0.71f;
            ChoseQuickData = "轮架";
        }
        if (GUILayout.Button("小圆棍"))
        {
            OuterRadius    = 0.14f;
            InnerRadius    = 0.01f;
            Height         = 0.79f; // 1.58
            ChoseQuickData = "小圆棍";
        }
        if (GUILayout.Button("插槽"))
        {
            OuterRadius    = 0.3f;
            InnerRadius    = 0.245f;
            Height         = 0.79f;
            ChoseQuickData = "插槽";
        }

        EditorGUILayout.EndHorizontal();
        // 第一组水平排版结束

        // 第二组水平排版开始
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("厚轮架格挡"))
        {
            OuterRadius    = 0.27f; // 丰满数据：0.28f
            InnerRadius    = 0.01f;
            Height         = 0.15f;
            ChoseQuickData = "厚轮架格挡";
        }
        if (GUILayout.Button("薄轮架格挡"))
        {
            OuterRadius    = 0.27f; // 丰满数据：0.28f
            InnerRadius    = 0.01f;
            Height         = 0.075f;
            ChoseQuickData = "薄轮架格挡";
        }
        if (GUILayout.Button("圆洞"))
        {
            OuterRadius    = 0.35f;
            InnerRadius    = 0.31f;
            Height         = 0.79f;
            ChoseQuickData = "圆洞";
            // 其它匹配数据
            //一：0.29  0.23  二：0.37  0.23
        }
        EditorGUILayout.EndHorizontal();
        // 第二组水平排版结束
        GUILayout.Space(3);

        // 第N组水平排版开始
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("提示：当前所选快捷数据是", SetGuiStyle(Color.black, 14));
        GUILayout.TextField(ChoseQuickData);
        EditorGUILayout.EndHorizontal();
        // 第N组水平排版结束

        GUILayout.EndVertical();
        // ------------ 一：结束垂直画盒子 ------------
    }


    /// <summary>
    /// 设置编辑状态
    /// </summary>
    public void SetEditorStatus()
    {
        // 如果已经有了碰撞盒 
        if (WorkingCollider                 != null) EditorStatus = EditorStatus.编辑;
        else if (Selection.activeGameObject != null)
        {
            var selectTrans = Selection.activeGameObject.transform;

            // 检查非均匀尺度目标对象和它的所有父物体
            while (selectTrans != null)
            {
                var targetObjectScale = selectTrans.localScale;
                if (Math.Abs(targetObjectScale.x - targetObjectScale.y) > 0.0001f ||
                    Math.Abs(targetObjectScale.x - targetObjectScale.z) > 0.0001f)
                {
                    EditorStatus = EditorStatus.非均匀尺度;
                    return;
                }

                selectTrans = selectTrans.parent;
            }

            EditorStatus = EditorStatus.准备;
        }
        else
        {
            // 没有选择任何内容
            EditorStatus = EditorStatus.无效的选择;
        }
    }


    /// <summary>
    /// 创建环形碰撞盒
    /// </summary>
    public void CreateRingBoxColl()
    {
        var selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject == null)
        {
            WindowTips("没有选中模型");
            return;
        }

        // 根据是否开启高宽度自适应来决定环形碰撞盒的高度
        var ringHeight = Height;

        // 调用创建环形碰撞盒
        var compoundCollider = CreateRing(PivotAxis, BoxCollNumGenerate, OuterRadius, InnerRadius, ringHeight, RotationOffset);

        // 移动对撞机，使其位置相对于选定的游戏对象
        // 修改前：compoundCollider.transform.position += selectedGameObject.transform.position;
        // 修改后：将生成位置设置为自定义的位置
        compoundCollider.transform.position += SelfRoundPivotAxis;

        // 暂时将所选游戏对象的旋转设置为零，以避免对撞机的父级设置时对撞机被扭曲
        var originalRotation = selectedGameObject.transform.rotation;
        selectedGameObject.transform.rotation = Quaternion.identity;

        // 将碰撞盒的父对象设置为所选的游戏对象
        compoundCollider.transform.SetParent(selectedGameObject.transform);

        // 恢复选择游戏对象的原始旋转
        selectedGameObject.transform.rotation = originalRotation;

        // 将创建的对撞机设置为工作对撞机
        WorkingCollider = compoundCollider;

        // 订阅撤消事件，以便我们知道何时清除对工作对撞机的引用
        Undo.undoRedoPerformed += OnUndoRedo;
    }


    /// <summary>
    /// 创建一个环形复合对撞机游戏对象
    /// </summary>
    public GameObject CreateRing(PivotAxis pivotAxis, int sides, float outerRadius, float innerRadius, float height, float rotationOffset)
    {
        // 空的游戏对象，将作为环形碰撞盒的根
        var compoundCollider = new GameObject("环形碰撞盒");
       
        var length           = 2 * outerRadius * Mathf.Tan(Mathf.PI / sides);
        var width            = outerRadius - innerRadius;
        for (var i = 1; i <= sides; i++)
        {
            var segment = new GameObject
            {
                name = "Bevel Box " + "(" + i + ")"
            };
            segment.AddComponent<BoxCollider>();
            var collider = segment.GetComponent<BoxCollider>();
            // collider.isTrigger = true;
            collider.size = new Vector3(length, height, width);
            // 设置比例(项目里父物体的localScale都为1)
            segment.transform.localScale = Vector3.one;

            //注释该行，让 Box 的 localScale 和 BoxCollider 的 size 交换，保证与项目规定统一，要不加完环形碰撞盒后，在Scene场景里会看到碰撞盒被放大
            //segment.transform.localScale= new Vector3(Length, Height, width);

            // 应用纵向旋转
            segment.transform.Rotate(Vector3.right, rotationOffset);
            var segmentAngleDegrees = i * (360f / sides);

            // 相对于父物体的位置
            segment.transform.position = new Vector3(0f, 0f, outerRadius - (width / 2));
            segment.transform.RotateAround(Vector3.zero, Vector3.up, segmentAngleDegrees);

            // 旋转默认为Y，所以如果Y则跳过此步骤
            if (pivotAxis != PivotAxis.Y)
            {
                var rotationAxis = (pivotAxis == PivotAxis.Z) ? Vector3.right : Vector3.forward;
                segment.transform.RotateAround(Vector3.zero, rotationAxis, 90f);
            }

            // 以"环形碰撞盒"为父物体
            segment.transform.SetParent(compoundCollider.transform, true);
        }

        Undo.RegisterCreatedObjectUndo(compoundCollider, "Create Torus Compound Collider");
        return compoundCollider;
    }


    /// <summary>
    /// 环形碰撞盒的注销事件
    /// </summary>
    public void OnUndoRedo()
    {
        // 明确提及工作对撞机
        WorkingCollider = null;
        // 取消订阅将来的撤消事件
        Undo.undoRedoPerformed -= OnUndoRedo;
    }


    /// <summary>
    /// 更新环形碰撞盒
    /// </summary>
    public void UpdateRingBoxColl()
    {
        // 如果存在工作对撞机，则删除它
        if (WorkingCollider != null) Object.DestroyImmediate(WorkingCollider);
        // 创建一个新的对撞机
        CreateRingBoxColl();
    }


    /// <summary>
    /// 绘制一个文本框，指示工具的状态
    /// </summary>
    public void DrawStatusText()
    {
        var statusText  = string.Empty;
        var statusColor = Color.clear;
        switch (EditorStatus)
        {
            case EditorStatus.编辑:
                statusColor = new Color(1f, 0.5f, 0f); //橘黄色
                statusText  = "编辑";
                break;
            case EditorStatus.准备:
                statusColor = Color.green;
                statusText  = "准备";
                break;
            case EditorStatus.非均匀尺度:
                statusColor = Color.red;
                statusText  = "对象或者父节点没有统一的尺寸";
                break;
            case EditorStatus.无效的选择:
                break;
            default:
                statusColor = Color.yellow;
                statusText  = "请先选择一个对象";
                break;
        }

        GUI.color = statusColor;
        GUILayout.Box(statusText, GUILayout.ExpandWidth(true));
        GUI.color = Color.white;
    }

    /// <summary>
    /// 整理、拆分碰撞盒
    /// </summary>
    public void DeleteAndArrangeRing()
    {
        var selectedGameObjects = Selection.gameObjects;
        if (selectedGameObjects.Length == 0)
        {
            WindowTips("所选物体没有子物体，不能拆分");
            return;
        }

        foreach (var ringBox in selectedGameObjects)
        {
            var childLength = ringBox.transform.childCount;
            // 存储每个父物体下所有子物体的数组
            var ringBoxObjArray = new GameObject[childLength];

            // 为每个数组赋值obj对象
            for (var i = 0; i < childLength; i++)
            {
                ringBoxObjArray[i] = ringBox.transform.GetChild(i).gameObject;
            }
            // 设置颗粒预设为父物体
            foreach (var bevelBox in ringBoxObjArray)
            {
                bevelBox.transform.SetParent(ringBox.transform.parent);
            }
            // 删除每个父物体
            Object.DestroyImmediate(ringBox);
        }
    }

    #endregion

    #region 三：自定义倾斜环形碰撞盒

    /// <summary>
    /// 旋转
    /// </summary>
    public void RotateBoxCollider()
    {
        var selectObj = Selection.gameObjects;
        if (selectObj == null)
        {
            WindowTips("所选物体不能为空");
            return;
        }
        if (CustomBoxCollNum == 0)
        {
            WindowTips("克隆个数不能为0");
            return;
        }

        // 克隆物体时的朝向
        var pivot = Vector3.zero;
        switch (PivotAxis)
        {
            case PivotAxis.X:
                pivot = Vector3.right;
                break;
            case PivotAxis.Y:
                pivot = Vector3.up;
                break;
            case PivotAxis.Z:
                pivot = Vector3.forward;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // 所选名称括号内的数字下标(返回值无法自增，特用变量接收)
        // var nameIndex = GetBuWeiMaxNameIndex(selectObj);
        for (var i = 0; i < CustomBoxCollNum; i++)
        {
            var cloneObj = Object.Instantiate(selectObj[0]);
            Undo.RegisterCreatedObjectUndo(cloneObj, "RotateBoxCollider");
            cloneObj.name = "Bevel Box " + "(" + (i + 1) + ")";
            // cloneObj.name = GetBuWeiChineseName(selectObj[0]) + " (" + (++nameIndex) + ")";
            // 克隆的角度
            var cloneAngle = i * (360f / CustomBoxCollNum);

            cloneObj.transform.RotateAround(SelfRoundPivotAxis, pivot, cloneAngle);
            cloneObj.transform.SetParent(selectObj[0].transform.parent, true);
        }
        Undo.DestroyObjectImmediate(selectObj[0]);
    }


    /// <summary>
    /// 自定义旋转中心
    /// </summary>
    public void SetMyPivot()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("所选物体不能为空");
            return;
        }

        var localPosition = selectObj.transform.localPosition;
        SelfRoundPivotAxis = new Vector3(localPosition.x, localPosition.y, localPosition.z);
    }


    /// <summary>
    /// 平移
    /// </summary>
    public void PanBoxCollider()
    {
        var selectObj = Selection.gameObjects;
        if (selectObj == null)
        {
            WindowTips("所选物体不能为空");
            return;
        }
        if (CustomBoxCollNum == 0)
        {
            WindowTips("克隆个数不能为0");
            return;
        }

        // 记录被克隆物体的x,y,z
        var selectObjX = selectObj[0].transform.localPosition.x;
        var selectObjY = selectObj[0].transform.localPosition.y;
        var selectObjZ = selectObj[0].transform.localPosition.z;
       
        // 所选名称括号内的数字下标(返回值无法自增，特用变量接收)
        var nameIndex = GetBuWeiMaxNameIndex(selectObj);

        for (var i = 1; i <= CustomBoxCollNum; i++)
        {
            var cloneObj = Object.Instantiate(selectObj[0]);
            Undo.RegisterCreatedObjectUndo(cloneObj, "PanBoxCollider");
            cloneObj.name = GetBuWeiChineseName(selectObj[0]) + " (" + (++nameIndex) + ")";
            switch (SelfPivotAxis)
            {
                case SelfPivotAxis.X轴正方向:
                    cloneObj.transform.localPosition = new Vector3(selectObjX + i * CloneSpace, selectObjY, selectObjZ);
                    break;
                case SelfPivotAxis.X轴负方向:
                    cloneObj.transform.localPosition = new Vector3(selectObjX - i * CloneSpace, selectObjY, selectObjZ);
                    break;
                case SelfPivotAxis.Y轴正方向:
                    cloneObj.transform.localPosition = new Vector3(selectObjX, selectObjY + i * CloneSpace, selectObjZ);
                    break;
                case SelfPivotAxis.Y轴负方向:
                    cloneObj.transform.localPosition = new Vector3(selectObjX, selectObjY - i * CloneSpace, selectObjZ);
                    break;
                case SelfPivotAxis.Z轴正方向:
                    cloneObj.transform.localPosition = new Vector3(selectObjX, selectObjY, selectObjZ + i * CloneSpace);
                    break;
                case SelfPivotAxis.Z轴负方向:
                    cloneObj.transform.localPosition = new Vector3(selectObjX, selectObjY, selectObjZ - i * CloneSpace);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            cloneObj.transform.SetParent(selectObj[0].transform.parent);
        }
    }

    #endregion

    #region 四：一键添加长方体类型的碰撞盒(如高一粒)

    /// <summary>
    /// 为长方体类型颗粒添加碰撞盒
    /// </summary>
    public void AddBoxCollider()
    {
        IfSelectionIsNull("没有选中颗粒");

        // 从内存中找到颗粒
        var prefabObj = Resources.Load<GameObject>("Prefab/ModelPrefabs/" + Selection.activeGameObject.name);
        // 预设路径
        var prefabPath = AssetDatabase.GetAssetPath(prefabObj);

        // 实例化颗粒到场景
        var cloneObj = Object.Instantiate(prefabObj);
        // 获取“物件_1”上的 Mesh
        var bounds = cloneObj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds;
        // 获得模型原始尺寸的大小,并减去规定的差值
        var sizeX  = bounds.size.x * cloneObj.transform.localScale.x              - DiffValue;
        var sizeY  = bounds.size.y * cloneObj.transform.localScale.y - TuQiHeight - DiffValue;
        var scaleZ = bounds.size.z * cloneObj.transform.localScale.z              - DiffValue;

        var normalBox = new GameObject("Normal Box (1)");
        normalBox.transform.SetParent(cloneObj.transform);
        // 添加碰撞盒，并设置大小及中心点
        var boxCollider = normalBox.AddComponent<BoxCollider>();
        boxCollider.size   = new Vector3(sizeX, sizeY,             scaleZ);
        boxCollider.center = new Vector3(0,     -(TuQiHeight / 2), 0);

        // 替换模型
        PrefabUtility.SaveAsPrefabAsset(cloneObj, prefabPath);
        Object.DestroyImmediate(cloneObj);
    }

    #endregion


    #endregion

    #region 二：碰撞盒信息

    #region 一：隐藏，移除碰撞盒

    /// <summary>
    /// 隐藏，移除所有碰撞盒
    /// </summary>
    public void CollBtn()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中颗粒");
            return;
        }
        // 获得所选物体上所有的碰撞盒
        var allBoxCollider = selectObj.GetComponentsInChildren<BoxCollider>();
        if (allBoxCollider.Length == 0)
        {
            WindowTips("没有一个碰撞盒!!!");
            return;
        }

        // 如果点了隐藏或者显示碰撞盒按钮
        if (IsClickHideBtn)
        {
            IsHideAllBoxColl = !IsHideAllBoxColl;
            foreach (var boxCollider in allBoxCollider)
            {
                boxCollider.enabled = !IsHideAllBoxColl;
            }
            IsClickHideBtn = false;
        }

        // 如果点了移除所有碰撞盒按钮
        if (IsClickRemoveBtn)
        {
            foreach (var coll in allBoxCollider)
            {
                Object.DestroyImmediate(coll);
            }
            IsClickRemoveBtn = false;
        }
    }

    #endregion

    #region 二：显示模型数据

    /// <summary>
    /// 显示模型的长宽高
    /// </summary>
    public void ShowModelLengthWidthHeight()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中模型");
            return;
        }
        if (selectObj.transform.childCount == 0)
        {
            WindowTips("不能选择关键部位，要选中颗粒");
            return;
        }

        // 通过MeshFilter获得原始模型的Mesh,该值返回的结果是原始Mesh的尺寸，若要获得模型的尺寸大小还需要乘以模型的LocalScale
        var size = selectObj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds.size;
        ModelLength = Math.Round(size.x * selectObj.transform.localScale.x, 2); //长
        ModelHeight = Math.Round(size.y * selectObj.transform.localScale.y, 2); //高
        ModelWidth  = Math.Round(size.z * selectObj.transform.localScale.z, 2); //宽

        // 以下为临时追加的功能，仅为了做管道槽用到的
        AoCaoY = Math.Round(-(ModelHeight / 2),              3);
        TuQiY  = Math.Round((ModelHeight  / 2) - TuQiHeight, 3);

        // 获取到所有的顶点坐标
        var mesh     = selectObj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
        var vertices = mesh.vertices;
        
        var arrayX = new float[vertices.Length];
        var arrayY = new float[vertices.Length];
        var arrayZ = new float[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            arrayX[i] = vertices[i].x;
            arrayY[i] = vertices[i].y;
            arrayZ[i] = vertices[i].z;
        }
        MinX = Math.Round(arrayX.Min(), 4); // 最小X轴坐标
        MaxX = Math.Round(arrayX.Max(), 4); // 最大X轴坐标
        MinY = Math.Round(arrayY.Min(), 4); // 最小Y轴坐标
        MaxY = Math.Round(arrayY.Max(), 4); // 最大Y轴坐标
        MinZ = Math.Round(arrayZ.Min(), 4); // 最小Z轴坐标
        MaxZ = Math.Round(arrayZ.Max(), 4); // 最大Z轴坐标

        //碰撞盒总个数
        ShowBoxCollNum();
    }


    /// <summary>
    /// 清空模型数据
    /// </summary>
    public void ClearModelData()
    {
        ModelLength = ModelWidth = ModelHeight = AoCaoY = TuQiY = MinX = MaxX =
            MinY = MaxY = MinZ = MaxZ = ChildBoxCollNum = 0;
    }

    #endregion

    #region 三：打三点确定圆心

    public void CreateCenterOfCircle()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中任何对象");
            return;
        }

        WhichTimeClick++;

        switch (WhichTimeClick)
        {
            case 1:
                FirstPoint = selectObj.transform.position;
                WhichTimeTips = "第 1 个点已有值";
                break;
            case 2:
                SecondPoint = selectObj.transform.position;
                WhichTimeTips = "第 2 个点已有值";
                break;
            case 3:
                ThirdPoint = selectObj.transform.position;
                selectObj.transform.position = GetCenterOfCircle(FirstPoint, SecondPoint, ThirdPoint);
                WhichTimeTips = "圆心坐标：" + "( "+ selectObj.transform.position.x + "，" + selectObj.transform.position.y + "，" + selectObj.transform.position.z + " )";
                WhichTimeClick = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public Vector3 GetCenterOfCircle(Vector3 firstPoint, Vector3 secondPoint, Vector3 thirdPoint)
    {
        // 用于表达过点一，点二中垂线的平面
        var sfDiffX = secondPoint.x - firstPoint.x;
        var sfSumX = secondPoint.x + firstPoint.x;
        var sfDiffY = secondPoint.y - firstPoint.y;
        var sfSumY = secondPoint.y + firstPoint.y;
        var sfDiffZ = secondPoint.z - firstPoint.z;
        var sfSumZ = secondPoint.z + firstPoint.z;

        // 用于表达过点一，点三中垂线的平面
        var tfDiffX = thirdPoint.x - firstPoint.x;
        var tfSumX = thirdPoint.x + firstPoint.x;
        var tfDiffY = thirdPoint.y - firstPoint.y;
        var tfSumY = thirdPoint.y + firstPoint.y;
        var tfDiffZ = thirdPoint.z - firstPoint.z;
        var tfSumZ = thirdPoint.z + firstPoint.z;

        // 平面的法向量
        var sfDiff = secondPoint - firstPoint;
        var tfDiff = thirdPoint - firstPoint;
        var normalVector = Vector3.Cross(sfDiff, tfDiff);

        var t1 = (sfDiffX * sfSumX + sfDiffY * sfSumY + sfDiffZ * sfSumZ) / 2;
        var t2 = (tfDiffX * tfSumX + tfDiffY * tfSumY + tfDiffZ * tfSumZ) / 2;
        var t3 = normalVector.x * firstPoint.x + normalVector.y * firstPoint.y + normalVector.z * firstPoint.z;

        var d = CountDeterminant(sfDiffX, sfDiffY, sfDiffZ, tfDiffX, tfDiffY, tfDiffZ, normalVector.x, normalVector.y, normalVector.z);
        var d1 = CountDeterminant(t1, sfDiffY, sfDiffZ, t2, tfDiffY, tfDiffZ, t3, normalVector.y, normalVector.z);
        var d2 = CountDeterminant(sfDiffX, t1, sfDiffZ, tfDiffX, t2, tfDiffZ, normalVector.x, t3, normalVector.z);
        var d3 = CountDeterminant(sfDiffX, sfDiffY, t1, tfDiffX, tfDiffY, t2, normalVector.x, normalVector.y, t3);

        var mindX = d1 / d;
        var mindY = d2 / d;
        var mindZ = d3 / d;
    
        return new Vector3(mindX, mindY, mindZ);
    }

    public float CountDeterminant(float a1, float a2, float a3, float b1, float b2, float b3, float c1, float c2, float c3)
    {
        // 经试验，原本以为是加减法可以随意换位置，但改变顺序后会影响计算，我感觉是因为向量计算，而不是普通的加减法
        return (a1 * b2 * c3 + b1 * c2 * a3 + c1 * a2 * b3 - a3 * b2 * c1 - b3 * c2 * a1 - c3 * a2 * b1);
    }

    #endregion

    #region 四：计算带角度方向

    /// <summary>
    /// 带角度方向的计算
    /// </summary>
    public void AngleModel()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中任何对象");
            return;
        }

        var pos = selectObj.transform.position;
        if (AngleClickTime == 1)
        {
            FirstAnglePos = pos;
            StrAngleTips = "第 1 个点已有值";
        }
        else if (AngleClickTime == 2)
        {
            SecondAnglePos = pos;
            AngleResult = (FirstAnglePos - SecondAnglePos).normalized;
            selectObj.GetComponent<GuanJianBuWei>().dirVector = AngleResult;
            StrAngleTips = "向量值是：" + "( " + AngleResult.x + "，" + AngleResult.y + "，" + AngleResult.z + " )";
        }
        AngleClickTime++;
        if (AngleClickTime > 2) AngleClickTime = 1;
    }


    /// <summary>
    /// 绘制线框，并且获得中心位置
    /// </summary>
    public void DrawLineAndGetCenterPos()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中关键部位");
            return;
        }

        if (PosClickTime == 1)
        {
            FirstPos = selectObj.transform.position;
        }
        else if (PosClickTime == 2)
        {
            SecondAnglePos = selectObj.transform.position;
            var td = selectObj.GetComponent<TestDrawLine>();
            td.StartVector = FirstPos;
            td.EndVector = SecondPos;
        }

        PosClickTime++;
        if (PosClickTime > 2)
        {
            PosClickTime = 1;
        }
    }


    /// <summary>
    ///  清空获得到的数据
    /// </summary>
    public void ClearPosValue()
    {
        FirstPos = Vector3.zero;
        SecondPos = Vector3.zero;
        var td = Selection.activeGameObject.GetComponent<TestDrawLine>();
        td.StartVector = Vector3.zero;
        td.EndVector   = Vector3.zero;
        PosClickTime = 1;
    }


    public void AddTestDrawLine()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中关键部位");
            return;
        }
        selectObj.AddComponent<TestDrawLine>();
    }

    #endregion

    #region 五：从ACE项目移动预设

    /// <summary>
    /// 从ACE项目移动预设
    /// </summary>
    public void MovePrefabFromAce()
    {
        var acePath = "D:/Works/testFileFormat/Assets/Resources/Prefab/";
        var qdPath  = Application.dataPath + "/A-SuXing/selfPrefab/";
        try
        {
            // 如果指定路径不存在，则创建
            CreateNewDirectory(acePath);
            CreateNewDirectory(qdPath);
            var aceFiles = Directory.GetFiles(acePath, "*", SearchOption.AllDirectories);
            if (aceFiles.Length == 0) WindowTips("D:/Works/testFileFormat/Assets/Resources/Prefab/文件夹下没有预设");
            foreach (var str in aceFiles)
            {
                if (Path.GetExtension(str) == ".prefab")
                {
                    var prefabFileName = Path.GetFileNameWithoutExtension(str);
                    File.Move(str, Path.GetDirectoryName(qdPath) + "\\" + prefabFileName + Path.GetExtension(str));
                }

                if (Path.GetExtension(str) == ".meta") File.Delete(str);
            }

            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            WindowTips("错误信息：" + e);
            throw;
        }
    }


    /// <summary>
    /// 删除从ACE项目导入的预设
    /// </summary>
    public void DeletePrefab()
    {
        //加这个判断是因为每次删除后unity都会报警告，所以直接把meta也删了
        var meta = Application.dataPath + "/A-SuXing/selfPrefab/碰撞盒集合体.meta";
        if (File.Exists(meta)) File.Delete(meta);
        var qdPath = Application.dataPath + "/A-SuXing/selfPrefab/碰撞盒集合体.prefab";
        if (File.Exists(qdPath)) File.Delete(qdPath);
        AssetDatabase.Refresh();
    }

    #endregion

    #region 显示有几个碰撞盒

    /// <summary>
    /// 显示父物体，子物体碰撞盒的个数
    /// </summary>
    public void ShowBoxCollNum()
    {
        // 先清空上一个碰撞盒数据
        ClearBoxCollNum();
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中颗粒");
            return;
        }

        // 父物体上不能存在碰撞盒
        ParentBoxNumGroup = selectObj.GetComponents<BoxCollider>();
        if (ParentBoxNumGroup.Length != 0)
        {
            WindowTips("父物体上有碰撞盒，请注意查看！！！");
            return;
        }

        var i = 0;
        //获取到所有子物体上的碰撞盒
        var allCollider = selectObj.GetComponentsInChildren<BoxCollider>();
        foreach (var coll in allCollider)
        {
            if (!coll.name.Contains("Align")) i++;
        }

        ChildBoxCollNum = i;
    }


    public void ClearBoxCollNum()
    {
        ChildBoxCollNum = 0;
    }

    #endregion

    #region 测试面板

    /// <summary>
    /// 获取到 Inspector面板
    /// </summary>
    public void TestInspector()
    {
        Type                type    = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
        EditorWindow        window  = EditorWindow.GetWindow(type);
        FieldInfo           info    = type.GetField("m_Tracker", BindingFlags.NonPublic | BindingFlags.Instance);
        ActiveEditorTracker tracker = info.GetValue(window) as ActiveEditorTracker;
        for (int i = 0; i < tracker.activeEditors.Length; i++)
        {
            if (tracker.activeEditors[i].target.GetType().Name == "BoxCollider")
            {
                Debug.Log("这是BoxCollider，在第 " + i + " 个");
            }
        }
    }

    #endregion

    #endregion

    public static SelfCollider Instance()
    {
        return instance ?? (instance = new SelfCollider());
    }
}