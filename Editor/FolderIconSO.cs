using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleFolderIcon.Editor
{
    /// <summary>
    /// 自定义文件夹图标的ScriptableObject类，用于存储文件夹图标和对应的文件夹名称。
    /// </summary>
    // [CreateAssetMenu(fileName = "FolderIcon", menuName = "xuexue/编辑器/文件夹图标配置")]
    public class FolderIconSO : ScriptableObject
    {
        /// <summary>
        /// 文件夹图标纹理。
        /// </summary>
        // public Texture2D icon;
        public Sprite icon;

        /// <summary>
        /// 应用该图标的文件夹名称列表。
        /// </summary>
        public List<string> folderNames;

        public void OnValidate()
        {
        }
    }
}
