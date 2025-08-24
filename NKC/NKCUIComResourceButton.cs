using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIComResourceButton : NKCUIComStateButton
{
	public GameObject m_Obj;

	public Image m_ResourceIcon;

	public Text m_ResourceCount;

	public Image m_Background;

	private bool bInit;

	private Color TextOriginalColor = Color.white;

	private int m_ItemID;

	private void OnEnable()
	{
		Init();
	}

	public void Init()
	{
		if (!bInit)
		{
			bInit = true;
			if (m_ResourceCount != null)
			{
				TextOriginalColor = m_ResourceCount.color;
			}
		}
	}

	public int GetItemID()
	{
		return m_ItemID;
	}

	public void OnShow(bool bShow)
	{
		NKCUtil.SetGameobjectActive(m_Obj, bShow);
	}

	public void SetData(int itemID, int itemCount)
	{
		m_ItemID = itemID;
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID != null)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
			NKCUtil.SetLabelText(m_ResourceCount, itemCount.ToString());
			NKCUtil.SetImageSprite(m_ResourceIcon, orLoadMiscItemSmallIcon);
			bool bEnough = true;
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null && nKMUserData.m_InventoryData.GetCountMiscItem(itemID) < itemCount)
			{
				bEnough = false;
			}
			UpdateTextColor(bEnough);
		}
	}

	public void SetDataWithUseCount(int itemID, int itemUseCount, string format = "{0}/{1}")
	{
		m_ItemID = itemID;
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID == null)
		{
			return;
		}
		Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
		NKCUtil.SetImageSprite(m_ResourceIcon, orLoadMiscItemSmallIcon);
		bool flag = true;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		long num = 0L;
		if (nKMUserData != null)
		{
			num = nKMUserData.m_InventoryData.GetCountMiscItem(itemID);
			if (num < itemUseCount)
			{
				flag = false;
			}
		}
		UpdateTextColor(bEnough: true);
		if (flag)
		{
			NKCUtil.SetLabelText(m_ResourceCount, string.Format(format, num, itemUseCount));
			return;
		}
		string arg = $"<color=#CD2121>{num}</color>";
		NKCUtil.SetLabelText(m_ResourceCount, string.Format(format, arg, itemUseCount));
	}

	private void UpdateTextColor(bool bEnough = false)
	{
		Color textColor = TextOriginalColor;
		if (!bEnough)
		{
			textColor = NKCUtil.GetColor("#CD2121");
		}
		SetTextColor(textColor);
	}

	public void SetTextColor(Color col)
	{
		NKCUtil.SetLabelTextColor(m_ResourceCount, col);
	}

	public void SetIconColor(Color col)
	{
		NKCUtil.SetImageColor(m_ResourceIcon, col);
	}

	public void SetText(string newText)
	{
		NKCUtil.SetLabelText(m_ResourceCount, newText);
	}

	public void SetBackgroundSprite(Sprite newSprite)
	{
		NKCUtil.SetImageSprite(m_Background, newSprite);
	}
}
