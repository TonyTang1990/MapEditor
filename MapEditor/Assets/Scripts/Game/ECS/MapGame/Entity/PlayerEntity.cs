/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:05
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:38:56
 * @ Description:
 */

/// <summary>
/// PlayerEntity.cs
/// ���Entity
/// </summary>
public class PlayerEntity : BaseActorEntity
{
    /// <summary>
    /// ��ʼ��Entity����
    /// </summary>
    protected override void InitEntityType()
    {
        EntityType = EntityType.Player;
    }
}