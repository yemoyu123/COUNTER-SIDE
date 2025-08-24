using ClientPacket.Community;
using ClientPacket.User;
using NKC.UI;
using NKC.UI.Collection;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_COLLECTION : NKC_SCEN_BASIC
{
	private NKCUICollectionGeneral m_NKCUICollection;

	private NKCUIManager.LoadedUIData m_UICollectionLoadData;

	private NKCUICollectionGeneral.CollectionType m_eReserveState = NKCUICollectionGeneral.CollectionType.CT_NONE;

	private string m_ReserveUnitStrID = "";

	public NKC_SCEN_COLLECTION()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_COLLECTION;
	}

	public void SetOpenReserve(NKCUICollectionGeneral.CollectionType reciveType, string targetUnitStrID)
	{
		m_eReserveState = reciveType;
		m_ReserveUnitStrID = targetUnitStrID;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		NKCCollectionManager.Init();
		if (!NKCUIManager.IsValid(m_UICollectionLoadData))
		{
			m_NKCUICollection = null;
			bool flag = false;
			if (NKCDefineManager.DEFINE_UNITY_EDITOR() && Input.GetKey(KeyCode.LeftControl))
			{
				flag = true;
			}
			if (NKCCollectionManager.IsCollectionV2Active && !flag)
			{
				m_UICollectionLoadData = NKCUIManager.OpenNewInstanceAsync<NKCUICollectionV2>("ab_ui_collection", "AB_UI_COLLECTION", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), null);
			}
			else
			{
				m_UICollectionLoadData = NKCUIManager.OpenNewInstanceAsync<NKCUICollection>("ab_ui_nkm_ui_collection", "NKM_UI_COLLECTION", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), null);
			}
		}
		NKCUnitMissionManager.Init();
	}

	public NKM_SHORTCUT_TYPE GetCurrentShortcutType()
	{
		if (m_NKCUICollection != null && m_NKCUICollection.IsOpen)
		{
			return m_NKCUICollection.GetShortcutType();
		}
		return NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION;
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_NKCUICollection == null)
		{
			if (m_UICollectionLoadData != null)
			{
				if (!m_UICollectionLoadData.IsLoadComplete)
				{
					Debug.LogError("Error - NKC_SCEN_COLLECTION.ScenLoadComplete() : UICollectionLoadResourceData.IsDone() is false");
				}
				else if (m_UICollectionLoadData.CheckLoadAndGetInstance<NKCUICollectionGeneral>(out m_NKCUICollection))
				{
					m_NKCUICollection.Init();
				}
			}
		}
		else
		{
			Debug.LogError("Error - NKC_SCEN_COLLECTION.ScenLoadComplete() : UICollectionLoadResourceData is null");
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_NKCUICollection.Open(m_eReserveState, m_ReserveUnitStrID);
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_NKCUICollection != null)
		{
			m_NKCUICollection.Close();
		}
		m_UICollectionLoadData?.CloseInstance();
		m_UICollectionLoadData = null;
		m_NKCUICollection = null;
		if (NKCUICutScenPlayer.HasInstance)
		{
			NKCUICutScenPlayer.Instance.StopWithInvalidatingCallBack();
			NKCUICutScenPlayer.Instance.UnLoad();
		}
		m_eReserveState = NKCUICollectionGeneral.CollectionType.CT_NONE;
		m_ReserveUnitStrID = "";
	}

	public void OnRecvReviewTagVoteCancelAck(NKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_ACK sPacket)
	{
		if (m_NKCUICollection != null && m_NKCUICollection.IsOpen)
		{
			m_NKCUICollection.OnRecvReviewTagVoteCancelAck(sPacket);
		}
	}

	public void OnRecvReviewTagVoteAck(NKMPacket_UNIT_REVIEW_TAG_VOTE_ACK sPacket)
	{
		if (m_NKCUICollection != null && m_NKCUICollection.IsOpen)
		{
			m_NKCUICollection.OnRecvReviewTagVoteAck(sPacket);
		}
	}

	public void OnRecvReviewTagListAck(NKMPacket_UNIT_REVIEW_TAG_LIST_ACK sPacket)
	{
		if (m_NKCUICollection != null && m_NKCUICollection.IsOpen)
		{
			m_NKCUICollection.OnRecvReviewTagListAck(sPacket);
		}
	}

	public void OnRecvTeamCollectionRewardAck(NKMPacket_TEAM_COLLECTION_REWARD_ACK sPacket)
	{
		if (m_NKCUICollection != null && m_NKCUICollection.IsOpen)
		{
			m_NKCUICollection.OnRecvTeamCollectionRewardAck(sPacket);
		}
	}

	public void OnRecvUnitMissionReward(int unitId)
	{
		if (m_NKCUICollection != null)
		{
			m_NKCUICollection.OnRecvUnitMissionReward(unitId);
		}
	}

	public void OnRecvMiscCollectionReward()
	{
		if (m_NKCUICollection != null)
		{
			NKCUICollectionV2 nKCUICollectionV = m_NKCUICollection as NKCUICollectionV2;
			if (nKCUICollectionV != null)
			{
				nKCUICollectionV.OnRecvMiscCollectionReward();
			}
		}
	}
}
