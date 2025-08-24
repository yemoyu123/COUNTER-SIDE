using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCUIHudShipSkillDeck : MonoBehaviour
{
	private int m_Index;

	private long m_UnitUID;

	private int m_UnitID;

	private NKMUnitTemplet m_UnitTemplet;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public RectTransform m_rtSubRoot;

	public EventTrigger m_EventTrigger;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public Image m_imgSkill;

	public Image m_imgGray;

	public Image m_imgAddPanel;

	public Animator m_AnimatorSkillReady;

	[Header("\ufffd\ufffdŸ\ufffd\ufffd")]
	public GameObject m_objCooltime;

	public Image m_imgCooltime;

	[Header("\ufffd\ufffdŸ")]
	public GameObject m_objSelectBorder;

	private NKMTrackingFloat m_TouchScale = new NKMTrackingFloat();

	private float m_fStateCoolTime;

	private bool m_bEventControl;

	private float m_fMoveScale;

	private float m_fScaleBefore;

	private Vector2 m_Vec2Temp;

	public short m_GameUnitUID { get; private set; }

	public NKMShipSkillTemplet m_NKMShipSkillTemplet { get; private set; }

	public void InitUI(int index)
	{
		m_Index = index;
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.BeginDrag;
		entry.callback.AddListener(delegate(BaseEventData eventData)
		{
			NKCSystemEvent.UI_HUD_SHIP_SKILL_DECK_DRAG_BEGIN(eventData);
		});
		m_EventTrigger.triggers.Add(entry);
		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Drag;
		entry.callback.AddListener(delegate(BaseEventData eventData)
		{
			NKCSystemEvent.UI_HUD_SHIP_SKILL_DECK_DRAG(eventData);
		});
		m_EventTrigger.triggers.Add(entry);
		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.EndDrag;
		entry.callback.AddListener(delegate(BaseEventData eventData)
		{
			NKCSystemEvent.UI_HUD_SHIP_SKILL_DECK_DRAG_END(eventData);
		});
		m_EventTrigger.triggers.Add(entry);
		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener(delegate(BaseEventData eventData)
		{
			NKCSystemEvent.UI_HUD_SHIP_SKILL_DECK_DOWN(m_Index, eventData);
		});
		m_EventTrigger.triggers.Add(entry);
		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerUp;
		entry.callback.AddListener(delegate
		{
			NKCSystemEvent.UI_HUD_SHIP_SKILL_DECK_UP(m_Index);
		});
		m_EventTrigger.triggers.Add(entry);
	}

	public void Init()
	{
		m_UnitUID = 0L;
		m_UnitID = 0;
		m_UnitTemplet = null;
		m_NKMShipSkillTemplet = null;
		NKCAssetResourceData nKCAssetResourceData = null;
		nKCAssetResourceData = NKCResourceUtility.GetAssetResource("AB_UI_SHIP_SKILL_ICON", "SS_NO_SKILL_ICON");
		NKCUtil.SetGameobjectActive(m_imgSkill, bValue: true);
		NKCUtil.SetGameobjectActive(m_imgGray, bValue: true);
		if (nKCAssetResourceData != null)
		{
			m_imgSkill.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_imgGray.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_imgAddPanel.sprite = nKCAssetResourceData.GetAsset<Sprite>();
		}
		else
		{
			m_imgSkill.sprite = null;
			m_imgGray.sprite = null;
			m_imgAddPanel.sprite = null;
		}
		NKCUtil.SetGameobjectActive(m_objCooltime, bValue: false);
		m_TouchScale.SetNowValue(1f);
		m_fStateCoolTime = 0f;
		m_bEventControl = false;
	}

	public void SetDeckSprite(NKMUnitData unitData, NKMShipSkillTemplet cNKMShipSkillTemplet)
	{
		if (unitData == null)
		{
			Init();
			return;
		}
		m_UnitTemplet = NKMUnitManager.GetUnitTemplet(unitData.m_UnitID);
		if (m_UnitTemplet == null)
		{
			Init();
			return;
		}
		m_UnitUID = unitData.m_UnitUID;
		m_UnitID = unitData.m_UnitID;
		if (unitData.m_listGameUnitUID.Count > 0)
		{
			m_GameUnitUID = unitData.m_listGameUnitUID[0];
		}
		SetActive(bActive: true);
		m_NKMShipSkillTemplet = cNKMShipSkillTemplet;
		if (cNKMShipSkillTemplet == null)
		{
			Init();
			return;
		}
		NKCUtil.SetGameobjectActive(m_imgAddPanel, bValue: false);
		NKCAssetResourceData nKCAssetResourceData = null;
		nKCAssetResourceData = NKCResourceUtility.GetAssetResource("AB_UI_SHIP_SKILL_ICON", cNKMShipSkillTemplet.m_ShipSkillIcon);
		if (nKCAssetResourceData != null)
		{
			m_imgSkill.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_imgAddPanel.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_imgGray.sprite = nKCAssetResourceData.GetAsset<Sprite>();
		}
		else
		{
			m_imgSkill.sprite = null;
			m_imgAddPanel.sprite = null;
			m_imgGray.sprite = null;
		}
		NKCUtil.SetGameobjectActive(m_imgSkill, bValue: true);
		NKCUtil.SetGameobjectActive(m_imgGray, bValue: true);
	}

	public void UpdateShipSkillDeck(float fDeltaTime)
	{
		m_TouchScale.Update(fDeltaTime);
		float num = m_fMoveScale * m_TouchScale.GetNowValue();
		if (m_fScaleBefore != num)
		{
			m_fScaleBefore = num;
			m_Vec2Temp.Set(num, num);
			m_rtSubRoot.localScale = m_Vec2Temp;
		}
	}

	public void SetDeckData(float fStateCoolTime)
	{
		m_fStateCoolTime = fStateCoolTime;
		float num = 1f;
		if (m_NKMShipSkillTemplet != null)
		{
			if (m_NKMShipSkillTemplet.m_UnitStateName.Length > 1)
			{
				float num2 = 0f;
				NKCUnitClient nKCUnitClient = null;
				if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetGameClient() != null)
				{
					nKCUnitClient = (NKCUnitClient)NKCScenManager.GetScenManager().GetGameClient().GetUnit(m_GameUnitUID, bChain: true, bPool: true);
				}
				if (nKCUnitClient != null)
				{
					num2 = nKCUnitClient.GetStateMaxCoolTime(m_NKMShipSkillTemplet.m_UnitStateName);
				}
				else
				{
					NKMUnitState unitState = m_UnitTemplet.GetUnitState(m_NKMShipSkillTemplet.m_UnitStateName);
					if (unitState != null)
					{
						num2 = unitState.m_StateCoolTime.m_Max;
					}
				}
				num = 1f - fStateCoolTime / num2;
				if (num > 1f)
				{
					num = 1f;
				}
			}
			if (m_NKMShipSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_PASSIVE)
			{
				num = 1f;
			}
		}
		m_imgCooltime.fillAmount = num;
		if (fStateCoolTime > 0f || num < 1f)
		{
			NKCUtil.SetGameobjectActive(m_imgSkill, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgGray, bValue: true);
		}
		else
		{
			if (!m_imgSkill.gameObject.activeSelf)
			{
				m_imgSkill.gameObject.SetActive(value: true);
				if (m_AnimatorSkillReady.gameObject.activeInHierarchy)
				{
					m_AnimatorSkillReady.Play("NKM_UI_GAME_SHIP_SKILL_DECK_CARD_READY", -1, 0f);
				}
			}
			NKCUtil.SetGameobjectActive(m_imgGray, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objCooltime, num < 1f);
	}

	public bool CanUse()
	{
		if (m_imgCooltime.fillAmount >= 1f && m_fStateCoolTime <= 0f)
		{
			return true;
		}
		return false;
	}

	public void MoveShipSkillDeck(float fPosX, float fPosY)
	{
		float width = NKCUIManager.Get_NUF_DRAG().GetWidth();
		float height = NKCUIManager.Get_NUF_DRAG().GetHeight();
		float num = width / (float)Screen.width;
		float num2 = height / (float)Screen.height;
		fPosX = fPosX * num - 0.5f * width;
		fPosY = fPosY * num2 - 0.5f * height;
		m_Vec2Temp.Set(fPosX, fPosY);
		m_rtSubRoot.position = m_Vec2Temp;
		m_rtSubRoot.ForceUpdateRectTransforms();
		m_Vec2Temp = m_rtSubRoot.anchoredPosition;
		float num3 = Vector2.Distance(m_Vec2Temp, Vector2.zero);
		m_fMoveScale = (200f - num3) / 200f;
		if (m_fMoveScale < 0f)
		{
			m_fMoveScale = 0f;
		}
	}

	public void ReturnShipSkillDeck()
	{
		SetActive(m_NKMShipSkillTemplet != null);
		m_rtSubRoot.anchoredPosition = Vector2.zero;
		m_fMoveScale = 1f;
	}

	public void TouchDown()
	{
		m_TouchScale.SetTracking(0.9f, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	public void TouchUp()
	{
		m_TouchScale.SetTracking(1f, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	public void TouchSelectShipSkillDeck()
	{
		m_TouchScale.SetTracking(1.1f, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		if (m_NKMShipSkillTemplet != null && m_NKMShipSkillTemplet.m_NKM_SKILL_TYPE != NKM_SKILL_TYPE.NST_PASSIVE)
		{
			NKCUtil.SetGameobjectActive(m_objSelectBorder, bValue: true);
		}
	}

	public void TouchUnSelectShipSkillDeck()
	{
		m_TouchScale.SetTracking(1f, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		NKCUtil.SetGameobjectActive(m_objSelectBorder, bValue: false);
	}

	public void SetActive(bool bActive, bool bEventControl = false)
	{
		if (!bActive || !m_bEventControl || bEventControl)
		{
			m_bEventControl = bEventControl;
			NKCUtil.SetGameobjectActive(m_rtSubRoot, bActive);
		}
	}
}
