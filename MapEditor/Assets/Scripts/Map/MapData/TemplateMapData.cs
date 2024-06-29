/*
 * Description:             MapConst.cs
 * Author:                  TONYTANG
 * Create Date:             2024/06/29
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// TemplateMapData.cs
    /// 模板地图埋点数据
    /// </summary>
    [Serializable]
    public class TemplateMapData : MapData
    {
        /// <summary>
        /// 模板策略UID
        /// </summary>
        [Header("模板策略UID")]
        public int StrategyUID;

        /// <summary>
        /// 是否展开
        /// </summary>
        [Header("是否展开")]
        public bool IsUnfold;

        /// <summary>
        /// 带参构造函数1
        /// </summary>
        /// <param name="uid"></param>
        public TemplateMapData(int uid) : base(uid)
        {

        }

        /// <summary>
        /// 带参构造函数2
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="templateLocalPosition"></param>
        /// <param name="templateLocalRotation"></param>
        public TemplateMapData(int uid, Vector3 position, Vector3 rotation, Vector3? templateLocalPosition = null, Vector3? templateLocalRotation = null)
                                : base(uid, position, rotation, templateLocalPosition, templateLocalRotation)
        {

        }

        /// <summary>
        /// 复制自定义数据
        /// </summary>
        /// <param name="sourceMapData"></param>
        /// <returns></returns>
        public override bool CopyCustomData(MapData sourceMapData)
        {
            if (!base.CopyCustomData(sourceMapData))
            {
                return false;
            }
            var realSourceMapData = sourceMapData as TemplateMapData;
            StrategyUID = realSourceMapData.StrategyUID;
            IsUnfold = realSourceMapData.IsUnfold;
            return true;
        }
    }
}
