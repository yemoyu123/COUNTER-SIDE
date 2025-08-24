namespace NKC.UI.NPC;

public class NKCUINPCShop : NKCUINPCSpine
{
	protected override string LUA_ASSET_NAME => "LUA_NPC_STORE_SERINA_TEMPLET";

	protected override NPC_TYPE NPCType => NPC_TYPE.STORE_SERINA;

	public override void Init(bool bUseIdleAnimation = true)
	{
		if (base.m_dicNPCTemplet == null || base.m_dicNPCTemplet.Count == 0)
		{
			LoadFromLua();
		}
		m_bUseIdleAni = bUseIdleAnimation && base.m_dicNPCTemplet != null && base.m_dicNPCTemplet.ContainsKey(NPC_ACTION_TYPE.IDLE);
		base.Init(bUseIdleAnimation: true);
	}

	public static void PlayVoice(NPC_TYPE npcType, NPC_ACTION_TYPE npcActionType, bool bStopCurrentSound)
	{
		NKCNPCTemplet nPCTemplet = NKCUINPCBase.GetNPCTemplet(npcType, npcActionType);
		if (nPCTemplet != null)
		{
			NKCUINPCBase.PlayVoice(npcType, nPCTemplet, bStopCurrentSound);
		}
	}

	public override void PlayAni(eEmotion emotion)
	{
	}

	public void ShowText(string text = "")
	{
	}

	public override void DragEvent()
	{
	}
}
