using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComItemRequire : MonoBehaviour
{
	public enum InsufficientMarking
	{
		REQUIRE_COUNT,
		MY_COUNT
	}

	public NKCUISlot m_Slot;

	public Image m_lbItemSmallIcon;

	public Text m_lbRequireCount;

	public Text m_lbHasCount;

	[Header("부족할때 어떤 값이 빨간색으로 표시되나요?")]
	public InsufficientMarking m_eInsufficientMarking = InsufficientMarking.MY_COUNT;

	private Color m_colTextOrigColor;

	public int ItemID { get; private set; }

	public void Init()
	{
		if (m_Slot != null)
		{
			m_Slot.Init();
		}
		Text insufficientLabel = GetInsufficientLabel();
		if (insufficientLabel != null)
		{
			m_colTextOrigColor = insufficientLabel.color;
		}
	}

	public void SetItem(int itemID, int RequireCount, bool bQuantityCheck = false)
	{
		ItemID = itemID;
		if (m_Slot != null)
		{
			if (-1 == itemID)
			{
				m_Slot.SetEmpty();
			}
			else
			{
				m_Slot.SetData(NKCUISlot.SlotData.MakeMiscItemData(itemID, 0L), bShowName: false, bShowNumber: false, bEnableLayoutElement: true, null);
				m_Slot.SetOpenItemBoxOnClick();
			}
		}
		if (m_lbItemSmallIcon != null)
		{
			m_lbItemSmallIcon.sprite = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemID);
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		long num = 0L;
		if (nKMUserData != null)
		{
			num = nKMUserData.m_InventoryData.GetCountMiscItem(itemID);
		}
		NKCUtil.SetLabelText(m_lbHasCount, num.ToString());
		if (bQuantityCheck)
		{
			if (RequireCount <= num)
			{
				NKCUtil.SetLabelText(m_lbRequireCount, $"<color=#ffffff>{RequireCount}/{num}</color>");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbRequireCount, $"<color=#ffffff>{RequireCount}/</color><color=#ff0000ff>{num}</color>");
			}
		}
		else if (RequireCount >= 0)
		{
			NKCUtil.SetLabelText(m_lbRequireCount, RequireCount.ToString());
			Text insufficientLabel = GetInsufficientLabel();
			if (insufficientLabel != null)
			{
				if (num < RequireCount)
				{
					insufficientLabel.color = Color.red;
				}
				else
				{
					insufficientLabel.color = m_colTextOrigColor;
				}
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRequireCount, "0");
			Text insufficientLabel2 = GetInsufficientLabel();
			if (insufficientLabel2 != null)
			{
				insufficientLabel2.color = m_colTextOrigColor;
			}
		}
	}

	private Text GetInsufficientLabel()
	{
		if (m_eInsufficientMarking != InsufficientMarking.REQUIRE_COUNT)
		{
			_ = 1;
			return m_lbHasCount;
		}
		return m_lbRequireCount;
	}
}
