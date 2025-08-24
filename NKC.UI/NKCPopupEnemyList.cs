using System;
using System.Collections.Generic;
using System.Linq;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEnemyList : NKCUIBase
{
	[Serializable]
	public struct strSkillInfo
	{
		public GameObject obj;

		public Image Icon;

		public Text Desc;
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_enemy";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_ENEMY_LIST";

	private static NKCPopupEnemyList m_Instance;

	[Header("Common")]
	public Text m_lbBattleTitle;

	public NKCUIComStateButton m_csbtnClose;

	public EventTrigger m_eventTrigger;

	[Header("Enemy UI")]
	public Image m_imgEnemyFace;

	public GameObject m_objBossIcon;

	public Text m_lbEnemyName;

	public Text m_lbEnemyType;

	public Text m_lbEnemyLv;

	public Image m_imgEnemyClass;

	public Text m_lbEnemyClass;

	public Image m_imgEnemyAttackType;

	public Text m_lbEnemyAttackType;

	public RectTransform m_rtLeftSlotParent;

	[Space]
	public GameObject m_objWeakMain;

	public Image m_imgWeakMain;

	public Text m_lbWeakMain;

	public GameObject m_objWeakSub;

	public Image m_imgWeakSub;

	public Text m_lbWeakSub;

	[Space]
	public List<strSkillInfo> m_lstSkillInfo;

	public GameObject m_objTagInfo;

	public GameObject m_objTagNone;

	[Header("Warfare UI")]
	public GameObject m_objBottomWarfareUI;

	public Text m_lbVictoryCond1;

	public Text m_lbVictoryCond2;

	public Text m_lbMedalDesc1;

	public Text m_lbMedalDesc2;

	public Text m_lbMedalDesc3;

	[Space]
	public GameObject m_objBattleCond;

	public Image m_imgBattleCond;

	public NKCUIComStateButton m_csbtnBattleCond;

	[Header("prefab")]
	private List<NKCDeckViewEnemySlot> m_lstEnemySlot = new List<NKCDeckViewEnemySlot>();

	private List<NKCEnemyData> m_lstEnemyData = new List<NKCEnemyData>();

	public static NKCPopupEnemyList Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEnemyList>("ab_ui_nkm_ui_popup_enemy", "NKM_UI_POPUP_ENEMY_LIST", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupEnemyList>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "ENEMY_LIST_POPUP";

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
		Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnClose, base.Close);
		NKCUtil.SetEventTriggerDelegate(m_eventTrigger, base.Close);
	}

	public void Open(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet != null)
		{
			Dictionary<string, NKCEnemyData> enemyUnits = NKMDungeonManager.GetEnemyUnits(stageTemplet);
			(NKM_UNIT_SOURCE_TYPE, NKM_UNIT_SOURCE_TYPE) sourceTypes = stageTemplet.GetSourceTypes();
			Open(enemyUnits.Values.ToList(), sourceTypes.Item1, sourceTypes.Item2);
		}
	}

	public void Open(NKMWarfareTemplet cNKMWarfareTemplet)
	{
		if (cNKMWarfareTemplet != null)
		{
			Dictionary<string, NKCEnemyData> enemyUnits = NKMDungeonManager.GetEnemyUnits(cNKMWarfareTemplet);
			Open(enemyUnits.Values.ToList());
		}
	}

	public void Open(NKMDungeonTempletBase cNKMDungeonTempletBase)
	{
		if (cNKMDungeonTempletBase != null)
		{
			Dictionary<string, NKCEnemyData> enemyUnits = NKMDungeonManager.GetEnemyUnits(cNKMDungeonTempletBase);
			Open(enemyUnits.Values.ToList(), cNKMDungeonTempletBase.m_StageSourceTypeMain, cNKMDungeonTempletBase.m_StageSourceTypeSub);
		}
	}

	public void Open(NKMDungeonTempletBase cNKMDungeonTempletBase, NKMDiveTemplet diveTemplet, bool isBossSector)
	{
		if (cNKMDungeonTempletBase == null)
		{
			return;
		}
		Dictionary<string, NKCEnemyData> enemyUnits = NKMDungeonManager.GetEnemyUnits(cNKMDungeonTempletBase);
		if (diveTemplet != null)
		{
			foreach (NKCEnemyData value in enemyUnits.Values)
			{
				if (isBossSector)
				{
					value.m_Level += diveTemplet.SetLevelScale + diveTemplet.StageLevelScale;
				}
				else
				{
					value.m_Level += diveTemplet.StageLevelScale;
				}
			}
		}
		Open(enemyUnits.Values.ToList(), cNKMDungeonTempletBase.m_StageSourceTypeMain, cNKMDungeonTempletBase.m_StageSourceTypeSub);
	}

	public void Open(List<NKCEnemyData> lstEnemyUnits, NKM_UNIT_SOURCE_TYPE sourceTypeMain = NKM_UNIT_SOURCE_TYPE.NUST_NONE, NKM_UNIT_SOURCE_TYPE sourceTypeSub = NKM_UNIT_SOURCE_TYPE.NUST_NONE)
	{
		for (int num = lstEnemyUnits.Count - 1; num >= 0; num--)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(lstEnemyUnits[num].m_UnitStrID);
			if (nKMUnitTempletBase == null || nKMUnitTempletBase.m_bHideBattleResult)
			{
				lstEnemyUnits.RemoveAt(num);
			}
		}
		NKCUtil.SetLabelText(m_lbBattleTitle, NKCUtilString.GET_STRING_ENEMY_LIST_TITLE);
		if (lstEnemyUnits.Count <= 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		UpdateEnemySlot(lstEnemyUnits, sourceTypeMain, sourceTypeSub);
		NKCUtil.SetGameobjectActive(m_objBottomWarfareUI, bValue: false);
		UIOpened();
	}

	public void Open(int dungeonID, string battleConditionStrID = "")
	{
		NKCUtil.SetGameobjectActive(m_objBottomWarfareUI, bValue: true);
		NKCUtil.SetLabelText(m_lbBattleTitle, NKCUtilString.GET_STRING_ENEMY_LIST_TITLE);
		string msg = NKCUtilString.GET_STRING_WARFARE_POPUP_ENEMY_INFO_KILL;
		string msg2 = "";
		NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(dungeonID);
		if (dungeonTemplet != null && dungeonTemplet.m_DungeonTempletBase != null)
		{
			Dictionary<string, NKCEnemyData> enemyUnits = NKMDungeonManager.GetEnemyUnits(dungeonTemplet.m_DungeonTempletBase);
			UpdateEnemySlot(enemyUnits.Values.ToList(), dungeonTemplet.m_DungeonTempletBase.m_StageSourceTypeMain, dungeonTemplet.m_DungeonTempletBase.m_StageSourceTypeSub);
			if (dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE)
			{
				msg = NKCUtilString.GET_STRING_WARFARE_POPUP_ENEMY_INFO_WAVE;
				msg2 = string.Format(NKCUtilString.GET_STRING_WARFARE_POPUP_ENEMY_INFO_WAVE_ONE_PARAM, dungeonTemplet.m_listDungeonWave.Count);
			}
			NKCUtil.SetLabelText(m_lbMedalDesc3, NKCUtilString.GetDGMissionText(DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR, 0));
			DUNGEON_GAME_MISSION_TYPE dGMissionType_ = dungeonTemplet.m_DungeonTempletBase.m_DGMissionType_1;
			DUNGEON_GAME_MISSION_TYPE dGMissionType_2 = dungeonTemplet.m_DungeonTempletBase.m_DGMissionType_2;
			int dGMissionValue_ = dungeonTemplet.m_DungeonTempletBase.m_DGMissionValue_1;
			int dGMissionValue_2 = dungeonTemplet.m_DungeonTempletBase.m_DGMissionValue_2;
			NKCUtil.SetLabelText(m_lbMedalDesc2, NKCUtilString.GetDGMissionText(dGMissionType_, dGMissionValue_));
			NKCUtil.SetLabelText(m_lbMedalDesc1, NKCUtilString.GetDGMissionText(dGMissionType_2, dGMissionValue_2));
		}
		NKCUtil.SetLabelText(m_lbVictoryCond1, msg);
		NKCUtil.SetLabelText(m_lbVictoryCond2, msg2);
		m_csbtnBattleCond?.PointerDown.RemoveAllListeners();
		if (!string.IsNullOrEmpty(battleConditionStrID))
		{
			NKMBattleConditionTemplet cNKMBattleConditionTemplet = NKMBattleConditionManager.GetTempletByStrID(battleConditionStrID);
			bool flag = cNKMBattleConditionTemplet != null && cNKMBattleConditionTemplet.BattleCondID != 0 && !cNKMBattleConditionTemplet.m_bHide;
			if (flag)
			{
				NKCUtil.SetImageSprite(m_imgBattleCond, NKCUtil.GetSpriteBattleConditionICon(cNKMBattleConditionTemplet));
				m_csbtnBattleCond?.PointerDown.AddListener(delegate(PointerEventData e)
				{
					NKCUITooltip.Instance.Open(NKCUISlot.eSlotMode.Etc, cNKMBattleConditionTemplet.BattleCondName_Translated, cNKMBattleConditionTemplet.BattleCondDesc_Translated, e.position);
				});
			}
			NKCUtil.SetGameobjectActive(m_objBattleCond, flag);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objBattleCond, bValue: false);
		}
		UIOpened();
	}

	private void UpdateEnemySlot(List<NKCEnemyData> lstEnemyUnits, NKM_UNIT_SOURCE_TYPE sourceTypeMain, NKM_UNIT_SOURCE_TYPE sourceTypeSub)
	{
		m_lstEnemyData = lstEnemyUnits;
		Clear();
		m_lstEnemyData.Sort(new NKCEnemyData.CompNED());
		for (int i = 0; i < m_lstEnemyData.Count; i++)
		{
			if (m_lstEnemyData[i] == null)
			{
				continue;
			}
			NKCDeckViewEnemySlot unitSlot = GetSlot(i);
			if (!(null != unitSlot))
			{
				continue;
			}
			if (unitSlot.m_NKCUIComButton != null)
			{
				unitSlot.m_NKCUIComButton.PointerClick.AddListener(delegate
				{
					OnSelectUnitInfo(unitSlot.m_Index, sourceTypeMain, sourceTypeSub);
				});
			}
			unitSlot.SetEnemyData(NKMUnitManager.GetUnitTempletBase(m_lstEnemyData[i].m_UnitStrID), m_lstEnemyData[i]);
			if (sourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				unitSlot.OverrideUnitSourceType(sourceTypeMain, sourceTypeSub);
			}
		}
		OnSelectUnitInfo(0, sourceTypeMain, sourceTypeSub);
	}

	private NKCDeckViewEnemySlot GetSlot(int iCnt)
	{
		NKCDeckViewEnemySlot newInstance = NKCDeckViewEnemySlot.GetNewInstance(m_rtLeftSlotParent);
		if (null != newInstance)
		{
			newInstance.Init(iCnt);
			newInstance.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			Vector3 localPosition = newInstance.gameObject.transform.localPosition;
			newInstance.gameObject.transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
			m_lstEnemySlot.Add(newInstance);
		}
		return newInstance;
	}

	private void Clear()
	{
		for (int i = 0; i < m_lstEnemySlot.Count; i++)
		{
			if (null != m_lstEnemySlot[i])
			{
				UnityEngine.Object.Destroy(m_lstEnemySlot[i].gameObject);
				m_lstEnemySlot[i] = null;
			}
		}
		m_lstEnemySlot.Clear();
	}

	private void OnSelectUnitInfo(int idx, NKM_UNIT_SOURCE_TYPE sourceTypeMain, NKM_UNIT_SOURCE_TYPE sourceTypeSub)
	{
		if (m_lstEnemyData.Count <= idx)
		{
			Debug.LogError($"Size Error {idx} / max {m_lstEnemyData.Count - 1}");
		}
		else
		{
			if (m_lstEnemyData[idx] == null)
			{
				return;
			}
			foreach (NKCDeckViewEnemySlot item in m_lstEnemySlot)
			{
				if (item.m_Index == idx)
				{
					item.ButtonSelect();
				}
				else
				{
					item.ButtonDeSelect(bForce: true, bImmediate: true);
				}
			}
			NKCUtil.SetGameobjectActive(m_objBossIcon, m_lstEnemyData[idx].m_NKM_BOSS_TYPE != NKM_BOSS_TYPE.NBT_NONE);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_lstEnemyData[idx].m_UnitStrID);
			m_imgEnemyFace.preserveAspect = unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP && unitTempletBase.IsShip();
			NKCUtil.SetImageSprite(m_imgEnemyFace, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase));
			NKCUtil.SetLabelText(m_lbEnemyType, NKCUtilString.GetUnitStyleMarkString(unitTempletBase, bUseColor: false));
			string msg = ((!string.IsNullOrEmpty(m_lstEnemyData[idx].m_ChangeUnitName)) ? NKCStringTable.GetString(m_lstEnemyData[idx].m_ChangeUnitName) : unitTempletBase.GetUnitName());
			NKCUtil.SetLabelText(m_lbEnemyName, msg);
			NKCUtil.SetLabelText(m_lbEnemyLv, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, m_lstEnemyData[idx].m_Level));
			NKCUtil.SetImageSprite(m_imgEnemyClass, NKCResourceUtility.GetOrLoadUnitRoleIcon(unitTempletBase));
			if (unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_INVALID)
			{
				NKCUtil.SetLabelText(m_lbEnemyClass, "");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbEnemyClass, NKCUtilString.GetRoleText(unitTempletBase));
			}
			NKCUtil.SetImageSprite(m_imgEnemyAttackType, NKCResourceUtility.GetOrLoadUnitAttackTypeIcon(unitTempletBase));
			NKCUtil.SetLabelText(m_lbEnemyAttackType, NKCUtilString.GetAtkTypeText(unitTempletBase));
			if (sourceTypeMain == NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				sourceTypeMain = unitTempletBase.m_NKM_UNIT_SOURCE_TYPE;
				sourceTypeSub = unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB;
			}
			NKCUtil.SetGameobjectActive(m_objWeakMain, NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE") && sourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
			if (sourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				NKCUtil.SetImageSprite(m_imgWeakMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(sourceTypeMain, bSmall: true));
				NKCUtil.SetLabelText(m_lbWeakMain, NKCUtilString.GetSourceTypeName(sourceTypeMain));
			}
			NKCUtil.SetGameobjectActive(m_objWeakSub, NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE") && sourceTypeSub != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
			if (sourceTypeSub != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				NKCUtil.SetImageSprite(m_imgWeakSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(sourceTypeSub, bSmall: true));
				NKCUtil.SetLabelText(m_lbWeakSub, NKCUtilString.GetSourceTypeName(sourceTypeSub));
			}
			UpdateMonsterSkill(unitTempletBase.m_UnitID);
		}
	}

	private void UpdateMonsterSkill(int unitID)
	{
		Debug.Log($"<color=green>UpdateMonsterSkill : {unitID}</color>");
		NKCMonsterTagTemplet nKCMonsterTagTemplet = NKMTempletContainer<NKCMonsterTagTemplet>.Find(unitID);
		if (nKCMonsterTagTemplet == null || !nKCMonsterTagTemplet.EnabledByTag)
		{
			NKCUtil.SetGameobjectActive(m_objTagNone, bValue: true);
			NKCUtil.SetGameobjectActive(m_objTagInfo, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objTagNone, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTagInfo, bValue: true);
		for (int i = 0; i < m_lstSkillInfo.Count; i++)
		{
			if (nKCMonsterTagTemplet.lstTags.Count > i)
			{
				NKCMonsterTagInfoTemplet nKCMonsterTagInfoTemplet = NKMTempletContainer<NKCMonsterTagInfoTemplet>.Find(nKCMonsterTagTemplet.lstTags[i]);
				if (nKCMonsterTagInfoTemplet != null)
				{
					NKCUtil.SetLabelText(m_lstSkillInfo[i].Desc, NKCStringTable.GetString(nKCMonsterTagInfoTemplet.m_MonsterTagDesc));
					NKCUtil.SetImageSprite(m_lstSkillInfo[i].Icon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_ENEMY_SKILL_ICON", nKCMonsterTagInfoTemplet.m_MonsterTagIcon));
					NKCUtil.SetGameobjectActive(m_lstSkillInfo[i].obj, bValue: true);
					continue;
				}
			}
			NKCUtil.SetGameobjectActive(m_lstSkillInfo[i].obj, bValue: false);
		}
	}
}
