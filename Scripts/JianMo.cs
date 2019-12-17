// ========================================================
// 描述：
// 作者：Chinar 
// 创建时间：2019-12-17 22:52:46
// 版 本：1.0
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JianMo
{
    public string 序号 { get; set; }
    public string 建模已发 { get; set; }
}

public class JianMoRoot
{
    public List<JianMo> links { get; set; }
}
