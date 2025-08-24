using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Unit;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupRearmamentConfirm : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_rearm";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_REARM_CONFIRM";

	private static NKCUIPopupRearmamentConfirm m_Instance;

	public Image m_TargetUnitFaceCard;

	public Text m_TargetUnitName;

	[Header("Before&After")]
	public NKCUIPopupRearmamentConfirmSlotInfo m_BeforeUnitInfo;

	public NKCUIPopupRearmamentConfirmSlotInfo m_AfterUnitInfo;

	[Header("Skill")]
	public NKCUIPopupRearmamentConfirmSkillInfo m_leaderSkillSlot;

	public List<NKCUIPopupRearmamentConfirmSkillInfo> m_lstSkillSlot;

	public NKCUIComStateButton m_stbnConfirm;

	private int m_iTargetRearmID;

	private long m_lResourceRearmUID;

	public static NKCUIPopupRearmamentConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupRearmamentConfirm>("ab_ui_rearm", "AB_UI_POPUP_REARM_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupRearmamentConfirm>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_REARM_CONFIRM_POPUP_TITLE;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.BackButtonOnly;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(m_stbnConfirm, OnClickConfirm);
		NKCUtil.SetHotkey(m_stbnConfirm, HotkeyEventType.Confirm);
	}

	public void Open(int iTargetRearmID, long iResourceUnitUID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(iTargetRearmID);
		if (unitTempletBase == null)
		{
			return;
		}
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(iResourceUnitUID);
		if (unitFromUID == null)
		{
			return;
		}
		NKCUtil.SetImageSprite(m_TargetUnitFaceCard, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase), bDisableIfSpriteNull: true);
		NKCUtil.SetLabelText(m_TargetUnitName, unitTempletBase.GetUnitTitle());
		m_BeforeUnitInfo.SetData(unitFromUID);
		NKMUnitData data = MakeUnitDataAfterRearm(unitFromUID, iTargetRearmID);
		m_AfterUnitInfo.SetData(data);
		int num = 0;
		bool bValue = false;
		for (int i = 0; i < unitTempletBase.GetSkillCount(); i++)
		{
			string skillStrID = unitTempletBase.GetSkillStrID(i);
			if (string.IsNullOrEmpty(skillStrID))
			{
				continue;
			}
			NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(skillStrID, 1);
			if (skillTemplet != null)
			{
				if (skillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_LEADER)
				{
					m_leaderSkillSlot.SetData(skillTemplet);
					bValue = true;
				}
				else
				{
					m_lstSkillSlot[num].SetData(skillTemplet);
					NKCUtil.SetGameobjectActive(m_lstSkillSlot[num].gameObject, bValue: true);
					num++;
				}
			}
		}
		for (int j = num; j < m_lstSkillSlot.Count; j++)
		{
			NKCUtil.SetGameobjectActive(m_lstSkillSlot[j].gameObject, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_leaderSkillSlot.gameObject, bValue);
		m_iTargetRearmID = iTargetRearmID;
		m_lResourceRearmUID = iResourceUnitUID;
		UIOpened();
	}

	private NKMUnitData MakeUnitDataAfterRearm(NKMUnitData unitBefore, int targetUnitID)
	{
		NKMUnitData nKMUnitData = new NKMUnitData();
		nKMUnitData.m_UnitID = targetUnitID;
		nKMUnitData.m_LimitBreakLevel = unitBefore.m_LimitBreakLevel;
		nKMUnitData.m_iUnitLevelEXP = 0;
		nKMUnitData.m_UnitLevel = 1;
		int cumulatedTotalExp = NKCExpManager.GetCumulatedTotalExp(unitBefore);
		NKMUnitExpTemplet nKMUnitExpTemplet = NKMUnitExpTemplet.FindByUnitId(unitBefore.m_UnitID, 110);
		if (nKMUnitExpTemplet == null)
		{
			return unitBefore;
		}
		int expGain = cumulatedTotalExp - nKMUnitExpTemplet.m_iExpCumulated;
		NKCExpManager.CalculateFutureUnitExpAndLevel(nKMUnitData, expGain, out nKMUnitData.m_UnitLevel, out nKMUnitData.m_iUnitLevelEXP);
		return nKMUnitData;
	}

	private void OnClickConfirm()
	{
		NKCUIPopupRearmamentConfirmBox.Instance.Open(m_lResourceRearmUID, m_iTargetRearmID);
	}
}
