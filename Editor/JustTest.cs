// ========================================================
// 描述：仅仅是随便写的测试功能
// 作者：苏醒 
// 创建时间：2019-12-17 16:55:07
// 版 本：1.0
// ========================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using LitJson;

public class JustTest : CommonFun
{
    private static JustTest instance;

    /// <summary>
    /// 仅是测试按钮功能而用
    /// </summary>
    public void JustTestBtn()
    {
        // 比较建模发了模型，但在编码表里搜索不到的颗粒名称
        CompareWrongFbxName();

        // 查询一下已经上架了零件库，但仍在表格标记X的颗粒名称
        // FindGranuleType();

        // 检查分配错颗粒大类文件夹的fbx文件
        // CheckTypeError();

        // 检查fbx的同名文件  输入不重名的文件名
        // CheckSameName();

        // 检查资源文件夹下颗粒预设物和fbx模型名称中含有 X 的名称，并更改名称
        // ToCheckBigXName();

        // 查询场景中含有大写 X 的颗粒名
        // ReadCapitalName();
    }

    /// <summary>
    /// 查询一下已经上架了零件库，但仍在表格标记X的颗粒名称
    /// </summary>
    public void FindGranuleType()
    {
        string jsonPath = Application.dataPath + "/AllenSuTools/Data/是否已上架数据.txt";

        // 先从 json 中读取数据
        TextReader tr = File.OpenText(jsonPath);
        var jsonRootData = JsonMapper.ToObject<Root>(tr.ReadToEnd());
        tr.Dispose();
        tr.Close();

        var granuleNameDic = new Dictionary<string, string>();
        foreach (var primaryData in jsonRootData.links)
        {
            // 获取到所有未上架的颗粒名称
            if (Equals(primaryData.是否已上架,"2"))
            {
                granuleNameDic.Add(primaryData.总序,primaryData.全称);
            }
        }

        foreach (var granule in PrimaryGranuleList)
        {
            if (granuleNameDic.ContainsValue(granule.name))
            {
                Debug.Log(granule.name);
            }
        }
    }

    /// <summary>
    /// 比较建模发了模型，但在编码表里搜索不到的颗粒名称
    /// </summary>
    public void CompareWrongFbxName()
    {
        string jsonPath = Application.dataPath + "/AllenSuTools/Data/建模已发模型名称.txt";

        // 先从 json 中读取数据
        TextReader tr = File.OpenText(jsonPath);
        var jsonRootData = JsonMapper.ToObject<JianMoRoot>(tr.ReadToEnd());
        tr.Dispose();
        tr.Close();

        //初级颗粒数据
        string jsonPath1 = Application.dataPath + "/AllenSuTools/Data/初级颗粒数据.txt";

        // 先从 json 中读取数据
        TextReader tr1 = File.OpenText(jsonPath1);
        var jsonRootData1 = JsonMapper.ToObject<RootData>(tr1.ReadToEnd());
        tr1.Dispose();
        tr1.Close();

        var tempDic = new Dictionary<string, string>();
        foreach (var primaryData in jsonRootData1.primaryData)
        {
            tempDic.Add(primaryData.ID,primaryData.FullCode);
        }

        foreach (var rootData in jsonRootData.links)
        {
            if (!tempDic.ContainsValue(rootData.建模已发))
            {
                WriteToTxt(TxtDirPath,"建模发的模型中错误名称",rootData.建模已发);
            }
        }
    }

    /// <summary>
    /// 查询场景中含有大写 X 的颗粒名
    /// </summary>
    public void ReadCapitalName()
    {
        if (PrimaryGranuleList.Count == 0)
        {
            WindowTips("需先点击《一般工具》编辑器里的《零件库相关数据显示区》按钮为列表赋值");
            return;
        }
        if (Content)
        {
            foreach (var granule in PrimaryGranuleList)
            {
                if (granule.name.Contains("X"))
                {
                    // WriteToTxt(TxtDirPath,"含有大写 X 的颗粒名称",granule.name);
                    granule.name = granule.name.Replace("X", "x");
                }
            }
        }

        // 直接打开文件
        // System.Diagnostics.Process.Start(TxtDirPath + "含有大写 X 的颗粒名称.txt");
    }

    /// <summary>
    /// 检查资源文件夹下颗粒预设物和fbx模型名称中含有 X 的名称，并更改名称
    /// </summary>
    public void ToCheckBigXName()
    {
        // 检查 prefab 中是否含有 X 的文件
        CheckBigXName(PrefabPath, "*.prefab", "prefab 中含有 X 的文件名称", "prefab 中已不存在含有 X 的文件");
        // 检查 fbx 中是否含有 X 的文件
        CheckBigXName(FbxPath, "*.fbx", "fbx 中含有 X 的文件名称", "fbx 中已不存在含有 X 的文件");
        // 检查 边框文件 中是否含有 X 的文件
        CheckBigXName(EdgePath, "*.dat", "边框文件 中含有 X 的文件名称", "边框文件 中已不存在含有 X 的文件");


        #region 高华代码(已注释)

        // var datas = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets).
        // Where(_ => Path.GetExtension(AssetDatabase.GetAssetPath(_)) != "");
        //
        //  var dict= datas.DistinctBy(a =>Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))).
        // ToDictionary(_ => Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(_)),AssetDatabase.GetAssetPath);
        //
        //  foreach (var o in dict)
        //  {
        //
        //      var newName = o.Key.Replace("毋", "x");
        //      // Debug.Log(o.Value.Replace(o.Key, newName));
        //     AssetDatabase.RenameAsset(o.Value, newName);
        // }
        //
        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();
        // return;

        #endregion
    }

    /// <summary>
    /// 检查资源文件夹下颗粒预设物、fbx模型、边框文件中含有 X 的名称，并写入txt
    /// </summary>
    /// <param name="dirPath">文件路径</param>
    /// <param name="extension">扩展名</param>
    /// <param name="txtFileName">要写入的txt文件名</param>
    /// <param name="tips">提示信息</param>
    /// <param name="isHave">名称中是否还含有X</param>
    public void CheckBigXName(string dirPath, string extension, string txtFileName, string tips, bool isHave = false)
    {
        var files = Directory.GetFiles(dirPath, extension, SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.Contains("X"))
                {
                    isHave = true;
                    WriteToTxt(TxtDirPath, txtFileName, fileName);

                    //替换文件名（可用，暂时注释）
                    // var newName = fileName.Replace("X", "x");
                    // File.Move(file, Path.GetDirectoryName(file) + "\\" + newName + Path.GetExtension(file));
                }
            }
        }

        if (!isHave) Debug.Log(tips);
    }

    /// <summary>
    /// 检查是否含有同名的 fbx 文件
    /// </summary>
    public void CheckSameName(int index = 0)
    {
        int                     tempCount   = 0;
        int                     allCount    = 0;
        var                     filePath    = "D:/Users/suxing/Desktop/高华发的/模型";
        Dictionary<int, string> dictionary  = new Dictionary<int, string>();
        var                     prefabFiles = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories);
        foreach (var file in prefabFiles)
        {
            var fileExtension = Path.GetExtension(file);
            if (Equals(fileExtension, ".fbx") || Equals(fileExtension, ".obj"))
            {
                allCount++;

                var fileName = Path.GetFileNameWithoutExtension(file);

                if (!dictionary.ContainsValue(fileName))
                {
                    dictionary.Add(index++, fileName);
                    WriteToTxt(TxtDirPath, "所有建模已发模型名称汇总", fileName);
                }
                else
                {
                    tempCount++;
                    WriteToTxt(TxtDirPath, "已发模型的同名fbx文件", fileName);
                }
            }
        }
        Debug.Log("不重名的有多少个："    + dictionary.Count);
        Debug.Log("重名的有多少个："     + tempCount);
        Debug.Log("总的文件个数（含重名）：" + allCount);
    }

    /// <summary>
    /// 检查分配错颗粒大类文件夹的fbx文件
    /// </summary>
    public void CheckTypeError()
    {
        // 先从 json 中读取数据
        using (TextReader tr = File.OpenText(PrimaryJsonPath))
        {
            var jsonRootData = JsonMapper.ToObject<RootData>(tr.ReadToEnd());
            tr.Close();
        }

        var dirInfo = new DirectoryInfo(FbxPath);
        // 获取fbx所在的文件夹名称，如高一粒在文件夹"方高类"
        var allDirectory = dirInfo.GetDirectories();
        // 存放获取到的所有文件夹名称
        var allDirNameList = new List<string>();
        foreach (var directory in allDirectory)
        {
            if (directory.Name.Contains("中颗粒") || directory.Name.Contains("生化机械类")) continue;
            allDirNameList.Add(directory.Name);
        }

        foreach (var dirName in allDirNameList)
        {
            // 获取到单个文件夹下的所有 fbx 文件
            var files = Directory.GetFiles(FbxPath + "/" + dirName, "*.fbx", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fbxName = Path.GetFileNameWithoutExtension(file);
            }
            return;
        }

        // StreamReader sr = new FileInfo(PrimaryDataPath).OpenText();
        // var jsonRootData = JsonMapper.ToObject<RootData>(sr.ReadToEnd());
        // sr.Close();
        // sr.Dispose();
    }

    public static JustTest Instance()
    {
        return instance ?? (instance = new JustTest());
    }
}
