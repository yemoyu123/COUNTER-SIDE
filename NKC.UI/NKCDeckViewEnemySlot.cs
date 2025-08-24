using System;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCDeckViewEnemySlot : MonoBehaviour
{
	[NonSerialized]
	public int m_Index;

	public NKCUIComButton m_NKCUIComButton;

	public Image m_imgBGPanel;

	public Image m_imgBgAddPanel;

	public GameObject m_objUnitMain;

	public Image m_imgUnitPanel;

	public NKCUIComTextUnitLevel m_textLevel;

	public Image m_imgAttackType;

	public GameObject m_objWeakTag;

	public Image m_imgWeakMain;

	public Image m_imgWeakSub;

	[Header("Enemy")]
	public GameObject m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS;

	public Image m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS_img;

	private NKCAssetInstanceData m_instace;

	private const string DECK_SPRITE_BUNDLE_NAME = "ab_ui_unit_slot_deck_sprite";

	public static NKCDeckViewEnemySlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_UNIT_SLOT_DECK", "NKM_UI_DECK_VIEW_UNIT_SLOT_ENEMY");
		NKCDeckViewEnemySlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCDeckViewEnemySlot>();
		if (component == null)
		{
			Debug.LogError("NKCDeckViewUnitSlot Prefab null!");
			return null;
		}
		component.m_instace = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		return component;
	}

	public void Init(int index)
	{
		m_Index = index;
	}

	public void SetEnemyData(NKMUnitTempletBase cNKMUnitTempletBase, NKCEnemyData cNKMEnemyData)
	{
		if (cNKMEnemyData == null)
		{
			return;
		}
		Sprite backPanelImage = GetBackPanelImage(NKM_UNIT_GRADE.NUG_N);
		NKCUtil.SetImageSprite(m_imgBGPanel, backPanelImage);
		NKCUtil.SetImageSprite(m_imgBgAddPanel, backPanelImage);
		if (backPanelImage == null)
		{
			Debug.LogError("SetEnemyData m_spPanelN: null");
		}
		if (m_imgBGPanel.sprite == null)
		{
			Debug.LogError("SetEnemyData m_imgBGPanel.sprite: null");
		}
		if (cNKMUnitTempletBase != null)
		{
			Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, cNKMUnitTempletBase);
			m_imgUnitPanel.sprite = sprite;
			m_textLevel.SetText(cNKMEnemyData.m_Level.ToString(), 0);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS, cNKMEnemyData.m_NKM_BOSS_TYPE >= NKM_BOSS_TYPE.NBT_DUNGEON_BOSS);
			if (cNKMEnemyData.m_NKM_BOSS_TYPE == NKM_BOSS_TYPE.NBT_DUNGEON_BOSS)
			{
				m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS_img.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_POPUP_ENEMY_ICON");
			}
			else if (cNKMEnemyData.m_NKM_BOSS_TYPE == NKM_BOSS_TYPE.NBT_WARFARE_BOSS)
			{
				m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS_img.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_POPUP_ENEMY_BOSS_ICON");
			}
			Sprite orLoadUnitRoleAttackTypeIcon = NKCResourceUtility.GetOrLoadUnitRoleAttackTypeIcon(cNKMUnitTempletBase, bSmall: true);
			NKCUtil.SetImageSprite(m_imgAttackType, orLoadUnitRoleAttackTypeIcon, bDisableIfSpriteNull: true);
			NKCUtil.SetGameobjectActive(m_objWeakTag, NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE") && cNKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
			if (cNKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				NKCUtil.SetImageSprite(m_imgWeakMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(cNKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE), bDisableIfSpriteNull: true);
				NKCUtil.SetImageSprite(m_imgWeakSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(cNKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB), bDisableIfSpriteNull: true);
			}
			NKCUtil.SetGameobjectActive(m_objUnitMain, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgUnitPanel, bValue: true);
			NKCUtil.SetGameobjectActive(m_textLevel, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgBgAddPanel, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objUnitMain, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS, bValue: false);
		}
	}

	public void OverrideUnitSourceType(NKM_UNIT_SOURCE_TYPE sourceTypeMain, NKM_UNIT_SOURCE_TYPE sourceTypeSub)
	{
		NKCUtil.SetGameobjectActive(m_objWeakTag, NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE") && sourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
		if (m_objWeakTag != null && m_objWeakTag.activeSelf)
		{
			NKCUtil.SetImageSprite(m_imgWeakMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(sourceTypeMain), bDisableIfSpriteNull: true);
			NKCUtil.SetImageSprite(m_imgWeakSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(sourceTypeSub), bDisableIfSpriteNull: true);
		}
	}

	private Sprite GetBackPanelImage(NKM_UNIT_GRADE unitGrade)
	{
		return unitGrade switch
		{
			NKM_UNIT_GRADE.NUG_N => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_unit_slot_deck_sprite", "FACE_DECK_BG_N"), 
			NKM_UNIT_GRADE.NUG_R => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_unit_slot_deck_sprite", "FACE_DECK_BG_R"), 
			NKM_UNIT_GRADE.NUG_SR => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_unit_slot_deck_sprite", "FACE_DECK_BG_SR"), 
			NKM_UNIT_GRADE.NUG_SSR => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_unit_slot_deck_sprite", "FACE_DECK_BG_SSR"), 
			_ => null, 
		};
	}

	public void ButtonSelect()
	{
		m_NKCUIComButton.Select(bSelect: true);
	}

	public void ButtonDeSelect(bool bForce = false, bool bImmediate = false)
	{
		m_NKCUIComButton.Select(bSelect: false, bForce, bImmediate);
	}

	private void OnDestroy()
	{
		CloseInstance();
	}

	public void CloseInstance()
	{
		if (m_instace != null)
		{
			NKCAssetResourceManager.CloseInstance(m_instace);
			m_instace = null;
		}
	}
}
