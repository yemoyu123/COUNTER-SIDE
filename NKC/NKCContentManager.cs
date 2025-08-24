using System;
using System.Collections.Generic;
using System.Text;
using Cs.Logging;
using NKC.Publisher;
using NKC.UI;
using NKC.UI.Collection;
using NKC.UI.Event;
using NKC.UI.Friend;
using NKC.UI.Gauntlet;
using NKC.UI.Guide;
using NKC.UI.Guild;
using NKC.UI.Module;
using NKC.UI.Office;
using NKC.UI.Shop;
using NKC.UI.Trim;
using NKM;
using NKM.Event;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;

namespace NKC;

public static class NKCContentManager
{
	public enum eContentStatus
	{
		Open,
		Lock,
		Hide
	}

	public class NKCUnlockableContent
	{
		public int m_Code;

		public ContentsType m_eContentsType;

		public int m_ContentsValue;

		public UnlockInfo m_UnlockInfo;

		public string m_LockedText;

		public string m_PopupTitle;

		public string m_PopupDesc;

		public string m_PopupImageName;

		public string m_PopupIconAssetBundleName;

		public string m_PopupIconName;

		public NKCUnlockableContent()
		{
		}

		public NKCUnlockableContent(NKMContentUnlockTemplet templet)
		{
			m_eContentsType = templet.m_eContentsType;
			m_ContentsValue = templet.m_ContentsValue;
			m_UnlockInfo = templet.m_UnlockInfo;
			m_LockedText = templet.m_LockedText;
			m_PopupTitle = templet.m_strPopupTitle;
			m_PopupDesc = templet.m_strPopupDesc;
			m_PopupImageName = templet.m_strPopupImageName;
			m_PopupIconAssetBundleName = templet.m_PopupIconAssetBundleName;
			m_PopupIconName = templet.m_PopupIconName;
			m_Code = Encode(m_eContentsType, m_ContentsValue);
		}

		public NKCUnlockableContent(ShopItemTemplet shopTemplet)
		{
			m_eContentsType = ContentsType.SHOP_ITEM_POPUP;
			m_ContentsValue = shopTemplet.m_ProductID;
			m_UnlockInfo = shopTemplet.m_UnlockInfo;
			m_LockedText = ((shopTemplet.m_UnlockReqStrID != "AUTO") ? shopTemplet.m_UnlockReqStrID : "");
			m_Code = Encode(m_eContentsType, m_ContentsValue);
		}

		public NKCUnlockableContent(ContentsType contentsType, int contentsValue, UnlockInfo unlockInfo, string title, string desc, string imgName)
		{
			m_eContentsType = contentsType;
			m_ContentsValue = contentsValue;
			m_UnlockInfo = unlockInfo;
			m_LockedText = "";
			m_PopupTitle = title;
			m_PopupDesc = desc;
			m_PopupImageName = imgName;
			m_Code = Encode(m_eContentsType, m_ContentsValue);
		}

		public static int Encode(ContentsType contentsType, int contentsValue)
		{
			return (int)(87 * contentsValue + contentsType);
		}

		public static bool Decode(int code, out ContentsType contentsType, out int contentsValue)
		{
			contentsType = (ContentsType)(code % 87);
			contentsValue = code / 87;
			return false;
		}
	}

	public delegate void OnClose();

	private static Dictionary<int, NKCUnlockableContent> m_dicUnlockableContents = new Dictionary<int, NKCUnlockableContent>();

	private static Dictionary<int, NKCUnlockableContent> m_dicUnlockedContent = new Dictionary<int, NKCUnlockableContent>();

	private static HashSet<int> m_hsUnlockCompletedContents = new HashSet<int>();

	private static Queue<NKCUnlockableContent> m_qUnlockedContent = new Queue<NKCUnlockableContent>();

	private static Dictionary<int, NKMStageTempletV2> m_dicLockedCounterCaseStageTemplet = new Dictionary<int, NKMStageTempletV2>();

	private static OnClose dOnClose;

	private static bool m_bPopupOpened = false;

	private static readonly HashSet<ContentsType> SET_NEED_UNLOCK_EFFECTS = new HashSet<ContentsType>
	{
		ContentsType.EPISODE,
		ContentsType.ACT,
		ContentsType.DUNGEON,
		ContentsType.FIELD,
		ContentsType.DAILY,
		ContentsType.SIDESTORY,
		ContentsType.COUNTERCASE
	};

	private static bool m_bLevelChanged = false;

	public static void AddUnlockableContents()
	{
		m_dicUnlockableContents.Clear();
		foreach (NKMContentUnlockTemplet value in NKMTempletContainer<NKMContentUnlockTemplet>.Values)
		{
			NKCUnlockableContent nKCUnlockableContent = new NKCUnlockableContent(value);
			if (!m_dicUnlockableContents.ContainsKey(nKCUnlockableContent.m_Code))
			{
				if (nKCUnlockableContent.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED)
				{
					m_dicUnlockableContents.Add(nKCUnlockableContent.m_Code, nKCUnlockableContent);
				}
				else if (nKCUnlockableContent.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_UNLOCK_STAGE)
				{
					NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(nKCUnlockableContent.m_ContentsValue);
					if (nKMStageTempletV != null)
					{
						if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKMStageTempletV.m_UnlockInfo))
						{
							m_dicUnlockableContents.Add(nKCUnlockableContent.m_Code, nKCUnlockableContent);
						}
						else
						{
							m_hsUnlockCompletedContents.Add(nKCUnlockableContent.m_Code);
						}
					}
				}
				else if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKCUnlockableContent.m_UnlockInfo))
				{
					m_dicUnlockableContents.Add(nKCUnlockableContent.m_Code, nKCUnlockableContent);
				}
				else
				{
					m_hsUnlockCompletedContents.Add(nKCUnlockableContent.m_Code);
				}
			}
			else
			{
				Debug.LogWarning($"{nKCUnlockableContent.m_eContentsType} 중복");
			}
		}
		foreach (ShopItemTemplet lockedProduct in NKCShopManager.GetLockedProductList(bIgnoreSuperUser: true))
		{
			if (lockedProduct.m_bUnlockBanner || lockedProduct.IsInstantProduct)
			{
				NKCUnlockableContent nKCUnlockableContent2 = new NKCUnlockableContent(lockedProduct);
				if (!m_dicUnlockableContents.ContainsKey(nKCUnlockableContent2.m_Code))
				{
					m_dicUnlockableContents.Add(nKCUnlockableContent2.m_Code, nKCUnlockableContent2);
				}
				else
				{
					Debug.LogWarning($"{nKCUnlockableContent2.m_eContentsType} 중복");
				}
			}
		}
		NKCUnlockableContent nKCUnlockableContent3 = new NKCUnlockableContent(ContentsType.ALARM_ONLY, NKMPvpCommonConst.Instance.RankUnlockInfo.reqValue, NKMPvpCommonConst.Instance.RankUnlockInfo, NKMPvpCommonConst.Instance.RankUnlockPopupTitle, NKMPvpCommonConst.Instance.RankUnlockPopupDesc, NKMPvpCommonConst.Instance.RankUnlockPopupImageName);
		if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKCUnlockableContent3.m_UnlockInfo))
		{
			if (!m_dicUnlockableContents.ContainsKey(nKCUnlockableContent3.m_Code))
			{
				m_dicUnlockableContents.Add(nKCUnlockableContent3.m_Code, nKCUnlockableContent3);
			}
			else
			{
				Debug.LogWarning("랭크전 언락정보 중복");
			}
		}
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_LEAGUE_MODE))
		{
			return;
		}
		NKCUnlockableContent nKCUnlockableContent4 = new NKCUnlockableContent(ContentsType.ALARM_ONLY, NKMPvpCommonConst.Instance.LeagueUnlockInfo.reqValue, NKMPvpCommonConst.Instance.LeagueUnlockInfo, NKMPvpCommonConst.Instance.LeagueUnlockPopupTitle, NKMPvpCommonConst.Instance.LeagueUnlockPopupDesc, NKMPvpCommonConst.Instance.LeagueUnlockPopupImageName);
		if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKCUnlockableContent4.m_UnlockInfo))
		{
			if (!m_dicUnlockableContents.ContainsKey(nKCUnlockableContent4.m_Code))
			{
				m_dicUnlockableContents.Add(nKCUnlockableContent4.m_Code, nKCUnlockableContent4);
			}
			else
			{
				Debug.LogWarning("리그전 언락정보 중복");
			}
		}
	}

	public static bool IsUnlockableContents(ContentsType contentsType, int contentsValue)
	{
		int key = NKCUnlockableContent.Encode(contentsType, contentsValue);
		if (m_dicUnlockableContents.ContainsKey(key))
		{
			return true;
		}
		return false;
	}

	public static bool IsContentAlwaysLocked(ContentsType contentsType, int contentsValue)
	{
		int key = NKCUnlockableContent.Encode(contentsType, contentsValue);
		if (m_dicUnlockableContents.TryGetValue(key, out var value))
		{
			return value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED;
		}
		return false;
	}

	public static eContentStatus CheckContentStatus(ContentsType contentsType, out bool bAdmin, int contentsValue = 0, int contentsValue2 = 0)
	{
		bAdmin = false;
		if (NKCScenManager.CurrentUserData() == null)
		{
			if ((uint)(contentsType - 46) <= 2u)
			{
				return eContentStatus.Open;
			}
			return eContentStatus.Hide;
		}
		if (contentsType == ContentsType.None)
		{
			return eContentStatus.Open;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		int num = NKCUnlockableContent.Encode(contentsType, contentsValue);
		eContentStatus eContentStatus = eContentStatus.Hide;
		if (m_dicUnlockableContents.ContainsKey(num))
		{
			NKCUnlockableContent nKCUnlockableContent = m_dicUnlockableContents[num];
			eContentStatus = nKCUnlockableContent.m_UnlockInfo.eReqType switch
			{
				STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED => eContentStatus.Lock, 
				STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_HIDDEN => eContentStatus.Hide, 
				_ => (!NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in nKCUnlockableContent.m_UnlockInfo, out bAdmin)) ? eContentStatus.Lock : eContentStatus.Open, 
			};
		}
		else if (m_hsUnlockCompletedContents.Contains(num))
		{
			eContentStatus = eContentStatus.Open;
		}
		else
		{
			Log.Warn($"{contentsType} / {contentsValue} is not exist in Templet", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentManager.cs", 312);
		}
		if (eContentStatus != eContentStatus.Open && nKMUserData != null && nKMUserData.IsSuperUser())
		{
			bAdmin = true;
			return eContentStatus.Open;
		}
		return eContentStatus;
	}

	public static bool IsContentsUnlocked(ContentsType contentsType, int contentsValue = 0, int contentsValue2 = 0)
	{
		if (NKCScenManager.CurrentUserData() == null)
		{
			if ((uint)(contentsType - 46) <= 2u)
			{
				return true;
			}
			return false;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && nKMUserData.IsSuperUser())
		{
			return true;
		}
		if (contentsType == ContentsType.None)
		{
			return true;
		}
		int key = NKCUnlockableContent.Encode(contentsType, contentsValue);
		if (m_dicUnlockableContents.ContainsKey(key))
		{
			NKCUnlockableContent nKCUnlockableContent = m_dicUnlockableContents[key];
			if (nKCUnlockableContent.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED)
			{
				return false;
			}
			return NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKCUnlockableContent.m_UnlockInfo);
		}
		return true;
	}

	public static NKMStageTempletV2 GetFirstStageTemplet(NKMEpisodeTempletV2 episodeTemplet, int actID, EPISODE_DIFFICULTY difficulty)
	{
		if (episodeTemplet != null && episodeTemplet.m_DicStage.Count > actID && episodeTemplet.m_DicStage[actID].Count > 0)
		{
			NKMStageTempletV2 nKMStageTempletV = episodeTemplet.m_DicStage[actID][0];
			if (nKMStageTempletV != null)
			{
				return nKMStageTempletV;
			}
		}
		return null;
	}

	public static int GetFirstStageID(NKMEpisodeTempletV2 episodeTemplet, int actID, EPISODE_DIFFICULTY difficulty)
	{
		return GetFirstStageTemplet(episodeTemplet, actID, difficulty)?.Key ?? (-1);
	}

	public static void RemoveUnlockedContent(ContentsType contentsType, int contentsValue = 0, bool bRemoveKey = true)
	{
		int key = NKCUnlockableContent.Encode(contentsType, contentsValue);
		if (m_dicUnlockableContents.ContainsKey(key))
		{
			m_dicUnlockableContents.Remove(key);
		}
		if (bRemoveKey)
		{
			string preferenceString = GetPreferenceString(contentsType, contentsValue);
			if (PlayerPrefs.HasKey(preferenceString))
			{
				PlayerPrefs.DeleteKey(preferenceString);
			}
		}
	}

	public static bool IsStageUnlocked(ContentsType contentsType, int contentsValue)
	{
		if (contentsType != ContentsType.EPISODE && contentsType != ContentsType.ACT)
		{
			return false;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(contentsValue);
		if (nKMStageTempletV != null)
		{
			return NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKMStageTempletV.m_UnlockInfo);
		}
		return false;
	}

	public static string GetLockedMessage(ContentsType contentsType, int contentsValue = 0)
	{
		int key = NKCUnlockableContent.Encode(contentsType, contentsValue);
		if (m_dicUnlockableContents.ContainsKey(key))
		{
			NKCUnlockableContent nKCUnlockableContent = m_dicUnlockableContents[key];
			if (string.IsNullOrEmpty(nKCUnlockableContent.m_LockedText))
			{
				return MakeUnlockConditionString(in nKCUnlockableContent.m_UnlockInfo, bSimple: false);
			}
			return NKCStringTable.GetString(m_dicUnlockableContents[key].m_LockedText);
		}
		return string.Empty;
	}

	public static bool ShowLockedMessagePopup(ContentsType contentsType, int contentsValue = 0)
	{
		string lockedMessage = GetLockedMessage(contentsType, contentsValue);
		if (!string.IsNullOrEmpty(lockedMessage))
		{
			NKCPopupMessageManager.AddPopupMessage(lockedMessage);
			return true;
		}
		return false;
	}

	public static void AddUnlockableCounterCase()
	{
		m_dicLockedCounterCaseStageTemplet.Clear();
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(50, EPISODE_DIFFICULTY.NORMAL);
		if (nKMEpisodeTempletV == null)
		{
			return;
		}
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in nKMEpisodeTempletV.m_DicStage)
		{
			for (int i = 0; i < item.Value.Count; i++)
			{
				NKMStageTempletV2 nKMStageTempletV = item.Value[i];
				if (!IsStageUnlocked(ContentsType.EPISODE, nKMStageTempletV.Key) && !NKMEpisodeMgr.CheckClear(NKCScenManager.CurrentUserData(), nKMStageTempletV) && !m_dicLockedCounterCaseStageTemplet.ContainsKey(nKMStageTempletV.Key))
				{
					m_dicLockedCounterCaseStageTemplet.Add(nKMStageTempletV.Key, nKMStageTempletV);
				}
			}
		}
	}

	public static bool SetUnlockedCounterCaseKey()
	{
		bool result = false;
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(50, EPISODE_DIFFICULTY.NORMAL);
		if (nKMEpisodeTempletV != null)
		{
			foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in nKMEpisodeTempletV.m_DicStage)
			{
				for (int i = 0; i < item.Value.Count; i++)
				{
					NKMStageTempletV2 nKMStageTempletV = item.Value[i];
					if (!IsStageUnlocked(ContentsType.EPISODE, nKMStageTempletV.Key))
					{
						break;
					}
					if (!NKMEpisodeMgr.CheckClear(NKCScenManager.CurrentUserData(), nKMStageTempletV) && m_dicLockedCounterCaseStageTemplet.ContainsKey(nKMStageTempletV.Key) && !PlayerPrefs.HasKey(GetCounterCaseNormalKey(nKMStageTempletV.ActId)))
					{
						PlayerPrefs.SetInt(GetCounterCaseNormalKey(nKMStageTempletV.ActId), nKMStageTempletV.ActId);
						m_dicLockedCounterCaseStageTemplet.Remove(nKMStageTempletV.Key);
						result = true;
					}
				}
			}
		}
		return result;
	}

	public static bool CheckNewCounterCase(NKMEpisodeTempletV2 episodeTemplet)
	{
		if (episodeTemplet == null)
		{
			return false;
		}
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in episodeTemplet.m_DicStage)
		{
			if (!PlayerPrefs.HasKey(GetCounterCaseNormalKey(item.Key)))
			{
				continue;
			}
			for (int i = 0; i < item.Value.Count; i++)
			{
				if (item.Value[i].EnableByTag)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void RemoveUnlockedCounterCaseKey(int actID)
	{
		if (PlayerPrefs.HasKey(GetCounterCaseNormalKey(actID)))
		{
			PlayerPrefs.DeleteKey(GetCounterCaseNormalKey(actID));
		}
	}

	public static string GetCounterCaseNormalKey(int actID)
	{
		return $"NewCC_{NKCScenManager.CurrentUserData().m_UserUID}_{actID}";
	}

	public static bool HasUnlockedContent(params STAGE_UNLOCK_REQ_TYPE[] aUnlockReq)
	{
		if (aUnlockReq == null || aUnlockReq.Length == 0)
		{
			if (m_dicUnlockedContent != null)
			{
				return m_dicUnlockedContent.Count > 0;
			}
			return false;
		}
		HashSet<STAGE_UNLOCK_REQ_TYPE> hashSet = new HashSet<STAGE_UNLOCK_REQ_TYPE>(aUnlockReq);
		foreach (KeyValuePair<int, NKCUnlockableContent> item in m_dicUnlockedContent)
		{
			if (hashSet.Contains(item.Value.m_UnlockInfo.eReqType))
			{
				return true;
			}
		}
		return false;
	}

	public static bool UnlockEffectRequired(ContentsType contentsType, int contentsValue = 0)
	{
		return PlayerPrefs.HasKey(GetPreferenceString(contentsType, contentsValue));
	}

	public static string GetPreferenceString(ContentsType contentsType, int contentsValue = 0)
	{
		if (NKCScenManager.CurrentUserData() != null)
		{
			return $"{NKCScenManager.CurrentUserData().m_UserUID}_{contentsType}_{contentsValue}";
		}
		return string.Empty;
	}

	public static void SetUnlockedContent(STAGE_UNLOCK_REQ_TYPE eReqType = STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_UNLOCKED, int reqValue = -1)
	{
		switch (eReqType)
		{
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE:
			OnWarfareClear(reqValue);
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON:
			OnDungeonClear(reqValue);
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE:
			OnPhaseClear(reqValue);
			break;
		default:
			OnContentUnlock(eReqType);
			break;
		}
	}

	private static bool UnlockPopupProcessRequired(NKCUnlockableContent content)
	{
		if (content == null)
		{
			return false;
		}
		switch (content.m_eContentsType)
		{
		case ContentsType.SHOP_ITEM_POPUP:
		{
			NKCShopManager.SetReserveRefreshShop();
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(content.m_ContentsValue);
			if (shopItemTemplet == null)
			{
				return false;
			}
			if (!shopItemTemplet.m_bUnlockBanner)
			{
				return false;
			}
			if (NKCShopManager.CanBuyFixShop(NKCScenManager.CurrentUserData(), shopItemTemplet, out var _, out var _) != NKM_ERROR_CODE.NEC_OK)
			{
				return false;
			}
			return true;
		}
		case ContentsType.MARKET_REVIEW_REQUEST:
			return true;
		default:
			return !string.IsNullOrEmpty(content.m_PopupImageName);
		}
	}

	private static void OnContentUnlock(STAGE_UNLOCK_REQ_TYPE eReqType)
	{
		new List<int>();
		foreach (KeyValuePair<int, NKCUnlockableContent> dicUnlockableContent in m_dicUnlockableContents)
		{
			if ((eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_UNLOCKED || dicUnlockableContent.Value.m_UnlockInfo.eReqType == eReqType) && NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in dicUnlockableContent.Value.m_UnlockInfo))
			{
				MarkAsUnlockedContent(dicUnlockableContent.Value);
			}
		}
	}

	private static void OnDungeonClear(int warfareID)
	{
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(warfareID);
		if (dungeonTempletBase == null)
		{
			return;
		}
		foreach (KeyValuePair<int, NKCUnlockableContent> dicUnlockableContent in m_dicUnlockableContents)
		{
			if (dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED)
			{
				continue;
			}
			if (dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON && dicUnlockableContent.Value.m_UnlockInfo.reqValue == warfareID)
			{
				MarkAsUnlockedContent(dicUnlockableContent.Value);
			}
			else if (dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_STAGE)
			{
				NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(dicUnlockableContent.Value.m_UnlockInfo.reqValue);
				if (nKMStageTempletV.m_STAGE_TYPE == STAGE_TYPE.ST_DUNGEON && nKMStageTempletV.m_StageBattleStrID == dungeonTempletBase.m_DungeonStrID)
				{
					MarkAsUnlockedContent(dicUnlockableContent.Value);
				}
			}
			else if ((dicUnlockableContent.Value.m_eContentsType == ContentsType.EPISODE || dicUnlockableContent.Value.m_eContentsType == ContentsType.ACT || dicUnlockableContent.Value.m_eContentsType == ContentsType.DUNGEON) && dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_UNLOCK_STAGE)
			{
				NKMStageTempletV2 nKMStageTempletV2 = NKMStageTempletV2.Find(dicUnlockableContent.Value.m_ContentsValue);
				if (nKMStageTempletV2 != null && nKMStageTempletV2.m_UnlockInfo.reqValue == warfareID && NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKMStageTempletV2.m_UnlockInfo))
				{
					MarkAsUnlockedContent(dicUnlockableContent.Value);
				}
			}
		}
	}

	private static void OnWarfareClear(int warfareID)
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		foreach (KeyValuePair<int, NKCUnlockableContent> dicUnlockableContent in m_dicUnlockableContents)
		{
			if (dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED)
			{
				continue;
			}
			if (dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE && dicUnlockableContent.Value.m_UnlockInfo.reqValue == warfareID)
			{
				MarkAsUnlockedContent(dicUnlockableContent.Value);
			}
			else if (dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_STAGE)
			{
				NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(dicUnlockableContent.Value.m_UnlockInfo.reqValue);
				if (nKMStageTempletV.m_STAGE_TYPE == STAGE_TYPE.ST_WARFARE && nKMStageTempletV.m_StageBattleStrID == nKMWarfareTemplet.m_WarfareStrID)
				{
					MarkAsUnlockedContent(dicUnlockableContent.Value);
				}
			}
			else if ((dicUnlockableContent.Value.m_eContentsType == ContentsType.EPISODE || dicUnlockableContent.Value.m_eContentsType == ContentsType.ACT || dicUnlockableContent.Value.m_eContentsType == ContentsType.DUNGEON) && dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_UNLOCK_STAGE)
			{
				NKMStageTempletV2 nKMStageTempletV2 = NKMStageTempletV2.Find(dicUnlockableContent.Value.m_ContentsValue);
				if (nKMStageTempletV2 != null && nKMStageTempletV2.m_UnlockInfo.reqValue == warfareID && NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKMStageTempletV2.m_UnlockInfo))
				{
					MarkAsUnlockedContent(dicUnlockableContent.Value);
				}
			}
		}
	}

	private static void OnPhaseClear(int phaseID)
	{
		NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(phaseID);
		if (nKMPhaseTemplet == null)
		{
			return;
		}
		foreach (KeyValuePair<int, NKCUnlockableContent> dicUnlockableContent in m_dicUnlockableContents)
		{
			if (dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED)
			{
				continue;
			}
			if (dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE && dicUnlockableContent.Value.m_UnlockInfo.reqValue == phaseID)
			{
				MarkAsUnlockedContent(dicUnlockableContent.Value);
			}
			else if (dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_STAGE)
			{
				NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(dicUnlockableContent.Value.m_UnlockInfo.reqValue);
				if (nKMStageTempletV.m_STAGE_TYPE == STAGE_TYPE.ST_PHASE && nKMStageTempletV.m_StageBattleStrID == nKMPhaseTemplet.StrId)
				{
					MarkAsUnlockedContent(dicUnlockableContent.Value);
				}
			}
			else if ((dicUnlockableContent.Value.m_eContentsType == ContentsType.EPISODE || dicUnlockableContent.Value.m_eContentsType == ContentsType.ACT || dicUnlockableContent.Value.m_eContentsType == ContentsType.DUNGEON) && dicUnlockableContent.Value.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_UNLOCK_STAGE)
			{
				NKMStageTempletV2 nKMStageTempletV2 = NKMStageTempletV2.Find(dicUnlockableContent.Value.m_ContentsValue);
				if (nKMStageTempletV2 != null && nKMStageTempletV2.m_UnlockInfo.reqValue == phaseID && NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKMStageTempletV2.m_UnlockInfo))
				{
					MarkAsUnlockedContent(dicUnlockableContent.Value);
				}
			}
		}
	}

	public static void AddUnlockedContentCC(NKCUnlockableContent content)
	{
		if (content.m_eContentsType == ContentsType.COUNTERCASE_NEW_CHARACTER)
		{
			m_dicUnlockedContent.Add(content.m_Code, content);
		}
	}

	private static void MarkAsUnlockedContent(NKCUnlockableContent content)
	{
		if (UnlockPopupProcessRequired(content) && !m_dicUnlockedContent.ContainsKey(content.m_Code))
		{
			m_dicUnlockedContent.Add(content.m_Code, content);
		}
		m_hsUnlockCompletedContents.Add(content.m_Code);
		if (SET_NEED_UNLOCK_EFFECTS.Contains(content.m_eContentsType))
		{
			PlayerPrefs.SetInt(GetPreferenceString(content.m_eContentsType, content.m_ContentsValue), 1);
		}
	}

	public static void ShowContentUnlockPopup(OnClose onClose = null, params STAGE_UNLOCK_REQ_TYPE[] aReqType)
	{
		if (m_bPopupOpened)
		{
			onClose?.Invoke();
			return;
		}
		if (m_dicUnlockedContent.Count <= 0)
		{
			onClose?.Invoke();
			return;
		}
		List<NKCUnlockableContent> list = new List<NKCUnlockableContent>();
		dOnClose = onClose;
		m_bPopupOpened = true;
		if (aReqType == null || aReqType.Length == 0)
		{
			list.AddRange(m_dicUnlockedContent.Values);
			m_dicUnlockedContent.Clear();
		}
		else
		{
			HashSet<STAGE_UNLOCK_REQ_TYPE> hashSet = new HashSet<STAGE_UNLOCK_REQ_TYPE>(aReqType);
			foreach (KeyValuePair<int, NKCUnlockableContent> item in m_dicUnlockedContent)
			{
				if (hashSet.Contains(item.Value.m_UnlockInfo.eReqType))
				{
					list.Add(item.Value);
				}
			}
			foreach (NKCUnlockableContent item2 in list)
			{
				if (m_dicUnlockedContent.ContainsKey(item2.m_Code))
				{
					m_dicUnlockedContent.Remove(item2.m_Code);
				}
			}
		}
		list.Sort(Compare);
		foreach (NKCUnlockableContent item3 in list)
		{
			m_qUnlockedContent.Enqueue(item3);
		}
		if (m_qUnlockedContent.Count > 0)
		{
			ContentUnlockPopupProcess();
			return;
		}
		m_bPopupOpened = false;
		dOnClose?.Invoke();
	}

	private static int Compare(NKCUnlockableContent lhs, NKCUnlockableContent rhs)
	{
		if (lhs.m_eContentsType == rhs.m_eContentsType)
		{
			return lhs.m_ContentsValue.CompareTo(rhs.m_ContentsValue);
		}
		return lhs.m_eContentsType.CompareTo(rhs.m_eContentsType);
	}

	private static void ContentUnlockPopupProcess()
	{
		List<int> list = new List<int>();
		int count = m_qUnlockedContent.Count;
		for (int i = 0; i < count; i++)
		{
			NKCUnlockableContent nKCUnlockableContent = m_qUnlockedContent.Dequeue();
			if (nKCUnlockableContent != null)
			{
				if (nKCUnlockableContent.m_eContentsType == ContentsType.SHOP_ITEM_POPUP)
				{
					list.Add(nKCUnlockableContent.m_ContentsValue);
					RemoveUnlockedContent(nKCUnlockableContent.m_eContentsType, nKCUnlockableContent.m_ContentsValue, bRemoveKey: false);
				}
				else
				{
					m_qUnlockedContent.Enqueue(nKCUnlockableContent);
				}
			}
		}
		if (list.Count > 0)
		{
			NKCPopupShopBannerNotice.Open(list, OnCloseContentUnlockPopup);
			return;
		}
		NKCUnlockableContent nKCUnlockableContent2 = m_qUnlockedContent.Dequeue();
		if (nKCUnlockableContent2 == null)
		{
			Debug.LogError("unlockContent null!");
			OnCloseContentUnlockPopup();
			return;
		}
		RemoveUnlockedContent(nKCUnlockableContent2.m_eContentsType, nKCUnlockableContent2.m_ContentsValue, bRemoveKey: false);
		switch (nKCUnlockableContent2.m_eContentsType)
		{
		case ContentsType.SHOP_ITEM_POPUP:
			NKCPopupShopBannerNotice.Open(nKCUnlockableContent2.m_ContentsValue, OnCloseContentUnlockPopup);
			break;
		case ContentsType.MARKET_REVIEW_REQUEST:
			if (NKCPublisherModule.Marketing.MarketReviewEnabled)
			{
				Debug.Log("MarketReview Enabled. Try review popup...");
				NKCPublisherModule.Marketing.OpenMarketReviewPopup(MakeReviewDescription(nKCUnlockableContent2), OnCloseContentUnlockPopup);
			}
			else
			{
				Debug.Log("MarketReview Disabled!");
				OnCloseContentUnlockPopup();
			}
			break;
		case ContentsType.COUNTERCASE_NEW_CHARACTER:
			NKCPopupContentUnlock.instance.Open(nKCUnlockableContent2, OnCloseContentUnlockPopup);
			break;
		default:
			NKCPopupContentUnlock.instance.Open(nKCUnlockableContent2, OnCloseContentUnlockPopup);
			break;
		}
	}

	private static void OnCloseContentUnlockPopup()
	{
		if (m_qUnlockedContent.Count > 0)
		{
			ContentUnlockPopupProcess();
			return;
		}
		m_bPopupOpened = false;
		dOnClose?.Invoke();
	}

	public static GameObject AddUnlockedEffect(Transform parent)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(NKCResourceUtility.GetOrLoadAssetResource<GameObject>("ab_fx_ui_deck_open", "AB_FX_UI_CONTENT_UNLOCK_LOOP_NOPARTICLE"));
		gameObject.transform.SetParent(parent);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
		gameObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;
		return gameObject;
	}

	public static string MakeUnlockConditionString(in UnlockInfo info, bool bSimple)
	{
		return NKCUtilString.GetUnlockConditionRequireDesc(info, bSimple);
	}

	public static NKM_SCEN_ID GetShortCutTargetSceneID(NKM_SHORTCUT_TYPE shortCutType)
	{
		switch (shortCutType)
		{
		case NKM_SHORTCUT_TYPE.SHORTCUT_MAINSTREAM:
		case NKM_SHORTCUT_TYPE.SHORTCUT_DUNGEON:
		case NKM_SHORTCUT_TYPE.SHORTCUT_OPERATION:
			return NKM_SCEN_ID.NSI_OPERATION;
		case NKM_SHORTCUT_TYPE.SHORTCUT_DIVE:
		case NKM_SHORTCUT_TYPE.SHORTCUT_WORLDMAP_MISSION:
			return NKM_SCEN_ID.NSI_WORLDMAP;
		case NKM_SHORTCUT_TYPE.SHORTCUT_PVP_MAIN:
		case NKM_SHORTCUT_TYPE.SHORTCUT_PVP_RANK:
		case NKM_SHORTCUT_TYPE.SHORTCUT_PVP_ASYNC:
		case NKM_SHORTCUT_TYPE.SHORTCUT_PVP_LEAGUE:
			return NKM_SCEN_ID.NSI_GAUNTLET_LOBBY;
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_CONTRACT:
			return NKM_SCEN_ID.NSI_CONTRACT;
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_NEGOTIATE:
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_SCOUT:
		case NKM_SHORTCUT_TYPE.SHORTCUT_EQUIP_MAKE:
		case NKM_SHORTCUT_TYPE.SHORTCUT_EQUIP_ENCHANT:
		case NKM_SHORTCUT_TYPE.SHORTCUT_EQUIP_TUNING:
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHIP_MAKE:
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHIP_UPGRADE:
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHIP_LEVELUP:
		case NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE:
			return NKM_SCEN_ID.NSI_OFFICE;
		case NKM_SHORTCUT_TYPE.SHORTCUT_BASEMAIN:
			return NKM_SCEN_ID.NSI_BASE;
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHOP:
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHOP_SCENE:
			return NKM_SCEN_ID.NSI_SHOP;
		case NKM_SHORTCUT_TYPE.SHORTCUT_FRIEND_ADD:
		case NKM_SHORTCUT_TYPE.SHORTCUT_FRIEND_MYPROFILE:
			return NKM_SCEN_ID.NSI_FRIEND;
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION:
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_SHIP:
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_UNIT:
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_ILLUST:
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_STORY:
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_TEAMUP:
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_OPERATOR:
			return NKM_SCEN_ID.NSI_COLLECTION;
		case NKM_SHORTCUT_TYPE.SHORTCUT_MISSION:
		case NKM_SHORTCUT_TYPE.SHORTCUT_RANKING:
			return NKM_SCEN_ID.NSI_HOME;
		case NKM_SHORTCUT_TYPE.SHORTCUT_INVENTORY:
			return NKM_SCEN_ID.NSI_INVENTORY;
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNITLIST:
			return NKM_SCEN_ID.NSI_UNIT_LIST;
		case NKM_SHORTCUT_TYPE.SHORTCUT_DECKSETUP:
			return NKM_SCEN_ID.NSI_TEAM;
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHADOW_PALACE:
			return NKM_SCEN_ID.NSI_SHADOW_PALACE;
		case NKM_SHORTCUT_TYPE.SHORTCUT_EVENT:
			return NKM_SCEN_ID.NSI_INVALID;
		case NKM_SHORTCUT_TYPE.SHORTCUT_FIERCE:
			return NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT;
		case NKM_SHORTCUT_TYPE.SHORTCUT_TRIM:
			return NKM_SCEN_ID.NSI_TRIM;
		case NKM_SHORTCUT_TYPE.SHORTCUT_EVENT_COLLECTION:
			return NKM_SCEN_ID.NSI_HOME;
		default:
			return NKM_SCEN_ID.NSI_HOME;
		}
	}

	public static void MoveToShortCut(NKM_SHORTCUT_TYPE shortCutType, string shortCutParam, bool bForce = false)
	{
		if (NKCUIMail.IsInstanceOpen)
		{
			NKCUIMail.Instance.Close();
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_BASE)
		{
			if (NKCUIInventory.IsInstanceOpen)
			{
				NKCUIInventory.Instance.Close();
			}
			if (NKCUIUnitSelectList.IsInstanceOpen)
			{
				NKCUIUnitSelectList.Instance.Close();
			}
			if (NKCUIPersonnel.IsInstanceOpen)
			{
				NKCUIPersonnel.Instance.Close();
			}
		}
		switch (shortCutType)
		{
		case NKM_SHORTCUT_TYPE.SHORTCUT_MAINSTREAM:
			if (shortCutParam != "")
			{
				NKC_SCEN_OPERATION_V2 sCEN_OPERATION = NKCScenManager.GetScenManager().Get_SCEN_OPERATION();
				if (sCEN_OPERATION != null)
				{
					NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(int.Parse(shortCutParam));
					if (nKMWarfareTemplet != null)
					{
						NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(nKMWarfareTemplet.m_WarfareStrID);
						if (nKMStageTempletV != null)
						{
							if (NKMEpisodeMgr.CheckEpisodeMission(NKCScenManager.CurrentUserData(), nKMStageTempletV))
							{
								sCEN_OPERATION.SetReservedStage(nKMStageTempletV);
								NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
							}
							else
							{
								MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OPERATION, nKMStageTempletV.EpisodeCategory.ToString());
							}
							break;
						}
					}
				}
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_DUNGEON:
			if (shortCutParam != "")
			{
				NKC_SCEN_OPERATION_V2 sCEN_OPERATION3 = NKCScenManager.GetScenManager().Get_SCEN_OPERATION();
				if (sCEN_OPERATION3 != null)
				{
					NKMStageTempletV2 nKMStageTempletV2 = NKMStageTempletV2.Find(int.Parse(shortCutParam));
					if (nKMStageTempletV2 != null)
					{
						ContentsType contentsType2 = GetContentsType(nKMStageTempletV2.EpisodeCategory);
						if (!IsContentsUnlocked(contentsType2))
						{
							ShowLockedMessagePopup(contentsType2);
							break;
						}
						if (!nKMStageTempletV2.EpisodeTemplet.IsOpenedDayOfWeek())
						{
							NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_DAILY_CHECK_DAY);
							break;
						}
						EPISODE_DIFFICULTY difficulty = nKMStageTempletV2.m_Difficulty;
						bool flag = NKMEpisodeMgr.IsPossibleEpisode(NKCScenManager.CurrentUserData(), nKMStageTempletV2.EpisodeTemplet.m_EpisodeID, difficulty);
						if (!flag && nKMStageTempletV2.m_Difficulty == EPISODE_DIFFICULTY.HARD)
						{
							difficulty = EPISODE_DIFFICULTY.NORMAL;
							flag = NKMEpisodeMgr.IsPossibleEpisode(NKCScenManager.CurrentUserData(), nKMStageTempletV2.EpisodeTemplet.m_EpisodeID, difficulty);
						}
						if (!flag)
						{
							if (NKMEpisodeMgr.GetListNKMEpisodeTempletByCategory(nKMStageTempletV2.EpisodeCategory, bOnlyOpen: true, nKMStageTempletV2.m_Difficulty).Count <= 0)
							{
								NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_ALWAYS_LOCKED"));
								break;
							}
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.Append(nKMStageTempletV2.EpisodeCategory);
							stringBuilder.Append("@");
							stringBuilder.Append(nKMStageTempletV2.EpisodeTemplet.m_EpisodeID);
							MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OPERATION, stringBuilder.ToString());
							break;
						}
						bool flag2 = NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_OPERATION;
						bool flag3 = sCEN_OPERATION3.GetReservedEpisodeTemplet() != nKMStageTempletV2.EpisodeTemplet;
						NKC_SCEN_OPERATION_V2 sCEN_OPERATION4 = NKCScenManager.GetScenManager().Get_SCEN_OPERATION();
						if (sCEN_OPERATION4 != null)
						{
							if (NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKMStageTempletV2.m_UnlockInfo))
							{
								sCEN_OPERATION4.SetReservedStage(nKMStageTempletV2);
							}
							else if (NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), nKMStageTempletV2.EpisodeTemplet.GetUnlockInfo()))
							{
								sCEN_OPERATION4.SetReservedEpisodeTemplet(nKMStageTempletV2.EpisodeTemplet);
							}
						}
						if (nKMStageTempletV2.EpisodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_COUNTERCASE)
						{
							NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
							break;
						}
						if (!NKMEpisodeMgr.CheckEpisodeMission(NKCScenManager.CurrentUserData(), nKMStageTempletV2))
						{
							NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_ALWAYS_LOCKED"));
						}
						if (flag2 || flag3)
						{
							NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
						}
						else
						{
							sCEN_OPERATION3.ReopenEpisodeView();
						}
						break;
					}
				}
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_OPERATION:
			if (shortCutParam != "")
			{
				string[] array2 = shortCutParam.Split('@');
				if (array2.Length != 0)
				{
					NKC_SCEN_OPERATION_V2 sCEN_OPERATION2 = NKCScenManager.GetScenManager().Get_SCEN_OPERATION();
					if (sCEN_OPERATION2 != null && Enum.TryParse<EPISODE_CATEGORY>(array2[0], out var result10))
					{
						ContentsType contentsType = GetContentsType(result10);
						if (!IsContentsUnlocked(contentsType))
						{
							ShowLockedMessagePopup(contentsType);
							break;
						}
						switch (result10)
						{
						case EPISODE_CATEGORY.EC_TRIM:
							NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_TRIM);
							return;
						case EPISODE_CATEGORY.EC_SHADOW:
							NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_PALACE);
							return;
						case EPISODE_CATEGORY.EC_FIERCE:
						{
							NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr2 = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
							if (nKCFierceBattleSupportDataMgr2.FierceTemplet == null || !nKCFierceBattleSupportDataMgr2.IsCanAccessFierce())
							{
								NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NORMAL, NKCStringTable.GetString("SI_PF_POPUP_NO_EVENT"));
							}
							else
							{
								NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT);
							}
							return;
						}
						}
						if (array2.Length > 1)
						{
							if (int.TryParse(array2[1], out var result11))
							{
								NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(result11, EPISODE_DIFFICULTY.NORMAL);
								if (nKMEpisodeTempletV != null)
								{
									if (!NKMEpisodeMgr.IsPossibleEpisode(NKCScenManager.CurrentUserData(), nKMEpisodeTempletV))
									{
										NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GetUnlockConditionRequireDesc(nKMEpisodeTempletV.GetFirstStage(1)), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
										break;
									}
									if (result11 == 50)
									{
										sCEN_OPERATION2.SetReservedEpisodeTemplet(nKMEpisodeTempletV);
									}
									else
									{
										NKMStageTempletV2 firstStage = nKMEpisodeTempletV.GetFirstStage(1);
										if (firstStage != null && firstStage.IsOpenedDayOfWeek())
										{
											sCEN_OPERATION2.SetReservedEpisodeTemplet(nKMEpisodeTempletV);
										}
									}
								}
							}
						}
						else
						{
							sCEN_OPERATION2.SetReservedEpisodeCategory(result10);
						}
					}
				}
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION, bForce);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_DIVE:
			if (!IsContentsUnlocked(ContentsType.DIVE))
			{
				ShowLockedMessagePopup(ContentsType.DIVE);
				break;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE_READY().SetTargetEventID(0, 0);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DIVE_READY, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_WORLDMAP_MISSION:
			if (!IsContentsUnlocked(ContentsType.WORLDMAP))
			{
				ShowLockedMessagePopup(ContentsType.WORLDMAP);
				break;
			}
			if (shortCutParam == "POPUP_RAID")
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetReserveOpenEventList();
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetShowIntro();
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_PVP_MAIN:
			if (!IsContentsUnlocked(ContentsType.PVP))
			{
				ShowLockedMessagePopup(ContentsType.PVP);
			}
			else
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_INTRO, bForce: false);
			}
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_PVP_LEAGUE:
			if (!NKCPVPManager.IsPvpLeagueUnlocked())
			{
				NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_GAUNTLET_NOT_OPEN_LEAGUE_MODE, NKMPvpCommonConst.Instance.LEAGUE_PVP_OPEN_POINT));
				break;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_PVP_RANK:
			if (!NKCPVPManager.IsPvpRankUnlocked())
			{
				NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_GAUNTLET_NOT_OPEN_RANK_MODE, NKMPvpCommonConst.Instance.RANK_PVP_OPEN_POINT));
				break;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_PVP_ASYNC:
			if (!IsContentsUnlocked(ContentsType.PVP))
			{
				ShowLockedMessagePopup(ContentsType.PVP);
				break;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_CONTRACT:
			if (!IsContentsUnlocked(ContentsType.CONTRACT))
			{
				ShowLockedMessagePopup(ContentsType.CONTRACT);
				break;
			}
			if (!string.IsNullOrEmpty(shortCutParam))
			{
				NKCScenManager.GetScenManager().GET_SCEN_CONTRACT()?.SetReserveContractID(shortCutParam);
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CONTRACT, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_TRAINING:
		{
			if (!IsContentsUnlocked(ContentsType.LAB_TRAINING))
			{
				ShowLockedMessagePopup(ContentsType.LAB_TRAINING);
				break;
			}
			long result5 = 0L;
			long.TryParse(shortCutParam, out result5);
			NKMUnitData unitFromUID2 = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(result5);
			if (unitFromUID2 != null)
			{
				NKCScenManager.GetScenManager().GET_NKC_SCEN_UNIT_LIST().SetOpenReserve(NKC_SCEN_UNIT_LIST.eUIOpenReserve.UnitSkillTraining, result5, bForce);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_UNIT_LIST, bForce: false);
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_LIMITBREAK:
		{
			if (!IsContentsUnlocked(ContentsType.LAB_LIMITBREAK))
			{
				ShowLockedMessagePopup(ContentsType.LAB_LIMITBREAK);
				break;
			}
			long result4 = 0L;
			long.TryParse(shortCutParam, out result4);
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(result4);
			if (unitFromUID != null)
			{
				NKCScenManager.GetScenManager().GET_NKC_SCEN_UNIT_LIST().SetOpenReserve(NKC_SCEN_UNIT_LIST.eUIOpenReserve.UnitLimitbreak, result4, bForce);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_UNIT_LIST, bForce: false);
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_ENCHANT:
		{
			if (!IsContentsUnlocked(ContentsType.BASE_LAB))
			{
				ShowLockedMessagePopup(ContentsType.BASE_LAB);
				break;
			}
			long result20 = 0L;
			long.TryParse(shortCutParam, out result20);
			NKCScenManager.GetScenManager().Get_SCEN_BASE().SetOpenReserve(NKC_SCEN_BASE.eUIOpenReserve.LAB_Enchant, result20, bForce);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_BASE, bForce: false);
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_NEGOTIATE:
		{
			if (!IsContentsUnlocked(ContentsType.PERSONNAL_NEGO))
			{
				ShowLockedMessagePopup(ContentsType.PERSONNAL_NEGO);
				break;
			}
			long result9 = 0L;
			long.TryParse(shortCutParam, out result9);
			NKMUnitData unitFromUID3 = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(result9);
			if (unitFromUID3 != null)
			{
				NKCScenManager.GetScenManager().GET_NKC_SCEN_UNIT_LIST().SetOpenReserve(NKC_SCEN_UNIT_LIST.eUIOpenReserve.UnitNegotiate, result9, bForce);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_UNIT_LIST, bForce: false);
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_SCOUT:
			if (!IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
			{
				ShowLockedMessagePopup(ContentsType.BASE_PERSONNAL);
				break;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_OFFICE().ReserveShortcut(shortCutType, shortCutParam);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OFFICE, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_LIFETIME:
			if (!IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
			{
				ShowLockedMessagePopup(ContentsType.BASE_PERSONNAL);
				break;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_OFFICE().ReserveShortcut(shortCutType, shortCutParam);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OFFICE, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_DISMISS:
		case NKM_SHORTCUT_TYPE.SHORTCUT_UNITLIST:
		{
			if (!string.IsNullOrEmpty(shortCutParam) && Enum.TryParse<NKC_SCEN_UNIT_LIST.UNIT_LIST_TAB>(shortCutParam, out var result24))
			{
				NKCScenManager.GetScenManager().GET_NKC_SCEN_UNIT_LIST()?.SetReservedTab(result24);
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_UNIT_LIST, bForce: false);
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_EQUIP_MAKE:
			if (!IsContentsUnlocked(ContentsType.BASE_FACTORY))
			{
				ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
				break;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_OFFICE().ReserveShortcut(shortCutType, shortCutParam);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OFFICE, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_EQUIP_ENCHANT:
			if (!IsContentsUnlocked(ContentsType.BASE_FACTORY))
			{
				ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
				break;
			}
			if (!IsContentsUnlocked(ContentsType.FACTORY_ENCHANT))
			{
				ShowLockedMessagePopup(ContentsType.FACTORY_ENCHANT);
				break;
			}
			NKCScenManager.GetScenManager().Get_SCEN_BASE().SetOpenReserve(NKC_SCEN_BASE.eUIOpenReserve.Factory_Enchant, 0L, bForce);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_BASE, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_EQUIP_TUNING:
			if (!IsContentsUnlocked(ContentsType.BASE_FACTORY))
			{
				ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
				break;
			}
			if (!IsContentsUnlocked(ContentsType.FACTORY_TUNING))
			{
				ShowLockedMessagePopup(ContentsType.FACTORY_TUNING);
				break;
			}
			NKCScenManager.GetScenManager().Get_SCEN_BASE().SetOpenReserve(NKC_SCEN_BASE.eUIOpenReserve.Factory_Tunning, 0L, bForce);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_BASE, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHIP_MAKE:
			if (!IsContentsUnlocked(ContentsType.BASE_HANGAR))
			{
				ShowLockedMessagePopup(ContentsType.BASE_HANGAR);
				break;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_OFFICE().ReserveShortcut(shortCutType, shortCutParam);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OFFICE, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHIP_UPGRADE:
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHIP_LEVELUP:
		{
			if (!IsContentsUnlocked(ContentsType.HANGER_SHIPYARD))
			{
				ShowLockedMessagePopup(ContentsType.HANGER_SHIPYARD);
				break;
			}
			long result25 = 0L;
			long.TryParse(shortCutParam, out result25);
			NKMUnitData shipFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(result25);
			if (shipFromUID != null)
			{
				NKCScenManager.GetScenManager().GET_NKC_SCEN_UNIT_LIST().SetOpenReserve(NKC_SCEN_UNIT_LIST.eUIOpenReserve.ShipRepair, result25, bForce);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_UNIT_LIST, bForce: false);
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHOP:
			if (!IsContentsUnlocked(ContentsType.LOBBY_SUBMENU))
			{
				ShowLockedMessagePopup(ContentsType.LOBBY_SUBMENU);
			}
			else
			{
				if (string.IsNullOrEmpty(shortCutParam))
				{
					break;
				}
				string[] array3 = shortCutParam.Split(',', ' ', '@');
				if (array3.Length != 0)
				{
					int result15 = 0;
					int result16 = 0;
					string tabType = array3[0];
					if (array3.Length > 1)
					{
						int.TryParse(array3[1], out result15);
					}
					if (array3.Length > 2)
					{
						int.TryParse(array3[2], out result16);
					}
					NKCUIShop.ShopShortcut(tabType, result15, result16);
				}
			}
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHOP_SCENE:
			if (!IsContentsUnlocked(ContentsType.LOBBY_SUBMENU))
			{
				ShowLockedMessagePopup(ContentsType.LOBBY_SUBMENU);
			}
			else
			{
				if (string.IsNullOrEmpty(shortCutParam))
				{
					break;
				}
				string[] array = shortCutParam.Split(',', ' ', '@');
				if (array.Length != 0)
				{
					int result2 = 0;
					int result3 = 0;
					string shopType = array[0];
					if (array.Length > 1)
					{
						int.TryParse(array[1], out result2);
					}
					if (array.Length > 2)
					{
						int.TryParse(array[2], out result3);
					}
					NKCScenManager.GetScenManager().Get_NKC_SCEN_SHOP().SetReservedOpenTab(shopType, result2, result3);
					if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_SHOP)
					{
						NKCScenManager.GetScenManager().Get_NKC_SCEN_SHOP().MoveToReservedTab();
					}
					else
					{
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHOP, bForce: false);
					}
				}
			}
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_FRIEND_ADD:
			if (!IsContentsUnlocked(ContentsType.FRIENDS))
			{
				ShowLockedMessagePopup(ContentsType.FRIENDS);
				break;
			}
			if (!string.IsNullOrEmpty(shortCutParam))
			{
				NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE fRIEND_TOP_MENU_TYPE = (NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE)Enum.Parse(typeof(NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE), shortCutParam);
				if (fRIEND_TOP_MENU_TYPE == NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE.FTMT_REGISTER)
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().SetReservedTab(fRIEND_TOP_MENU_TYPE);
				}
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_FRIEND, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_FRIEND_MYPROFILE:
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				NKCUIUserInfoV2.Instance.Open(NKCScenManager.GetScenManager().GetMyUserData());
				break;
			}
			NKCScenManager.GetScenManager().Get_SCEN_HOME().SetReservedOpenUI(NKC_SCEN_HOME.RESERVE_OPEN_TYPE.ROT_PROFILE, 0);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_UNIT:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().SetOpenReserve(NKCUICollectionGeneral.CollectionType.CT_UNIT, shortCutParam);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_COLLECTION, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_SHIP:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().SetOpenReserve(NKCUICollectionGeneral.CollectionType.CT_SHIP, shortCutParam);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_COLLECTION, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_ILLUST:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().SetOpenReserve(NKCUICollectionGeneral.CollectionType.CT_ILLUST, shortCutParam);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_COLLECTION, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_STORY:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().SetOpenReserve(NKCUICollectionGeneral.CollectionType.CT_STORY, shortCutParam);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_COLLECTION, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_TEAMUP:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().SetOpenReserve(NKCUICollectionGeneral.CollectionType.CT_TEAM_UP, shortCutParam);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_COLLECTION, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_OPERATOR:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().SetOpenReserve(NKCUICollectionGeneral.CollectionType.CT_OPERATOR, shortCutParam);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_COLLECTION, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION:
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_COLLECTION, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_MISSION:
		{
			if (!IsContentsUnlocked(ContentsType.LOBBY_SUBMENU))
			{
				ShowLockedMessagePopup(ContentsType.LOBBY_SUBMENU);
				break;
			}
			int result13 = 0;
			int.TryParse(shortCutParam, out result13);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				if (NKCUIMissionAchievement.IsInstanceOpen)
				{
					NKCUIManager.SetAsTopmost(NKCUIMissionAchievement.Instance);
				}
				else
				{
					NKCUIMissionAchievement.Instance.Open(result13);
				}
			}
			else
			{
				NKCScenManager.GetScenManager().Get_SCEN_HOME().SetReservedOpenUI(NKC_SCEN_HOME.RESERVE_OPEN_TYPE.ROT_MISSION, result13);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_INVENTORY:
		{
			if (!string.IsNullOrEmpty(shortCutParam) && Enum.TryParse<NKCUIInventory.NKC_INVENTORY_TAB>(shortCutParam, out var result8))
			{
				NKCScenManager.GetScenManager().Get_SCEN_INVENTORY()?.SetReservedOpenTyp(result8);
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_INVENTORY, bForce: false);
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_DECKSETUP:
			if (!IsContentsUnlocked(ContentsType.DECKVIEW))
			{
				ShowLockedMessagePopup(ContentsType.DECKVIEW);
			}
			else
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_TEAM, bForce: false);
			}
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_BASEMAIN:
			if (!IsContentsUnlocked(ContentsType.BASE))
			{
				ShowLockedMessagePopup(ContentsType.BASE);
				break;
			}
			NKCScenManager.GetScenManager().Get_SCEN_BASE().SetOpenReserve(NKC_SCEN_BASE.eUIOpenReserve.Base_Main, 0L, bForce: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_BASE, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_URL:
			if (!string.IsNullOrEmpty(shortCutParam))
			{
				Application.OpenURL(shortCutParam);
			}
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_SHADOW_PALACE:
			if (!IsContentsUnlocked(ContentsType.SHADOW_PALACE))
			{
				ShowLockedMessagePopup(ContentsType.SHADOW_PALACE);
			}
			else
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_PALACE, bForce: false);
			}
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_RANKING:
		{
			if (!IsContentsUnlocked(ContentsType.LEADERBOARD))
			{
				ShowLockedMessagePopup(ContentsType.LEADERBOARD);
				break;
			}
			NKMLeaderBoardTemplet reservedTemplet = null;
			if (int.TryParse(shortCutParam, out var result19))
			{
				reservedTemplet = NKMTempletContainer<NKMLeaderBoardTemplet>.Find(result19);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				if (NKCUILeaderBoard.IsInstanceOpen)
				{
					NKCUIManager.SetAsTopmost(NKCUILeaderBoard.Instance);
				}
				else
				{
					NKCUILeaderBoard.Instance.Open(reservedTemplet);
				}
			}
			else
			{
				NKCScenManager.GetScenManager().Get_SCEN_HOME().SetReservedOpenUI(NKC_SCEN_HOME.RESERVE_OPEN_TYPE.ROT_RANKING_BOARD, result19);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_RANKING_POPUP:
		{
			if (!IsContentsUnlocked(ContentsType.LEADERBOARD))
			{
				ShowLockedMessagePopup(ContentsType.LEADERBOARD);
				break;
			}
			NKMLeaderBoardTemplet nKMLeaderBoardTemplet = null;
			if (int.TryParse(shortCutParam, out var result17))
			{
				nKMLeaderBoardTemplet = NKMTempletContainer<NKMLeaderBoardTemplet>.Find(result17);
			}
			if (nKMLeaderBoardTemplet != null)
			{
				NKCPopupLeaderBoardSingle.Instance.OpenSingle(nKMLeaderBoardTemplet);
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_GUILD:
			if (!IsContentsUnlocked(ContentsType.GUILD))
			{
				ShowLockedMessagePopup(ContentsType.GUILD);
				break;
			}
			if (NKCGuildManager.MyData.guildUid <= 0)
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_INTRO);
				break;
			}
			switch (shortCutParam)
			{
			case "TAB_POINT":
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().SetReserveLobbyTab(NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Point);
				break;
			case "TAB_SHOP":
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().SetReserveMoveToShop(bValue: true);
				break;
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_EVENT:
		{
			NKMEventTabTemplet reservedTabTemplet = null;
			if (int.TryParse(shortCutParam, out var result14))
			{
				reservedTabTemplet = NKMEventTabTemplet.Find(result14);
			}
			NKCUIEvent.Instance.Open(reservedTabTemplet);
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_FIERCE:
		{
			if (!IsContentsUnlocked(ContentsType.FIERCE))
			{
				ShowLockedMessagePopup(ContentsType.FIERCE);
				break;
			}
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			if (nKCFierceBattleSupportDataMgr.FierceTemplet == null || !nKCFierceBattleSupportDataMgr.IsCanAccessFierce())
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NORMAL, NKCStringTable.GetString("SI_PF_POPUP_NO_EVENT"));
			}
			else
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT, bForce: false);
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE:
		{
			bool flag4 = false;
			NKMOfficeRoomTemplet.RoomType result22;
			NKCUIOfficeMapFront.SectionType result23;
			if (int.TryParse(shortCutParam, out var result21))
			{
				NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(result21);
				if (nKMOfficeRoomTemplet == null)
				{
					break;
				}
				if (!nKMOfficeRoomTemplet.IsFacility)
				{
					flag4 = true;
				}
			}
			else if (Enum.TryParse<NKMOfficeRoomTemplet.RoomType>(shortCutParam, out result22))
			{
				if (result22 == NKMOfficeRoomTemplet.RoomType.Dorm)
				{
					flag4 = true;
				}
			}
			else if (Enum.TryParse<NKCUIOfficeMapFront.SectionType>(shortCutParam, out result23) && result23 == NKCUIOfficeMapFront.SectionType.Room)
			{
				flag4 = true;
			}
			if (flag4 && !IsContentsUnlocked(ContentsType.OFFICE))
			{
				ShowLockedMessagePopup(ContentsType.OFFICE);
				break;
			}
			if (!string.IsNullOrEmpty(shortCutParam))
			{
				if (string.Equals(shortCutParam, "Hangar") && !IsContentsUnlocked(ContentsType.BASE_HANGAR))
				{
					ShowLockedMessagePopup(ContentsType.BASE_HANGAR);
					break;
				}
				if (string.Equals(shortCutParam, "Forge") && !IsContentsUnlocked(ContentsType.BASE_FACTORY))
				{
					ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
					break;
				}
			}
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				nKMUserData.OfficeData.ResetFriendUId();
				NKCScenManager.GetScenManager().Get_NKC_SCEN_OFFICE().ReserveShortcut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, shortCutParam);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OFFICE, bForce: false);
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_GUIDE:
			if (!string.IsNullOrEmpty(shortCutParam))
			{
				NKCUIPopUpGuide.Instance.Open(shortCutParam);
			}
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_PT_EXCHANGE:
			NKCUIPointExchangeLobby.OpenPtExchangePopup();
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_GUIDE_MISSION:
		{
			if (!IsContentsUnlocked(ContentsType.LOBBY_SUBMENU))
			{
				ShowLockedMessagePopup(ContentsType.LOBBY_SUBMENU);
				break;
			}
			int.TryParse(shortCutParam, out var result18);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				NKCUIMissionGuide.Instance.Open(result18);
				break;
			}
			NKCScenManager.GetScenManager().Get_SCEN_HOME().SetReservedOpenUI(NKC_SCEN_HOME.RESERVE_OPEN_TYPE.ROT_GUIDE_MISSION, result18);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_TRIM:
		{
			NKMTrimIntervalTemplet nKMTrimIntervalTemplet = NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime);
			if (!NKCUITrimUtility.OpenTagEnabled || nKMTrimIntervalTemplet == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TRIM_NOT_INTERVAL_TIME);
			}
			else if (!IsContentsUnlocked(ContentsType.DIMENSION_TRIM))
			{
				ShowLockedMessagePopup(ContentsType.DIMENSION_TRIM);
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_TRIM)
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_TRIM, bForce: false);
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_EVENT_COLLECTION:
		{
			int result12;
			NKMEventCollectionIndexTemplet nKMEventCollectionIndexTemplet = ((!int.TryParse(shortCutParam, out result12) || result12 <= 0) ? NKCUIModuleLobby.GetEventCollectionIndexTemplet() : NKMTempletContainer<NKMEventCollectionIndexTemplet>.Find(result12));
			if (nKMEventCollectionIndexTemplet != null && nKMEventCollectionIndexTemplet.IsOpen)
			{
				if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
				{
					NKCUIModuleHome.OpenEventModule(nKMEventCollectionIndexTemplet);
					break;
				}
				NKCScenManager.GetScenManager().Get_SCEN_HOME().SetReservedOpenUI(NKC_SCEN_HOME.RESERVE_OPEN_TYPE.ROT_EVENT_COLLECTION, nKMEventCollectionIndexTemplet.Key);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_MENU_EXCEPTION_EVENT_EXPIRED_POPUP"));
			}
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_UI_PREFAB:
		{
			Dictionary<string, string> dictionary = NKCUtil.ParseStringTable(shortCutParam);
			if (!dictionary.TryGetValue("Prefab", out var value))
			{
				Debug.LogError("SHORTCUT_UI_PREFAB : 패러미터에 Prefab 형식이 존재하지 않음.");
				break;
			}
			NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName("", value);
			if (string.IsNullOrEmpty(nKMAssetName.m_BundleName))
			{
				Debug.LogError("SHORTCUT_UI_PREFAB : Prefab 항목 " + value + "의 Parse 실패");
				break;
			}
			NKCUIManager.eUIBaseRect parent = NKCUIManager.eUIBaseRect.UIFrontPopup;
			if (dictionary.TryGetValue("BaseRect", out var value2))
			{
				if (Enum.TryParse<NKCUIManager.eUIBaseRect>(value2, ignoreCase: true, out var result7))
				{
					parent = result7;
				}
				else
				{
					Debug.LogError("SHORTCUT_UI_PREFAB : BaseRect 항목 " + value2 + "의 파싱 실패");
				}
			}
			NKCUIBase instance = NKCUIManager.OpenNewInstance<NKCUIBase>(nKMAssetName, parent, null).GetInstance();
			instance.Initialize();
			instance.OpenByShortcut(dictionary);
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_HOME_EVENT_BANNER:
		{
			int.TryParse(shortCutParam, out var result6);
			NKCScenManager.GetScenManager().Get_SCEN_HOME().SetReservedOpenUI(NKC_SCEN_HOME.RESERVE_OPEN_TYPE.ROT_EVENT_BANNER, result6);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
			break;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_LOBBY_CHANGE:
			NKCScenManager.GetScenManager().SetActionAfterScenChange(NKCScenManager.GetScenManager().Get_SCEN_HOME().ForceOpenLobbyChange);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_PVP_EVENT:
			if (!IsContentsUnlocked(ContentsType.PVP))
			{
				ShowLockedMessagePopup(ContentsType.PVP);
				break;
			}
			if (!NKCEventPvpMgr.CanAccessEventPvp() || !NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ARCADE_MODE))
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENTMATCH_CANNOT_ENTER);
				break;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY, bForce: false);
			break;
		case NKM_SHORTCUT_TYPE.SHORTCUT_EVENT_RACE:
		{
			if (!NKCPopupEventRaceUtil.IsLastDay() && NKCPopupEventRaceUtil.IsMaintenanceTime())
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_RACE_BETTING_MAINTENANCE_TIME);
				break;
			}
			int.TryParse(shortCutParam, out var result);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
			{
				if (NKCPopupEventRaceV2.IsInstanceOpen)
				{
					NKCUIManager.SetAsTopmost(NKCPopupEventRaceV2.Instance);
				}
				NKCPopupEventRaceV2.Instance.Open(result);
			}
			else
			{
				NKCScenManager.GetScenManager().Get_SCEN_HOME().SetReservedOpenUI(NKC_SCEN_HOME.RESERVE_OPEN_TYPE.ROT_EVENT_RACE, result);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
			}
			break;
		}
		default:
			Debug.LogWarning($"정의되지 않은 숏컷 타입 - {shortCutType}");
			break;
		}
	}

	public static ContentsType GetContentsType(EPISODE_CATEGORY category)
	{
		switch (category)
		{
		case EPISODE_CATEGORY.EC_SIDESTORY:
			return ContentsType.SIDESTORY;
		case EPISODE_CATEGORY.EC_SUPPLY:
			return ContentsType.SUPPLY_MISSION;
		case EPISODE_CATEGORY.EC_DAILY:
			return ContentsType.DAILY;
		case EPISODE_CATEGORY.EC_COUNTERCASE:
			return ContentsType.COUNTERCASE;
		case EPISODE_CATEGORY.EC_FIELD:
			return ContentsType.FIELD;
		case EPISODE_CATEGORY.EC_EVENT:
		case EPISODE_CATEGORY.EC_TIMEATTACK:
		case EPISODE_CATEGORY.EC_SEASONAL:
			return ContentsType.EVENT;
		case EPISODE_CATEGORY.EC_CHALLENGE:
			return ContentsType.CHALLENGE;
		case EPISODE_CATEGORY.EC_SHADOW:
			return ContentsType.SHADOW_PALACE;
		case EPISODE_CATEGORY.EC_TRIM:
			return ContentsType.DIMENSION_TRIM;
		case EPISODE_CATEGORY.EC_FIERCE:
			return ContentsType.FIERCE;
		default:
			return ContentsType.None;
		}
	}

	public static bool CheckLevelChanged()
	{
		return m_bLevelChanged;
	}

	public static void SetLevelChanged(bool bValue)
	{
		m_bLevelChanged = bValue;
	}

	public static void RegisterCallback(NKMUserData userData)
	{
		if (userData != null)
		{
			userData.dOnUserLevelUpdate = (NKMUserData.OnUserLevelUpdate)Delegate.Combine(userData.dOnUserLevelUpdate, new NKMUserData.OnUserLevelUpdate(OnUserLevelChanged));
		}
	}

	private static void OnUserLevelChanged(NKMUserData userData)
	{
		SetUnlockedContent(STAGE_UNLOCK_REQ_TYPE.SURT_PLAYER_LEVEL, userData.UserLevel);
	}

	private static string MakeReviewDescription(NKCUnlockableContent content)
	{
		StringBuilder stringBuilder = new StringBuilder();
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			stringBuilder.Append("A");
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.IPhonePlayer:
		case RuntimePlatform.tvOS:
			stringBuilder.Append("I");
			break;
		}
		stringBuilder.Append((int)content.m_UnlockInfo.eReqType);
		if (!string.IsNullOrEmpty(content.m_UnlockInfo.reqValueStr))
		{
			stringBuilder.Append("@");
			stringBuilder.Append(content.m_UnlockInfo.reqValueStr);
		}
		if (content.m_UnlockInfo.reqValue != 0)
		{
			stringBuilder.Append("@");
			stringBuilder.Append(content.m_UnlockInfo.reqValue);
		}
		return stringBuilder.ToString();
	}
}
