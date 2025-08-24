using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCGameHudDeckSlot : MonoBehaviour
{
	private NKCGameHud m_NKCGameHud;

	private int m_Index;

	private NKMUnitTemplet m_UnitTemplet;

	[Header("메인 오브젝트")]
	public RectTransform m_rtSubRoot;

	public EventTrigger m_eventTrigger;

	public Animator m_animatorUnitCardRoot;

	[Header("Unit Image")]
	public Image m_imgUnitPanel;

	public Image m_imgUnitAddPanel;

	public Image m_imgUnitGrayPanel;

	[Header("적 표식/퇴각")]
	public GameObject m_objCardEnemy;

	public GameObject m_objTouchBorder;

	public NKCUIComStateButton m_csbtnRetreat;

	[Header("코스트")]
	public Text m_lbCost;

	public GameObject m_objCostRoot;

	public GameObject m_objCostBGGray;

	[Header("배경")]
	public RectTransform m_rtBG;

	public Image m_imgBG;

	public Image m_imgBGEmpty;

	[Header("소환 코스트 쿨타임")]
	public GameObject m_objSummonCool;

	public Image m_imgSummonCool;

	[Header("추가 정보")]
	public GameObject m_objInBattle;

	public GameObject m_objAuto;

	public GameObject m_objAssist;

	public NKCUIGameUnitSkillCooltime m_SkillCoolTime;

	[Header("유닛 정보")]
	public NKCUIComTextUnitLevel m_comUnitLevel;

	public Image m_imgUnitRole;

	public Image m_imgAttackType;

	[Header("출격제한")]
	public GameObject m_objRespawnLimit;

	public Text m_lbRespawnLimit;

	[Header("기타")]
	public GameObject m_objBorderCombo;

	private float m_fRespawnCostNow;

	private float m_fRespawnCostRateNow;

	private bool m_bCanRespawn;

	public NKMTrackingFloat m_TrackPosX = new NKMTrackingFloat();

	public NKMTrackingFloat m_TrackScale = new NKMTrackingFloat();

	public NKMTrackingFloat m_TouchScale = new NKMTrackingFloat();

	private bool m_bEventControl;

	private Vector2 m_Vec2Temp;

	public NKMUnitData m_UnitData { get; private set; }

	public bool RespawnReady { get; set; }

	public bool RetreatReady { get; set; }

	public void InitUI(NKCGameHud cNKCGameHud, int index)
	{
		m_NKCGameHud = cNKCGameHud;
		m_Index = index;
		if (m_Index < 4 || m_Index == 5)
		{
			m_eventTrigger = GetComponentInChildren<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.BeginDrag;
			entry.callback.AddListener(delegate(BaseEventData eventData)
			{
				NKCSystemEvent.UI_HUD_DECK_DRAG_BEGIN(eventData);
			});
			m_eventTrigger.triggers.Add(entry);
			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.Drag;
			entry.callback.AddListener(delegate(BaseEventData eventData)
			{
				NKCSystemEvent.UI_HUD_DECK_DRAG(eventData);
			});
			m_eventTrigger.triggers.Add(entry);
			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.EndDrag;
			entry.callback.AddListener(delegate(BaseEventData eventData)
			{
				NKCSystemEvent.UI_HUD_DECK_DRAG_END(eventData);
			});
			m_eventTrigger.triggers.Add(entry);
			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(delegate
			{
				NKCSystemEvent.UI_HUD_DECK_DOWN(m_Index);
			});
			m_eventTrigger.triggers.Add(entry);
			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerUp;
			entry.callback.AddListener(delegate
			{
				NKCSystemEvent.UI_HUD_DECK_UP(m_Index);
			});
			m_eventTrigger.triggers.Add(entry);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnRetreat, UnitRetreat);
	}

	public void Init()
	{
		m_UnitData = null;
		m_UnitTemplet = null;
		m_imgUnitPanel.sprite = null;
		m_imgUnitGrayPanel.sprite = null;
		NKCUtil.SetImageSprite(m_imgUnitAddPanel, null);
		NKCUtil.SetGameobjectActive(m_imgUnitAddPanel, bValue: false);
		SetAuto(bSet: false);
		SetAssist(bSet: false);
		NKCUtil.SetGameobjectActive(m_objTouchBorder, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnRetreat, bValue: false);
		NKCUtil.SetGameobjectActive(m_objInBattle, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRespawnLimit, bValue: false);
		RespawnReady = false;
		RetreatReady = false;
		m_fRespawnCostNow = 0f;
		m_bCanRespawn = true;
		m_bEventControl = false;
		m_TrackScale.SetNowValue(1f);
		m_TouchScale.SetNowValue(1f);
		m_TrackPosX.SetNowValue(m_rtSubRoot.anchoredPosition.x);
		if (m_SkillCoolTime != null)
		{
			m_SkillCoolTime.SetSkillCoolVisible(value: false);
			m_SkillCoolTime.SetHyperCoolVisible(value: false);
		}
		Color color = m_imgBG.color;
		color.r = 1f;
		color.g = 1f;
		color.b = 1f;
		m_imgBG.color = color;
	}

	public void SetEnemy(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objCardEnemy, bSet);
	}

	public void SetAuto(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objAuto, bSet);
	}

	public void SetAssist(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objAssist, bSet);
	}

	public void SetEmpty(bool bEmpty)
	{
		NKCUtil.SetGameobjectActive(m_imgBG, !bEmpty);
		NKCUtil.SetGameobjectActive(m_imgBGEmpty, bEmpty);
		NKCUtil.SetGameobjectActive(m_imgUnitPanel, !bEmpty);
		NKCUtil.SetGameobjectActive(m_imgUnitAddPanel, !bEmpty);
		NKCUtil.SetGameobjectActive(m_imgUnitGrayPanel, !bEmpty);
		SetAuto(bSet: false);
		SetAssist(bSet: false);
		NKCUtil.SetGameobjectActive(m_objSummonCool, !bEmpty);
		NKCUtil.SetGameobjectActive(m_objCostRoot, !bEmpty);
		NKCUtil.SetGameobjectActive(m_lbCost, !bEmpty);
		NKCUtil.SetGameobjectActive(m_objBorderCombo, bValue: false);
		if (bEmpty)
		{
			m_comUnitLevel.text = "";
			NKCUtil.SetGameobjectActive(m_imgUnitRole, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgAttackType, bValue: false);
			if (m_imgUnitRole != null)
			{
				m_imgUnitRole.sprite = null;
			}
			m_imgAttackType.sprite = null;
			if (m_SkillCoolTime != null)
			{
				m_SkillCoolTime.SetSkillCoolVisible(value: false);
				m_SkillCoolTime.SetHyperCoolVisible(value: false);
			}
		}
	}

	public void SetDeckSprite(NKMUnitData unitData, bool bLeader, bool bAssist, bool bAutoRespawn, float fTime = 0.3f)
	{
		if (unitData == null)
		{
			SetEmpty(bEmpty: true);
			Init();
			return;
		}
		m_UnitTemplet = NKMUnitManager.GetUnitTemplet(unitData.m_UnitID);
		if (m_UnitTemplet == null)
		{
			SetEmpty(bEmpty: true);
			Init();
			return;
		}
		SetEmpty(bEmpty: false);
		NKCUtil.SetGameobjectActive(m_imgUnitAddPanel, bValue: false);
		SetAuto(bAutoRespawn);
		SetAssist(bAssist);
		NKCUtil.SetGameobjectActive(m_objCostBGGray, bAssist);
		m_UnitData = unitData;
		NKCAssetResourceData nKCAssetResourceData = null;
		nKCAssetResourceData = NKCResourceUtility.GetUnitResource(NKCResourceUtility.eUnitResourceType.INVEN_ICON, m_UnitData);
		if (nKCAssetResourceData == null)
		{
			m_NKCGameHud?.LoadUnitDeck(m_UnitData, bAsync: false);
			nKCAssetResourceData = NKCResourceUtility.GetUnitResource(NKCResourceUtility.eUnitResourceType.INVEN_ICON, m_UnitData);
		}
		if (nKCAssetResourceData != null)
		{
			m_imgUnitPanel.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_imgUnitAddPanel.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_imgUnitGrayPanel.sprite = nKCAssetResourceData.GetAsset<Sprite>();
		}
		if (m_NKCGameHud != null)
		{
			int respawnCost = m_NKCGameHud.GetRespawnCost(m_UnitTemplet.m_StatTemplet, bLeader);
			NKCUtil.SetLabelText(m_lbCost, respawnCost.ToString());
			if (bLeader)
			{
				if (m_NKCGameHud.IsBanUnit(m_UnitTemplet.m_UnitTempletBase.m_UnitID))
				{
					m_lbCost.color = new Color(1f, 0.3f, 0.3f);
				}
				else if (m_NKCGameHud.IsUpUnit(m_UnitTemplet.m_UnitTempletBase.m_UnitID))
				{
					m_lbCost.color = new Color(0f, 1f, 1f);
				}
				else
				{
					m_lbCost.color = new Color(1f, 0.8039f, 0.02745f);
				}
			}
			else if (m_NKCGameHud.IsBanUnit(m_UnitTemplet.m_UnitTempletBase.m_UnitID))
			{
				m_lbCost.color = new Color(1f, 0.3f, 0.3f);
			}
			else if (m_NKCGameHud.IsUpUnit(m_UnitTemplet.m_UnitTempletBase.m_UnitID))
			{
				m_lbCost.color = new Color(0f, 1f, 1f);
			}
			else
			{
				m_lbCost.color = new Color(1f, 1f, 1f);
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbCost, "");
		}
		m_comUnitLevel.SetLevel(m_UnitData, 0, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM);
		NKCUtil.SetGameobjectActive(m_imgUnitRole, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgAttackType, bValue: true);
		m_imgAttackType.sprite = NKCResourceUtility.GetOrLoadUnitRoleAttackTypeIcon(m_UnitTemplet.m_UnitTempletBase, bSmall: true);
		if (fTime > 0f)
		{
			m_TrackPosX.SetNowValue(-1000f);
			m_TrackPosX.SetTracking(0f, fTime, TRACKING_DATA_TYPE.TDT_SLOWER);
			Vector2 anchoredPosition = m_rtSubRoot.anchoredPosition;
			anchoredPosition.y = 0f;
			SetAnchoredPos(anchoredPosition);
			m_TrackScale.SetNowValue(0f);
			m_TrackScale.SetTracking(1f, fTime, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		if (m_SkillCoolTime != null)
		{
			m_SkillCoolTime.SetUnit(m_UnitTemplet, m_UnitData);
		}
	}

	public void SetDeckUnitLevel(NKMUnitData unitData, int buffUnitLevel)
	{
		m_comUnitLevel.SetLevel(m_UnitData, buffUnitLevel, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM);
	}

	public void SetDeckData(float fRespawnCostNow, bool bCanRespawn, bool bLeader, float fSkillCoolNow, float fSkillCoolMax, float fHyperSkillCoolNow, float fHyperSkillMax, NKMTacticalCombo cNKMTacticalComboGoal, int RespawnLimitCount)
	{
		if (m_UnitTemplet == null)
		{
			return;
		}
		m_fRespawnCostNow = fRespawnCostNow;
		m_bCanRespawn = bCanRespawn;
		int num = 0;
		if (m_NKCGameHud != null)
		{
			num = ((!bLeader) ? m_NKCGameHud.GetRespawnCost(m_UnitTemplet.m_StatTemplet, bLeader: false) : m_NKCGameHud.GetRespawnCost(m_UnitTemplet.m_StatTemplet, bLeader: true));
		}
		m_fRespawnCostRateNow = m_fRespawnCostNow / (float)num;
		if (m_fRespawnCostRateNow > 1f)
		{
			m_fRespawnCostRateNow = 1f;
		}
		m_imgSummonCool.fillAmount = m_fRespawnCostRateNow;
		if (!m_bCanRespawn || m_fRespawnCostRateNow < 1f)
		{
			NKCUtil.SetGameobjectActive(m_imgUnitPanel, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgUnitGrayPanel, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_imgUnitPanel, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgUnitGrayPanel, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objSummonCool, m_fRespawnCostRateNow < 1f);
		Color color = m_imgBG.color;
		if (CanRespawn())
		{
			if (color.r < 1f && m_animatorUnitCardRoot.gameObject.activeInHierarchy)
			{
				m_animatorUnitCardRoot.Play("READY", -1, 0f);
			}
			color.r = 1f;
			color.g = 1f;
			color.b = 1f;
		}
		else
		{
			color.r = 0.5f;
			color.g = 0.5f;
			color.b = 0.5f;
		}
		m_imgBG.color = color;
		if (!m_bCanRespawn)
		{
			NKCUtil.SetGameobjectActive(m_objInBattle, bValue: true);
			if (m_NKCGameHud.GetSelectUnitDeckIndex() == m_Index)
			{
				if (m_UnitData != null && m_NKCGameHud.GetGameClient().IsGameUnitAllInBattle(m_UnitData.m_UnitUID) == NKM_ERROR_CODE.NEC_OK)
				{
					NKCUtil.SetGameobjectActive(m_csbtnRetreat, bValue: true);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_csbtnRetreat, bValue: false);
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objInBattle, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnRetreat, bValue: false);
			if (RetreatReady)
			{
				UseCompleteDeck();
			}
		}
		if (m_SkillCoolTime != null)
		{
			m_SkillCoolTime.SetSkillCooltime(fSkillCoolNow, fSkillCoolMax);
			m_SkillCoolTime.SetHyperCooltime(fHyperSkillCoolNow, fHyperSkillMax);
		}
		if (cNKMTacticalComboGoal == null)
		{
			NKCUtil.SetGameobjectActive(m_objBorderCombo, bValue: false);
		}
		else
		{
			bool bValue = cNKMTacticalComboGoal.CheckCond(m_UnitTemplet.m_UnitTempletBase, num);
			NKCUtil.SetGameobjectActive(m_objBorderCombo, bValue);
		}
		if (RespawnLimitCount > 0)
		{
			NKCUtil.SetGameobjectActive(m_objRespawnLimit, bValue: true);
			NKCUtil.SetLabelText(m_lbRespawnLimit, RespawnLimitCount.ToString());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRespawnLimit, bValue: false);
		}
	}

	public bool CanRespawn()
	{
		if (m_fRespawnCostRateNow >= 1f && m_bCanRespawn)
		{
			return true;
		}
		return false;
	}

	public void MoveDeck(float fPosX, float fPosY)
	{
		float width = NKCUIManager.Get_NUF_DRAG().GetWidth();
		float height = NKCUIManager.Get_NUF_DRAG().GetHeight();
		float num = width / (float)Screen.width;
		float num2 = height / (float)Screen.height;
		fPosX = fPosX * num - 0.5f * width;
		fPosY = fPosY * num2 - 0.5f * height;
		m_Vec2Temp.Set(fPosX, fPosY);
		SetPos(m_Vec2Temp);
		m_rtSubRoot.ForceUpdateRectTransforms();
		m_Vec2Temp = m_rtSubRoot.anchoredPosition;
		float num3 = Vector2.Distance(m_Vec2Temp, Vector2.zero);
		Vector3 localScale = m_rtSubRoot.localScale;
		float num4 = (200f - num3) / 200f;
		if (num4 < 0f)
		{
			num4 = 0f;
		}
		localScale.x = num4;
		localScale.y = num4;
		SetScale(localScale);
	}

	public void ReturnDeck(bool bReturnDeckActive = true)
	{
		if (!RespawnReady && (m_UnitData == null || !m_NKCGameHud.GetGameClient().GetMyTeamData().IsAssistUnit(m_UnitData.m_UnitUID) || m_NKCGameHud.GetGameClient().GetMyTeamData().m_DeckData.GetAutoRespawnIndexAssist() != -1))
		{
			SetActive(bReturnDeckActive);
			SetAnchoredPos(Vector2.zero);
			SetScale(Vector2.one);
		}
	}

	public void UseDeck(bool bRetreat)
	{
		RespawnReady = true;
		if (bRetreat)
		{
			RetreatReady = true;
		}
		SetActive(bActive: false);
	}

	public void UseCompleteDeck(bool bReturnDeckActive = true)
	{
		RespawnReady = false;
		RetreatReady = false;
		ReturnDeck(bReturnDeckActive);
	}

	public void UpdateDeck(float fDeltaTime)
	{
		bool flag = m_TrackPosX.IsTracking();
		m_TrackPosX.Update(fDeltaTime);
		if (m_TrackPosX.IsTracking() || flag)
		{
			Vector2 anchoredPosition = m_rtSubRoot.anchoredPosition;
			anchoredPosition.x = m_TrackPosX.GetNowValue();
			SetAnchoredPos(anchoredPosition);
		}
		flag = m_TrackScale.IsTracking();
		m_TrackScale.Update(fDeltaTime);
		m_TouchScale.Update(fDeltaTime);
		if (m_TrackScale.IsTracking() || m_TouchScale.IsTracking() || flag)
		{
			Vector3 localScale = m_rtSubRoot.localScale;
			localScale.x = m_TrackScale.GetNowValue() * m_TouchScale.GetNowValue();
			localScale.y = m_TrackScale.GetNowValue() * m_TouchScale.GetNowValue();
			localScale.z = 1f;
			SetScale(localScale);
		}
	}

	public void Enable()
	{
		NKCUtil.SetGameobjectActive(m_imgUnitGrayPanel, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSummonCool, bValue: false);
		Color color = m_imgBG.color;
		color.r = 1f;
		color.g = 1f;
		color.b = 1f;
		m_imgBG.color = color;
		NKCUtil.SetGameobjectActive(m_imgUnitPanel, bValue: true);
	}

	public void TouchDown()
	{
		m_TouchScale.SetNowValue(m_rtSubRoot.localScale.x);
		m_TouchScale.SetTracking(0.9f, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	public void TouchUp()
	{
		m_TouchScale.SetNowValue(m_rtSubRoot.localScale.x);
		m_TouchScale.SetTracking(1f, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	public void TouchSelectUnitDeck(bool bUseTouchScale)
	{
		if (bUseTouchScale)
		{
			m_TouchScale.SetNowValue(m_rtSubRoot.localScale.x);
			m_TouchScale.SetTracking(1.1f, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		NKCUtil.SetGameobjectActive(m_objTouchBorder, bValue: true);
		if (!m_bCanRespawn && m_UnitData != null && m_NKCGameHud.GetGameClient().IsGameUnitAllInBattle(m_UnitData.m_UnitUID) == NKM_ERROR_CODE.NEC_OK)
		{
			NKCUtil.SetGameobjectActive(m_csbtnRetreat, bValue: true);
		}
	}

	public void TouchUnSelectUnitDeck()
	{
		m_TouchScale.SetNowValue(m_rtSubRoot.localScale.x);
		m_TouchScale.SetTracking(1f, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		NKCUtil.SetGameobjectActive(m_objTouchBorder, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnRetreat, bValue: false);
	}

	public void Drag(Vector2 pos, GameObject frontObject = null)
	{
		if (frontObject != null)
		{
			SetParent(frontObject);
		}
		SetPos(pos);
	}

	public void DragEnd(float fTrackingTime = 0f)
	{
		if (m_rtSubRoot.parent != base.transform)
		{
			SetParent();
		}
		Vector2 anchoredPosition = m_rtSubRoot.anchoredPosition;
		anchoredPosition.Set(0f, 0f);
		SetAnchoredPos(anchoredPosition);
	}

	private void SetPos(Vector2 pos)
	{
		m_rtSubRoot.position = pos;
	}

	private void SetAnchoredPos(Vector2 pos)
	{
		m_rtSubRoot.anchoredPosition = pos;
	}

	private void SetScale(Vector2 scale)
	{
		m_rtSubRoot.localScale = scale;
	}

	private void SetScale(Vector3 scale)
	{
		m_rtSubRoot.localScale = scale;
	}

	public void SetActive(bool bActive, bool bEventControl = false)
	{
		if (!bActive || !m_bEventControl || bEventControl)
		{
			m_bEventControl = bEventControl;
			if (!bActive)
			{
				NKCUtil.SetGameobjectActive(m_objTouchBorder, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnRetreat, bValue: false);
				NKCUtil.SetGameobjectActive(m_imgUnitAddPanel, bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_rtSubRoot, bActive);
		}
	}

	public void SetParent(GameObject go = null)
	{
		if (go == null)
		{
			m_rtSubRoot.SetParent(base.transform);
		}
		else
		{
			m_rtSubRoot.SetParent(go.transform);
		}
	}

	public RectTransform GetRectATKMark()
	{
		if (m_imgAttackType == null)
		{
			return null;
		}
		return m_imgAttackType.gameObject.GetComponent<RectTransform>();
	}

	private void UnitRetreat()
	{
		if (m_UnitData != null && m_NKCGameHud.GetGameClient() != null)
		{
			m_NKCGameHud.GetGameClient().Send_Packet_GAME_UNIT_RETREAT_REQ(m_UnitData.m_UnitUID);
			m_NKCGameHud.UseDeck(m_Index, bRetreat: true);
			m_NKCGameHud.GetGameClient().UI_HUD_DECK_UP(m_Index);
		}
	}
}
