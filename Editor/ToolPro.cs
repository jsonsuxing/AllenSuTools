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
using UI.ThreeDimensional;
using UnityEditor;
using UnityEngine;
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

    /// <summary>
    /// 是否检查颗粒和物件_1的位置，旋转问题
    /// </summary>
    public bool IsCheckGranuleAndWu = false;

    /// <summary>
    /// 是否检查物件的比例，以及移除关键部位上的已存在的 MeshRenderer 和 MeshFilter
    /// </summary>
    public bool IsCheckWuScaleAndRemoveMrMf = false;

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


    /// <summary>
    /// 删除 txt
    /// </summary>
    public void DeleteTxt()
    {
        if (File.Exists(TxtPath))
        {
            File.Delete(TxtPath);
            WindowTips("已删除");
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
        if (Selection.gameObjects.Length == 0)
        {
            WindowTips("至少选择一个物体");
            return;
        }

        var errorPositionNumGranule = 0; // 颗粒位置错误个数
        var errorRotationNumGranule = 0; // 颗粒旋转位置错误个数

        var errorPositionNumWu = 0; // 物件位置错误个数
        var errorRotationNumWu = 0; // 物件旋转位置错误个数
        var errorScaleNumWu = 0; // 物件比例错误个数

        var errorBuWeiNum = 0; // 关键部位上有碰撞盒的个数
        var count = 0; // 进度表示

        foreach (var selectObj in Selection.gameObjects)
        {
            var selectTransform = selectObj.transform;
            Transform wuTrans;
            if (selectObj.transform.Find("物件_1"))
            {
                wuTrans = selectTransform.Find("物件_1");
            }
            else
            {
                WindowTips("物件命名错误：" + selectObj.name);
                wuTrans = null;
            }

            #region 检查项一：颗粒和物件_1的位置，旋转

            if (IsCheckGranuleAndWu)
            {
                // 检查位置不为 0 的颗粒
                if (selectTransform.position != Vector3.zero)
                {
                    selectTransform.position = Vector3.zero;
                    errorPositionNumGranule++;
                    WriteToTxt(TxtDirPath, "颗粒位置错误字典", "第 " + errorPositionNumGranule + " 个：" + selectTransform.name);
                }
                // 检查旋转不为 0 的颗粒
                if (selectTransform.rotation != Quaternion.identity)
                {
                    errorRotationNumGranule++;
                    WriteToTxt(TxtDirPath, "颗粒旋转错误字典", "第 " + errorRotationNumGranule + " 个：" + selectTransform.name);
                }

                if (wuTrans)
                {
                    // 检查位置不为 0 的物件
                    if (wuTrans.position != Vector3.zero)
                    {
                        errorPositionNumWu++;
                        WriteToTxt(TxtDirPath, "物件位置错误字典", "第 " + errorPositionNumWu + " 个：" + selectTransform.name);
                        Vector3 wuJianOriginalVector3 = wuTrans.position;
                        foreach (Transform t in selectTransform)
                        {
                            if (t.name.Contains("物件")) //这里注意，把物件的改为物件_1 统一。由于不是所有都是物件_1所以，我这里模糊下查找。
                            {
                                t.position = Vector3.zero;
                            }
                            else
                            {
                                t.position -= wuJianOriginalVector3;
                            }
                        }
                    }
                    // 检查旋转不为 0 的物件
                    if (wuTrans.rotation != Quaternion.identity) //物件旋转问题，属于他们的问题，不做变动。下方文件，已做记录。通知他们修改初始角度。
                    {
                        errorRotationNumWu++;
                        WriteToTxt(TxtDirPath, "物件旋转错误字典", "第 " + errorRotationNumWu + " 个：" + selectTransform.name);
                    }
                }
            }

            #endregion

            var newPrefab = selectObj;
            // 单个颗粒
            var singleGranule = newPrefab.transform;

            #region 检查项二：记录物件比例不为1的颗粒名称，并且删除关键部位上的MeshRenderer和MeshFilter

            if (IsCheckWuScaleAndRemoveMrMf)
            {
                singleGranule.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(_ =>
                {
                    if (_.name.Contains("物件_1"))
                    {
                        if (_.transform.localScale == Vector3.one) return;
                        errorScaleNumWu++;
                        WriteToTxt(TxtDirPath, "物件比例错误个数", "第 " + errorScaleNumWu + " 个：" + selectTransform.name);
                    }
                    else
                    {
                        DestroyImmediate(_.GetComponent<MeshFilter>());
                        DestroyImmediate(_);
                    }
                });
            }

            #endregion

            // 获得父物体上的所有碰撞盒
            var allCollider = newPrefab.GetComponentsInChildren<BoxCollider>();
            var bevelCount = 0;  // 倾斜盒子个数
            var normalCount = 0; // 正常盒子个数
            foreach (var box in allCollider)
            {
                //默认 isTrigger为关
                box.isTrigger = false;
                if (box.name.Contains("Normal Box") || box.name.Contains("Bevel Box")) continue;
                if (box.name.Contains("Box"))
                {
                    bevelCount++;
                    if (box.transform.parent.name.Contains("碰撞盒"))
                    {
                        var parent = box.transform.parent;
                        box.transform.SetParent(newPrefab.transform);
                        box.name = "Bevel Box " + "(" + bevelCount + ")";
                        box.transform.SetAsLastSibling();
                        if (parent.childCount != 0) continue;
                        DestroyImmediate(parent.gameObject);
                    }
                    else
                    {
                        if (box.GetComponents<BoxCollider>().Length == 1)
                        {
                            box.name = "Bevel Box " + "(" + bevelCount + ")";
                            box.transform.SetAsLastSibling();
                        }
                        else
                        {
                            var bevelBox2 = new GameObject("Bevel Box " + "(" + bevelCount + ")");
                            // 复制、粘贴原Box上的碰撞盒
                            UnityEditorInternal.ComponentUtility.CopyComponent(box);
                            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(bevelBox2);
                            bevelBox2.GetComponent<BoxCollider>().isTrigger = false;
                            bevelBox2.transform.SetParent(newPrefab.transform);
                            // 复制、粘贴原Box上的位置信息
                            UnityEditorInternal.ComponentUtility.CopyComponent(box.transform);
                            UnityEditorInternal.ComponentUtility.PasteComponentValues(bevelBox2.transform);
                            DestroyImmediate(box);
                        }
                    }
                }
                else if (box.name.Contains("Align"))
                {
                    box.name = "Align Box";
                }
                // 从父物体上获取到的碰撞盒  （也有可能是关键部位上有碰撞盒）
                else
                {
                    // 从父物体上获取到的碰撞盒
                    if (Equals(box.name, singleGranule.name))
                    {
                        normalCount++;
                        var normalBox = new GameObject("Normal Box " + "(" + normalCount + ")");
                        UnityEditorInternal.ComponentUtility.CopyComponent(box);
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(normalBox);
                        normalBox.transform.SetParent(newPrefab.transform);
                        normalBox.transform.SetAsLastSibling();
                        DestroyImmediate(box);
                    }
                    // 关键部位上有碰撞盒
                    else
                    {
                        errorBuWeiNum++;
                        WriteToTxt(TxtDirPath, "关键部位上有碰撞盒汇总", "第 " + errorBuWeiNum + " 个：" + singleGranule.name + "上的 " + box.name);

                        // 删除碰撞盒
                        DestroyImmediate(box);
                    }
                }
            }


            count++;
            PrefabUtility.SaveAsPrefabAsset(newPrefab, "Assets/Resources/Prefab/ModelPrefabs/" + selectObj.name);
            DestroyImmediate(newPrefab);
            var floatProgress = (float)count / Selection.gameObjects.Length;
            EditorUtility.DisplayProgressBar("修改进度", count + "/" + Selection.gameObjects.Length + "完成修改", floatProgress);
        }

        EditorUtility.ClearProgressBar();
        if (File.Exists("D:/Users/suxing/Desktop/没有实例化颗粒成功汇总.txt"))
        {
            WindowTips("没有实例化颗粒成功汇总文件已存在，详见路径：D:/Users/suxing/Desktop/没有实例化颗粒成功汇总.txt");
        }
    }

    #endregion

    public static ToolPro Instance()
    {
        return instance ?? (instance = new ToolPro());
    }
}