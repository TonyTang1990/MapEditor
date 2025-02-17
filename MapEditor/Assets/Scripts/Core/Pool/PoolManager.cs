/*
 * Description:             PoolManager.cs
 * Author:                  TONYTANG
 * Create Date:             2018/12/31
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Note:
// 设计是以string作为Key，存在潜在重复的可能性，注意Key的取名避免冲突

/// <summary>
/// 全局GameObject缓存单例管理类
/// </summary>
public class PoolManager : SingletonTemplate<PoolManager>
{

    /// <summary>
    /// 对象池挂载对象
    /// </summary>
    private Transform mObjectPoolParent;

    /// <summary>
    /// 对象池重用Map
    /// Key为GameObject名字
    /// Value为可重用的对象池对象列表
    /// </summary>
    private Dictionary<string, List<GameObject>> mObjectPoolMap;

    /// <summary>
    /// 空节点对象池名
    /// </summary>
    private const string EmptyGameObjectKeyName = "EmptyGameObjectPool";

    /// <summary>
    /// 空对象模板实例对象
    /// </summary>
    private GameObject mEmptyGameObjectTemplateInstance;

    public PoolManager()
    {
        mObjectPoolMap = new Dictionary<string, List<GameObject>>();
        mObjectPoolParent = new GameObject("ObjectPoolRoot").transform;
        Object.DontDestroyOnLoad(mObjectPoolParent.gameObject);
        mEmptyGameObjectTemplateInstance = new GameObject("EmptyGameObjectTemplateInstance");
        mEmptyGameObjectTemplateInstance.transform.SetParent(mObjectPoolParent);
        mEmptyGameObjectTemplateInstance.SetActive(false);
    }

    /// <summary>
    /// 弹出空节点对象
    /// </summary>
    /// <returns></returns>
    public GameObject popEmptyGo()
    {
        if (mObjectPoolMap.ContainsKey(EmptyGameObjectKeyName) && mObjectPoolMap[EmptyGameObjectKeyName].Count > 0)
        {
            var instance = mObjectPoolMap[EmptyGameObjectKeyName][0];
            mObjectPoolMap[EmptyGameObjectKeyName].RemoveAt(0);
            instance.SetActive(true);
            return instance;
        }
        else
        {
            if (!mEmptyGameObjectTemplateInstance.activeSelf)
            {
                mEmptyGameObjectTemplateInstance.SetActive(true);
            }
            var instance = GameObject.Instantiate(mEmptyGameObjectTemplateInstance) as GameObject;
            mEmptyGameObjectTemplateInstance.SetActive(false);
            return instance;
        }
    }

    /// <summary>
    /// 空节点对象入池
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public void pushEmptyGo(GameObject instance)
    {
        if (instance == null)
        {
            Debug.LogError("不能缓存为null的实例对象!");
            return;
        }

        if (mObjectPoolMap.ContainsKey(EmptyGameObjectKeyName))
        {
            mObjectPoolMap[EmptyGameObjectKeyName].Add(instance);
        }
        else
        {
            var objectlist = new List<GameObject>();
            objectlist.Add(instance);
            mObjectPoolMap.Add(EmptyGameObjectKeyName, objectlist);
        }
        instance.transform.SetParent(mObjectPoolParent);
        instance.SetActive(false);
    }

    /// <summary>
    /// 缓存特定实例对象到对象池
    /// </summary>
    /// <param name="poolName">缓存池名</param>
    /// <param name="instance">放入对象池的实例对象</param>
    public void push(string poolName, GameObject instance)
    {
        if (instance == null)
        {
            Debug.LogError("不能缓存为null的实例对象!");
            return;
        }

        if (mObjectPoolMap.ContainsKey(poolName))
        {
            mObjectPoolMap[poolName].Add(instance);
        }
        else
        {
            var objectlist = new List<GameObject>();
            objectlist.Add(instance);
            mObjectPoolMap.Add(poolName, objectlist);
        }
        instance.transform.SetParent(mObjectPoolParent);
        instance.SetActive(false);
    }

    /// <summary>
    /// 返回可用的实例对象
    /// </summary>
    /// <param name="prefabPath">预制件路径</param>
    /// <param name="loadCompleteCb">加载完成回调</param>
    /// <returns></returns>
    public bool pop(string prefabPath, Action<GameObject> loadCompleteCb = null)
    {
        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.LogError("不能弹出空预制件路径的实例对象!");
            loadCompleteCb?.Invoke(null);
            return false;
        }
        else
        {
            if (mObjectPoolMap.ContainsKey(prefabPath) && mObjectPoolMap[prefabPath].Count > 0)
            {
                var instance = mObjectPoolMap[prefabPath][0];
                mObjectPoolMap[prefabPath].RemoveAt(0);
                instance.SetActive(true);
                loadCompleteCb?.Invoke(instance);
                return true;
            }
            else
            {
                var prefabAsset = Resources.Load<GameObject>(prefabPath);
                if (prefabAsset == null)
                {
                    Debug.LogError($"找不到路径:{prefabPath}的资源，返回示例对象失败！", prefabPath));
                    loadCompleteCb?.Invoke(null);
                    return false;
                }
                if (!prefab.activeSelf)
                {
                    prefab.SetActive(true);
                }
                var instance = GameObject.Instantiate(prefab) as GameObject;
                prefab.SetActive(false);
                loadCompleteCb?.Invoke(instance);
                return true;
            }
        }
    }

    /// <summary>
    /// 清除特定预制件所缓存的实例对象
    /// </summary>
    /// <param name="poolName">缓存池名</param>
    public void clear(string poolName)
    {
        if (mObjectPoolMap.ContainsKey(poolName))
        {
            for (int i = 0; i < mObjectPoolMap[poolName].Count; i++)
            {
                GameObject.Destroy(mObjectPoolMap[poolName][i]);
                mObjectPoolMap[poolName][i] = null;
            }
            mObjectPoolMap[poolName] = null;
            mObjectPoolMap.Remove(poolName);
        }
        else
        {
            Debug.LogError(string.Format("找不到GameObject : {0}的缓存对象！", poolName));
        }
    }

    /// <summary>
    /// 清除所有缓存的对象
    /// </summary>
    public void clearAll()
    {
        foreach (var objectlist in mObjectPoolMap)
        {
            for (int i = 0; i < objectlist.Value.Count; i++)
            {
                GameObject.Destroy(objectlist.Value[i]);
                objectlist.Value[i] = null;
            }
            objectlist.Value.Clear();
        }
        mObjectPoolMap.Clear(); ;
    }
}
