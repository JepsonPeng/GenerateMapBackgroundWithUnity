using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerateGoogleMapWindow : EditorWindow
{
    string googleMapPosition;
    string GoogleMapPosition
    {
        set
        {
            if (googleMapPosition != value)
            {
                googleMapPosition = value;
                GenerateGoogleMapController.GetAllTextures(value);
                //解决每次路径选择完毕后在点击其他输入框时才显示出选择路径的问题
                GoogleMapPosition = GoogleMapPosition;
                ClickButtonState = false;
            }
        }
        get
        {
            return googleMapPosition;
        }
    }
    string GenerateMaterialPosition = "";

    float lengthPerPixel;
    float LengthPerPixel
    {
        set
        {
            if (lengthPerPixel != value)
            {
                lengthPerPixel = value;
                LengthPerPixel = LengthPerPixel;
                ClickButtonState = false;
                GenerateGoogleMapController.LengthPerPixel = value;
            }
        }
        get
        {
            return lengthPerPixel;
        }
    }

    string LengthPerPixelString = "";

    string MapObjectName = "GeneratedMaps";


    string ClickGenerateButtonTipInfo = "";


    bool ClickButtonState = false;

    void OnInspectorUpdate() //更新
    {
        Repaint();  //重新绘制
    }


    GenerateGoogleMapWindow()
    {
        this.titleContent = new GUIContent("Google瓦片地图生成器");
    }


    [MenuItem("Tools/Google瓦片地图生成器")]
    private static void GenerateGoogleMap()
    {
        EditorWindow.GetWindow(typeof(GenerateGoogleMapWindow));
    }


 

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Space(10);

        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 20;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Google瓦片地图生成器", titleStyle);

        GUILayout.Space(10);


        #region 选择瓦片地图地址
        GUILayout.BeginHorizontal();
        GUILayout.Label("瓦片地图地址:", GUILayout.Width(75));
        GoogleMapPosition = EditorGUILayout.TextField("", GoogleMapPosition);
        if (GUILayout.Button("...",GUILayout.Width(30)))
        {
            GoogleMapPosition = EditorUtility.OpenFolderPanel("选择瓦片地图地址", UnityEngine.Application.dataPath, "");
        }
        GUILayout.EndHorizontal();




        GUIStyle TipInfoStyle = new GUIStyle();
        TipInfoStyle.alignment = TextAnchor.MiddleCenter;
        if (GenerateGoogleMapController.AllMapTextures != null && GenerateGoogleMapController.AllMapTextures.Count != 0 && GenerateGoogleMapController.AllMapTextures[0] != null && GenerateGoogleMapController.AllMapTextures[0].Count != 0)
        {
            TipInfoStyle.normal.textColor = Color.blue;
            GUILayout.Space(5);
            GUILayout.Label("共检索到" + GenerateGoogleMapController.AllMapTextures.Count + "x" + GenerateGoogleMapController.AllMapTextures[0].Count + " = " + GenerateGoogleMapController.AllMapTextures.Count * GenerateGoogleMapController.AllMapTextures[0].Count + "张瓦片" + "    瓦片分辨率为:" + GenerateGoogleMapController.AllMapTextures[0][0].Map.width + "x" + GenerateGoogleMapController.AllMapTextures[0][0].Map.height, TipInfoStyle);
            GenerateGoogleMapController.MapSize = new Vector2(GenerateGoogleMapController.AllMapTextures[0][0].Map.width, GenerateGoogleMapController.AllMapTextures[0][0].Map.height);
            GUILayout.Space(5);
            TipInfoStyle.normal.textColor = Color.red;
        }
        else
        {
            TipInfoStyle.normal.textColor = Color.red;
            GUILayout.Space(5);
            GUILayout.Label("未检索到瓦片！",TipInfoStyle);
            GUILayout.Space(5);
        }


        #endregion

        GUILayout.BeginHorizontal();

        GUIStyle TextFieldStyle = new GUIStyle(EditorStyles.label);

        bool inputError = false;
        GUILayout.Label("每像素代表的实际距离:", GUILayout.Width(120));

        LengthPerPixelString = EditorGUILayout.TextField("", LengthPerPixelString, GUILayout.Width(50));

        try
        {
            LengthPerPixel = float.Parse(LengthPerPixelString);
            inputError = false;
        }
        catch (System.Exception)
        {
            inputError = true;
        }
        GUILayout.Label("米", GUILayout.Width(15));

        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        if (inputError == true)
        {
            GUILayout.Label("请重新输入数字！", TipInfoStyle,GUILayout.Width(185));
            GUILayout.Space(5);
        }


        #region 选择材质保存位置
        GUILayout.BeginHorizontal();

        GUILayout.Label("材质保存地址:",GUILayout.Width(75));
        GenerateMaterialPosition = EditorGUILayout.TextField("", GenerateMaterialPosition);

        if (GUILayout.Button("...",GUILayout.Width(30)))
        {
            GenerateMaterialPosition = EditorUtility.OpenFolderPanel("选择材质保存地址", UnityEngine.Application.dataPath, "");
            GenerateGoogleMapController.MaterialSavePath = GenerateMaterialPosition;
        }



        GUILayout.EndHorizontal();
        #endregion



        
        if (GUILayout.Button("生成地图瓦片"))
        {
            ClickGenerateButtonTipInfo = "";
            if (string.IsNullOrEmpty(GoogleMapPosition))
            {
                ClickGenerateButtonTipInfo += "请选择瓦片地图地址\n";
            }
            if (LengthPerPixel == 0)
            {
                ClickGenerateButtonTipInfo += "请输入瓦片每像素代表的距离\n";
            }
            if (string.IsNullOrEmpty(GenerateMaterialPosition))
            {
                ClickGenerateButtonTipInfo += "请选择材质保存地址\n";
            }
            ClickButtonState = true;
            GenerateGoogleMapController.StartGenerateGoogleMap();
        }
        if (!string.IsNullOrEmpty(ClickGenerateButtonTipInfo))
        {
            GUILayout.Label(ClickGenerateButtonTipInfo, TipInfoStyle);
        }
        if (string.IsNullOrEmpty(ClickGenerateButtonTipInfo) && ClickButtonState)
        {
            if (GenerateGoogleMapController.AllMapTextures == null || GenerateGoogleMapController.AllMapTextures.Count == 0 || GenerateGoogleMapController.AllMapTextures[0].Count == 0)
            {
                return;
            }


            GUILayout.Space(5);
            Rect ProcessRect = GUILayoutUtility.GetRect(50, 20);

            float process = GenerateGoogleMapController.CurrentGenerateCountNum / (GenerateGoogleMapController.AllMapTextures.Count * GenerateGoogleMapController.AllMapTextures[0].Count);


            if (process != 1f)
            {
                EditorGUI.ProgressBar(ProcessRect, process, "生成进度");
            }
            else
            {
                EditorGUI.ProgressBar(ProcessRect, process, "完成");
            }
        }


        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();

    }


}
