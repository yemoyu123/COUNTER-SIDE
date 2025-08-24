using NKM.Templet;
using UnityEngine.EventSystems;

namespace NKC.UI.NPC;

public class NKCUINPCProfessorOlivia : NKCUINPCSpine
{
	protected override string LUA_ASSET_NAME => "LUA_NPC_PROFESSOR_OLIVIA_TEMPLET";

	protected override NPC_TYPE NPCType => NPC_TYPE.PROFESSOR_OLIVIA;

	public override void Init(bool bUseIdleAnimation = true)
	{
		if (base.m_dicNPCTemplet == null || base.m_dicNPCTemplet.Count == 0)
		{
			LoadFromLua();
		}
		m_bUseIdleAni = bUseIdleAnimation && base.m_dicNPCTemplet != null && base.m_dicNPCTemplet.ContainsKey(NPC_ACTION_TYPE.IDLE);
		base.Init(bUseIdleAnimation: true);
	}

	public void PlayVoice(NKM_UNIT_STYLE_TYPE unitStyleType, NKCUILab.LAB_DETAIL_STATE labState)
	{
		switch (unitStyleType)
		{
		case NKM_UNIT_STYLE_TYPE.NUST_COUNTER:
			switch (labState)
			{
			case NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN:
				PlayAni(NPC_ACTION_TYPE.TRAINING_COUNTER);
				break;
			case NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
				PlayAni(NPC_ACTION_TYPE.ENHANCE_COUNTER);
				break;
			case NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
				PlayAni(NPC_ACTION_TYPE.LIMIT_BREAK_COUNTER);
				break;
			}
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SOLDIER:
			switch (labState)
			{
			case NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN:
				PlayAni(NPC_ACTION_TYPE.TRAINING_SOLDIER);
				break;
			case NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
				PlayAni(NPC_ACTION_TYPE.ENHANCE_SOLDIER);
				break;
			case NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
				PlayAni(NPC_ACTION_TYPE.LIMIT_BREAK_SOLDIER);
				break;
			}
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_MECHANIC:
			switch (labState)
			{
			case NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_SKILL_TRAIN:
				PlayAni(NPC_ACTION_TYPE.TRAINING_MECHANIC);
				break;
			case NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_ENHANCE:
				PlayAni(NPC_ACTION_TYPE.ENHANCE_MECHANIC);
				break;
			case NKCUILab.LAB_DETAIL_STATE.LDS_UNIT_LIMITBREAK:
				PlayAni(NPC_ACTION_TYPE.LIMIT_BREAK_MECHANIC);
				break;
			}
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
		EventTrigger eventTrigger = base.gameObject.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = base.gameObject.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Drag;
		entry.callback.AddListener(NKCSystemEvent.UI_SCEN_BG_DRAG);
		eventTrigger.triggers.Add(entry);
	}
}
