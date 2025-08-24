namespace NKC.UI.NPC;

public class NKCUINPCOperatorLena : NKCUINPCSpine
{
	protected override string LUA_ASSET_NAME => "LUA_NPC_OPERATOR_LENA_TEMPLET";

	protected override NPC_TYPE NPCType => NPC_TYPE.OPERATOR_LENA;

	public override void Init(bool bUseIdleAnimation = true)
	{
		if (base.m_dicNPCTemplet == null || base.m_dicNPCTemplet.Count == 0)
		{
			LoadFromLua();
		}
		m_bUseIdleAni = bUseIdleAnimation && base.m_dicNPCTemplet != null && base.m_dicNPCTemplet.ContainsKey(NPC_ACTION_TYPE.IDLE);
		base.Init(bUseIdleAnimation: true);
	}

	public static void PlayVoice(VOICE_TYPE type)
	{
		switch (type)
		{
		case VOICE_TYPE.VT_LOBBY_CONNECT:
			PlayVoice(NPC_TYPE.OPERATOR_LENA, NPC_ACTION_TYPE.LOBBY_CONNECT, bStopCurrentSound: true);
			break;
		case VOICE_TYPE.VT_LOBBY_RETURN:
			PlayVoice(NPC_TYPE.OPERATOR_LENA, NPC_ACTION_TYPE.LOBBY_RETURN, bStopCurrentSound: true);
			break;
		}
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
