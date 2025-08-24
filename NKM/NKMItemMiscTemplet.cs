using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs.Logging;
using NKC;
using NKM.Contract2;
using NKM.Item;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Templet.Office;

namespace NKM;

public class NKMItemMiscTemplet : INKMTemplet
{
	private readonly List<int> customRewardGroupId = new List<int>();

	private readonly List<NKMCustomPackageGroupTemplet> customRewardTemplets = new List<NKMCustomPackageGroupTemplet>();

	private string dateStrId;

	public int m_ItemMiscID;

	public string m_ItemMiscStrID = "";

	public string m_ItemMiscName = "";

	public string m_ItemMiscIconName = "";

	public string m_ItemMiscDesc = "";

	public NKM_ITEM_MISC_TYPE m_ItemMiscType;

	public NKM_ITEM_MISC_SUBTYPE m_ItemMiscSubType;

	public NKM_ITEM_GRADE m_NKM_ITEM_GRADE;

	public int m_RewardGroupID;

	public string m_ShortCutShopTabID = "TAB_NONE";

	public int m_ShortCutShopIndex;

	public string m_BannerImage = "";

	public bool m_RandomBoxRatioBool;

	public string m_NexonLogSubType = "";

	private string m_OpenTag;

	public int m_typeValue;

	public bool m_ItemDropInfo;

	private string m_Option = "";

	public int m_CustomBoxId;

	public bool ChangeStat;

	public bool ChangeSetOption;

	public bool ChangePotenOption;

	public bool ChangePotenFirstOptionMax;

	public List<int> m_lstRecommandProductItemIfNotEnough;

	public static IEnumerable<NKMItemMiscTemplet> Values => NKMTempletContainer<NKMItemMiscTemplet>.Values;

	public static IEnumerable<NKMOfficeInteriorTemplet> InteriorValues => NKMTempletContainer<NKMOfficeInteriorTemplet>.Values;

	public int Key => m_ItemMiscID;

	public string DebugName => $"[{m_ItemMiscID}]{m_ItemMiscStrID}";

	public IReadOnlyList<NKMCustomPackageGroupTemplet> CustomPackageTemplets => customRewardTemplets;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public MiscContractTemplet MiscContractTemplet { get; private set; }

	public NKMIntervalTemplet IntervalTemplet { get; private set; }

	public bool IsPackageItem => m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_PACKAGE;

	public bool IsCustomPackageItem => m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE;

	public bool IsContractItem => m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CONTRACT;

	public bool IsTimeIntervalItem => !string.IsNullOrEmpty(dateStrId);

	public static NKMItemMiscTemplet Find(int key)
	{
		return NKMTempletContainer<NKMItemMiscTemplet>.Find(key);
	}

	public static NKMItemMiscTemplet Find(string key)
	{
		return NKMTempletContainer<NKMItemMiscTemplet>.Find(key);
	}

	public static NKMOfficeInteriorTemplet FindInterior(int key)
	{
		return NKMTempletContainer<NKMOfficeInteriorTemplet>.Find(key);
	}

	public static NKMItemMiscTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 61))
		{
			return null;
		}
		NKMItemMiscTemplet nKMItemMiscTemplet = new NKMItemMiscTemplet();
		if (!nKMItemMiscTemplet.Load(lua))
		{
			return null;
		}
		return nKMItemMiscTemplet;
	}

	public bool IsUsable()
	{
		NKM_ITEM_MISC_TYPE itemMiscType = m_ItemMiscType;
		if ((uint)(itemMiscType - 1) <= 1u || itemMiscType == NKM_ITEM_MISC_TYPE.IMT_CONTRACT)
		{
			return true;
		}
		return IsChoiceItem();
	}

	public bool IsRatioOpened()
	{
		return m_RandomBoxRatioBool;
	}

	public bool IsHideInInven()
	{
		switch (m_ItemMiscType)
		{
		case NKM_ITEM_MISC_TYPE.IMT_RESOURCE:
		case NKM_ITEM_MISC_TYPE.IMT_EMBLEM:
		case NKM_ITEM_MISC_TYPE.IMT_EMBLEM_RANK:
		case NKM_ITEM_MISC_TYPE.IMT_VIEW:
		case NKM_ITEM_MISC_TYPE.IMT_PIECE:
		case NKM_ITEM_MISC_TYPE.IMT_BACKGROUND:
		case NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME:
		case NKM_ITEM_MISC_TYPE.IMT_INTERIOR:
		case NKM_ITEM_MISC_TYPE.IMT_TITLE:
			return true;
		default:
			return false;
		}
	}

	public bool IsEmblem()
	{
		if (m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_EMBLEM || m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_EMBLEM_RANK)
		{
			return true;
		}
		return false;
	}

	public bool IsChoiceItem()
	{
		NKM_ITEM_MISC_TYPE itemMiscType = m_ItemMiscType;
		if ((uint)(itemMiscType - 7) <= 3u || itemMiscType == NKM_ITEM_MISC_TYPE.IMT_CHOICE_OPERATOR || (uint)(itemMiscType - 19) <= 1u)
		{
			return true;
		}
		return false;
	}

	public bool HasChoiceSubType()
	{
		NKM_ITEM_MISC_SUBTYPE itemMiscSubType = m_ItemMiscSubType;
		if (itemMiscSubType == NKM_ITEM_MISC_SUBTYPE.IMST_EQUIP_CHOICE_SET_OPTION || itemMiscSubType == NKM_ITEM_MISC_SUBTYPE.IMST_EQUIP_CHOICE_OPTION_CUSTOM)
		{
			return true;
		}
		return false;
	}

	public virtual void Join()
	{
		foreach (int item in customRewardGroupId)
		{
			NKMCustomPackageGroupTemplet nKMCustomPackageGroupTemplet = NKMCustomPackageGroupTemplet.Find(item);
			if (nKMCustomPackageGroupTemplet == null)
			{
				NKMTempletError.Add($"[MiscItem] CustomRewardGroupId가 올바르지 않음. miscId:{DebugName} groupId:{item}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 147);
			}
			else
			{
				customRewardTemplets.Add(nKMCustomPackageGroupTemplet);
			}
		}
		if (m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CONTRACT)
		{
			MiscContractTemplet = MiscContractTemplet.Find(m_typeValue);
			if (MiscContractTemplet == null)
			{
				NKMTempletError.Add($"[MiscItem] 채용 아이디가 올바르지 않음. miscId:{DebugName} typeValue:{m_typeValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 159);
			}
		}
		if (!string.IsNullOrEmpty(dateStrId) && NKMUtil.IsServer)
		{
			IntervalTemplet = NKMIntervalTemplet.Find(dateStrId);
			if (IntervalTemplet == null)
			{
				NKMTempletError.Add("[MiscItem] 잘못된 인터벌 아이디. miscId:" + DebugName + " dateStrId:" + dateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 168);
			}
		}
		Dictionary<string, bool> dictionary = NKMUtil.ParseStringTable(m_Option);
		if (dictionary.Count > 0)
		{
			foreach (KeyValuePair<string, bool> item2 in dictionary)
			{
				switch (item2.Key)
				{
				case "Stat":
					ChangeStat = item2.Value;
					break;
				case "SetOption":
					ChangeSetOption = item2.Value;
					break;
				case "PotenOption":
					ChangePotenOption = item2.Value;
					break;
				case "PotenOptionMax":
					ChangePotenFirstOptionMax = item2.Value;
					break;
				default:
					NKMTempletError.Add($"[NKMItemMiscTemplet] invalid Option Data. {item2.Key}:{item2.Value}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 192);
					break;
				}
			}
		}
		_ = m_CustomBoxId;
		_ = 0;
	}

	public virtual void Validate()
	{
		if (!EnableByTag)
		{
			return;
		}
		if (m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE && customRewardTemplets.Any())
		{
			NKMTempletError.Add("[MiscItem] 커스텀패키지가 아닌데 선택보상 정보를 갖고있음. miscId:" + DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 214);
		}
		if (m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CONTRACT)
		{
			MiscContractTemplet = MiscContractTemplet.Find(m_typeValue);
			if (MiscContractTemplet != null)
			{
				MiscContractTemplet.ValidateMiscContract();
			}
		}
		if (m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE)
		{
			if (customRewardTemplets.Count == 0)
			{
				NKMTempletError.Add("[MiscItem] 커스텀패키지 선택보상 정보가 비어있음. miscId:" + DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 230);
			}
			if (customRewardTemplets.Count > 10)
			{
				NKMTempletError.Add($"[MiscItem] 커스텀패키지 선택보상 최대개수 초과. miscId:{DebugName} count:{customRewardTemplets.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 235);
			}
		}
		if (m_ItemMiscSubType == NKM_ITEM_MISC_SUBTYPE.IMST_EQUIP_CHOICE_OPTION_CUSTOM && m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_CHOICE_EQUIP)
		{
			NKMTempletError.Add($"[MiscItem] 장비 아이템 선택권이 아니지만 장비 옵션 커스텀 타입이 SubType으로 지정됨. key:{Key} type{m_ItemMiscType} subType{m_ItemMiscSubType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 242);
		}
		if (m_CustomBoxId > 0 && NKMCustomBoxTemplet.Find(m_CustomBoxId) == null)
		{
			NKMTempletError.Add($"[MiscItem] 커스텀 옵션을 지정할 수 있으나 템플릿이 없음. key:{Key} type{m_ItemMiscType} customBoxId{m_CustomBoxId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 250);
		}
	}

	public bool ValidateCustompackageSelection(IReadOnlyList<int> selectIndices)
	{
		if (m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE || selectIndices == null || selectIndices.Count != customRewardTemplets.Count)
		{
			return false;
		}
		for (int i = 0; i < customRewardTemplets.Count; i++)
		{
			NKMCustomPackageGroupTemplet nKMCustomPackageGroupTemplet = customRewardTemplets[i];
			int num = selectIndices[i];
			if (num < 0 || num >= nKMCustomPackageGroupTemplet.Elements.Count)
			{
				return false;
			}
			if (!nKMCustomPackageGroupTemplet.Elements[num].EnableByTag)
			{
				return false;
			}
		}
		return true;
	}

	public bool SelectCustomPackageElement(IReadOnlyList<int> selectIndices, out List<NKMCustomPackageElement> elements)
	{
		if (m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE || selectIndices == null || selectIndices.Count != customRewardTemplets.Count)
		{
			elements = null;
			return false;
		}
		elements = new List<NKMCustomPackageElement>(customRewardTemplets.Count);
		for (int i = 0; i < customRewardTemplets.Count; i++)
		{
			NKMCustomPackageGroupTemplet nKMCustomPackageGroupTemplet = customRewardTemplets[i];
			int num = selectIndices[i];
			if (num < 0 || num >= nKMCustomPackageGroupTemplet.Elements.Count)
			{
				return false;
			}
			if (!nKMCustomPackageGroupTemplet.Elements[num].EnableByTag)
			{
				return false;
			}
			elements.Add(nKMCustomPackageGroupTemplet.Elements[num]);
		}
		return true;
	}

	protected virtual bool Load(NKMLua lua)
	{
		lua.GetData("m_OpenTag", ref m_OpenTag);
		lua.GetData("m_ItemMiscID", ref m_ItemMiscID);
		lua.GetData("m_ItemMiscStrID", ref m_ItemMiscStrID);
		lua.GetData("m_ItemMiscName", ref m_ItemMiscName);
		lua.GetData("m_ItemMiscIconName", ref m_ItemMiscIconName);
		lua.GetData("m_ItemMiscDesc", ref m_ItemMiscDesc);
		bool data = lua.GetData("m_ItemMiscType", ref m_ItemMiscType);
		lua.GetData("m_ItemMiscSubType", ref m_ItemMiscSubType);
		if (!(data & lua.GetData("m_NKM_ITEM_GRADE", ref m_NKM_ITEM_GRADE)))
		{
			Log.Error("NKMItemMiscTemplet.LoadFromLUA fail - " + m_ItemMiscStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemMiscTemplet.cs", 330);
			return false;
		}
		lua.GetData("m_RewardGroupID", ref m_RewardGroupID);
		lua.GetData("m_BannerImage", ref m_BannerImage);
		lua.GetData("m_RandomBoxRatioBool", ref m_RandomBoxRatioBool);
		lua.GetData("m_ShortCutShopTabID", ref m_ShortCutShopTabID);
		lua.GetData("m_ShortCutShopIndex", ref m_ShortCutShopIndex);
		lua.GetData("m_NexonLogSubType", ref m_NexonLogSubType);
		if (lua.OpenTable("m_PopupDisplayProductID"))
		{
			m_lstRecommandProductItemIfNotEnough = new List<int>();
			int i = 1;
			for (int rValue = 0; lua.GetData(i, ref rValue); i++)
			{
				m_lstRecommandProductItemIfNotEnough.Add(rValue);
			}
			lua.CloseTable();
		}
		if (lua.OpenTable("m_CustomRewardGroupID"))
		{
			int j = 1;
			for (int rValue2 = 0; lua.GetData(j, ref rValue2); j++)
			{
				customRewardGroupId.Add(rValue2);
			}
			lua.CloseTable();
		}
		lua.GetData("m_typeValue", ref m_typeValue);
		lua.GetData("m_ItemDropInfo", ref m_ItemDropInfo);
		lua.GetData("m_DateStrID", ref dateStrId);
		lua.GetData("m_Option", ref m_Option);
		lua.GetData("m_CustomBoxID", ref m_CustomBoxId);
		return true;
	}

	public string GetItemName()
	{
		return NKCStringTable.GetString(m_ItemMiscName);
	}

	public string GetItemDesc()
	{
		if (m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_INTERIOR)
		{
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = FindInterior(Key);
			if (nKMOfficeInteriorTemplet != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(NKCStringTable.GetString("SI_DP_INTERIOR_SCORE_ONE_PARAM", nKMOfficeInteriorTemplet.InteriorScore));
				if (nKMOfficeInteriorTemplet.InteriorCategory == InteriorCategory.FURNITURE)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(NKCStringTable.GetString("SI_DP_INTERIOR_SIZE_TWO_PARAM", nKMOfficeInteriorTemplet.CellX, nKMOfficeInteriorTemplet.CellY));
				}
				stringBuilder.AppendLine();
				stringBuilder.Append(NKCStringTable.GetString(m_ItemMiscDesc));
				return stringBuilder.ToString();
			}
		}
		return NKCStringTable.GetString(m_ItemMiscDesc);
	}

	public TimeSpan GetIntervalTimeSpanLeft()
	{
		if (!IsTimeIntervalItem)
		{
			return new TimeSpan(99999, 0, 0, 0);
		}
		NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(dateStrId);
		if (nKMIntervalTemplet == null)
		{
			Log.Error($"MiscItemId: {m_ItemMiscID} DateStrId: {dateStrId} IntervalTemplet is not found", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMItemManagerEx.cs", 168);
			return new TimeSpan(99999, 0, 0, 0);
		}
		return NKCSynchronizedTime.GetTimeLeft(nKMIntervalTemplet.GetEndDateUtc());
	}

	public bool WillBeDeletedSoon()
	{
		return GetIntervalTimeSpanLeft().Days < 2;
	}
}
