using ClientPacket.Common;
using ClientPacket.Guild;
using UnityEngine;

namespace NKC.UI.Guild;

public class NKCUIGuildMemberSlot : MonoBehaviour
{
	public delegate void OnSelectedSlot(long userUid);

	public NKCUIComStateButton m_btnSlot;

	public NKCUIGuildMemberSlotNormal m_slotNormal;

	public NKCUIGuildMemberSlotRequest m_slotRequest;

	private OnSelectedSlot m_dOnSelectedSlot;

	private NKCAssetInstanceData m_instance;

	private long m_Useruid;

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
	}

	public static NKCUIGuildMemberSlot GetNewInstance(Transform parent, OnSelectedSlot selectedSlot)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_MEMBER_SLOT");
		NKCUIGuildMemberSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIGuildMemberSlot>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIGuildMemberSlot Prefab null!");
			return null;
		}
		component.m_instance = nKCAssetInstanceData;
		component.SetOnSelectedSlot(selectedSlot);
		component.InitUI();
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	private void InitUI()
	{
		m_slotNormal.InitUI();
		m_slotRequest.InitUI();
		m_btnSlot.PointerClick.RemoveAllListeners();
		m_btnSlot.PointerClick.AddListener(OnClickSlot);
	}

	private void SetOnSelectedSlot(OnSelectedSlot onSelectedSlot)
	{
		m_dOnSelectedSlot = onSelectedSlot;
	}

	public void SetData(NKMGuildMemberData guildMemberData, bool bIsMyGuild)
	{
		m_Useruid = guildMemberData.commonProfile.userUid;
		NKCUtil.SetGameobjectActive(m_slotNormal, bValue: true);
		NKCUtil.SetGameobjectActive(m_slotRequest, bValue: false);
		m_slotNormal.SetData(guildMemberData, bIsMyGuild);
	}

	public void SetData(FriendListData userData, NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE lobbyUIType)
	{
		m_Useruid = userData.commonProfile.userUid;
		NKCUtil.SetGameobjectActive(m_slotNormal, bValue: false);
		NKCUtil.SetGameobjectActive(m_slotRequest, bValue: true);
		m_slotRequest.SetData(userData, lobbyUIType);
	}

	private void OnClickSlot()
	{
		m_dOnSelectedSlot?.Invoke(m_Useruid);
	}
}
