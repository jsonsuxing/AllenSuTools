// ========================================================
// 描述：
// 作者：Chinar 
// 创建时间：2019-12-17 22:16:23
// 版 本：1.0
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinksItem
{
    public string 总序 { get; set; }
    public string 全称 { get; set; }
    public string 是否已上架 { get; set; }
}

public class Root
{
    public List<LinksItem> links { get; set; }
}
