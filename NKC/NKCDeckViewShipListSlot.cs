using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCDeckViewShipListSlot : MonoBehaviour
{
	public delegate void OnShipChange(long shipUID);

	private int m_Index;

	private NKMUnitData m_ShipUnitData;

	private RectTransform m_RectTransform;

	public Animator m_Animator;

	public NKCUIComButton m_cbtnSlot;

	public Image m_imgShip;

	public Text m_lbLevel;

	public Text m_lbName;

	public GameObject m_objRootHasDeck;

	public GameObject m_objRootNoDeck;

	public Text m_lbDeckNumber;

	public NKCUIComButton m_cbtnDetail;

	public NKCUIComButton m_cbtnChange;

	private NKMTrackingFloat m_PosX = new NKMTrackingFloat();

	private OnShipChange dOnShipChange;

	public NKMUnitData ShipUnitData => m_ShipUnitData;

	public NKCUIComButton GetNKCUIComButton()
	{
		return m_cbtnSlot;
	}

	public static NKCDeckViewShipListSlot GetNewInstance(int index, Transform parent, OnShipChange delegateOnShipChange)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_DECK_VIEW_SHIP_LIST_SLOT", "NKM_UI_DECK_VIEW_SHIP_LIST_SLOT");
		if (nKCAssetInstanceData == null || nKCAssetInstanceData.m_Instant == null)
		{
			Debug.LogError("NKCDeckViewShipListSlot Prefab null!");
			return null;
		}
		NKCDeckViewShipListSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCDeckViewShipListSlot>();
		if (component == null)
		{
			Debug.LogError("NKCDeckViewShipListSlot Prefab null!");
			return null;
		}
		component.m_Index = index;
		component.transform.SetParent(parent);
		component.m_RectTransform = component.GetComponent<RectTransform>();
		component.m_RectTransform.anchoredPosition = new Vector2(0f, -index * 230);
		component.m_RectTransform.localScale = Vector3.one;
		component.m_cbtnChange.PointerClick.RemoveAllListeners();
		component.m_cbtnChange.PointerClick.AddListener(component.ShipChangeButtonClicked);
		component.dOnShipChange = delegateOnShipChange;
		component.gameObject.SetActive(value: false);
		return component;
	}

	private void ShipChangeButtonClicked()
	{
		if (dOnShipChange != null)
		{
			dOnShipChange(m_ShipUnitData.m_UnitUID);
		}
	}

	public bool Update()
	{
		if (!base.gameObject.activeSelf)
		{
			return false;
		}
		m_PosX.Update(Time.deltaTime);
		UpdatePos();
		return true;
	}

	public void UpdatePos()
	{
		Vector2 anchoredPosition = m_RectTransform.anchoredPosition;
		anchoredPosition.x = m_PosX.GetNowValue();
		m_RectTransform.anchoredPosition = anchoredPosition;
	}

	public void SetData(NKMUnitData cShipUnitData, NKM_DECK_TYPE eCurrentDeckType, NKMDeckIndex deckIndex, bool bAnimate)
	{
		m_ShipUnitData = cShipUnitData;
		if (m_ShipUnitData != null)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			m_lbLevel.text = m_ShipUnitData.m_UnitLevel.ToString();
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_ShipUnitData.m_UnitID);
			if (unitTempletBase != null)
			{
				NKCAssetResourceData unitResource = NKCResourceUtility.GetUnitResource(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
				if (unitResource != null)
				{
					m_imgShip.sprite = unitResource.GetAsset<Sprite>();
				}
				m_lbName.text = unitTempletBase.GetUnitName();
			}
			else
			{
				m_lbName.text = "";
			}
			if (deckIndex.m_eDeckType == eCurrentDeckType)
			{
				m_objRootHasDeck.SetActive(value: true);
				m_objRootNoDeck.SetActive(value: false);
				m_lbDeckNumber.text = NKCUtilString.GetDeckNumberString(deckIndex);
			}
			else
			{
				m_objRootHasDeck.SetActive(value: false);
				m_objRootNoDeck.SetActive(value: true);
			}
			if (bAnimate)
			{
				FadeInMove();
			}
		}
		else if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public bool FadeInMove()
	{
		if (!base.gameObject.activeSelf)
		{
			return false;
		}
		m_PosX.SetNowValue(900f);
		m_PosX.SetTracking(0f, 0.2f * (float)(m_Index + 1), TRACKING_DATA_TYPE.TDT_SLOWER);
		UpdatePos();
		return true;
	}

	public void Select()
	{
		m_Animator.Play("START_ON", -1, 0f);
	}

	public void DeSelect()
	{
		m_Animator.Play("START_OFF", -1, 0f);
	}
}
