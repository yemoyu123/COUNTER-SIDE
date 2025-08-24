using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIBattleStatisticsSlot : MonoBehaviour
{
	public class UnitSlot
	{
		public NKCDeckViewUnitSlot unitSlot;

		public Text name;

		public Slider damageSlider;

		public Text damageText;

		public Slider recvDamageSlider;

		public Text recvDamageText;

		public GameObject objHeal;

		public Slider healSlider;

		public Text healText;

		public GameObject objCount;

		public Text count;

		public Text time;

		public GameObject objSummon;

		public GameObject objAssist;

		public Image imgShip;

		public GameObject objBoss;

		public GameObject objKill;

		public Text lbKill;
	}

	private Transform m_teamA;

	private Transform m_teamB;

	private UnitSlot m_slotA = new UnitSlot();

	private UnitSlot m_slotB = new UnitSlot();

	private bool m_leader;

	private const string BundleName = "AB_UI_NKM_UI_RESULT";

	private const string AssetNameNormal = "NKM_UI_RESULT_BATTLE_STATISTICS_SLOT_UNIT";

	private const string AssetNameLeader = "NKM_UI_RESULT_BATTLE_STATISTICS_SLOT_LEADER";

	private NKCAssetInstanceData m_instance;

	private bool m_bEnableShowBan;

	private bool m_bEnableShowUpUnit;

	private string COUNT_TEXT => NKCUtilString.GET_STRING_ATTACK_COUNT_ONE_PARAM;

	private string KILL_TEXT => NKCUtilString.GET_STRING_KILL_COUNT_ONE_PARAM;

	public void SetEnableShowBan(bool bSet)
	{
		m_bEnableShowBan = bSet;
	}

	public void SetEnableShowUpUnit(bool bSet)
	{
		m_bEnableShowUpUnit = bSet;
	}

	public void SetLeaderBossMarkActive(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_slotB.objBoss, bSet);
	}

	public static NKCUIBattleStatisticsSlot GetNewInstance(Transform parent, bool bLeader)
	{
		string assetName = (bLeader ? "NKM_UI_RESULT_BATTLE_STATISTICS_SLOT_LEADER" : "NKM_UI_RESULT_BATTLE_STATISTICS_SLOT_UNIT");
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_RESULT", assetName);
		NKCUIBattleStatisticsSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIBattleStatisticsSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIBattleStatisticsSlot Prefab null!");
			return null;
		}
		component.m_instance = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.Init(bLeader);
		component.transform.localScale = Vector3.one;
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void CloseInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
		m_instance = null;
	}

	public void Init(bool bLeader)
	{
		m_teamA = base.transform.Find("LEFT/CONTENTS");
		m_teamB = base.transform.Find("RIGHT/CONTENTS");
		m_leader = bLeader;
		initUI(m_slotA, m_teamA);
		initUI(m_slotB, m_teamB);
		if (!bLeader)
		{
			initNormalUI(m_slotA, m_teamA, bRight: false);
			initNormalUI(m_slotB, m_teamB, bRight: true);
		}
		else
		{
			initLeaderUI(m_slotA, m_teamA, bRight: false);
			initLeaderUI(m_slotB, m_teamB, bRight: true);
		}
	}

	private void initUI(UnitSlot slot, Transform teamRoot)
	{
		slot.unitSlot = teamRoot.Find("ICON/UNIT_DECK/NKM_UI_DECK_VIEW_UNIT_SLOT").GetComponent<NKCDeckViewUnitSlot>();
		slot.unitSlot.Init(0, bEnableDrag: false);
		slot.name = teamRoot.Find("NAME/NAME_TEXT").GetComponent<Text>();
		slot.damageSlider = teamRoot.Find("GAUGE/GAUGE_01/GAUGE_01_Slider").GetComponent<Slider>();
		slot.recvDamageSlider = teamRoot.Find("GAUGE/GAUGE_02/GAUGE_02_Slider").GetComponent<Slider>();
		slot.healSlider = teamRoot.Find("GAUGE/GAUGE_03/GAUGE_03_Slider").GetComponent<Slider>();
		slot.damageText = teamRoot.Find("GAUGE/GAUGE_01/GAUGE_01_Slider/Handle Slide Area/Handle").GetComponent<Text>();
		slot.recvDamageText = teamRoot.Find("GAUGE/GAUGE_02/GAUGE_02_Slider/Handle Slide Area/Handle").GetComponent<Text>();
		slot.healText = teamRoot.Find("GAUGE/GAUGE_03/GAUGE_03_Slider/Handle Slide Area/Handle").GetComponent<Text>();
		slot.objHeal = teamRoot.Find("GAUGE/GAUGE_03").gameObject;
	}

	private void initNormalUI(UnitSlot slot, Transform teamRoot, bool bRight)
	{
		slot.objCount = teamRoot.Find("NAME/COUNT").gameObject;
		slot.count = teamRoot.Find("NAME/COUNT/COUNT_TEXT").GetComponent<Text>();
		if (!bRight)
		{
			slot.time = teamRoot.Find("NAME/TIME/TIME_TEXT").GetComponent<Text>();
		}
		slot.objSummon = teamRoot.Find("ICON/SUMMON").gameObject;
		slot.objAssist = teamRoot.Find("ICON/ASSIST").gameObject;
		slot.objKill = teamRoot.Find("NAME/KILL").gameObject;
		slot.lbKill = teamRoot.Find("NAME/KILL/COUNT_TEXT").GetComponent<Text>();
	}

	private void initLeaderUI(UnitSlot slot, Transform teamRoot, bool bRight)
	{
		slot.imgShip = teamRoot.Find("ICON/SHIP_DECK/ICON").GetComponent<Image>();
		if (bRight)
		{
			slot.objBoss = teamRoot.Find("ICON/BOSS").gameObject;
		}
	}

	public void SetDataA(NKCUIBattleStatistics.UnitBattleData data, float maxValue, bool isDps = false)
	{
		if (data == null)
		{
			NKCUtil.SetGameobjectActive(m_teamA, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_teamA, bValue: true);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(data.unitData.m_UnitID);
		if (m_slotA.unitSlot != null)
		{
			m_slotA.unitSlot.SetEnableShowBan(m_bEnableShowBan);
			m_slotA.unitSlot.SetEnableShowUpUnit(m_bEnableShowUpUnit);
		}
		if (m_leader)
		{
			if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				m_slotA.imgShip.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase);
			}
			else
			{
				m_slotA.unitSlot?.SetData(data.unitData, bEnableButton: false);
			}
			NKCUtil.SetGameobjectActive(m_slotA.unitSlot, unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP);
			m_slotA.imgShip.enabled = unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP;
		}
		else
		{
			m_slotA.unitSlot?.SetData(data.unitData, bEnableButton: false);
			m_slotA.unitSlot?.SetLeader(data.bLeader, bEffect: false);
			m_slotA.time.text = data.recordData.playtime.ToString();
			NKCUtil.SetGameobjectActive(m_slotA.objSummon, data.bSummon);
			NKCUtil.SetGameobjectActive(m_slotA.objCount, !data.bSummon);
			NKCUtil.SetGameobjectActive(m_slotA.objAssist, data.bAssist);
			if (!data.bSummon)
			{
				m_slotA.count.text = string.Format(COUNT_TEXT, data.recordData.recordSummonCount);
			}
		}
		string text = ((!string.IsNullOrEmpty(data.recordData.changeUnitName)) ? NKCStringTable.GetString(data.recordData.changeUnitName) : unitTempletBase.GetUnitName());
		m_slotA.name.text = text;
		NKCUtil.SetGameobjectActive(m_slotA.objKill, data.recordData.recordKillCount > 0);
		if (m_slotA.objKill != null && m_slotA.objKill.activeSelf)
		{
			NKCUtil.SetLabelText(m_slotA.lbKill, string.Format(KILL_TEXT, data.recordData.recordKillCount));
		}
		setSlider(m_slotA, data.recordData, maxValue, isDps);
	}

	public void SetDataB(NKCUIBattleStatistics.UnitBattleData data, float maxValue, bool isDps = false, bool showBossMark = false)
	{
		if (data == null)
		{
			NKCUtil.SetGameobjectActive(m_teamB, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_teamB, bValue: true);
		if (m_slotB.unitSlot != null)
		{
			m_slotB.unitSlot.SetEnableShowBan(m_bEnableShowBan);
			m_slotB.unitSlot.SetEnableShowUpUnit(m_bEnableShowUpUnit);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(data.unitData.m_UnitID);
		if (m_leader)
		{
			if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				m_slotB.imgShip.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase);
			}
			else
			{
				m_slotB.unitSlot?.SetData(data.unitData, bEnableButton: false);
			}
			NKCUtil.SetGameobjectActive(m_slotB.unitSlot, unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP);
			m_slotB.imgShip.enabled = unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP;
		}
		else
		{
			m_slotB.unitSlot?.SetData(data.unitData, bEnableButton: false);
			m_slotB.unitSlot?.SetLeader(data.bLeader, bEffect: false);
			NKCUtil.SetGameobjectActive(m_slotB.objSummon, data.bSummon);
			NKCUtil.SetGameobjectActive(m_slotB.objCount, !data.bSummon);
			NKCUtil.SetGameobjectActive(m_slotB.objAssist, data.bAssist);
			if (!data.bSummon)
			{
				m_slotB.count.text = string.Format(COUNT_TEXT, data.recordData.recordSummonCount);
			}
		}
		NKCUtil.SetGameobjectActive(m_slotB.objBoss, m_leader && showBossMark);
		string text = ((!string.IsNullOrEmpty(data.recordData.changeUnitName)) ? NKCStringTable.GetString(data.recordData.changeUnitName) : unitTempletBase.GetUnitName());
		m_slotB.name.text = text;
		NKCUtil.SetGameobjectActive(m_slotB.objKill, data.recordData.recordKillCount > 0);
		if (m_slotB.objKill != null && m_slotB.objKill.activeSelf)
		{
			NKCUtil.SetLabelText(m_slotB.lbKill, string.Format(KILL_TEXT, data.recordData.recordKillCount));
		}
		setSlider(m_slotB, data.recordData, maxValue, isDps);
	}

	private void setSlider(UnitSlot unitSlot, NKMGameRecordUnitData data, float maxValue, bool isDps)
	{
		if (maxValue <= 0f)
		{
			maxValue = 1f;
		}
		float num = 0f;
		float num2 = 0f;
		if (!isDps)
		{
			num = data.recordGiveDamage;
			num2 = data.recordTakeDamage;
		}
		else if (data.playtime > 0)
		{
			num = data.recordGiveDamage / (float)data.playtime;
			num2 = data.recordTakeDamage / (float)data.playtime;
		}
		unitSlot.damageSlider.value = num / maxValue;
		unitSlot.recvDamageSlider.value = num2 / maxValue;
		unitSlot.healSlider.value = data.recordHeal / maxValue;
		unitSlot.damageText.text = $"{(int)num:#,##0}";
		unitSlot.recvDamageText.text = $"{(int)num2:#,##0}";
		unitSlot.healText.text = $"{(int)data.recordHeal:#,##0}";
		NKCUtil.SetGameobjectActive(unitSlot.objHeal, data.recordHeal > 0f && !isDps);
	}
}
