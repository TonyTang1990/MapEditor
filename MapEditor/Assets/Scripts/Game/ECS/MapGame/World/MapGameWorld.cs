/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-21 17:55:00
* @ Description:
*/

using Cinemachine;
using MapEditor;
using UnityEngine;

/// <summary>
/// MapGameWorld.cs
/// 地图游戏世界
/// </summary>
public class MapGameWorld : BaseWorld
{
    public MapGameWorld() : base()
    {

    }

    /// <summary>
    /// 响应地图游戏世界创建
    /// </summary>
    public override void OnCreate()
    {
        base.OnCreate();
        CreateAllSystem();
    }

    /// <summary>
    /// 响应地图游戏世界销毁
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    /// <summary>
    /// 创建所有系统
    /// </summary>
    private void CreateAllSystem()
    {
        CreateSystem<InputControlSystem>();
        CreateSystem<PlayerSpawnSystem>();
        CreateSystem<MapObjectEntitySpawnSystem>();
        CreateSystem<MapObjectGoSpawnSystem>();
        CreateSystem<ActorSyncSystem>();
        CreateSystem<CameraFollowSystem>();
        // MapGameEntitySpawnSystem一来摄像机的位置改变，所以必须放在CameraFollowSystem后
        CreateSystem<MapGameEntitySpawnSystem>();
    }
}