using ClientPacket.LeaderBoard;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCUIFierceBattleBossPersonalRankSlot : MonoBehaviour
{
	private NKCAssetInstanceData m_InstanceData;

	public NKCUISlot m_userProfile;

	public Text m_Ranking;

	public Text m_UserName;

	public Text m_Score;

	private int m_bossGroupID;

	private long m_UserUID;

	public static NKCUIFierceBattleBossPersonalRankSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_fierce_battle", "NKM_UI_POPUP_FIERCE_BATTLE_BOSS_PERSONAL_RANK_SLOT");
		NKCUIFierceBattleBossPersonalRankSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIFierceBattleBossPersonalRankSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIFierceBattleBossPersonalRankSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.Init();
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.transform.localScale = new Vector3(1f, 1f, 1f);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	public void Init()
	{
		if (m_userProfile != null)
		{
			m_userProfile.Init();
		}
	}

	public void SetData(NKMFierceData fierceData, int Rank)
	{
		NKCUtil.SetLabelText(m_Ranking, "#" + Rank);
		NKCUtil.SetLabelText(m_UserName, fierceData.commonProfile.nickname.ToString());
		NKCUtil.SetLabelText(m_Score, fierceData.fiercePoint.ToString("#,##0"));
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeUnitData(fierceData.commonProfile.mainUnitId, fierceData.commonProfile.level, fierceData.commonProfile.mainUnitSkinId);
		m_userProfile.SetData(data, bEnableLayoutElement: true, OnSlotClick);
		m_UserUID = fierceData.commonProfile.userUid;
	}

	private void OnSlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (NKCPopupFierceUserInfo.IsHasInstance() && NKCPopupFierceUserInfo.Instance.IsSameProfile(m_UserUID))
		{
			NKCPopupFierceUserInfo.Instance.Open(null);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_FIERCE_PROFILE_REQ(m_UserUID, bForce: true);
		}
	}
}
