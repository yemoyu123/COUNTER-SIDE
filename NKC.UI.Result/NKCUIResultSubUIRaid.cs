using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUIRaid : NKCUIResultSubUIBase
{
	public Text m_lbDamageAmount;

	public Text m_lbRemainBossHP;

	public Image m_imgRemainBossHP;

	private bool m_bFinished;

	private float RAID_DELAY_TIME = 1f;

	public void SetData(NKCUIResult.BattleResultData data, bool bIgnoreAutoClose = false)
	{
		if (data == null || data.m_RaidBossResultData == null)
		{
			base.ProcessRequired = false;
			return;
		}
		base.ProcessRequired = true;
		m_bIgnoreAutoClose = bIgnoreAutoClose;
		int num = 0;
		num = ((data.m_RaidBossResultData.curHP > 0f && data.m_RaidBossResultData.curHP < 1f) ? 1 : ((int)data.m_RaidBossResultData.curHP));
		int num2 = (int)data.m_RaidBossResultData.maxHp;
		float num3 = (float)num / (float)num2 * 100f;
		int num4 = (int)data.m_RaidBossResultData.damage;
		float num5 = 0f;
		if (num2 != 0)
		{
			num5 = (float)num4 / (float)num2 * 100f;
		}
		m_lbDamageAmount.text = string.Format($"{num4} ({$"{num5:0.##}"}%)");
		m_lbRemainBossHP.text = string.Format($"{num} ({$"{num3:0.##}"}%)");
		m_imgRemainBossHP.fillAmount = num3 / 100f;
	}

	public override bool IsProcessFinished()
	{
		return m_bFinished;
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		m_bFinished = false;
		yield return new WaitForSeconds(RAID_DELAY_TIME);
		m_bFinished = true;
	}

	public override void FinishProcess()
	{
		m_bFinished = true;
	}
}
