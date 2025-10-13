/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-10-13 10:55:00
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-10-13 10:55:00
 * @ Description:
 */

using System;
using System.Collections.Generic;

/// <summary>
/// BaseViewEntity.cs
/// Entity可视化基类抽象
/// </summary>
public class BaseViewEntity<T> : BaseEntity where T : BaseEntityView<BaseEntity>
{
}