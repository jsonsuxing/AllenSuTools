// 描述：颗粒编辑器
// 作者：苏醒 
// 创建时间：2019-05-17 20:55:51
// 版 本：1.0
// ========================================================

using System;
using System.Windows.Forms;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器GUI控制类
/// </summary>
public class AllenSuTools : EditorWindow
{

    #region MenuItem

    // 1--新建窗口
    [UnityEditor.MenuItem("AllenSu/2、打开苏醒工程窗口", false, CommonFun.INDEXNUM)]
    static void ToolWindows()
    {
        var allenSuTools = GetWindow<AllenSuTools>(false, "苏醒工程窗口");
        allenSuTools.Show();
        var colliderManager = GetWindow<ColliderManager>(false, "碰撞盒专用窗口");
        colliderManager.Show();
    }
    #endregion

    #region 字段声明

    // 编辑器组
    private readonly string[] _topInfoType = { "关键部位","模型相关", "常用工具" , "ToolPro", "一般工具"};
    private          int      whichOneSelect; // 选中哪一个

    private Vector2 scrowPos=Vector2.zero;

    #endregion

    void OnGUI()
    {
        scrowPos = GUILayout.BeginScrollView(scrowPos,GUILayout.Width(position.width),GUILayout.Height(position.height));

        EditorGUI.BeginChangeCheck();

        // 设置标题语（用途:错行）
        EditorGUILayout.HelpBox("木叶飞舞之处，火亦生生不息", MessageType.Info, true);
        whichOneSelect = GUI.Toolbar(new Rect(5, 5, _topInfoType.Length * 70, 37), whichOneSelect, _topInfoType);
        
        switch (whichOneSelect)
        {
            case 0:
                #region 关键部位
               
                #region 一：克隆关键部位

                GUILayout.Label("一：克隆关键部位", TitleStyle());
                GUILayout.Space(3);

                // 默认间隔
                SelfBuWei.Instance().ModelSpace = SelfBuWei.Instance().GeneralSpace;
                GUILayout.Label("A：选择关键部位间隔", SetGuiStyle(Color.gray, 14));

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                // 指定间隔的值
                if (GUILayout.Button("1：正常颗粒间隔值=0.8")) SelfBuWei.Instance().IsClickChangeSpace = false;
                else if (GUILayout.Button("2：中颗粒间隔值=1.6")) SelfBuWei.Instance().IsClickChangeSpace = true;
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------


                // ------------ 二：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("提示：当前关键部位间隔的值是", SetGuiStyle(Color.black, 14));
                SelfBuWei.Instance().ModelSpace = SelfBuWei.Instance().IsClickChangeSpace ? SelfBuWei.Instance().MediumSpace : SelfBuWei.Instance().GeneralSpace;
                GUILayout.TextField(SelfBuWei.Instance().ModelSpace+"");
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                // ------------ 二：结束垂直画盒子 ------------

                GUILayout.Label("B：选择克隆关键部位方式", SetGuiStyle(Color.gray, 14));

                // ------------ 三：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                // 指定间隔的值
                if (GUILayout.Button("1：单行克隆"))
                {
                    SelfBuWei.Instance().IsMoreLine = false;
                    SelfBuWei.Instance().ShowCloneWay = "单行克隆";
                }
                else if (GUILayout.Button("2：多行克隆"))
                {
                    SelfBuWei.Instance().IsMoreLine = true;
                    SelfBuWei.Instance().ShowCloneWay = "多行克隆";
                }
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                // ------------ 三：结束垂直画盒子 ------------

                // ------------ 四：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("提示：当前的克隆方式为", SetGuiStyle(Color.black, 14));
                GUILayout.TextField(SelfBuWei.Instance().ShowCloneWay);
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                // ------------ 四：结束垂直画盒子 ------------

                // ------------ 五：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                GUILayout.Label("沿着坐标轴 正 / 负 方向克隆多少个(不含自身)", SetGuiStyle(Color.red, 14));
                GUILayout.Space(5);

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("x 轴正方向", SetGuiStyle(Color.black, 14));
                SelfBuWei.Instance().CloneNumByAxisX = int.Parse(GUILayout.TextField(SelfBuWei.Instance().CloneNumByAxisX.ToString()));
                GUILayout.Space(50);
                GUILayout.Label("z 轴正方向", SetGuiStyle(Color.black, 14));
                SelfBuWei.Instance().CloneNumByAxisZ = int.Parse(GUILayout.TextField(SelfBuWei.Instance().CloneNumByAxisZ.ToString()));
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(5);

                // 第二组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("x 轴负方向", SetGuiStyle(Color.black, 14));
                SelfBuWei.Instance().CloneNumByAxisXFu = int.Parse(GUILayout.TextField(SelfBuWei.Instance().CloneNumByAxisXFu.ToString()));
                GUILayout.Space(50);
                GUILayout.Label("z 轴负方向", SetGuiStyle(Color.black, 14));
                SelfBuWei.Instance().CloneNumByAxisZFu = int.Parse(GUILayout.TextField(SelfBuWei.Instance().CloneNumByAxisZFu.ToString()));
                EditorGUILayout.EndHorizontal();
                // 第二组水平排版结束
                GUILayout.Space(3);

                GUILayout.EndVertical();
                // ------------ 五：结束垂直画盒子 ------------

                if (GUILayout.Button("开始克隆关键部位")) SelfBuWei.Instance().CloneBuWeiOneByOne();

                // ------------ 六：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("注：可以手动改变关键部位名称下标的值", SetGuiStyle(Color.black, 14));
                CommonFun.CurrentIndexNum = int.Parse(GUILayout.TextField(CommonFun.CurrentIndexNum.ToString()));
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                //------------ 六：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8); //设置上下间隔

                #region 二：特殊部位坐标的计算(+-0.4类型)

                GUILayout.Label("二：特殊部位坐标的计算", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");
                //第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("x轴"))
                {
                    SelfBuWei.Instance().IsClickX = true;
                    SelfBuWei.Instance().IsClickZ = false;
                    SelfBuWei.Instance().ShowClickAxis = "x轴";
                }
                else if (GUILayout.Button("z轴"))
                {
                    SelfBuWei.Instance().IsClickZ = true;
                    SelfBuWei.Instance().IsClickX = false;
                    SelfBuWei.Instance().ShowClickAxis = "z轴";
                }

                if (GUILayout.Button("+0.4"))
                {
                    SelfBuWei.Instance().AddValueNum = true;
                    SelfBuWei.Instance().ReduceValueNum = false;
                    SelfBuWei.Instance().ShowValueNum = "+0.4";
                }
                else if (GUILayout.Button("-0.4"))
                {
                    SelfBuWei.Instance().ReduceValueNum = true;
                    SelfBuWei.Instance().AddValueNum = false;
                    SelfBuWei.Instance().ShowValueNum = "-0.4";
                }
                if (GUILayout.Button("都减0.4")) SelfBuWei.Instance().SubtractAllValue();
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(5);

                // 第二组垂直排版开始
                EditorGUILayout.BeginVertical();
                GUILayout.TextField("当前操作方式：" + SelfBuWei.Instance().ShowClickAxis + "    " + SelfBuWei.Instance().ShowValueNum);
                EditorGUILayout.EndVertical();
                // 第二组垂直排版结束
                GUILayout.Space(3);

                GUILayout.EndVertical();
                //------------ 一：结束垂直画盒子 ------------

                if (GUILayout.Button("点击设定新坐标")) SelfBuWei.Instance().SpecialAxis();

                #endregion
                GUILayout.Space(8);

                #region 三：对称克隆关键部位

                GUILayout.Label("三：对称克隆关键部位", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                //第一组垂直排版开始
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("对称克隆关键部位")) SelfBuWei.Instance().SymmetryCloneBuWei();
                EditorGUILayout.EndVertical();
                // 第一组垂直排版结束

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 四：生成默认关键部位、Box

                GUILayout.Label("四：生成空物体、Normal Box", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("1：GameObject")) SelfBuWei.Instance().CreateDefaultObj();
                if (GUILayout.Button("2：Normal Box")) SelfBuWei.Instance().CreateDefaultNormalBox();
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #endregion
                break;
            case 1:
                #region 模型相关
               
                #region 前提

                #region 前提一：选择颗粒大类

                GUILayout.Label("前提一：选择颗粒大类", TitleStyle());
                // 第一组垂直排版开始
                EditorGUILayout.BeginVertical();

                // 设置首写字母按钮
                SelfModel.Instance().IsOpenGranuleToggle = EditorGUILayout.Toggle("点击选择颗粒种类 → → → ", SelfModel.Instance().IsOpenGranuleToggle);
                if (SelfModel.Instance().IsOpenGranuleToggle)
                {
                    // 每次点击该 Toggle 按钮时都让ShowGranuleType为空
                    CommonFun.ShowGranuleType = string.Empty;
                    SelfModel.Instance().SetTypeBtn();
                }
                // 是否需要记住颗粒路径
                SelfModel.Instance().IsRememberGranuleType = EditorGUILayout.Toggle("记住颗粒大类", SelfModel.Instance().IsRememberGranuleType);
                if (SelfModel.Instance().IsRememberGranuleType)
                {
                    // 记住颗粒大类前确保名字不为空
                    if (Equals(CommonFun.ShowGranuleType, string.Empty))
                    {
                        WindowTips("当前颗粒大类为空，无需记住！！！");
                        SelfModel.Instance().IsRememberGranuleType = false;
                        return;
                    }
                    // 存储颗粒大类名称
                    PlayerPrefs.SetString("颗粒大类名称", CommonFun.ShowGranuleType);
                }

                EditorGUILayout.EndVertical();
                // 第一组垂直排版结束

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第二组垂直排版开始
                EditorGUILayout.BeginVertical();
                if (SelfModel.Instance().ResultList.Count > 1)
                {
                    // 第三组水平排版开始
                    EditorGUILayout.BeginHorizontal();
                    SelfModel.Instance().SetGranuleName();
                    EditorGUILayout.EndHorizontal();
                    // 第三组水平排版结束
                }
                // 颗粒大类有值就清除数据
                if (CommonFun.ShowGranuleType != string.Empty)
                {
                    SelfModel.Instance().ClearGranuleData();
                    SelfModel.Instance().IsOpenGranuleToggle = false;
                }
                GUILayout.Label("1、当前颗粒类别", SetGuiStyle(Color.black, 14));
                CommonFun.ShowGranuleType = PlayerPrefs.HasKey("颗粒大类名称") ? GUILayout.TextField(PlayerPrefs.GetString("颗粒大类名称")) : GUILayout.TextField(CommonFun.ShowGranuleType);
                EditorGUILayout.EndVertical();
                // 第二组垂直排版结束

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 前提二：指定模型外部路径

                GUILayout.Label("前提二：指定模型外部路径", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组垂直排版开始
                EditorGUILayout.BeginVertical();
                GUILayout.Label("1、外部模型路径", SetGuiStyle(Color.black, 14));
                // SelfModel.Instance().IsRememberOutModelPath = EditorGUILayout.Toggle("记住模型路径", SelfModel.Instance().IsRememberOutModelPath);
                
                SelfModel.Instance().ModelFolderPath = PlayerPrefs.HasKey("外部模型路径") ? 
                    GUILayout.TextField (PlayerPrefs.GetString("外部模型路径")): GUILayout.TextField(SelfModel.Instance().ModelFolderPath);

                // 第二组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("1：记住模型路径"))
                {
                    SelfModel.Instance().IsRememberOutModelPath = true;
                    // 记住模型路径前确保路径不为空
                    if (Equals(SelfModel.Instance().ModelFolderPath, string.Empty))
                    {
                        WindowTips("当前路径为空，无需记住！！！");
                        SelfModel.Instance().IsRememberOutModelPath = false;
                        return;
                    }
                    // 存储颗粒大类名称
                    PlayerPrefs.SetString("外部模型路径", SelfModel.Instance().ModelFolderPath);
                }
                if (GUILayout.Button("2：取消记住路径"))
                {
                    if (PlayerPrefs.HasKey("外部模型路径")) PlayerPrefs.DeleteKey("外部模型路径");
                }
                EditorGUILayout.EndHorizontal();
                // 第二组水平排版结束

                EditorGUILayout.EndVertical();
                // 第一组垂直排版结束

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 前提三：给一个默认的操作方式

                // 给一个默认的操作方式
                SelfModel.Instance().ShowWay = "新上架颗粒";
                GUILayout.Label("前提三：选择操作方式", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                // 重新指定操作方式
                if (GUILayout.Button("一：新上架颗粒")) SelfModel.Instance().isAddNewGranuleWay = true;
                else if (GUILayout.Button("二：更换旧模型")) SelfModel.Instance().isAddNewGranuleWay = false;
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion

                #endregion
                GUILayout.Space(8);

                #region 根据操作方式，绘制不同的GUI

                if (SelfModel.Instance().isAddNewGranuleWay)
                {
                    DrawAddNewGranuleWayGui();
                }
                else
                {
                    DrawReplaceOldGranuleWayGui();
                }

                #endregion
                GUILayout.Space(8);

                #region 省事功能

                GUILayout.Label("省事功能", TitleStyle());
            
                #region 3：批量检查指定功能

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");
                if (GUILayout.Button("三：批量检查指 物件_1")) SelfModel.Instance().CheckFunction();
                // ------------ 一：结束垂直画盒子 ------------
                GUILayout.EndVertical();

                #endregion

                #endregion

                #endregion
                break;
            case 2:
                #region 常用工具

                #region 一：3 个顶点确定圆心

                GUILayout.Label("一：三个顶点确定圆心", TitleStyle());
                GUILayout.Space(3);

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("提示信息", SetGuiStyle(Color.black, 14));
                GUILayout.TextField(SelfCollider.Instance().WhichTimeTips);
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(3);

                if (GUILayout.Button("点击 3 次按钮，获取圆心")) SelfCollider.Instance().CreateCenterOfCircle();

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 二:计算 Length 的值

                GUILayout.Label("二：计算长度的值", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                GUILayout.Label("1、获取当前哪个轴的坐标值", SetGuiStyle(Color.black, 14));
                SelfTools.Instance().SetAxis = GUILayout.TextField(SelfTools.Instance().SetAxis);

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("x")) SelfTools.Instance().SetAxis = "x";
                else if (GUILayout.Button("y")) SelfTools.Instance().SetAxis = "y";
                else if (GUILayout.Button("z")) SelfTools.Instance().SetAxis = "z";
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                // 第二组垂直排版开始
                EditorGUILayout.BeginVertical();
                GUILayout.Label("2、第一个坐标值是", SetGuiStyle(Color.black, 14));
                GUILayout.TextField(SelfTools.Instance().FirstValue + "");
                EditorGUILayout.EndVertical();
                // 第二组垂直排版结束

                // 第三组垂直排版开始
                EditorGUILayout.BeginVertical();
                GUILayout.Label("3、长度的值等于", SetGuiStyle(Color.black, 14));
                GUILayout.TextField(SelfTools.Instance().Length + "");
                EditorGUILayout.EndVertical();
                // 第三组垂直排版结束

                // 第四组垂直排版开始
                EditorGUILayout.BeginVertical();
                GUILayout.Label("4、中点的值等于", SetGuiStyle(Color.black, 14));
                GUILayout.TextField(SelfTools.Instance().MidPoint + "");
                EditorGUILayout.EndVertical();
                // 第四组垂直排版结束

                // ------------ 一：结束垂直画盒子 ------------
                GUILayout.EndVertical();

                // 第三组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("1：点击两次求长度及中点的值")) SelfTools.Instance().GetLength();
                if (GUILayout.Button("2：清空按钮"))
                {
                    SelfTools.Instance().ClearValue();
                    // 单独重置长度和中点值，如果直接写到ClearValue函数里，在编辑器里回看不到长度，因为已经被重置
                    SelfTools.Instance().Length = 0; 
                    SelfTools.Instance().MidPoint = 0;
                }
                EditorGUILayout.EndHorizontal();
                // 第三组水平排版结束

                #endregion
                GUILayout.Space(8);

                #region 三：带角度模型的方向

                GUILayout.Label("三：带角度模型的方向", TitleStyle());
                GUILayout.Space(3);

                #region 1：角度

                GUILayout.Label("A：角度", SetGuiStyle(Color.red, 16));

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("提示信息", SetGuiStyle(Color.black, 14));
                GUILayout.TextField(SelfCollider.Instance().StrAngleTips);
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(3);

                if (GUILayout.Button("点击两次按钮，计算单位向量")) SelfCollider.Instance().AngleModel();

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #endregion
                GUILayout.Space(8);

                #endregion
                break;
            case 3:
                #region ToolPro

                #region 一：修改完旧模型的后续操作

                GUILayout.Label("一：修改完旧模型的后续操作", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                #region 一：复制名称到指定 txt 文件

                GUILayout.Label("1：复制颗粒名称到 txt 文件(修改一个点一次)", SetGuiStyle(Color.red, 14));
                GUILayout.Space(3);

                // ------------ 二：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("1、点击颗粒预设，添加至 txt")) ToolPro.Instance().CopyNameToTxt();
                if (GUILayout.Button("2、打开 txt")) ToolPro.Instance().OpenTxt();
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                // ------------ 二：结束垂直画盒子 ------------

                GUILayout.Space(3);

                #endregion
                GUILayout.Space(2);

                #region 二：复制fbx到指定文件夹

                GUILayout.Label("2：复制fbx到文件夹，并取消待定颗粒标识(只点一次)", SetGuiStyle(Color.red, 14));

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                GUILayout.Label("指定外部文件夹路径", SetGuiStyle(Color.black, 14));
                ToolPro.Instance().OutFbxPath = GUILayout.TextField(ToolPro.Instance().OutFbxPath);

                // 第一组垂排版开始
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("点击按钮")) ToolPro.Instance().CopyFbxToDirectory();
                EditorGUILayout.EndVertical();
                // 第一组垂直排版结束
                GUILayout.Space(3);

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(2);

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 二：从 Hierarchy 批量修改颗粒预设

                GUILayout.Label("二：批量处理颗粒预设", TitleStyle());
                GUILayout.Space(3);

                #region A：选择处理方式

                GUILayout.Label("A：选择处理方式", SetGuiStyle(Color.red, 14));
                GUILayout.Space(3);

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("1：Hierarchy")) ToolPro.Instance().IsHierarchy = true;
                if (GUILayout.Button("2：选择颗粒预设")) ToolPro.Instance().IsHierarchy    = false;
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                // 第二组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("提示：当前修改方式为：", SetGuiStyle(Color.black, 14));
                ToolPro.Instance().strTips = ToolPro.Instance().IsHierarchy ? "Hierarchy" : "选择颗粒预设";
                GUILayout.TextField(ToolPro.Instance().strTips);
                EditorGUILayout.EndHorizontal();
                // 第二组水平排版结束
                GUILayout.Space(3);

                #endregion

                #region B：自定义检查面板

                // 第零组水平排版开始
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("B：检查面板", SetGuiStyle(Color.red, 14));
                if(GUILayout.Button("一键 " + (ToolPro.Instance().IsOpenAll ? "关闭" : "开启") + " 检查面板")) ToolPro.Instance().OpenAndCloseAll();
                EditorGUILayout.EndHorizontal();
                // 第零组水平排版结束
                GUILayout.Space(3);

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                ToolPro.Instance().IsCheckBlockPrefab = 
                    EditorGUILayout.Toggle(new GUIContent("1：检查《颗粒预设》", "是否检查颗粒预设的 1：位置、旋转。2：移除 KeLiData 和 GranuleModel 脚本和刚体。"), 
                        ToolPro.Instance().IsCheckBlockPrefab);
                ToolPro.Instance().IsCheckWu =
                    EditorGUILayout.Toggle(new GUIContent("2：检查《物件_1》",
                            "是否检查物件的 1：位置、旋转、比例、名称"), ToolPro.Instance().IsCheckWu);
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(3);

                // 第二组水平排版开始
                EditorGUILayout.BeginHorizontal();
                ToolPro.Instance().IsCheckBuWei = EditorGUILayout.Toggle(new GUIContent("3：检查《关键部位》",
                        "是否检查关键部位的 1：比例。2：移除 MeshRenderer 和 MeshFilter 组件。"),
                    ToolPro.Instance().IsCheckBuWei);
                ToolPro.Instance().IsCheckBoxCollider = EditorGUILayout.Toggle(new GUIContent("4：检查《碰撞盒》",
                    "是否检查碰撞盒的 1：Center 转化"), ToolPro.Instance().IsCheckBoxCollider);
                EditorGUILayout.EndHorizontal();
                // 第二组水平排版结束
                GUILayout.Space(3);

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------
                #endregion

                #region C：一次性功能区

                GUILayout.Label("C：一次性功能区", SetGuiStyle(Color.red, 14));
                GUILayout.Space(3);

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                ToolPro.Instance().IsRenameBoxCollider = EditorGUILayout.Toggle(new GUIContent("1：碰撞盒大改版",
                    "将父物体上的碰撞盒移动到子物体，普通碰撞盒改成 Normal Box，倾斜碰撞盒改成 Bevel Box，该项属于实现功能范围，" +
                    "只用一次即可"), ToolPro.Instance().IsRenameBoxCollider);
                ToolPro.Instance().IsAddSoAoCao = EditorGUILayout.Toggle(new GUIContent("2：生成 SonAoCao ",
                    "将含有小插销的颗粒，在同位置克隆一个 SonAoCao"), ToolPro.Instance().IsAddSoAoCao);
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(3);

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");
                if (GUILayout.Button("开始")) ToolPro.Instance().CheckGranule();
                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------
                GUILayout.Space(8);

                #endregion
                GUILayout.Space(8);

                #region 三：批量处理 Hierarchy 上的颗粒大类

                GUILayout.Label("三：批量处理 Hierarchy 上的颗粒大类", TitleStyle());
                GUILayout.Space(3);

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                GUILayout.Label("A：选择预设", SetGuiStyle(Color.red, 14));
                GUILayout.Space(3);
                ToolPro.Instance().PrefabObj = (GameObject)EditorGUILayout.ObjectField("点击选择预设：", ToolPro.Instance().PrefabObj, typeof(GameObject), true);
                GUILayout.Space(3);

                GUILayout.Label("B：待生成子物体面板", SetGuiStyle(Color.red, 14));
                GUILayout.Space(3);

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                ToolPro.Instance().IsCreateSingleObj =
                    EditorGUILayout.Toggle(new GUIContent("1：添加单个子物体",
                        "在某一颗粒大类下直接添加一个子物体"), ToolPro.Instance().IsCreateSingleObj);
                ToolPro.Instance().IsCreateBorder =
                    EditorGUILayout.Toggle(new GUIContent("2：添加 《Border》",
                        "在某一颗粒大类下添加一个 Border 子物体，负责鼠标经过移出时的边框显示"), ToolPro.Instance().IsCreateBorder);
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(3);

                // 第二组水平排版开始
                EditorGUILayout.BeginHorizontal();
                ToolPro.Instance().IsCreateMain =
                    EditorGUILayout.Toggle(new GUIContent("3：添加 《Main》",
                        "在某一颗粒大类下添加一个 Main 子物体，负责鼠标经过移出时颗粒大类 UI 的放大与缩小"), ToolPro.Instance().IsCreateMain);
                EditorGUILayout.EndHorizontal();
                // 第二组水平排版结束
                GUILayout.Space(3);

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------
               
                if (GUILayout.Button("点击批量处理颗粒大类")) ToolPro.Instance().ChangeGranuleImage();
                
                #endregion
                GUILayout.Space(8);

                #endregion
                break;
            case 4:
                #region 一般工具

                #region 一:批量修改“-”为“&”

                GUILayout.Label("一：批量修改“-”为“&”", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组垂直排版开始
                EditorGUILayout.BeginVertical();
                GUILayout.Label("1、要修改的文件路径", SetGuiStyle(Color.black, 14));
                SelfTools.Instance().ChangeFileNamePath = GUILayout.TextField(SelfTools.Instance().ChangeFileNamePath);
                EditorGUILayout.EndVertical();
                // 第一组垂直排版结束

                if (GUILayout.Button("点击批量修改“-”为“&”")) SelfTools.Instance().ChangeFileName();

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 二：添加，删除 MeshRenderer,MeshFilter

                GUILayout.Label("二：添加，移除紫色 Mesh", TitleStyle());

                // ------------ 一：开始水平画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("添加紫色 Mesh")) SelfTools.Instance().AddMesh();
                if (GUILayout.Button("移除紫色 Mesh")) SelfTools.Instance().RemoveMesh();
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                // ------------ 一：结束水平画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 三：删除指定后缀名的文件

                GUILayout.Label("三：删除指定后缀名的文件", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组垂直排版开始
                EditorGUILayout.BeginVertical();
                GUILayout.Label("1、要修改的文件路径", SetGuiStyle(Color.black, 14));
                SelfTools.Instance().DeleteFilePath = GUILayout.TextField(SelfTools.Instance().DeleteFilePath);
                GUILayout.Label("2、选择要删除的文件后缀名", SetGuiStyle(Color.black, 14));

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(".meta")) SelfTools.Instance().SelectExtension = ".meta";
                if (GUILayout.Button(".fbx")) SelfTools.Instance().SelectExtension = ".fbx";
                if (GUILayout.Button(".jpg")) SelfTools.Instance().SelectExtension = ".jpg";
                if (GUILayout.Button(".png")) SelfTools.Instance().SelectExtension = ".png";
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                SelfTools.Instance().SelectExtension = GUILayout.TextField(SelfTools.Instance().SelectExtension);

                if (GUILayout.Button("点击删除指定后缀名的文件")) SelfTools.Instance().DeleteSelectExtension();

                EditorGUILayout.EndVertical();
                // 第一组垂直排版结束

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 四：从ACE项目移动预设

                GUILayout.Label("四：导出ACE预设", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("1：从ACE项目移动预设")) SelfCollider.Instance().MovePrefabFromAce();
                if (GUILayout.Button("2：删除QD下的预设")) SelfCollider.Instance().DeletePrefab();
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #region 五：复制模型全称、半称

                GUILayout.Label("五：复制模型名全称、半称", TitleStyle());

                // ------------ 一：开始垂直画盒子 ------------
                GUILayout.BeginVertical("box");

                // 第一组水平排版开始
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button("1：全称")) Clipboard.SetText(Selection.activeGameObject.name);
                if (GUILayout.Button("2：半称"))
                {
                    var str  = Selection.activeGameObject.name;
                    var flag = '&';
                    // 获取到 & 后面的内容
                    var text = str.Substring(str.IndexOf(flag) + 1);
                    Clipboard.SetText(text);
                }
                EditorGUILayout.EndHorizontal();
                // 第一组水平排版结束
                GUILayout.Space(3);

                GUILayout.EndVertical();
                // ------------ 一：结束垂直画盒子 ------------

                #endregion
                GUILayout.Space(8);

                #endregion
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        GUILayout.EndScrollView();
    }


    #region 其它函数

    #region 一：绘制 新上架颗粒 或者 更换旧模型

    /// <summary>
    /// 绘制新上架颗粒的 GUI
    /// </summary>
    public void DrawAddNewGranuleWayGui()
    {
        GUILayout.Label("             新上架颗粒", SetGuiStyle(Color.red, 23));
        GUILayout.Space(8);

        #region 一：点击导入新模型

        GUILayout.Label("步骤1：批量导入新模型", TitleStyle());

        // ------------ 一：开始垂直画盒子 ------------
        GUILayout.BeginVertical("box");

        // 第一组垂直排版开始
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("点击批量导入新模型")) SelfModel.Instance().ImportNewModel();
        EditorGUILayout.EndVertical();
        // 第一组垂直排版结束

        GUILayout.EndVertical();
        // ------------ 一：结束垂直画盒子 ------------

        #endregion
        GUILayout.Space(8);

        #region 二：添加 CreateGuanJians脚本

        GUILayout.Label("步骤2：批量挂载脚本", TitleStyle());

        // ------------ 一：开始垂直画盒子 ------------
        GUILayout.BeginVertical("box");
        if (GUILayout.Button("点击添加关键部位脚本")) SelfModel.Instance().AddScript();
        GUILayout.EndVertical();
        // ------------ 一：结束垂直画盒子 ------------

        #endregion
        GUILayout.Space(8);

        #region 三：上架新颗粒

        GUILayout.Label("步骤3：批量上架新颗粒", TitleStyle());

        // ------------ 一：开始垂直画盒子 ------------
        GUILayout.BeginVertical("box");

        // 第一组垂直排版开始
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("点击批量上架新颗粒")) SelfModel.Instance().ShelfNewGranule();
        EditorGUILayout.EndVertical();
        // 第一组垂直排版结束

        GUILayout.EndVertical();
        // ------------ 一：结束垂直画盒子 ------------

        #endregion
        GUILayout.Space(8);
    }


    /// <summary>
    /// 绘制更换旧模型的 GUI
    /// </summary>
    public void DrawReplaceOldGranuleWayGui()
    {
        GUILayout.Label("             更换旧模型", SetGuiStyle(Color.red, 23));
        GUILayout.Space(8);

        #region 一：导入单个模型

        GUILayout.Label("步骤1：导入单个模型", TitleStyle());

        // ------------ 一：开始垂直画盒子 ------------
        GUILayout.BeginVertical("box");

        // 第一组垂直排版开始
        EditorGUILayout.BeginVertical();
        GUILayout.Label("请输入导入颗粒名字", SetGuiStyle(Color.black, 14));
        GUILayout.Space(5);
        SelfModel.Instance().ImportGranuleName = GUILayout.TextField(SelfModel.Instance().ImportGranuleName);
        if (GUILayout.Button("点击导入单个模型")) SelfModel.Instance().ImportNewModel();
        EditorGUILayout.EndVertical();
        // 第一组垂直排版结束

        GUILayout.EndVertical();
        // ------------ 一：结束垂直画盒子 ------------

        #endregion
        GUILayout.Space(8);

        #region 二：更换新材质

        GUILayout.Label("步骤2：更换新材质", TitleStyle());

        // ------------ 一：开始垂直画盒子 ------------
        GUILayout.BeginVertical("box");

        // 第一组垂直排版开始
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("选中物件_1，点击更换材质"))
        {
            SelfModel.Instance().ReplaceMesh();
        }
        EditorGUILayout.EndVertical();
        // 第一组垂直排版结束

        GUILayout.EndVertical();
        // ------------ 一：结束垂直画盒子 ------------

        #endregion
        GUILayout.Space(8);
    }

    #endregion

    /// <summary>
    /// 弹出提示窗口
    /// </summary>
    /// <param name="message">提示内容</param>
    public static void WindowTips(string message)
    {
        EditorUtility.DisplayDialog("提示窗口", message, "确定");
    }

    #endregion

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

    #endregion

}