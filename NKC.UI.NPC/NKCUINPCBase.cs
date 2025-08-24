using System.Collections.Generic;
using Cs.Logging;
using NKM;
using UnityEngine;

namespace NKC.UI.NPC;

public abstract class NKCUINPCBase : MonoBehaviour
{
	private const string LUA_ASSET_BUNDLE_NAME = "AB_SCRIPT_NPC";

	protected static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicStoreSerinaTemplet = new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>();

	protected static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicMachineGapTemplet = new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>();

	protected static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicOeratorLenaTemplet = new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>();

	protected static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicOeratorChloeTemplet = new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>();

	protected static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicProfessorOliviaTemplet = new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>();

	protected static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicFactoryAnastasiaTemplet = new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>();

	protected static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicManagerKimHaNaTemplet = new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>();

	protected static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicHangarNaHeeRinTemplet = new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>();

	protected static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicAssistantLeeYoonJung = new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>();

	protected static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicStoreSigmaTemplet = new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>();

	protected static int m_CurrentSoundUID;

	protected static NPC_TYPE prevNPCType;

	protected static Dictionary<NPC_ACTION_TYPE, float> m_dicCoolTimeInfo = new Dictionary<NPC_ACTION_TYPE, float>();

	protected abstract string LUA_ASSET_NAME { get; }

	protected abstract NPC_TYPE NPCType { get; }

	protected Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> m_dicNPCTemplet => GetNPCTempletDic(NPCType);

	public abstract void Init(bool bUseIdleAnimation = true);

	public static Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> GetNPCTempletDic(NPC_TYPE npcType)
	{
		return npcType switch
		{
			NPC_TYPE.STORE_SERINA => m_dicStoreSerinaTemplet, 
			NPC_TYPE.MACHINE_GAP => m_dicMachineGapTemplet, 
			NPC_TYPE.OPERATOR_LENA => m_dicOeratorLenaTemplet, 
			NPC_TYPE.OPERATOR_CHLOE => m_dicOeratorChloeTemplet, 
			NPC_TYPE.PROFESSOR_OLIVIA => m_dicProfessorOliviaTemplet, 
			NPC_TYPE.FACTORY_ANASTASIA => m_dicFactoryAnastasiaTemplet, 
			NPC_TYPE.MANAGER_KIMHANA => m_dicManagerKimHaNaTemplet, 
			NPC_TYPE.HANGAR_NAHEERIN => m_dicHangarNaHeeRinTemplet, 
			NPC_TYPE.ASSISTANT_LEEYOONJUNG => m_dicAssistantLeeYoonJung, 
			NPC_TYPE.STORE_SIGMA => m_dicStoreSigmaTemplet, 
			_ => new Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>>(), 
		};
	}

	protected void LoadFromLua()
	{
		NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT_NPC", LUA_ASSET_NAME))
		{
			return;
		}
		if (nKMLua.OpenTable("m_dicNPCTemplet"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKCNPCTemplet nKCNPCTemplet = new NKCNPCTemplet();
				if (nKCNPCTemplet.LoadLUA(nKMLua))
				{
					if (!m_dicNPCTemplet.ContainsKey(nKCNPCTemplet.m_ActionType))
					{
						m_dicNPCTemplet.Add(nKCNPCTemplet.m_ActionType, new List<NKCNPCTemplet>());
					}
					m_dicNPCTemplet[nKCNPCTemplet.m_ActionType].Add(nKCNPCTemplet);
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
	}

	public List<NKCNPCTemplet> GetNPCTempletList(NPC_ACTION_TYPE npcActionType)
	{
		return GetNPCTempletList(NPCType, npcActionType);
	}

	public static List<NKCNPCTemplet> GetNPCTempletList(NPC_TYPE npcType, List<NPC_ACTION_TYPE> lstNpcActionType)
	{
		Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> nPCTempletDic = GetNPCTempletDic(npcType);
		List<NKCNPCTemplet> list = new List<NKCNPCTemplet>();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		for (int i = 0; i < lstNpcActionType.Count; i++)
		{
			if (nPCTempletDic == null || !nPCTempletDic.ContainsKey(lstNpcActionType[i]))
			{
				continue;
			}
			List<NKCNPCTemplet> list2 = nPCTempletDic[lstNpcActionType[i]];
			for (int j = 0; j < list2.Count; j++)
			{
				NPC_CONDITION conditionType = list2[j].m_ConditionType;
				if ((uint)conditionType <= 1u || conditionType != NPC_CONDITION.TOTAL_PAY)
				{
					list.Add(list2[j]);
				}
				else if (nKMUserData?.m_ShopData.GetTotalPayment() >= (double)list2[j].m_ConditionValue)
				{
					list.Add(list2[j]);
				}
			}
		}
		return list;
	}

	public static List<NKCNPCTemplet> GetNPCTempletList(NPC_TYPE npcType, NPC_ACTION_TYPE npcActionType)
	{
		Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> nPCTempletDic = GetNPCTempletDic(npcType);
		List<NKCNPCTemplet> list = new List<NKCNPCTemplet>();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nPCTempletDic != null && nPCTempletDic.ContainsKey(npcActionType))
		{
			List<NKCNPCTemplet> list2 = nPCTempletDic[npcActionType];
			for (int i = 0; i < list2.Count; i++)
			{
				NPC_CONDITION conditionType = list2[i].m_ConditionType;
				if ((uint)conditionType <= 1u || conditionType != NPC_CONDITION.TOTAL_PAY)
				{
					list.Add(list2[i]);
				}
				else if (nKMUserData?.m_ShopData.GetTotalPayment() >= (double)list2[i].m_ConditionValue)
				{
					list.Add(list2[i]);
				}
			}
		}
		return list;
	}

	public NKCNPCTemplet GetNPCTemplet(NPC_ACTION_TYPE npcActionType)
	{
		return GetNPCTemplet(NPCType, npcActionType);
	}

	public static NKCNPCTemplet GetNPCTemplet(NPC_TYPE npcType, List<NPC_ACTION_TYPE> lstNpcActionType)
	{
		List<NKCNPCTemplet> nPCTempletList = GetNPCTempletList(npcType, lstNpcActionType);
		if (nPCTempletList.Count > 0)
		{
			int index = Random.Range(0, nPCTempletList.Count);
			return nPCTempletList[index];
		}
		return null;
	}

	public static NKCNPCTemplet GetNPCTemplet(NPC_TYPE npcType, NPC_ACTION_TYPE npcActionType)
	{
		List<NKCNPCTemplet> nPCTempletList = GetNPCTempletList(npcType, npcActionType);
		if (nPCTempletList.Count > 0)
		{
			int index = Random.Range(0, nPCTempletList.Count);
			return nPCTempletList[index];
		}
		return null;
	}

	private static string GetVoiceBundleName(NPC_TYPE npcType)
	{
		return npcType switch
		{
			NPC_TYPE.ASSISTANT_LEEYOONJUNG => "AB_NPC_VOICE_ASSISTANT_LEE_YOON_JUNG", 
			NPC_TYPE.FACTORY_ANASTASIA => "AB_NPC_VOICE_BLACKSMITH_ANASTASIA", 
			NPC_TYPE.HANGAR_NAHEERIN => "AB_NPC_VOICE_HANGAR_NAHEERIN", 
			NPC_TYPE.MACHINE_GAP => "AB_NPC_VOICE_MACHINE_GAP", 
			NPC_TYPE.MANAGER_KIMHANA => "AB_NPC_VOICE_MANAGER_KIMHANA", 
			NPC_TYPE.OPERATOR_CHLOE => "AB_NPC_VOICE_OPERATOR_CHLOE", 
			NPC_TYPE.OPERATOR_LENA => "AB_NPC_VOICE_OPERATOR_LENA", 
			NPC_TYPE.PROFESSOR_OLIVIA => "AB_NPC_VOICE_PROFESSOR_OLIVIA", 
			NPC_TYPE.STORE_SERINA => "AB_NPC_VOICE_STORE_SERINA", 
			_ => "", 
		};
	}

	public static NKMAssetName PlayVoice(NPC_TYPE npcType, NKCNPCTemplet npcTemplet, bool bStopCurrentSound = true, bool bIgnoreCoolTime = false, bool bShowCaption = false)
	{
		if (string.IsNullOrEmpty(npcTemplet.m_VoiceFileName))
		{
			return null;
		}
		if (!VoiceCoolTimeElapsed(npcType, npcTemplet, bIgnoreCoolTime))
		{
			return null;
		}
		if (!bStopCurrentSound && NKCSoundManager.IsPlayingVoice())
		{
			return null;
		}
		NKCSoundManager.StopSound(m_CurrentSoundUID);
		Log.Debug("[Voice] play " + npcTemplet.m_VoiceFileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Components/NKCUINPCBase.cs", 433);
		m_CurrentSoundUID = NKCSoundManager.PlayVoice(GetVoiceBundleName(npcType), npcTemplet.m_VoiceFileName, 0, bClearVoice: true, bIgnoreSameVoice: false, npcTemplet.m_Volume, 0f, 0f, bLoop: false, 0f, bShowCaption);
		if (m_CurrentSoundUID <= 0)
		{
			return null;
		}
		return new NKMAssetName(NKCAssetResourceManager.GetBundleName(npcTemplet.m_VoiceFileName), npcTemplet.m_VoiceFileName);
	}

	public static void PlayVoice(NPC_TYPE npcType, string voiceFileName, bool bStopCurrentSound = true, bool bShowCaption = false)
	{
		if (bStopCurrentSound || !NKCSoundManager.IsPlayingVoice())
		{
			NKCSoundManager.StopSound(m_CurrentSoundUID);
			Log.Debug("[Voice] play " + voiceFileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Components/NKCUINPCBase.cs", 455);
			m_CurrentSoundUID = NKCSoundManager.PlayVoice(GetVoiceBundleName(npcType), voiceFileName, 0, bClearVoice: true, bIgnoreSameVoice: false, 1f, 0f, 0f, bLoop: false, 0f, bShowCaption);
		}
	}

	public void StopVoice()
	{
		if (NKCSoundManager.IsPlayingVoice())
		{
			NKCSoundManager.StopSound(m_CurrentSoundUID);
		}
	}

	private static bool VoiceCoolTimeElapsed(NPC_TYPE npcType, NKCNPCTemplet npcTemplet, bool bIgnoreCoolTime)
	{
		if (prevNPCType != npcType)
		{
			m_dicCoolTimeInfo.Clear();
		}
		prevNPCType = npcType;
		float value = Time.time + npcTemplet.m_ActionCoolTime;
		if (m_dicCoolTimeInfo.TryGetValue(npcTemplet.m_ActionType, out var value2))
		{
			if (Time.time < value2)
			{
				if (!bIgnoreCoolTime)
				{
					return false;
				}
				return true;
			}
			m_dicCoolTimeInfo[npcTemplet.m_ActionType] = value;
		}
		else
		{
			m_dicCoolTimeInfo.Add(npcTemplet.m_ActionType, value);
		}
		return true;
	}

	public bool HasAction(NPC_ACTION_TYPE actionType)
	{
		return GetNPCTemplet(actionType) != null;
	}

	public abstract void PlayAni(NPC_ACTION_TYPE actionType, bool bMute = false);

	public abstract void PlayAni(eEmotion emotion);

	public abstract void DragEvent();

	public abstract void OpenTalk(bool bLeft, NKC_UI_TALK_BOX_DIR dir, string talk, float fadeTime = 0f);

	public abstract void CloseTalk();
}
