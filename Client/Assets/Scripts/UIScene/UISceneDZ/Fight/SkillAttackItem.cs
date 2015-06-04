﻿using Game.Msg;
using SDK.Common;

namespace Game.UI
{
    /**
     * @brief 技能攻击不会发生移动过去的情况，仅仅是将法术卡拖到场景中，然后释放，就出发技能，可能会有功能准备特效和攻击特效
     */
    public class SkillAttackItem : AttackItemBase
    {
        protected uint m_skillId;
        protected MList<uint> m_hurtIdList;     // 被击者 this id 列表

        public SkillAttackItem()
        {
            m_hurtIdList = new MList<uint>();
        }

        public MList<uint> hurtIdList
        {
            get
            {
                return m_hurtIdList;
            }
            set
            {
                m_hurtIdList = value;
            }
        }

        public uint skillId
        {
            get
            {
                return m_skillId;
            }
            set
            {
                m_skillId = value;
            }
        }

        override public void execAttack(SceneCardBase card)
        {
            card.behaviorControl.execAttack(this);
        }

        override public void initItemData(SceneCardBase att, SceneCardBase def, stNotifyBattleCardPropertyUserCmd msg)
        {
            base.initItemData(att, def, msg);

            m_skillId = msg.dwMagicType;
            foreach(var item in msg.defList)
            {
                m_hurtIdList.Add(item.qwThisID);
            }
        }
    }
}