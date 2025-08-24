using ClientPacket.Raid;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIRaidSupportUserSlot : MonoBehaviour
{
	public NKCUISlot m_NKCUISlotUnit;

	public Text m_lbUserName;

	public Text m_lbUID;

	public Text m_lbDamageAmount;

	public Text m_lbDamagePercent;

	public GameObject m_objHighScore;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_GuildBadgeUI;

	public Text m_lbGuildName;

	public Text m_lbTryCount;

	public Text m_lbRanking;

	public NKCUIComTitlePanel m_TitlePanel;

	private NKCAssetInstanceData m_InstanceData;

	public static NKCUIRaidSupportUserSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WORLD_MAP_RAID", "NKM_UI_WORLD_MAP_RAID_SUPPORTSLOT");
		NKCUIRaidSupportUserSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIRaidSupportUserSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIRaidSupportUserSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.transform.localScale = new Vector3(1f, 1f, 1f);
		component.Init();
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	private void Init()
	{
		m_NKCUISlotUnit.Init();
	}

	public void SetUI(NKMRaidDetailData cRaidData, NKMRaidJoinData joinData, int ranking)
	{
		if (cRaidData == null || joinData == null)
		{
			return;
		}
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(cRaidData.stageID);
		if (nKMRaidTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbTryCount, $"{joinData.tryCount}/{nKMRaidTemplet.RaidTryCount}");
			NKCUtil.SetLabelText(m_lbRanking, ranking.ToString());
			bool num = NKCScenManager.CurrentUserData().m_UserUID == joinData.userUID;
			m_lbUserName.text = joinData.nickName;
			m_lbUID.text = NKCUtilString.GetFriendCode(joinData.friendCode);
			if (num)
			{
				m_lbUserName.color = NKCUtil.GetColor("#FFCF3B");
			}
			else
			{
				m_lbUserName.color = Color.white;
			}
			int num2 = (int)joinData.damage;
			float num3 = 0f;
			m_lbDamageAmount.text = num2.ToString("N0");
			float maxHP = cRaidData.maxHP;
			if (maxHP != 0f)
			{
				num3 = joinData.damage / maxHP * 100f;
			}
			m_lbDamagePercent.text = $"{num3:0.##}" + "%";
			SetGuildData(joinData);
			m_NKCUISlotUnit.SetUnitData(joinData.mainUnitID, 1, joinData.mainUnitSkinID, bShowName: false, bShowLevel: false, bEnableLayoutElement: true, null);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_objHighScore, joinData.highScore);
			m_TitlePanel?.SetData(joinData);
		}
	}

	private void SetGuildData(NKMRaidJoinData data)
	{
		if (m_objGuild != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, data.guildData != null && data.guildData.guildUid > 0);
			if (m_objGuild.activeSelf)
			{
				m_GuildBadgeUI.SetData(data.guildData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, data.guildData.guildName);
			}
		}
	}
}
