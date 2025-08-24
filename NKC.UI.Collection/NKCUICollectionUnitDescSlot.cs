using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionUnitDescSlot : MonoBehaviour
{
	public TMP_Text m_Title;

	public TMP_Text m_Desc;

	public GameObject m_TextArea;

	public GameObject m_None;

	public Image[] m_Bars;

	public NKCUICollectionProfileToolTip m_profileToolTip;

	private const float m_BarMin = 166f;

	private const float m_BarMax = 725f;

	private List<Image> m_BarList = new List<Image>();

	public void Init()
	{
		m_profileToolTip?.Init();
	}

	public void SetData(int index, int unitId, string type, string value)
	{
		if (NKCStringTable.CheckExistString(type))
		{
			NKCUtil.SetLabelText(m_Title, NKCStringTable.GetString(type));
		}
		else
		{
			NKCUtil.SetLabelText(m_Title, NKCStringTable.GetString("SI_COLLECTION_PROFILE_TYPE_PROFILE_CENSORED"));
		}
		if (NKCStringTable.CheckExistString(value))
		{
			NKCUtil.SetGameobjectActive(m_None, bValue: false);
			NKCUtil.SetGameobjectActive(m_TextArea, bValue: true);
			NKCUtil.SetLabelText(m_Desc, NKCStringTable.GetString(value));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_None, bValue: true);
			NKCUtil.SetGameobjectActive(m_TextArea, bValue: false);
			ResetBarSequence();
			ThisCalculationHasNoMeaningSoDontSpendTimeToReadThisCode(unitId, index);
		}
		m_profileToolTip?.SetDescData(type);
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
	}

	private void ResetBarSequence()
	{
		m_BarList.Clear();
		int num = m_Bars.Length;
		for (int i = 0; i < num; i++)
		{
			m_Bars[i].transform.SetSiblingIndex(i);
			m_BarList.Add(m_Bars[i]);
		}
	}

	private void ThisCalculationHasNoMeaningSoDontSpendTimeToReadThisCode(int unitId, int index)
	{
		string s = NKCStringTable.GetString(NKCCollectionManager.GetEmployeeTemplet(unitId).NameValue);
		byte[] bytes = Encoding.Default.GetBytes(s);
		int num = bytes.Length;
		int num2 = m_Bars.Length;
		for (int i = 0; i < num2; i++)
		{
			int num3 = bytes[i % num] * (index + 1) % num2;
			if (num3 == i)
			{
				num3++;
			}
			if (num3 >= num2)
			{
				num3 = 0;
			}
			m_BarList[i].transform.SetSiblingIndex(num3);
			m_BarList[num3].transform.SetSiblingIndex(i);
		}
		m_BarList.Sort(delegate(Image e1, Image e2)
		{
			if (e1.transform.GetSiblingIndex() > e2.transform.GetSiblingIndex())
			{
				return 1;
			}
			return (e1.transform.GetSiblingIndex() < e2.transform.GetSiblingIndex()) ? (-1) : 0;
		});
		int[] array = new int[num];
		int num4 = 0;
		for (int num5 = 0; num5 < num; num5++)
		{
			num4 += bytes[num5];
		}
		int num6 = Mathf.Max(1, (index + 1) % num);
		int num7 = num4 / num;
		int num8 = 559;
		for (int num9 = 0; num9 < num; num9++)
		{
			array[num9] = bytes[num9] * (num4 / num6) * num7 % num8;
			if (array[num9] == 0)
			{
				array[num9] = bytes[num9];
			}
		}
		int num10 = 0;
		int num11 = 0;
		int num12 = 0;
		int index2 = -1;
		for (int num13 = 0; num13 < num2; num13++)
		{
			int num14 = array[num13 % num] * (num13 / num + 1);
			if (num14 % num == 0 && num10 <= 0 && num11 < 2)
			{
				m_BarList[num13].rectTransform.SetWidth(0f);
			}
			else
			{
				int num15 = num14 % num8;
				m_BarList[num13].rectTransform.SetWidth(Mathf.Min((float)num15 + 166f, 725f));
			}
			if (num14 % 5 == 0)
			{
				NKCUtil.SetGameobjectActive(m_BarList[num13], bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_BarList[num13], bValue: true);
			if (m_BarList[num13].rectTransform.GetWidth() > 0f)
			{
				num12++;
				num10 = 0;
			}
			else
			{
				num10++;
				num11++;
				index2 = num13;
			}
		}
		for (int num16 = 0; num16 < num2; num16++)
		{
			if (m_BarList[num16].gameObject.activeSelf)
			{
				if (m_BarList[num16].rectTransform.GetWidth() <= 0f)
				{
					NKCUtil.SetGameobjectActive(m_BarList[num16], bValue: false);
				}
				if (num12 > 4)
				{
					break;
				}
			}
			if (!m_BarList[num16].gameObject.activeSelf && m_BarList[num16].rectTransform.GetWidth() > 0f)
			{
				NKCUtil.SetGameobjectActive(m_BarList[num16], bValue: true);
			}
		}
		if (num2 > 0 && num10 > 0)
		{
			NKCUtil.SetGameobjectActive(m_BarList[index2], bValue: false);
		}
	}

	private void OnDestroy()
	{
		m_BarList?.Clear();
		m_BarList = null;
	}
}
