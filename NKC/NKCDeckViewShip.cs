using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCDeckViewShip : MonoBehaviour
{
	public delegate void OnShipClicked();

	private NKMUnitData m_ShipUnitData;

	private NKMUnitTempletBase m_ShipUnitTempletBase;

	public Image m_imgShip;

	public NKCUIComButton m_cbtnShip;

	public CanvasGroup m_cgShipInfo;

	public NKCUIShipInfoSummary m_UIShipInfo;

	public CanvasGroup m_cgShipInfoSmall;

	public NKCUIShipInfoSummary m_UIShipInfo_Small;

	public GameObject m_objEmptySelected;

	public GameObject m_objBan;

	public Text m_lbBanLevel;

	public Text m_lbBanApplyDesc;

	public GameObject m_objSeized;

	[Header("디폴트 이미지")]
	public Sprite m_spriteNoShipImage;

	private OnShipClicked dOnShipClicked;

	public void Open(NKMUnitData shipUnitData, bool bEnableShowBan = false)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		SetShipSlotData(shipUnitData, bEnableShowBan);
	}

	public NKMUnitData GetUnitData()
	{
		return m_ShipUnitData;
	}

	public void Init(OnShipClicked onShipClicked)
	{
		dOnShipClicked = onShipClicked;
		m_cbtnShip.PointerClick.RemoveAllListeners();
		m_cbtnShip.PointerClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		if (dOnShipClicked != null)
		{
			dOnShipClicked();
		}
	}

	public void Close()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void SetShipSlotData(NKMUnitData shipUnitData, bool bEnableShowBan = false)
	{
		m_ShipUnitData = shipUnitData;
		if (m_ShipUnitData != null)
		{
			m_ShipUnitTempletBase = NKMUnitManager.GetUnitTempletBase(m_ShipUnitData.m_UnitID);
			if (m_ShipUnitTempletBase != null)
			{
				Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, m_ShipUnitTempletBase);
				if (sprite != null)
				{
					m_imgShip.sprite = sprite;
					if (bEnableShowBan && NKCBanManager.IsBanShip(m_ShipUnitTempletBase.m_ShipGroupID))
					{
						NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
						int shipBanLevel = NKCBanManager.GetShipBanLevel(m_ShipUnitTempletBase.m_ShipGroupID);
						NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, shipBanLevel));
						int nerfPercentByShipBanLevel = NKMUnitStatManager.GetNerfPercentByShipBanLevel(shipBanLevel);
						NKCUtil.SetLabelText(m_lbBanApplyDesc, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_APPLY_DESC_ONE_PARAM, nerfPercentByShipBanLevel));
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
					}
				}
				if (m_UIShipInfo != null)
				{
					m_UIShipInfo.SetShipData(shipUnitData, m_ShipUnitTempletBase, NKMDeckIndex.None, bInDeck: true);
				}
				if (m_UIShipInfo_Small != null)
				{
					m_UIShipInfo_Small.SetShipData(shipUnitData, m_ShipUnitTempletBase, NKMDeckIndex.None, bInDeck: true);
				}
				NKCUtil.SetGameobjectActive(m_objSeized, m_ShipUnitData.IsSeized);
				return;
			}
		}
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		m_imgShip.sprite = m_spriteNoShipImage;
		NKCUtil.SetGameobjectActive(m_objSeized, bValue: false);
		if (m_UIShipInfo != null)
		{
			m_UIShipInfo.SetShipData(null, null, NKMDeckIndex.None);
		}
		if (m_UIShipInfo_Small != null)
		{
			m_UIShipInfo_Small.SetShipData(null, null, NKMDeckIndex.None);
		}
	}

	public void UpdateShipSlotData(NKMUnitData shipUnitData, bool bEnableShowBan = false)
	{
		if (m_ShipUnitData.m_UnitUID == shipUnitData.m_UnitUID)
		{
			SetShipSlotData(shipUnitData, bEnableShowBan);
		}
	}

	public void SetSelectEffect(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objEmptySelected, value);
	}

	public void Enable(bool bInfoActive = true)
	{
		if (m_cgShipInfo != null)
		{
			m_cgShipInfo.DOKill();
			m_cgShipInfo.DOFade(0f, 0.4f).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				NKCUtil.SetGameobjectActive(m_cgShipInfo.gameObject, bValue: false);
			});
		}
		if (m_cgShipInfoSmall != null)
		{
			m_cgShipInfoSmall.DOKill();
			NKCUtil.SetGameobjectActive(m_cgShipInfoSmall.gameObject, bInfoActive);
			m_cgShipInfoSmall.DOFade(1f, 0.4f).SetEase(Ease.OutCubic);
		}
	}

	public void Disable()
	{
		if (m_cgShipInfo != null)
		{
			m_cgShipInfo.DOKill();
			NKCUtil.SetGameobjectActive(m_cgShipInfo.gameObject, bValue: true);
			m_cgShipInfo.DOFade(1f, 0.4f).SetEase(Ease.OutCubic);
		}
		if (m_cgShipInfoSmall != null)
		{
			m_cgShipInfoSmall.DOKill();
			m_cgShipInfoSmall.DOFade(0f, 0.4f).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				NKCUtil.SetGameobjectActive(m_cgShipInfoSmall.gameObject, bValue: false);
			});
		}
	}

	private Sprite GetSpriteMoveType(NKM_UNIT_STYLE_TYPE type)
	{
		string stringMoveType = GetStringMoveType(type);
		if (string.IsNullOrEmpty(stringMoveType))
		{
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_SPRITE", stringMoveType);
	}

	private string GetStringMoveType(NKM_UNIT_STYLE_TYPE type)
	{
		string result = string.Empty;
		switch (type)
		{
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT:
			result = "NKM_DECK_VIEW_SHIP_MOVETYPE_1";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY:
			result = "NKM_DECK_VIEW_SHIP_MOVETYPE_4";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER:
			result = "NKM_DECK_VIEW_SHIP_MOVETYPE_2";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL:
			result = "NKM_DECK_VIEW_SHIP_MOVETYPE_3";
			break;
		}
		return result;
	}
}
