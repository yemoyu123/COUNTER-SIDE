using System;
using System.Collections.Generic;
using System.Text;
using NKC.Templet;
using NKM;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComItemDropInfoSlot : MonoBehaviour
{
	public delegate void OnMove();

	public Text m_lbDropStage;

	public Text m_lbDropInfoDesc;

	public NKCUIComStateButton m_csbtnMove;

	public GameObject m_objMoveText;

	public GameObject m_objPurchaseText;

	private NKCAssetInstanceData m_InstanceData;

	private string m_LockMsg;

	private OnMove m_OnMove;

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnMove, OnClickMove);
		m_OnMove = null;
	}

	public void SetData(ItemDropInfo itemDropInfo)
	{
		if (itemDropInfo == null)
		{
			ResetUI();
			return;
		}
		m_LockMsg = "";
		switch (itemDropInfo.ContentType)
		{
		case DropContent.Stage:
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(itemDropInfo.ContentID);
			if (nKMStageTempletV == null)
			{
				ResetUI();
				break;
			}
			if (nKMStageTempletV.EpisodeTemplet != null)
			{
				ContentsType contentsType = NKCContentManager.GetContentsType(nKMStageTempletV.EpisodeTemplet.m_EPCategory);
				if (!NKCContentManager.IsContentsUnlocked(contentsType))
				{
					m_LockMsg = NKCContentManager.GetLockedMessage(contentsType);
				}
				else if (contentsType == ContentsType.DAILY)
				{
					switch (nKMStageTempletV.EpisodeId)
					{
					case 101:
						m_LockMsg = NKCContentManager.GetLockedMessage(ContentsType.DAILY_ATTACK);
						break;
					case 103:
						m_LockMsg = NKCContentManager.GetLockedMessage(ContentsType.DAILY_DEFENCE);
						break;
					case 102:
						m_LockMsg = NKCContentManager.GetLockedMessage(ContentsType.DAILY_SEARCH);
						break;
					}
				}
			}
			if (!NKMEpisodeMgr.CheckEpisodeMission(NKCScenManager.CurrentUserData(), nKMStageTempletV))
			{
				m_LockMsg = NKCUtilString.GetUnlockConditionRequireDesc(nKMStageTempletV);
			}
			if (string.IsNullOrEmpty(m_LockMsg) && !nKMStageTempletV.IsOpenedDayOfWeek())
			{
				m_LockMsg = NKCUtilString.GET_STRING_DAILY_CHECK_DAY;
			}
			SetUI(nKMStageTempletV, itemDropInfo.Summary);
			break;
		}
		case DropContent.Shop:
		{
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(itemDropInfo.ContentID);
			if (shopItemTemplet == null)
			{
				ResetUI();
			}
			else
			{
				SetUI(shopItemTemplet, itemDropInfo.Summary);
			}
			break;
		}
		case DropContent.WorldMapMission:
		{
			NKMWorldMapMissionTemplet nKMWorldMapMissionTemplet = NKMTempletContainer<NKMWorldMapMissionTemplet>.Find(itemDropInfo.ContentID);
			if (nKMWorldMapMissionTemplet == null)
			{
				ResetUI();
			}
			else
			{
				SetUI(nKMWorldMapMissionTemplet, itemDropInfo.Summary);
			}
			break;
		}
		case DropContent.Raid:
		{
			NKMRaidTemplet nKMRaidTemplet = NKMTempletContainer<NKMRaidTemplet>.Find(itemDropInfo.ContentID);
			if (nKMRaidTemplet == null)
			{
				ResetUI();
			}
			else
			{
				SetUI(nKMRaidTemplet, itemDropInfo.Summary);
			}
			break;
		}
		case DropContent.Shadow:
		{
			NKMShadowPalaceTemplet nKMShadowPalaceTemplet = NKMTempletContainer<NKMShadowPalaceTemplet>.Find(itemDropInfo.ContentID);
			if (nKMShadowPalaceTemplet == null)
			{
				ResetUI();
			}
			else
			{
				SetUI(nKMShadowPalaceTemplet, itemDropInfo.Summary);
			}
			break;
		}
		case DropContent.Dive:
		{
			NKMDiveTemplet nKMDiveTemplet = NKMTempletContainer<NKMDiveTemplet>.Find(itemDropInfo.ContentID);
			if (nKMDiveTemplet == null)
			{
				ResetUI();
			}
			else
			{
				SetUI(nKMDiveTemplet, itemDropInfo.Summary);
			}
			break;
		}
		case DropContent.Fierce:
		{
			NKMFiercePointRewardTemplet nKMFiercePointRewardTemplet = NKMTempletContainer<NKMFiercePointRewardTemplet>.Find(itemDropInfo.ContentID);
			if (nKMFiercePointRewardTemplet == null)
			{
				ResetUI();
			}
			else
			{
				SetUI(nKMFiercePointRewardTemplet, itemDropInfo.Summary);
			}
			break;
		}
		case DropContent.RandomMoldBox:
			SetRandomMoldBoxUI(itemDropInfo.ItemID);
			break;
		case DropContent.UnitDismiss:
			SetUnitDismissUI();
			break;
		case DropContent.UnitExtract:
			SetUnitExtractUI();
			break;
		case DropContent.Trim:
			SetTrimRewardUI(itemDropInfo.ContentID);
			break;
		case DropContent.SubStreamShop:
			SetStageShopUI(itemDropInfo);
			break;
		default:
			ResetUI();
			break;
		}
	}

	public static NKCUIComItemDropInfoSlot GetNewInstance(Transform parent, bool bMentoringSlot = false)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_ITEM_DROP_SLOT");
		NKCUIComItemDropInfoSlot nKCUIComItemDropInfoSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUIComItemDropInfoSlot>();
		if (nKCUIComItemDropInfoSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKM_UI_POPUP_COLLECTION_ACHIEVEMENT_SLOT Prefab null!");
			return null;
		}
		nKCUIComItemDropInfoSlot.m_InstanceData = nKCAssetInstanceData;
		nKCUIComItemDropInfoSlot.Init();
		if (parent != null)
		{
			nKCUIComItemDropInfoSlot.transform.SetParent(parent);
		}
		nKCUIComItemDropInfoSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUIComItemDropInfoSlot.gameObject.SetActive(value: false);
		return nKCUIComItemDropInfoSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		m_OnMove = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void SetPurchaseTextState(bool isPurchaseButton)
	{
		NKCUtil.SetGameobjectActive(m_objMoveText, !isPurchaseButton);
		NKCUtil.SetGameobjectActive(m_objPurchaseText, isPurchaseButton);
	}

	private void SetUI(NKMStageTempletV2 stageTemplet, bool summary)
	{
		if (!summary)
		{
			if (stageTemplet.EpisodeTemplet != null)
			{
				string episodeCategoryEx = NKCUtilString.GetEpisodeCategoryEx1(stageTemplet.EpisodeTemplet.m_EPCategory);
				switch (stageTemplet.EpisodeTemplet.m_EPCategory)
				{
				case EPISODE_CATEGORY.EC_MAINSTREAM:
				{
					string text2 = ((stageTemplet.m_Difficulty == EPISODE_DIFFICULTY.NORMAL) ? "" : ("[" + NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_ADDON_HARD") + "]"));
					string msg2 = $"{episodeCategoryEx} {stageTemplet.EpisodeTemplet.GetEpisodeTitle()}-{stageTemplet.ActId}-{stageTemplet.m_StageUINum} {text2}";
					NKCUtil.SetLabelText(m_lbDropStage, msg2);
					break;
				}
				case EPISODE_CATEGORY.EC_EVENT:
				case EPISODE_CATEGORY.EC_SEASONAL:
				{
					string text = ((stageTemplet.m_Difficulty == EPISODE_DIFFICULTY.NORMAL) ? "" : ("[" + NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_ADDON_HARD") + "]"));
					string msg = $"{episodeCategoryEx} {stageTemplet.EpisodeTemplet.GetEpisodeTitle()}-{stageTemplet.ActId}-{stageTemplet.m_StageUINum} {text}";
					NKCUtil.SetLabelText(m_lbDropStage, msg);
					break;
				}
				default:
					NKCUtil.SetLabelText(m_lbDropStage, episodeCategoryEx);
					break;
				}
			}
			else
			{
				NKCUtil.SetLabelText(m_lbDropStage, NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_DEFAULT"));
			}
			SetStageTempletShortCut(stageTemplet, setLabelDesc: true);
		}
		else
		{
			if (stageTemplet.EpisodeTemplet != null)
			{
				NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GetEpisodeCategoryEx1(stageTemplet.EpisodeCategory));
				SetStageTempletShortCut(stageTemplet, setLabelDesc: false);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_MENU_NAME_OPERATION_VIEWER);
				m_OnMove = delegate
				{
					NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OPERATION, "");
				};
			}
			NKCUtil.SetLabelText(m_lbDropInfoDesc, NKCStringTable.GetString("SI_ITEM_DROP_INFO_ALL_DROP"));
		}
		SetPurchaseTextState(isPurchaseButton: false);
	}

	private void SetUI(ShopItemTemplet shopItemTemplet, bool summary)
	{
		if (!summary)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append((shopItemTemplet.TabTemplet != null) ? shopItemTemplet.TabTemplet.GetTabName() : " - ");
			if (shopItemTemplet != null)
			{
				string text = "";
				int buyCountLeft = NKCShopManager.GetBuyCountLeft(shopItemTemplet.m_ProductID);
				text = ((!shopItemTemplet.TabTemplet.IsCountResetType) ? NKCShopManager.GetBuyCountString(shopItemTemplet.resetType, buyCountLeft, shopItemTemplet.m_QuantityLimit) : string.Format(NKCUtilString.GET_STRING_SHOP_PURCHASE_COUNT_TWO_PARAM, buyCountLeft, shopItemTemplet.m_QuantityLimit));
				stringBuilder.Append(" ");
				stringBuilder.Append(text);
			}
			NKCUtil.SetLabelText(m_lbDropInfoDesc, stringBuilder.ToString());
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_SHOP);
			bool isSupplyItem = shopItemTemplet.TabTemplet != null && shopItemTemplet.TabTemplet.m_TabName == "TAB_SUPPLY";
			m_OnMove = delegate
			{
				NKM_ERROR_CODE nKM_ERROR_CODE = NKCShopManager.OnBtnProductBuy(shopItemTemplet.m_ProductID, isSupplyItem);
				if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
				{
					NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
				}
			};
			SetPurchaseTextState(isPurchaseButton: true);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbDropInfoDesc, NKCUtilString.GET_STRING_SHOP);
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_SHOP);
			m_OnMove = delegate
			{
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_SHOP, "");
			};
			SetPurchaseTextState(isPurchaseButton: false);
		}
	}

	private void SetUI(NKMWorldMapMissionTemplet wmmTemplet, bool summary)
	{
		if (!summary)
		{
			string msg = ((wmmTemplet != null) ? wmmTemplet.GetMissionName() : " - ");
			NKCUtil.SetLabelText(m_lbDropInfoDesc, msg);
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_WORLDMAP);
			m_OnMove = delegate
			{
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_WORLDMAP_MISSION, "");
			};
		}
		else
		{
			NKCUtil.SetLabelText(m_lbDropInfoDesc, NKCStringTable.GetString("SI_DP_WORLDMAP_MISSION"));
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_WORLDMAP);
			m_OnMove = delegate
			{
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_WORLDMAP_MISSION, "");
			};
		}
		SetPurchaseTextState(isPurchaseButton: false);
	}

	private void SetUI(NKMRaidTemplet raidTemplet, bool summary)
	{
		if (!summary)
		{
			string msg = ((raidTemplet != null && raidTemplet.DungeonTempletBase != null) ? raidTemplet.DungeonTempletBase.GetDungeonName() : " - ");
			NKCUtil.SetLabelText(m_lbDropInfoDesc, msg);
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_RAID);
			m_OnMove = delegate
			{
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_WORLDMAP_MISSION, "");
			};
		}
		else
		{
			NKCUtil.SetLabelText(m_lbDropInfoDesc, NKCUtilString.GET_STRING_RAID);
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_WORLDMAP);
			m_OnMove = delegate
			{
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_WORLDMAP_MISSION, "");
			};
		}
		SetPurchaseTextState(isPurchaseButton: false);
	}

	private void SetUI(NKMShadowPalaceTemplet shadowPalaceTemplet, bool summary)
	{
		if (!summary)
		{
			string msg = ((shadowPalaceTemplet != null) ? shadowPalaceTemplet.PalaceName : " - ");
			NKCUtil.SetLabelText(m_lbDropInfoDesc, msg);
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_SHADOW_PALACE);
			m_OnMove = delegate
			{
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_SHADOW_PALACE, "");
			};
		}
		else
		{
			NKCUtil.SetLabelText(m_lbDropInfoDesc, " - ");
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_SHADOW_PALACE);
			m_OnMove = delegate
			{
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_SHADOW_PALACE, "");
			};
		}
		SetPurchaseTextState(isPurchaseButton: false);
	}

	private void SetUI(NKMDiveTemplet diveTemplet, bool summary)
	{
		if (!summary)
		{
			string msg = ((diveTemplet != null) ? diveTemplet.Get_STAGE_NAME() : " - ");
			NKCUtil.SetLabelText(m_lbDropInfoDesc, msg);
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_DIVE);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbDropInfoDesc, NKCUtilString.GET_STRING_DIVE);
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_WORLDMAP);
		}
		m_OnMove = delegate
		{
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.DIVE))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.DIVE);
			}
			else
			{
				UnlockInfo unlockInfo = new UnlockInfo(diveTemplet.StageUnlockReqType, diveTemplet.StageUnlockReqValue);
				if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in unlockInfo))
				{
					string unlockConditionRequireDesc = NKCUtilString.GetUnlockConditionRequireDesc(STAGE_UNLOCK_REQ_TYPE.SURT_DIVE_HISTORY_CLEARED, diveTemplet.StageUnlockReqValue, string.Empty, DateTime.MinValue);
					if (!string.IsNullOrEmpty(unlockConditionRequireDesc))
					{
						NKCPopupMessageManager.AddPopupMessage(unlockConditionRequireDesc);
					}
				}
				else
				{
					NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_DIVE, "");
				}
			}
		};
		SetPurchaseTextState(isPurchaseButton: false);
	}

	private void SetUI(NKMFiercePointRewardTemplet fprTemplet, bool summary)
	{
		if (!summary)
		{
			string msg = ((fprTemplet != null) ? NKCStringTable.GetString("SI_SHORTCUT_TITLE_FIERCE") : " - ");
			NKCUtil.SetLabelText(m_lbDropInfoDesc, msg);
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_FIERCE);
			m_OnMove = delegate
			{
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_FIERCE, "");
			};
		}
		else
		{
			NKCUtil.SetLabelText(m_lbDropInfoDesc, NKCStringTable.GetString("SI_PF_FIERCE_BATTLE_BUTTON_SCORE_REWARD_TEXT"));
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_FIERCE);
			m_OnMove = delegate
			{
				NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_FIERCE, "");
			};
		}
		SetPurchaseTextState(isPurchaseButton: false);
	}

	private void SetRandomMoldBoxUI(string itemID)
	{
		NKCUtil.SetLabelText(m_lbDropStage, NKCStringTable.GetString("SI_OFFICE_ROOM_NAME_TECH_FACTORY"));
		NKCUtil.SetLabelText(m_lbDropInfoDesc, NKCStringTable.GetString("SI_DP_FORGE_CRAFT_MOLD"));
		NKM_CRAFT_TAB_TYPE destTab = NKM_CRAFT_TAB_TYPE.MT_EQUIP;
		string[] names = Enum.GetNames(typeof(NKM_CRAFT_TAB_TYPE));
		int num = names.Length;
		for (int i = 0; i < num; i++)
		{
			NKM_CRAFT_TAB_TYPE result = NKM_CRAFT_TAB_TYPE.MT_EQUIP;
			if (Enum.TryParse<NKM_CRAFT_TAB_TYPE>(names[i], out result) && CheckCraftTabContainItem(result, itemID))
			{
				destTab = result;
				break;
			}
		}
		m_OnMove = delegate
		{
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_EQUIP_MAKE, destTab.ToString());
		};
		SetPurchaseTextState(isPurchaseButton: false);
	}

	private void SetUnitDismissUI()
	{
		NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_MANAGEMENT);
		NKCUtil.SetLabelText(m_lbDropInfoDesc, NKCUtilString.GET_STRING_REMOVE_UNIT);
		m_OnMove = delegate
		{
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_DISMISS, "");
		};
		SetPurchaseTextState(isPurchaseButton: false);
	}

	private void SetUnitExtractUI()
	{
		NKCUtil.SetLabelText(m_lbDropStage, NKCStringTable.GetString("SI_OFFICE_ROOM_NAME_TECH_LAB"));
		NKCUtil.SetLabelText(m_lbDropInfoDesc, NKCUtilString.GET_STRING_REARM_EXTRACT_TITLE);
		m_OnMove = delegate
		{
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OFFICE, "EXTRACT");
		};
		SetPurchaseTextState(isPurchaseButton: false);
	}

	private void SetTrimRewardUI(int trimId)
	{
		NKCUtil.SetLabelText(m_lbDropStage, NKCStringTable.GetString("SI_GUIDE_CATEGORY_TITLE_MANUAL_TRIM"));
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(trimId);
		string msg = ((nKMTrimTemplet != null) ? NKCStringTable.GetString(nKMTrimTemplet.TirmGroupName) : " - ");
		NKCUtil.SetLabelText(m_lbDropInfoDesc, msg);
		m_OnMove = delegate
		{
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_TRIM, "");
		};
		SetPurchaseTextState(isPurchaseButton: false);
	}

	private void SetStageShopUI(ItemDropInfo itemDropInfo)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(itemDropInfo.ContentID);
		if (shopItemTemplet != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append((shopItemTemplet.TabTemplet != null) ? shopItemTemplet.TabTemplet.GetTabName() : " - ");
			if (shopItemTemplet != null)
			{
				string text = "";
				int buyCountLeft = NKCShopManager.GetBuyCountLeft(shopItemTemplet.m_ProductID);
				text = ((!shopItemTemplet.TabTemplet.IsCountResetType) ? NKCShopManager.GetBuyCountString(shopItemTemplet.resetType, buyCountLeft, shopItemTemplet.m_QuantityLimit) : string.Format(NKCUtilString.GET_STRING_SHOP_PURCHASE_COUNT_TWO_PARAM, buyCountLeft, shopItemTemplet.m_QuantityLimit));
				stringBuilder.Append(" ");
				stringBuilder.Append(text);
			}
			NKCUtil.SetLabelText(m_lbDropStage, NKCUtilString.GET_STRING_SHOP);
			NKCUtil.SetLabelText(m_lbDropInfoDesc, stringBuilder.ToString());
			m_OnMove = delegate
			{
				NKM_ERROR_CODE nKM_ERROR_CODE = NKCShopManager.OnBtnProductBuy(shopItemTemplet.m_ProductID, bSupply: false);
				if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
				{
					NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
				}
			};
		}
		SetPurchaseTextState(isPurchaseButton: true);
	}

	private void SetStageTempletShortCut(NKMStageTempletV2 stageTemplet, bool setLabelDesc)
	{
		switch (stageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_WARFARE:
		{
			NKMWarfareTemplet warfareTemplet = NKMWarfareTemplet.Find(stageTemplet.m_StageBattleStrID);
			if (warfareTemplet != null)
			{
				if (setLabelDesc)
				{
					NKCUtil.SetLabelText(m_lbDropInfoDesc, warfareTemplet.GetWarfareName());
				}
				m_OnMove = delegate
				{
					NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_MAINSTREAM, warfareTemplet.m_WarfareID.ToString());
				};
				break;
			}
			if (setLabelDesc)
			{
				NKCUtil.SetLabelText(m_lbDropInfoDesc, " - ");
			}
			if (stageTemplet.EpisodeTemplet != null)
			{
				m_OnMove = delegate
				{
					NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OPERATION, stageTemplet.EpisodeCategory.ToString());
				};
			}
			else
			{
				m_OnMove = delegate
				{
					NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_MAINSTREAM, "");
				};
			}
			break;
		}
		case STAGE_TYPE.ST_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(stageTemplet.m_StageBattleStrID);
			if (dungeonTempletBase != null)
			{
				if (setLabelDesc)
				{
					NKCUtil.SetLabelText(m_lbDropInfoDesc, dungeonTempletBase.GetDungeonName());
				}
				m_OnMove = delegate
				{
					NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_DUNGEON, stageTemplet.Key.ToString());
				};
				break;
			}
			if (setLabelDesc)
			{
				NKCUtil.SetLabelText(m_lbDropInfoDesc, " - ");
			}
			if (stageTemplet.EpisodeTemplet != null)
			{
				m_OnMove = delegate
				{
					NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OPERATION, stageTemplet.EpisodeCategory.ToString());
				};
			}
			else
			{
				m_OnMove = delegate
				{
					NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_DUNGEON, "");
				};
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
		{
			NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(stageTemplet.m_StageBattleStrID);
			string paramException = ((stageTemplet.EpisodeTemplet != null) ? stageTemplet.EpisodeCategory.ToString() : "");
			NKM_SHORTCUT_TYPE shortCutTypeException = ((stageTemplet.EpisodeTemplet != null) ? NKM_SHORTCUT_TYPE.SHORTCUT_OPERATION : NKM_SHORTCUT_TYPE.SHORTCUT_DUNGEON);
			if (nKMPhaseTemplet != null)
			{
				if (setLabelDesc)
				{
					NKCUtil.SetLabelText(m_lbDropInfoDesc, nKMPhaseTemplet.GetName());
				}
				if (nKMPhaseTemplet.PhaseList != null && nKMPhaseTemplet.PhaseList.List.Count > 0)
				{
					if (nKMPhaseTemplet.PhaseList.List[0].Dungeon != null)
					{
						m_OnMove = delegate
						{
							NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_DUNGEON, stageTemplet.Key.ToString());
						};
					}
					else
					{
						m_OnMove = delegate
						{
							NKCContentManager.MoveToShortCut(shortCutTypeException, paramException);
						};
					}
				}
				else
				{
					m_OnMove = delegate
					{
						NKCContentManager.MoveToShortCut(shortCutTypeException, paramException);
					};
				}
			}
			else
			{
				if (setLabelDesc)
				{
					NKCUtil.SetLabelText(m_lbDropInfoDesc, " - ");
				}
				m_OnMove = delegate
				{
					NKCContentManager.MoveToShortCut(shortCutTypeException, paramException);
				};
			}
			break;
		}
		default:
			NKCUtil.SetLabelText(m_lbDropInfoDesc, "");
			break;
		}
	}

	private bool CheckCraftTabContainItem(NKM_CRAFT_TAB_TYPE craftTabType, string itemStrID)
	{
		List<NKMMoldItemData> moldItemData = NKMItemManager.GetMoldItemData(craftTabType);
		int count = moldItemData.Count;
		for (int i = 0; i < count; i++)
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(moldItemData[i].m_MoldID);
			if (itemMoldTempletByID == null || !NKMItemManager.m_dicRandomMoldBox.TryGetValue(itemMoldTempletByID.m_RewardGroupID, out var value))
			{
				continue;
			}
			foreach (KeyValuePair<NKM_REWARD_TYPE, List<int>> item in value)
			{
				int num = 0;
				if (item.Key == NKM_REWARD_TYPE.RT_EQUIP)
				{
					NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemStrID);
					if (equipTemplet == null)
					{
						continue;
					}
					num = equipTemplet.m_ItemEquipID;
				}
				else
				{
					NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(itemStrID);
					if (nKMItemMiscTemplet == null)
					{
						continue;
					}
					num = nKMItemMiscTemplet.m_ItemMiscID;
				}
				if (item.Value.Contains(num))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ResetUI()
	{
		NKCUtil.SetLabelText(m_lbDropStage, "");
		NKCUtil.SetLabelText(m_lbDropInfoDesc, "");
		SetPurchaseTextState(isPurchaseButton: false);
		m_OnMove = null;
	}

	private void OnClickMove()
	{
		if (!string.IsNullOrEmpty(m_LockMsg))
		{
			NKCPopupMessageManager.AddPopupMessage(m_LockMsg);
		}
		else if (m_OnMove != null)
		{
			m_OnMove();
		}
	}
}
