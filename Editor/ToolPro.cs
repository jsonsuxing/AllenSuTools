// ========================================================
// 描述：工具加强版
// 作者：苏醒 
// 创建时间：2019-10-25 17:06:08
// 版 本：1.0
// ========================================================
using System;
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
using Object = UnityEngine.Object;


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
    public int errorBuWeiBoxNum = 0; // 关键部位上有碰撞盒的个数
    public int errorBuWeiRotationNum = 0; // 关键部位上旋转角度不正确的个数
    public int count         = 0; // 进度表示
    public int AllProblemCount = 0; // 所有问题个数
    public string strTips = string.Empty;  // 提示批量操作的方式
    public bool IsHierarchy = true; // 批量处理预设的方式，默认是从层级面板处理，为 false 代表是直接点击预设处理
    public bool IsOpenAll = false; // 一键开启、关闭，默认全关
    public bool IsCheckBlockPrefab = false; // 是否处理 颗粒预设
    public bool IsCheckWu = false; // 是否处理 物件_1
    public bool IsCheckBoxCollider = false; // 是否处理 碰撞盒
    public bool IsCheckBuWei = false; // 是否 处理 关键部位
    public bool IsRenameBoxCollider = false; // 是否将父物体上的碰撞盒移动到子物体，普通碰撞盒改成 Normal Box，倾斜碰撞盒改成 Bevel Box
    public bool IsAddSoAoCao = false; // 是否将含有小插销的颗粒，克隆一个 SoAoCao

    // 三：批量处理 Hierarchy 上的颗粒大类
    public GameObject PrefabObj; // 选择的预设
    public bool IsCreateBorder = false;  // 是否添加 Border
    public bool IsCreateMain = false; // 是否添加 Main
    public bool IsCreateSingleObj = false; // 是否添加指定的单个子物体
    public string DeleteChildName = string.Empty;

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
        // 总进度显示
        var allCount = Selection.gameObjects.Length;

        // 每次点击时重置标记数字
        errorPositionNumGranule = errorRotationNumGranule = 0;
        errorPositionNumWu = errorRotationNumWu = errorScaleNumWu = 0;
        errorBuWeiBoxNum = errorNoWuName = errorBuWeiRotationNum = 0;
        count = AllProblemCount = 0;

        foreach (var selectObj in Selection.gameObjects)
        {
            //--------------------------------------- 检查区 ---------------------------------------

            // 检查项一：处理 颗粒预设
            if (IsCheckBlockPrefab) CheckBlockPrefab(selectObj);

            // 检查项二：处理 物件
            if (IsCheckWu) CheckWu(selectObj);

            // 检查项三：处理 碰撞盒
            if (IsCheckBoxCollider) CheckBoxCollider(selectObj);

            // 检查项四：处理 关键部位
            if(IsCheckBuWei) CheckBuWei(selectObj);


            // ------------------------------------ 一次性功能区 -------------------------------------

            // 功能区一：将父物体上的碰撞盒移动到子物体，普通碰撞盒改成 Normal Box，倾斜碰撞盒改成 Bevel Box，
            if (IsRenameBoxCollider) ChangeColliderMode(selectObj);

            // 功能区二：将含有小插销的颗粒，在同位置克隆一个 SonAoCao
            if(IsAddSoAoCao) AddSonAoCao(selectObj);


            count++;
            PrefabUtility.SaveAsPrefabAsset(selectObj, "Assets/Resources/Prefab/ModelPrefabs/" + selectObj.name+ ".prefab");
            if(IsHierarchy) Object.DestroyImmediate(selectObj);

            //检测出来的问题个数
            AllProblemCount = errorPositionNumGranule + errorRotationNumGranule + errorPositionNumWu + errorRotationNumWu + errorScaleNumWu +
                              errorBuWeiBoxNum        + errorNoWuName           + errorBuWeiRotationNum;

            // 显示修改进度
            var floatProgress = (float)count / allCount;
            EditorUtility.DisplayProgressBar("腾讯管家病毒检测", "已检查个数：" + count + " / " +" 未检查个数："+ Selection.gameObjects.Length
                +" / " + "已检测出 "+ AllProblemCount + " 个病毒", floatProgress);
        }

        // 清除进度条
        EditorUtility.ClearProgressBar();

        if (errorPositionNumGranule != 0 || errorRotationNumGranule != 0 || errorPositionNumWu !=0 || errorRotationNumWu != 0
            || errorScaleNumWu != 0 || errorBuWeiBoxNum != 0 || errorNoWuName != 0 || errorBuWeiRotationNum != 0)
        {
            WindowTips("已检查出 "+ AllProblemCount +" 个问题，详见 《 D:/ 编辑器生成的txt文件汇总 》 文件夹");
            System.Diagnostics.Process.Start(TxtDirPath); // 文件夹存在就直接打开
        }
        else if (errorPositionNumGranule == 0 && errorRotationNumGranule == 0 && errorPositionNumWu == 0 && errorRotationNumWu == 0
                 && errorScaleNumWu == 0 && errorBuWeiBoxNum == 0 && errorNoWuName == 0 && errorBuWeiRotationNum == 0)
        {
            WindowTips("恭喜你，已经没有问题了！！！");
        }
    }

    //--------------------------------------- 检查区 ---------------------------------------
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
            WriteToTxt(TxtDirPath, "颗粒《位置错误》汇总（已自动修正）", "第 " + errorPositionNumGranule + " 个：" + selectObj.name);
        }

        // 2：如果旋转不为 0
        if (selectObj.transform.rotation != Quaternion.identity)
        {
            errorRotationNumGranule++;
            WriteToTxt(TxtDirPath, "颗粒《旋转错误》汇总（仅记录）", "第 " + errorRotationNumGranule + " 个：" + selectObj.name);
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
                WriteToTxt(TxtDirPath, "物件《位置错误》汇总（已自动修正）", "第 " + errorPositionNumWu + " 个：" + selectObj.name);

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
                WriteToTxt(TxtDirPath, "物件《旋转错误》汇总（仅记录）", "第 " + errorRotationNumWu + " 个：" + selectObj.name);
            }

            // 3：比例不为 1
            if (wuTrans.localScale != Vector3.one)
            {
                errorScaleNumWu++;
                WriteToTxt(TxtDirPath, "物件《比例错误》汇总（仅记录）", "第 " + errorScaleNumWu + " 个：" + selectObj.name);
            }
        }
        else
        {
            errorNoWuName++;
            WriteToTxt(TxtDirPath, "物件《名称错误》汇总（仅记录）", "第 " + errorNoWuName + " 个：" + selectObj.name);
            WindowTips("有名称不是物件_1 的颗粒");
        }
    }

    /// <summary>
    /// 《检查项三：处理关键部位》 1：旋转。2：移除 MeshRenderer 和 MeshFilter 组件。
    /// 3：所有子物体的比例。
    /// </summary>
    /// <param name="selectObj">所选物体</param>
    public void CheckBuWei(GameObject selectObj)
    {
        // 先对除物件之外的所有子物体进行一个比例判断,并直接修正，不做记录。
        foreach (Transform child in selectObj.transform)
        {
            if (Equals(child.name, "物件_1")) continue;
            if (child.localScale != Vector3.one) child.localScale = Vector3.one;
        }

        // 获得所有的关键部位
        var buWeiArr = selectObj.GetComponentsInChildren<GuanJianBuWei>();

        var index = 0;
        foreach (var buWei in buWeiArr)
        {
            if (buWei.transform.localRotation != Quaternion.identity)
            {
                index++;
                WriteToTxt(TxtDirPath, "关键部位《旋转错误》汇总（仅记录）", index == 1 ? 
                    "第 " + (++errorBuWeiRotationNum) + " 个：" + selectObj.name + " 上的 " + buWei.name : buWei.name);
            }

            // 如果关键部位上存在 MeshRenderer 和 MeshFilter 组件，则移除。
            if (buWei.transform.GetComponent<MeshRenderer>()) Object.DestroyImmediate(buWei.transform.GetComponent<MeshRenderer>());
            if (buWei.transform.GetComponent<MeshFilter>()) Object.DestroyImmediate(buWei.transform.GetComponent<MeshFilter>());
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

    // ------------------------------------ 一次性功能区 -------------------------------------
    /// <summary>
    /// 《功能区一》：将父物体上的碰撞盒移动到子物体，普通碰撞盒改成 Normal Box，倾斜碰撞盒改成 Bevel Box
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
                    errorBuWeiBoxNum++;
                    WriteToTxt(TxtDirPath, "关键部位上有碰撞盒的汇总", "第 " + errorBuWeiBoxNum + " 个：" + selectObj.name + "上的 " + box.name);

                    // 删除碰撞盒
                    Object.DestroyImmediate(box);
                }
            }
        }
    }

    /// <summary>
    /// 《功能区二》：将含有小插销的颗粒，在同位置克隆一个 SonAoCao
    /// </summary>
    /// <param name="selectObj">所选对象</param>
    public void AddSonAoCao(GameObject selectObj)
    {
        var selectTrans = selectObj.transform;

        // 先把之前有的 SonAoCao 删除
        foreach (Transform t in selectTrans)
        {
            if (t.name.Contains("SonAoCao"))
            {
                Object.DestroyImmediate(t.gameObject);
            }
        }

        // 克隆 SonAoCao
        var number = 1;
        foreach (Transform t in selectTrans)
        {
            if (t.name.Contains("XiaoChaXiao"))
            {
                var sonAoCao = Object.Instantiate(t, selectTrans);
                sonAoCao.name = "SonAoCao " + "(" + (number++) + ")";
                var buWeiSonAoCao = sonAoCao.GetComponent<GuanJianBuWei>();
                buWeiSonAoCao.type      = GuanJianType.Grid;
                buWeiSonAoCao.size      = 0;
                buWeiSonAoCao.dirVector = Vector3.zero;
                buWeiSonAoCao.length    = 0;
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

        // 获取到所有颗粒大类对象
        for (var i = 0; i < content.transform.childCount; i++)
        {
            if (content.transform.GetChild(i).GetComponent<GranuleUiType>())
            {
                granuleList.Add(content.transform.GetChild(i).gameObject);
            }
        }

        foreach (var granule in granuleList)
        {
            // 1：是否添加指定的单个子物体
            if (IsCreateSingleObj) CreateSingleChild(granule);

            // 2：是否添加 Border
            if (IsCreateBorder) CreateBorder(granule);

            // 3：是否添加 Main
            if(IsCreateMain) CreateMain(granule);

            if(!Equals(DeleteChildName,string.Empty)) DeleteGranuleChild(granule);
        }

        PrefabObj       = null;
        IsCreateSingleObj = false;
        IsCreateBorder = false;
        IsCreateMain    = false;
        DeleteChildName = string.Empty;
    }

    /// <summary>
    /// 添加单个子物体
    /// </summary>
    /// <param name="granule">某一颗粒大类</param>
    public void CreateSingleChild(GameObject granule)
    {
        if (PrefabObj)
        {
            var prefab = Object.Instantiate(PrefabObj, granule.transform);
            Undo.RegisterCreatedObjectUndo(prefab, "SingleChildPrefab");
            prefab.name = PrefabObj.name;
        }
        else
        {
            WindowTips("预设不能为空");
        }
    }

    /// <summary>
    /// 为每个大类下添加一个子物体 Border
    /// </summary>
    /// <param name="granule">某一颗粒大类</param>
    public void CreateBorder(GameObject granule)
    {
        if (PrefabObj)
        {
            var prefab = Object.Instantiate(PrefabObj, granule.transform);
            Undo.RegisterCreatedObjectUndo(prefab, "BorderPrefab");

            prefab.name = PrefabObj.name;
            //颗粒类原来的 sprite 设置为空
            if (granule.GetComponent<Image>().sprite != null) granule.GetComponent<Image>().sprite = null;
        }
        else
        {
            WindowTips(" Border 的预设不能为空");
        }
    }

    /// <summary>
    /// 为每个大类下添加一个子物体 Main
    /// </summary>
    /// <param name="granule">某一颗粒大类</param>
    public void CreateMain(GameObject granule)
    {
        if (PrefabObj)
        {
            var prefab = Object.Instantiate(PrefabObj, granule.transform);
            Undo.RegisterCreatedObjectUndo(prefab, "MainPrefab");

            prefab.name = PrefabObj.name;
            // 给 Main 更换 Sprite ，并且直接传值给颗粒大类的 Image；
            prefab.transform.GetComponent<Image>().sprite = ChinarAtlas.LoadSprite("UI/Assembling/Granule Library", "零件库-" + granule.name);
            granule.GetComponent<Button>().targetGraphic = granule.transform.Find("Main").GetComponent<Image>();
        }
        else
        {
            WindowTips(" Main 的预设不能为空");
        }
    }

    /// <summary>
    /// 根据指定名称，删除子物体
    /// </summary>
    /// <param name="granule">所选对象</param>
    public void DeleteGranuleChild(GameObject granule)
    {
        if (granule.transform.Find(DeleteChildName))
        {
            Undo.DestroyObjectImmediate(granule.transform.Find(DeleteChildName).gameObject);
        }
    }

    #endregion

    public static ToolPro Instance()
    {
        return instance ?? (instance = new ToolPro());
    }
}