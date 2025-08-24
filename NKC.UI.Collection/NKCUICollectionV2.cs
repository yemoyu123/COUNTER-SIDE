using System.Collections.Generic;
using ClientPacket.Community;
using ClientPacket.User;
using Cs.Logging;
using NKC.Templet;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Collection;

public class NKCUICollectionV2 : NKCUICollectionGeneral
{
	public delegate void OnNotify(bool bNotify);

	public delegate void OnSyncCollectingData(CollectionType type, int iCur, int iTotal);

	public delegate void OnStoryCutscen(bool bPlay);

	public const string ASSET_BUNDLE_NAME = "ab_ui_collection";

	public const string UI_ASSET_NAME = "AB_UI_COLLECTION";

	private CollectionType m_eCollectionType = CollectionType.CT_NONE;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffdġ")]
	public RectTransform m_rtNKM_UI_COLLECTION_CONTENT;

	[Header("\ufffd\u07b4\ufffd")]
	public Transform m_menuRoot;

	public NKCUIComToggleGroup m_toggleGroup;

	public NKCUICollectionMenuSlot m_prfCollectionMenuSlot;

	public NKCUICollectionMenuSubSlot m_prfCollectionMenuSubSlot;

	private Dictionary<CollectionType, ICollectionMenuButton> m_toggleMenu = new Dictionary<CollectionType, ICollectionMenuButton>();

	private CollectionType m_firstType = CollectionType.CT_NONE;

	private const string ASSET_BUNDLE_NAME_COLLECTION = "ab_ui_nkm_ui_collection";

	private const string ASSET_BUNDLE_NAME_COLLECTION_V2 = "ab_ui_collection";

	private const string UI_ASSET_NAME_TEAMUP = "AB_UI_COLLECTION_TEAMUP";

	private NKCUICollectionTeamUp m_NKCUICollectionTeamUp;

	private const string UI_ASSET_NAME_UNIT = "AB_UI_COLLECTION_UNIT";

	private NKCUICollectionUnitListV2 m_NKCUICollectionUnit;

	private const string UI_ASSET_NAME_OPERATOR = "AB_UI_COLLECTION_OPERATOR";

	private NKCUICollectionOperatorList m_NKCUICollectionOperator;

	private const string UI_ASSET_NAME_SHIP = "AB_UI_COLLECTION_SHIP";

	private NKCUICollectionUnitListV2 m_NKCUICollectionShip;

	private const string UI_ASSET_NAME_ILLUST = "NKM_UI_COLLECTION_ILLUST";

	private NKCUICollectionIllust m_NKCUICollectionIllust;

	private const string UI_ASSET_NAME_STORY = "AB_UI_COLLECTION_STORY";

	private NKCUICollectionStory m_NKCUICollectionStory;

	private const string UI_ASSET_NAME_EMBLEM = "AB_UI_COLLECTION_EMBLEM";

	private NKCUICollectionMisc m_NKCUICollectionEmblem;

	private const string UI_ASSET_NAME_FRAME = "AB_UI_COLLECTION_FRAME";

	private NKCUICollectionMisc m_NKCUICollectionFrame;

	private const string UI_ASSET_NAME_BACKGROUND = "AB_UI_COLLECTION_BACKGROUND";

	private NKCUICollectionMisc m_NKCUICollectionBackground;

	private List<NKCAssetInstanceData> m_lstAssetInstance = new List<NKCAssetInstanceData>();

	private List<int> m_lstUpdateUnitMissionUnitId = new List<int>();

	private bool m_bDataValid;

	public override string MenuName => NKCUtilString.GET_STRING_COLLECTION;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID => "ARTICLE_SYSTEM_COLLETION";

	public override void OnBackButton()
	{
		if (NKCUICutScenPlayer.IsInstanceOpen && NKCUICutScenPlayer.Instance.IsPlaying())
		{
			NKCUICutScenPlayer.Instance.StopWithCallBack();
			return;
		}
		base.OnBackButton();
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void UnHide()
	{
		if (!m_bDataValid)
		{
			m_bDataValid = true;
		}
		base.UnHide();
		if (m_lstUpdateUnitMissionUnitId.Count > 0)
		{
			if (m_NKCUICollectionUnit != null && m_NKCUICollectionUnit.gameObject.activeSelf)
			{
				m_NKCUICollectionUnit.UpdateCollectionMissionRate(m_lstUpdateUnitMissionUnitId);
			}
			m_lstUpdateUnitMissionUnitId.Clear();
		}
		if (m_toggleMenu.ContainsKey(CollectionType.CT_UNIT))
		{
			m_toggleMenu[CollectionType.CT_UNIT].SetRedDotActive(NKCUnitMissionManager.HasRewardEnableMission());
		}
		if (m_NKCUICollectionUnit != null && m_NKCUICollectionUnit.gameObject.activeSelf)
		{
			m_NKCUICollectionUnit.CheckRewardToggle();
		}
		switch (m_eCollectionType)
		{
		case CollectionType.CT_EMBLEM:
			m_NKCUICollectionEmblem.RefreshScrollRect();
			break;
		case CollectionType.CT_FRAME:
			m_NKCUICollectionFrame.RefreshScrollRect();
			break;
		case CollectionType.CT_BACKGROUND:
			m_NKCUICollectionBackground.RefreshScrollRect();
			break;
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		CloseAllInstance();
	}

	public override NKM_SHORTCUT_TYPE GetShortcutType()
	{
		return m_eCollectionType switch
		{
			CollectionType.CT_TEAM_UP => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_TEAMUP, 
			CollectionType.CT_UNIT => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_UNIT, 
			CollectionType.CT_SHIP => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_SHIP, 
			CollectionType.CT_OPERATOR => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_OPERATOR, 
			CollectionType.CT_ILLUST => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_ILLUST, 
			CollectionType.CT_STORY => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_STORY, 
			_ => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION, 
		};
	}

	public override void Init()
	{
		InitButton();
	}

	private void InitButton()
	{
		if (m_menuRoot != null)
		{
			int childCount = m_menuRoot.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(m_menuRoot.GetChild(i).gameObject);
			}
		}
		m_firstType = CollectionType.CT_NONE;
		NKCUICollectionMenuSlot nKCUICollectionMenuSlot = null;
		foreach (NKCCollectionMenuTemplet templet in NKCCollectionManager.GetCollectionMenuData().Values)
		{
			if (templet == null || !NKMOpenTagManager.IsOpened(templet.OpenTag))
			{
				continue;
			}
			string text = NKCStringTable.GetString(templet.TabDescStrID);
			if (templet.UpperTab)
			{
				nKCUICollectionMenuSlot = null;
				Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(new NKMAssetName("ab_ui_collection_sprite", templet.TabIconStrID));
				nKCUICollectionMenuSlot = CreateMenuSlot(text, orLoadAssetResource);
				if (!string.IsNullOrEmpty(templet.TabPrefabID))
				{
					nKCUICollectionMenuSlot.SetCallBackFunction(delegate
					{
						ChangeState(templet.TabType);
					});
					if (m_toggleMenu.ContainsKey(templet.TabType))
					{
						Log.Error($"{templet.TabType} type toggle is repeated", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/CollectionV2/NKCUICollectionV2.cs", 175);
					}
					else
					{
						m_toggleMenu.Add(templet.TabType, nKCUICollectionMenuSlot);
					}
					if (m_firstType == CollectionType.CT_NONE)
					{
						m_firstType = templet.TabType;
					}
				}
				continue;
			}
			NKCUICollectionMenuSubSlot nKCUICollectionMenuSubSlot = CreateMenuSubSlot(text);
			nKCUICollectionMenuSlot?.AddSubSlot(nKCUICollectionMenuSubSlot);
			if (nKCUICollectionMenuSlot != null && !string.IsNullOrEmpty(templet.TabPrefabID))
			{
				nKCUICollectionMenuSubSlot.SetOnClickFunc(delegate
				{
					ChangeState(templet.TabType);
				});
				if (m_toggleMenu.ContainsKey(templet.TabType))
				{
					Log.Error($"{templet.TabType} type toggle is repeated", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/CollectionV2/NKCUICollectionV2.cs", 193);
				}
				else
				{
					m_toggleMenu.Add(templet.TabType, nKCUICollectionMenuSubSlot);
				}
				if (m_firstType == CollectionType.CT_NONE)
				{
					m_firstType = templet.TabType;
				}
			}
		}
	}

	private NKCUICollectionMenuSlot CreateMenuSlot(string name, Sprite icon)
	{
		NKCUICollectionMenuSlot nKCUICollectionMenuSlot = Object.Instantiate(m_prfCollectionMenuSlot);
		NKCUtil.SetGameobjectActive(nKCUICollectionMenuSlot, bValue: true);
		nKCUICollectionMenuSlot.Init(m_toggleGroup, name, icon);
		nKCUICollectionMenuSlot.transform.SetParent(m_menuRoot);
		nKCUICollectionMenuSlot.transform.localScale = Vector3.one;
		return nKCUICollectionMenuSlot;
	}

	private NKCUICollectionMenuSubSlot CreateMenuSubSlot(string name)
	{
		NKCUICollectionMenuSubSlot nKCUICollectionMenuSubSlot = Object.Instantiate(m_prfCollectionMenuSubSlot);
		NKCUtil.SetGameobjectActive(nKCUICollectionMenuSubSlot, bValue: true);
		nKCUICollectionMenuSubSlot.Init(name);
		nKCUICollectionMenuSubSlot.transform.SetParent(m_menuRoot);
		nKCUICollectionMenuSubSlot.transform.localScale = Vector3.one;
		return nKCUICollectionMenuSubSlot;
	}

	public override void Open(CollectionType reserveType = CollectionType.CT_NONE, string reserveUnitStrID = "")
	{
		if (reserveType == CollectionType.CT_NONE)
		{
			ToggleSelect(m_firstType);
		}
		else
		{
			if (!NKCOperatorUtil.IsActive() && reserveType == CollectionType.CT_OPERATOR)
			{
				reserveType = m_firstType;
			}
			ToggleSelect(reserveType, !string.IsNullOrEmpty(reserveUnitStrID));
		}
		UIOpened();
		if (!string.IsNullOrEmpty(reserveUnitStrID))
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(reserveUnitStrID);
			if (unitTempletBase != null)
			{
				switch (unitTempletBase.m_NKM_UNIT_TYPE)
				{
				default:
					NKCUICollectionUnitInfo.CheckInstanceAndOpen(NKCUtil.MakeDummyUnit(unitTempletBase.m_UnitID, 100, 3), null);
					break;
				case NKM_UNIT_TYPE.NUT_SHIP:
					NKCUICollectionShipInfo.CheckInstanceAndOpen(NKCUtil.MakeDummyUnit(unitTempletBase.m_UnitID, 100, 3), NKMDeckIndex.None);
					break;
				case NKM_UNIT_TYPE.NUT_OPERATOR:
					NKCUICollectionOperatorInfo.Instance.Open(NKCOperatorUtil.GetDummyOperator(unitTempletBase, bSetMaximum: true));
					break;
				}
			}
		}
		if (m_toggleMenu.ContainsKey(CollectionType.CT_UNIT))
		{
			m_toggleMenu[CollectionType.CT_UNIT].SetRedDotActive(NKCUnitMissionManager.HasRewardEnableMission());
		}
		if (m_toggleMenu.ContainsKey(CollectionType.CT_EMBLEM))
		{
			m_toggleMenu[CollectionType.CT_EMBLEM].SetRedDotActive(NKCCollectionManager.IsMiscRewardEnable(NKM_ITEM_MISC_TYPE.IMT_EMBLEM));
		}
		if (m_toggleMenu.ContainsKey(CollectionType.CT_FRAME))
		{
			m_toggleMenu[CollectionType.CT_FRAME].SetRedDotActive(NKCCollectionManager.IsMiscRewardEnable(NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME));
		}
		if (m_toggleMenu.ContainsKey(CollectionType.CT_BACKGROUND))
		{
			m_toggleMenu[CollectionType.CT_BACKGROUND].SetRedDotActive(NKCCollectionManager.IsMiscRewardEnable(NKM_ITEM_MISC_TYPE.IMT_BACKGROUND));
		}
	}

	private void ToggleSelect(CollectionType collectionType, bool skipSubButtonAni = false)
	{
		if (m_toggleMenu.ContainsKey(collectionType))
		{
			NKCUIComToggle nKCUIComToggle = m_toggleMenu[collectionType].GetToggle().m_ToggleGroup?.GetComponent<NKCUIComToggle>();
			if (nKCUIComToggle != null)
			{
				nKCUIComToggle.Select(bSelect: true, bForce: true);
				nKCUIComToggle.GetComponent<NKCUICollectionMenuSlot>()?.OnActiveChild(bActive: true, skipSubButtonAni);
			}
			m_toggleMenu[collectionType].GetToggle().Select(bSelect: true);
		}
	}

	private void CloseAllInstance()
	{
		if (null != m_NKCUICollectionUnit)
		{
			m_NKCUICollectionUnit.Clear();
			m_NKCUICollectionUnit = null;
		}
		if (null != m_NKCUICollectionShip)
		{
			m_NKCUICollectionShip.Clear();
			m_NKCUICollectionShip = null;
		}
		if (null != m_NKCUICollectionOperator)
		{
			m_NKCUICollectionOperator.Clear();
			m_NKCUICollectionOperator = null;
		}
		if (null != m_NKCUICollectionTeamUp)
		{
			m_NKCUICollectionTeamUp.Clear();
			m_NKCUICollectionTeamUp = null;
		}
		if (null != m_NKCUICollectionIllust)
		{
			m_NKCUICollectionIllust.Clear();
			m_NKCUICollectionIllust = null;
		}
		if (null != m_NKCUICollectionStory)
		{
			m_NKCUICollectionStory.Clear();
			m_NKCUICollectionStory = null;
		}
		if (null != m_NKCUICollectionEmblem)
		{
			m_NKCUICollectionEmblem.Clear();
			m_NKCUICollectionEmblem = null;
		}
		if (null != m_NKCUICollectionFrame)
		{
			m_NKCUICollectionFrame.Clear();
			m_NKCUICollectionFrame = null;
		}
		if (m_lstUpdateUnitMissionUnitId != null)
		{
			m_lstUpdateUnitMissionUnitId.Clear();
			m_lstUpdateUnitMissionUnitId = null;
		}
		for (int i = 0; i < m_lstAssetInstance.Count; i++)
		{
			m_lstAssetInstance[i].Unload();
		}
		m_toggleMenu.Clear();
		m_eCollectionType = CollectionType.CT_NONE;
		Object.Destroy(base.gameObject);
		NKCAssetResourceManager.CloseResource("ab_ui_collection", "AB_UI_COLLECTION");
	}

	private void ChangeState(CollectionType type)
	{
		if (m_eCollectionType == type)
		{
			Debug.Log("already loaded type " + type);
			return;
		}
		PreUIUnHide();
		OpenUI(type);
		m_eCollectionType = type;
	}

	private void OpenUI(CollectionType type)
	{
		switch (type)
		{
		case CollectionType.CT_TEAM_UP:
			if (null == m_NKCUICollectionTeamUp)
			{
				NKCAssetInstanceData AssetInstance8 = null;
				LoadInstance(ref AssetInstance8, ref m_NKCUICollectionTeamUp, "ab_ui_collection", "AB_UI_COLLECTION_TEAMUP");
				m_NKCUICollectionTeamUp.Init(SyncCollectingData, NotifyTeamup);
				m_NKCUICollectionTeamUp.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				m_lstAssetInstance.Add(AssetInstance8);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionTeamUp.gameObject, bValue: true);
			m_NKCUICollectionTeamUp.Open();
			break;
		case CollectionType.CT_UNIT:
			if (null == m_NKCUICollectionUnit)
			{
				NKCAssetInstanceData AssetInstance2 = null;
				LoadInstance(ref AssetInstance2, ref m_NKCUICollectionUnit, "ab_ui_collection", "AB_UI_COLLECTION_UNIT");
				m_NKCUICollectionUnit.Init(SyncCollectingData);
				m_NKCUICollectionUnit.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				m_lstAssetInstance.Add(AssetInstance2);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionUnit.gameObject, bValue: true);
			m_NKCUICollectionUnit.Open();
			break;
		case CollectionType.CT_OPERATOR:
			if (null == m_NKCUICollectionOperator)
			{
				NKCAssetInstanceData AssetInstance9 = null;
				LoadInstance(ref AssetInstance9, ref m_NKCUICollectionOperator, "ab_ui_collection", "AB_UI_COLLECTION_OPERATOR");
				m_NKCUICollectionOperator.Init(SyncCollectingData);
				m_NKCUICollectionOperator.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				m_lstAssetInstance.Add(AssetInstance9);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionOperator.gameObject, bValue: true);
			m_NKCUICollectionOperator.Open();
			break;
		case CollectionType.CT_SHIP:
			if (null == m_NKCUICollectionShip)
			{
				NKCAssetInstanceData AssetInstance6 = null;
				LoadInstance(ref AssetInstance6, ref m_NKCUICollectionShip, "ab_ui_collection", "AB_UI_COLLECTION_SHIP");
				m_NKCUICollectionShip.Init(SyncCollectingData);
				m_NKCUICollectionShip.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				m_lstAssetInstance.Add(AssetInstance6);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionShip.gameObject, bValue: true);
			m_NKCUICollectionShip.Open();
			break;
		case CollectionType.CT_ILLUST:
			if (null == m_NKCUICollectionIllust)
			{
				NKCAssetInstanceData AssetInstance5 = null;
				LoadInstance(ref AssetInstance5, ref m_NKCUICollectionIllust, "ab_ui_nkm_ui_collection", "NKM_UI_COLLECTION_ILLUST");
				m_NKCUICollectionIllust.Init(SyncCollectingData);
				m_NKCUICollectionIllust.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				m_lstAssetInstance.Add(AssetInstance5);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionIllust.gameObject, bValue: true);
			m_NKCUICollectionIllust.Open();
			break;
		case CollectionType.CT_STORY:
			if (null == m_NKCUICollectionStory)
			{
				NKCAssetInstanceData AssetInstance3 = null;
				LoadInstance(ref AssetInstance3, ref m_NKCUICollectionStory, "ab_ui_collection", "AB_UI_COLLECTION_STORY");
				m_NKCUICollectionStory.Init(SyncCollectingData, StoryCutscen);
				m_NKCUICollectionStory.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				SetupScrollRects(m_NKCUICollectionStory.gameObject);
				m_lstAssetInstance.Add(AssetInstance3);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionStory.gameObject, bValue: true);
			m_NKCUICollectionStory.Open();
			break;
		case CollectionType.CT_EMBLEM:
			if (null == m_NKCUICollectionEmblem)
			{
				NKCAssetInstanceData AssetInstance7 = null;
				LoadInstance(ref AssetInstance7, ref m_NKCUICollectionEmblem, "ab_ui_collection", "AB_UI_COLLECTION_EMBLEM");
				m_NKCUICollectionEmblem.Init(NKM_ITEM_MISC_TYPE.IMT_EMBLEM, CollectionType.CT_EMBLEM);
				m_NKCUICollectionEmblem.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				SetupScrollRects(m_NKCUICollectionEmblem.gameObject);
				m_lstAssetInstance.Add(AssetInstance7);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionEmblem.gameObject, bValue: true);
			m_NKCUICollectionEmblem.Open();
			break;
		case CollectionType.CT_FRAME:
			if (null == m_NKCUICollectionFrame)
			{
				NKCAssetInstanceData AssetInstance4 = null;
				LoadInstance(ref AssetInstance4, ref m_NKCUICollectionFrame, "ab_ui_collection", "AB_UI_COLLECTION_FRAME");
				m_NKCUICollectionFrame.Init(NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME, CollectionType.CT_FRAME);
				m_NKCUICollectionFrame.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				SetupScrollRects(m_NKCUICollectionFrame.gameObject);
				m_lstAssetInstance.Add(AssetInstance4);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionFrame.gameObject, bValue: true);
			m_NKCUICollectionFrame.Open();
			break;
		case CollectionType.CT_BACKGROUND:
			if (null == m_NKCUICollectionBackground)
			{
				NKCAssetInstanceData AssetInstance = null;
				LoadInstance(ref AssetInstance, ref m_NKCUICollectionBackground, "ab_ui_collection", "AB_UI_COLLECTION_BACKGROUND");
				m_NKCUICollectionBackground.Init(NKM_ITEM_MISC_TYPE.IMT_BACKGROUND, CollectionType.CT_BACKGROUND);
				m_NKCUICollectionBackground.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				SetupScrollRects(m_NKCUICollectionBackground.gameObject);
				m_lstAssetInstance.Add(AssetInstance);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionBackground.gameObject, bValue: true);
			m_NKCUICollectionBackground.Open();
			break;
		}
	}

	public void NotifyTeamup(bool bNotify)
	{
		if (m_toggleMenu.ContainsKey(CollectionType.CT_TEAM_UP))
		{
			m_toggleMenu[CollectionType.CT_TEAM_UP].SetRedDotActive(bNotify);
		}
	}

	public void SyncCollectingData(CollectionType type, int iCur, int iTotal)
	{
	}

	private void LoadInstance<T>(ref NKCAssetInstanceData AssetInstance, ref T script, string AssetBundleName, string AssetName)
	{
		AssetInstance = NKCAssetResourceManager.OpenInstance<GameObject>(AssetBundleName, AssetName);
		if (AssetInstance.m_Instant != null)
		{
			AssetInstance.m_Instant.transform.SetParent(base.gameObject.transform, worldPositionStays: false);
			script = AssetInstance.m_Instant.GetComponent<T>();
			return;
		}
		Debug.LogError("Load Faile " + typeof(T).ToString() + ", path" + AssetBundleName.ToString() + ", name" + AssetName.ToString());
	}

	private void PreUIUnHide()
	{
		if (m_eCollectionType != CollectionType.CT_NONE)
		{
			switch (m_eCollectionType)
			{
			case CollectionType.CT_TEAM_UP:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionTeamUp.gameObject, bValue: false);
				break;
			case CollectionType.CT_UNIT:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionUnit.gameObject, bValue: false);
				break;
			case CollectionType.CT_SHIP:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionShip.gameObject, bValue: false);
				break;
			case CollectionType.CT_OPERATOR:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionOperator.gameObject, bValue: false);
				break;
			case CollectionType.CT_ILLUST:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionIllust.gameObject, bValue: false);
				break;
			case CollectionType.CT_STORY:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionStory.gameObject, bValue: false);
				break;
			case CollectionType.CT_EMBLEM:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionEmblem.gameObject, bValue: false);
				break;
			case CollectionType.CT_FRAME:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionFrame.gameObject, bValue: false);
				break;
			case CollectionType.CT_BACKGROUND:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionBackground.gameObject, bValue: false);
				break;
			}
		}
	}

	public override void OnRecvReviewTagVoteCancelAck(NKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_ACK sPacket)
	{
		NKCUICollectionUnitInfo.Instance.OnRecvReviewTagVoteCancelAck(sPacket);
	}

	public override void OnRecvReviewTagVoteAck(NKMPacket_UNIT_REVIEW_TAG_VOTE_ACK sPacket)
	{
		NKCUICollectionUnitInfo.Instance.OnRecvReviewTagVoteAck(sPacket);
	}

	public override void OnRecvReviewTagListAck(NKMPacket_UNIT_REVIEW_TAG_LIST_ACK sPacket)
	{
		NKCUICollectionUnitInfo.Instance.OnRecvReviewTagListAck(sPacket);
	}

	public override void OnRecvTeamCollectionRewardAck(NKMPacket_TEAM_COLLECTION_REWARD_ACK sPacket)
	{
		m_NKCUICollectionTeamUp.OnRecvTeamCollectionRewardAck(sPacket);
	}

	public override void OnRecvUnitMissionReward(int unitId)
	{
		m_lstUpdateUnitMissionUnitId.Add(unitId);
	}

	public void OnRecvMiscCollectionReward()
	{
		switch (m_eCollectionType)
		{
		case CollectionType.CT_EMBLEM:
			m_NKCUICollectionEmblem?.UpdateReward();
			if (m_toggleMenu.ContainsKey(m_eCollectionType))
			{
				m_toggleMenu[m_eCollectionType].SetRedDotActive(NKCCollectionManager.IsMiscRewardEnable(NKM_ITEM_MISC_TYPE.IMT_EMBLEM));
			}
			break;
		case CollectionType.CT_FRAME:
			m_NKCUICollectionFrame?.UpdateReward();
			if (m_toggleMenu.ContainsKey(m_eCollectionType))
			{
				m_toggleMenu[m_eCollectionType].SetRedDotActive(NKCCollectionManager.IsMiscRewardEnable(NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME));
			}
			break;
		case CollectionType.CT_BACKGROUND:
			m_NKCUICollectionBackground?.UpdateReward();
			if (m_toggleMenu.ContainsKey(m_eCollectionType))
			{
				m_toggleMenu[m_eCollectionType].SetRedDotActive(NKCCollectionManager.IsMiscRewardEnable(NKM_ITEM_MISC_TYPE.IMT_BACKGROUND));
			}
			break;
		}
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		if (eEventType == NKMUserData.eChangeNotifyType.Add)
		{
			if (m_bHide)
			{
				m_bDataValid = false;
			}
			else
			{
				UpdateCollection();
			}
		}
	}

	private void UpdateCollection()
	{
		switch (m_eCollectionType)
		{
		case CollectionType.CT_TEAM_UP:
			m_NKCUICollectionTeamUp.Open();
			break;
		case CollectionType.CT_UNIT:
			m_NKCUICollectionUnit.Open();
			break;
		case CollectionType.CT_SHIP:
			m_NKCUICollectionShip.Open();
			break;
		case CollectionType.CT_STORY:
			m_NKCUICollectionStory.Open();
			break;
		case CollectionType.CT_OPERATOR:
			m_NKCUICollectionOperator.Open();
			break;
		default:
			Debug.Log("Can not fount collection type : " + m_eCollectionType);
			break;
		}
	}

	private void UpdateMiscCollection()
	{
		switch (m_eCollectionType)
		{
		case CollectionType.CT_EMBLEM:
			if (m_NKCUICollectionEmblem != null)
			{
				m_NKCUICollectionEmblem.UpdateList();
				m_NKCUICollectionEmblem.UpdateReward();
			}
			if (m_toggleMenu.ContainsKey(m_eCollectionType))
			{
				m_toggleMenu[m_eCollectionType].SetRedDotActive(NKCCollectionManager.IsMiscRewardEnable(NKM_ITEM_MISC_TYPE.IMT_EMBLEM));
			}
			break;
		case CollectionType.CT_FRAME:
			if (m_NKCUICollectionFrame != null)
			{
				m_NKCUICollectionFrame.UpdateList();
				m_NKCUICollectionFrame.UpdateReward();
			}
			if (m_toggleMenu.ContainsKey(m_eCollectionType))
			{
				m_toggleMenu[m_eCollectionType].SetRedDotActive(NKCCollectionManager.IsMiscRewardEnable(NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME));
			}
			break;
		case CollectionType.CT_BACKGROUND:
			if (m_NKCUICollectionBackground != null)
			{
				m_NKCUICollectionBackground.UpdateList();
				m_NKCUICollectionBackground.UpdateReward();
			}
			if (m_toggleMenu.ContainsKey(m_eCollectionType))
			{
				m_toggleMenu[m_eCollectionType].SetRedDotActive(NKCCollectionManager.IsMiscRewardEnable(NKM_ITEM_MISC_TYPE.IMT_BACKGROUND));
			}
			break;
		}
	}

	public void StoryCutscen(bool bPlay)
	{
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (itemData != null)
		{
			NKMCollectionV2MiscTemplet nKMCollectionV2MiscTemplet = NKMCollectionV2MiscTemplet.Find(itemData.ItemID);
			if (nKMCollectionV2MiscTemplet != null && nKMCollectionV2MiscTemplet.CollectionMiscTemplet != null && GetCollectionTypeFromMiscItem(nKMCollectionV2MiscTemplet.CollectionMiscTemplet.m_ItemMiscType) == m_eCollectionType)
			{
				UpdateMiscCollection();
			}
		}
	}
}
