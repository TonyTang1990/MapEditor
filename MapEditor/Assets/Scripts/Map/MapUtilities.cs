/*
 * Description:             MapUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System.IO;
using UnityEditor;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapUtilities.cs
    /// 地图编辑器工具类
    /// </summary>
    public static class MapUtilities
    {
        /// <summary>
        /// 加载或创建地图配置数据
        /// </summary>
        /// <returns></returns>
        public static MapSetting LoadOrCreateGameMapSetting()
        {
#if UNITY_EDITOR
            var mapSetting = AssetDatabase.LoadAssetAtPath<MapSetting>(MapConst.MapSettingSavePath);
            if(mapSetting != null)
            {
                Debug.Log($"加载MapSetting:{MapConst.MapSettingSavePath}");
            }
            else
            {
                mapSetting = ScriptableObject.CreateInstance<MapSetting>();
                var mapSettingAssetFullPath = PathUtilities.GetAssetFullPath(MapConst.MapSettingSavePath);
                var mapSettingAssetFolderFullPath = Path.GetDirectoryName(mapSettingAssetFullPath);
                if(!Directory.Exists(mapSettingAssetFolderFullPath))
                {
                    Directory.CreateDirectory(mapSettingAssetFolderFullPath);
                }
                AssetDatabase.CreateAsset(mapSetting, MapConst.MapSettingSavePath);
                AssetDatabase.SaveAssets();
                Debug.Log($"创建MapSetting:{MapConst.MapSettingSavePath}");
            }
            return mapSetting;
#else
            Debug.LogError($"非Editor不允许加载地图配置数据，加载地图配置数据失败！");
            return null;
#endif
        }

        /// <summary>
        /// 创建指定地图埋点数据类型，指定uid和指定位置的埋点数据
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <param name="uid"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static MapData CreateMapDataByType(MapDataType mapDataType, int uid, Vector3 position)
        {
            if (mapDataType == MapDataType.MONSTER)
            {
                return new MonsterMapData(uid, position);
            }
            else if(mapDataType == MapDataType.MONSTER_GROUP)
            {
                return new MonsterGroupMapData(uid, position);
            }
            else
            {
                return new MapData(uid, position);
            }
        }

        /// <summary>
        /// 指定GameObject根据指定碰撞器数据更新
        /// </summary>
        /// <param name="go"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static void UpdateColliderByColliderData(GameObject go, Vector3 center, Vector3 size, float radius = 0f)
        {
            // 有ColliderDataMono则根据对应信息更新碰撞器数据
            if(go == null)
            {
                return;
            }
            var boxCollider = go.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.center = center;
                boxCollider.size = size;
            }
            var sphereCollider = go.GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                sphereCollider.center = center;
                sphereCollider.radius = radius;
            }
        }

        /// <summary>
        /// 指定GameObject根据挂载的ColliderDataMono更新碰撞体数据
        /// </summary>
        /// <param name="go"></param>
        public static void UpdateColliderByColliderDataMono(GameObject go)
        {
            if(go == null)
            {
                return;
            }
            var colliderDataMono = go.GetComponent<ColliderDataMono>();
            if(colliderDataMono == null)
            {
                return;
            }
            if(colliderDataMono.ColliderType == ColliderType.BOX)
            {
                var boxCollider = go.GetOrAddComponet<BoxCollider>();
                boxCollider.center = colliderDataMono.Center;
                boxCollider.size = colliderDataMono.Size;
            }
            else if(colliderDataMono.ColliderType == ColliderType.SPHERE)
            {
                var sphereCollider = go.GetOrAddComponet<SphereCollider>();
                sphereCollider.center = colliderDataMono.Center;
                sphereCollider.radius = colliderDataMono.Radius;
            }
        }
    }
}