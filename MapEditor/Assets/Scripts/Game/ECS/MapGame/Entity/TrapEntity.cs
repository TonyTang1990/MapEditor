/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:26
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:38:49
 * @ Description:
 */

/// <summary>
/// TrapEntity.cs
/// ����Entity
/// </summary>
public class TrapEntity : BaseActorEntity
{
    /// <summary>
    /// ��ʼ��Entity����
    /// </summary>
    protected override void InitEntityType()
    {
        EntityType = EntityType.Trap;
    }
}