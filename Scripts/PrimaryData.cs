// ========================================================
// 描述：初级颗粒的基本数据
// 作者：苏醒
// 创建时间：2019-12-12 15:29:04
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryData
{
    /// <summary>
    /// 全编码，如：02.01.01.01.01.12高一粒
    /// </summary>
    public string FullCode;

    /// <summary>
    /// 初级颗粒种类，如：方高类
    /// </summary>
    public string GranuleType;

    /// <summary>
    /// 每个颗粒对应的序号
    /// </summary>
    public string ID;

    /// <summary>
    /// 乐高ID
    /// </summary>
    public string LegoID;

    /// <summary>
    /// 新编码，如：FG001
    /// </summary>
    public string NewCode;
}

public class RootData
{
    /// <summary>
    /// 初级颗粒数据
    /// </summary>
    public List<PrimaryData> primaryData;
}
