/*
 * Description:             MapTemplateStrategyData.cs
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
    /// MapTemplateStrategyData.cs
    /// 地图埋点模板策略数据
    /// </summary>
    [Serializable]
    public class MapTemplateStrategyData
    {
        /// <summary>
        /// 策略UID
        /// </summary>
        [Header("策略UID")]
        public int StrategyUID = 1;

        /// <summary>
        /// 策略名
        /// </summary>
        [Header("策略名")]
        public string StrategyName = "默认策略名";

        /// <summary>
        /// UID替换数据列表
        /// </summary>
        [Header("UID替换数据列表")]
        public List<ReplaceIntData> UIDReplaceDatas = new List<ReplaceIntData>();

        /// <summary>
        /// 怪物组ID替换数据列表
        /// </summary>
        [Header("怪物组ID替换数据列表")]
        public List<ReplaceIntData> MonsterGroupIdReplaceDatas = new List<ReplaceIntData>();
    
        public MapTemplateStrategyData(int strategyUID, string strategyName)
        {
            StrategyUID = strategyUID;
            StrategyName = strategyName;
        }

        /// <summary>
        /// 添加指定老UID和新UID的UID替换数据
        /// </summary>
        /// <param name="oldMonsterGroupId"></param>
        /// <param name="newMonsterGroupId"></param>
        /// <returns></returns>
        public bool AddUIDRepalceData(int oldMonsterGroupId, int newMonsterGroupId)
        {
            if(IsContainUIDReplaceData(oldMonsterGroupId))
            {
                Debug.LogError($"重复添加老UID:{oldMonsterGroupId}的UID替换规则，添加UID替换规则失败！");
                return false;
            }
            var uidReplaceData = new ReplaceIntData(oldMonsterGroupId, newMonsterGroupId);
            UIDReplaceDatas.Add(uidReplaceData);
            return true;
        }

        /// <summary>
        /// 删除指定索引的UID替换规则数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveUIDReplaceDataByIndex(int index)
        {
            var uidReplaceDataNum = UIDReplaceDatas.Count;
            if (index < 0 || index >= uidReplaceDataNum)
            {
                Debug.LogError($"索引:{index}超出UID替换规则索引范围:0-{uidReplaceDataNum - 1}，移除UID替换规则数据失败！")
                return false;
            }
            UIDReplaceDatas.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 获取指定老UID的替换数据
        /// </summary>
        /// <param name="oldUID"></param>
        /// <returns></returns>
        public ReplaceIntData GetUIDReplaceData(int oldUID)
        {
            foreach (var uidReplaceData in UIDReplaceDatas)
            {
                if (uidReplaceData.OldData == oldUID)
                {
                    return uidReplaceData;
                }
            }
            return null;
        }

        /// <summary>
        /// 是否包含指定老UID的UID替换数据
        /// </summary>
        /// <param name="oldUID"></param>
        /// <returns></returns>
        public bool IsContainUIDReplaceData(int oldUID)
        {
            foreach(var uidReplaceData in UIDReplaceDatas)
            {
                if(uidReplaceData.OldData == oldUID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加指定老怪物组ID和新怪物组ID的怪物组ID替换数据
        /// </summary>
        /// <param name="oldMonsterGroupId"></param>
        /// <param name="newMonsterGroupId"></param>
        /// <returns></returns>
        public bool AddMonsterGroupIdRepalceData(int oldMonsterGroupId, int newMonsterGroupId)
        {
            if (IsContainMonsterGroupIdReplaceData(oldMonsterGroupId))
            {
                Debug.LogError($"重复添加老怪物组ID:{oldMonsterGroupId}的怪物组ID替换规则，添加怪物组ID替换规则失败！");
                return false;
            }
            var uidReplaceData = new ReplaceIntData(oldMonsterGroupId, newMonsterGroupId);
            MonsterGroupIdReplaceDatas.Add(uidReplaceData);
            return true;
        }

        /// <summary>
        /// 删除指定索引的怪物组ID替换规则数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveMonsterGroupIdReplaceDataByIndex(int index)
        {
            var monsterGroupIdReplaceDataNum = MonsterGroupIdReplaceDatas.Count;
            if (index < 0 || index >= monsterGroupIdReplaceDataNum)
            {
                Debug.LogError($"索引:{index}超出怪物组ID替换规则索引范围:0-{monsterGroupIdReplaceDataNum - 1}，替换怪物组ID替换规则数据失败！")
                return false;
            }
            MonsterGroupIdReplaceDatas.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 获取指定老怪物组ID的替换数据
        /// </summary>
        /// <param name="oldMonsterGroupId"></param>
        /// <returns></returns>
        public ReplaceIntData GetMonsterGroupIdReplaceData(int oldMonsterGroupId)
        {
            foreach (var monsterGroupIdReplaceData in MonsterGroupIdReplaceDatas)
            {
                if (monsterGroupIdReplaceData.OldData == oldMonsterGroupId)
                {
                    return monsterGroupIdReplaceData;
                }
            }
            return null;
        }

        /// <summary>
        /// 是否包含指定老怪物组ID的怪物组ID替换数据
        /// </summary>
        /// <param name="oldMonsterGroupId"></param>
        /// <returns></returns>
        public bool IsContainMonsterGroupIdReplaceData(int oldMonsterGroupId)
        {
            foreach (var monsterGroupIdReplaceData in MonsterGroupIdReplaceDatas)
            {
                if (monsterGroupIdReplaceData.OldData == oldMonsterGroupId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
