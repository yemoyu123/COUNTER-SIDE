using NKC.UI.Office;
using NKM.Templet;
using UnityEngine;

namespace NKC.Office;

public class NKCOfficeFacilityLab : NKCOfficeFacility
{
	[Header("연구소 가구들")]
	public NKCOfficeFacilityFuniture m_fnUnitList;

	public NKCOfficeFacilityFuniture m_fnRearmament;

	public NKCOfficeFacilityFuniture m_fnExtract;

	public override void Init()
	{
		base.Init();
		if (m_fnUnitList != null)
		{
			m_fnUnitList.dOnClickFuniture = OnClickUnitList;
		}
		if (m_fnRearmament != null)
		{
			NKCUtil.SetGameobjectActive(m_fnRearmament, NKCRearmamentUtil.IsCanUseContent());
			m_fnRearmament.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.REARM));
			m_fnRearmament.dOnClickFuniture = OnClickRearmament;
		}
		if (m_fnExtract != null)
		{
			bool bAdmin;
			NKCContentManager.eContentStatus eContentStatus = NKCContentManager.CheckContentStatus(ContentsType.EXTRACT, out bAdmin);
			NKCUtil.SetGameobjectActive(m_fnExtract, NKCRearmamentUtil.CanUseExtract() && eContentStatus != NKCContentManager.eContentStatus.Hide);
			m_fnExtract.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.EXTRACT));
			m_fnExtract.dOnClickFuniture = OnClickExtract;
		}
	}

	private void OnClickUnitList(int id, long uid)
	{
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice.GetInstance().OfficeFacilityInterfaces?.OnLabUnitList();
		}
	}

	public void OnClickRearmament(int id, long uid)
	{
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice.GetInstance().OfficeFacilityInterfaces?.OnLabUnitRearm();
		}
	}

	public void OnClickExtract(int id, long uid)
	{
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice.GetInstance().OfficeFacilityInterfaces?.OnLabUnitExtract();
		}
	}
}
