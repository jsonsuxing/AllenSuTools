// ========================================================
// 描述：
// 作者：Chinar 
// 创建时间：2019-07-05 09:23:58
// 版 本：1.0
// ========================================================
using System.Collections;
using System.Collections.Generic;
using SRF;
using UnityEditor;
using UnityEngine;


public class SSSCreateGuanJians : MonoBehaviour
{
    public Bounds bound;
    float         baseLenth  = 0.80f; // 高一粒长宽
    float         baseHight  = 0.32f; // 泊高
    float         baseHight2 = 0.17f; // 凸起高

    Vector3 basePoint  = Vector3.zero; // 关键部位阵列中 基础的关键部位
    Vector3 basePoints = Vector3.zero; // 关键部位阵列中 基础的关键部位

    float x;
    float y;
    float z;

    string prefabFilePath  = "Assets/Resources/Prefab/ModelPrefabs/";
    string prefabFilePath2 = "Prefab/ModelPrefabs/";


    // Use this for initialization
    IEnumerator Start()
    {
        bound = GetBound(gameObject);
        x     = bound.size.x / baseLenth;
        z     = bound.size.z / baseLenth;
        y     = bound.size.y / baseHight;

        // 生成父物体
        GameObject father = new GameObject(); // 物体挂的空物体
        // 挂上颗粒脚本
        father.name     = gameObject.name;
        gameObject.name = "物件_1";
        //更换模型材质颜色
        gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Material/UseModelMat/材质_2");
        //我更改为原点
        father.transform.position = Vector3.zero;
        //father.transform.position = bound.center;
        transform.parent = father.transform;
        father.AddComponent<KeLiData>();
        father.GetComponent<KeLiData>().transparency = Resources.Load<Material>("Material/UseJoinPointMat/BanTouMing"); //sx更换模型材质为半透明

        GameObject cubeObject = new GameObject();
        cubeObject.name = "AoCao (1)";
        GuanJianBuWei guanJian = cubeObject.AddComponent<GuanJianBuWei>();
        guanJian.director = Director.Up;
        Vector3 position = basePoint;
        position.x                    = baseLenth;
        position.z                    = baseLenth;
        cubeObject.transform.position = position;
        cubeObject.transform.parent   = father.transform;

        // 自动生成预制体
        Object tempPrefab = PrefabUtility.CreateEmptyPrefab(prefabFilePath + father.name + ".prefab");
        PrefabUtility.ReplacePrefab(father, tempPrefab);
        GameObject prefab  = (GameObject) Resources.Load(prefabFilePath2 + father.name);
        GameObject keLi    = Instantiate(prefab);
        Renderer[] renders = keLi.transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in renders)
        {
            if (child.name.Contains("物件"))
            {
                SSSCreateGuanJians jiaoBen = child.GetComponent<SSSCreateGuanJians>();
                Destroy(jiaoBen);
                break;
            }
        }

        // 自动去掉生成关键部位的脚本
        yield return new WaitForSeconds(0.5F);
        PrefabUtility.ReplacePrefab(keLi, tempPrefab);
    }


    // 计算一般物体的包围盒，里面的所有物体都会被计算
    static public Bounds GetBound(GameObject obj)
    {
        Vector3    center  = Vector3.zero;
        Renderer[] renders = obj.transform.GetComponentsInChildren<Renderer>();
        int        number  = 0;
        foreach (Renderer child in renders)
        {
            center += child.bounds.center;
            number++;
        }

        center /= number;
        Bounds boundTmp = new Bounds(center, Vector3.zero);
        foreach (Renderer child in renders)
        {
            boundTmp.Encapsulate(child.bounds);
        }

        return boundTmp;
    }
}