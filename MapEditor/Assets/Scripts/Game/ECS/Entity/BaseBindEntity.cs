/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:37:52
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-24 15:29:00
* @ Description:
*
*/

using UnityEngine;

/// <summary>
/// BaseBindEntity.cs
/// 带实体对象的Entity基类(无视寻路的物体)
/// </summary>
public abstract class BaseBindEntity : BaseEntity
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
    /// 是否同步实体逻辑位置
    /// </summary>
    public bool SyncPosition
    {
        get;
        set;
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
    /// 是否同步实体旋转
    /// </summary>
    public bool SyncRotation
    {
        get;
        set;
    }

    /// <summary>
    /// 绑定实体对象
    /// </summary>
    public GameObject BindGo
    {
        get;
        set;
    }

    /// <summary>
    /// 播放动画名
    /// </summary>
    public string PlayAnimName
    {
        get;
        private set;
    }

    /// <summary>
    /// 动画组件
    /// </summary>
    public Animator Animator
    {
        get;
        set;
    }

    /// <summary>
    /// 是否自动销毁绑定对象
    /// </summary>
    public bool IsAutoDestroyBindGo
    {
        get;
        private set;
    }

    public BaseBindEntity() : base()
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
        BindGo = null;
        IsAutoDestroyBindGo = false;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="parameters">初始化参数</param>
    public override void Init(params object[] parameters)
    {
        base.Init(parameters);
        BindGo = (GameObject)parameters[1];
        IsAutoDestroyBindGo = (bool)parameters[2];
    }

    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="posZ"></param>
    public void SetPosition(float posX, float posY, float posZ)
    {
        Position = new Vector3(posX, posY, posZ);
        SyncPosition = true;
    }

    /// <summary>
    /// 设置旋转
    /// </summary>
    /// <param name="rotationX"></param>
    /// <param name="rotationY"></param>
    /// <param name="rotationZ"></param>
    public void SetRotation(float rotationX, float rotationY, float rotationZ)
    {
        Rotation = new Vector3(rotationX, rotationY, rotationZ);
        SyncRotation = true;
    }

    /// <summary>
    /// 播放指定动画
    /// </summary>
    /// <param name="animName"></param>
    public void PlayAnim(string animName)
    {
        PlayAnimName = animName;
        if (Animator != null && !string.IsNullOrEmpty(PlayAnimName))
        {
            Animator.Play(PlayAnimName);
        }
    }

    /// <summary>
    /// 销毁实例
    /// </summary>
    protected virtual void DestroyInstance()
    {
        if (IsAutoDestroyBindGo && BindGo != null)
        {
            GameObject.Destroy(BindGo);
            BindGo = null;
        }
    }
}