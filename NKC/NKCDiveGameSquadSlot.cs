using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCDiveGameSquadSlot : MonoBehaviour
{
	public delegate void OnClickSquadSlot(NKMDiveSquad cNKMDiveSquad);

	public NKCUIComStateButton m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT;

	public GameObject m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_BATTLEPOINT_1;

	public GameObject m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_BATTLEPOINT_2;

	public Image m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_1;

	public Image m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_2;

	public Image m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_3;

	public NKCUIComTextUnitLevel m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_LV_TEXT;

	public Text m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_DECKNUMBER_COUNT;

	public Image m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_SHIP;

	public GameObject m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_DISABLE;

	public Text m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_DISABLE_TEXT;

	public GameObject m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_SELECT;

	private NKMDiveSquad m_NKMDiveSquad;

	private NKCAssetInstanceData m_InstanceData;

	private OnClickSquadSlot m_dOnClickSquadSlot;

	public int GetDeckIndex()
	{
		if (m_NKMDiveSquad != null)
		{
			return m_NKMDiveSquad.DeckIndex;
		}
		return -1;
	}

	public static NKCDiveGameSquadSlot GetNewInstance(Transform parent, OnClickSquadSlot dOnClickSquadSlot = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT");
		NKCDiveGameSquadSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCDiveGameSquadSlot>();
		if (component == null)
		{
			Debug.LogError("NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		component.m_dOnClickSquadSlot = dOnClickSquadSlot;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localScale = new Vector3(1f, 1f, 1f);
		component.m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT.PointerClick.RemoveAllListeners();
		component.m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT.PointerClick.AddListener(component.OnClick);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void SetSelected(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_SELECT, bSet);
	}

	private void OnDestroy()
	{
		if (m_InstanceData != null)
		{
			NKCAssetResourceManager.CloseInstance(m_InstanceData);
		}
		m_InstanceData = null;
	}

	private void OnClick()
	{
		if (m_dOnClickSquadSlot != null)
		{
			m_dOnClickSquadSlot(m_NKMDiveSquad);
		}
	}

	private float GetProperRatioValue(float fRatio)
	{
		if (fRatio < 0f)
		{
			fRatio = 0f;
		}
		if (fRatio > 1f)
		{
			fRatio = 1f;
		}
		return fRatio;
	}

	private void SetSuuplyCountUI(int count)
	{
		if (count <= 0)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_BATTLEPOINT_1, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_BATTLEPOINT_2, bValue: false);
		}
		if (count == 1)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_BATTLEPOINT_1, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_BATTLEPOINT_2, bValue: false);
		}
		if (count >= 2)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_BATTLEPOINT_1, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_BATTLEPOINT_2, bValue: true);
		}
	}

	public void SetUI(NKMDiveSquad cNKMDiveSquad)
	{
		if (cNKMDiveSquad == null)
		{
			return;
		}
		m_NKMDiveSquad = cNKMDiveSquad;
		SetSuuplyCountUI(cNKMDiveSquad.Supply);
		float fRatio = 0f;
		if (cNKMDiveSquad.MaxHp > 0f)
		{
			fRatio = cNKMDiveSquad.CurHp / cNKMDiveSquad.MaxHp;
		}
		fRatio = GetProperRatioValue(fRatio);
		if (fRatio > 0.6f)
		{
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_1.transform.localScale = new Vector3(1f, 1f, 1f);
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_2.transform.localScale = new Vector3(1f, 1f, 1f);
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_3.transform.localScale = new Vector3(GetProperRatioValue((fRatio - 0.6f) / 0.4f), 1f, 1f);
		}
		else if (fRatio > 0.3f)
		{
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_1.transform.localScale = new Vector3(1f, 1f, 1f);
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_2.transform.localScale = new Vector3(GetProperRatioValue((fRatio - 0.3f) / 0.3f), 1f, 1f);
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_3.transform.localScale = new Vector3(0f, 1f, 1f);
		}
		else
		{
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_1.transform.localScale = new Vector3(GetProperRatioValue(fRatio / 0.3f), 1f, 1f);
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_2.transform.localScale = new Vector3(0f, 1f, 1f);
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_HP_3.transform.localScale = new Vector3(0f, 1f, 1f);
		}
		NKMDeckIndex deckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, cNKMDiveSquad.DeckIndex);
		m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_DECKNUMBER_COUNT.text = (deckIndex.m_iIndex + 1).ToString();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_DISABLE, cNKMDiveSquad.CurHp <= 0f || cNKMDiveSquad.Supply <= 0);
		if (cNKMDiveSquad.CurHp <= 0f)
		{
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_DISABLE_TEXT.text = NKCUtilString.GET_STRING_DIVE_SQUAD_NO_EXIST_HP;
		}
		else if (cNKMDiveSquad.Supply <= 0)
		{
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_DISABLE_TEXT.text = NKCUtilString.GET_STRING_DIVE_SQUAD_NO_EXIST_SUPPLY;
		}
		NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(deckIndex);
		if (deckData == null)
		{
			return;
		}
		NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
		if (shipFromUID == null)
		{
			return;
		}
		if (m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_LV_TEXT != null)
		{
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_LV_TEXT.SetLevel(shipFromUID, 0);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
		if (unitTempletBase == null)
		{
			return;
		}
		Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
		if (sprite == null)
		{
			NKCAssetResourceData assetResourceUnitInvenIconEmpty = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
			if (assetResourceUnitInvenIconEmpty != null)
			{
				m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_SHIP.sprite = assetResourceUnitInvenIconEmpty.GetAsset<Sprite>();
			}
			else
			{
				m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_SHIP.sprite = null;
			}
		}
		else
		{
			m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SLOT_SHIP.sprite = sprite;
		}
	}
}
