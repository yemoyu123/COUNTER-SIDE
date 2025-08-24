using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuShop : NKCUILobbyMenuButtonBase
{
	public delegate bool DotEnableConditionFunction(NKMUserData userData);

	public delegate void OnButton();

	public GameObject m_objReddot;

	public GameObject m_objReddot_RED;

	public GameObject m_objReddot_YELLOW;

	public Text m_lbReddotCount;

	public NKCUIComStateButton m_csbtnButton;

	private DotEnableConditionFunction dDotEnableConditionFunction;

	private OnButton dOnButton;

	public void Init(DotEnableConditionFunction conditionFunc, OnButton onButton, ContentsType contentsType = ContentsType.None)
	{
		dDotEnableConditionFunction = conditionFunc;
		dOnButton = onButton;
		m_ContentsType = contentsType;
		m_csbtnButton.PointerClick.RemoveAllListeners();
		m_csbtnButton.PointerClick.AddListener(OnBtn);
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		ShopReddotType reddotType;
		int reddotCount = NKCShopManager.CheckTabReddotCount(out reddotType);
		NKCUtil.SetShopReddotImage(reddotType, m_objReddot, m_objReddot_RED, m_objReddot_YELLOW);
		NKCUtil.SetShopReddotLabel(reddotType, m_lbReddotCount, reddotCount);
	}

	private void OnBtn()
	{
		if (m_bLocked)
		{
			NKCContentManager.ShowLockedMessagePopup(m_ContentsType);
		}
		else if (dOnButton != null)
		{
			dOnButton();
		}
	}
}
