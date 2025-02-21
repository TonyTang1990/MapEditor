/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:37:52
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:51:32
 * @ Description:
 */


/// <summary>
/// BaseActorEntity.cs
/// 带实体对象的Entity基类
/// </summary>
public abstract class BaseActorEntity : BaseEntity
{
    /// <summary>
    /// 逻辑位置
    /// </summary>
    public Vector3 Position
    {
        get;
        private set;
    }

    /// <summary>
    /// 逻辑旋转
    /// </summary>
    public Vector3 Rotation
    {
        get;
        private set;
    }

    /// <summary>
    /// 预制体路径
    /// </summary>
    public string PrefabPath
    {
        get;
        private set;
    }

    /// <summary>
    /// 实体对象
    /// </summary>
    public GameObject Go
    {
        get;
        set;
    }

    public BaseActorEntity() : base()
    {

    }

    /// <summary>
    /// 响应销毁
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
        DestroyInstance();
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    protected override void ResetDatas()
    {
        base.ResetDatas();
        Position = Vector3.zero;
        Rotation = Vector3.zero;
        PrefabPath = null;
        Go = null;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="parameters">初始化参数</param>
    public override void Init(params object[] parameters)
    {
        base.Init(parameters);
        PrefabPath = (string)parameters[1];
    }

    /// <summary>
    /// 销毁实例
    /// </summary>
    protected virtual void DestroyInstance()
    {
        if(Go != null)
        {
            PoolManager.Singleton.push(PrefabPath, Go);
            Go = null;
        }
    }
}