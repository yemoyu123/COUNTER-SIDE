using NKC.UI;
using NKM.Templet;
using UnityEngine;

namespace NKC.Office;

public class NKCOfficeFacilityTerrabrain : NKCOfficeFacility
{
	public NKCOfficeFacilityFuniture m_fnTerrabrain;

	public GameObject m_objFloorEffect;

	public GameObject m_objFloorTouchEffect;

	public NKCOfficeCharacterNPC m_NPCTerraBrainGap;

	public override void Init()
	{
		base.Init();
		if (m_fnTerrabrain != null)
		{
			m_fnTerrabrain.SetLock(value: false);
			m_fnTerrabrain.dOnClickFuniture = OnClickTerrabrainDesk;
		}
		if (m_NPCTerraBrainGap != null)
		{
			m_NPCTerraBrainGap.SetOnClick(OnTouchTerrabrainGap);
		}
		NKCUtil.SetGameobjectActive(m_objFloorEffect, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFloorTouchEffect, bValue: false);
	}

	private void OnClickTerrabrainDesk(int id, long uid)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.TERRA_BRAIN))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.TERRA_BRAIN);
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCStringTable.GetString("SI_CONTENTS_READY_TERRA_BRAIN"));
		}
	}

	protected override void Update()
	{
		base.Update();
		if (m_NPCTerraBrainGap != null && m_NPCTerraBrainGap.GetBTValue("IdleBegun", defaultValue: false))
		{
			NKCUtil.SetGameobjectActive(m_objFloorEffect, bValue: true);
		}
	}

	private bool OnTouchTerrabrainGap()
	{
		if (m_NPCTerraBrainGap != null && m_NPCTerraBrainGap.GetBTValue("IdleBegun", defaultValue: false))
		{
			NKCUtil.SetGameobjectActive(m_objFloorTouchEffect, bValue: false);
			NKCUtil.SetGameobjectActive(m_objFloorTouchEffect, bValue: true);
		}
		return false;
	}
}
