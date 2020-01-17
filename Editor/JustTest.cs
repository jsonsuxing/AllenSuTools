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
using System.Text;
using ChinarX.ExtensionMethods;
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
        // AboutPrimaryData();

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
        if(granuleDic1.Count != 0) return;

        SelfTools.Instance().ShowLibraryData(); // 为 PrimaryGranuleList 赋值

        // json 文件路径
        var dataPath = Application.dataPath + "/AllenSuTools/Data/初级颗粒数据.txt";

        if (!File.Exists(dataPath))
        {
            WindowTips("JSON 文件不存在！！！");
            return;
        }
        
        // 先从 json 中读取数据
        TextReader tr = File.OpenText(dataPath);
        var primary = JsonMapper.ToObject<Primary>(tr.ReadToEnd());
        tr.Dispose();
        tr.Close();

        // 为相关字典赋值
        foreach (var primaryData in primary.PrimaryData)
        {
            if (Equals(primaryData.是否已上架, "0")) granuleDic0.Add(primaryData.总序, primaryData.全称); // 未上架颗粒名称字典
            else if (Equals(primaryData.是否已上架, "1")) granuleDic1.Add(primaryData.总序, primaryData.全称); // 已上架颗粒名称字典
            else
            {
                Debug.Log("均不属于《未上架》和《已上架》颗粒的名称：" + primaryData.全称);
            }
            // 建模没法的（不包含未上架的以及名称是"未记录该颗粒"）
            // if (Equals(primaryData.建模是否已发,"0") && (!Equals(primaryData.是否已上架, "0")))
            // {
            //     if (!Equals(primaryData.全称, "未记录该颗粒"))
            //     {
            //         WriteToTxt(TxtDirPath, "建模尚未发的颗粒名称", primaryData.全称);
            //     }
            // }
            if (Equals(primaryData.建模是否已发, "1")) ModelPersonHadSendDic.Add(primaryData.总序,primaryData.全称); // 建模已发模型名称字典
        }

        // 零件库中初级颗粒的名称字典
        var primaryGranuleNameDic = new Dictionary<int,string>();
        foreach (var granule in PrimaryGranuleList)
        {
            primaryGranuleNameDic.Add(granule.transform.GetSiblingIndex(), granule.name);
        }
       
        // 检测功能一：《零件库名称字典》要和《已上架颗粒名称字典》的颗粒名称要一致，如果有不一样的，则利用差集求两个字典的差值
        var diffNameGroup = granuleDic1.Values.ToList().Except(primaryGranuleNameDic.Values.ToList());
        var granuleNames = diffNameGroup as string[] ?? diffNameGroup.ToArray();
        if (granuleNames.Length != 0)
        {
            foreach (var granuleName in granuleNames)
            {
                Debug.Log("编码表有，但零件库没有的颗粒名称 " + granuleName);
            }
        }

        // 检测功能二：零件库初级颗粒的相关处理
        foreach (var granule in PrimaryGranuleList)
        {
            // 处理一：检查零件库中的错误名称(不包含组合颗粒)
            if (!granuleDic1.ContainsValue(granule.name) && !granule.name.Contains("组合"))
            {
                Debug.Log("零件库错误编码的颗粒名称：" + granule.name);
            }

            // 处理二：零件库已上架，但在编码表中依然显示 0 的颗粒：
            if (granuleDic0.ContainsValue(granule.name))
            {
                Debug.Log("零件库已上架，但在编码表中依然显示 0 的颗粒：" + granule.name);
            }
        }

        // 比较上次与本次待定颗粒的差值名称
        CompareDaiDingGranule();

        // 输出模型已发，但我这边还未做的颗粒名称
        //        foreach (var granuleName in DaiDingDic.Values)
        //        {
        //            if (ModelPersonHadSendDic.ContainsValue(granuleName))
        //            {
        //                WriteToTxt(TxtDirPath,"模型已发，但我这边还未做的颗粒名称",granuleName);
        //            }
        //        }
    }

    /// <summary>
    /// 比较上次与本次待定颗粒的差值名称
    /// </summary>
    public void CompareDaiDingGranule()
    {
        // json 文件路径 
        var dataPath = Application.dataPath + "/AllenSuTools/Data/待定颗粒数据.txt";

        TextReader tr      = File.OpenText(dataPath);
        var        daiDing = JsonMapper.ToObject<DaiDing>(tr.ReadToEnd());
        tr.Dispose();
        tr.Close();

        foreach (var daiDingData in daiDing.DaiDingList)
        {
            DaiDingDic.Add(daiDingData.序号,daiDingData.待定颗粒名称);
        }

        // 零件库中初级颗粒的名称字典
        var primaryGranuleDaiDingNameDic = new Dictionary<int, string>();
        foreach (var granule in PrimaryDaiDingList)
        {
            primaryGranuleDaiDingNameDic.Add(granule.transform.GetSiblingIndex(), granule.name);
        }

        var diffNameGroup = DaiDingDic.Values.ToList().Except(primaryGranuleDaiDingNameDic.Values.ToList());
        var granuleNames  = diffNameGroup as string[] ?? diffNameGroup.ToArray();
        if (granuleNames.Length != 0)
        {
            foreach (var granuleName in granuleNames)
            {
                WriteToTxt(TxtDirPath, "新取消的待定颗粒名称",granuleName);
            }
            System.Diagnostics.Process.Start(TxtDirPath);
        }
        else Debug.Log("待定颗粒个数没有发生改变");

        // 排查待定颗粒标识中建模已发但我未做的颗粒名称
        // var path = Application.dataPath + "/AllenSuTools/Data/建模已发模型名称.txt";
        // 先从 json 中读取数据
        // TextReader tr1           = File.OpenText(path);
        // var        jsonRootData = JsonMapper.ToObject<JianMoRoot>(tr1.ReadToEnd());
        // tr1.Dispose();
        // tr1.Close();

        // 检查模型已发但我未做的颗粒名称
        // var MoDic = new Dictionary<string,string>();
        // foreach (var rootData in jsonRootData.links)
        // {
        //     MoDic.Add(rootData.序号,rootData.建模已发);
        // }
        //
        // foreach (var granuleName in DaiDingDic.Values)
        // {
        //     if (MoDic.ContainsValue(granuleName))
        //     {
        //         WriteToTxt(TxtDirPath,"模型已发但我未做的颗粒名称",granuleName);
        //     }
        // }
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
        var jsonRootData1 = JsonMapper.ToObject<Primary>(tr1.ReadToEnd());
        tr1.Dispose();
        tr1.Close();

        var tempDic = new Dictionary<string, string>();
        foreach (var primaryData in jsonRootData1.PrimaryData)
        {
            tempDic.Add(primaryData.总序,primaryData.全称);
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
