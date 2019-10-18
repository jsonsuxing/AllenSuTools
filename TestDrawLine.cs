// ========================================================
// 描述：
// 作者：Chinar 
// 创建时间：2019-10-18 17:25:55
// 版 本：1.0
// ========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawLine : MonoBehaviour
{
    public Vector3 StartVector;
    public Vector3 EndVector;
    public Color Color;
    public void OnDrawGizmosSelected()
    {
        // if (StartVector != Vector3.zero && EndVector != Vector3.zero)
        // {
        //     Gizmos.color = Color;
        //     Gizmos.DrawLine(StartVector, EndVector);
        // }

        Gizmos.color = Color;
        Gizmos.DrawLine(StartVector, EndVector);
    }
}
