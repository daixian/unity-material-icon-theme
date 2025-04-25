using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SimpleFolderIcon.Editor
{
    /// <summary>
    /// 图标字典创建器类，用于在资源导入、删除或移动时重建图标字典。
    /// </summary>
    public class IconDictionaryCreator : AssetPostprocessor
    {
        public static readonly string PackageName = "com.xuexue.unity-material-icon-theme";

        /// <summary>
        /// 图标资源的路径。
        /// </summary>
        private static readonly string AssetsPath = $"{PackageName}/Icons";

        /// <summary>
        /// 存储文件夹名称和对应图标的字典。
        /// </summary>
        internal static Dictionary<string, Texture> IconDictionary;

        /// <summary>
        /// 在所有资源导入、删除或移动后调用，检查是否包含图标资源并重建字典。
        /// </summary>
        /// <param name="importedAssets">导入的资源路径数组。</param>
        /// <param name="deletedAssets">删除的资源路径数组。</param>
        /// <param name="movedAssets">移动的资源路径数组。</param>
        /// <param name="movedFromAssetPaths">移动前的资源路径数组。</param>
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!ContainsIconAsset(importedAssets) &&
                !ContainsIconAsset(deletedAssets) &&
                !ContainsIconAsset(movedAssets) &&
                !ContainsIconAsset(movedFromAssetPaths)) {
                return;
            }

            // 其实没什么必要自动判断然后BuildDictionary(),因为此时是对这个插件的开发,手动BuildDictionary也行.
            BuildDictionary();
        }

        /// <summary>
        /// 检查给定的资源路径数组中是否是在自己包的Icon文件夹下发生了变化.
        /// </summary>
        /// <param name="assets">资源路径数组。</param>
        /// <returns>如果包含图标资源则返回true，否则返回false。</returns>
        private static bool ContainsIconAsset(string[] assets)
        {
            try {
                foreach (string str in assets) {
                    if (ReplaceSeparatorChar(Path.GetDirectoryName(str)) == "Packages/" + AssetsPath) {
                        return true;
                    }
                }
            } catch (Exception e) {
            }
            // 随便的判断一下,不要因为异常输出错误.
            return false;
        }

        /// <summary>
        /// 替换路径中的分隔符字符，将反斜杠替换为正斜杠。
        /// </summary>
        /// <param name="path">原始路径。</param>
        /// <returns>替换分隔符后的路径。</returns>
        private static string ReplaceSeparatorChar(string path)
        {
            return path.Replace("\\", "/");
        }

        /// <summary>
        /// 重建图标字典，从指定路径加载图标资源并存储到字典中。
        /// </summary>
        internal static void BuildDictionary()
        {
            // Debug.Log("[IconDictionaryCreator] BuildDictionary");
            // dx:这里改为定义一个忽略大小写的字典.
            var dictionary = new Dictionary<string, Texture>(StringComparer.OrdinalIgnoreCase);

            var appDirPath = Application.dataPath.Replace("Assets", "Packages");
            var dir = new DirectoryInfo(appDirPath + "/" + AssetsPath);

            // 对所有的png做一个支持
            FileInfo[] info = dir.GetFiles("*.png");
            foreach (FileInfo f in info) {
                var texture = (Texture)AssetDatabase.LoadAssetAtPath($"Packages/{AssetsPath}/{f.Name}", typeof(Texture2D));
                dictionary.Add(Path.GetFileNameWithoutExtension(f.Name), texture);
            }

            // 对配置文件做一个支持
            FileInfo[] infoSO = dir.GetFiles("*.asset");
            foreach (FileInfo f in infoSO) {
                // 载入一个FolderIconSO
                FolderIconSO folderIconSO = (FolderIconSO)AssetDatabase.LoadAssetAtPath($"Packages/{AssetsPath}/{f.Name}", typeof(FolderIconSO));

                if (folderIconSO != null) {
                    if (folderIconSO.icon == null) {
                        Debug.LogWarning($"{folderIconSO.name}的icon为空，请检查");
                        continue;
                    }
                    var texture = (Texture)folderIconSO.icon.texture;

                    foreach (string folderName in folderIconSO.folderNames) {
                        if (!string.IsNullOrEmpty(folderName)) {
                            // dictionary.TryAdd(folderName, texture);
                            if (dictionary.ContainsKey(folderName)) {
                                Debug.LogWarning($"{folderIconSO.name}有重复的folderName={folderName}");
                            }
                            else
                                dictionary.Add(folderName, texture);
                        }
                    }
                }
            }

            IconDictionary = dictionary;
        }

        /// <summary>
        /// 搜索一个文件夹名称，如果存在则返回对应的FolderIconSO，否则返回null。
        /// </summary>
        /// <param name="name"></param>
        internal static FolderIconSO SearchFolderIconSOWithFolderName(string name)
        {
            var appDirPath = Application.dataPath.Replace("Assets", "Packages");
            var dir = new DirectoryInfo(appDirPath + "/" + AssetsPath);
            // 对配置文件做一个支持
            FileInfo[] infoSO = dir.GetFiles("*.asset");
            foreach (FileInfo f in infoSO) {
                // 载入一个FolderIconSO
                FolderIconSO folderIconSO = (FolderIconSO)AssetDatabase.LoadAssetAtPath($"Packages/{AssetsPath}/{f.Name}", typeof(FolderIconSO));

                if (folderIconSO != null) {
                    foreach (string folderName in folderIconSO.folderNames) {
                        if (folderName == name.ToLower()) {
                            return folderIconSO;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 搜索一个SO的名称，如果存在则返回对应的FolderIconSO，否则返回null。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static FolderIconSO SearchFolderIconSOWithSOName(string name)
        {
            var appDirPath = Application.dataPath.Replace("Assets", "Packages");
            var dir = new DirectoryInfo(appDirPath + "/" + AssetsPath);
            // 对配置文件做一个支持
            FileInfo[] infoSO = dir.GetFiles($"{name}.asset");
            foreach (FileInfo f in infoSO) {
                // 载入一个FolderIconSO
                FolderIconSO folderIconSO = (FolderIconSO)AssetDatabase.LoadAssetAtPath($"Packages/{AssetsPath}/{f.Name}", typeof(FolderIconSO));

                if (folderIconSO != null) {
                    return folderIconSO;
                }
            }
            return null;
        }
    }
}
