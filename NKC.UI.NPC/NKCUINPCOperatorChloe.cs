using NKM;
using NKM.Templet;

namespace NKC.UI.NPC;

public class NKCUINPCOperatorChloe : NKCUINPCSpine
{
	private static string VOICE_ASSET_BUNDLE_NAME => "AB_NPC_VOICE_OPERATOR_CHLOE";

	protected override string LUA_ASSET_NAME => "LUA_NPC_OPERATOR_CHLOE_TEMPLET";

	protected override NPC_TYPE NPCType => NPC_TYPE.OPERATOR_CHLOE;

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

	public static NPC_ACTION_TYPE GetNPCActionType(NKMDiveSlot diveSlot)
	{
		if (diveSlot == null)
		{
			return NPC_ACTION_TYPE.NONE;
		}
		switch (diveSlot.SectorType)
		{
		case NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_EUCLID:
		case NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_EUCLID_HARD:
			return NPC_ACTION_TYPE.SELECT_WHITE;
		case NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_REIMANN:
		case NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_REIMANN_HARD:
			return NPC_ACTION_TYPE.SELECT_PURPLE;
		case NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_GAUNTLET:
		case NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_GAUNTLET_HARD:
			return NPC_ACTION_TYPE.SELECT_YELLOW;
		case NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_POINCARE:
		case NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_POINCARE_HARD:
			return NPC_ACTION_TYPE.SELECT_RED;
		case NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_BOSS:
		case NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_BOSS_HARD:
			return NPC_ACTION_TYPE.SELECT_CORE;
		default:
			return NPC_ACTION_TYPE.NONE;
		}
	}

	public override void DragEvent()
	{
	}
}
