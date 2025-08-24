using System;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIGameHudDevUnit : MonoBehaviour
{
	public delegate void _OnValueChanged(int unitID, bool bSet);

	[NonSerialized]
	public NKMUnitTempletBase m_NKMUnitTempletBase;

	public GameObject m_objMain;

	public NKCUIComToggle m_NKCUIComToggle;

	public Image m_imgBGPanel;

	public Image m_imgBgAddPanel;

	public GameObject m_objUnitMain;

	public Image m_imgUnitPanel;

	public Image m_imgUnitAddPanel;

	public Image m_imgUnitGrayPanel;

	public Text m_textCardCost;

	public GameObject m_goEnemy;

	[Header("Back Panel Image")]
	public Sprite m_spritePanelCounter;

	public Sprite m_spritePanelSoldier;

	public Sprite m_spritePanelMechanic;

	public Sprite m_spritePanelEmptySlot;

	private _OnValueChanged m_OnValueChanged;

	private LayoutElement m_LayoutElement;

	public static NKCUIGameHudDevUnit GetNewInstance(Transform parent, _OnValueChanged onValueChanged)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_HUD_RENEWAL", "NKM_GAME_DEV_MENU_WINDOW_UNIT");
		if (nKCAssetInstanceData.m_Instant == null)
		{
			Debug.LogError("NKCUIGameHudDevUnit Prefab null!");
			return null;
		}
		NKCUIGameHudDevUnit component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIGameHudDevUnit>();
		if (component == null)
		{
			Debug.LogError("NKCUIGameHudDevUnit Prefab null!");
			return null;
		}
		component.m_OnValueChanged = onValueChanged;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.m_NKCUIComToggle.SetToggleGroup(parent.gameObject.GetComponent<NKCUIComToggleGroup>());
		component.SetChangeEventToToggleBtn();
		component.m_LayoutElement = component.gameObject.GetComponent<LayoutElement>();
		return component;
	}

	public void SetLayoutElementActive(bool bSet)
	{
		m_LayoutElement.enabled = bSet;
	}

	public void SetOnValueChanged(_OnValueChanged onValueChanged)
	{
		m_OnValueChanged = onValueChanged;
	}

	public void OnValueChanged(bool bSet)
	{
		if (m_OnValueChanged != null)
		{
			if (m_NKMUnitTempletBase == null)
			{
				m_OnValueChanged(0, bSet);
			}
			else
			{
				m_OnValueChanged(m_NKMUnitTempletBase.m_UnitID, bSet);
			}
		}
	}

	public void SetChangeEventToToggleBtn()
	{
		m_NKCUIComToggle.OnValueChanged.RemoveAllListeners();
		m_NKCUIComToggle.OnValueChanged.AddListener(OnValueChanged);
	}

	public void SetRevToggleCallbackSeq()
	{
		m_NKCUIComToggle.SetbReverseSeqCallbackCall(bSet: true);
	}

	public void Preload(NKMUnitTempletBase cNKMUnitTempletBase)
	{
		NKCResourceUtility.PreloadUnitResource(NKCResourceUtility.eUnitResourceType.INVEN_ICON, cNKMUnitTempletBase);
	}

	private void SetEnemy(bool bSet)
	{
		if (m_goEnemy.activeSelf == !bSet)
		{
			m_goEnemy.SetActive(bSet);
		}
	}

	public void SetLayoutElement(bool bSet)
	{
	}

	public void SetData(NKMUnitData cNKMUnitData, bool bEnemy = false)
	{
		m_NKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData);
		if (cNKMUnitData != null && m_NKMUnitTempletBase != null)
		{
			SetEnemy(bEnemy);
			Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, cNKMUnitData);
			if (sprite != null)
			{
				m_imgUnitPanel.sprite = sprite;
				m_imgUnitAddPanel.sprite = sprite;
				m_imgUnitGrayPanel.sprite = sprite;
			}
			else
			{
				Debug.LogError($"INVEN_ICON Load Failed! unitID : {cNKMUnitData.m_UnitID}");
			}
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(cNKMUnitData.m_UnitID);
			if (unitStatTemplet != null)
			{
				m_textCardCost.text = unitStatTemplet.GetRespawnCost(bLeader: false, null, null).ToString();
			}
			else
			{
				Debug.LogError($"NKMUnitStatTemplet Load Failed! unitID : {cNKMUnitData.m_UnitID}");
			}
			m_objUnitMain.SetActive(value: true);
			m_imgUnitPanel.gameObject.SetActive(value: true);
			m_imgUnitGrayPanel.gameObject.SetActive(value: false);
			m_textCardCost.gameObject.SetActive(value: true);
			m_imgBgAddPanel.gameObject.SetActive(value: false);
			m_imgUnitAddPanel.gameObject.SetActive(value: false);
		}
		else
		{
			m_imgBGPanel.sprite = m_spritePanelEmptySlot;
			m_imgBgAddPanel.sprite = m_spritePanelEmptySlot;
			m_objUnitMain.SetActive(value: false);
			SetEnemy(bSet: false);
		}
	}
}
