using NKC.Office;
using NKM;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component.Office;

public class NKCUIComOfficeFurniturePreview : MonoBehaviour
{
	public Text m_lbName;

	public Text m_lbDescription;

	public GameObject m_objEnvPoint;

	public Text m_lbEnvPoint;

	public RectTransform m_rtFurniturePreviewViewport;

	public RectTransform m_rtFurnitureRoot;

	public Image m_imgBackgroundIcon;

	public bool bShowTile;

	private NKCOfficeFuniture m_furniturePreview;

	public void SetData(NKMOfficeInteriorTemplet templet)
	{
		Clear();
		NKCUtil.SetGameobjectActive(m_lbName, templet != null);
		NKCUtil.SetGameobjectActive(m_lbDescription, templet != null);
		NKCUtil.SetGameobjectActive(m_objEnvPoint, templet != null);
		if (templet == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_lbName, templet.GetItemName());
		NKCUtil.SetLabelText(m_lbDescription, templet.GetItemDesc());
		NKCUtil.SetLabelText(m_lbEnvPoint, templet.InteriorScore.ToString());
		NKCUtil.SetGameobjectActive(m_rtFurnitureRoot, templet.Target != InteriorTarget.Background);
		NKCUtil.SetGameobjectActive(m_imgBackgroundIcon, templet.Target == InteriorTarget.Background);
		if (templet.Target == InteriorTarget.Background)
		{
			NKCUtil.SetImageSprite(m_imgBackgroundIcon, NKCResourceUtility.GetOrLoadMiscItemIcon(templet));
			return;
		}
		m_rtFurnitureRoot.localScale = Vector3.one;
		m_furniturePreview = NKCOfficeFuniture.GetInstance(-1L, templet, 100f, m_rtFurnitureRoot);
		if (m_furniturePreview == null)
		{
			Debug.LogError("Furniture prefab not exist! itemID : " + templet.m_ItemMiscID);
			return;
		}
		m_furniturePreview.SetShowTile(bShowTile);
		if (m_furniturePreview != null)
		{
			m_furniturePreview.SetInvert(bInvert: false, bEditMode: true);
			m_furniturePreview.transform.localPosition = Vector3.zero;
		}
		FitFurnitureToRect(m_furniturePreview);
	}

	public void SetData(NKMOfficeThemePresetTemplet templet)
	{
		Clear();
		NKCUtil.SetGameobjectActive(m_lbName, templet != null);
		NKCUtil.SetGameobjectActive(m_lbDescription, bValue: true);
		NKCUtil.SetGameobjectActive(m_objEnvPoint, bValue: false);
		if (templet != null)
		{
			NKCUtil.SetLabelText(m_lbName, NKCStringTable.GetString(templet.ThemaPresetStringID));
			NKCUtil.SetLabelText(m_lbDescription, NKCStringTable.GetString(templet.ThemaPresetDescID));
			NKCUtil.SetGameobjectActive(m_rtFurnitureRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgBackgroundIcon, bValue: true);
			NKCUtil.SetImageSprite(m_imgBackgroundIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("AB_INVEN_ICON_FNC_THEME", templet.ThemaPresetIMG)));
		}
	}

	private void FitFurnitureToRect(NKCOfficeFuniture furniture)
	{
		Rect worldRect = furniture.GetWorldRect(!bShowTile);
		Vector2 vector = default(Vector2);
		vector.x = furniture.transform.position.x - worldRect.center.x;
		vector.y = furniture.transform.position.y - worldRect.center.y;
		Vector3 vector2 = default(Vector3);
		if (worldRect.width / m_rtFurniturePreviewViewport.GetWidth() > worldRect.height / m_rtFurniturePreviewViewport.GetHeight())
		{
			float width = m_rtFurniturePreviewViewport.GetWidth();
			vector2.x = Mathf.Min(width / worldRect.width, 1f);
			vector2.y = vector2.x;
		}
		else
		{
			float height = m_rtFurniturePreviewViewport.GetHeight();
			vector2.y = Mathf.Min(height / worldRect.height, 1f);
			vector2.x = vector2.y;
		}
		vector2.z = 0f;
		m_rtFurnitureRoot.localScale = vector2;
		m_rtFurnitureRoot.anchoredPosition = vector * vector2;
	}

	public void Clear()
	{
		if (m_furniturePreview != null)
		{
			m_furniturePreview.CleanUp();
			m_furniturePreview = null;
		}
	}
}
