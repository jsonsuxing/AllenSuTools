// ========================================================
// 描述：处理碰撞盒相关的操作
// 作者：苏醒
// 创建时间：2019-07-02 11:38:08
// 版 本：1.0
// ========================================================

using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditorInternal;
using Application = UnityEngine.Application;


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
    [UnityEditor.MenuItem("CONTEXT/BoxCollider/一键复制，粘贴当前碰撞盒")]
    static void CopyAndPaste(MenuCommand command)
    {
        // 复制
        ComponentUtility.CopyComponent((BoxCollider) command.context);
        // 粘贴
        ComponentUtility.PasteComponentAsNew(Selection.activeGameObject);
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
    public EditorStatus EditorStatus       = EditorStatus.无效的选择; // 默认编辑状态
    public int          BoxCollNumGenerate = 8;                 // 生成多少个碰撞盒，默认为8个
    public float        OuterRadius        = 0.3f;                 // 碰撞盒外半径
    public float        InnerRadius        = 0.2f;               // 碰撞盒内半径
    public float        height             = 0.5f;                 // 碰撞盒高度
    public float        RotationOffset     = 0f;                 // 旋转角度
    public float        DiffValue          = 0.004f;             // 自动生成的碰撞盒与实际模型尺寸的差值(尺寸完全一致不会吸合)
    public bool         IsLockInnerRadius;                       // 是否禁用内半径，默认关闭
    public bool         IsMatchHeightToWidth;                    // 是否开启高宽度匹配，默认开启
    public GameObject   WorkingCollider      = null;             // 接收生成的环形碰撞盒
    public string choseQuickData = string.Empty; // 当前所选的快捷数据

    // 自定义倾斜环形碰撞盒
    // 旋转
    public int CustomBoxCollNum = 0; // 生成多少个碰撞盒，默认为0个
    // 平移
    public SelfPivotAxis SelfPivotAxis = SelfPivotAxis.X轴正方向; // 选择轴向
    public int CloneObjNum = 0; // 克隆的个数
    public float CloneSpace = 0; // 克隆的间隔

    // 显示模型的长宽高
    public double ModelLength = 0; // 模型长度
    public double ModelWidth  = 0; // 模型宽度
    public double ModelHeight = 0; // 模型高度
    public double AoCaoY      = 0; // 正方体类型颗粒底部凹槽Y轴坐标
    public double TuQiY       = 0; // 正方体类型颗粒凸起下方Y轴坐标
    public double MinY = 0;        // Y轴最小坐标
    public double MaxY = 0;        // Y轴最大坐标
    public double MinX = 0;        // X轴最小坐标
    public double MaxX = 0;        // X轴最大坐标
    public double MinZ = 0;        // Z轴最小坐标
    public double MaxZ = 0;        // Z轴最大坐标

    // 显示有几个碰撞盒
    public int ChildBoxCollNum     = 0; // 所有子物体下有几个碰撞盒

    #endregion

    #region 一：克隆碰撞盒

    #region 一：一键添加长方体类型的碰撞盒(如高一粒)

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
        var cloneObj = Instantiate(prefabObj);
        // 获取“物件_1”上的 Mesh
        var bounds = cloneObj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds;
        // 获得模型原始尺寸的大小,并减去规定的差值
        var sizeX  = bounds.size.x * cloneObj.transform.localScale.x              - DiffValue;
        var sizeY  = bounds.size.y * cloneObj.transform.localScale.y - TuQiHeight - DiffValue;
        var scaleZ = bounds.size.z * cloneObj.transform.localScale.z              - DiffValue;

        // 添加碰撞盒，并设置大小及中心点
        var boxCollider = cloneObj.AddComponent<BoxCollider>();
        boxCollider.size   = new Vector3(sizeX, sizeY, scaleZ);
        boxCollider.center = new Vector3(0, -(TuQiHeight / 2), 0);

        // 替换模型
        PrefabUtility.SaveAsPrefabAsset(cloneObj, prefabPath);
        DestroyImmediate(cloneObj);
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
    /// 环形碰撞盒GUI
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
        GUILayout.Label("二：添加环形类的碰撞盒", style);

        //------------一：开始垂直画盒子------------
        GUILayout.BeginVertical("box");

        // 第一组垂直排版开始
        EditorGUILayout.BeginVertical();
        // 选择轴心
        PivotAxis = (PivotAxis) EditorGUILayout.EnumPopup("轴心", PivotAxis);
        // 设置边数，最小3，最大64
        BoxCollNumGenerate = EditorGUILayout.IntSlider("设置边数", BoxCollNumGenerate, 3, 64);
        // 外半径（从指定外半径的值到无穷大。0.011f,无穷大）
        OuterRadius = Mathf.Max(EditorGUILayout.FloatField("外半径", OuterRadius), 0.011f);
        EditorGUILayout.EndVertical();
        // 第一组垂直排版结束

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

        // 高宽度匹配
        IsMatchHeightToWidth = EditorGUILayout.Toggle(new GUIContent("高宽度自适应", "将高度锁定到外半径减去内半径"), IsMatchHeightToWidth);
        if (IsMatchHeightToWidth) GUI.enabled = false;

        // 碰撞盒高度从0.01到无穷大
        height      = Mathf.Max(EditorGUILayout.FloatField("手动调整碰撞盒高度", height), 0.01f);
        GUI.enabled = true;

        GUILayout.EndVertical();
        //------------一：结束垂直画盒子------------

        // ------------ 一：开始垂直画盒子 ------------
        GUILayout.BeginVertical("box");
    
        GUILayout.Label("快捷设置数据", SetGUIStyle(Color.red, 16));
     
        // 第一组水平排版开始
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("一：轮架"))
        {
            // 轮架数据 length=0.8，limit勾选
            // 小圆洞的 length=0.8-0.18=0.72，limit不勾选
            OuterRadius = 0.24f;
            InnerRadius = 0.16f;
            height      = 0.71f;
            choseQuickData = "0.8长度轮架";
        }
        if (GUILayout.Button("二：小轮架"))
        {
            OuterRadius    = 0.16f;
            InnerRadius    = 0.11f;
            height         = 0.49f;
            choseQuickData = "小轮架";
        }
        EditorGUILayout.EndHorizontal();
        // 第一组水平排版结束

        // 第二组水平排版开始
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("三：厚轮架格挡"))
        {
            OuterRadius    = 0.28f;
            InnerRadius    = 0.01f;
            height         = 0.15f;
            choseQuickData = "厚轮架格挡";
        }
        if (GUILayout.Button("四：薄轮架格挡"))
        {
            OuterRadius    = 0.28f;
            InnerRadius    = 0.01f;
            height         = 0.075f;
            choseQuickData = "薄轮架格挡";
        }
        EditorGUILayout.EndHorizontal();
        // 第二组水平排版结束

        // 第三组水平排版开始
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("五：小圆棍"))
        {
            OuterRadius    = 0.14f;
            InnerRadius    = 0.01f;
            height         = 0.79f;  // 1.58
            choseQuickData = "小圆棍";
        }
        if (GUILayout.Button("六：圆洞"))
        {
            OuterRadius    = 0.35f;
            InnerRadius    = 0.31f;
            height         = 0.79f;
            choseQuickData = "圆洞";

            // 其它匹配数据
            //一：0.29  0.23  二：0.37  0.23
        }

        EditorGUILayout.EndHorizontal();
        // 第三组水平排版结束

        // 第四组水平排版开始
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("七：凸起"))
        {
            // 如果是给凸起加碰撞盒，则是 0.22 0.15 0.165
            OuterRadius    = 0.26f;
            InnerRadius    = 0.15f;
            height         = 0.215f;
            choseQuickData = "凸起";
        }
        EditorGUILayout.EndHorizontal();
        // 第四组水平排版结束

        // 第N组水平排版开始
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("提示：当前所选快捷数据是：", SetGUIStyle(Color.black, 14));
        GUILayout.TextField(choseQuickData);
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
        if (WorkingCollider != null) EditorStatus = EditorStatus.编辑;
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
        var ringHeight = IsMatchHeightToWidth ? (OuterRadius - InnerRadius) : height;

        // 这里调用创建环形碰撞盒的方法
        var compoundCollider = CreateRing(PivotAxis, BoxCollNumGenerate, OuterRadius, InnerRadius, ringHeight, RotationOffset);

        // 移动对撞机，使其位置相对于选定的游戏对象
        compoundCollider.transform.position += selectedGameObject.transform.position;
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
            //segment.transform.localScale= new Vector3(Length, height, width);

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
        if (WorkingCollider != null) DestroyImmediate(WorkingCollider);
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

    #endregion

    #region 三：自定义倾斜环形碰撞盒

    /// <summary>
    /// 旋转
    /// </summary>
    public void RotateBoxCollider()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("所选物体不能为空");
            return;
        }

        var tempParent = new GameObject("拖出子物体，删除该父物体");
        tempParent.transform.SetParent(selectObj.transform.parent);
        for (var i = 0; i < CustomBoxCollNum; i++)
        {
            var cloneObj = Instantiate(selectObj);
            cloneObj.name = "Bevel Box " + "(" + (i + 1) + ")";

            // 克隆的角度
            var cloneAngle = i * (360f / CustomBoxCollNum);
            cloneObj.transform.RotateAround(Vector3.zero, Vector3.up,cloneAngle);
            cloneObj.transform.SetParent(tempParent.transform, true);
        }
        Undo.RegisterCreatedObjectUndo(tempParent,"tempParent");
        DestroyImmediate(selectObj);
    }

    /// <summary>
    /// 平移
    /// </summary>
    public void PanBoxCollider()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("所选物体不能为空");
            return;
        }

        // 记录被克隆物体的x,y,z
        float selectObjX = selectObj.transform.localPosition.x;
        float selectObjY = selectObj.transform.localPosition.y;
        float selectObjZ = selectObj.transform.localPosition.z;

        for (int i = 1; i <= CloneObjNum; i++)
        {
            var cloneObj = Instantiate(selectObj);
            cloneObj.name = "Normal Box " + "(" + i + ")";

            switch (SelfPivotAxis)
            {
                case SelfPivotAxis.X轴正方向:
                    cloneObj.transform.localPosition = new Vector3(selectObjX + i * CloneSpace,selectObjY,selectObjZ);
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
            }

            cloneObj.transform.SetParent(selectObj.transform.parent);
        }
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
                DestroyImmediate(coll);
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

        // 最小Y轴坐标
        var arrayMinY = new float[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            arrayMinY[i] = vertices[i].y;
        }
        MinY = Math.Round(arrayMinY.Min(),4);

        // 最大Y轴坐标
        var arrayMaxY = new float[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            arrayMaxY[i] = vertices[i].y;
        }
        MaxY = Math.Round(arrayMinY.Max(),4);

        // 最小X轴坐标
        var arrayMinX = new float[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            arrayMinX[i] = vertices[i].x;
        }
        MinX = Math.Round(arrayMinX.Min(), 4);

        // 最大X轴坐标
        var arrayMaxX = new float[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            arrayMaxX[i] = vertices[i].x;
        }
        MaxX = Math.Round(arrayMaxX.Max(), 4);

        // 最小Z轴坐标
        var arrayMinZ = new float[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            arrayMinZ[i] = vertices[i].z;
        }
        MinZ = Math.Round(arrayMinZ.Min(), 4);

        // 最大Z轴坐标
        var arrayMaxZ = new float[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            arrayMaxZ[i] = vertices[i].z;
        }
        MaxZ = Math.Round(arrayMaxZ.Max(), 4);

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

    #region 三：从ACE项目移动预设

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

    #region 四：显示有几个碰撞盒

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

    #region N：一键验证颗粒规范性

    /// <summary>
    /// 一键验证颗粒规范性
    /// </summary>
    public void VerifyGranule()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            WindowTips("没有选中模型");
            return;
        }

        var selectTrans = selectObj.GetComponent<Transform>();

        // 模型的初始坐标，角度都为0，尺寸为1
        if (selectTrans.localPosition                       != Vector3.zero) selectTrans.localPosition                              = new Vector3(0, 0, 0);
        if (selectTrans.localRotation                       != Quaternion.identity) selectTrans.localRotation                       = Quaternion.identity;
        if (selectTrans.localScale                          != Vector3.one) selectTrans.localScale                                  = new Vector3(1, 1, 1);
        // if (selectTrans.transform.GetChild(0).localPosition != Vector3.zero) selectTrans.transform.GetChild(0).localPosition        = new Vector3(0, 0, 0);
        // if (selectTrans.transform.GetChild(0).localRotation != Quaternion.identity) selectTrans.transform.GetChild(0).localRotation = Quaternion.identity;
        // if (selectTrans.transform.GetChild(0).localScale    != Vector3.one) selectTrans.transform.GetChild(0).localScale            = new Vector3(1, 1, 1);

        
        for (var i = 0; i < selectObj.transform.childCount; i++)
        {
            var render = selectObj.transform.GetChild(i).GetComponent<MeshRenderer>();
            var filter = selectObj.transform.GetChild(i).GetComponent<MeshFilter>();
            // 移除掉关键部位上已经存在的mesh
            if (!Equals(selectObj.transform.GetChild(i).name, "物件_1"))
            {
                // 如果子物体上有MeshRenderer和MeshFilter组件再移除
                if ( render && filter)
                {
                    DestroyImmediate(render);
                    DestroyImmediate(filter);
                }
            }

            //把关键部位上的初始坐标，角度，尺寸也要为0(除了Box外)
            // if (!selectObj.transform.GetChild(i).gameObject.name.Contains("Box"))
            // {
            //     if (selectObj.transform.GetChild(i).transform.localRotation != Quaternion.identity) selectObj.transform.GetChild(i).transform.localRotation = Quaternion.identity;
            //     if (selectObj.transform.GetChild(i).transform.localScale    != Vector3.one) selectObj.transform.GetChild(i).transform.localScale            = Vector3.one;
            // }
        }

        //这里直接调用显示多少个碰撞盒的方法
        ShowBoxCollNum();
    }

    #endregion

    #endregion


    public static SelfCollider Instance()
    {
        return instance ?? (instance = new SelfCollider());
    }
}