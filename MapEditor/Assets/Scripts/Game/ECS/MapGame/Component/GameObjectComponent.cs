/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:16:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:16:00
* @ Description:
*
*/

using UnityEngine;

/// <summary>
/// GameObjectComponent.cs
/// GameObject组件
/// </summary>
public class GameObjectComponent : BaseComponent
{
    /// <summary>
    /// 实体对象
    /// </summary>
    public GameObject Go;

    /// <summary>
    /// 预制件资源路径
    /// </summary>
    public string PrefabPath;

    /// <summary>
    /// 重置数据
    /// </summary>
    protected override void ResetDatas()
    {
        base.ResetDatas();
        Go = null;
        PrefabPath = null;
    }
}
