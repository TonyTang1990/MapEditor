/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:16:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:16:00
* @ Description:
*
*/

/// <summary>
/// GameObjectSyncComponent.cs
/// GameObject同步组件
/// </summary>
public class GameObjectSyncComponent : BaseComponent
{
    /// <summary>
    /// 是否同步实体逻辑位置
    /// </summary>
    public bool SyncPosition;

    /// <summary>
    /// 是否同步实体逻辑旋转
    /// </summary>
    public bool SyncRotation;

    /// <summary>
    /// 重置数据
    /// </summary>
    protected override void ResetDatas()
    {
        base.ResetDatas();
        SyncPosition = false;
        SyncRotation = false;
    }
}
