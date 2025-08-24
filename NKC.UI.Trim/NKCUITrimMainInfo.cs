using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Trim;

public class NKCUITrimMainInfo : MonoBehaviour
{
	public Transform m_battleCondParent;

	public NKCUIComStateButton m_csbtnEnter;

	public NKCUITrimReward m_trimReward;

	public Text m_lbTrimLevel;

	public Text m_lbTrimName;

	public Text m_lbTrimDesc;

	public Text m_lbEnterLimit;

	public Text m_lbRecommendedPower;

	public Image m_imgMapLarge;

	public Image m_imgMapSmall;

	public Image m_imgColor;

	public GameObject m_objEnterLimitRoot;

	private int m_trimId;

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnEnter, OnClickEnter);
		NKCUtil.SetHotkey(m_csbtnEnter, HotkeyEventType.Confirm);
		m_trimReward?.Init();
		NKCUITrimUtility.InitBattleCondition(m_battleCondParent, showToolTip: true);
	}

	public void SetData(int trimId)
	{
		m_trimId = trimId;
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(trimId);
		string text = null;
		string text2 = null;
		Sprite sp = null;
		int a = 0;
		int trimGroup = 0;
		if (nKMTrimTemplet != null)
		{
			text = NKCStringTable.GetString(nKMTrimTemplet.TirmGroupName);
			text2 = NKCStringTable.GetString(nKMTrimTemplet.TirmGroupDesc);
			sp = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_TRIM_MAP_IMG", nKMTrimTemplet.TrimGroupBGPrefab);
			a = nKMTrimTemplet.MaxTrimLevel;
			trimGroup = nKMTrimTemplet.TrimPointGroup;
			ColorUtility.TryParseHtmlString(nKMTrimTemplet.TrimBGColor, out var color);
			NKCUtil.SetImageColor(m_imgColor, color);
		}
		else
		{
			text = " - ";
			text2 = " - ";
		}
		int clearedTrimLevel = NKCUITrimUtility.GetClearedTrimLevel(NKCScenManager.CurrentUserData(), trimId);
		int trimLevel = Mathf.Min(a, clearedTrimLevel + 1);
		NKCUITrimUtility.SetBattleCondition(m_battleCondParent, nKMTrimTemplet, trimLevel, showToolTip: true);
		NKCUtil.SetLabelText(m_lbTrimName, text);
		NKCUtil.SetLabelText(m_lbTrimDesc, text2);
		NKCUtil.SetLabelText(m_lbTrimLevel, trimLevel.ToString());
		NKCUtil.SetImageSprite(m_imgMapLarge, sp, bDisableIfSpriteNull: true);
		NKCUtil.SetImageSprite(m_imgMapSmall, sp, bDisableIfSpriteNull: true);
		NKMTrimIntervalTemplet trimInterval = NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime);
		NKCUtil.SetGameobjectActive(m_objEnterLimitRoot, NKCUITrimUtility.IsEnterCountLimited(trimInterval));
		if (NKCUITrimUtility.IsEnterCountLimited(trimInterval))
		{
			string enterLimitMsg = NKCUITrimUtility.GetEnterLimitMsg(trimInterval);
			NKCUtil.SetLabelText(m_lbEnterLimit, enterLimitMsg);
		}
		int recommendedPower = NKCUITrimUtility.GetRecommendedPower(trimGroup, trimLevel);
		NKCUtil.SetLabelText(m_lbRecommendedPower, recommendedPower.ToString("N0"));
		m_trimReward?.SetData(trimId, trimLevel);
	}

	public void OnClickEnter()
	{
		NKCUIPopupTrimDungeon.Instance.Open(m_trimId);
	}
}
