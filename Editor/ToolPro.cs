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
using ChinarX;
using UI.ThreeDimensional;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Directory = System.IO.Directory;


public class ToolPro : CommonFun
{
    private static ToolPro instance;

    #region 字段声明

    // 一：修改完旧模型的后续操作
    public GameObject SelectObj  = null;
    public string     TxtPath    = "D:/所选颗粒名称汇总.txt";                      // 存放记录做的哪些颗粒名称的txt文件路径
    public string     OutFbxPath = string.Empty;                                   // 复制fbx到哪个文件夹
    public string     ModelPath  = Application.dataPath + "/Other/InitialModels/"; // 工程fbx模型路径
    public string[]   StrContent = null;                                           // txt文件内容

    // 二：批量修改颗粒预设
    public int errorPositionNumGranule = 0; // 颗粒位置错误个数
    public int errorRotationNumGranule = 0; // 颗粒旋转位置错误个数
    public int errorPositionNumWu = 0; // 物件位置错误个数
    public int errorRotationNumWu = 0; // 物件旋转位置错误个数
    public int errorScaleNumWu    = 0; // 物件比例错误个数
    public int errorNoWuName = 0; // 名称不是物件_1的个数
    public int errorBuWeiNum = 0; // 关键部位上有碰撞盒的个数
    public int count         = 0; // 进度表示
    public string strTips = string.Empty;  // 提示批量操作的方式
    public bool IsHierarchy = true; // 批量处理预设的方式，默认是从层级面板处理，为 false 代表是直接点击预设处理
    public bool IsOpenAll = false; // 一键开启、关闭，默认全关
    public bool IsCheckBlockPrefab = false; // 是否处理 颗粒预设
    public bool IsCheckWu = false; // 是否处理 物件_1
    public bool IsCheckBoxCollider = false; // 是否处理 碰撞盒
    public bool IsCheckBuWei = false; // 是否 处理 关键部位
    public bool IsRenameBoxCollider = false; // 是否将父物体上的碰撞盒移动到子物体，普通碰撞盒改成 Normal Box，倾斜碰撞盒改成 Bevel Box

    // 三：批量处理 Hierarchy 上的颗粒大类
    public GameObject PrefabObj; // 选择的预设
    public bool IsAddBorder = false;  // 是否添加 Border

    #endregion

    #region 一：修改完旧模型的后续操作

    #region 一：复制名称到指定 txt 文件

    /// <summary>
    /// 复制名称到指定 txt 文件
    /// </summary>
    public void CopyNameToTxt()
    {
        SelectObj = Selection.activeGameObject;
        if (SelectObj == null)
        {
            WindowTips("所选物体不能为空");
            return;
        }
        // 文件已存在时判断加入颗粒名称的重复性
        if (File.Exists(TxtPath))
        {
            StrContent = File.ReadAllLines(TxtPath);
            if (StrContent.Contains(SelectObj.name))
            {
                WindowTips("不能重复添加");
                return;
            }
        }
        else
        {
            // 这里赋值为空，是因为删除txt文件时，StrContent仍然有值，导致其长度出现问题
            StrContent = null;
        }

        // 写入到本地文件
        WriteToTxt("D:/", "所选颗粒名称汇总", SelectObj.name);
        WindowTips(message: "添加成功,当前已加入 " + (StrContent == null ? "1" : (StrContent.Length + 1).ToString()) + " 个颗粒");
    }


    /// <summary>
    /// 打开 txt
    /// </summary>
    public void OpenTxt()
    {
        if (File.Exists(TxtPath))
        {
            // 直接打开文件
            System.Diagnostics.Process.Start(TxtPath);
        }
        else
        {
            WindowTips("不存在 D:/所选颗粒名称汇总.txt 这个文件");
        }
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

    #region 二：批量修改颗粒预设

    /// <summary>
    /// 批量修改颗粒预设
    /// </summary>
    public void CheckGranule()
    {
        // 检查项二：记录物件比例不为 1 的颗粒名称，并且删除关键部位上的 MeshRenderer 和 MeshFilter
        if (Selection.gameObjects.Length == 0)
        {
            WindowTips("至少选择一个物体");
            return;
        }

        // 每次点击时重置标记数字
        errorPositionNumGranule = errorRotationNumGranule = 0;
        errorPositionNumWu = errorRotationNumWu = 0;
        errorScaleNumWu = errorBuWeiNum = count = errorNoWuName = 0;

        foreach (var selectObj in Selection.gameObjects)
        {
            // 检查项一：处理 颗粒预设
            if (IsCheckBlockPrefab) CheckBlockPrefab(selectObj);

            // 检查项二：处理 物件
            if (IsCheckWu) CheckWu(selectObj);

            // 检查项三：处理 碰撞盒
            if (IsCheckBoxCollider) CheckBoxCollider(selectObj);

            // 检查项四：处理 关键部位
            if(IsCheckBuWei) CheckBuWei(selectObj);

            // 特殊检查：将父物体上的碰撞盒移动到子物体，普通碰撞盒改成 Normal Box，倾斜碰撞盒改成 Bevel Box，
            if (IsRenameBoxCollider) ChangeColliderMode(selectObj);

            count++;
            PrefabUtility.SaveAsPrefabAsset(selectObj, "Assets/Resources/Prefab/ModelPrefabs/" + selectObj.name+ ".prefab");
            if(IsHierarchy) Object.DestroyImmediate(selectObj);

            // 显示修改进度
            var floatProgress = (float)count / Selection.gameObjects.Length;
            EditorUtility.DisplayProgressBar("修改进度", count + " / " + Selection.gameObjects.Length + "完成修改", floatProgress);
        }

        // 清除进度条
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 《检查项一：处理颗粒预设》
    /// 1：位置、旋转。2：移除 KeLiData 和 GranuleModel 脚本和刚体。
    /// </summary>
    /// <param name="selectObj">所选对象</param>
    public void CheckBlockPrefab(GameObject selectObj)
    {
        // 1：如果位置不为 0
        if (selectObj.transform.position != Vector3.zero)
        {
            selectObj.transform.position = Vector3.zero;
            errorPositionNumGranule++;
            WriteToTxt(TxtDirPath, "颗粒《位置错误》汇总", "第 " + errorPositionNumGranule + " 个：" + selectObj.name);
        }

        // 2：如果旋转不为 0
        if (selectObj.transform.rotation != Quaternion.identity)
        {
            errorRotationNumGranule++;
            WriteToTxt(TxtDirPath, "颗粒《旋转错误》汇总", "第 " + errorRotationNumGranule + " 个：" + selectObj.name);
        }

        // 3：移除原来存在的 KeLiData 和 GranuleModel 脚本和刚体。
        if (selectObj.GetComponent<KeLiData>()) Object.DestroyImmediate(selectObj.GetComponent<KeLiData>());
        if (selectObj.GetComponent<GranuleModel>()) Object.DestroyImmediate(selectObj.GetComponent<GranuleModel>());
        if (selectObj.GetComponent<Rigidbody>()) Object.DestroyImmediate(selectObj.GetComponent<Rigidbody>());
    }

    /// <summary>
    /// 《检查项二：处理物件》 1：位置、旋转、比例、名称
    /// </summary>
    /// <param name="selectObj">所选对象</param>
    public void CheckWu(GameObject selectObj)
    {
        // 物件_1
        var wuTrans = selectObj.transform.GetChild(0).transform;

        if (wuTrans)
        {
            // 1：位置不为 0
            if (wuTrans.position != Vector3.zero)
            {
                errorPositionNumWu++;
                WriteToTxt(TxtDirPath, "物件《位置错误》汇总", "第 " + errorPositionNumWu + " 个：" + selectObj.name);

                var wuOriginalVector3 = wuTrans.position;
                for (var i = 0; i < selectObj.transform.childCount; i++)
                {
                    if (selectObj.transform.GetChild(i).name.Contains("物件"))
                    {
                        selectObj.transform.GetChild(i).position = Vector3.zero;
                    }
                    else
                    {
                        selectObj.transform.GetChild(i).position -= wuOriginalVector3;
                    }
                }
            }

            // 2：旋转不为 0
            if (wuTrans.rotation != Quaternion.identity)
            {
                errorRotationNumWu++;
                // 物件旋转问题，属于建模问题，这里只做记录，不做改动。
                WriteToTxt(TxtDirPath, "物件《旋转错误》汇总", "第 " + errorRotationNumWu + " 个：" + selectObj.name);
            }

            // 3：比例不为 1
            if (wuTrans.localScale != Vector3.one)
            {
                errorScaleNumWu++;
                WriteToTxt(TxtDirPath, "物件《比例错误》汇总", "第 " + errorScaleNumWu + " 个：" + selectObj.name);
            }
        }
        else
        {
            errorNoWuName++;
            WriteToTxt(TxtDirPath, "物件《名称错误》汇总", "第 " + errorNoWuName + " 个：" + selectObj.name);
            WindowTips("有名称不是物件_1 的颗粒");
        }
    }

    /// <summary>
    /// 《检查项三：处理关键部位》 1：比例。2：移除 MeshRenderer 和 MeshFilter 组件。
    /// </summary>
    /// <param name="selectObj">所选物体</param>
    public void CheckBuWei(GameObject selectObj)
    {
        for (var i = 0; i < selectObj.transform.childCount; i++)
        {
            // 获取到所有子物体(排除物件_1)
            var childTrans = selectObj.transform.GetChild(i);
            if (Equals(childTrans.name,"物件_1")) continue;

            // 1：把所有子物体的 localScale 统一设置为 1
            if (childTrans.localScale != Vector3.one) childTrans.localScale = Vector3.one;

            // 2：移除掉除所有子物体上的 MeshRenderer 和 MeshFilter 组件
            if (childTrans.GetComponent<MeshRenderer>()) Object.DestroyImmediate(childTrans.GetComponent<MeshRenderer>());
            if (childTrans.GetComponent<MeshFilter>()) Object.DestroyImmediate(childTrans.GetComponent<MeshFilter>());
        }
    }

    /// <summary>
    /// 《检查项四：处理碰撞盒》 1：Center 转化
    /// </summary>
    /// <param name="selectObj">所选对象</param>
    public void CheckBoxCollider(GameObject selectObj)
    {
        // 获取到所有的碰撞盒
        var childrenCollider = selectObj.GetComponentsInChildren<BoxCollider>();

        // 1：碰撞盒的 Center 转化
        foreach (var boxCollider in childrenCollider)
        {
            // 如果碰撞盒 Center 符合要求，则跳过
            if (boxCollider.center == Vector3.zero) continue;

            // TransformPoint：将相对“当前游戏对象”的坐标转化为基于世界坐标系的坐标，与其相反的函数是
            // InverseTransformPoint：将世界坐标转化为相对 当前游戏对象”的基于世界坐标系的坐标
            boxCollider.transform.position = boxCollider.transform.TransformPoint(boxCollider.center);
            boxCollider.center             = Vector3.zero;

            /*
             * 解释说明
             * 拼接代码里要求碰撞盒的 Center 为默认值 Vector3.zero ，但因为修改颗粒预设时会改动碰撞盒，导致碰撞盒 Center 不为 0，
             * 从而引起一些拼搭问题，所以需要用该函数将碰撞盒的 Center 与父物体的 Position 转换一下。
             */
        }
    }

    /// <summary>
    /// 《检查项五：将父物体上的碰撞盒移动到子物体，普通碰撞盒改成 Normal Box，倾斜碰撞盒改成 Bevel Box》
    /// </summary>
    /// <param name="selectObj">所选对象</param>
    public void ChangeColliderMode(GameObject selectObj)
    {
        // 获得父物体上的所有碰撞盒
        var allCollider = selectObj.GetComponentsInChildren<BoxCollider>();
        var bevelCount = 0;  // 倾斜盒子个数
        var normalCount = 0; // 正常盒子个数

        foreach (var box in allCollider)
        {
            //默认 isTrigger 为关
            box.isTrigger = false;

            // 如果是已经更改过名称的颗粒则跳过
            if (box.name.Contains("Normal Box") || box.name.Contains("Bevel Box")) continue;

            /*
             * 之前碰撞盒的加法是
             * 1：如果是普通盒子，就加到父物体上。
             * 2：如果是倾斜盒子，就新建一个 Box (1) 的子物体，单独把碰撞盒加到该子物体。
             * 3：属于新增类型，比如围绕小圆棍加了一圈(8个)倾斜碰撞盒，把这8个碰撞盒放到一个名为 环形碰撞盒 的子物体
             */

            // 情况一：如果子物体名称含有 Box 
            if (box.name.Contains("Box"))
            {
                bevelCount++;

                // 如果是环形碰撞盒下的倾斜碰撞盒 Box
                if (box.transform.parent.name.Contains("碰撞盒"))
                {
                    var parent = box.transform.parent;
                    box.transform.SetParent(selectObj.transform);
                    box.name = "Bevel Box " + "(" + bevelCount + ")";
                    box.transform.SetAsLastSibling();
                    if (parent.childCount != 0) continue;
                    Object.DestroyImmediate(parent.gameObject);
                }
                // 如果是单独的倾斜碰撞盒 Box
                else
                {
                    // 如果 Box 子物体上只有一个碰撞盒，直接改名即可
                    if (box.GetComponents<BoxCollider>().Length == 1)
                    {
                        box.name = "Bevel Box " + "(" + bevelCount + ")";
                        box.transform.SetAsLastSibling();
                    }
                    // 如果 Box 子物体上有多个碰撞盒
                    else
                    {
                        var bevelBox2 = new GameObject("Bevel Box " + "(" + bevelCount + ")");

                        // 复制、粘贴原 Box 上的碰撞盒
                        UnityEditorInternal.ComponentUtility.CopyComponent(box);
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(bevelBox2);
                        bevelBox2.GetComponent<BoxCollider>().isTrigger = false;
                        bevelBox2.transform.SetParent(selectObj.transform);

                        // 复制、粘贴原 Box 上的位置信息
                        UnityEditorInternal.ComponentUtility.CopyComponent(box.transform);
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(bevelBox2.transform);

                        // 删除掉原来的倾斜碰撞盒 Box
                        Object.DestroyImmediate(box);
                    }
                }
            }
            // 情况二：如果是对齐盒子
            else if (box.name.Contains("Align")) box.name = "Align Box";

            // 情况三：排除掉情况一、二后，剩下的碰撞盒
            else
            {
                // 如果是从父物体上获取到的碰撞盒
                if (Equals(box.name, selectObj.name))
                {
                    normalCount++;
                    var normalBox = new GameObject("Normal Box " + "(" + normalCount + ")");
                    UnityEditorInternal.ComponentUtility.CopyComponent(box);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(normalBox);
                    normalBox.transform.SetParent(selectObj.transform);
                    normalBox.transform.SetAsLastSibling();
                    Object.DestroyImmediate(box);
                }
                // 如果关键部位上有碰撞盒
                else
                {
                    errorBuWeiNum++;
                    WriteToTxt(TxtDirPath, "关键部位上有碰撞盒的汇总", "第 " + errorBuWeiNum + " 个：" + selectObj.name + "上的 " + box.name);

                    // 删除碰撞盒
                    Object.DestroyImmediate(box);
                }
            }
        }
    }

    // 一键开启、关闭所有选项
    public void OpenAndCloseAll()
    {
        IsOpenAll = !IsOpenAll;
        IsCheckBlockPrefab = IsCheckWu = IsCheckBoxCollider = IsCheckBuWei = IsOpenAll;
    }

    #endregion

    #region 三：批量处理 Hierarchy 上的颗粒大类

    /// <summary>
    /// 批量处理 Hierarchy 上的颗粒大类，如：方高类
    /// </summary>
    public void ChangeGranuleImage()
    {
        // 存放颗粒大类对象的列表 
        var granuleList = new List<GameObject>();
        // 图集中的小图片
        var content = GameObject.Find("View/Canvas Assembling/Left Tool Panel/Granule Library/Viewport/Content");

        for (var i = 0; i < content.transform.childCount; i++)
        {
            if (content.transform.GetChild(i).GetComponent<GranuleUiType>())
            {
                granuleList.Add(content.transform.GetChild(i).gameObject);
            }
        }

        foreach (var granule in granuleList)
        {
            // 1:是否添加 Border
            if(IsAddBorder) AddBorderAndMain(granule);

//            var prefabObj = Resources.Load<GameObject>("GranuleState");
//            if (prefabObj)
//            {
//                var prefab = Object.Instantiate(prefabObj, granule.transform);
//                Undo.RegisterCreatedObjectUndo(prefab, "GranuleStatePrefab");
//                prefab.name = "GranuleState";
//            }

        }
    }

    /// <summary>
    /// 为每个大类下添加一个子物体 Border 以及一个孙物体 Main，负责零件库单个颗粒大类的UI显示
    /// </summary>
    /// <param name="granule">某一颗粒大类</param>
    public void AddBorderAndMain(GameObject granule)
    {
//        granule.GetComponent<Image>().color = granuleList[0].GetComponent<Image>().color;
        var prefabObj = Resources.Load<GameObject>("Border");
        if (prefabObj)
        {
            var prefab = Object.Instantiate(prefabObj, granule.transform);
            Undo.RegisterCreatedObjectUndo(prefab, "prefab");
        
            prefab.name = "Border";
        
            // 给 Main 换 Sprite
            prefab.transform.GetChild(0).GetComponent<Image>().sprite = 
                ChinarAtlas.LoadSprite("UI/Assembling/Granule Library", "零件库-" + granule.name);
        
            //颗粒类原来的 sprite 设置为空
            granule.GetComponent<Image>().sprite = null;
            granule.GetComponent<Button>().targetGraphic = granule.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        }
    }

    #endregion

    public static ToolPro Instance()
    {
        return instance ?? (instance = new ToolPro());
    }
}