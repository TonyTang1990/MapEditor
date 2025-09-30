/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-17 16:54:54
* @ Description:
*
*/

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// WorldManager.cs
/// 世界管理单例类(相当于Tiny)
/// </summary>
public class WorldManager : SingletonTemplate<WorldManager>
{
    /// <summary>
    /// 所有世界Map<世界名, 世界>
    /// </summary>
    private Dictionary<string, BaseWorld> mAllWorldMap;

    /// <summary>
    /// 所有世界列表
    /// Note:
    /// 用于确保确定性更新顺序
    /// </summary>
    private List<BaseWorld> mAllWorlds;

    /// <summary>
    /// 所有更新的世界名列表
    /// </summary>
    private List<string> mAllUpdateWorldNames;

    public WorldManager()
    {
        mAllWorldMap = new Dictionary<string, BaseWorld>();
        mAllWorlds = new List<BaseWorld>();
        mAllUpdateWorldNames = new List<string>();
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        UpdateAllUpdateWorldNames();
        foreach(var updateWorldName in mAllUpdateWorldNames)
        {
            var world = GetWorld<BaseWorld>(updateWorldName);
            world?.Update(deltaTime);
        }
    }

    /// <summary>
    /// LogicUpdate
    /// </summary>
    /// <param name="logicFrameTime"></param>
    public void LogicUpdate(float logicFrameTime)
    {
        UpdateAllUpdateWorldNames();
        foreach (var updateWorldName in mAllUpdateWorldNames)
        {
            var world = GetWorld<BaseWorld>(updateWorldName);
            world?.LogicUpdate(logicFrameTime);
        }
    }

    /// <summary>
    /// FixedUpdate
    /// </summary>
    /// <param name="fixedDeltaTime"></param>
    public void FixedUpdate(float fixedDeltaTime)
    {
        UpdateAllUpdateWorldNames();
        foreach (var updateWorldName in mAllUpdateWorldNames)
        {
            var world = GetWorld<BaseWorld>(updateWorldName);
            world?.FixedUpdate(fixedDeltaTime);
        }
    }


    /// <summary>
    /// LateUpdate
    /// </summary>
    /// <param name="deltaTime"></param>
    public void LateUpdate(float deltaTime)
    {
        UpdateAllUpdateWorldNames();
        foreach (var updateWorldName in mAllUpdateWorldNames)
        {
            var world = GetWorld<BaseWorld>(updateWorldName);
            world?.LateUpdate(deltaTime);
        }
    }

    /// <summary>
    /// 创建指定类型和名字的世界
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="worldBasicData"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public T CreateWrold<T>(WorldBasicData worldBasicData, params object[] parameters) where T : BaseWorld, new()
    {
        var worldName = worldBasicData.WorldName;
        var existWorld = GetWorld<T>(worldName);
        if(existWorld != null)
        {
            var worldType = typeof(T);
            Debug.LogError($"已存在世界类型:{worldType.Name}和世界名:{worldBasicData}的世界，创建世界失败！");
            return null;
        }
        var newWorld = new T();
        newWorld.Init(worldBasicData, parameters);
        var result = AddWorld(newWorld);
        if(result)
        {
            newWorld.OnCreate();
        }
        return newWorld;
    }

    /// <summary>
    /// 移除指定世界名世界
    /// </summary>
    /// <param name="worldName"></param>
    /// <returns></returns>
    public bool RemoveWorld(string worldName)
    {
        var targetWorld = GetWorld<BaseWorld>(worldName);
        if(targetWorld == null)
        {
            Debug.LogError($"找不到世界名:{worldName}的世界，移除世界失败！");
            return false;
        }
        mAllWorldMap.Remove(worldName);
        mAllWorlds.Remove(targetWorld);
        targetWorld.OnDestroy();
        return true;
    }

    /// <summary>
    /// 获取指定类型和世界名的世界
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="worldName"></param>
    /// <returns></returns>
    public T GetWorld<T>(string worldName) where T : BaseWorld
    {
        BaseWorld targetWorld;
        if(!mAllWorldMap.TryGetValue(worldName, out targetWorld))
        {
            return null;
        }
        return targetWorld as T;
    }

    /// <summary>
    /// 添加指定世界对象
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    protected bool AddWorld(BaseWorld world)
    {
        if(world == null)
        {
            Debug.LogError($"不允许添加空世界！");
            return false;
        }
        var worldName = world.WorldName;
        var targetWorld = GetWorld<BaseWorld>(worldName);
        if (targetWorld != null)
        {
            Debug.LogError($"已包含世界名:{worldName}的世界，添加世界失败！");
            return false;
        }
        mAllWorldMap.Add(worldName, world);
        mAllWorlds.Add(world);
        return true;
    }

    /// <summary>
    /// 更新所有需要更新的世界名列表
    /// </summary>
    protected void UpdateAllUpdateWorldNames()
    {
        mAllUpdateWorldNames.Clear();
        foreach(var world in mAllWorlds)
        {
            mAllUpdateWorldNames.Add(world.WorldName);
        }
    }
}