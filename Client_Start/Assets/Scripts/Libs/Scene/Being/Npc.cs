namespace SDK.Lib
{		
	/**
	 * @brief ���� NPC
	 */
	public class Npc : BeingEntity 
	{
		public Npc()
            : base()
		{
            //m_skinAniModel.m_modelList = new SkinSubModel[(int)eNpcModelType.eModelTotal];
            //int idx = 0;
            //while (idx < (int)eNpcModelType.eModelTotal)
            //{
            //    m_skinAniModel.m_modelList[idx] = new SkinSubModel();
            //    ++idx;
            //}
		}

        override public void dispose()
        {
            base.dispose();
            Ctx.m_instance.m_npcMgr.removeEntity(this);
        }
    }
}