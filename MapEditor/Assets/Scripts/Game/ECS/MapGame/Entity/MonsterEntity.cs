/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:13
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:38:53
 * @ Description:
 */

/// <summary>
/// MonsterEntity.cs
/// ����Entity
/// </summary>
public class MonsterEntity : BaseActorEntity
{
    /// <summary>
    /// ��ʼ��Entity����
    /// </summary>
    protected override void InitEntityType()
    {
        EntityType = EntityType.Monster;
    }
}