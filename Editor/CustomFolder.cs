using System.IO;
using UnityEditor;
using UnityEngine;

namespace SimpleFolderIcon.Editor
{
    /// <summary>
    /// 自定义文件夹图标类，用于在Unity编辑器中为特定文件夹绘制自定义图标。
    /// </summary>
    [InitializeOnLoad]
    public class CustomFolder
    {
        /// <summary>
        /// 静态构造函数，在类加载时初始化图标字典并注册项目窗口绘制事件。
        /// </summary>
        static CustomFolder()
        {
            IconDictionaryCreator.BuildDictionary();
            EditorApplication.projectWindowItemOnGUI += DrawFolderIcon;
        }

        /// <summary>
        /// 在项目窗口中绘制自定义文件夹图标。
        /// </summary>
        /// <param name="guid">文件夹的GUID。</param>
        /// <param name="rect">文件夹在项目窗口中的矩形区域。</param>
        static void DrawFolderIcon(string guid, Rect rect)
        {
            // 将GUID转换为实际路径
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var iconDictionary = IconDictionaryCreator.IconDictionary;

            // 检查路径是否为空、当前事件类型是否为重绘、路径是否为目录以及图标字典中是否能匹配该文件夹名称
            if (string.IsNullOrEmpty(path) ||
                Event.current.type != EventType.Repaint ||
                !File.GetAttributes(path).HasFlag(FileAttributes.Directory)) {
                return;
            }
            // 获取图标纹理
            if (!iconDictionary.TryGetValue(Path.GetFileName(path), out Texture texture)) {
                return;
            }

            // 根据矩形区域的高度和位置调整图标绘制区域(这个稍微扩大一点是为了把原生icon覆盖掉)
            // 并且我们的icon是正方形的,所以绘制的区域必须是一个正方形,
            // 以下这些微调参数仅仅适用于当前material icon素材
            Rect imageRect;
            // Debug.Log($"当前rect={rect}");

            if (rect.height > 20) {
                // 注意: 此时对应的是两列UI:大UI的时候
                // 这种模型下width<height,width最小是32,height会大于16
                // width=96  height=110
                // width=53  height=67
                imageRect = new Rect(rect.x + 2.5f / 96f * rect.width,
                    rect.y - 4f / 110f * rect.height, rect.width, rect.width + 6f / 110f * rect.height);
            }
            else if (rect.x >= 16) {
                // 注意: 此时对应的是1列UI模式:小UI
                // 此时x最小都是30,但是Assets这个顶着格子的会是16整数.所以选择16
                imageRect = new Rect(rect.x + 1, rect.y, rect.height - 1, rect.height);
            }
            else {
                // 注意: 此时对应的是两列UI模式:小UI,纵向一列的时候,此时x总是等于14
                // 此时height=16,width会远远大于16
                imageRect = new Rect(rect.x + 3f, rect.y - 1.5f, rect.height - 1f, rect.height + 2f);
            }

            // 绘制图标
            GUI.DrawTexture(imageRect, texture);
        }
    }
}
