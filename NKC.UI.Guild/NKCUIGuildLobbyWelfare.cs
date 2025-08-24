using System.Collections.Generic;
using System.Linq;
using NKM;
using NKM.Guild;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildLobbyWelfare : MonoBehaviour
{
	public enum UITabType
	{
		User,
		Guild
	}

	public NKCUIComToggle m_tglUser;

	public NKCUIComToggle m_tglGuild;

	public Image m_imgPoint;

	public Text m_lbPointCount;

	public NKCUIComStateButton m_btnAddPoint;

	public NKCUIGuildLobbyWelfareSlot m_pfbSlot;

	public LoopScrollRect m_loop;

	public Transform m_trSlotParent;

	public GameObject m_objNone;

	private Stack<NKCUIGuildLobbyWelfareSlot> m_stkSlot = new Stack<NKCUIGuildLobbyWelfareSlot>();

	private List<GuildWelfareTemplet> m_lstUserTemplet = new List<GuildWelfareTemplet>();

	private List<GuildWelfareTemplet> m_lstGuildTemplet = new List<GuildWelfareTemplet>();

	private UITabType m_currentType;

	public void InitUI()
	{
		m_tglUser.OnValueChanged.RemoveAllListeners();
		m_tglUser.OnValueChanged.AddListener(OnChangedUserTgl);
		m_tglGuild.OnValueChanged.RemoveAllListeners();
		m_tglGuild.OnValueChanged.AddListener(OnChangedGuildTgl);
		m_btnAddPoint.PointerClick.RemoveAllListeners();
		m_btnAddPoint.PointerClick.AddListener(OnClickAddPoint);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loop);
	}

	private RectTransform GetObject(int idx)
	{
		NKCUIGuildLobbyWelfareSlot nKCUIGuildLobbyWelfareSlot = null;
		nKCUIGuildLobbyWelfareSlot = ((m_stkSlot.Count <= 0) ? Object.Instantiate(m_pfbSlot) : m_stkSlot.Pop());
		nKCUIGuildLobbyWelfareSlot.InitUI();
		return nKCUIGuildLobbyWelfareSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIGuildLobbyWelfareSlot component = tr.GetComponent<NKCUIGuildLobbyWelfareSlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		tr.SetParent(base.transform);
		m_stkSlot.Push(component);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIGuildLobbyWelfareSlot component = tr.GetComponent<NKCUIGuildLobbyWelfareSlot>();
		if (m_currentType == UITabType.User)
		{
			if (idx < m_lstUserTemplet.Count)
			{
				component.SetData(m_lstUserTemplet[idx]);
			}
			else
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
		}
		else if (idx < m_lstGuildTemplet.Count)
		{
			component.SetData(m_lstGuildTemplet[idx]);
		}
		else
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
	}

	public void SetData()
	{
		m_lstUserTemplet = NKMTempletContainer<GuildWelfareTemplet>.Values.ToList().FindAll((GuildWelfareTemplet x) => x.WelfareCategory == WELFARE_BUFF_TYPE.PERSONAL);
		m_lstUserTemplet.Sort(Comparer);
		m_lstGuildTemplet = NKMTempletContainer<GuildWelfareTemplet>.Values.ToList().FindAll((GuildWelfareTemplet x) => x.WelfareCategory == WELFARE_BUFF_TYPE.GUILD);
		m_lstGuildTemplet.Sort(Comparer);
		if (m_currentType == UITabType.User)
		{
			NKCUtil.SetGameobjectActive(m_btnAddPoint, bValue: true);
			m_btnAddPoint.UnLock();
			m_tglUser.Select(bSelect: true, bForce: true, bImmediate: true);
			OnChangedUserTgl(bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_btnAddPoint, bValue: false);
			m_btnAddPoint.Lock();
			m_tglGuild.Select(bSelect: true, bForce: true, bImmediate: true);
			OnChangedGuildTgl(bValue: true);
		}
	}

	private int Comparer(GuildWelfareTemplet left, GuildWelfareTemplet right)
	{
		if (left.Order == right.Order)
		{
			return left.ID.CompareTo(right.ID);
		}
		return left.Order.CompareTo(right.Order);
	}

	private void SetPoint()
	{
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		if (m_currentType == UITabType.User)
		{
			NKCUtil.SetImageSprite(m_imgPoint, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(23));
			NKCUtil.SetLabelText(m_lbPointCount, inventoryData.GetCountMiscItem(23).ToString("#,##0"));
			NKCUtil.SetGameobjectActive(m_btnAddPoint, bValue: true);
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgPoint, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(24));
			NKCUtil.SetLabelText(m_lbPointCount, NKCGuildManager.MyGuildData.unionPoint.ToString("#,##0"));
			NKCUtil.SetGameobjectActive(m_btnAddPoint, bValue: false);
		}
	}

	private void OnChangedUserTgl(bool bValue)
	{
		if (bValue)
		{
			m_currentType = UITabType.User;
			m_loop.TotalCount = m_lstUserTemplet.Count;
			m_loop.RefreshCells(bForce: true);
			NKCUtil.SetGameobjectActive(m_objNone, m_loop.TotalCount == 0);
			SetPoint();
		}
	}

	private void OnChangedGuildTgl(bool bValue)
	{
		if (bValue)
		{
			m_currentType = UITabType.Guild;
			m_loop.TotalCount = m_lstGuildTemplet.Count;
			m_loop.RefreshCells(bForce: true);
			NKCUtil.SetGameobjectActive(m_objNone, m_loop.TotalCount == 0);
			SetPoint();
		}
	}

	private void OnClickAddPoint()
	{
		if (m_currentType == UITabType.User)
		{
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(23, isPaid: true) + NKMCommonConst.Guild.WelfarePointBuyAmount > NKMCommonConst.Guild.WelfarePointBuyLimit)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GUILD_WELFARE_POINT_LIMIT);
			}
			else
			{
				NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_WELFARE_BUY_POINT_TITLE, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_WELFARE_BUY_POINT_DESC, NKMCommonConst.Guild.WelfarePointBuyAmount), 101, (int)NKMCommonConst.Guild.WelfarePointPrice, OnConfirmAddPoint);
			}
		}
	}

	private void OnConfirmAddPoint()
	{
		if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(101) >= NKMCommonConst.Guild.WelfarePointPrice)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_BUY_WELFARE_POINT_REQ(NKMCommonConst.Guild.WelfarePointBuyAmount);
		}
		else
		{
			NKCPopupItemLack.Instance.OpenItemMiscLackPopup(101, (int)NKMCommonConst.Guild.WelfarePointPrice);
		}
	}
}
