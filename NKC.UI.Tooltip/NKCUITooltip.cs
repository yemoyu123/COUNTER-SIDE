using System.Collections.Generic;
using System.Text;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Tooltip;

public class NKCUITooltip : NKCUIBase
{
	public enum DataType
	{
		None,
		Item,
		ShipSkill,
		UnitSkill,
		SkillLevel,
		Text,
		Ship,
		Unit,
		DiveArtifact,
		TacticalCommand,
		OperatorSkill,
		OperatorSkillCombo,
		Etc
	}

	public class Data
	{
		public DataType Type;
	}

	public class ItemData : Data
	{
		public NKCUISlot.SlotData Slot;

		public ItemData(NKCUISlot.SlotData slot)
		{
			Type = DataType.Item;
			Slot = slot;
		}
	}

	public class ShipSkillData : Data
	{
		public NKMShipSkillTemplet ShipSkillTemplet;

		public ShipSkillData(NKMShipSkillTemplet shipSkillTemplet)
		{
			Type = DataType.ShipSkill;
			ShipSkillTemplet = shipSkillTemplet;
		}
	}

	public class TacticalCommandData : Data
	{
		public NKMTacticalCommandTemplet TacticalCommandTemplet;

		public TacticalCommandData(NKMTacticalCommandTemplet tacticalCommandTemplet)
		{
			Type = DataType.TacticalCommand;
			TacticalCommandTemplet = tacticalCommandTemplet;
		}
	}

	public class UnitSkillData : Data
	{
		public NKMUnitSkillTemplet UnitSkillTemplet;

		public int UnitStarGradeMax;

		public int UnitLimitBreakLevel;

		public bool IsFury;

		public UnitSkillData(NKMUnitSkillTemplet unitSkillTemplet, int unitStarMax, int unitLimitBreakLv, bool bIsFury)
		{
			Type = DataType.UnitSkill;
			UnitSkillTemplet = unitSkillTemplet;
			UnitStarGradeMax = unitStarMax;
			UnitLimitBreakLevel = unitLimitBreakLv;
			IsFury = bIsFury;
		}
	}

	public class SkillLevelData : Data
	{
		public NKMUnitSkillTemplet SkillTemplet;

		public SkillLevelData(NKMUnitSkillTemplet skillTemplet)
		{
			Type = DataType.SkillLevel;
			SkillTemplet = skillTemplet;
		}
	}

	public class TextData : Data
	{
		public string Text;

		public TextData(string text)
		{
			Type = DataType.Text;
			Text = text;
		}
	}

	public class UnitData : Data
	{
		public NKCUISlot.SlotData Slot;

		public NKMUnitTempletBase UnitTempletBase;

		public UnitData(NKCUISlot.SlotData slot, NKMUnitTempletBase unitTempletBase)
		{
			if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				Type = DataType.Unit;
			}
			else
			{
				Type = DataType.Ship;
			}
			Slot = slot;
			UnitTempletBase = unitTempletBase;
		}
	}

	public class DiveArtifactData : Data
	{
		public NKCUISlot.SlotData Slot;

		public DiveArtifactData(NKCUISlot.SlotData slot)
		{
			Type = DataType.DiveArtifact;
			Slot = slot;
		}
	}

	public class EtcData : Data
	{
		public NKCUISlot.SlotData Slot;

		public string m_Title;

		public string m_Desc;

		public EtcData(string title, string desc)
		{
			Type = DataType.Etc;
			m_Title = title;
			m_Desc = desc;
		}
	}

	public class OperatorSkillData : Data
	{
		public NKMOperatorSkillTemplet skillTemplet;

		public int skillLevel;

		public OperatorSkillData(NKMOperatorSkillTemplet templet, int skillLv)
		{
			Type = DataType.OperatorSkill;
			skillTemplet = templet;
			skillLevel = skillLv;
		}
	}

	public class OperatorSkillComboData : Data
	{
		public List<NKMTacticalCombo> skillCombo = new List<NKMTacticalCombo>();

		public OperatorSkillComboData(List<NKMTacticalCombo> lstCombo)
		{
			Type = DataType.OperatorSkillCombo;
			skillCombo = lstCombo;
		}
	}

	private enum PivotType
	{
		None,
		RightUp,
		RightDown,
		LeftUp,
		LeftDown
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_TOOLTIP";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_TOOLTIP";

	private static NKCUITooltip m_Instance;

	public RectTransform m_rtPanel;

	public RectTransform m_rtParent;

	public RectTransform m_rtDeco;

	public VerticalLayoutGroup m_vlg;

	public GameObject m_deco_rightUp;

	public GameObject m_deco_rightDown;

	public GameObject m_deco_leftUp;

	public GameObject m_deco_leftDown;

	private List<NKCAssetInstanceData> m_listAsset = new List<NKCAssetInstanceData>();

	private RectTransform m_RectToCalcTouchPos;

	private Vector2 firstTouch = Vector2.zero;

	private bool bFirstTouch = true;

	private const int VERTICAL_LAYOUT_GROUP_SPACING = 160;

	private const int VERTICAL_LAYOUT_GROUP_SPACING_SKILL = 20;

	private const float TOUCH_DISTANCE = 50f;

	private Vector3 m_touchPos;

	public static NKCUITooltip Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUITooltip>("AB_UI_NKM_UI_POPUP_TOOLTIP", "NKM_UI_POPUP_TOOLTIP", NKCUIManager.eUIBaseRect.UIOverlay, CleanupInstance).GetInstance<NKCUITooltip>();
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

	public override string MenuName => NKCUtilString.GET_STRING_TOOLTIP;

	public override eMenutype eUIType => eMenutype.Overlay;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		Clear();
	}

	private void Init()
	{
		if (!(m_RectToCalcTouchPos != null))
		{
			m_RectToCalcTouchPos = GetComponent<RectTransform>();
		}
	}

	public void Open(NKCUISlot.SlotData slotData, Vector2? touchPos)
	{
		if (slotData == null)
		{
			return;
		}
		List<Data> list = new List<Data>();
		if (slotData.eType == NKCUISlot.eSlotMode.DiveArtifact)
		{
			DiveArtifactData item = new DiveArtifactData(slotData);
			list.Add(item);
		}
		else if (slotData.eType == NKCUISlot.eSlotMode.Unit || slotData.eType == NKCUISlot.eSlotMode.UnitCount)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(slotData.ID);
			if (unitTempletBase == null)
			{
				return;
			}
			UnitData item2 = new UnitData(slotData, unitTempletBase);
			list.Add(item2);
		}
		else
		{
			ItemData item3 = new ItemData(slotData);
			list.Add(item3);
			TextData item4 = new TextData(NKCUISlot.GetDesc(slotData.eType, slotData.ID));
			list.Add(item4);
		}
		m_vlg.spacing = 160f;
		Open(list, touchPos);
	}

	public void Open(NKCUISlot.eSlotMode type, string title, string desc, Vector2? touchPos)
	{
		List<Data> list = new List<Data>();
		if (type == NKCUISlot.eSlotMode.Etc)
		{
			EtcData item = new EtcData(title, desc);
			list.Add(item);
		}
		m_vlg.spacing = 160f;
		Open(list, touchPos);
	}

	public void Open(NKMUnitTempletBase unitTempletBase, Vector2? touchPos)
	{
		if (unitTempletBase != null)
		{
			List<Data> list = new List<Data>();
			UnitData item = new UnitData(NKCUISlot.SlotData.MakeUnitData(unitTempletBase.m_UnitID, 1), unitTempletBase);
			list.Add(item);
			m_vlg.spacing = 160f;
			Open(list, touchPos);
		}
	}

	public void Open(NKMShipSkillTemplet shipSkillTemplet, Vector2? touchPos)
	{
		if (shipSkillTemplet != null)
		{
			List<Data> list = new List<Data>();
			ShipSkillData item = new ShipSkillData(shipSkillTemplet);
			list.Add(item);
			TextData item2 = new TextData(shipSkillTemplet.GetDesc());
			list.Add(item2);
			m_vlg.spacing = 160f;
			Open(list, touchPos);
		}
	}

	public void Open(NKMTacticalCommandTemplet cNKMTacticalCommandTemplet, int level, Vector2? touchPos)
	{
		if (cNKMTacticalCommandTemplet != null && (cNKMTacticalCommandTemplet.CheckMyTeamTargetBuffExist(bPvP: false) || cNKMTacticalCommandTemplet.CheckEnemyTargetBuffExist(bPvP: false)))
		{
			List<Data> list = new List<Data>();
			TacticalCommandData item = new TacticalCommandData(cNKMTacticalCommandTemplet);
			list.Add(item);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(NKCUtilString.ApplyBuffValueToString(cNKMTacticalCommandTemplet, level));
			stringBuilder.Append("\n\n");
			if (cNKMTacticalCommandTemplet.m_NKM_TACTICAL_COMMAND_TYPE == NKM_TACTICAL_COMMAND_TYPE.NTCT_ACTIVE)
			{
				stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_TC_COST"), cNKMTacticalCommandTemplet.m_StartCost, cNKMTacticalCommandTemplet.m_CostAdd) + "\n");
			}
			else if (cNKMTacticalCommandTemplet.m_NKM_TACTICAL_COMMAND_TYPE == NKM_TACTICAL_COMMAND_TYPE.NTCT_COMBO)
			{
				stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_COOLTIME_ONE_PARAM"), (int)cNKMTacticalCommandTemplet.m_fCoolTime) + "\n");
			}
			TextData item2 = new TextData(stringBuilder.ToString());
			list.Add(item2);
			m_vlg.spacing = 160f;
			Open(list, touchPos);
		}
	}

	public void Open(NKMUnitSkillTemplet unitSkillTemplet, Vector2? touchPos, int unitStarGradeMax, int unitLimitBreakLevel = 0, bool bIsFury = false)
	{
		if (unitSkillTemplet == null)
		{
			return;
		}
		List<Data> list = new List<Data>();
		UnitSkillData item = new UnitSkillData(unitSkillTemplet, unitStarGradeMax, unitLimitBreakLevel, bIsFury);
		list.Add(item);
		if (unitSkillTemplet.m_Level == 1)
		{
			TextData item2 = new TextData(unitSkillTemplet.GetSkillDesc());
			list.Add(item2);
		}
		else
		{
			NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(unitSkillTemplet.m_ID, 1);
			if (skillTemplet != null)
			{
				TextData item3 = new TextData(skillTemplet.GetSkillDesc());
				list.Add(item3);
			}
		}
		SkillLevelData item4 = new SkillLevelData(unitSkillTemplet);
		list.Add(item4);
		m_vlg.spacing = 20f;
		Open(list, touchPos);
	}

	public void Open(int operatorSkillID, int operatorSkillLevel, Vector2? touchPos)
	{
		NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(operatorSkillID);
		if (skillTemplet == null)
		{
			return;
		}
		List<Data> list = new List<Data>();
		OperatorSkillData item = new OperatorSkillData(skillTemplet, operatorSkillLevel);
		list.Add(item);
		TextData item2 = new TextData(NKCOperatorUtil.MakeOperatorSkillDesc(skillTemplet, operatorSkillLevel));
		list.Add(item2);
		if (skillTemplet.m_OperSkillType == OperatorSkillType.m_Tactical)
		{
			NKMTacticalCommandTemplet tacticalCommandTempletByStrID = NKMTacticalCommandManager.GetTacticalCommandTempletByStrID(skillTemplet.m_OperSkillTarget);
			if (tacticalCommandTempletByStrID.m_listComboType != null && tacticalCommandTempletByStrID.m_listComboType.Count > 0)
			{
				OperatorSkillComboData item3 = new OperatorSkillComboData(tacticalCommandTempletByStrID.m_listComboType);
				list.Add(item3);
			}
		}
		m_vlg.spacing = 20f;
		Open(list, touchPos);
	}

	public void Open(TextData textData, Vector2? touchPos)
	{
		if (!string.IsNullOrEmpty(textData.Text))
		{
			List<Data> list = new List<Data>();
			list.Add(textData);
			Open(list, touchPos);
		}
	}

	private void Open(List<Data> datas, Vector2? touchPos)
	{
		if (m_listAsset.Count > 0)
		{
			Clear();
		}
		for (int i = 0; i < datas.Count; i++)
		{
			Data data = datas[i];
			string assetName = GetName(data.Type);
			NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_POPUP_TOOLTIP", assetName);
			if (nKCAssetInstanceData.m_Instant == null)
			{
				Debug.LogError("툴팁 " + data.Type);
				return;
			}
			m_listAsset.Add(nKCAssetInstanceData);
			Transform obj = nKCAssetInstanceData.m_Instant.transform;
			obj.SetParent(m_rtParent.transform);
			obj.localScale = Vector3.one;
			Vector3 localPosition = obj.localPosition;
			localPosition.z = 0f;
			obj.localPosition = localPosition;
			NKCUITooltipBase component = obj.GetComponent<NKCUITooltipBase>();
			component.Init();
			component.SetData(data);
		}
		bFirstTouch = true;
		UIOpened();
		if (touchPos.HasValue)
		{
			m_touchPos = touchPos.Value;
		}
		SetPosition(touchPos);
	}

	private void Update()
	{
		if (!Input.anyKey)
		{
			Close();
		}
		if (Input.GetMouseButton(0))
		{
			if (bFirstTouch)
			{
				firstTouch = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				bFirstTouch = false;
			}
			else
			{
				Vector2 vector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				if ((firstTouch - vector).sqrMagnitude > 10000f)
				{
					Close();
				}
			}
		}
		if (base.IsOpen)
		{
			m_rtPanel.SetHeight(m_rtParent.GetHeight());
			m_rtDeco.transform.localPosition = m_rtParent.transform.localPosition;
			m_rtDeco.sizeDelta = m_rtParent.sizeDelta;
			SetPosition(m_touchPos);
		}
	}

	private void Clear()
	{
		for (int i = 0; i < m_listAsset.Count; i++)
		{
			NKCAssetResourceManager.CloseInstance(m_listAsset[i]);
		}
		m_listAsset.Clear();
	}

	private void OnDestroy()
	{
		if (m_RectToCalcTouchPos != null)
		{
			Object.Destroy(m_RectToCalcTouchPos.gameObject);
			m_RectToCalcTouchPos = null;
		}
		m_Instance = null;
	}

	private void SetPosition(Vector2? touchPos)
	{
		PivotType pivotType = VectorToPivotType(touchPos);
		Vector3 zero = Vector3.zero;
		if (touchPos.HasValue)
		{
			m_touchPos = touchPos.Value;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RectToCalcTouchPos, touchPos.Value, NKCCamera.GetSubUICamera(), out var localPoint);
			zero.x = localPoint.x;
			zero.y = localPoint.y;
			zero.z = 0f;
		}
		Vector3 localPosition;
		switch (pivotType)
		{
		default:
			m_rtPanel.pivot = new Vector2(0.5f, 0.5f);
			localPosition = Vector3.zero;
			break;
		case PivotType.RightUp:
			m_rtPanel.pivot = new Vector2(1f, 1f);
			localPosition = zero + new Vector3(-1f, -1f, 0f) * 50f;
			break;
		case PivotType.RightDown:
			m_rtPanel.pivot = new Vector2(1f, 0f);
			localPosition = zero + new Vector3(-1f, 1f, 0f) * 50f;
			break;
		case PivotType.LeftUp:
			m_rtPanel.pivot = new Vector2(0f, 1f);
			localPosition = zero + new Vector3(1f, -1f, 0f) * 50f;
			break;
		case PivotType.LeftDown:
			m_rtPanel.pivot = new Vector2(0f, 0f);
			localPosition = zero + new Vector3(1f, 1f, 0f) * 50f;
			break;
		}
		NKCUtil.SetGameobjectActive(m_deco_rightUp, pivotType == PivotType.RightUp);
		NKCUtil.SetGameobjectActive(m_deco_rightDown, pivotType == PivotType.RightDown);
		NKCUtil.SetGameobjectActive(m_deco_leftUp, pivotType == PivotType.LeftUp);
		NKCUtil.SetGameobjectActive(m_deco_leftDown, pivotType == PivotType.LeftDown);
		float num = m_rtPanel.GetHeight() + 50f;
		float height = m_RectToCalcTouchPos.GetHeight();
		float num2;
		float num3;
		switch (pivotType)
		{
		default:
			num2 = height * 0.5f - zero.y;
			num3 = -1f;
			break;
		case PivotType.RightUp:
		case PivotType.LeftUp:
			num2 = height * 0.5f + zero.y;
			num3 = 1f;
			break;
		}
		if (num > num2)
		{
			localPosition += new Vector3(0f, (num - num2) * num3, 0f);
		}
		m_rtPanel.localPosition = localPosition;
	}

	private PivotType VectorToPivotType(Vector2? touchPos)
	{
		if (!touchPos.HasValue)
		{
			return PivotType.None;
		}
		Vector2 value = touchPos.Value;
		float num = (float)Screen.width * 0.5f;
		float num2 = (float)Screen.height * 0.5f;
		if (value.x > num)
		{
			if (value.y > num2)
			{
				return PivotType.RightUp;
			}
			return PivotType.RightDown;
		}
		if (value.y > num2)
		{
			return PivotType.LeftUp;
		}
		return PivotType.LeftDown;
	}

	private string GetName(DataType type)
	{
		return type switch
		{
			DataType.Item => "NKM_UI_POPUP_TOOLTIP_ITEM", 
			DataType.ShipSkill => "NKM_UI_POPUP_TOOLTIP_SKILL", 
			DataType.UnitSkill => "NKM_UI_POPUP_TOOLTIP_SKILL_UNIT", 
			DataType.SkillLevel => "NKM_UI_POPUP_TOOLTIP_SKILL_LEVEL", 
			DataType.Text => "NKM_UI_POPUP_TOOLTIP_TEXT", 
			DataType.Unit => "NKM_UI_POPUP_TOOLTIP_UNIT", 
			DataType.Ship => "NKM_UI_POPUP_TOOLTIP_SHIP", 
			DataType.DiveArtifact => "NKM_UI_POPUP_TOOLTIP_ARTIFACT", 
			DataType.TacticalCommand => "NKM_UI_POPUP_TOOLTIP_TACTICAL_COMMAND", 
			DataType.OperatorSkill => "NKM_UI_POPUP_TOOLTIP_SKILL_OPERATOR", 
			DataType.OperatorSkillCombo => "NKM_UI_POPUP_TOOLTIP_SKILL_OPERATOR_COMBO", 
			DataType.Etc => "NKM_UI_POPUP_TOOLTIP_ETC", 
			_ => "", 
		};
	}
}
