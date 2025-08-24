using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupSkillFullInfo : NKCUIBase
{
	private const string UNIT_POPUP_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_info";

	private const string UI_ASSET_NAME_FOR_UNIT = "NKM_UI_POPUP_SKILL_FULL_INFO";

	private const string SHIP_POPUP_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_ship_info";

	private const string UI_ASSET_NAME_FOR_SHIP = "NKM_UI_POPUP_SKILL_SHIP_INFO";

	private static NKCPopupSkillFullInfo m_UnitInstance;

	private static NKCPopupSkillFullInfo m_ShipInstance;

	private NKCUIOpenAnimator m_openAni;

	public GameObject m_objRearmUnitTitle;

	public List<NKCPopupSkillInfo> m_lstSkillInfo = new List<NKCPopupSkillInfo>();

	public Text m_lbTitle;

	public NKCUIComStateButton m_btnClose;

	public EventTrigger m_eventTrigger;

	private bool m_bInitComplete;

	public static NKCPopupSkillFullInfo UnitInstance
	{
		get
		{
			if (m_UnitInstance == null)
			{
				m_UnitInstance = NKCUIManager.OpenNewInstance<NKCPopupSkillFullInfo>("ab_ui_nkm_ui_unit_info", "NKM_UI_POPUP_SKILL_FULL_INFO", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupUnitInstance).GetInstance<NKCPopupSkillFullInfo>();
				m_UnitInstance.InitUI();
			}
			return m_UnitInstance;
		}
	}

	public static NKCPopupSkillFullInfo ShipInstance
	{
		get
		{
			if (m_ShipInstance == null)
			{
				m_ShipInstance = NKCUIManager.OpenNewInstance<NKCPopupSkillFullInfo>("ab_ui_nkm_ui_ship_info", "NKM_UI_POPUP_SKILL_SHIP_INFO", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupShipInstance).GetInstance<NKCPopupSkillFullInfo>();
				m_ShipInstance.InitUI();
			}
			return m_ShipInstance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_POPUP_SKILL_FULL_INFO;

	private static void CleanupUnitInstance()
	{
		m_UnitInstance = null;
	}

	private static void CleanupShipInstance()
	{
		m_ShipInstance = null;
	}

	private void InitUI()
	{
		if (m_bInitComplete)
		{
			return;
		}
		for (int i = 0; i < m_lstSkillInfo.Count; i++)
		{
			if (m_lstSkillInfo[i] == null)
			{
				Debug.LogError($"m_lstSkillInfo[{i}] 스킬정보 프리팹이 없음");
				return;
			}
		}
		if (m_lbTitle == null)
		{
			Debug.LogError("m_lbTitle is null");
			return;
		}
		if (m_btnClose == null)
		{
			Debug.LogError("m_btnClose is null");
			return;
		}
		m_openAni = new NKCUIOpenAnimator(base.gameObject);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback = new EventTrigger.TriggerEvent();
		entry.callback.AddListener(OnUserInputEvent);
		m_eventTrigger = base.transform.Find("BG").GetComponent<EventTrigger>();
		m_eventTrigger.triggers.Add(entry);
		m_bInitComplete = true;
	}

	public override void CloseInternal()
	{
		Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Clear()
	{
		foreach (NKCPopupSkillInfo item in m_lstSkillInfo)
		{
			item.Clear();
		}
	}

	public void OpenForUnit(NKMUnitData unitData, string unitName, int unitStarGradeMax, int unitLimitBreakLevel, bool bIsRearmUnit)
	{
		m_openAni.PlayOpenAni();
		m_lbTitle.text = string.Format(NKCUtilString.GET_STRING_UNIT_SKILL_INFO_ONE_PARAM, unitName);
		NKCUtil.SetGameobjectActive(m_objRearmUnitTitle, bValue: false);
		List<NKMUnitSkillTemplet> unitAllSkillTempletList = NKMUnitSkillManager.GetUnitAllSkillTempletList(unitData);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase == null)
		{
			return;
		}
		bool bValue = false;
		int num = 1;
		for (int i = 0; i < m_lstSkillInfo.Count; i++)
		{
			if (unitAllSkillTempletList.Count < num)
			{
				break;
			}
			NKMUnitSkillTemplet nKMUnitSkillTemplet = unitAllSkillTempletList[i];
			if (nKMUnitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_LEADER)
			{
				m_lstSkillInfo[0].InitUI();
				m_lstSkillInfo[0].OpenForUnit(nKMUnitSkillTemplet, unitStarGradeMax, unitLimitBreakLevel, unitTempletBase.m_RearmGrade, unitTempletBase.StopDefaultCoolTime);
				bValue = true;
			}
			else
			{
				m_lstSkillInfo[num].InitUI();
				m_lstSkillInfo[num].OpenForUnit(nKMUnitSkillTemplet, unitStarGradeMax, unitLimitBreakLevel, unitTempletBase.m_RearmGrade, unitTempletBase.StopDefaultCoolTime);
				NKCUtil.SetGameobjectActive(m_lstSkillInfo[num], bValue: true);
				num++;
			}
		}
		NKCUtil.SetGameobjectActive(m_lstSkillInfo[0], bValue);
		for (int j = num; j < m_lstSkillInfo.Count; j++)
		{
			NKCUtil.SetGameobjectActive(m_lstSkillInfo[j], bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objRearmUnitTitle, bIsRearmUnit);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
	}

	public void OpenForShip(int unitID, long shipUID = 0L)
	{
		m_openAni.PlayOpenAni();
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_lbTitle, string.Format(NKCUtilString.GET_STRING_UNIT_SKILL_INFO_ONE_PARAM, unitTempletBase.GetUnitName()));
		int hasShipLv = 0;
		switch (shipUID)
		{
		case -1L:
			hasShipLv = 6;
			break;
		default:
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				NKMUnitData shipFromUID = nKMUserData.m_ArmyData.GetShipFromUID(shipUID);
				if (shipFromUID != null)
				{
					hasShipLv = GetShipLevel(shipFromUID.m_UnitID);
				}
			}
			break;
		}
		case 0L:
			break;
		}
		for (int i = 0; i < m_lstSkillInfo.Count; i++)
		{
			m_lstSkillInfo[i].InitUI();
			m_lstSkillInfo[i].OpenForShip(i, unitID, hasShipLv);
		}
		NKCUtil.SetGameobjectActive(m_objRearmUnitTitle, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
	}

	private int GetShipLevel(int shipID)
	{
		if (shipID < 10000)
		{
			return 0;
		}
		return (int)((double)shipID * 0.001) % 10;
	}

	private void OnUserInputEvent(BaseEventData eventData)
	{
		Close();
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_openAni.Update();
		}
	}
}
