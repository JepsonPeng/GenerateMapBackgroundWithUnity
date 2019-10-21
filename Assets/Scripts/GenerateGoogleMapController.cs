using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Google地图生成器
/// 在Editor中执行
/// </summary>
//[ExecuteInEditMode]
public class GenerateGoogleMapController : MonoBehaviour
{
    /// <summary>
    /// 地图大小(像素)
    /// </summary>
    public static Vector2 MapSize;

    /// <summary>
    /// 设置每个像素代表的距离
    /// </summary>
    public static float LengthPerPixel;


    /// <summary>
    /// 材质保存位置
    /// </summary>
    public static string MaterialSavePath;

    /// <summary>
    /// 获取到的所有贴图，每一行
    /// </summary>
    public static List<List<MapProperty>> AllMapTextures = new List<List<MapProperty>>();


    /// <summary>
    /// 当前生成的瓦片数量
    /// </summary>
    public static int CurrentGenerateCountNum = 0;


   



    /// <summary>
    /// 生成地图瓦片
    /// </summary>
    public static void StartGenerateGoogleMap()
    {
        CurrentGenerateCountNum = 0;
        GameObject container = GameObject.Find("GenerateMapContainer");
        if (container != null)
        {
            //删除Scene已有的地图瓦片
            DestroyChildren(container.transform);
        }
        else
        {
            container = new GameObject("GenerateMapContainer");
        }

        //有多少列——每行有多少个
        int ColumnCount = AllMapTextures.Count;

        //按列生成
        for (int column = 0; column < ColumnCount; column++)
        {

            //有多少行
            int RowCount = AllMapTextures[column].Count;

            //生成每一列的父物体
            GameObject parentObj = new GameObject(AllMapTextures[column][0].MapName.Split('_')[0]);
            parentObj.transform.SetParent(container.transform);

            //生成每一列中的子物体
            for (int Row = 0; Row < RowCount; Row++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                obj.transform.SetParent(parentObj.transform);
                //设置瓦片大小
                obj.transform.localScale = new Vector3(MapSize.x * LengthPerPixel / 10.0f, 1, MapSize.y * LengthPerPixel / 10.0f);
                //设置瓦片位置
                obj.transform.localPosition = -new Vector3(
                    (column + 0.5f - ColumnCount / 2.0f) * obj.transform.localScale.x,
                    0,
                    (RowCount / 2.0f - Row - 0.5f) * obj.transform.localScale.z) * 10f;
                //生成瓦片材质
                Material material = CreateMaterail(AllMapTextures[column][Row].MapName, AllMapTextures[column][Row].Map,
                    MaterialSavePath.Remove(0, MaterialSavePath.IndexOf("Assets")) + "\\" + AllMapTextures[column][Row].MapName + ".mat");
                //为瓦片物体赋值材质
                obj.GetComponent<Renderer>().material = material;
                //改变瓦片名称
                obj.name = material.name;

                CurrentGenerateCountNum++;
                //Debug.Log(column + ":" + Row);
            }
        }

    }

    /// <summary>
    /// 获取指定路径下的所有贴图(地图瓦片)
    /// </summary>
    /// <param name="mapPath"></param>
    public static void GetAllTextures(string mapPath)
    {
        if (string.IsNullOrEmpty(mapPath))
        {
            Debug.LogError("请输入路径！");
            return;
        }
        AllMapTextures = new List<List<MapProperty>>();

        DirectoryInfo directoryInfo = null;

        //获取路径下的所有文件夹
        try
        {
            directoryInfo = new DirectoryInfo(mapPath);
        }
        catch (Exception)
        {
            directoryInfo = null;
        }

        if (directoryInfo == null)
        {
            Debug.LogError("目录不存在！");
            return;
        }

        DirectoryInfo[] dirs = null;

        try
        {
            dirs = directoryInfo.GetDirectories();
        }
        catch (Exception)
        {
            Debug.LogError("目录不存在！");
            return;
        }

        if (dirs == null || dirs.Length == 0)
        {
            Debug.LogError("该目录下不存在存放瓦片的子文件夹！");
            return;
        }


        //遍历所有目录
        foreach (var dir in dirs)
        {
            //获取所有后缀为jpg的文件(仅遍历当前目录，子目录不遍历)
            FileInfo[] fileInfos = dir.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);


            List<MapProperty> textures = new List<MapProperty>();

            //获取当前路径下的所有Texture并指定AllTextures的属性
            foreach (var file in fileInfos)
            {
                //Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(file.FullName);

                //删除当前文件路径的前部(Assets之前的路径)，从而获取相对路径
                string path = file.FullName.Remove(0, file.FullName.IndexOf("Assets"));
                //获取当前路径下指定的Texture
                Texture texture = (Texture)AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
                //获取针对Texture生成的Material的名称
                string textureSaveName = dir.Name + "_" + file.Name.Replace(".jpg", "");

                //初始化TextureProperty属性
                MapProperty textureProperty = new MapProperty();
                textureProperty.Map = texture;
                textureProperty.MapName = textureSaveName;

                textures.Add(textureProperty);
            }
            if (textures != null && textures.Count != 0)
                AllMapTextures.Add(textures);
        }

    }

    /// <summary>
    /// 删除子物体
    /// </summary>
    /// <param name="container">父物体</param>
    private static void DestroyChildren(Transform container)
    {
        Transform[] trs = container.GetComponentsInChildren<Transform>();
        for (int i = trs.Length - 1; i >= 0; i--)
        {
            if (trs[i] != container)
            {
                DestroyImmediate(trs[i].gameObject);
            }
        }
    }


    /// <summary>
    /// 创建材质(Standard材质)
    /// </summary>
    /// <param name="matName">材质名称</param>
    /// <param name="texture">材质贴图</param>
    /// <param name="savePath">材质保存位置</param>
    /// <returns></returns>
    private static Material CreateMaterail(string matName, Texture texture, string savePath)
    {
        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = texture;
        material.SetFloat("_Glossiness", 0f);
        //创建材质
        AssetDatabase.CreateAsset(material, savePath);
        //更新Asset目录(没必要每个新生成的材质都刷新Asset目录)
        //AssetDatabase.Refresh();
        return material;
    }


}

/// <summary>
/// 地图瓦片属性
/// </summary>
public class MapProperty
{
    /// <summary>
    /// 贴图的保存名称
    /// </summary>
    public string MapName;

    /// <summary>
    /// 贴图
    /// </summary>
    public Texture Map;
}