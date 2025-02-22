/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:05
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:38:56
 * @ Description:
 */

/// <summary>
/// PlayerEntity.cs
/// 玩家Entity
/// </summary>
public class PlayerEntity : BaseActorEntity
{
    /// <summary>
    /// 初始化Entity类型
    /// </summary>
    protected override void InitEntityType()
    {
        EntityType = EntityType.Player;
    }
}