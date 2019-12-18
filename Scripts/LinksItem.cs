// ========================================================
// 描述：检测零件库已上架，但表格仍然打的是 X 的颗粒名称
// 作者：苏醒 
// 创建时间：2019-12-17 22:16:23
// 版 本：1.0
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinksItemX
{
    public string 总序 { get; set; }
    public string 全称 { get; set; }
    public string 是否已上架 { get; set; }
}

public class RootX
{
    public List<LinksItemX> links { get; set; }
}
