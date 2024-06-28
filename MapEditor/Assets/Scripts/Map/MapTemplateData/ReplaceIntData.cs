/*
 * Description:             ReplaceIntData.cs
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
    /// ReplaceIntData.cs
    /// 整形替换数据结构
    /// </summary>
    [Serializable]
    public class ReplaceIntData : BaseReplaceData<int>
    {
        public ReplaceIntData(int oldData, int newData) : base(oldData, newData)
        {

        }
    }
}
