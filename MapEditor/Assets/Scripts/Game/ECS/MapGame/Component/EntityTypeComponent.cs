/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:16:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:16:00
* @ Description:
*
*/

/// <summary>
/// EntityTypeComponent.cs
/// Entity类型组件
/// </summary>
public class EntityTypeComponent : BaseComponent
{
    /// <summary>
    /// Entity类型
    /// </summary>
    public EntityType EntityType;

    /// <summary>
    /// 重置数据
    /// </summary>
    protected override void ResetDatas()
    {
        base.ResetDatas();
        EntityType = EntityType.Invalide;
    }
}
