/*
 * Description:             MapObjectConfig.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

namespace MapEditor
{
    /// <summary>
    /// MapObjectConfig.cs
    /// 地图对象数据配置
    /// </summary>
    [Serilizable]
    public class MapObjectConfig
    {
        /// <summary>
        /// 唯一ID(用于标识地图对象配置唯一)
        /// </summary>
        [Header("唯一ID")]
        public int UID;

        /// <summary>
        /// 地图对象类型
        /// </summary>
        [Header("地图对象类型")]
        public MapObjectType ObjectType;

        /// <summary>
        /// 是否是动态地图对象
        /// </summary>
        [Header("是否是动态地图对象")]
        public bool IsDynamic;

        /// <summary>
        /// 关联Id
        /// </summary>
        [Header("关联Id")]
        public int ConfId;

        /// <summary>
        /// 资源Asset
        /// </summary>
        [Header("资源Asset")]
        public GameObject Asset;

        /// <summary>
        /// 描述
        /// </summary>
        [Header("描述")]
        public string Des;

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="objectType"></param>
        /// <param name="isDynamic"></param>
        /// <param name="asset"></param>
        /// <param name="des"></param>
        public MapObjectConfig(int uid, MapObjectType objectType, bool isDynamic = false, GameObject asset = null, string des = "")
        {
            UID = uid;
            ObjectType = objectType;
            IsDynamic = isDynamic;
            Asset = asset;
            Des = des;
        }

        /// <summary>
        /// 获取地图对象配置选项名字
        /// </summary>
        /// <returns></returns>
        public string GetOptionName()
        {
            return $"{UID}({ObjectType.ToString()})({Des})";
        }
    }
}