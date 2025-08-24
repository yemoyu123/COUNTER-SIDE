using ClientPacket.WorldMap;
using NKM;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitPlacementSlot : MonoBehaviour
{
	public delegate void OnRelease(NKCUIUnitPlacement.UnitPlacementData placementData);

	public Text m_lbPosition;

	public NKCUIComStateButton m_csbtnRelease;

	public GameObject m_objRelease;

	public GameObject m_objMove;

	private NKCUIUnitPlacement.UnitPlacementData m_placementData;

	private OnRelease dOnRelease;

	public void Init(OnRelease onRelease)
	{
		dOnRelease = onRelease;
		NKCUtil.SetButtonClickDelegate(m_csbtnRelease, OnBtnRelease);
	}

	private void OnBtnRelease()
	{
		dOnRelease?.Invoke(m_placementData);
	}

	public void SetData(NKCUIUnitPlacement.UnitPlacementData data, NKCUIUnitPlacement.UnitType _unitType)
	{
		m_placementData = data;
		bool flag = true;
		switch (data.PlacementType)
		{
		case NKCUIUnitPlacement.UnitPlacementType.Deck:
			if (NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(m_placementData.DeckIndex).m_DeckState != NKM_DECK_STATE.DECK_STATE_NORMAL)
			{
				flag = false;
			}
			if (m_placementData.DeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_PVP_DEFENCE)
			{
				flag = false;
			}
			NKCUtil.SetLabelText(m_lbPosition, NKCUtilString.GetDeckNumberString(m_placementData.DeckIndex));
			break;
		case NKCUIUnitPlacement.UnitPlacementType.OfficeRoom:
		{
			NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(m_placementData.RoomID);
			NKCUtil.SetLabelText(m_lbPosition, NKCStringTable.GetString("SI_PF_DEPLOY_STATUS_OFFICE", NKCStringTable.GetString(nKMOfficeRoomTemplet.Name)));
			break;
		}
		case NKCUIUnitPlacement.UnitPlacementType.MainLobby:
		{
			string strID = ((_unitType == NKCUIUnitPlacement.UnitType.Operator) ? "SI_PF_DEPLOY_STATUS_LOBBY_OPR" : "SI_PF_DEPLOY_STATUS_LOBBY");
			NKCUtil.SetLabelText(m_lbPosition, NKCStringTable.GetString(strID));
			break;
		}
		case NKCUIUnitPlacement.UnitPlacementType.WorldmapCityleader:
		{
			NKMWorldMapCityData cityData = NKCScenManager.CurrentUserData().m_WorldmapData.GetCityData(m_placementData.CityID);
			NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(m_placementData.CityID);
			if (cityData.HasMission())
			{
				NKCUtil.SetLabelText(m_lbPosition, NKCStringTable.GetString("SI_PF_DEPLOY_STATUS_WORLDMAP_CITY_MISSION_DOING", cityTemplet.GetName()));
				flag = false;
			}
			else
			{
				NKCUtil.SetLabelText(m_lbPosition, NKCStringTable.GetString("SI_PF_DEPLOY_STATUS_MANAGER", cityTemplet.GetName()));
				flag = true;
			}
			break;
		}
		}
		NKCUtil.SetGameobjectActive(m_objRelease, flag);
		NKCUtil.SetGameobjectActive(m_objMove, !flag);
	}
}
