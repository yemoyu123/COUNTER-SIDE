using NKC;
using NKC.UI;
using UnityEngine;

namespace NKM;

public class NKCUnitTouchObject
{
	private NKCAssetInstanceData m_UnitTouchObject;

	private RectTransform m_UnitTouchObject_RectTransform;

	private NKCUIComButton m_UnitTouchObject_NKCUIComButton;

	public void Init()
	{
		m_UnitTouchObject = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UNIT_GAME_NKM_UNIT", "NKM_GAME_UNIT_TOUCH");
		if (m_UnitTouchObject != null)
		{
			m_UnitTouchObject_RectTransform = m_UnitTouchObject.m_Instant.GetComponent<RectTransform>();
			m_UnitTouchObject_NKCUIComButton = m_UnitTouchObject.m_Instant.GetComponent<NKCUIComButton>();
			m_UnitTouchObject_RectTransform.transform.SetParent(NKCUIManager.m_NUF_GAME_TOUCH_OBJECT.transform, worldPositionStays: false);
			m_UnitTouchObject.m_Instant.SetActive(value: false);
		}
	}

	public void SetLinkButton(NKCUIComButton cLinkButton)
	{
		m_UnitTouchObject_NKCUIComButton.SetLinkButton(cLinkButton);
	}

	public void SetSize(NKMUnitTemplet cNKMUnitTemplet)
	{
		if (cNKMUnitTemplet.m_UnitSizeX > 200f)
		{
			m_UnitTouchObject_RectTransform.SetWidth(cNKMUnitTemplet.m_UnitSizeX);
		}
		if (cNKMUnitTemplet.m_fGageOffsetY > 300f)
		{
			m_UnitTouchObject_RectTransform.SetHeight(cNKMUnitTemplet.m_fGageOffsetY + 200f);
		}
	}

	public void Close()
	{
		m_UnitTouchObject_NKCUIComButton?.SetLinkButton(null);
		if (m_UnitTouchObject != null)
		{
			NKCUtil.SetGameobjectActive(m_UnitTouchObject.m_Instant, bValue: false);
		}
	}

	public void Unload()
	{
		NKCAssetResourceManager.CloseInstance(m_UnitTouchObject);
		m_UnitTouchObject = null;
		m_UnitTouchObject_RectTransform = null;
	}

	public void ActiveObject(bool bActive)
	{
		if (bActive)
		{
			if (!m_UnitTouchObject.m_Instant.activeSelf)
			{
				m_UnitTouchObject.m_Instant.SetActive(value: true);
			}
		}
		else if (m_UnitTouchObject.m_Instant.activeSelf)
		{
			m_UnitTouchObject.m_Instant.SetActive(value: false);
		}
	}

	public bool IsActiveObject()
	{
		return m_UnitTouchObject.m_Instant.activeSelf;
	}

	public GameObject GetGameObject()
	{
		return m_UnitTouchObject.m_Instant;
	}

	public RectTransform GetRectTransform()
	{
		return m_UnitTouchObject_RectTransform;
	}

	public NKCUIComButton GetButton()
	{
		return m_UnitTouchObject_NKCUIComButton;
	}

	public void MoveToLastTouchObject()
	{
		m_UnitTouchObject_RectTransform.SetAsLastSibling();
	}
}
