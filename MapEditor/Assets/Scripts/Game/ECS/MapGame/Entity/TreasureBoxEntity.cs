/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:20
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:38:51
 * @ Description:
 */

/// <summary>
/// TreasureBoxEntity.cs
/// ����Entity
/// </summary>
public class TreasureBoxEntity : BaseActorEntity
{
    /// <summary>
    /// ��ʼ��Entity����
    /// </summary>
    protected override void InitEntityType()
    {
        EntityType = EntityType.TreasureBox;
    }
}