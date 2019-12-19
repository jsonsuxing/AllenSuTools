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
        // 从《初级颗粒数据》json 中读取数据，希望处理的功能
        AboutPrimaryData();

        // 比较建模发了模型，但在编码表里搜索不到的颗粒名称
        // CompareWrongFbxName();

        // 从《是否已上架数据》json 中读取数据，希望处理的功能
        // AboutIsAlreadyShelves();

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
    /// 从《初级颗粒数据》json 中读取数据，希望处理的功能
    /// </summary>
    public void AboutPrimaryData()
    {
        var jsonPath = Application.dataPath + "/AllenSuTools/Data/自己整理的表.txt";

        // 先从 json 中读取数据
        TextReader tr = File.OpenText(jsonPath);
        var rootData = JsonMapper.ToObject<YuanBiaoRoot>(tr.ReadToEnd());
        tr.Dispose();
        tr.Close();

        foreach (var yuanBiao in rootData.links)
        {
            if (yuanBiao.颗粒名称.IndexOf(" ")>=0)
            {
                Debug.Log(yuanBiao.颗粒名称);
            }
        }

        // if (!File.Exists(jsonPath))
        // {
        //     WindowTips(jsonPath + " 下没有《初级颗粒数据》");
        //     return;
        // }
        //
        // // 先从 json 中读取数据
        // TextReader tr       = File.OpenText(jsonPath);
        // var        rootData = JsonMapper.ToObject<RootData>(tr.ReadToEnd());
        // tr.Dispose();
        // tr.Close();


    }

    /// <summary>
    /// 从《是否已上架数据》json 中读取数据，希望处理的功能
    /// </summary>
    public void AboutIsAlreadyShelves()
    {
        string jsonPath = Application.dataPath + "/AllenSuTools/Data/是否已上架数据.txt";

        // 先从 json 中读取数据
        TextReader tr = File.OpenText(jsonPath);
        var jsonRootData = JsonMapper.ToObject<RootX>(tr.ReadToEnd());
        tr.Dispose();
        tr.Close();

        var granuleNameDic1 = new Dictionary<string, string>(); // 已上架的个数
        var granuleNameDic2 = new Dictionary<string, string>(); // 未上架的个数
        
        foreach (var primaryData in jsonRootData.links)
        {
            // 获取到所有已上架的颗粒名称
            if (Equals(primaryData.是否已上架, "1")) granuleNameDic1.Add(primaryData.总序, primaryData.全称);
            // 获取到所有未上架的颗粒名称
            else if (Equals(primaryData.是否已上架,"2")) granuleNameDic2.Add(primaryData.总序, primaryData.全称);
            else Debug.Log("不属于上架和未上架的：" + primaryData.全称);
        }

        Debug.Log("表格中显示已上架颗粒个数："+granuleNameDic1.Count);
        Debug.Log("零件库已上架颗粒个数：" + PrimaryGranuleList.Count);
        Debug.Log("零件库未上架颗粒个数：" + granuleNameDic2.Count);

        var index = 0;
        var primaryDic = new Dictionary<int,string>(); // 初级零件库颗粒名称的字典
        foreach (var granule in PrimaryGranuleList)
        {
            primaryDic.Add(index++,granule.name);
        }

        // 保证零件库字典和已上架颗粒的字典数据要完全一致，之前有不一致的，利用差集求两个字典的差值
        // var s = granuleNameDic1.Values.ToList().Except(primaryDic.Values.ToList());
        // foreach (var _ in s)
        // {
        //     Debug.Log(_);
        // }

        // 对已上架的数据进行处理
        foreach (var dic1 in granuleNameDic1)
        {
            // 检测零件库有(不包括组合颗粒)，但是表格没有的颗粒名称，也就是检查出了零件库中的错误名称
            if (!granuleNameDic1.ContainsValue(dic1.Value) && !dic1.Value.Contains("组合"))
            {
                WriteToTxt(TxtDirPath,"零件库有，但是表格没有的颗粒名称",dic1.Value);
            }
        }

        // 功能二：检测已上架到零件库，但是在表格中依然显示 ×
        foreach (var granule in PrimaryGranuleList)
        {
            if (granuleNameDic2.ContainsValue(granule.name))
            {
                Debug.Log("检测已上架到零件库，但是在表格中依然显示 × 的颗粒：" + granule.name);
            }
        }
    }

    /// <summary>
    /// 比较建模发了模型，但在编码表里搜索不到的颗粒名称
    /// </summary>
    public void CompareWrongFbxName()
    {
        var jsonPath = Application.dataPath + "/AllenSuTools/Data/建模已发模型名称.txt";

        // 先从 json 中读取数据
        TextReader tr = File.OpenText(jsonPath);
        var jsonRootData = JsonMapper.ToObject<JianMoRoot>(tr.ReadToEnd());
        tr.Dispose();
        tr.Close();

        //初级颗粒数据
        // var jsonPath1 = Application.dataPath + "/AllenSuTools/Data/初级颗粒数据.txt"; // 家用
        var jsonPath1 = Application.dataPath + "/A-SuXing/AllenSuTools/Data/初级颗粒数据.txt"; // 公用

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

    public static JustTest Instance()
    {
        return instance ?? (instance = new JustTest());
    }
}
