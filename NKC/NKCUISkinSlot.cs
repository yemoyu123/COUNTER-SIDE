using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUISkinSlot : MonoBehaviour
{
	public delegate void OnClick(int skinID);

	public Image m_imgSkin;

	public Image m_imgBG;

	public Text m_lbName;

	public GameObject m_objEquipped;

	public GameObject m_objNotOwned;

	public GameObject m_objSelected;

	public GameObject m_objSpecialSkinBG;

	public NKCUIComButton m_cbtnSlot;

	public Sprite m_spBG_Variation;

	public Sprite m_spBG_Normal;

	public Sprite m_spBG_Rare;

	public Sprite m_spBG_Premium;

	public GameObject m_specialCutscen;

	private OnClick dOnClick;

	public GameObject m_NKM_UI_COLLECTION_UNIT_INFO_UNIT_SKIN_SLOT_ON_BLACK;

	public int SkinID { get; private set; }

	public void Init(OnClick onClick)
	{
		dOnClick = onClick;
		m_cbtnSlot.PointerClick.RemoveAllListeners();
		m_cbtnSlot.PointerClick.AddListener(OnBtnSlot);
	}

	public void SetData(NKMSkinTemplet skinTemplet, bool bOwned = false, bool bEquipped = false, bool bBlack = false)
	{
		if (skinTemplet == null)
		{
			m_imgSkin.sprite = null;
			m_lbName.text = "";
			SetEquipped(bValue: false);
			SetOwned(bValue: false);
			SkinID = 0;
		}
		else
		{
			SetEquipped(bEquipped);
			SetOwned(bOwned);
			SetSkinBG(skinTemplet.m_SkinGrade);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_UNIT_SKIN_SLOT_ON_BLACK, bBlack);
			SkinID = skinTemplet.m_SkinID;
			m_imgSkin.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, skinTemplet);
			m_lbName.text = skinTemplet.GetTitle();
		}
	}

	public void SetData(NKMUnitTempletBase unitTemplet, bool bEquipped = false)
	{
		SetOwned(bValue: true);
		SetEquipped(bEquipped);
		SkinID = 0;
		m_imgSkin.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTemplet);
		m_lbName.text = unitTemplet.GetUnitTitle();
		SetSkinBG(NKMSkinTemplet.SKIN_GRADE.SG_VARIATION);
	}

	public void SetOwned(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objNotOwned, !bValue);
	}

	public void SetEquipped(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objEquipped, bValue);
	}

	public void SetSelected(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objSelected, bValue);
	}

	public void SetSpecialCutScene(NKMSkinTemplet skinTemplet)
	{
		bool bValue = !string.IsNullOrEmpty(skinTemplet.m_CutsceneLifetime_start) || !string.IsNullOrEmpty(skinTemplet.m_CutsceneLifetime_end);
		NKCUtil.SetGameobjectActive(m_specialCutscen, bValue);
	}

	private void SetSkinBG(NKMSkinTemplet.SKIN_GRADE grade)
	{
		switch (grade)
		{
		default:
			NKCUtil.SetImageSprite(m_imgBG, m_spBG_Variation);
			break;
		case NKMSkinTemplet.SKIN_GRADE.SG_NORMAL:
			NKCUtil.SetImageSprite(m_imgBG, m_spBG_Normal);
			break;
		case NKMSkinTemplet.SKIN_GRADE.SG_RARE:
			NKCUtil.SetImageSprite(m_imgBG, m_spBG_Rare);
			break;
		case NKMSkinTemplet.SKIN_GRADE.SG_PREMIUM:
		case NKMSkinTemplet.SKIN_GRADE.SG_SPECIAL:
			NKCUtil.SetImageSprite(m_imgBG, m_spBG_Premium);
			break;
		}
		NKCUtil.SetGameobjectActive(m_objSpecialSkinBG, grade == NKMSkinTemplet.SKIN_GRADE.SG_SPECIAL);
	}

	private void OnBtnSlot()
	{
		if (dOnClick != null)
		{
			dOnClick(SkinID);
		}
	}
}
