using UnityEngine.EventSystems;

namespace NKC.UI.NPC;

public class NKCUINPCHangarNaHeeRin : NKCUINPCSpine
{
	protected override string LUA_ASSET_NAME => "LUA_NPC_HANGAR_NAHEERIN_TEMPLET";

	protected override NPC_TYPE NPCType => NPC_TYPE.HANGAR_NAHEERIN;

	public override void Init(bool bUseIdleAnimation = true)
	{
		if (base.m_dicNPCTemplet == null || base.m_dicNPCTemplet.Count == 0)
		{
			LoadFromLua();
		}
		m_bUseIdleAni = bUseIdleAnimation && base.m_dicNPCTemplet != null && base.m_dicNPCTemplet.ContainsKey(NPC_ACTION_TYPE.IDLE);
		base.Init(bUseIdleAnimation: true);
	}

	public static void PlayVoice(NPC_TYPE npcType, NPC_ACTION_TYPE npcActionType, bool bStopCurrentSound = true)
	{
		NKCNPCTemplet nPCTemplet = NKCUINPCBase.GetNPCTemplet(npcType, npcActionType);
		if (nPCTemplet != null)
		{
			NKCUINPCBase.PlayVoice(npcType, nPCTemplet, bStopCurrentSound);
		}
	}

	public override void PlayAni(eEmotion emotion)
	{
		base.PlayAni(emotion);
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
