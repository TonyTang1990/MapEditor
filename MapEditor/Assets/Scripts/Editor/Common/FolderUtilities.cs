/*
 * Description:             FolderUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/20
 */

using System.IO;
/// <summary>
/// FolderUtilities.cs
/// 目录静态工具类
/// </summary>
public static class FolderUtilities
{
    /// <summary>
    /// 检查指定目录是否存在，不存在创建一个
    /// </summary>
    /// <param name="folderPath"></param>
    public static void CheckAndCreateSpecificFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}

