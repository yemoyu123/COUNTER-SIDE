using NKC.UI;
using NKC.UI.Office;
using NKM.Templet;
using UnityEngine;

namespace NKC.Office;

public class NKCOfficeFacilityHangar : NKCOfficeFacility
{
	[Header("격납고 정보")]
	public NKCOfficeFacilityFuniture m_fnBuild;

	public NKCOfficeFacilityFuniture m_fnShipList;

	public override void Init()
	{
		base.Init();
		if (m_fnBuild != null)
		{
			m_fnBuild.dOnClickFuniture = OnClickBuild;
			m_fnBuild.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPBUILD));
		}
		if (m_fnShipList != null)
		{
			m_fnShipList.dOnClickFuniture = OnClickShipList;
		}
	}

	public override void UpdateAlarm()
	{
		if (m_fnBuild != null)
		{
			m_fnBuild.SetAlarm(NKCAlarmManager.ALARM_TYPE.HANGAR);
		}
	}

	private void OnClickBuild(int id, long uid)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPBUILD))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.HANGER_SHIPBUILD);
			return;
		}
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice.GetInstance().OfficeFacilityInterfaces?.OnHangarBuild();
		}
		UpdateAlarm();
	}

	private void OnClickShipList(int id, long uid)
	{
		if (NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice.GetInstance().OfficeFacilityInterfaces?.OnHangerShipList();
		}
	}
}
