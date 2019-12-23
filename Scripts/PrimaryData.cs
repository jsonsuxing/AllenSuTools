// ========================================================
// 描述：初级颗粒的基本数据
// 作者：苏醒
// 创建时间：2019年12月21日15:18:51
// ========================================================
using System.Collections.Generic;

public class PrimaryData
{
    /// <summary>
    /// 颗粒类别，如：方高类
    /// </summary>
    public string 类别;

    /// <summary>
    /// 颗粒总序号
    /// </summary>
    public string 总序;

    /// <summary>
    /// 五位新编码，如：FG001
    /// </summary>
    public string 新编码;

    /// <summary>
    /// 颗粒全称，如：02.01.01.01.01.12高一粒
    /// </summary>
    public string 全称;

    /// <summary>
    /// 颗粒对应乐高ID
    /// </summary>
    public string 乐高ID;

    /// <summary>
    /// 颗粒是否已上架，0 代表未上架，1 代表已上架
    /// </summary>
    public string 是否已上架;

    /// <summary>
    /// 建模是否已发，0 代表未发，1 代表已发
    /// </summary>
    public string 建模是否已发;
}

public class Primary
{
    /// <summary>
    /// 颗粒数据列表对象
    /// </summary>
    public List<PrimaryData> PrimaryData;
}
