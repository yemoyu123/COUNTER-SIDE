using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.Game.Unit;

public class NKCGameUnitBuffIcon : MonoBehaviour
{
	public Image m_UNIT_BUFF_Image;

	public Image m_UNIT_BUFF_COOL_Image;

	public GameObject m_UNIT_BUFF_OUTLINE;

	private int m_GageBuffID;

	public Text m_UNIT_BUFF_TEXT_OVERLAP_Text;

	private byte m_OverlapCount;

	public void InitObject(GameObject cUNIT_GAGE, int index)
	{
	}

	public void Init()
	{
		m_GageBuffID = 0;
	}

	public void Unload()
	{
		m_UNIT_BUFF_Image.sprite = null;
		m_UNIT_BUFF_COOL_Image.sprite = null;
		m_GageBuffID = 0;
	}

	public void GageSetBuffIconActive(bool bActive, NKMBuffData cNKMBuffData = null, float fLifeTimeRate = 1f)
	{
		if (cNKMBuffData != null && cNKMBuffData.m_NKMBuffTemplet != null && !cNKMBuffData.m_NKMBuffTemplet.m_bShowBuffIcon)
		{
			bActive = false;
		}
		bool bValue = false;
		if (cNKMBuffData != null && cNKMBuffData.m_NKMBuffTemplet != null && cNKMBuffData.m_NKMBuffTemplet.m_bInfinity && bActive)
		{
			bValue = true;
		}
		if (cNKMBuffData != null && cNKMBuffData.m_NKMBuffTemplet != null && cNKMBuffData.m_NKMBuffTemplet.m_bNotDispel && bActive)
		{
			bValue = true;
		}
		NKCUtil.SetGameobjectActive(m_UNIT_BUFF_Image, bActive);
		NKCUtil.SetGameobjectActive(m_UNIT_BUFF_OUTLINE, bValue);
		if (!bActive)
		{
			NKCUtil.SetGameobjectActive(m_UNIT_BUFF_TEXT_OVERLAP_Text, bValue: false);
			return;
		}
		m_UNIT_BUFF_COOL_Image.fillAmount = 1f - fLifeTimeRate;
		if (cNKMBuffData.m_NKMBuffTemplet != null && m_GageBuffID != cNKMBuffData.m_NKMBuffTemplet.m_BuffID && cNKMBuffData.m_NKMBuffTemplet.m_IconName.Length > 1)
		{
			m_UNIT_BUFF_Image.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UNIT_GAME_NKM_UNIT_SPRITE", cNKMBuffData.m_NKMBuffTemplet.m_IconName);
		}
		m_GageBuffID = cNKMBuffData.m_NKMBuffTemplet.m_BuffID;
		if (cNKMBuffData.m_NKMBuffTemplet.m_RangeOverlap)
		{
			if (cNKMBuffData.m_BuffSyncData.m_BuffStatLevel >= 1)
			{
				NKCUtil.SetGameobjectActive(m_UNIT_BUFF_TEXT_OVERLAP_Text, bValue: true);
				if (m_OverlapCount != cNKMBuffData.m_BuffSyncData.m_BuffStatLevel)
				{
					m_OverlapCount = cNKMBuffData.m_BuffSyncData.m_BuffStatLevel;
					m_UNIT_BUFF_TEXT_OVERLAP_Text.text = $"{m_OverlapCount - 1}";
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_UNIT_BUFF_TEXT_OVERLAP_Text, bValue: false);
			}
		}
		else if (cNKMBuffData.m_BuffSyncData.m_OverlapCount > 1)
		{
			NKCUtil.SetGameobjectActive(m_UNIT_BUFF_TEXT_OVERLAP_Text, bValue: true);
			if (m_OverlapCount != cNKMBuffData.m_BuffSyncData.m_OverlapCount)
			{
				m_OverlapCount = cNKMBuffData.m_BuffSyncData.m_OverlapCount;
				m_UNIT_BUFF_TEXT_OVERLAP_Text.text = $"{m_OverlapCount}";
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_UNIT_BUFF_TEXT_OVERLAP_Text, bValue: false);
		}
	}
}
