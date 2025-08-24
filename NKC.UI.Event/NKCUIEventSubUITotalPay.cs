using NKM.Event;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventSubUITotalPay : NKCUIEventSubUIBase
{
	public Text m_lbTotalPay;

	public Text m_lbTotalPayReturn;

	public NKCUIComStateButton m_csbtnReturnGet;

	public override void Init()
	{
		base.Init();
		if (m_csbtnReturnGet != null)
		{
			m_csbtnReturnGet.PointerClick.RemoveAllListeners();
			m_csbtnReturnGet.PointerClick.AddListener(OnClickReturnGet);
		}
	}

	private void OnClickReturnGet()
	{
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_COMING_SOON_SYSTEM);
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		m_tabTemplet = tabTemplet;
		UpdateUI();
	}

	public override void Refresh()
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		SetDateLimit();
		double totalPayment = NKCScenManager.GetScenManager().GetMyUserData().m_ShopData.GetTotalPayment();
		double num = 0.0;
		if (totalPayment <= 3000.0)
		{
			num = totalPayment * 10.0 * 1.5;
		}
		else
		{
			double num2 = totalPayment - 3000.0;
			num = 45000.0;
			num += num2 * 10.0;
		}
		NKCUtil.SetLabelText(m_lbTotalPay, NKCUtilString.GET_STRING_EVENT_TOTAL_PAY, totalPayment);
		NKCUtil.SetLabelText(m_lbTotalPayReturn, NKCUtilString.GET_STRING_EVENT_TOTAL_PAY_RETURN, num);
	}
}
