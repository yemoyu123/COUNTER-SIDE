using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCUIMainGageBuffCell : MonoBehaviour
{
	public Image m_imgBuff;

	public Image m_imgCooldown;

	public Text m_lbOverlap;

	private int m_GageBuffID;

	public Image Get_UNIT_BUFF_Image()
	{
		return m_imgBuff;
	}

	public Image Get_UNIT_BUFF_COOL_Image()
	{
		return m_imgCooldown;
	}

	public void InitUI()
	{
		NKCUtil.SetImageFillAmount(m_imgCooldown, 0f);
		base.gameObject.SetActive(value: false);
	}

	private void SetOverlapCount(int overlapCount)
	{
		if (!(m_lbOverlap == null))
		{
			if (overlapCount <= 1)
			{
				NKCUtil.SetGameobjectActive(m_lbOverlap, bValue: false);
				m_lbOverlap.text = string.Empty;
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lbOverlap, bValue: true);
				m_lbOverlap.text = overlapCount.ToString();
			}
		}
	}

	public void SetData(NKMBuffTemplet cNKMBuffTemplet, float fLifeTimeRate, int overlapCount)
	{
		m_imgCooldown.fillAmount = 1f - fLifeTimeRate;
		if (cNKMBuffTemplet != null && m_GageBuffID != cNKMBuffTemplet.m_BuffID && cNKMBuffTemplet.m_IconName.Length > 1)
		{
			m_imgBuff.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UNIT_GAME_NKM_UNIT_SPRITE", cNKMBuffTemplet.m_IconName);
		}
		m_GageBuffID = cNKMBuffTemplet.m_BuffID;
		SetOverlapCount(overlapCount);
	}
}
