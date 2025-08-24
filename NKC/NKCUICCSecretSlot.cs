using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUICCSecretSlot : MonoBehaviour
{
	public delegate void OnSelectedCCSSlot(int actID);

	public Image m_AB_UI_NKM_UI_COUNTER_CASE_SECRET_SLOT_UNIT_IMAGE;

	public GameObject m_NKM_UI_COUNTER_CASE_SECRET_SLOT_LOCK;

	public NKCUIComButton m_comBtn;

	public RectTransform m_RTButton;

	public Animator m_Animator;

	private float m_fOrgHalfWidth;

	private OnSelectedCCSSlot m_OnSelectedSlot;

	private int m_ActID;

	private bool m_bBookOpen;

	public const float SIZE_X = 642f;

	public const float SIZE_OFFSET_X = 70f;

	public const float POS_OFFSET_X = 960f;

	public const float POS_OFFSET_X_BY_VIEWPORT = -300f;

	private int m_Index = -1;

	public void SetOnSelectedItemSlot(OnSelectedCCSSlot _OnSelectedSlot)
	{
		m_OnSelectedSlot = _OnSelectedSlot;
	}

	public int GetActID()
	{
		return m_ActID;
	}

	public bool IsBookOpen()
	{
		return m_bBookOpen;
	}

	public void SetBookOpen(bool bSet)
	{
		m_bBookOpen = bSet;
	}

	public void ResetPos()
	{
		m_RTButton.anchoredPosition = new Vector2(660f + (float)m_Index * 572f, 0f);
	}

	public static NKCUICCSecretSlot GetNewInstance(int index, Transform parent, OnSelectedCCSSlot dOnSelectedItemSlot)
	{
		NKCUICCSecretSlot component = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_COUNTER_CASE", "NKM_UI_COUNTER_CASE_SECRET_SLOT").m_Instant.GetComponent<NKCUICCSecretSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUICCSecretSlot Prefab null!");
			return null;
		}
		component.m_Index = index;
		component.SetOnSelectedItemSlot(dOnSelectedItemSlot);
		component.m_comBtn.PointerClick.RemoveAllListeners();
		component.m_comBtn.PointerClick.AddListener(component.OnSelectedSlotImpl);
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.ResetPos();
		Vector3 localScale = new Vector3(0.9f, 0.9f, 1f);
		component.m_RTButton.localScale = localScale;
		component.m_fOrgHalfWidth = component.m_RTButton.sizeDelta.x / 2f;
		component.ResetPos();
		component.gameObject.SetActive(value: false);
		return component;
	}

	public float GetHalfOfWidth()
	{
		return m_fOrgHalfWidth;
	}

	public float GetCenterX()
	{
		return m_RTButton.anchoredPosition.x + m_RTButton.sizeDelta.x / 2f;
	}

	public void SetData(NKMEpisodeTempletV2 cNKMEpisodeTemplet, int actID, bool bLock = false)
	{
		m_ActID = actID;
		if (cNKMEpisodeTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_SECRET_SLOT_LOCK, bLock);
			SetActive(bSet: true);
		}
	}

	public void OnSelectedSlotImpl()
	{
		if (m_OnSelectedSlot != null)
		{
			m_OnSelectedSlot(m_ActID);
		}
	}

	public void SetActive(bool bSet)
	{
		if (base.gameObject.activeSelf == !bSet)
		{
			base.gameObject.SetActive(bSet);
		}
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}
}
