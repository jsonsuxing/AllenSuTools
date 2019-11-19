// ========================================================
// 描述：模型相关编辑器管理类
// 作者：苏醒 
// 创建时间：2019-07-18 21:07:36
// 版 本：1.0
// ========================================================

using System.Collections.Generic;
using System.IO;
using System.Linq;
using QmDreamer.Data.Enum;
using ToolGood.Words;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 模型相关编辑器管理类
/// </summary>
public class SelfModel : CommonFun
{
    private static SelfModel instance;

    #region 字段声明

    // 前提
    public string ShowWay = string.Empty; // 模型的操作方式
    public bool   isAddNewGranuleWay;     // 是否是新上架的操作方式，默认为false，即默认是更换模型的方式。

    // 导入新模型
    public string       ModelFolderPath = string.Empty; // 模型文件夹路径
    public int          NoSignNum       = 0;            // 没有 & 符号的颗粒个数
    public int          ImportModelNum  = 0;            // 导入模型个数
    public string       ResultStr       = string.Empty; // 查询结果
    public List<string> ResultList      = new List<string>();

    // 添加 SSS关键部位 脚本
    public int FailureGranuleNum = 0; // 模型含有子物体的文件个数
    public int SucceedGranuleNum = 0; // 成功添加脚本的个数

    // 上架新颗粒
    public int    CloneModelNum     = 0;            // 克隆了多少个
    public string ImportGranuleName = string.Empty; //单个导入的颗粒名称
    public bool   IsWrongDirectory;                 // 模型是否分配错了文件夹

    // 其它
    public bool IsOpenGranuleToggle    = false; // 颗粒种类按钮是否打开
    public bool IsRememberGranuleType  = false; //是否记住颗粒大类路径
    public bool IsRememberOutModelPath = false; // 是否记住外部模型路径

    #endregion

    #region 一：导入新模型

    /// <summary>
    /// 导入新模型
    /// </summary>
    public void ImportNewModel()
    {
        if (string.Equals(ModelFolderPath, string.Empty))
        {
            WindowTips("外部模型路径不能为空");
            return;
        }

        // 获取外部模型文件夹里的所有文件
        var modelFiles = Directory.GetFiles(ModelFolderPath, "*.*", SearchOption.AllDirectories);
        if (modelFiles.Length == 0)
        {
            WindowTips("外部模型为 空 文件夹");
            return;
        }

        // 新上架颗粒
        if (isAddNewGranuleWay)
        {
            // 颗粒要导入的工程路径:这里会统一导入到 tempModelFile 文件夹，方便下一步添加 SSSGuanJianBuWei 脚本
            var importKeLiPath = Application.dataPath + "/Other/InitialModels/" + "tempModelFile/";
            CreateNewDirectory(importKeLiPath);

            // 导入外部模型文件到工程文件
            foreach (var modelFile in modelFiles)
            {
                string extension = Path.GetExtension(modelFile);

                //扩展名为 png,jpg 类型的文件，删掉
                if (string.Equals(extension, ".png") || string.Equals(extension, ".jpg"))
                {
                    File.Delete(modelFile);
                }

                //扩展名为 fbx 类型的文件，导入到 Other/InitialModels/tempModelFile 下
                if (string.Equals(extension, ".fbx"))
                {
                    //获得模型文件名
                    var modelFileName = Path.GetFileNameWithoutExtension(modelFile);
                    #region 把没有 & 符号的文件名写入本地(不加此判断会出现越界情况)

                    if (!modelFileName.Contains("&"))
                    {
                        NoSignNum++;
                        WriteToTxt("D:/suxing/", "不含有&符号的颗粒名称",
                            "第" + NoSignNum + "个没有 & 分隔符的颗粒名称是：" + modelFileName);

                        //跳出当前循环
                        continue;
                    }

                    #endregion

                    // 移动到工程目录下
                    File.Move(modelFile,
                        Path.GetDirectoryName(importKeLiPath) + "\\" + modelFileName + Path.GetExtension(modelFile));
                    ImportModelNum++;
                }
            }

            if (NoSignNum == 0)
            {
                WindowTips("导入模型成功：外部模型文件有 " + modelFiles.Length + " 个，此次导入模型 " + ImportModelNum + " 个");
                ImportModelNum = 0;
            }
            else
            {
                WindowTips("导入模型成功：外部模型文件有 " + modelFiles.Length    + " 个，此次导入模型 " +
                           ImportModelNum    + " 个，外部模型不含标志符号的文件有 " + NoSignNum    + " 个，" +
                           "详见：D/suxing/不含有&符号的颗粒名称.txt 文件");
                ImportModelNum = 0;
                NoSignNum      = 0;
            }
        }
        // 更换旧模型
        else
        {
            // 要导入的工程路径
            string projectModelPath = Application.dataPath + "/Other/InitialModels/" + ShowGranuleType + "/";

            // 获得外部需要导入的具体模型文件全路径
            string outFileName = ModelFolderPath + "/" + ImportGranuleName + ".fbx";
            if (!File.Exists(outFileName))
            {
                WindowTips("外部模型文件夹里没有 " + ImportGranuleName + ".fbx " + "这个文件");
                return;
            }

            // 导入前先把原来的模型给删除掉
            if (File.Exists(projectModelPath + ImportGranuleName + ".fbx"))
            {
                File.Delete(projectModelPath + ImportGranuleName + ".fbx");
            }
            else
            {
                // 如果指定的文件夹没有该模型，那就是原来模型分配错了文件夹或者不存在
                IsWrongDirectory = true;
            }

            // 把指定模型文件移动到对应工程文件路径
            string fileName = Path.GetFileNameWithoutExtension(outFileName);
            File.Move(outFileName, Path.GetDirectoryName(projectModelPath) + "\\" + fileName + Path.GetExtension(outFileName));
            WindowTips(IsWrongDirectory ? "恭喜你，导入成功，但 之前模型分配错了文件夹 或 不存在，请查看！！！" : "恭喜你，导入成功");
            IsWrongDirectory = false;
        }

        // 刷新资源
        AssetDatabase.Refresh();
        // ModelFolderPath = string.Empty;
        // 清空模型名
        ImportGranuleName = string.Empty;
    }

    #endregion

    #region 二：添加 SSSCreateGuanJians 脚本

    /// <summary>
    /// 添加 SSSCreateGuanJians 脚本
    /// </summary>
    public void AddScript()
    {
        // 所选物体
        var selectObj = Selection.gameObjects;
        // 如果没有选中模型，则提示
        if (selectObj.Length == 0)
        {
            WindowTips("没有选中模型");
            return;
        }

        for (int i = 0; i < selectObj.Length; i++)
        {
            // 先判断模型是否有错误(模型含有子物体)
            if (selectObj[i].transform.childCount != 0)
            {
                FailureGranuleNum++;

                // 错误模型文件写入到本地
                WriteToTxt("D:/suxing/", "含有子物体的错误模型",
                    "第" + FailureGranuleNum + "个含有子物体的颗粒名称是：" + selectObj[i].name);
                continue;
            }

            selectObj[i].AddComponent<SSSCreateGuanJians>();
            SucceedGranuleNum++;
        }

        if (FailureGranuleNum == 0)
        {
            WindowTips("此次共计选中模型 " + selectObj.Length + " 个，成功添加脚本 " + SucceedGranuleNum + " 个");
            SucceedGranuleNum = 0;
        }
        else
        {
            WindowTips("此次共计选中模型 "   + selectObj.Length  + " 个，成功添加脚本 " + SucceedGranuleNum + " 个，" +
                       "含有子物体的错误模型 " + FailureGranuleNum + " 个，详见：D:/suxing/含有子物体的错误模型.txt 文件");
            SucceedGranuleNum = 0;
            FailureGranuleNum = 0;
        }
    }

    #endregion

    #region 三：上架新颗粒

    /// <summary>
    /// 上架新颗粒
    /// </summary>
    public void ShelfNewGranule()
    {
        // 所有颗粒的父物体 Content
        var content = GameObject.Find("View/Canvas Assembling/Left Tool Panel/Granule Library/Viewport/Content");

        // 如果没有选中颗粒，则提示
        if (content == null || content.name != "Content")
        {
            WindowTips("没有选中颗粒父物体 Content");
            return;
        }

        JudgeGranuleType();
        #region 克隆新颗粒

        //要往哪个颗粒大类上架颗粒
        var transKeLiType = content.transform.Find(ShowGranuleType);
        //获得 BlockType 对象
        var blockType = transKeLiType.GetComponent<BlockType>();
        /*
         * 先获得所选类别的第一个颗粒
         * 1：因为是同级关系，所以用了这个方法
         */
        if (transKeLiType.GetSiblingIndex() + 1 >= content.transform.childCount)
        {
            WindowTips("当前颗粒大类尚未添加任何颗粒，请手动复制一个到该类下面");
            return;
        }

        var firstKeLi = content.transform.GetChild(transKeLiType.GetSiblingIndex() + 1).gameObject;

        //获取 /Other/InitialModels/tempModelFile/ 路径下的文件
        //假如此次有100个模型，有20个属于方高类，要把这20个模型另外单独放到一个文件夹里
        var importKeLiPath = Application.dataPath + "/Other/InitialModels/";
        var modelFiles     = Directory.GetFiles(importKeLiPath + "tempModelFile/", "*.*", SearchOption.AllDirectories);
        if (modelFiles.Length == 0)
        {
            WindowTips("/Other/InitialModels/tempModelFile/ 下没有任何文件");
            return;
        }

        foreach (var modelFile in modelFiles)
        {
            // 只需要扩展名是 fbx 的文件
            var extension = Path.GetExtension(modelFile);
            if (string.Equals(extension, ".fbx"))
            {
                // 获得路径下不含扩展名的文件名
                var modelFileName = Path.GetFileNameWithoutExtension(modelFile);
                // 克隆新颗粒,并重新命名
                var newKeLi = Object.Instantiate(firstKeLi, content.transform);
                if (newKeLi == null)
                {
                    WindowTips("克隆新颗粒失败！！！");
                    return;
                }

                newKeLi.name = modelFileName;
                // 设置新颗粒下标
                // newKeLi.transform.SetSiblingIndex(transKeLiType.GetSiblingIndex() + blockType.GranuleModelList.Count + 1);
                // 因为后来改为了点开颗粒大类再去动态加载数据，所以注释下面代码
                // blockType.GranuleModelList.Add(content.transform.Find(modelFileName).gameObject);
                CloneModelNum++;
            }
        }

        //相等时自动移动 tempModelFile/ 下的所有文件到所属颗粒大类
        if (CloneModelNum == modelFiles.Length / 2)
        {
            foreach (var modelFile in modelFiles)
            {
                //只需要扩展名是 fbx 的文件
                var extension = Path.GetExtension(modelFile);
                if (string.Equals(extension, ".fbx"))
                {
                    //获得路径下不含扩展名的文件名
                    var modelFileName = Path.GetFileNameWithoutExtension(modelFile);
                    //移动到工程目录下
                    File.Move(modelFile,
                        Path.GetDirectoryName(importKeLiPath + ShowGranuleType + "/") + "\\" + modelFileName +
                        Path.GetExtension(modelFile));
                }
            }

            //刷新资源
            AssetDatabase.Refresh();
        }

        WindowTips("克隆详情：/Other/InitialModels/tempModelFile/ 下有 " + modelFiles.Length / 2 + " 个文件，" +
                   "此次成功克隆 "                                      + CloneModelNum         + " 个新颗粒");
        CloneModelNum = 0;

        #endregion
    }

    #endregion

    #region 其它函数

    #region 一：修改模型导入工程后，用该模型的Mesh,替换预设上丢失的Mesh

    /// <summary>
    /// 修改模型导入工程后，用该模型的Mesh,替换预设上丢失的Mesh
    /// </summary>
    public void ReplaceMesh()
    {
        var obj = Selection.activeGameObject;
        if (!Equals(obj.name, "物件_1"))
        {
            WindowTips("选中的物体不是 物件_1");
            return;
        }

        var parentName = obj.transform.parent.name;
        obj.GetComponent<MeshFilter>().mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Other/InitialModels/" + ShowGranuleType + "/" + parentName + ".fbx");
    }

    #endregion

    #region 二：根据汉字首写字母，返回颗粒大类名称

    /// <summary>
    /// 根据汉字首字母，获取相应的颗粒大类名称
    /// </summary>
    public void ChineseChar()
    {
        foreach (var granuleType in AllGranuleTypeGroup)
        {
            if (WordsHelper.GetFirstPinYin(granuleType).Contains(ResultStr))
            {
                ResultList.Add(granuleType);
            }
        }

        // 搜索结果只有一个
        if (ResultList.Count == 1)
        {
            ShowGranuleType = ResultList[0];
        }

        // 搜索结果大于1的在 SetGranuleName() 函数里判断

        // 没搜到
        if (ResultList.Count == 0)
        {
            WindowTips("未找到结果，请重新搜索");
            ResultStr = string.Empty;
        }
    }


    /// <summary>
    /// 搜索结果不唯一时通过按钮赋值
    /// </summary>
    public void SetGranuleName()
    {
        // 把搜到的结果通过按钮形式显示出来
        for (int i = 0; i < ResultList.Count; i++)
        {
            if (GUILayout.Button(ResultList[i]))
            {
                ShowGranuleType = ResultList[i];
            }
        }
    }


    /// <summary>
    /// 把搜索到的结果设置为按钮名称
    /// </summary>
    public void SetTypeBtn()
    {
        // 内存有值则删除
        if (PlayerPrefs.HasKey("颗粒大类名称"))
        {
            PlayerPrefs.DeleteKey("颗粒大类名称");
        }

        for (int i = 0; i < 4; i++)
        {
            // ------------ 一：开始水平画盒子 ------------
            GUILayout.BeginVertical("box");

            // 第一组水平排版开始
            EditorGUILayout.BeginHorizontal();
            for (int j = i * 5; j < (i + 1) * 5; j++)
            {
                if (j < FirstPinYin.Length)
                {
                    if (GUILayout.Button(FirstPinYin[j]))
                    {
                        // 每次点击按钮要清空结果列表
                        ResultList.Clear();
                        ResultStr += FirstPinYin[j];
                        ChineseChar();
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            // 第一组水平排版结束
            GUILayout.EndVertical();
            // ------------ 一：结束水平画盒子 ------------
        }
    }


    /// <summary>
    /// 清除ResultStr，ResultList数据
    /// </summary>
    public void ClearGranuleData()
    {
        ResultStr = string.Empty;
        ResultList.Clear();
    }

    #endregion

    #endregion


    public static SelfModel Instance()
    {
        return instance ?? (instance = new SelfModel());
    }
}