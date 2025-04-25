using System.Collections.Generic;

#if USE_NEWTONSOFTJSON
//"com.unity.nuget.newtonsoft-json": "3.2.1"
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SimpleFolderIcon.Editor
{
    /// <summary>
    /// 解析material-icons.json,这是vscode插件中的配置
    /// </summary>
    public class VSCodeDto
    {
        /// <summary>
        /// key是文件夹名，value是图标名
        /// </summary>
        public Dictionary<string, string> folderNames;


        /// <summary>
        /// 在Tools菜单栏显示
        /// </summary>
        [MenuItem("Tools/Material Folder Icons/使用vscode配置创建所有SO")]
        public static void CreateAllSettingsSO()
        {
            // 在项目文件夹中搜索读取material-icons.json文件反序列化成VSCodeDto
            string json = AssetDatabase.LoadAssetAtPath<TextAsset>(
                $"Packages/{IconDictionaryCreator.PackageName}/Editor/material-icons.json").text;
            VSCodeDto dto = JsonConvert.DeserializeObject<VSCodeDto>(json);

            // 遍历所有dto.folderNames, 创建FolderIconSO到Packages/com.xuexue.material-folder-icons/Icons文件夹中
            // 先整理到一个字典(key是icon的名字)
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            foreach (var kvp in dto.folderNames) {
                string iconName = kvp.Value;
                if (!iconName.StartsWith("folder-")) {
                    // 只处理folder-开头的icon,不然应该也盖不住原生的
                    continue;
                }
                if (!dict.ContainsKey(iconName)) {
                    dict.Add(iconName, new List<string>());
                }
                dict[iconName].Add(kvp.Key);
            }

            foreach (var kvp in dict) {
                string soPath = $"Packages/{IconDictionaryCreator.PackageName}/Icons/{kvp.Key}.asset";
                // 如果文件存在了就载入
                FolderIconSO folderIconSO = AssetDatabase.LoadAssetAtPath<FolderIconSO>(soPath);
                if (folderIconSO == null) {
                    folderIconSO = ScriptableObject.CreateInstance<FolderIconSO>();
                    AssetDatabase.CreateAsset(folderIconSO, soPath);
                }

                // 如果没有就载入
                if (folderIconSO.icon == null)
                    folderIconSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>(
                        $"Packages/{IconDictionaryCreator.PackageName}/Arts/png/{folderIconSO.name}.png");

                if (folderIconSO.folderNames == null) {
                    folderIconSO.folderNames = new List<string>();
                }
                // 调整所有folderIconSO.folderNames中的项为小写
                folderIconSO.folderNames = folderIconSO.folderNames.ConvertAll(x => x.ToLower());

                foreach (string folderName in kvp.Value) {
                    //如果文件夹名不存在就添加
                    if (!folderIconSO.folderNames.Contains(folderName.ToLower())) {
                        folderIconSO.folderNames.Add(folderName);
                    }
                }
                EditorUtility.SetDirty(folderIconSO);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif
