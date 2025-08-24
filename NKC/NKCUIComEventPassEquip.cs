using NKM;
using NKM.EventPass;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIComEventPassEquip : MonoBehaviour
{
	public Image m_imgEquipIcon;

	public Image m_imgEquipType;

	private NKCAssetInstanceData m_NKCAssetInstanceData;

	public static NKCUIComEventPassEquip GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_EVENT_PASS", "NKM_UI_EVENT_PASS_EQUIP");
		NKCUIComEventPassEquip component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIComEventPassEquip>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKM_UI_EVENT_PASS_EQUIP Prefab null!");
			return null;
		}
		component.m_NKCAssetInstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void SetData(NKMEventPassTemplet eventPassTemplet)
	{
		if (eventPassTemplet != null)
		{
			switch (eventPassTemplet.EventPassMainRewardType)
			{
			case NKM_REWARD_TYPE.RT_EQUIP:
			{
				NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(eventPassTemplet.EventPassMainReward);
				NKCUtil.SetImageSprite(m_imgEquipIcon, NKCResourceUtility.GetOrLoadEquipIcon(equipTemplet));
				NKCUtil.SetImageSprite(m_imgEquipType, NKCResourceUtility.GetOrLoadUnitStyleIcon(equipTemplet.m_EquipUnitStyleType));
				break;
			}
			case NKM_REWARD_TYPE.RT_MISC:
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(eventPassTemplet.EventPassMainReward);
				NKCUtil.SetImageSprite(m_imgEquipIcon, NKCResourceUtility.GetOrLoadMiscItemIcon(itemMiscTempletByID));
				NKCUtil.SetImageSprite(m_imgEquipType, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_ICON", "NKM_UI_COMMON_UNIT_TYPE_ETC"));
				break;
			}
			case NKM_REWARD_TYPE.RT_MOLD:
			{
				NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(eventPassTemplet.EventPassMainReward);
				NKCUtil.SetImageSprite(m_imgEquipIcon, NKCResourceUtility.GetOrLoadMoldIcon(itemMoldTempletByID));
				NKCUtil.SetImageSprite(m_imgEquipType, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_ICON", "NKM_UI_COMMON_UNIT_TYPE_ETC"));
				break;
			}
			case NKM_REWARD_TYPE.RT_USER_EXP:
				break;
			}
		}
	}

	private void OnDestroy()
	{
		m_imgEquipIcon = null;
		m_imgEquipType = null;
		NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceData);
	}
}
