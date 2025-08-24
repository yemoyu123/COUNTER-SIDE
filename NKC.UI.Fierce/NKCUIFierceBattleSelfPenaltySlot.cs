using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCUIFierceBattleSelfPenaltySlot : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnClick;

	public Image m_imgIcon;

	public Image m_imgIconBG;

	public GameObject m_objSelect;

	public NKCComText m_lbDesc;

	public NKCComText m_lbLevel;

	public GameObject m_objDisable;

	private OnClickPenalty dOnClick;

	private NKMFiercePenaltyTemplet m_templet;

	public NKMFiercePenaltyTemplet TempletData => m_templet;

	public void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnClick, OnClickSlot);
	}

	public void SetData(NKMFiercePenaltyTemplet PenaltyTemplt, OnClickPenalty OnClick)
	{
		if (PenaltyTemplt != null)
		{
			m_templet = PenaltyTemplt;
			string msg = NKCStringTable.GetString(PenaltyTemplt.PenaltyLevelDesc);
			NKCUtil.SetLabelText(m_lbDesc, msg);
			NKCUtil.SetLabelText(m_lbLevel, PenaltyTemplt.PenaltyLevel.ToString());
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_FIERCE_BATTLE_TEXTURE", PenaltyTemplt.PenaltyIcon));
			NKCUtil.SetImageSprite(m_imgIconBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_FIERCE_BATTLE_TEXTURE", PenaltyTemplt.PenaltyIconBG));
			Select(bSelect: false);
			Disable(bDisable: false);
			dOnClick = OnClick;
		}
	}

	public void Select(bool bSelect)
	{
		NKCUtil.SetGameobjectActive(m_objSelect, bSelect);
	}

	public void Disable(bool bDisable)
	{
		NKCUtil.SetGameobjectActive(m_objDisable, bDisable);
	}

	private void OnClickSlot()
	{
		dOnClick?.Invoke(m_templet);
	}
}
