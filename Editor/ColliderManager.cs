// ========================================================
// 描述：
// 作者：苏醒 
// 创建时间：2019-11-14 16:52:36
// 版 本：1.0
// ========================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColliderManager:EditorWindow
{
    #region 字段声明

    // 编辑器组
    private readonly string[] _topInfoType = {"克隆 A 类碰撞盒", "克隆 B 类碰撞盒", "碰撞盒信息" };
    private          int      whichOneSelect; // 选中哪一个
    private Vector2 scrowPos = Vector2.zero;

    #endregion

    void OnGUI()
    {
        scrowPos = GUILayout.BeginScrollView(scrowPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
        EditorGUI.BeginChangeCheck();

        // 设置标题语（用途:错行）
        EditorGUILayout.HelpBox("", MessageType.Info, true);

        whichOneSelect = GUI.Toolbar(new Rect(5, 5, _topInfoType.Length * 100, 37), whichOneSelect, _topInfoType);

        switch (whichOneSelect)
        {
            case 0:
                #region 克隆 A 类碰撞盒

                #region 一：添加环形类的碰撞盒

                SelfCollider.Instance().RingBoxCollGuiControl();
                if (EditorGUI.EndChangeCheck() && SelfCollider.Instance().EditorStatus == EditorStatus.编辑) // <-- 控制检查结束在这里
                {
                    SelfCollider.Instance().UpdateRingBoxColl();
                }

                SelfCollider.Instance().DrawStatusText();
                if (SelfCollider.Instance().EditorStatus == EditorStatus.编辑)
                {
                    // 第一组水平排版开始
                    EditorGUILayout.BeginHorizontal();

                    //完成创建碰撞盒
                    if (GUILayout.Button("确认"))
                    {
                        // 清除工作对撞机参考，结束编辑状态
                        SelfCollider.Instance().WorkingCollider = null;
                    }

                    //取消
                    if (GUILayout.Button("取消"))
                    {
                        // //删除工作对撞机，取消其创建
                        DestroyImmediate(SelfCollider.Instance().WorkingCollider);

                        // 清除工作对撞机参考，结束编辑状态
                        SelfCollider.Instance().WorkingCollider = null;
                    }

                    EditorGUILayout.EndHorizontal();
                    // 第一组水平排版结束
                }
                else
                {
                    // 第一组水平排版开始
                    EditorGUILayout.BeginHorizontal();
                    if (SelfCollider.Instance().EditorStatus != EditorStatus.准备) GUI.enabled = false;
                    if (GUILayout.Button("1：点击颗粒预设，开始克隆")) SelfCollider.Instance().CreateRingBoxColl();
                    GUI.enabled = true;
                    if (GUILayout.Button("2：整理、拆分碰撞盒")) SelfCollider.Instance().DeleteAndArrangeRing();
                    EditorGUILayout.EndHorizontal();
                    // 第一组水平排版结束
                }

                #endregion
                GUILayout.Space(8);

                #region 二：自定义倾斜环形碰撞盒

                #region 一：旋转

                GUILayout.Label("二：自定义操作碰撞盒", TitleStyle());
                GUILayout.Space(3);
                GUILayout.Label(" A：旋转", SetGuiStyle(Color.red, 16));
                GUILayout.Space(3);
                GUILayout.Label(" 提示：默认沿 Y 轴，同步环形碰撞盒的轴心", SetGuiStyle(Color.black, 14));

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");
                GUILayout.Space(3);

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("1：旋转中心(默认为原点)", SetGuiStyle(Color.black, 14));
                if (GUILayout.Button("可点击物体，自动传值")) SelfCollider.Instance().SetMyPivot();
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(5);

                // 第二组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("x:", SetGuiStyle(Color.black, 14));
                SelfCollider.Instance().SelfPivotAxisX = float.Parse(EditorGUILayout.TextField(SelfCollider.Instance().SelfPivotAxisX.ToString()));
                GUILayout.Label("y:", SetGuiStyle(Color.black, 14));
                SelfCollider.Instance().SelfPivotAxisY = float.Parse(EditorGUILayout.TextField(SelfCollider.Instance().SelfPivotAxisY.ToString()));
                GUILayout.Label("z:", SetGuiStyle(Color.black, 14));
                SelfCollider.Instance().SelfPivotAxisZ = float.Parse(EditorGUILayout.TextField(SelfCollider.Instance().SelfPivotAxisZ.ToString()));
                EditorGUILayout.EndHorizontal();
                // 第二组水平排版结束
                GUILayout.Space(5);

                GUILayout.Label("2：快捷设置克隆个数(含自身，且会删除所选对象)", SetGuiStyle(Color.black, 14));
                GUILayout.Space(5);

                // 第四组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("4")) { SelfCollider.Instance().CustomBoxCollNum = 4; }
                if (GUILayout.Button("8")) { SelfCollider.Instance().CustomBoxCollNum = 8; }
                if (GUILayout.Button("10")) { SelfCollider.Instance().CustomBoxCollNum = 10; }
                if (GUILayout.Button("12")) { SelfCollider.Instance().CustomBoxCollNum = 12; }
                if (GUILayout.Button("16")) { SelfCollider.Instance().CustomBoxCollNum = 16; }
                if (GUILayout.Button("x2")) { SelfCollider.Instance().CustomBoxCollNum = SelfCollider.Instance().CustomBoxCollNum * 2; }
                EditorGUILayout.EndHorizontal();
                // 第四组水平排版结束
                GUILayout.Space(5);

                // 第三组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("3:选中已做好物体，设置克隆个数：", SetGuiStyle(Color.black, 14));
                SelfCollider.Instance().CustomBoxCollNum = int.Parse(GUILayout.TextField(SelfCollider.Instance().CustomBoxCollNum.ToString()));
                EditorGUILayout.EndHorizontal();
                // 第三组水平排版结束
                GUILayout.Space(3);

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                if (GUILayout.Button("开始克隆")) SelfCollider.Instance().RotateBoxCollider();


                #endregion
                GUILayout.Space(3);

                #region 二：平移

                GUILayout.Label(" B：平移", SetGuiStyle(Color.red, 16));

                // ------------ 二：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组垂直排版开始
                EditorGUILayout.BeginVertical();
                SelfCollider.Instance().SelfPivotAxis = (SelfPivotAxis)EditorGUILayout.EnumPopup("选择克隆方向", SelfCollider.Instance().SelfPivotAxis);
                EditorGUILayout.EndVertical();
                // 第一组垂直排版结束
                GUILayout.Space(3);

                SelfCollider.Instance().CustomBoxCollNum = EditorGUILayout.IntSlider("设置克隆个数", SelfCollider.Instance().CustomBoxCollNum, 1, 64);
                GUILayout.Space(3);

                // 第二组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(" 设置克隆间隔", SetGuiStyle(Color.black, 11));
                SelfCollider.Instance().CloneSpace = float.Parse(EditorGUILayout.TextField(SelfCollider.Instance().CloneSpace.ToString()));
                EditorGUILayout.EndHorizontal();
                // 第二组水平排版结束
                GUILayout.Space(3);

                if (GUILayout.Button("开始克隆")) SelfCollider.Instance().PanBoxCollider();

                GUILayout.EndVertical();
                // ------------ 二：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #endregion
                GUILayout.Space(8);

                #endregion
                break;
            case 1:
                #region 克隆 B 类碰撞盒

                #region 一：长方体类模型，通过一对角线的两个顶点，确定碰撞盒

                GUILayout.Label("一：对角线两顶点确定碰撞盒", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组垂直排版开始
                EditorGUILayout.BeginVertical();
                GUILayout.Label("当前已确定 " + (SelfCollider.Instance().VertexList == null ? 0 : SelfCollider.Instance().VertexList.Count) + " 个顶点", SetGuiStyle(Color.black, 14));
                if (GUILayout.Button("获取对角线两顶点，确定碰撞盒")) SelfCollider.Instance().VertexBox();
                EditorGUILayout.EndVertical();
                // 第一组垂直排版结束
                GUILayout.Space(3);

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 二：镜像克隆对象

                GUILayout.Label("二：镜像克隆对象", TitleStyle());
                GUILayout.Space(3);

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");
                GUILayout.Space(3);

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("一：选择对称轴 ",                         SetGuiStyle(Color.red,   14));
                GUILayout.Label(SelfTools.Instance().SetMirrorAxis, SetGuiStyle(Color.black, 14));
                GUILayout.Label("");
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(2);

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("x")) SelfTools.Instance().SetMirrorAxis = "x";
                if (GUILayout.Button("y")) SelfTools.Instance().SetMirrorAxis = "y";
                if (GUILayout.Button("z")) SelfTools.Instance().SetMirrorAxis = "z";
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(3);

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("二：指定对称中心（默认原点）", SetGuiStyle(Color.red, 14));
                if (GUILayout.Button("选中物体设置对称中心")) SelfTools.Instance().MirrorPoint = Selection.activeGameObject.transform.position;
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(3);

                EditorGUILayout.Vector3Field("对称中心", SelfTools.Instance().MirrorPoint);

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                GUILayout.Space(3);

                if (GUILayout.Button("点击按钮，镜像克隆对象")) SelfTools.Instance().MirrorObj();

                #endregion
                GUILayout.Space(8);

                #region 三：一键添加长方体类型的碰撞盒(如高一粒)

                GUILayout.Label("三：正方类碰撞盒", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");
                if (GUILayout.Button("点击克隆碰撞盒")) SelfCollider.Instance().AddBoxCollider();
                GUILayout.EndVertical();
                //------------一：结束垂直画盒子------------

                #endregion
                GUILayout.Space(8);

                #endregion
                break;
            case 2:
                #region 碰撞盒信息

                #region 一：显示模型数据

                GUILayout.Space(3);
                GUILayout.Label("一：显示模型数据", TitleStyle());

                GUILayout.Space(3);
                GUILayout.Label("1：模型基础数据", SetGuiStyle(Color.red, 14));

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // ------------ 第一组水平排版开始 -----------
                EditorGUILayout.BeginHorizontal();
                GUILayout.TextField("长度：" + SelfCollider.Instance().ModelLength);
                GUILayout.TextField("宽度：" + SelfCollider.Instance().ModelWidth);
                GUILayout.TextField("高度：" + SelfCollider.Instance().ModelHeight);
                EditorGUILayout.EndHorizontal();
                // ------------ 第一组水平排版结束 ------------
                GUILayout.Space(3);

                // ------------ 第二组水平排版开始 ------------
                EditorGUILayout.BeginHorizontal();
                GUILayout.TextField("凹槽 Y 轴坐标：" + SelfCollider.Instance().AoCaoY);
                GUILayout.TextField("凸起 Y 轴坐标：" + SelfCollider.Instance().TuQiY);
                EditorGUILayout.EndHorizontal();
                // ------------ 第二组水平排版结束 ------------
                GUILayout.Space(3);

                GUILayout.EndVertical();
                // ------------一：结束垂直画盒子------------

                GUILayout.Label("2：模型最坐标", SetGuiStyle(Color.red, 14));

                // ------------ 二：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.TextField("X轴 最小坐标是：" + SelfCollider.Instance().MinX);
                GUILayout.Space(3);
                GUILayout.TextField("X轴 最大坐标是：" + SelfCollider.Instance().MaxX);
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(3);

                // 第二组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.TextField("Y轴 最小坐标是：" + SelfCollider.Instance().MinY);
                GUILayout.Space(3);
                GUILayout.TextField("Y轴 最大坐标是：" + SelfCollider.Instance().MaxY);
                EditorGUILayout.EndHorizontal();
                // 第二组水平排版结束
                GUILayout.Space(3);

                // 第三组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.TextField("Z轴 最小坐标是：" + SelfCollider.Instance().MinZ);
                GUILayout.Space(3);
                GUILayout.TextField("Z轴 最大坐标是：" + SelfCollider.Instance().MaxZ);
                EditorGUILayout.EndHorizontal();
                // 第三组水平排版结束
                GUILayout.Space(3);

                GUILayout.EndVertical();
                // ------------ 二：结束垂直画盒子 ------------

                GUILayout.Label("3：碰撞盒总个数", SetGuiStyle(Color.red, 14));

                // ------------ 三：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组垂直排版开始
                EditorGUILayout.BeginVertical();
                GUILayout.TextField("所有碰撞盒个数：" + SelfCollider.Instance().ChildBoxCollNum);
                EditorGUILayout.EndVertical();
                // 第一组垂直排版结束

                GUILayout.EndVertical();
                // ------------ 三：结束垂直画盒子 ------------

                // 第四组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("1：点击颗粒名称获取")) SelfCollider.Instance().ShowModelLengthWidthHeight();
                if (GUILayout.Button("2：清空以上所有数据")) SelfCollider.Instance().ClearModelData();
                EditorGUILayout.EndHorizontal();
                // 第四组水平排版结束

                #endregion
                GUILayout.Space(8);

                #region 二：隐藏碰撞盒

                GUILayout.Label("二：隐藏,移除全部碰撞盒", TitleStyle());

                // ------------ 一：开始水平画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                SelfCollider.Instance().HideOrShowTips = !SelfCollider.Instance().IsHideAllBoxColl ? "1：隐藏全部碰撞盒" : "1：显示全部碰撞盒";
                if (GUILayout.Button(SelfCollider.Instance().HideOrShowTips))
                {
                    SelfCollider.Instance().IsClickHideBtn = true;
                    SelfCollider.Instance().CollBtn();
                }
                if (GUILayout.Button("2：移除所有碰撞盒"))
                {
                    SelfCollider.Instance().IsClickRemoveBtn = true;
                    SelfCollider.Instance().CollBtn();
                }
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                //------------ 一：结束水平画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 临时测试

                //if (GUILayout.Button("测试"))
                //{
                //    SelfCollider.Instance().TestInspector();
                //}

                #endregion
                GUILayout.Space(8);

                #endregion

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        GUILayout.EndScrollView();
    }

    #region 设置GUIStyle

    /// <summary>
    /// 标题样式
    /// </summary>
    /// <returns></returns>
    public GUIStyle TitleStyle()
    {
        var style = new GUIStyle
        {
            normal =
            {
                textColor = Color.blue
            },
            fontSize = 20
        };
        return style;
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


    public GUIStyle SetBtnStyle(int fontSize,Color textColor)
    {
        var style=new GUIStyle();
        // style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = fontSize;
        style.normal.textColor = textColor;
        
        // style.normal.background
        return style;
    }

    #endregion
}
