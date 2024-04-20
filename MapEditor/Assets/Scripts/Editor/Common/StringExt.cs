/*
 * Description:             StringExt.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/20
 */

/// <summary>
/// StringExt.cs
/// 字符串扩展类
/// </summary>
public static class StringExt
{
    /// <summary>
    /// 移除指定子字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="subStr"></param>
    /// <returns></returns>
    public static string RemoveSubStr(this string str, string subStr)
    {
        int i = str.IndexOf(subStr);
        if(i != -1)
        {
            return str.Remove(i, subStr.Length);
        }
        return str;
    }
}
