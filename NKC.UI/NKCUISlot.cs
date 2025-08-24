using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Contract;
using ClientPacket.Office;
using DG.Tweening;
using NKC.UI.Contract;
using NKC.UI.Result;
using NKC.UI.Tooltip;
using NKM;
using NKM.Guild;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISlot : MonoBehaviour
{
	public delegate void OnClick(SlotData slotData, bool bLocked);

	public delegate void PointerDown(SlotData slotData, bool bLocked, PointerEventData eventData);

	public enum eSlotMode
	{
		Unit,
		ItemMisc,
		Equip,
		EquipCount,
		Skin,
		Mold,
		DiveArtifact,
		Buff,
		UnitCount,
		Emoticon,
		GuildArtifact,
		UnitReactor,
		Etc
	}

	public class SlotData
	{
		public eSlotMode eType;

		public int ID;

		public long Count;

		public long UID;

		public int Data;

		public int GroupID;

		public int BonusRate;

		public SlotData()
		{
		}

		public SlotData(SlotData slotData)
		{
			if (slotData != null)
			{
				eType = slotData.eType;
				ID = slotData.ID;
				Count = slotData.Count;
				UID = slotData.UID;
				Data = slotData.Data;
				GroupID = slotData.GroupID;
				BonusRate = slotData.BonusRate;
			}
		}

		public static List<SlotData> MakeUnitData(List<NKMUnitData> lstUnitData, bool bSort = true)
		{
			if (lstUnitData == null)
			{
				return new List<SlotData>();
			}
			if (bSort)
			{
				List<NKMUnitData> list = new List<NKMUnitData>(lstUnitData);
				list.Sort(RewardUnitSort);
				return list.Select((NKMUnitData x) => MakeUnitData(x)).ToList();
			}
			return lstUnitData.Select((NKMUnitData x) => MakeUnitData(x)).ToList();
		}

		public static SlotData MakeUnitData(int unitID, int level, int SkinID = 0, int GroupID = 0)
		{
			return new SlotData
			{
				eType = eSlotMode.Unit,
				ID = unitID,
				Count = level,
				Data = SkinID,
				GroupID = GroupID
			};
		}

		public static SlotData MakeUnitData(NKMUnitData unitData)
		{
			SlotData slotData = new SlotData();
			slotData.eType = eSlotMode.Unit;
			if (unitData != null)
			{
				slotData.ID = unitData.m_UnitID;
				slotData.Count = unitData.m_UnitLevel;
				slotData.UID = unitData.m_UnitUID;
				slotData.Data = unitData.m_SkinID;
			}
			else
			{
				slotData.ID = 0;
				slotData.Count = 1L;
				slotData.UID = 0L;
				slotData.Data = 0;
			}
			return slotData;
		}

		public static SlotData MakeUnitData(NKMOperator operatorData)
		{
			SlotData slotData = new SlotData();
			slotData.eType = eSlotMode.Unit;
			if (operatorData != null)
			{
				slotData.ID = operatorData.id;
				slotData.Count = operatorData.level;
				slotData.UID = operatorData.uid;
				slotData.Data = 0;
			}
			else
			{
				slotData.ID = 0;
				slotData.Count = 1L;
				slotData.UID = 0L;
				slotData.Data = 0;
			}
			return slotData;
		}

		public static SlotData MakeUnitCountData(int unitID, int count, int SkinID = 0, int GroupID = 0)
		{
			return new SlotData
			{
				eType = eSlotMode.UnitCount,
				ID = unitID,
				Count = count,
				Data = SkinID,
				GroupID = GroupID
			};
		}

		public static SlotData MakeMoldItemData(int itemID, long Count)
		{
			return new SlotData
			{
				eType = eSlotMode.Mold,
				ID = itemID,
				Count = Count
			};
		}

		public static SlotData MakeMoldItemData(NKMMoldItemData itemData)
		{
			SlotData slotData = new SlotData();
			slotData.eType = eSlotMode.Mold;
			if (itemData != null)
			{
				slotData.ID = itemData.m_MoldID;
				slotData.Count = itemData.m_Count;
			}
			else
			{
				slotData.ID = 0;
				slotData.Count = 0L;
			}
			return slotData;
		}

		public static SlotData MakeMiscItemData(int itemID, long Count, int BonusRate = 0)
		{
			return new SlotData
			{
				eType = eSlotMode.ItemMisc,
				ID = itemID,
				Count = Count,
				BonusRate = BonusRate
			};
		}

		public static SlotData MakeMiscItemData(NKMItemMiscData itemData, int BonusRate = 0)
		{
			SlotData slotData = new SlotData();
			slotData.eType = eSlotMode.ItemMisc;
			if (itemData != null)
			{
				slotData.ID = itemData.ItemID;
				slotData.Count = itemData.TotalCount;
				slotData.BonusRate = BonusRate;
			}
			else
			{
				slotData.ID = 0;
				slotData.Count = 0L;
				slotData.BonusRate = 0;
			}
			return slotData;
		}

		public static SlotData MakeEquipData(int itemID, int Level, int setOptionID = 0)
		{
			return new SlotData
			{
				eType = eSlotMode.EquipCount,
				ID = itemID,
				Count = Level,
				GroupID = setOptionID
			};
		}

		public static SlotData MakeEquipData(NKMEquipItemData equipData)
		{
			SlotData slotData = new SlotData();
			slotData.eType = eSlotMode.Equip;
			if (equipData != null)
			{
				slotData.ID = equipData.m_ItemEquipID;
				slotData.Count = equipData.m_EnchantLevel;
				slotData.UID = equipData.m_ItemUid;
				slotData.GroupID = equipData.m_SetOptionId;
			}
			else
			{
				slotData.ID = 0;
				slotData.Count = 1L;
				slotData.UID = 0L;
				slotData.GroupID = 0;
			}
			return slotData;
		}

		public static SlotData MakePostItemData(NKMRewardInfo postItem)
		{
			return MakeRewardTypeData(postItem.rewardType, postItem.ID, postItem.Count);
		}

		public static SlotData MakeBuffItemData(int itemID, int Count)
		{
			return new SlotData
			{
				eType = eSlotMode.Buff,
				ID = itemID,
				Count = Count
			};
		}

		public static SlotData MakeShopItemData(ShopItemTemplet shopTemplet, bool bFirstBuy)
		{
			if (shopTemplet == null)
			{
				Debug.LogError("ShopTemplet Null!");
				return MakeMiscItemData(1, 0L);
			}
			if (bFirstBuy && shopTemplet.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.FIRST_PURCHASE_CHANGE_REWARD_VALUE)
			{
				return MakeRewardTypeData(shopTemplet.m_ItemType, shopTemplet.m_ItemID, shopTemplet.m_PurchaseEventValue);
			}
			return MakeRewardTypeData(shopTemplet.m_ItemType, shopTemplet.m_ItemID, shopTemplet.TotalValue);
		}

		public static SlotData MakeShopItemData(NKMShopRandomListData shopRandomTemplet)
		{
			if (shopRandomTemplet == null)
			{
				Debug.LogError("NKMShopRandomListData Null!");
				return MakeMiscItemData(1, 0L);
			}
			return MakeRewardTypeData(shopRandomTemplet.itemType, shopRandomTemplet.itemId, shopRandomTemplet.itemCount);
		}

		public static SlotData MakeSkinData(int skinID)
		{
			return new SlotData
			{
				eType = eSlotMode.Skin,
				ID = skinID
			};
		}

		public static SlotData MakeRewardTypeData(NKMRewardInfo info, int bonusRate = 0)
		{
			if (info == null)
			{
				return null;
			}
			return MakeRewardTypeData(info.rewardType, info.ID, info.Count, bonusRate);
		}

		public static SlotData MakeDiveArtifactData(int id, int count = 1)
		{
			return new SlotData
			{
				eType = eSlotMode.DiveArtifact,
				ID = id,
				Count = count
			};
		}

		public static SlotData MakeEmoticonData(int id, int count = 1)
		{
			return new SlotData
			{
				eType = eSlotMode.Emoticon,
				ID = id,
				Count = count
			};
		}

		public static SlotData MakeGuildArtifactData(int id, int count = 1)
		{
			return new SlotData
			{
				eType = eSlotMode.GuildArtifact,
				ID = id,
				Count = count
			};
		}

		public static SlotData MakeRewardTypeData(NKM_REWARD_TYPE type, int ID, int count, int bonusRate = 0)
		{
			switch (type)
			{
			case NKM_REWARD_TYPE.RT_EQUIP:
				return MakeEquipData(ID, count);
			case NKM_REWARD_TYPE.RT_MISC:
			case NKM_REWARD_TYPE.RT_MISSION_POINT:
			case NKM_REWARD_TYPE.RT_PASS_EXP:
				return MakeMiscItemData(ID, count, bonusRate);
			case NKM_REWARD_TYPE.RT_UNIT:
			case NKM_REWARD_TYPE.RT_SHIP:
			case NKM_REWARD_TYPE.RT_OPERATOR:
				return MakeUnitCountData(ID, count);
			case NKM_REWARD_TYPE.RT_USER_EXP:
				return MakeMiscItemData(501, count, bonusRate);
			case NKM_REWARD_TYPE.RT_SKIN:
				return MakeSkinData(ID);
			case NKM_REWARD_TYPE.RT_MOLD:
				return MakeMoldItemData(ID, count);
			case NKM_REWARD_TYPE.RT_BUFF:
				return MakeBuffItemData(ID, count);
			case NKM_REWARD_TYPE.RT_EMOTICON:
				return MakeEmoticonData(ID, count);
			default:
				Debug.LogError("Undefined type");
				return MakeMiscItemData(1, 0L);
			}
		}
	}

	public enum SlotClickType
	{
		Tooltip,
		ItemBox,
		RatioList,
		BoxList,
		MoldList,
		ChoiceList,
		None
	}

	[Header("부모 오브젝트")]
	public GameObject m_objParent;

	[Header("Common")]
	public Image m_imgIcon;

	public Text m_lbName;

	public Image m_imgUpperRightIcon;

	public LayoutElement m_layoutElement;

	public NKCUIComButton m_cbtnButton;

	public GameObject m_objItemCount;

	public Text m_lbItemCount;

	public Text m_lbItemAddCount;

	public Text m_lbAdditionalText;

	[Header("별")]
	public GameObject m_objStarRoot;

	public List<GameObject> m_lstStar;

	[Header("Background images")]
	public Image m_imgBG;

	public Sprite m_spLocked;

	public Sprite m_spEmpty;

	public Sprite m_sp_MatAdd;

	public Sprite m_spBGRarityN;

	public Sprite m_spBGRarityR;

	public Sprite m_spBGRaritySR;

	public Sprite m_spBGRaritySSR;

	public Sprite m_spBGRarityEPIC;

	public Sprite m_spBGRarityReactor;

	[FormerlySerializedAs("m_spBGInteriorRarityN")]
	public Sprite m_spBGWhiteRarityN;

	[FormerlySerializedAs("m_spBGInteriorRarityR")]
	public Sprite m_spBGWhiteRarityR;

	[FormerlySerializedAs("m_spBGInteriorRaritySR")]
	public Sprite m_spBGWhiteRaritySR;

	[FormerlySerializedAs("m_spBGInteriorRaritySSR")]
	public Sprite m_spBGWhiteRaritySSR;

	public Sprite m_spEmblem;

	public Sprite m_spBGLock;

	public Sprite m_spBGLockReactor;

	public Sprite m_spBGLockNoReactorUnit;

	[Header("Extra")]
	public GameObject m_objCompleteMark;

	public GameObject m_objFirstClear;

	public Image m_imgFirstClearBG;

	public Text m_lbFirstClear;

	public GameObject m_objSelected;

	public GameObject m_objTier;

	public Text m_lbTier;

	public GameObject m_AB_ICON_SLOT_DISABLE;

	public Text m_lbDisable;

	public GameObject m_AB_ICON_SLOT_CLEARED;

	public GameObject m_AB_ICON_SLOT_DENIED;

	public GameObject m_AB_ICON_SLOT_INDUCE_ARROW;

	public GameObject m_AB_ICON_SLOT_INDUCE_ARROW_RED;

	public GameObject m_objItemDetail;

	public GameObject m_objSeized;

	public GameObject m_objRewardFx;

	public GameObject m_objEventGet;

	public GameObject m_objTopNotice;

	public GameObject m_objTimeInterval;

	public Text m_lbTopNoticeText;

	[Header("보유 갯수 표시")]
	public GameObject m_objHaveCount;

	public Text m_lbHaveCount;

	[Header("유닛 각성 애니")]
	public Animator m_animAwakenFX;

	[Header("세트 장비 아이콘")]
	public Image m_AB_ICON_SLOT_EQUIP_SET_ICON;

	[Header("격전지원 착용 표시")]
	public GameObject m_EQUIP_FIERCE_BATTLE;

	[Header("잠재 개방")]
	public GameObject m_objRelic;

	public List<Image> m_lstImgRelic;

	[Header("티어7 표시")]
	public GameObject m_objTier_7;

	[Header("전술 업데이트 MAX")]
	public GameObject m_objTacticMAX;

	[Header("얼터 리액터 FX")]
	public GameObject m_objReactorFX;

	[Header("오퍼레이터 토큰 화살표")]
	public GameObject m_objOperToken;

	public GameObject m_objOperTokenArrowEnhance;

	public GameObject m_objOperTokenArrowImplant;

	private const int MAX_ITEM_NAME_COUNT = 7;

	private SlotData m_SlotData;

	private OnClick dOnClick;

	private PointerDown dOnPointerDown;

	private bool m_bEmpty = true;

	private bool m_bCustomizedEmptySP;

	private Sprite m_spCustomizedEmpty;

	private NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE m_EQUIP_BOX_BOTTOM_MENU_TYPE = NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_ENFORCE_AND_EQUIP;

	private bool m_bUseBigImg;

	private NKCAssetInstanceData m_NKCAssetInstanceData;

	private bool m_bFirstReward;

	private bool m_bOneTimeReward;

	private bool m_bFirstAllClear;

	public bool IsLocked { get; private set; }

	public void Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE type)
	{
		m_EQUIP_BOX_BOTTOM_MENU_TYPE = type;
	}

	public void SetUseBigImg(bool bSet)
	{
		m_bUseBigImg = bSet;
	}

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceData);
	}

	public int GetCount()
	{
		return int.Parse(m_lbItemCount.text);
	}

	public void SetActiveCount(bool value)
	{
		NKCUtil.SetGameobjectActive(m_lbItemCount, value);
		NKCUtil.SetGameobjectActive(m_objItemCount, value);
	}

	public void SetNewCountAni(long newCount, float fDuration = 0.4f)
	{
		m_lbItemCount.DOText(newCount.ToString(), fDuration, richTextEnabled: true, ScrambleMode.Numerals);
	}

	public void SetBGVisible(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_imgBG, bSet);
	}

	public void SetAlphaColorDisableImg(float fVal)
	{
		Image component = m_AB_ICON_SLOT_DISABLE.GetComponent<Image>();
		if (!(null == component))
		{
			Color color = component.color;
			color.a = fVal;
			component.color = color;
		}
	}

	public void SetBonusRate(int bonusRate)
	{
		NKCUtil.SetGameobjectActive(m_lbItemAddCount, bonusRate > 0);
		if (bonusRate > 0)
		{
			NKCUtil.SetLabelText(m_lbItemAddCount, $"(+{bonusRate}%)");
		}
	}

	public static NKCUISlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_INVEN_ICON", "AB_ICON_SLOT");
		NKCUISlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUISlot>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUISlot Prefab null!");
			return null;
		}
		component.m_NKCAssetInstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.gameObject.SetActive(value: false);
		component.Init();
		return component;
	}

	public void Init()
	{
		m_cbtnButton.PointerClick.RemoveAllListeners();
		m_cbtnButton.PointerClick.AddListener(OnPress);
		m_cbtnButton.PointerDown.RemoveAllListeners();
		m_cbtnButton.PointerDown.AddListener(OnPointerDown);
		NKCUtil.SetGameobjectActive(m_lbItemAddCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCompleteMark, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFirstClear, bValue: false);
		NKCUtil.SetGameobjectActive(m_objHaveCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTopNotice, bValue: false);
		SetUsable(usable: false);
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_EQUIP_SET_ICON.gameObject, bValue: false);
		m_bEmpty = true;
	}

	public SlotData GetSlotData()
	{
		return m_SlotData;
	}

	public void CleanUp()
	{
		dOnClick = null;
		m_SlotData = null;
		m_bEmpty = true;
		dOnPointerDown = null;
	}

	public virtual void TurnOffExtraUI()
	{
		NKCUtil.SetGameobjectActive(m_objCompleteMark, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFirstClear, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbItemAddCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbAdditionalText, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTier, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgUpperRightIcon, bValue: false);
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_DISABLE, bValue: false);
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_CLEARED, bValue: false);
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_DENIED, bValue: false);
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_INDUCE_ARROW, bValue: false);
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_INDUCE_ARROW_RED, bValue: false);
		NKCUtil.SetGameobjectActive(m_objItemDetail, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSeized, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRewardFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEventGet, bValue: false);
		NKCUtil.SetGameobjectActive(m_objHaveCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTopNotice, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_EQUIP_SET_ICON.gameObject, bValue: false);
		NKCUtil.SetAwakenFX(m_animAwakenFX, null);
		NKCUtil.SetGameobjectActive(m_objTier_7, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRelic, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelected, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTimeInterval, bValue: false);
		NKCUtil.SetGameobjectActive(m_objReactorFX, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTacticMAX, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOperToken, bValue: false);
	}

	public static List<SlotData> MakeSlotDataListFromReward(NKMRewardData rewardData, bool bIncludeContractList = false, bool bStackEnchantType = false)
	{
		List<SlotData> list = new List<SlotData>();
		if (rewardData == null)
		{
			return list;
		}
		if (rewardData.UserExp > 0)
		{
			SlotData item = SlotData.MakeMiscItemData(501, rewardData.UserExp, rewardData.BonusRatioOfUserExp);
			list.Add(item);
		}
		List<NKMUnitData> list2 = new List<NKMUnitData>(rewardData.UnitDataList);
		List<NKMOperator> list3 = new List<NKMOperator>(rewardData.OperatorList);
		if (bIncludeContractList && rewardData.ContractList != null)
		{
			foreach (MiscContractResult contract in rewardData.ContractList)
			{
				if (contract != null)
				{
					list2.AddRange(contract.units);
				}
			}
		}
		list2.Sort(RewardUnitSort);
		foreach (NKMUnitData item13 in list2)
		{
			SlotData item2 = SlotData.MakeUnitData(item13);
			list.Add(item2);
		}
		list3.Sort(RewardOperatorSort);
		foreach (NKMOperator item14 in list3)
		{
			SlotData item3 = SlotData.MakeUnitData(item14);
			list.Add(item3);
		}
		List<NKMEquipItemData> list4 = new List<NKMEquipItemData>(rewardData.EquipItemDataList);
		list4.Sort(RewardEquipSort);
		Dictionary<int, List<NKMEquipItemData>> dictionary = new Dictionary<int, List<NKMEquipItemData>>();
		foreach (NKMEquipItemData item15 in list4)
		{
			if (bStackEnchantType)
			{
				NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(item15.m_ItemEquipID);
				if (equipTemplet != null && equipTemplet.m_EquipUnitStyleType == NKM_UNIT_STYLE_TYPE.NUST_ENCHANT)
				{
					if (!dictionary.ContainsKey(item15.m_ItemEquipID))
					{
						dictionary.Add(item15.m_ItemEquipID, new List<NKMEquipItemData>());
					}
					dictionary[item15.m_ItemEquipID].Add(item15);
					continue;
				}
			}
			SlotData item4 = SlotData.MakeEquipData(item15);
			list.Add(item4);
		}
		foreach (KeyValuePair<int, List<NKMEquipItemData>> item16 in dictionary)
		{
			int count = item16.Value.Count;
			if (count > 0)
			{
				SlotData slotData = SlotData.MakeEquipData(item16.Value[0]);
				slotData.eType = eSlotMode.EquipCount;
				slotData.Count = count;
				list.Add(slotData);
			}
		}
		if (rewardData.DailyMissionPoint > 0)
		{
			SlotData item5 = SlotData.MakeMiscItemData(203, rewardData.DailyMissionPoint);
			list.Add(item5);
		}
		if (rewardData.WeeklyMissionPoint > 0)
		{
			SlotData item6 = SlotData.MakeMiscItemData(204, rewardData.WeeklyMissionPoint);
			list.Add(item6);
		}
		if (rewardData.AchievePoint > 0)
		{
			SlotData item7 = SlotData.MakeMiscItemData(202, rewardData.AchievePoint);
			list.Add(item7);
		}
		if (rewardData.SkinIdList != null)
		{
			foreach (int skinId in rewardData.SkinIdList)
			{
				SlotData item8 = SlotData.MakeSkinData(skinId);
				list.Add(item8);
			}
		}
		List<NKMMoldItemData> list5 = new List<NKMMoldItemData>(rewardData.MoldItemDataList);
		list5.Sort(RewardMoldSort);
		foreach (NKMMoldItemData item17 in list5)
		{
			SlotData item9 = SlotData.MakeMoldItemData(item17);
			list.Add(item9);
		}
		Dictionary<int, long> dictionary2 = new Dictionary<int, long>();
		foreach (NKMItemMiscData miscItemData in rewardData.MiscItemDataList)
		{
			if (miscItemData.ItemID != 0 && miscItemData.TotalCount > 0)
			{
				if (dictionary2.ContainsKey(miscItemData.ItemID))
				{
					dictionary2[miscItemData.ItemID] = dictionary2[miscItemData.ItemID] + miscItemData.TotalCount;
				}
				else
				{
					dictionary2[miscItemData.ItemID] = miscItemData.TotalCount;
				}
			}
			else
			{
				Debug.LogError("ItemID 0 or count 0 itemdata. any error?, ItemMiscID : " + miscItemData.ItemID + ", count : " + miscItemData.TotalCount);
			}
		}
		if (rewardData.EmoticonList != null)
		{
			foreach (int emoticon in rewardData.EmoticonList)
			{
				SlotData item10 = SlotData.MakeEmoticonData(emoticon);
				list.Add(item10);
			}
		}
		if (rewardData.Interiors != null)
		{
			foreach (NKMInteriorData interior in rewardData.Interiors)
			{
				SlotData item11 = SlotData.MakeMiscItemData(interior.itemId, interior.count);
				list.Add(item11);
			}
		}
		foreach (KeyValuePair<int, long> kvPair in dictionary2)
		{
			int bonusRate = 0;
			if (rewardData.MiscItemDataList.Find((NKMItemMiscData x) => x.ItemID == kvPair.Key) != null)
			{
				bonusRate = rewardData.MiscItemDataList.Find((NKMItemMiscData x) => x.ItemID == kvPair.Key).BonusRatio;
			}
			SlotData item12 = SlotData.MakeMiscItemData(kvPair.Key, kvPair.Value, bonusRate);
			list.Add(item12);
		}
		return list;
	}

	public static List<SlotData> MakeSlotDataListFromReward(NKMAdditionalReward reward)
	{
		List<SlotData> list = new List<SlotData>();
		if (reward != null)
		{
			if (reward.guildExpDelta > 0)
			{
				SlotData item = SlotData.MakeMiscItemData(503, reward.guildExpDelta);
				list.Add(item);
			}
			if (reward.unionPointDelta > 0)
			{
				SlotData item2 = SlotData.MakeMiscItemData(24, reward.unionPointDelta);
				list.Add(item2);
			}
			if (reward.eventPassExpDelta > 0)
			{
				SlotData item3 = SlotData.MakeMiscItemData(504, reward.eventPassExpDelta);
				list.Add(item3);
			}
		}
		return list;
	}

	public static int RewardEquipSort(NKMEquipItemData A, NKMEquipItemData B)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(A.m_ItemEquipID);
		NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(B.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return 1;
		}
		if (equipTemplet2 == null)
		{
			return -1;
		}
		if (equipTemplet.m_NKM_ITEM_GRADE == equipTemplet2.m_NKM_ITEM_GRADE)
		{
			return A.m_ItemEquipID.CompareTo(B.m_ItemEquipID);
		}
		return equipTemplet2.m_NKM_ITEM_GRADE.CompareTo(equipTemplet.m_NKM_ITEM_GRADE);
	}

	public static int RewardUnitSort(NKMUnitData A, NKMUnitData B)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(A.m_UnitID);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(B.m_UnitID);
		if (unitTempletBase.m_bAwaken != unitTempletBase2.m_bAwaken)
		{
			return unitTempletBase2.m_bAwaken.CompareTo(unitTempletBase.m_bAwaken);
		}
		if (unitTempletBase.IsRearmUnit != unitTempletBase2.IsRearmUnit)
		{
			return unitTempletBase2.IsRearmUnit.CompareTo(unitTempletBase.IsRearmUnit);
		}
		if (unitTempletBase.m_NKM_UNIT_GRADE != unitTempletBase2.m_NKM_UNIT_GRADE)
		{
			return unitTempletBase2.m_NKM_UNIT_GRADE.CompareTo(unitTempletBase.m_NKM_UNIT_GRADE);
		}
		return A.m_UnitID.CompareTo(B.m_UnitID);
	}

	public static int RewardOperatorSort(NKMOperator A, NKMOperator B)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(A.id);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(B.id);
		if (unitTempletBase.m_NKM_UNIT_GRADE != unitTempletBase2.m_NKM_UNIT_GRADE)
		{
			return unitTempletBase2.m_NKM_UNIT_GRADE.CompareTo(unitTempletBase.m_NKM_UNIT_GRADE);
		}
		return A.id.CompareTo(B.id);
	}

	public static int RewardMoldSort(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID == null)
		{
			return 1;
		}
		if (itemMoldTempletByID2 == null)
		{
			return -1;
		}
		if (itemMoldTempletByID.m_Grade != itemMoldTempletByID2.m_Grade)
		{
			return itemMoldTempletByID2.m_Grade.CompareTo(itemMoldTempletByID.m_Grade);
		}
		return itemMoldTempletByID.m_MoldID.CompareTo(itemMoldTempletByID2.m_MoldID);
	}

	public static void SetSlotListData(List<NKCUISlot> lstSlot, List<SlotData> lstSlotData, bool bShowName, bool bShowNumber, bool bEnableLayoutElement, OnClick onClick, params SlotClickType[] clickTypes)
	{
		for (int i = 0; i < lstSlot.Count; i++)
		{
			if (i < lstSlotData.Count)
			{
				NKCUtil.SetGameobjectActive(lstSlot[i], bValue: true);
				lstSlot[i].SetData(lstSlotData[i], bShowName, bShowNumber, bEnableLayoutElement, onClick);
				if (clickTypes != null && clickTypes.Length != 0)
				{
					lstSlot[i].SetOnClickAction(clickTypes);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(lstSlot[i], bValue: false);
			}
		}
	}

	public void SetData(SlotData data, bool bEnableLayoutElement = true, OnClick onClick = null)
	{
		bool bShowNumber = WillShowCount(data);
		if (onClick == null)
		{
			SetData(data, bShowName: false, bShowNumber, bEnableLayoutElement, OpenItemBox);
		}
		else
		{
			SetData(data, bShowName: false, bShowNumber, bEnableLayoutElement, onClick);
		}
	}

	public static bool WillShowCount(SlotData data)
	{
		switch (data.eType)
		{
		case eSlotMode.ItemMisc:
		case eSlotMode.EquipCount:
		case eSlotMode.UnitCount:
		case eSlotMode.UnitReactor:
			return true;
		case eSlotMode.Mold:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(data.ID);
			if (itemMoldTempletByID != null)
			{
				return !itemMoldTempletByID.m_bPermanent;
			}
			break;
		}
		}
		return false;
	}

	public void SetData(SlotData data, bool bShowName, bool bShowNumber, bool bEnableLayoutElement, OnClick onClick)
	{
		switch (data.eType)
		{
		case eSlotMode.Unit:
			SetUnitData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.UnitCount:
			SetUnitCountData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.ItemMisc:
			SetMiscItemData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.Equip:
			SetEquipData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.EquipCount:
			SetEquipCountData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.Skin:
			SetSkinData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.Mold:
			SetMoldData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.DiveArtifact:
			SetDiveArtifactData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.Emoticon:
			SetEmoticonData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.Buff:
			SetBuffData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.GuildArtifact:
			SetGuildArtifactData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		case eSlotMode.UnitReactor:
			SetReactorData(data, bShowName, bShowNumber, bEnableLayoutElement, onClick);
			break;
		}
		m_bEmpty = false;
	}

	public void SetCountRange(long min, long max)
	{
		SetActiveCount(value: true);
		if (min == max)
		{
			NKCUtil.SetLabelText(m_lbItemCount, $"{max}");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbItemCount, $"{min}-{max}");
		}
	}

	public void SetHaveCount(long count, bool bInfinite = false)
	{
		if (bInfinite)
		{
			NKCUtil.SetLabelText(m_lbHaveCount, NKCStringTable.GetString("SI_DP_ICON_SLOT_HAVE", NKCUtilString.GET_STRING_FORGE_CRAFT_COUNT_INFINITE_SYMBOL));
			NKCUtil.SetGameobjectActive(m_objHaveCount, bValue: true);
			return;
		}
		if (count <= 0)
		{
			NKCUtil.SetGameobjectActive(m_objHaveCount, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objHaveCount, bValue: true);
		if (count > 999)
		{
			NKCUtil.SetLabelText(m_lbHaveCount, NKCStringTable.GetString("SI_DP_ICON_SLOT_HAVE", "999+"));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbHaveCount, NKCStringTable.GetString("SI_DP_ICON_SLOT_HAVE", count));
		}
	}

	public void SetHaveCountString(bool bShow, string str)
	{
		NKCUtil.SetGameobjectActive(m_objHaveCount, bShow);
		if (bShow)
		{
			NKCUtil.SetLabelText(m_lbHaveCount, str);
		}
	}

	public void SetSlotItemCount(long count)
	{
		NKCUtil.SetLabelText(m_lbItemCount, count.ToString("N0"));
	}

	public void SetSlotItemCountString(bool bShow, string str)
	{
		SetActiveCount(bShow);
		if (bShow)
		{
			NKCUtil.SetLabelText(m_lbItemCount, str);
		}
	}

	public void AddProbabilityToName(float probability)
	{
		string text = GetName();
		text += $"\n{probability:0.000%}";
		m_lbName.verticalOverflow = VerticalWrapMode.Overflow;
		NKCUtil.SetLabelText(m_lbName, text);
	}

	public void OverrideName(string name, bool supportRichText, bool forceShow = false)
	{
		NKCUtil.SetLabelText(m_lbName, name);
		m_lbName.supportRichText = supportRichText;
		if (forceShow)
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: true);
		}
	}

	private void SetName(string text, bool bShowFullname = true)
	{
		if (bShowFullname || text.Length <= 7)
		{
			NKCUtil.SetLabelText(m_lbName, text);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbName, text.Substring(0, 7) + "...");
		}
	}

	public string GetName()
	{
		if (m_lbName != null)
		{
			return m_lbName.text;
		}
		return "";
	}

	public bool IsEmpty()
	{
		return m_bEmpty;
	}

	public void SetEmpty(OnClick _dOnClick = null)
	{
		m_bEmpty = true;
		dOnClick = _dOnClick;
		dOnPointerDown = null;
		m_SlotData = null;
		TurnOffExtraUI();
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbName, bValue: false);
		SetActiveCount(value: false);
		NKCUtil.SetGameobjectActive(m_lbItemAddCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_EQUIP_SET_ICON.gameObject, bValue: false);
		SetEmptyBackground();
	}

	private void SetEmptyBackground()
	{
		if (m_bCustomizedEmptySP)
		{
			m_imgBG.sprite = m_spCustomizedEmpty;
		}
		else
		{
			m_imgBG.sprite = m_spEmpty;
		}
	}

	public void SetLock(OnClick _dOnClick = null)
	{
		IsLocked = true;
		CleanUp();
		dOnClick = _dOnClick;
		TurnOffExtraUI();
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbName, bValue: false);
		SetActiveCount(value: false);
		NKCUtil.SetGameobjectActive(m_lbItemAddCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_EQUIP_SET_ICON.gameObject, bValue: false);
		NKCUtil.SetImageSprite(m_imgBG, m_spBGLock);
	}

	public void SetLockReactorNotPossible(OnClick _dOnClick = null)
	{
		SetLock(_dOnClick);
		NKCUtil.SetImageSprite(m_imgBG, m_spBGLockReactor);
	}

	public void SetLockReactorNotHasReactor(OnClick _dOnClick = null)
	{
		SetLock(_dOnClick);
		NKCUtil.SetImageSprite(m_imgBG, m_spBGLockNoReactorUnit);
	}

	public void SetEmptyMaterial(OnClick _dOnClick = null)
	{
		m_bEmpty = true;
		dOnClick = _dOnClick;
		m_SlotData = null;
		TurnOffExtraUI();
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbName, bValue: false);
		SetActiveCount(value: false);
		NKCUtil.SetGameobjectActive(m_lbItemAddCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		if (m_sp_MatAdd != null)
		{
			m_imgBG.sprite = m_sp_MatAdd;
		}
		else
		{
			m_imgBG.sprite = m_spEmpty;
		}
	}

	public void SetCustomizedEmptySP(Sprite sp)
	{
		m_bCustomizedEmptySP = true;
		m_spCustomizedEmpty = sp;
	}

	public void SetCompleteMark(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objCompleteMark, bValue);
	}

	public void SetFirstGetMark(bool bValue)
	{
		if (bValue || m_bFirstReward)
		{
			m_bFirstReward = bValue;
			SetShowArrowBGText(bValue);
			if (bValue)
			{
				SetArrowBGText(NKCUtilString.GET_STRING_REWARD_FIRST_CLEAR, NKCUtil.GetColor("#4E4F52"));
			}
		}
	}

	public void SetMainRewardMark(bool bValue)
	{
		SetShowArrowBGText(bValue);
		if (bValue)
		{
			SetArrowBGText(NKCUtilString.GET_STRING_POPUP_DUNGEON_GET_MAIN_REWARD, NKCUtil.GetColor("#8A0D00"));
		}
		SetActiveCount(value: false);
	}

	public void SetUsedMark(bool bVal)
	{
		NKCUtil.SetGameobjectActive(m_EQUIP_FIERCE_BATTLE, bVal);
	}

	public void DisableItemCount()
	{
		SetActiveCount(value: false);
	}

	public void SetFirstAllClearMark(bool bValue)
	{
		if (bValue || m_bFirstAllClear)
		{
			m_bFirstAllClear = bValue;
			SetShowArrowBGText(bValue);
			if (bValue)
			{
				SetArrowBGText(NKCUtilString.GET_STRING_WARFARE_FIRST_ALL_CLEAR, NKCUtil.GetColor("#4E4F52"));
			}
		}
	}

	public void SetOnetimeMark(bool bValue)
	{
		if (bValue || m_bOneTimeReward)
		{
			m_bOneTimeReward = bValue;
			SetShowArrowBGText(bValue);
			if (bValue)
			{
				SetArrowBGText(NKCUtilString.GET_STRING_REWARD_CHANCE_UP, NKCUtil.GetColor("#4E4F52"));
			}
		}
	}

	public void SetEventDropMark(bool bValue)
	{
		SetShowArrowBGText(bValue);
		if (bValue)
		{
			SetArrowBGText(NKCStringTable.GetString("SI_PF_EVENT_BADGE_TEXT_EVENT"), NKCUtil.GetColor("#8600DB"));
		}
	}

	public void SetBuffDropMark(bool bValue)
	{
		SetShowArrowBGText(bValue);
		if (bValue)
		{
			SetArrowBGText(NKCStringTable.GetString("SI_PF_STAGE_ADD_TAG_TEXT"), NKCUtil.GetColor("#FF13A5"));
		}
	}

	public void SetShowArrowBGText(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objFirstClear, bSet);
	}

	public void SetArrowBGText(string desc, Color bgColor)
	{
		if (m_imgFirstClearBG != null)
		{
			m_imgFirstClearBG.color = bgColor;
		}
		NKCUtil.SetLabelText(m_lbFirstClear, desc);
	}

	public void SetDiveArtifactData(SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		SetDiveArtifactData(null, slotData, bShowName, bShowCount, bEnableLayoutElement, onClick);
	}

	public void SetDiveArtifactData(NKMUserData userData, SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		int iD = slotData.ID;
		long count = slotData.Count;
		m_bEmpty = false;
		TurnOffExtraUI();
		dOnClick = onClick;
		m_SlotData = slotData;
		m_layoutElement.enabled = bEnableLayoutElement;
		NKMDiveArtifactTemplet nKMDiveArtifactTemplet = NKMDiveArtifactTemplet.Find(iD);
		if (nKMDiveArtifactTemplet == null)
		{
			SetEmpty();
			return;
		}
		if (bShowName)
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: true);
			SetName(nKMDiveArtifactTemplet.ArtifactName_Translated);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: false);
		}
		if (userData != null)
		{
			int num = 0;
			NKMDiveGameData diveGameData = userData.m_DiveGameData;
			if (diveGameData != null)
			{
				List<int> artifacts = diveGameData.Player.PlayerBase.Artifacts;
				for (int i = 0; i < artifacts.Count; i++)
				{
					if (artifacts[i] == iD)
					{
						num = 1;
						break;
					}
				}
			}
			SetActiveCount(value: true);
			if (count <= num)
			{
				NKCUtil.SetLabelText(m_lbItemCount, $"{count}/{num}");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbItemCount, $"{count}/<color=#ff0000ff>{num}</color>");
			}
		}
		else if (bShowCount && count > 1)
		{
			SetActiveCount(value: true);
			NKCUtil.SetLabelText(m_lbItemCount, count.ToString());
		}
		else
		{
			SetActiveCount(value: false);
		}
		NKCUtil.SetGameobjectActive(m_imgBG, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		m_imgIcon.color = new Color(1f, 1f, 1f, 1f);
		Sprite sprite = null;
		sprite = ((!m_bUseBigImg) ? NKCResourceUtility.GetOrLoadDiveArtifactIcon(nKMDiveArtifactTemplet) : NKCResourceUtility.GetOrLoadDiveArtifactIconBig(nKMDiveArtifactTemplet));
		m_imgIcon.sprite = sprite;
		if (sprite == null)
		{
			Debug.LogError("iconSprite not found. artifact ID : " + iD);
		}
		NKCUtil.SetGameobjectActive(m_objTier, bValue: false);
	}

	public void SetEmoticonData(SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		SetEmoticonData(null, slotData, bShowName, bShowCount, bEnableLayoutElement, onClick);
	}

	public void SetEmoticonData(NKMUserData userData, SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		int iD = slotData.ID;
		long count = slotData.Count;
		m_bEmpty = false;
		TurnOffExtraUI();
		dOnClick = onClick;
		m_SlotData = slotData;
		m_layoutElement.enabled = bEnableLayoutElement;
		NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(iD);
		if (nKMEmoticonTemplet == null)
		{
			SetEmpty();
			return;
		}
		if (bShowName)
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: true);
			SetName(nKMEmoticonTemplet.GetEmoticonName());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: false);
		}
		if (userData != null)
		{
			int num = 0;
			NKMDiveGameData diveGameData = userData.m_DiveGameData;
			if (diveGameData != null)
			{
				List<int> artifacts = diveGameData.Player.PlayerBase.Artifacts;
				for (int i = 0; i < artifacts.Count; i++)
				{
					if (artifacts[i] == iD)
					{
						num = 1;
						break;
					}
				}
			}
			SetActiveCount(value: true);
			if (count <= num)
			{
				NKCUtil.SetLabelText(m_lbItemCount, $"{count}/{num}");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbItemCount, $"{count}/<color=#ff0000ff>{num}</color>");
			}
		}
		else if (bShowCount && count > 1)
		{
			SetActiveCount(value: true);
			NKCUtil.SetLabelText(m_lbItemCount, count.ToString());
		}
		else
		{
			SetActiveCount(value: false);
		}
		NKCUtil.SetGameobjectActive(m_imgBG, bValue: true);
		SetItemBackground(nKMEmoticonTemplet.m_EmoticonGrade);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		m_imgIcon.color = new Color(1f, 1f, 1f, 1f);
		Sprite sprite = null;
		sprite = NKCResourceUtility.GetOrLoadEmoticonIcon(nKMEmoticonTemplet);
		m_imgIcon.sprite = sprite;
		if (sprite == null)
		{
			Debug.LogError("iconSprite not found. emoticon ID : " + iD);
		}
		NKCUtil.SetGameobjectActive(m_objTier, bValue: false);
	}

	public void SetMoldData(SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		SetMoldData(null, slotData, bShowName, bShowCount, bEnableLayoutElement, onClick);
	}

	public void SetMoldData(NKMUserData userData, SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		int iD = slotData.ID;
		long count = slotData.Count;
		m_bEmpty = false;
		TurnOffExtraUI();
		dOnClick = onClick;
		m_SlotData = slotData;
		m_layoutElement.enabled = bEnableLayoutElement;
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(iD);
		if (itemMoldTempletByID == null)
		{
			SetEmpty();
			return;
		}
		if (bShowName)
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: true);
			SetName(itemMoldTempletByID.GetItemName());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: false);
		}
		if (userData != null)
		{
			SetActiveCount(value: true);
			long moldCount = userData.m_CraftData.GetMoldCount(iD);
			if (count <= moldCount)
			{
				NKCUtil.SetLabelText(m_lbItemCount, $"{count}/{moldCount}");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbItemCount, $"{count}/<color=#ff0000ff>{moldCount}</color>");
			}
		}
		else if (bShowCount && count > 1)
		{
			SetActiveCount(value: true);
			NKCUtil.SetLabelText(m_lbItemCount, count.ToString());
		}
		else
		{
			SetActiveCount(value: false);
		}
		NKCUtil.SetGameobjectActive(m_imgBG, bValue: true);
		SetItemBackground(itemMoldTempletByID.m_Grade, bUseWhiteBG: false);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		m_imgIcon.color = new Color(1f, 1f, 1f, 1f);
		Sprite orLoadMoldIcon = NKCResourceUtility.GetOrLoadMoldIcon(itemMoldTempletByID);
		m_imgIcon.sprite = orLoadMoldIcon;
		if (orLoadMoldIcon == null)
		{
			Debug.LogError("Item iconSprite not found. itemMoldID : " + iD);
		}
		NKCUtil.SetGameobjectActive(m_objTier, itemMoldTempletByID.IsEquipMold);
		NKCUtil.SetLabelText(m_lbTier, NKCUtilString.GetItemEquipTier(itemMoldTempletByID.m_Tier));
	}

	public void SetMiscItemData(NKMItemMiscData data, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		SetMiscItemData(null, SlotData.MakeMiscItemData(data), bShowName, bShowCount, bEnableLayoutElement, onClick);
	}

	public void SetMiscItemData(int itemID, long itemCount, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		SetMiscItemData(null, SlotData.MakeMiscItemData(itemID, itemCount), bShowName, bShowCount, bEnableLayoutElement, onClick);
	}

	public void SetMiscItemData(NKMUserData userData, NKMItemMiscData data, bool bShowName, bool bEnableLayoutElement, OnClick onClick)
	{
		SetMiscItemData(userData, SlotData.MakeMiscItemData(data), bShowName, bShowCount: true, bEnableLayoutElement, onClick);
	}

	public void SetMiscItemData(NKMUserData userData, int itemID, long itemCount, bool bShowName, bool bEnableLayoutElement, bool bHideUseCnt, OnClick onClick, bool bShowCount = true)
	{
		SetMiscItemData(userData, SlotData.MakeMiscItemData(itemID, itemCount), bShowName, bShowCount, bEnableLayoutElement, onClick, bHideUseCnt);
	}

	public void SetMiscItemData(NKMUserData userData, int itemID, long itemCount, bool bShowName, bool bEnableLayoutElement, OnClick onClick, bool bShowCount = true)
	{
		SetMiscItemData(userData, SlotData.MakeMiscItemData(itemID, itemCount), bShowName, bShowCount, bEnableLayoutElement, onClick);
	}

	public void SetMiscItemData(SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		SetMiscItemData(null, slotData, bShowName, bShowCount, bEnableLayoutElement, onClick);
	}

	public void SetMiscItemData(NKMUserData userData, SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick, bool bHideUseCnt = false)
	{
		int iD = slotData.ID;
		long count = slotData.Count;
		m_bEmpty = false;
		TurnOffExtraUI();
		dOnClick = onClick;
		m_SlotData = slotData;
		m_layoutElement.enabled = bEnableLayoutElement;
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(iD);
		if (itemMiscTempletByID == null)
		{
			SetEmpty();
			return;
		}
		if (bShowName)
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: true);
			SetName(itemMiscTempletByID.GetItemName());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: false);
		}
		if (userData != null)
		{
			if (bShowCount)
			{
				SetActiveCount(value: true);
				long countMiscItem = userData.m_InventoryData.GetCountMiscItem(iD);
				NKMItemMiscTemplet itemMiscTempletByID2 = NKMItemManager.GetItemMiscTempletByID(iD);
				if (itemMiscTempletByID2 != null)
				{
					bool flag = itemMiscTempletByID2.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_RESOURCE;
					if (bHideUseCnt)
					{
						if (countMiscItem >= count)
						{
							NKCUtil.SetLabelText(m_lbItemCount, $"<color=#ffffff>{count}</color>");
						}
						else
						{
							NKCUtil.SetLabelText(m_lbItemCount, $"<color=#ff0000ff>{count}</color>");
						}
					}
					else if (count <= countMiscItem)
					{
						if (flag)
						{
							NKCUtil.SetLabelText(m_lbItemCount, $"{count}");
						}
						else
						{
							NKCUtil.SetLabelText(m_lbItemCount, $"{count}/{countMiscItem}");
						}
					}
					else if (flag)
					{
						NKCUtil.SetLabelText(m_lbItemCount, $"<color=#ff0000ff>{count}</color>");
					}
					else
					{
						NKCUtil.SetLabelText(m_lbItemCount, $"{count}/<color=#ff0000ff>{countMiscItem}</color>");
					}
				}
			}
			else
			{
				SetActiveCount(value: false);
			}
		}
		else if (itemMiscTempletByID.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_EMBLEM_RANK)
		{
			SetActiveCount(value: false);
			if (bShowCount && count > 1)
			{
				SetAdditionalText(string.Format(NKCUtilString.GET_STRING_RANK_ONE_PARAM, count));
			}
		}
		else if (bShowCount && count > 1)
		{
			SetActiveCount(value: true);
			NKCUtil.SetLabelText(m_lbItemCount, count.ToString("N0"));
		}
		else
		{
			SetActiveCount(value: false);
		}
		NKCUtil.SetGameobjectActive(m_imgBG, bValue: true);
		SetItemBackground(itemMiscTempletByID);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		m_imgIcon.color = new Color(1f, 1f, 1f, 1f);
		Sprite orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(itemMiscTempletByID);
		m_imgIcon.sprite = orLoadMiscItemIcon;
		if (orLoadMiscItemIcon == null)
		{
			Debug.LogError("Item iconSprite not found. itemMiscID : " + iD);
		}
		NKCUtil.SetGameobjectActive(m_objTimeInterval, itemMiscTempletByID.IsTimeIntervalItem);
	}

	private bool ItemUsingWhiteBackground(NKMItemMiscTemplet itemTemplet)
	{
		NKM_ITEM_MISC_TYPE itemMiscType = itemTemplet.m_ItemMiscType;
		if ((uint)(itemMiscType - 14) <= 1u || itemMiscType == NKM_ITEM_MISC_TYPE.IMT_INTERIOR)
		{
			return true;
		}
		return false;
	}

	private void SetItemBackground(NKMItemMiscTemplet templet)
	{
		if (templet != null)
		{
			if (templet.IsEmblem())
			{
				m_imgBG.sprite = m_spEmblem;
				return;
			}
			bool bUseWhiteBG = ItemUsingWhiteBackground(templet);
			SetItemBackground(templet.m_NKM_ITEM_GRADE, bUseWhiteBG);
		}
	}

	private void SetItemBackground(NKM_ITEM_GRADE grade, bool bUseWhiteBG)
	{
		if (bUseWhiteBG)
		{
			switch (grade)
			{
			case NKM_ITEM_GRADE.NIG_N:
				m_imgBG.sprite = m_spBGWhiteRarityN;
				break;
			case NKM_ITEM_GRADE.NIG_R:
				m_imgBG.sprite = m_spBGWhiteRarityR;
				break;
			case NKM_ITEM_GRADE.NIG_SR:
				m_imgBG.sprite = m_spBGWhiteRaritySR;
				break;
			case NKM_ITEM_GRADE.NIG_SSR:
				m_imgBG.sprite = m_spBGWhiteRaritySSR;
				break;
			default:
				Debug.LogError("Item BG undefined");
				m_imgBG.sprite = m_spBGWhiteRarityN;
				break;
			}
		}
		else
		{
			switch (grade)
			{
			case NKM_ITEM_GRADE.NIG_N:
				m_imgBG.sprite = m_spBGRarityN;
				break;
			case NKM_ITEM_GRADE.NIG_R:
				m_imgBG.sprite = m_spBGRarityR;
				break;
			case NKM_ITEM_GRADE.NIG_SR:
				m_imgBG.sprite = m_spBGRaritySR;
				break;
			case NKM_ITEM_GRADE.NIG_SSR:
				m_imgBG.sprite = m_spBGRaritySSR;
				break;
			default:
				Debug.LogError("Item BG undefined");
				m_imgBG.sprite = m_spBGRarityN;
				break;
			}
		}
	}

	private void SetItemBackground(NKM_EMOTICON_GRADE grade)
	{
		switch (grade)
		{
		case NKM_EMOTICON_GRADE.NEG_N:
			m_imgBG.sprite = m_spBGRarityN;
			break;
		case NKM_EMOTICON_GRADE.NEG_R:
			m_imgBG.sprite = m_spBGRarityR;
			break;
		case NKM_EMOTICON_GRADE.NEG_SR:
			m_imgBG.sprite = m_spBGRaritySR;
			break;
		case NKM_EMOTICON_GRADE.NEG_SSR:
			m_imgBG.sprite = m_spBGRaritySSR;
			break;
		default:
			Debug.LogError("Item BG undefined");
			m_imgBG.sprite = m_spBGRarityN;
			break;
		}
	}

	public void SetUnitData(NKMOperator data, bool bShowName, bool bShowLevel, bool bEnableLayoutElement, OnClick onClick)
	{
		SetUnitData(SlotData.MakeUnitData(data), bShowName, bShowLevel, bEnableLayoutElement, onClick);
	}

	public void SetUnitData(NKMUnitData data, bool bShowName, bool bShowLevel, bool bEnableLayoutElement, OnClick onClick)
	{
		SetUnitData(SlotData.MakeUnitData(data), bShowName, bShowLevel, bEnableLayoutElement, onClick);
	}

	public void SetUnitData(int unitID, int unitLevel, int skinID, bool bShowName, bool bShowLevel, bool bEnableLayoutElement, OnClick onClick)
	{
		SetUnitData(SlotData.MakeUnitData(unitID, unitLevel, skinID), bShowName, bShowLevel, bEnableLayoutElement, onClick);
	}

	public void SetUnitData(SlotData slotData, bool bShowName, bool bShowLevel, bool bEnableLayoutElement, OnClick onClick)
	{
		m_bEmpty = false;
		m_SlotData = slotData;
		dOnClick = onClick;
		if (bShowLevel)
		{
			SetActiveCount(value: true);
			NKCUtil.SetLabelText(m_lbItemCount, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, slotData.Count));
		}
		else
		{
			SetActiveCount(value: false);
		}
		m_layoutElement.enabled = bEnableLayoutElement;
		SetUnitTempletData(m_SlotData.ID, m_SlotData.Data, bShowName, onClick);
	}

	public void SetUnitCountData(SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		m_bEmpty = false;
		m_SlotData = slotData;
		dOnClick = onClick;
		if (bShowCount && slotData.Count > 1)
		{
			SetActiveCount(value: true);
			NKCUtil.SetLabelText(m_lbItemCount, slotData.Count.ToString("N0"));
		}
		else
		{
			SetActiveCount(value: false);
		}
		m_layoutElement.enabled = bEnableLayoutElement;
		SetUnitTempletData(m_SlotData.ID, m_SlotData.Data, bShowName, onClick);
	}

	private void SetUnitTempletData(int unitID, int skinID, bool bShowName, OnClick onClick)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		TurnOffExtraUI();
		if (unitTempletBase == null)
		{
			SetEmpty(onClick);
			return;
		}
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		m_imgIcon.color = new Color(1f, 1f, 1f, 1f);
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
		if (skinTemplet != null && skinTemplet.m_SkinEquipUnitID == unitID)
		{
			m_imgIcon.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, skinTemplet);
		}
		else
		{
			m_imgIcon.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase);
		}
		NKCUtil.SetGameobjectActive(m_lbName, bShowName);
		NKCUtil.SetLabelText(m_lbName, unitTempletBase.GetUnitName());
		SetUnitBackground(unitTempletBase.m_NKM_UNIT_GRADE);
		NKCUtil.SetAwakenFX(m_animAwakenFX, unitTempletBase);
		NKCUtil.SetGameobjectActive(m_imgUpperRightIcon, bValue: true);
		Sprite orLoadUnitTypeIcon = NKCResourceUtility.GetOrLoadUnitTypeIcon(unitTempletBase, bSmall: true);
		NKCUtil.SetImageSprite(m_imgUpperRightIcon, orLoadUnitTypeIcon, bDisableIfSpriteNull: true);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
	}

	private void SetUnitBackground(NKM_UNIT_GRADE grade)
	{
		switch (grade)
		{
		case NKM_UNIT_GRADE.NUG_N:
			m_imgBG.sprite = m_spBGRarityN;
			break;
		case NKM_UNIT_GRADE.NUG_R:
			m_imgBG.sprite = m_spBGRarityR;
			break;
		case NKM_UNIT_GRADE.NUG_SR:
			m_imgBG.sprite = m_spBGRaritySR;
			break;
		case NKM_UNIT_GRADE.NUG_SSR:
			m_imgBG.sprite = m_spBGRaritySSR;
			break;
		default:
			Debug.LogError("Unit BG undefined");
			m_imgBG.sprite = m_spBGRarityN;
			break;
		}
	}

	private void SetEquipData(SlotData slotData, bool bShowName, bool bShowLevel, bool bEnableLayoutElement, OnClick onClick)
	{
		int iD = slotData.ID;
		int num = (int)slotData.Count;
		int groupID = slotData.GroupID;
		m_bEmpty = false;
		dOnClick = onClick;
		m_SlotData = slotData;
		m_layoutElement.enabled = bEnableLayoutElement;
		SetEquipTempletData(iD, bShowName);
		SetEquipItemData(slotData.UID);
		bool flag = bShowLevel && num > 0;
		SetActiveCount(flag);
		if (flag)
		{
			NKCUtil.SetLabelText(m_lbItemCount, "+" + num);
		}
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(groupID);
		if (equipSetOptionTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_EQUIP_SET_ICON.gameObject, bValue: true);
			NKCUtil.SetImageSprite(m_AB_ICON_SLOT_EQUIP_SET_ICON, NKCUtil.GetSpriteEquipSetOptionIcon(equipSetOptionTemplet));
		}
	}

	private void SetEquipCountData(SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		int iD = slotData.ID;
		int groupID = slotData.GroupID;
		m_bEmpty = false;
		dOnClick = onClick;
		m_SlotData = slotData;
		m_layoutElement.enabled = bEnableLayoutElement;
		SetEquipTempletData(iD, bShowName);
		SetEquipItemData(slotData.UID);
		bShowCount = bShowCount && slotData.Count > 1;
		SetActiveCount(bShowCount);
		if (bShowCount)
		{
			if (slotData.Count > 999)
			{
				NKCUtil.SetLabelText(m_lbItemCount, "999+");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbItemCount, slotData.Count.ToString("N0"));
			}
		}
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(groupID);
		if (equipSetOptionTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_EQUIP_SET_ICON.gameObject, bValue: true);
			NKCUtil.SetImageSprite(m_AB_ICON_SLOT_EQUIP_SET_ICON, NKCUtil.GetSpriteEquipSetOptionIcon(equipSetOptionTemplet));
		}
	}

	private void SetEquipTempletData(int equipID, bool bShowName)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipID);
		TurnOffExtraUI();
		if (equipTemplet == null)
		{
			SetEmpty();
			return;
		}
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		m_imgIcon.color = new Color(1f, 1f, 1f, 1f);
		m_imgIcon.sprite = NKCResourceUtility.GetOrLoadEquipIcon(equipTemplet);
		NKCUtil.SetGameobjectActive(m_lbName, bShowName);
		NKCUtil.SetLabelText(m_lbName, NKCUtilString.GetItemEquipNameWithTier(equipTemplet));
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgBG, bValue: true);
		SetItemBackground(equipTemplet.m_NKM_ITEM_GRADE, bUseWhiteBG: false);
		NKCUtil.SetGameobjectActive(m_objTier, bValue: true);
		NKCUtil.SetLabelText(m_lbTier, NKCUtilString.GetItemEquipTier(equipTemplet.m_NKM_ITEM_TIER));
		SetEquipEffect(equipTemplet.m_bShowEffect);
	}

	private void SetEquipItemData(long equipUID)
	{
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(equipUID);
		if (itemEquip == null)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRelic, equipTemplet.IsRelic());
		if (!m_objRelic.activeSelf)
		{
			return;
		}
		for (int i = 0; i < m_lstImgRelic.Count; i++)
		{
			if (itemEquip.potentialOptions.Count <= 0 || itemEquip.potentialOptions[0] == null || itemEquip.potentialOptions[0].sockets == null)
			{
				NKCUtil.SetGameobjectActive(m_lstImgRelic[i], bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstImgRelic[i], i < itemEquip.potentialOptions[0].sockets.Length && itemEquip.potentialOptions[0].sockets[i] != null);
			}
		}
	}

	private void SetSkinData(SlotData slotData, bool bShowName, bool bShowNumber, bool bEnableLayoutElement, OnClick onClick)
	{
		int iD = slotData.ID;
		m_bEmpty = false;
		m_SlotData = slotData;
		dOnClick = onClick;
		TurnOffExtraUI();
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(iD);
		if (skinTemplet == null)
		{
			SetEmpty();
			return;
		}
		NKCUtil.SetGameobjectActive(m_lbName, bShowName);
		SetActiveCount(bShowNumber);
		m_layoutElement.enabled = bEnableLayoutElement;
		NKCUtil.SetLabelText(m_lbName, skinTemplet.GetTitle());
		NKCUtil.SetLabelText(m_lbItemCount, NKCUtilString.GET_STRING_SKIN);
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		m_imgIcon.color = new Color(1f, 1f, 1f, 1f);
		m_imgIcon.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, skinTemplet);
		NKCUtil.SetGameobjectActive(m_imgBG, bValue: true);
		SetBackGround(skinTemplet.m_SkinGrade);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
	}

	public void SetBackGround(NKMSkinTemplet.SKIN_GRADE grade)
	{
		switch (grade)
		{
		case NKMSkinTemplet.SKIN_GRADE.SG_VARIATION:
			m_imgBG.sprite = m_spBGRarityN;
			break;
		case NKMSkinTemplet.SKIN_GRADE.SG_NORMAL:
			m_imgBG.sprite = m_spBGRarityR;
			break;
		case NKMSkinTemplet.SKIN_GRADE.SG_RARE:
			m_imgBG.sprite = m_spBGRaritySR;
			break;
		case NKMSkinTemplet.SKIN_GRADE.SG_PREMIUM:
		case NKMSkinTemplet.SKIN_GRADE.SG_SPECIAL:
			m_imgBG.sprite = m_spBGRaritySSR;
			break;
		default:
			Debug.LogError("SKIN_GRADE BG undefined");
			m_imgBG.sprite = m_spBGRarityN;
			break;
		}
	}

	private void SetBuffData(SlotData slotData, bool bShowName, bool bShowNumber, bool bEnableLayoutElement, OnClick onClick)
	{
		_ = slotData.ID;
		m_bEmpty = false;
		m_SlotData = slotData;
		dOnClick = onClick;
		TurnOffExtraUI();
		NKMCompanyBuffTemplet companyBuffTemplet = NKMCompanyBuffManager.GetCompanyBuffTemplet(slotData.ID);
		if (companyBuffTemplet == null)
		{
			SetEmpty();
			return;
		}
		NKCUtil.SetGameobjectActive(m_lbName, bShowName);
		SetActiveCount(bShowNumber);
		m_layoutElement.enabled = bEnableLayoutElement;
		NKCUtil.SetLabelText(m_lbName, companyBuffTemplet.GetBuffName());
		NKCUtil.SetLabelText(m_lbItemCount, string.Empty);
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		m_imgIcon.color = new Color(1f, 1f, 1f, 1f);
		m_imgIcon.sprite = NKCResourceUtility.GetOrLoadBuffIconForItemPopup(companyBuffTemplet);
		NKCUtil.SetGameobjectActive(m_imgBG, bValue: true);
		SetItemBackground(NKM_ITEM_GRADE.NIG_SSR, bUseWhiteBG: false);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
	}

	public void SetEtcData(SlotData slotData, Sprite iconSprite, string countText, string name, OnClick onClick)
	{
		TurnOffExtraUI();
		dOnClick = onClick;
		m_SlotData = slotData;
		NKCUtil.SetImageSprite(m_imgIcon, iconSprite);
		NKCUtil.SetGameobjectActive(m_lbItemCount, !string.IsNullOrEmpty(countText));
		NKCUtil.SetLabelText(m_lbItemCount, countText);
		NKCUtil.SetGameobjectActive(m_lbName, !string.IsNullOrEmpty(name));
		NKCUtil.SetLabelText(m_lbName, name);
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		m_imgIcon.color = new Color(1f, 1f, 1f, 1f);
		NKCUtil.SetGameobjectActive(m_imgBG, bValue: false);
	}

	public void SetEtcData(SlotData slotData, Sprite iconSprite, string countText, string name, NKM_ITEM_GRADE grade, OnClick onClick)
	{
		TurnOffExtraUI();
		dOnClick = onClick;
		m_SlotData = slotData;
		NKCUtil.SetImageSprite(m_imgIcon, iconSprite);
		NKCUtil.SetLabelText(m_lbItemCount, countText);
		NKCUtil.SetLabelText(m_lbName, name);
		SetItemBackground(grade, bUseWhiteBG: false);
	}

	public void SetGuildArtifactData(SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		SetGuildArtifactData(null, slotData, bShowName, bShowCount, bEnableLayoutElement, onClick);
	}

	public void SetGuildArtifactData(NKMUserData userData, SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		int iD = slotData.ID;
		long count = slotData.Count;
		m_bEmpty = false;
		TurnOffExtraUI();
		dOnClick = onClick;
		m_SlotData = slotData;
		m_layoutElement.enabled = bEnableLayoutElement;
		GuildDungeonArtifactTemplet artifactTemplet = GuildDungeonTempletManager.GetArtifactTemplet(iD);
		if (artifactTemplet == null)
		{
			SetEmpty();
			return;
		}
		if (bShowName)
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: true);
			SetName(artifactTemplet.GetName());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbName, bValue: false);
		}
		if (bShowCount && count > 1)
		{
			SetActiveCount(value: true);
			NKCUtil.SetLabelText(m_lbItemCount, count.ToString("N0"));
		}
		else
		{
			SetActiveCount(value: false);
		}
		NKCUtil.SetGameobjectActive(m_imgBG, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		m_imgIcon.color = new Color(1f, 1f, 1f, 1f);
		Sprite sprite = null;
		sprite = ((!m_bUseBigImg) ? NKCResourceUtility.GetOrLoadGuildArtifactIcon(artifactTemplet) : NKCResourceUtility.GetOrLoadGuildArtifactIconBig(artifactTemplet));
		m_imgIcon.sprite = sprite;
		if (sprite == null)
		{
			Debug.LogError("iconSprite not found. artifact ID : " + iD);
		}
		NKCUtil.SetGameobjectActive(m_objTier, bValue: false);
	}

	public void SetReactorData(SlotData slotData, bool bShowName, bool bShowCount, bool bEnableLayoutElement, OnClick onClick)
	{
		m_bEmpty = false;
		m_SlotData = slotData;
		dOnClick = onClick;
		SetActiveCount(value: false);
		TurnOffExtraUI();
		if (bShowCount && slotData.Count > 0)
		{
			NKCUtil.SetGameobjectActive(m_objTier, bValue: true);
			NKCUtil.SetLabelText(m_lbTier, string.Format(NKCUtilString.GET_STRING_UNIT_REACTOR_EQUIP_LEVEL_01, slotData.Count.ToString()));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objTier, bValue: false);
			SetActiveCount(value: false);
		}
		m_layoutElement.enabled = bEnableLayoutElement;
		string assetName = null;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(slotData.ID);
		if (unitTempletBase != null)
		{
			assetName = NKCReactorUtil.GetReactorTemplet(unitTempletBase)?.ReactorIcon;
		}
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
		NKCUtil.SetImageColor(m_imgIcon, Color.white);
		NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetReactorIcon(assetName));
		NKCUtil.SetImageSprite(m_imgBG, m_spBGRarityReactor);
		NKCUtil.SetGameobjectActive(m_lbName, bShowName);
		NKCUtil.SetGameobjectActive(m_imgUpperRightIcon, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objReactorFX, bValue: true);
	}

	public void PlaySmallToOrgSize()
	{
		base.transform.DOComplete();
		base.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		base.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
	}

	public void SetOriginalSize()
	{
		base.transform.DOComplete();
		base.transform.localScale = Vector3.one;
	}

	public void SetOnClickAction(params SlotClickType[] clickTypes)
	{
		dOnPointerDown = null;
		dOnClick = null;
		if (m_SlotData == null)
		{
			return;
		}
		for (int i = 0; i < clickTypes.Length; i++)
		{
			switch (clickTypes[i])
			{
			case SlotClickType.Tooltip:
				SetOpenTooltipOnPress(bBlockClick: true);
				return;
			case SlotClickType.ItemBox:
				if (m_SlotData.eType != eSlotMode.Skin)
				{
					SetOpenItemBoxOnClick();
					return;
				}
				break;
			case SlotClickType.RatioList:
			{
				if (m_SlotData.eType != eSlotMode.ItemMisc)
				{
					break;
				}
				NKMItemMiscTemplet itemMiscTempletByID3 = NKMItemManager.GetItemMiscTempletByID(m_SlotData.ID);
				if (itemMiscTempletByID3 != null && itemMiscTempletByID3.IsUsable() && itemMiscTempletByID3.IsRatioOpened())
				{
					if (itemMiscTempletByID3.IsContractItem)
					{
						SetOpenContractRatioBoxOnPress();
					}
					else
					{
						SetOpenRatioBoxOnPress();
					}
					return;
				}
				break;
			}
			case SlotClickType.BoxList:
				if (m_SlotData.eType == eSlotMode.ItemMisc)
				{
					NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_SlotData.ID);
					if (itemMiscTempletByID != null && itemMiscTempletByID.IsUsable() && itemMiscTempletByID.IsPackageItem)
					{
						SetOpenPackageList();
						return;
					}
				}
				break;
			case SlotClickType.ChoiceList:
				if (m_SlotData.eType == eSlotMode.ItemMisc)
				{
					NKMItemMiscTemplet itemMiscTempletByID2 = NKMItemManager.GetItemMiscTempletByID(m_SlotData.ID);
					if (itemMiscTempletByID2 != null && itemMiscTempletByID2.IsChoiceItem())
					{
						SetOpenChoiceList();
						return;
					}
				}
				break;
			case SlotClickType.MoldList:
				if (m_SlotData.eType == eSlotMode.Mold)
				{
					NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(m_SlotData.ID);
					if (itemMoldTempletByID != null)
					{
						SetOpenItemBoxOnClick();
						SetItemDetailIcon(itemMoldTempletByID.IsEquipMold);
						return;
					}
				}
				break;
			case SlotClickType.None:
				return;
			}
		}
	}

	public void SetOnClick(OnClick onClick)
	{
		dOnClick = onClick;
	}

	public void ResetOnClick()
	{
		dOnClick = null;
	}

	public void SetOpenItemBoxOnClick()
	{
		dOnClick = OpenItemBox;
	}

	private void OpenItemBox(SlotData slotData, bool bLocked)
	{
		if (slotData == null)
		{
			return;
		}
		switch (slotData.eType)
		{
		default:
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData);
			break;
		case eSlotMode.Unit:
		case eSlotMode.UnitCount:
		case eSlotMode.Emoticon:
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData);
			break;
		case eSlotMode.Equip:
		case eSlotMode.EquipCount:
			OpenEquipBox(slotData);
			break;
		case eSlotMode.Skin:
			Debug.LogWarning("Skin Popup under construction");
			break;
		case eSlotMode.Mold:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(m_SlotData.ID);
			if (itemMoldTempletByID != null && itemMoldTempletByID.IsEquipMold && NKMItemManager.m_dicRandomMoldBox.ContainsKey(itemMoldTempletByID.m_RewardGroupID))
			{
				Dictionary<NKM_REWARD_TYPE, List<int>> dictionary = NKMItemManager.m_dicRandomMoldBox[itemMoldTempletByID.m_RewardGroupID];
				if (dictionary != null && dictionary.Count > 0)
				{
					NKCUISlotListViewer.GetNewInstance().OpenEquipMoldRewardList(dictionary, itemMoldTempletByID.GetItemName(), NKCUtilString.GET_STRING_FORGE_CRAFT_MOLD_DESC);
				}
			}
			break;
		}
		}
	}

	private void SetOpenTooltipOnPress(bool bBlockClick = false)
	{
		dOnPointerDown = OpenTooltip;
		if (bBlockClick)
		{
			dOnClick = null;
		}
	}

	private void OpenTooltip(SlotData slotData, bool bLocked, PointerEventData eventData)
	{
		if (slotData != null)
		{
			NKCUITooltip.Instance.Open(slotData, eventData.position);
		}
	}

	private void OpenEquipBox(SlotData slotData)
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(slotData.UID);
		if (itemEquip == null)
		{
			itemEquip = NKCEquipSortSystem.MakeTempEquipData(slotData.ID, slotData.GroupID);
			NKCPopupItemEquipBox.Open(itemEquip, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
		else if (NKCUIWarfareResult.IsInstanceOpen)
		{
			NKCPopupItemEquipBox.Open(itemEquip, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
		else if (itemEquip.m_OwnerUnitUID > 0)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet == null)
			{
				return;
			}
			NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(itemEquip.m_OwnerUnitUID);
			if (unitFromUID != null)
			{
				NKM_ERROR_CODE nKM_ERROR_CODE = equipTemplet.CanUnEquipByUnit(NKCScenManager.GetScenManager().GetMyUserData(), unitFromUID);
				if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
				{
					NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE.ToString());
				}
				else
				{
					NKCPopupItemEquipBox.Open(itemEquip, m_EQUIP_BOX_BOTTOM_MENU_TYPE);
				}
			}
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_DUNGEON_ATK_READY)
		{
			NKCPopupItemEquipBox.Open(itemEquip, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
		else
		{
			NKCPopupItemEquipBox.Open(itemEquip, m_EQUIP_BOX_BOTTOM_MENU_TYPE);
		}
	}

	private void SetOpenRatioBoxOnPress()
	{
		SetItemDetailIcon(bValue: true);
		dOnClick = OpenRatioBox;
	}

	private void SetOpenContractRatioBoxOnPress()
	{
		SetItemDetailIcon(bValue: true);
		dOnClick = OpenContractRatio;
	}

	private void OpenRatioBox(SlotData slotData, bool bLocked)
	{
		if (slotData == null || slotData.eType != eSlotMode.ItemMisc)
		{
			return;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(slotData.ID);
		if (itemMiscTempletByID != null && itemMiscTempletByID.IsUsable() && itemMiscTempletByID.IsRatioOpened())
		{
			NKCUISlotListViewer newInstance = NKCUISlotListViewer.GetNewInstance();
			if (newInstance != null)
			{
				newInstance.OpenItemBoxRatio(slotData.ID);
			}
		}
	}

	private void OpenContractRatio(SlotData slotData, bool bLocked)
	{
		if (slotData != null && slotData.eType == eSlotMode.ItemMisc)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(slotData.ID);
			NKCUIContractPopupRateV2.Instance.Open(itemMiscTempletByID);
		}
	}

	private void SetOpenPackageList()
	{
		SetItemDetailIcon(bValue: true);
		dOnClick = OpenPackageList;
	}

	private void OpenPackageList(SlotData slotData, bool bLocked)
	{
		if (slotData == null || slotData.eType != eSlotMode.ItemMisc)
		{
			return;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(slotData.ID);
		if (itemMiscTempletByID != null && itemMiscTempletByID.IsUsable() && itemMiscTempletByID.IsPackageItem)
		{
			NKCUISlotListViewer newInstance = NKCUISlotListViewer.GetNewInstance();
			if (newInstance != null)
			{
				newInstance.OpenPackageInfo(m_SlotData.ID);
			}
		}
	}

	private void SetOpenChoiceList()
	{
		SetItemDetailIcon(bValue: true);
		dOnClick = OpenChoiceList;
	}

	private void OpenChoiceList(SlotData slotData, bool bLocked)
	{
		if (slotData == null || slotData.eType != eSlotMode.ItemMisc)
		{
			return;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(slotData.ID);
		if (itemMiscTempletByID != null && itemMiscTempletByID.IsChoiceItem())
		{
			NKCUISlotListViewer newInstance = NKCUISlotListViewer.GetNewInstance();
			if (newInstance != null)
			{
				newInstance.OpenChoiceInfo(m_SlotData.ID);
			}
		}
	}

	public void SetBackGround(int grade)
	{
		switch (grade)
		{
		case 0:
			m_imgBG.sprite = m_spBGRarityN;
			break;
		case 1:
			m_imgBG.sprite = m_spBGRarityR;
			break;
		case 2:
			m_imgBG.sprite = m_spBGRaritySR;
			break;
		case 3:
			m_imgBG.sprite = m_spBGRaritySSR;
			break;
		case 4:
			m_imgBG.sprite = m_spBGRarityEPIC;
			break;
		default:
			Debug.LogError("Unit BG undefined");
			m_imgBG.sprite = m_spBGRarityN;
			break;
		}
	}

	public void SetAdditionalText(string text, TextAnchor alignment = TextAnchor.MiddleCenter)
	{
		if (m_lbAdditionalText != null)
		{
			m_lbAdditionalText.alignment = alignment;
		}
		NKCUtil.SetLabelText(m_lbAdditionalText, text);
		NKCUtil.SetGameobjectActive(m_lbAdditionalText, bValue: true);
	}

	public void SetDenied(bool value)
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_DISABLE, value);
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_DENIED, value);
	}

	public void SetDisable(bool disable, string text = "")
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_DISABLE, disable);
		if (disable && !string.IsNullOrEmpty(text))
		{
			NKCUtil.SetLabelText(m_lbDisable, text);
			NKCUtil.SetGameobjectActive(m_lbDisable, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbDisable, bValue: false);
		}
	}

	public void SetUsable(bool usable)
	{
		if (usable)
		{
			bool flag = false;
			NKMItemMiscTemplet nKMItemMiscTemplet = null;
			if (m_SlotData != null && m_SlotData.eType == eSlotMode.ItemMisc)
			{
				nKMItemMiscTemplet = NKMItemManager.GetItemMiscTempletByID(m_SlotData.ID);
			}
			if (nKMItemMiscTemplet != null)
			{
				flag = nKMItemMiscTemplet.WillBeDeletedSoon();
			}
			NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_INDUCE_ARROW, !flag);
			NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_INDUCE_ARROW_RED, flag);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_INDUCE_ARROW, usable);
			NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_INDUCE_ARROW_RED, usable);
		}
	}

	public void SetClear(bool clear)
	{
		NKCUtil.SetGameobjectActive(m_objCompleteMark, clear);
	}

	public void SetSelected(bool bSelected)
	{
		NKCUtil.SetGameobjectActive(m_objSelected, bSelected);
	}

	public bool GetSelected()
	{
		if (m_objSelected == null)
		{
			return false;
		}
		return m_objSelected.activeSelf;
	}

	public void SetItemDetailIcon(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objItemDetail, bValue);
	}

	public void SetSeized(bool bSeized)
	{
		NKCUtil.SetGameobjectActive(m_objSeized, bSeized);
	}

	public void SetRewardFx(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_objRewardFx, bActive);
	}

	public void SetEventGet(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_objEventGet, bActive);
	}

	public void SetTopNotice(string notice, bool bActive)
	{
		NKCUtil.SetLabelText(m_lbTopNoticeText, notice);
		NKCUtil.SetGameobjectActive(m_objTopNotice, bActive);
	}

	public void SetEquipEffect(bool bShowEffect)
	{
		NKCUtil.SetGameobjectActive(m_objTier_7, bShowEffect);
	}

	public void SetRelic(SlotData slotData)
	{
		NKCUtil.SetGameobjectActive(m_objRelic, bValue: false);
		if (NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(slotData.UID) == null)
		{
			NKCEquipSortSystem.MakeTempEquipData(slotData.ID, slotData.GroupID);
		}
		if (NKMItemManager.GetEquipTemplet(slotData.ID) == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRelic, bValue: true);
		for (int i = 0; i < m_lstImgRelic.Count; i++)
		{
			if (i < 0)
			{
				NKCUtil.SetGameobjectActive(m_lstImgRelic[i], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstImgRelic[i], bValue: false);
			}
		}
	}

	public void SetMaxLevelTacticFX(NKMUnitData unitData)
	{
		if (unitData != null)
		{
			SetMaxLevelTacticFX(unitData.tacticLevel == 6);
		}
	}

	public void SetMaxLevelTacticFX(bool bShowFx)
	{
		if (bShowFx && m_objTacticMAX.activeSelf)
		{
			NKCUtil.SetGameobjectActive(m_objTacticMAX, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objTacticMAX, bShowFx);
	}

	public void SetTokenArrow(bool bEnhance)
	{
		NKCUtil.SetGameobjectActive(m_objOperToken, bValue: true);
		NKCUtil.SetGameobjectActive(m_objOperTokenArrowEnhance, bEnhance);
		NKCUtil.SetGameobjectActive(m_objOperTokenArrowImplant, !bEnhance);
	}

	public void OnPress()
	{
		if (dOnClick != null)
		{
			dOnClick(m_SlotData, IsLocked);
		}
	}

	public void SetActive(bool bSet)
	{
		if (m_objParent != null)
		{
			if (m_objParent.activeSelf != bSet)
			{
				m_objParent.SetActive(bSet);
			}
		}
		else if (base.gameObject.activeSelf == !bSet)
		{
			base.gameObject.SetActive(bSet);
		}
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (dOnPointerDown != null)
		{
			dOnPointerDown(m_SlotData, IsLocked, eventData);
		}
	}

	public void SetRedudantMark(bool value)
	{
		if (value)
		{
			SetHaveCountString(bShow: true, NKCStringTable.GetString("SI_DP_ICON_SLOT_ALREADY_HAVE"));
		}
		else
		{
			SetHaveCountString(bShow: false, null);
		}
	}

	public void SetDuplicateSelection(bool value)
	{
		if (value)
		{
			SetHaveCountString(bShow: true, NKCStringTable.GetString("SI_DP_SHOP_CUSTOM_DUPLICATE"));
		}
		else
		{
			SetHaveCountString(bShow: false, null);
		}
	}

	public static string GetName(SlotData slotData)
	{
		if (slotData == null)
		{
			return "";
		}
		return GetName(slotData.eType, slotData.ID);
	}

	public static string GetName(eSlotMode type, int ID)
	{
		switch (type)
		{
		case eSlotMode.Unit:
		case eSlotMode.UnitCount:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(ID);
			if (unitTempletBase != null)
			{
				return unitTempletBase.GetUnitName();
			}
			break;
		}
		case eSlotMode.Mold:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(ID);
			if (itemMoldTempletByID != null)
			{
				return itemMoldTempletByID.GetItemName();
			}
			break;
		}
		case eSlotMode.ItemMisc:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(ID);
			if (itemMiscTempletByID != null)
			{
				return itemMiscTempletByID.GetItemName();
			}
			break;
		}
		case eSlotMode.Equip:
		case eSlotMode.EquipCount:
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(ID);
			if (equipTemplet != null)
			{
				return NKCUtilString.GetItemEquipNameWithTier(equipTemplet);
			}
			break;
		}
		case eSlotMode.Skin:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(ID);
			if (skinTemplet != null)
			{
				return skinTemplet.GetTitle();
			}
			break;
		}
		case eSlotMode.DiveArtifact:
		{
			NKMDiveArtifactTemplet nKMDiveArtifactTemplet = NKMDiveArtifactTemplet.Find(ID);
			if (nKMDiveArtifactTemplet != null)
			{
				return nKMDiveArtifactTemplet.ArtifactName_Translated;
			}
			break;
		}
		case eSlotMode.Emoticon:
		{
			NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(ID);
			if (nKMEmoticonTemplet != null)
			{
				return nKMEmoticonTemplet.GetEmoticonName();
			}
			break;
		}
		case eSlotMode.GuildArtifact:
		{
			GuildDungeonArtifactTemplet artifactTemplet = GuildDungeonTempletManager.GetArtifactTemplet(ID);
			if (artifactTemplet != null)
			{
				return artifactTemplet.GetName();
			}
			break;
		}
		case eSlotMode.Buff:
		{
			NKMCompanyBuffTemplet nKMCompanyBuffTemplet = NKMCompanyBuffTemplet.Find(ID);
			if (nKMCompanyBuffTemplet != null)
			{
				return nKMCompanyBuffTemplet.GetBuffName();
			}
			break;
		}
		}
		return "";
	}

	public static string GetDesc(SlotData slotData, bool bFull = false)
	{
		if (slotData == null)
		{
			return "";
		}
		return GetDesc(slotData.eType, slotData.ID, bFull);
	}

	public static string GetDesc(eSlotMode type, int ID, bool bFull = false)
	{
		switch (type)
		{
		case eSlotMode.Unit:
		case eSlotMode.UnitCount:
			if (bFull)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(ID);
				if (unitTempletBase != null)
				{
					return unitTempletBase.GetUnitDesc();
				}
			}
			break;
		case eSlotMode.Mold:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(ID);
			if (itemMoldTempletByID != null)
			{
				return itemMoldTempletByID.GetItemDesc();
			}
			break;
		}
		case eSlotMode.DiveArtifact:
		{
			NKMDiveArtifactTemplet nKMDiveArtifactTemplet = NKMDiveArtifactTemplet.Find(ID);
			if (nKMDiveArtifactTemplet != null)
			{
				return nKMDiveArtifactTemplet.ArtifactMiscDesc_1_Translated;
			}
			break;
		}
		case eSlotMode.ItemMisc:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(ID);
			if (itemMiscTempletByID != null)
			{
				return itemMiscTempletByID.GetItemDesc();
			}
			break;
		}
		case eSlotMode.Equip:
		case eSlotMode.EquipCount:
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(ID);
			if (equipTemplet != null)
			{
				return equipTemplet.GetItemDesc();
			}
			break;
		}
		case eSlotMode.Skin:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(ID);
			if (skinTemplet != null)
			{
				return skinTemplet.GetSkinDesc();
			}
			break;
		}
		case eSlotMode.Emoticon:
		{
			NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(ID);
			if (nKMEmoticonTemplet != null)
			{
				return nKMEmoticonTemplet.GetEmoticonDesc();
			}
			break;
		}
		case eSlotMode.GuildArtifact:
		{
			GuildDungeonArtifactTemplet artifactTemplet = GuildDungeonTempletManager.GetArtifactTemplet(ID);
			if (artifactTemplet != null)
			{
				if (!bFull)
				{
					return artifactTemplet.GetDescShort();
				}
				return artifactTemplet.GetDescFull();
			}
			break;
		}
		case eSlotMode.Buff:
		{
			NKMCompanyBuffTemplet nKMCompanyBuffTemplet = NKMCompanyBuffTemplet.Find(ID);
			if (nKMCompanyBuffTemplet != null)
			{
				return nKMCompanyBuffTemplet.GetBuffDescForItemPopup();
			}
			break;
		}
		}
		return "";
	}

	public void SetIconImage(Sprite sprite)
	{
		m_imgIcon.sprite = sprite;
	}

	public void SetHotkey(HotkeyEventType eventType)
	{
		NKCUtil.SetHotkey(m_cbtnButton, eventType);
	}
}
