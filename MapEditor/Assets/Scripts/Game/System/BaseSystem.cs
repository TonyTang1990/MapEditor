/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:39:04
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:54:54
 * @ Description:
 */
 
// Note:
// 这里的BaseEntity和BaseSystem并非完整的ECS模式
// 只是设计上借鉴ECS的概念设计

/// <summary>
/// BaseSystem.cs
/// 逻辑系统基类抽象(相当于ECS里的S部分)
/// </summary>
public abstract class BaseSystem
{
    /// <summary>
    /// 系统名
    /// </summary>
    public string SystemName
    {
        get;
        private set;
    }
}