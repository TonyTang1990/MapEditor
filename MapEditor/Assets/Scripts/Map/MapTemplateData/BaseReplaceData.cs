/*
 * Description:             BaseReplaceData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/06/26
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// BaseReplaceData.cs
    /// 替换数据结构抽象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class BaseReplaceData<T>
    {
        /// <summary>
        /// 老数据
        /// </summary>
        [Header("老数据")]
        public T OldData;

        /// <summary>
        /// 新数据
        /// </summary>
        [Header("新数据")]
        public T NewData;

        public BaseReplaceData()
        {

        }

        public BaseReplaceData(T oldData, T newData)
        {
            OldData = oldData;
            NewData = newData;
        }
}
