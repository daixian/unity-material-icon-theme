using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// using Newtonsoft.Json;

namespace SimpleFolderIcon.Editor
{
    [CustomEditor(typeof(FolderIconSO))]
    public class FolderIconSOEditor : UnityEditor.Editor
    {
        string searchTextFN = ""; // 当前输入框中的内容
        string searchTextSO = ""; // 当前输入框中的内容
        FolderIconSO searchResult = null; // 结果缓存

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();


            FolderIconSO folderIcon = (FolderIconSO)target;

            EditorGUILayout.Space();
            // if (GUILayout.Button("添加默认值")) {
            //     // string textureName = folderIcon.icon?.texture.name;
            //     // 使用name去载入这个图片
            //      folderIcon.icon = AssetDatabase.LoadAssetAtPath<Sprite>(
            //         $"Packages/{IconDictionaryCreator.PackageName}/Arts/png/{folderIcon.name}.png");
            //
            //     // 在项目文件夹中搜索读取material-icons.json文件反序列化成VSCodeDto
            //     string json = AssetDatabase.LoadAssetAtPath<TextAsset>(
            //         $"Packages/{IconDictionaryCreator.PackageName}/Editor/material-icons.json").text;
            //     VSCodeDto vscodeDto = JsonConvert.DeserializeObject<VSCodeDto>(json);
            //
            //     //...
            // }


            if (GUILayout.Button("Check()")) {
                // folderIcon.OnValidate();
                if (folderIcon.folderNames != null) {
                    List<string> newList = folderIcon.folderNames.ConvertAll(x => x.ToLower());
                    newList = newList.Distinct().ToList(); // 去重
                    if (!newList.SequenceEqual(folderIcon.folderNames)) {
                        folderIcon.folderNames = newList;
                        EditorUtility.SetDirty(this);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
            if (GUILayout.Button("SaveAssets()")) {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("IconDictionaryCreator.BuildDictionary()")) {
                IconDictionaryCreator.BuildDictionary();
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box"); // 带边框的区域
            EditorGUILayout.LabelField("🔍 搜索", EditorStyles.boldLabel);
            // 1. 搜索框 + 搜索按钮
            EditorGUILayout.BeginHorizontal();
            searchTextFN = EditorGUILayout.TextField("按FolderName", searchTextFN);
            if (GUILayout.Button("搜索", GUILayout.Width(60))) {
                searchTextSO = null;
                // 点击搜索时执行搜索逻辑
                searchResult = IconDictionaryCreator.SearchFolderIconSOWithFolderName(searchTextFN);
                if (searchResult != null) {
                    // 自动选中资源
                    // Selection.activeObject = searchResult;
                    // 高亮显示该资源
                    EditorGUIUtility.PingObject(searchResult);
                }
            }
            EditorGUILayout.EndHorizontal();

            // 2. 搜索框 + 搜索按钮
            EditorGUILayout.BeginHorizontal();
            searchTextSO = EditorGUILayout.TextField("按FolderIconSO名", searchTextSO);
            if (GUILayout.Button("搜索", GUILayout.Width(60))) {
                searchTextFN = null;
                // 点击搜索时执行搜索逻辑
                searchResult = IconDictionaryCreator.SearchFolderIconSOWithSOName(searchTextSO);
                if (searchResult != null) {
                    // 自动选中资源
                    // Selection.activeObject = searchResult;
                    // 高亮显示该资源
                    EditorGUIUtility.PingObject(searchResult);
                }
            }
            EditorGUILayout.EndHorizontal();

            // 3. 显示搜索结果
            EditorGUILayout.LabelField("搜索结果:");
            if (searchResult == null) {
                EditorGUILayout.LabelField("（无匹配项）");
            }
            else {
                if (!string.IsNullOrEmpty(searchTextFN))
                    EditorGUILayout.LabelField($"{searchTextFN} ->" + searchResult.name);
                else
                    EditorGUILayout.LabelField($"{searchTextSO} ->" + searchResult.name);
            }

            EditorGUILayout.EndVertical();

            if (GUI.changed) {
                // EditorUtility.SetDirty(target);
                // AssetDatabase.SaveAssets();
                // IconDictionaryCreator.BuildDictionary();
            }
        }
    }
}
