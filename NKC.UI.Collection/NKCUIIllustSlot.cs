using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUIIllustSlot : MonoBehaviour
{
	public GameObject m_NKM_UI_COLLECTION_ALBUM_SLOT_LOCK;

	public GameObject m_NKM_UI_COLLECTION_ALBUM_SLOT_OPEN;

	public Text m_NKM_UI_COLLECTION_ALBUM_SLOT_ALBUM_TITLE_TEXT;

	public Image m_img_NKM_UI_COLLECTION_ALBUM_SLOT_BG_THUMBNAIL;

	public NKCUIComStateButton m_btn_NKM_UI_COLLECTION_ALBUM_SLOT_OPEN;

	private int m_iCategoryID;

	private int m_iBGGroupID;

	private NKCUICollectionIllust.OnIllustView OnIllustView;

	private NKCAssetInstanceData m_AssetInstanceData;

	public void Init(int categoryID, int BGGroupID, NKCUICollectionIllust.OnIllustView callback)
	{
		if (null != m_btn_NKM_UI_COLLECTION_ALBUM_SLOT_OPEN)
		{
			m_btn_NKM_UI_COLLECTION_ALBUM_SLOT_OPEN.PointerClick.RemoveAllListeners();
			m_btn_NKM_UI_COLLECTION_ALBUM_SLOT_OPEN.PointerClick.AddListener(ShowIllustView);
		}
		m_iCategoryID = categoryID;
		m_iBGGroupID = BGGroupID;
		OnIllustView = callback;
		base.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
	}

	private void ShowIllustView()
	{
		if (OnIllustView != null)
		{
			OnIllustView(m_iCategoryID, m_iBGGroupID);
		}
	}

	public void SetData(NKCCollectionIllustData data = null)
	{
		if (data == null)
		{
			return;
		}
		if (!data.IsClearMission())
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_ALBUM_SLOT_LOCK, bValue: true);
			return;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_ALBUM_SLOT_LOCK, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_ALBUM_SLOT_OPEN, bValue: true);
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_ALBUM_SLOT_ALBUM_TITLE_TEXT, data.m_BGGroupTitle);
		if (data.m_FileData.Count >= 1)
		{
			NKCUtil.SetImageSprite(m_img_NKM_UI_COLLECTION_ALBUM_SLOT_BG_THUMBNAIL, GetThumbnail(data.m_FileData[0].m_BGThumbnailFileName));
		}
	}

	private Sprite GetThumbnail(string name)
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_collection_thumbnail", name);
	}

	public void Clear()
	{
		m_AssetInstanceData.Unload();
	}
}
