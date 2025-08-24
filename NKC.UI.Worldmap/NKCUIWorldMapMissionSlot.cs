using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCUIWorldMapMissionSlot : MonoBehaviour
{
	public delegate void OnClickSlot(int missionID);

	[Header("Object")]
	public Image m_imgMissionBG;

	public Image m_imgMissinRank;

	public Text m_lbName;

	public Text m_lbRequiredLevel;

	public Image m_imgMissionType;

	public Text m_lbMissionType;

	public Text m_lbTime;

	public List<NKCUISlot> m_lstItemSlot;

	public NKCUISlot m_CompleteRewardItemSlot;

	[Header("Resource_MissionType")]
	public Sprite m_spMissionExplore;

	public Sprite m_spMissionDefence;

	public Sprite m_spMissionMining;

	public Sprite m_spMissionOffice;

	[Header("Resource_MissionRank")]
	public Sprite m_spMissionRankS;

	public Sprite m_spMissionRankA;

	public Sprite m_spMissionRankB;

	public Sprite m_spMissionRankC;

	public NKCUIComStateButton m_cbtnMissionSelect;

	private OnClickSlot dOnClickSlot;

	public int MissionID { get; private set; }

	public void Init(OnClickSlot onClick)
	{
		m_cbtnMissionSelect.PointerClick.AddListener(OnBtnClicked);
		dOnClickSlot = onClick;
		foreach (NKCUISlot item in m_lstItemSlot)
		{
			item.Init();
		}
		m_CompleteRewardItemSlot.Init();
	}

	public void SetData(NKMWorldMapMissionTemplet missionTemplet, int leaderLevel = -1)
	{
		if (missionTemplet == null)
		{
			MissionID = -1;
			return;
		}
		MissionID = missionTemplet.m_ID;
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_RENEWAL_MISSION_THUMBNAIL", missionTemplet.m_WorldMapMissionThumbnailFile);
		if (orLoadAssetResource != null)
		{
			NKCUtil.SetImageSprite(m_imgMissionBG, orLoadAssetResource);
		}
		else
		{
			Debug.LogWarning($"MissionBG sprite not found. mission id {MissionID}, spriteName {missionTemplet.m_WorldMapMissionThumbnailFile}");
		}
		NKCUtil.SetImageSprite(m_imgMissionType, GetMissionTypeSprite(missionTemplet.m_eMissionType), bDisableIfSpriteNull: true);
		NKCUtil.SetLabelText(m_lbMissionType, NKCUtilString.GetWorldMapMissionType(missionTemplet.m_eMissionType));
		NKCUtil.SetImageSprite(m_imgMissinRank, GetMissionRankSprite(missionTemplet.m_eMissionRank), bDisableIfSpriteNull: true);
		NKCUtil.SetLabelText(m_lbName, missionTemplet.GetMissionName());
		NKCUtil.SetLabelText(m_lbTime, NKCUtilString.GetTimeStringFromMinutes(missionTemplet.m_MissionTimeInMinutes));
		NKCUtil.SetLabelText(m_lbRequiredLevel, string.Format(NKCUtilString.GET_STRING_WORLDMAP_CITY_MISSION_REQ_LEVEL_ONE_PARAM, missionTemplet.m_ReqManagerLevel));
		if (m_lbRequiredLevel != null)
		{
			if (leaderLevel >= 0 && leaderLevel < missionTemplet.m_ReqManagerLevel)
			{
				m_lbRequiredLevel.color = Color.red;
			}
			else
			{
				m_lbRequiredLevel.color = new Color(0.6037f, 0.6037f, 0.6037f);
			}
		}
		SetMissionReward(missionTemplet);
	}

	private void SetMissionReward(NKMWorldMapMissionTemplet missionTemplet)
	{
		List<NKCUISlot.SlotData> list = MakeSlotDataListFromMissionTemplet(missionTemplet);
		for (int i = 0; i < m_lstItemSlot.Count; i++)
		{
			NKCUISlot nKCUISlot = m_lstItemSlot[i];
			if (i < list.Count)
			{
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
				nKCUISlot.SetData(list[i]);
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: false);
			}
		}
		NKCUISlot.SlotData slotData = MakeCompleteRewardSlotData(missionTemplet);
		if (slotData != null)
		{
			NKCUtil.SetGameobjectActive(m_CompleteRewardItemSlot, bValue: true);
			m_CompleteRewardItemSlot.SetData(slotData, bShowName: false, bShowNumber: true, bEnableLayoutElement: true, null);
			m_CompleteRewardItemSlot.SetAdditionalText(NKCUtilString.GET_STRING_WORLDMAP_CITY_MISSION_REWARD_ADD_TEXT);
			m_CompleteRewardItemSlot.SetOpenItemBoxOnClick();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_CompleteRewardItemSlot, bValue: false);
		}
	}

	private List<NKCUISlot.SlotData> MakeSlotDataListFromMissionTemplet(NKMWorldMapMissionTemplet missionTemplet)
	{
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		if (missionTemplet.m_RewardUnitExp > 0)
		{
			NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeMiscItemData(502, missionTemplet.m_RewardUnitExp);
			list.Add(item);
		}
		if (missionTemplet.m_RewardCredit > 0)
		{
			NKCUISlot.SlotData item2 = NKCUISlot.SlotData.MakeMiscItemData(1, missionTemplet.m_RewardCredit);
			list.Add(item2);
		}
		if (missionTemplet.m_RewardEternium > 0)
		{
			NKCUISlot.SlotData item3 = NKCUISlot.SlotData.MakeMiscItemData(2, missionTemplet.m_RewardEternium);
			list.Add(item3);
		}
		if (missionTemplet.m_RewardInformation > 0)
		{
			NKCUISlot.SlotData item4 = NKCUISlot.SlotData.MakeMiscItemData(3, missionTemplet.m_RewardInformation);
			list.Add(item4);
		}
		return list;
	}

	private NKCUISlot.SlotData MakeCompleteRewardSlotData(NKMWorldMapMissionTemplet missionTemplet)
	{
		if (missionTemplet.m_CompleteRewardQuantity > 0)
		{
			return NKCUISlot.SlotData.MakeRewardTypeData(missionTemplet.m_CompleteRewardType, missionTemplet.m_CompleteRewardID, missionTemplet.m_CompleteRewardQuantity);
		}
		return null;
	}

	private void SetSlotData(NKCUISlot slot, int itemID, int count)
	{
		slot.SetMiscItemData(itemID, count, bShowName: false, bShowCount: true, bEnableLayoutElement: false, null);
	}

	private Sprite GetMissionTypeSprite(NKMWorldMapMissionTemplet.WorldMapMissionType type)
	{
		return type switch
		{
			NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_DEFENCE => m_spMissionDefence, 
			NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_EXPLORE => m_spMissionExplore, 
			NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_MINING => m_spMissionMining, 
			NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_OFFICE => m_spMissionOffice, 
			_ => m_spMissionDefence, 
		};
	}

	private Sprite GetMissionRankSprite(NKMWorldMapMissionTemplet.WorldMapMissionRank rank)
	{
		return rank switch
		{
			NKMWorldMapMissionTemplet.WorldMapMissionRank.WMMR_S => m_spMissionRankS, 
			NKMWorldMapMissionTemplet.WorldMapMissionRank.WMMR_A => m_spMissionRankA, 
			NKMWorldMapMissionTemplet.WorldMapMissionRank.WMMR_B => m_spMissionRankB, 
			NKMWorldMapMissionTemplet.WorldMapMissionRank.WMMR_C => m_spMissionRankC, 
			_ => m_spMissionRankC, 
		};
	}

	private void OnBtnClicked()
	{
		if (!m_cbtnMissionSelect.m_bLock && dOnClickSlot != null)
		{
			dOnClickSlot(MissionID);
		}
	}
}
