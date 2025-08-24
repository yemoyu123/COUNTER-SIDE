using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCPopupEmoticonSlotComment : MonoBehaviour
{
	public delegate void dOnClick(int emoticonID);

	public Text m_lbComment;

	public NKCUIComStateButton m_csbtnComment;

	public NKCUIComStateButton m_csbtnCommentForChange;

	public GameObject m_objSelected;

	public Animator m_amtorChangeEffect;

	public Image m_imgBG;

	public Sprite m_spBGWhite;

	public Sprite m_spBGGold;

	public GameObject m_objFavorite;

	private int m_EmoticonID;

	private dOnClick m_dOnClick;

	private dOnClick m_dOnClickForChange;

	private NKCAssetInstanceData m_cNKCAssetInstanceData;

	public void SetClickEvent(dOnClick _dOnClick)
	{
		m_dOnClick = _dOnClick;
	}

	public void SetClickEventForChange(dOnClick _dOnClickForChange)
	{
		m_dOnClickForChange = _dOnClickForChange;
	}

	public static NKCPopupEmoticonSlotComment GetNewInstanceLarge(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_EMOTICON", "NKM_UI_EMOTICON_SLOT_COMMENT_LARGE");
		NKCPopupEmoticonSlotComment component = nKCAssetInstanceData.m_Instant.GetComponent<NKCPopupEmoticonSlotComment>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCPopupEmoticonSlotComment Prefab null!");
			return null;
		}
		component.m_cNKCAssetInstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.gameObject.SetActive(value: false);
		return component;
	}

	private void Start()
	{
		m_csbtnComment.PointerClick.RemoveAllListeners();
		m_csbtnComment.PointerClick.AddListener(OnClick);
		m_csbtnComment.dOnPointerHolding = OnClickFavorite;
		if (m_csbtnCommentForChange != null)
		{
			m_csbtnCommentForChange.PointerClick.RemoveAllListeners();
			m_csbtnCommentForChange.PointerClick.AddListener(OnClickForChange);
		}
	}

	public void PlayChangeEffect()
	{
		if (m_amtorChangeEffect != null)
		{
			m_amtorChangeEffect.Play("NKM_UI_COMMENT_CHANGE_BASE", -1, 0f);
		}
	}

	public int GetEmoticonID()
	{
		return m_EmoticonID;
	}

	public void SetUI(int emoticonID)
	{
		m_EmoticonID = emoticonID;
		NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(m_EmoticonID);
		if (nKMEmoticonTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbComment, nKMEmoticonTemplet.GetEmoticonName());
			if (m_imgBG != null)
			{
				if (nKMEmoticonTemplet.m_EmoticonaAnimationName == "WHITE")
				{
					m_imgBG.sprite = m_spBGWhite;
				}
				else if (nKMEmoticonTemplet.m_EmoticonaAnimationName == "GOLD")
				{
					m_imgBG.sprite = m_spBGGold;
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objFavorite, NKCEmoticonManager.IsFavorite(emoticonID));
	}

	private void OnClick()
	{
		if (m_dOnClick != null)
		{
			m_dOnClick(m_EmoticonID);
		}
	}

	private void OnClickForChange()
	{
		if (m_dOnClickForChange != null)
		{
			m_dOnClickForChange(m_EmoticonID);
		}
	}

	public void SetSelected(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objSelected, bSet);
	}

	public bool GetSelected()
	{
		return m_objSelected.activeSelf;
	}

	private void OnClickFavorite()
	{
		if (!(m_objFavorite == null))
		{
			NKCPacketSender.Send_NKMPacket_EMOTICON_FAVORITES_SET_REQ(m_EmoticonID, !m_objFavorite.activeSelf);
		}
	}
}
