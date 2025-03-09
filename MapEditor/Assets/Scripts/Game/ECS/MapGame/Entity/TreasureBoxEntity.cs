/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:20
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-03-09 00:26:08
 * @ Description:
 */

/// <summary>
/// TreasureBoxEntity.cs
/// 宝箱Entity
/// </summary>
public class TreasureBoxEntity : BaseActorEntity
{
    /// <summary>
    /// 初始化Entity类型
    /// </summary>
    protected override void InitEntityType()
    {
        EntityType = EntityType.TreasureBox;
    }
}