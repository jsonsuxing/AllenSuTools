// ========================================================
// 描述：
// 作者：Chinar 
// 创建时间：2019-12-19 15:47:13
// 版 本：1.0
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YuanBiao
{
    public string 总序 { get; set; }
    public string 颗粒名称 { get; set; }
}

public class YuanBiaoRoot
{
    public List<YuanBiao> links { get; set; }
}
