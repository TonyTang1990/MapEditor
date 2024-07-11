/*
 * Description:             GameObjectStatus.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

namespace MapEditor
{
    /// <summary>
    /// 实体对象状态
    /// </summary>
    public enum GameObjectStatus
    {
        INVALIDE = -1,          // 无效状态
        Normal = 0,             // 普通GameObject
        Asset = 1,              // Asset
        PrefabInstance = 2,     // 预制件实例对象
        PrefabContent = 3,      // 预制件编辑模式对象
    }
}
