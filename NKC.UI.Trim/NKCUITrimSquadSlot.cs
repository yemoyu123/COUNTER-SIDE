using System.Collections.Generic;
using System.Text;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Trim;

public class NKCUITrimSquadSlot : MonoBehaviour
{
	public delegate void OnDeckConfirm();

	public GameObject m_objWarning;

	public GameObject m_objShipRoot;

	public GameObject m_objAddImage;

	public GameObject m_objStageNormalState;

	public GameObject m_objStageLowPowerState;

	public Image m_imgShip;

	public Image m_imgBoss;

	public Text m_lbShipLevel;

	public Text m_lbStageNumberOn;

	public Text m_lbStageNumberOff;

	public NKCUIComStateButton m_csbtnAddSquad;

	public NKCUIComStateButton m_csbtnWarning;

	public NKCUIComStateButton m_csbtnBoss;

	public GameObject m_objSquadNormal;

	public GameObject m_objSquadWarning;

	public Text m_lbSquadPowerNormal;

	public Text m_lbSquadPowerWarning;

	private int m_slotIndex;

	private int m_trimId;

	private int m_trimDungeonId;

	private int m_trimLevel;

	private int m_operationLowDelta;

	private OnDeckConfirm m_dOnDeckConfirm;

	public bool IsActive => base.gameObject.activeSelf;

	public void Init(int slotIndex, OnDeckConfirm onDeckConfirm)
	{
		m_dOnDeckConfirm = onDeckConfirm;
		m_slotIndex = slotIndex;
		NKCUtil.SetButtonClickDelegate(m_csbtnAddSquad, OnClickAddSquad);
		NKCUtil.SetButtonClickDelegate(m_csbtnBoss, OnClickBoss);
		if (m_csbtnWarning != null)
		{
			m_csbtnWarning.PointerDown.RemoveAllListeners();
			m_csbtnWarning.PointerDown.AddListener(OnSquadButtonDown);
		}
	}

	public void SetData(int trimId, int trimDungeonId, int trimGroup, int trimLevel)
	{
		m_trimId = trimId;
		m_trimDungeonId = trimDungeonId;
		m_trimLevel = trimLevel;
		int num = m_slotIndex + 1;
		NKCUtil.SetLabelText(m_lbStageNumberOn, num.ToString());
		NKCUtil.SetLabelText(m_lbStageNumberOff, num.ToString());
		NKMUserData shipStateData = NKCScenManager.CurrentUserData();
		SetShipStateData(shipStateData);
		SetWarningState(trimGroup, trimLevel);
		SetBossImage(trimId, trimDungeonId, trimLevel);
	}

	private void SetWarningState(int trimGroup, int trimLevel)
	{
		NKMTrimPointTemplet nKMTrimPointTemplet = NKMTrimPointTemplet.Find(trimGroup, trimLevel);
		int num = int.MaxValue;
		if (nKMTrimPointTemplet != null)
		{
			num = nKMTrimPointTemplet.RecommendCombatPoint;
		}
		int operationPower = NKCLocalDeckDataManager.GetOperationPower(m_slotIndex, bPVP: false, bPossibleShowBan: false, bPossibleShowUp: false);
		bool flag = num > operationPower && operationPower > 0;
		m_operationLowDelta = 0;
		if (flag)
		{
			m_operationLowDelta = (num - operationPower) * 10000 / num;
		}
		NKCUtil.SetGameobjectActive(m_objWarning, flag);
		NKCUtil.SetGameobjectActive(m_objStageNormalState, !flag);
		NKCUtil.SetGameobjectActive(m_objStageLowPowerState, flag);
		if (operationPower == 0)
		{
			NKCUtil.SetLabelText(m_lbSquadPowerNormal, "-");
			NKCUtil.SetGameobjectActive(m_objSquadNormal, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSquadWarning, bValue: false);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSquadPowerNormal, operationPower.ToString("N0"));
			NKCUtil.SetLabelText(m_lbSquadPowerWarning, operationPower.ToString("N0"));
			NKCUtil.SetGameobjectActive(m_objSquadNormal, !flag);
			NKCUtil.SetGameobjectActive(m_objSquadWarning, flag);
		}
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
	}

	private void SetShipStateData(NKMUserData userData)
	{
		long shipUId = NKCLocalDeckDataManager.GetShipUId(m_slotIndex);
		NKCLocalDeckDataManager.GetOperationPower(m_slotIndex, bPVP: false, bPossibleShowBan: false, bPossibleShowUp: false);
		if (shipUId > 0)
		{
			NKCUtil.SetGameobjectActive(m_objShipRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objAddImage, bValue: false);
			NKMUnitData nKMUnitData = userData?.m_ArmyData.GetShipFromUID(shipUId);
			Sprite sp = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, nKMUnitData);
			NKCUtil.SetImageSprite(m_imgShip, sp);
			int num = 0;
			if (nKMUnitData != null)
			{
				num = nKMUnitData.m_UnitLevel;
			}
			NKCUtil.SetLabelText(m_lbShipLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, num.ToString()));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objShipRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objAddImage, bValue: true);
		}
	}

	private void SetBossImage(int trimId, int trimDungeonId, int trimLevel)
	{
		List<NKMTrimDungeonTemplet> value = null;
		NKMTrimTemplet.Find(trimId)?.TrimDungeonTemplets.TryGetValue(trimDungeonId, out value);
		NKMTrimDungeonTemplet nKMTrimDungeonTemplet = value?.Find((NKMTrimDungeonTemplet e) => e.TrimLevelLow <= trimLevel && e.TrimLevelHigh >= trimLevel);
		Sprite sp = null;
		if (nKMTrimDungeonTemplet != null && !string.IsNullOrEmpty(nKMTrimDungeonTemplet.TimeBossFaceCard))
		{
			sp = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_TRIM_BOSS_THUMBNAIL", nKMTrimDungeonTemplet.TimeBossFaceCard);
		}
		NKCUtil.SetImageSprite(m_imgBoss, sp);
	}

	private void OnClickAddSquad()
	{
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(m_trimId);
		int num = ((nKMTrimTemplet != null) ? nKMTrimTemplet.TrimDungeonIds.Length : 0);
		if (num > 0)
		{
			NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
			{
				eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.PrepareLocalDeck,
				DeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_TRIM, m_slotIndex),
				ShowDeckIndexList = new List<int>()
			};
			for (int i = 0; i < num; i++)
			{
				options.ShowDeckIndexList.Add(i);
			}
			options.dCheckSideMenuButton = CheckDeckButtonConfirm;
			options.dOnSideMenuButtonConfirm = OnDeckSideButtonConfirm;
			options.dOnBackButton = OnDeckBackButton;
			NKCUIDeckViewer.Instance.Open(options);
		}
	}

	private NKM_ERROR_CODE CheckDeckButtonConfirm(NKMDeckIndex selectedDeckIndex)
	{
		return NKM_ERROR_CODE.NEC_OK;
	}

	private void OnDeckSideButtonConfirm(NKMDeckIndex selectedDeckIndex, long supportUserUID = 0L)
	{
		NKCLocalDeckDataManager.SaveLocalDeck();
		NKCUIDeckViewer.CheckInstanceAndClose();
		if (m_dOnDeckConfirm != null)
		{
			m_dOnDeckConfirm();
		}
	}

	private void OnDeckBackButton()
	{
		NKCLocalDeckDataManager.SaveLocalDeck();
		NKCUIDeckViewer.CheckInstanceAndClose();
		if (m_dOnDeckConfirm != null)
		{
			m_dOnDeckConfirm();
		}
	}

	private void OnSquadButtonDown(PointerEventData pointEventData)
	{
		if (m_operationLowDelta <= 0)
		{
			return;
		}
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(m_trimId);
		string text = null;
		foreach (NKMTrimCombatPenaltyTemplet item in nKMTrimTemplet?.TrimCombatPenaltyList)
		{
			text = item.BattleConditionId;
			if (item.LowCombatRate > m_operationLowDelta)
			{
				break;
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(text);
			if (templetByStrID != null)
			{
				int num = m_operationLowDelta / 10;
				string text2 = null;
				text2 = ((num % 10 != 0 || m_operationLowDelta / 100 != 0) ? ((float)m_operationLowDelta / 100f).ToString("F1") : ((float)m_operationLowDelta / 100f).ToString("F2"));
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Format(NKCUtilString.GET_STRING_TRIM_SQUAD_COMBAT_PENALTY, text2));
				stringBuilder.Append("\n");
				stringBuilder.Append(templetByStrID.BattleCondDesc_Translated);
				NKCUITrimToolTip.Instance.Open(stringBuilder.ToString(), pointEventData.position);
			}
		}
	}

	private void OnClickBoss()
	{
		List<NKMTrimDungeonTemplet> value = null;
		NKMTrimTemplet.Find(m_trimId)?.TrimDungeonTemplets.TryGetValue(m_trimDungeonId, out value);
		NKMTrimDungeonTemplet obj = value?.Find((NKMTrimDungeonTemplet e) => e.TrimLevelLow <= m_trimLevel && e.TrimLevelHigh >= m_trimLevel);
		NKCPopupEnemyList.Instance.Open(obj?.DungeonTempletBase);
	}

	private void OnDestroy()
	{
		m_dOnDeckConfirm = null;
	}
}
