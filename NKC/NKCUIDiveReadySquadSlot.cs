using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDiveReadySquadSlot : MonoBehaviour
{
	public delegate void OnSelectedDiveReadySquadSlot();

	public NKCUIComStateButton m_NKM_UI_DIVE_INFO_SQUAD_SLOT;

	public GameObject m_NKM_UI_DIVE_INFO_SQUAD_SLOT_SELECTED;

	public GameObject m_NKM_UI_DIVE_INFO_SQUAD_SLOT_UNSELECTED;

	public Image m_NKM_UI_DIVE_INFO_SQUAD_SLOT_SHIP_IMAGE;

	public Text m_NKM_UI_DIVE_INFO_SQUAD_SLOT_DECKNUMBER_COUNT;

	private OnSelectedDiveReadySquadSlot m_OnSelectedDiveReadySquadSlot;

	private void Awake()
	{
		m_NKM_UI_DIVE_INFO_SQUAD_SLOT.PointerClick.RemoveAllListeners();
		m_NKM_UI_DIVE_INFO_SQUAD_SLOT.PointerClick.AddListener(OnClicked);
	}

	public void SetSelectedEvent(OnSelectedDiveReadySquadSlot _OnSelectedDiveReadySquadSlot)
	{
		m_OnSelectedDiveReadySquadSlot = _OnSelectedDiveReadySquadSlot;
	}

	private void OnClicked()
	{
		if (m_OnSelectedDiveReadySquadSlot != null)
		{
			m_OnSelectedDiveReadySquadSlot();
		}
	}

	public void SetUnSelected()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_SQUAD_SLOT_SELECTED, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_SQUAD_SLOT_UNSELECTED, bValue: true);
	}

	public void SetSelected(NKMDeckIndex sNKMDeckIndex)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_SQUAD_SLOT_SELECTED, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_SQUAD_SLOT_UNSELECTED, bValue: false);
		NKMDeckData deckData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(sNKMDeckIndex);
		if (deckData != null)
		{
			NKMUnitData shipFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
			if (shipFromUID != null)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
				if (unitTempletBase != null)
				{
					m_NKM_UI_DIVE_INFO_SQUAD_SLOT_SHIP_IMAGE.sprite = NKCResourceUtility.GetOrLoadMinimapFaceIcon(unitTempletBase);
				}
			}
		}
		m_NKM_UI_DIVE_INFO_SQUAD_SLOT_DECKNUMBER_COUNT.text = (sNKMDeckIndex.m_iIndex + 1).ToString();
	}
}
