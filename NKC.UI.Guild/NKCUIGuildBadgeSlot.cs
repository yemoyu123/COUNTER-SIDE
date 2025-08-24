using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildBadgeSlot : MonoBehaviour
{
	public delegate void OnSelectedSlot(int id);

	public NKCUIComToggle m_tgl;

	public Image m_imgColor;

	public Image m_imgFrame;

	public Image m_imgMark;

	public GameObject m_objSelected;

	public GameObject m_objLocked;

	private OnSelectedSlot m_dOnSelectedSlot;

	private NKCAssetInstanceData m_instance;

	public int m_slotId { get; private set; }

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
	}

	public static NKCUIGuildBadgeSlot GetNewInstance(Transform parent, NKCUIComToggleGroup tglGroup, OnSelectedSlot selectedSlot = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_MARK_SETTING_SLOT");
		NKCUIGuildBadgeSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIGuildBadgeSlot>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIGuildBadgeSlot Prefab null!");
			return null;
		}
		component.m_instance = nKCAssetInstanceData;
		component.SetOnSelectedSlot(selectedSlot);
		component.m_tgl.SetToggleGroup(tglGroup);
		component.m_tgl.OnValueChanged.RemoveAllListeners();
		component.m_tgl.OnValueChanged.AddListener(component.OnValueChanged);
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void SetData(NKMGuildBadgeFrameTemplet templet)
	{
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_imgFrame, bValue: true);
		NKCUtil.SetGameobjectActive(m_imgMark, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgColor, bValue: false);
		NKCUtil.SetImageSprite(m_imgFrame, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Mark", templet.BadgeFrameImg));
		m_slotId = templet.ID;
		SetLockObject(templet.UnlockInfo, templet.LockVisible);
	}

	public void SetData(NKMGuildBadgeMarkTemplet templet)
	{
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_imgFrame, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgMark, bValue: true);
		NKCUtil.SetGameobjectActive(m_imgColor, bValue: false);
		NKCUtil.SetImageSprite(m_imgMark, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Mark", templet.BadgeMarkImg));
		m_slotId = templet.ID;
		SetLockObject(templet.UnlockInfo, templet.LockVisible);
	}

	public void SetData(NKMGuildBadgeColorTemplet templet)
	{
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_imgFrame, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgMark, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgColor, bValue: true);
		NKCUtil.SetImageColor(m_imgColor, NKCUtil.GetColor(templet.BadgeColorCode));
		m_slotId = templet.ID;
		SetLockObject(templet.UnlockInfo, templet.LockVisible);
	}

	private void SetLockObject(UnlockInfo unlockInfo, bool bLockVisible)
	{
		if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in unlockInfo))
		{
			if (bLockVisible)
			{
				NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objLocked, bValue: false);
		}
	}

	private void SetOnSelectedSlot(OnSelectedSlot selectedSlot)
	{
		m_dOnSelectedSlot = selectedSlot;
	}

	private void OnValueChanged(bool bValue)
	{
		if (bValue)
		{
			m_dOnSelectedSlot(m_slotId);
		}
	}
}
