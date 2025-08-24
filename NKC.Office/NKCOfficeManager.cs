using System.Collections.Generic;
using ClientPacket.Office;
using Cs.Logging;
using NKC.Templet;
using NKC.Templet.Office;
using NKC.UI.Office;
using NKC.Util;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;

namespace NKC.Office;

public static class NKCOfficeManager
{
	private static bool m_bTempletInitialized;

	public static Dictionary<int, string> s_dicInteractionSkinGroup;

	public static void LoadTemplets()
	{
		if (!m_bTempletInitialized)
		{
			NKMTempletContainer<NKMOfficeRoomTemplet>.Load("AB_SCRIPT", "LUA_OFFICE_ROOM_TEMPLET", "m_OfficeRoom", NKMOfficeRoomTemplet.LoadFromLUA);
			NKMTempletContainer<NKMOfficeSectionTemplet>.Load("AB_SCRIPT", "LUA_OFFICE_SECTION_TEMPLET", "m_OfficeSection", NKMOfficeSectionTemplet.LoadFromLua);
			NKMTempletContainer<NKCOfficeCharacterTemplet>.Load("AB_SCRIPT", "LUA_OFFICE_CHARACTER_TEMPLET", "m_OfficeCharacter", NKCOfficeCharacterTemplet.LoadFromLUA);
			NKMTempletContainer<NKCOfficePartyTemplet>.Load("AB_SCRIPT", "LUA_OFFICE_PARTY_TEMPLET", "OFFICE_PARTY_TEMPLET", NKCOfficePartyTemplet.LoadFromLUA);
			NKMTempletContainer<NKMOfficeThemePresetTemplet>.Load("AB_SCRIPT", "LUA_OFFICE_THEMA_PRESET_TEMPLET", "OFFICE_THEMA_PRESET_TEMPLET", NKMOfficeThemePresetTemplet.LoadFromLUA, (NKMOfficeThemePresetTemplet e) => e.ThemaPresetStringID);
			if (!NKCAnimationEventManager.DataExist)
			{
				NKCAnimationEventManager.LoadFromLua();
			}
			NKMTempletContainer<NKMOfficeThemePresetTemplet>.Join();
			NKMTempletContainer<NKMOfficeSectionTemplet>.Join();
			NKMTempletContainer<NKMOfficeRoomTemplet>.Join();
			m_bTempletInitialized = true;
		}
	}

	public static void Drop()
	{
		m_bTempletInitialized = false;
		s_dicInteractionSkinGroup = null;
	}

	public static bool FunitureBoundaryCheck(int roomSizeX, int roomSizeY, NKCOfficeFunitureData funitureData)
	{
		if (funitureData.Templet == null)
		{
			return false;
		}
		if (funitureData.PosX < 0 || funitureData.PosY < 0)
		{
			return false;
		}
		if (funitureData.PosX + funitureData.SizeX - 1 >= roomSizeX || funitureData.PosY + funitureData.SizeY - 1 >= roomSizeY)
		{
			return false;
		}
		return true;
	}

	public static bool IsFunitureOverlaps(NKCOfficeFunitureData lhs, NKCOfficeFunitureData rhs)
	{
		if (lhs.PosX + lhs.SizeX - 1 < rhs.PosX)
		{
			return false;
		}
		if (rhs.PosX + rhs.SizeX - 1 < lhs.PosX)
		{
			return false;
		}
		if (lhs.PosY + lhs.SizeY - 1 < rhs.PosY)
		{
			return false;
		}
		if (rhs.PosY + rhs.SizeY - 1 < lhs.PosY)
		{
			return false;
		}
		return true;
	}

	public static (int, int) GetSize(this NKMOfficeRoomTemplet templet, BuildingFloor target)
	{
		switch (target)
		{
		case BuildingFloor.Floor:
		case BuildingFloor.Tile:
			return (templet.FloorX, templet.FloorY);
		case BuildingFloor.RightWall:
			return (templet.RightWallX, templet.RightWallY);
		case BuildingFloor.LeftWall:
			return (templet.LeftWallX, templet.LeftWallY);
		default:
			return (0, 0);
		}
	}

	public static void TryPlayReactionInteraction(NKCOfficeFuniture furniture, IEnumerable<NKCOfficeCharacter> lstCharacter)
	{
		if (furniture == null || furniture.Templet == null || lstCharacter == null)
		{
			return;
		}
		List<NKCOfficeFurnitureInteractionTemplet> interactionTempletList = NKCOfficeFurnitureInteractionTemplet.GetInteractionTempletList(furniture.Templet, NKCOfficeFurnitureInteractionTemplet.ActType.Reaction);
		if (interactionTempletList.Count == 0)
		{
			return;
		}
		Debug.Log("Try playing reaction : " + furniture.Templet.InteractionGroupID);
		foreach (NKCOfficeCharacter character in lstCharacter)
		{
			if (character == null)
			{
				continue;
			}
			List<NKCOfficeFurnitureInteractionTemplet> list = interactionTempletList.FindAll((NKCOfficeFurnitureInteractionTemplet x) => x.CheckUnitInteractionCondition(character) && NKCAnimationEventManager.CanPlayAnimEvent(character, x.UnitAni));
			if (list != null && list.Count != 0)
			{
				NKCOfficeFurnitureInteractionTemplet templet = NKCTempletUtility.PickRatio(list, (NKCOfficeFurnitureInteractionTemplet x) => x.ActProbability);
				character.RegisterInteraction(furniture, templet);
			}
		}
	}

	public static bool PlayInteraction(NKCOfficeCharacter character, NKCOfficeFuniture furniture)
	{
		if (character == null || furniture == null)
		{
			return false;
		}
		if (furniture.Templet == null)
		{
			return false;
		}
		List<NKCOfficeFurnitureInteractionTemplet> possibleTemplets = GetPossibleTemplets(character, furniture, NKCOfficeFurnitureInteractionTemplet.ActType.Common);
		if (possibleTemplets == null || possibleTemplets.Count == 0)
		{
			return false;
		}
		NKCOfficeFurnitureInteractionTemplet templet = NKCTempletUtility.PickRatio(possibleTemplets, (NKCOfficeFurnitureInteractionTemplet x) => x.ActProbability);
		character.RegisterInteraction(furniture, templet);
		return true;
	}

	public static bool PlayInteraction(NKCOfficeCharacter actor, NKCOfficeCharacter target, bool bIgnoreRange = false, bool bForceAlign = false)
	{
		if (actor == null || target == null)
		{
			return false;
		}
		List<NKCOfficeUnitInteractionTemplet> possibleTemplets = GetPossibleTemplets(actor, target, bIgnoreRange);
		if (possibleTemplets == null || possibleTemplets.Count == 0)
		{
			return false;
		}
		NKCOfficeUnitInteractionTemplet nKCOfficeUnitInteractionTemplet = possibleTemplets[Random.Range(0, possibleTemplets.Count)];
		if (bForceAlign || nKCOfficeUnitInteractionTemplet.AlignUnit)
		{
			if (!actor.OfficeBuilding.CalcInteractionPos(actor, target, out var actorPos, out var targetPos))
			{
				return false;
			}
			actor.RegisterInteraction(nKCOfficeUnitInteractionTemplet, target, IsMainActor: true, actorPos);
			target.RegisterInteraction(nKCOfficeUnitInteractionTemplet, actor, IsMainActor: false, targetPos);
		}
		else
		{
			actor.RegisterInteraction(nKCOfficeUnitInteractionTemplet, target, IsMainActor: true, actor.transform.localPosition);
			target.RegisterInteraction(nKCOfficeUnitInteractionTemplet, actor, IsMainActor: false, target.transform.localPosition);
		}
		return true;
	}

	public static bool CanPlayInteraction(NKCOfficeCharacter character, NKCOfficeFuniture furniture)
	{
		if (character == null || furniture == null)
		{
			return false;
		}
		if (NKCUIOffice.IsInstanceOpen && !NKCUIOffice.GetInstance().CanPlayInteraction())
		{
			return false;
		}
		if (furniture.Templet == null)
		{
			return false;
		}
		if (furniture.HasInteractionTarget())
		{
			return false;
		}
		if (character.HasInteractionTarget())
		{
			return false;
		}
		if (IsFunitureInteractionPointBlocked(furniture, character.OfficeBuilding))
		{
			return false;
		}
		List<NKCOfficeFurnitureInteractionTemplet> possibleTemplets = GetPossibleTemplets(character, furniture, NKCOfficeFurnitureInteractionTemplet.ActType.Common);
		if (possibleTemplets == null || possibleTemplets.Count == 0)
		{
			return false;
		}
		return true;
	}

	public static bool CanPlayInteraction(NKCOfficeCharacter actor, NKCOfficeCharacter target, bool bIgnoreRange = false)
	{
		if (actor == null || target == null)
		{
			return false;
		}
		if (actor == target)
		{
			return false;
		}
		if (NKCUIOffice.IsInstanceOpen && !NKCUIOffice.GetInstance().CanPlayInteraction())
		{
			return false;
		}
		if (!target.IsUnitInteractTargetable())
		{
			return false;
		}
		if (!IsUnitInSameInteractionSkinGroup(actor, target))
		{
			return false;
		}
		List<NKCOfficeUnitInteractionTemplet> possibleTemplets = GetPossibleTemplets(actor, target, bIgnoreRange);
		if (possibleTemplets == null || possibleTemplets.Count == 0)
		{
			return false;
		}
		return true;
	}

	private static List<NKCOfficeFurnitureInteractionTemplet> GetPossibleTemplets(NKCOfficeCharacter character, NKCOfficeFuniture furniture, NKCOfficeFurnitureInteractionTemplet.ActType type)
	{
		List<NKCOfficeFurnitureInteractionTemplet> interactionTempletList = NKCOfficeFurnitureInteractionTemplet.GetInteractionTempletList(furniture.Templet, type);
		if (interactionTempletList == null || interactionTempletList.Count == 0)
		{
			return null;
		}
		List<NKCOfficeFurnitureInteractionTemplet> list = new List<NKCOfficeFurnitureInteractionTemplet>();
		foreach (NKCOfficeFurnitureInteractionTemplet item in interactionTempletList)
		{
			if (item.CheckUnitInteractionCondition(character) && CheckInteractionPlay(item, character, furniture))
			{
				list.Add(item);
			}
		}
		return list;
	}

	private static string GetSkinInteractionGroup(int skinID)
	{
		if (skinID == 0)
		{
			return string.Empty;
		}
		if (s_dicInteractionSkinGroup == null)
		{
			NKCOfficeUnitInteractionTemplet.LoadFromLua();
		}
		if (s_dicInteractionSkinGroup.TryGetValue(skinID, out var value))
		{
			return value;
		}
		return string.Empty;
	}

	private static bool IsUnitInSameInteractionSkinGroup(NKCOfficeCharacter actor, NKCOfficeCharacter target)
	{
		string skinInteractionGroup = GetSkinInteractionGroup(actor.SkinID);
		string skinInteractionGroup2 = GetSkinInteractionGroup(target.SkinID);
		return skinInteractionGroup.Equals(skinInteractionGroup2);
	}

	private static List<NKCOfficeUnitInteractionTemplet> GetPossibleTemplets(NKCOfficeCharacter actor, NKCOfficeCharacter target, bool bIgnoreRange = false)
	{
		List<NKCOfficeUnitInteractionTemplet> unitInteractionCache = actor.UnitInteractionCache;
		if (unitInteractionCache == null || unitInteractionCache.Count == 0)
		{
			return null;
		}
		float magnitude = (target.transform.localPosition - actor.transform.localPosition).magnitude;
		List<NKCOfficeUnitInteractionTemplet> list = new List<NKCOfficeUnitInteractionTemplet>();
		foreach (NKCOfficeUnitInteractionTemplet item in unitInteractionCache)
		{
			if ((bIgnoreRange || !(item.ActRange < magnitude)) && item.CheckUnitInteractionCondition(target, bTarget: true) && NKCAnimationEventManager.CanPlayAnimEvent(target, item.TargetAni))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static bool IsFunitureInteractionPointBlocked(NKCOfficeFuniture furniture, NKCOfficeBuildingBase building)
	{
		GameObject interactionPoint = furniture.GetInteractionPoint();
		if (interactionPoint == null)
		{
			return false;
		}
		Vector3 localPos = building.m_Floor.Rect.InverseTransformPoint(interactionPoint.transform.position);
		OfficeFloorPosition pos = building.CalculateFloorPosition(localPos);
		if (!building.m_Floor.IsInBound(pos))
		{
			return true;
		}
		long num = building.FloorMap[pos.x, pos.y];
		if (num == 0L)
		{
			return false;
		}
		if (num == furniture.UID)
		{
			return false;
		}
		return true;
	}

	private static bool CheckInteractionPlay(NKCOfficeFurnitureInteractionTemplet templet, NKCOfficeCharacter character, NKCOfficeFuniture furniture)
	{
		if (!NKCAnimationEventManager.CanPlayAnimEvent(character, templet.UnitAni))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(templet.InteriorAni) && !NKCAnimationEventManager.CanPlayAnimEvent(furniture, templet.InteriorAni))
		{
			return false;
		}
		return true;
	}

	public static bool IsActTarget(NKCOfficeCharacter character, ActTargetType eActTargetType, HashSet<string> hsActTargetGroupID)
	{
		if (character == null)
		{
			return false;
		}
		return IsActTarget(character.UnitID, character.SkinID, eActTargetType, hsActTargetGroupID);
	}

	public static bool IsActTarget(int unitID, int skinID, ActTargetType eActTargetType, HashSet<string> hsActTargetGroupID)
	{
		if (hsActTargetGroupID == null)
		{
			return false;
		}
		switch (eActTargetType)
		{
		case ActTargetType.Group:
			return IsActGroup(unitID, hsActTargetGroupID);
		case ActTargetType.Skin:
			return hsActTargetGroupID.Contains(skinID.ToString());
		default:
		{
			if (hsActTargetGroupID.Contains(unitID.ToString()))
			{
				return true;
			}
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
			if (nKMUnitTempletBase != null && nKMUnitTempletBase.IsRearmUnit && nKMUnitTempletBase.BaseUnit != null && hsActTargetGroupID.Contains(nKMUnitTempletBase.BaseUnit.m_UnitID.ToString()))
			{
				return true;
			}
			return false;
		}
		}
	}

	public static bool IsActGroup(int unitID, IEnumerable<string> lstGroup)
	{
		return IsActGroup(NKMUnitManager.GetUnitTempletBase(unitID), lstGroup);
	}

	public static bool IsActGroup(NKMUnitTempletBase unitTemplet, IEnumerable<string> lstGroup)
	{
		if (unitTemplet == null)
		{
			return false;
		}
		if (lstGroup == null)
		{
			return false;
		}
		foreach (string item in lstGroup)
		{
			switch (item)
			{
			case "ALL":
				return true;
			case "COUNTER":
				if (unitTemplet.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_COUNTER))
				{
					return true;
				}
				break;
			case "SOLDIER":
				if (unitTemplet.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_SOLDIER))
				{
					return true;
				}
				break;
			case "MECHANIC":
				if (unitTemplet.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_MECHANIC))
				{
					return true;
				}
				break;
			case "REPLACER":
				if (unitTemplet.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_REPLACER))
				{
					return true;
				}
				break;
			case "CORRUPTED":
				if (unitTemplet.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED))
				{
					return true;
				}
				break;
			case "DEFENDER":
				if (unitTemplet.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_DEFENDER)
				{
					return true;
				}
				break;
			case "STRIKER":
				if (unitTemplet.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_STRIKER)
				{
					return true;
				}
				break;
			case "RANGER":
				if (unitTemplet.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_RANGER)
				{
					return true;
				}
				break;
			case "SNIPER":
				if (unitTemplet.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_SNIPER)
				{
					return true;
				}
				break;
			case "SUPPORTER":
				if (unitTemplet.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER)
				{
					return true;
				}
				break;
			case "TOWER":
				if (unitTemplet.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_TOWER)
				{
					return true;
				}
				break;
			case "SIEGE":
				if (unitTemplet.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_SIEGE)
				{
					return true;
				}
				break;
			default:
				if (unitTemplet.m_hsActGroup != null && unitTemplet.m_hsActGroup.Contains(item))
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	public static bool IsEmpryRoom(NKMOfficeRoom room)
	{
		if (room == null)
		{
			return true;
		}
		if (room.furnitures != null && room.furnitures.Count > 0)
		{
			return false;
		}
		if (room.floorInteriorId != 0 && room.floorInteriorId != NKMCommonConst.Office.DefaultFloorItem.Id)
		{
			return false;
		}
		if (room.wallInteriorId != 0 && room.wallInteriorId != NKMCommonConst.Office.DefaultWallItem.Id)
		{
			return false;
		}
		if (room.backgroundId != 0 && room.backgroundId != NKMCommonConst.Office.DefaultBackgroundItem.Id)
		{
			return false;
		}
		return true;
	}

	public static bool IsEmpryPreset(NKMOfficePreset preset)
	{
		if (preset == null)
		{
			return true;
		}
		if (preset.furnitures != null && preset.furnitures.Count > 0)
		{
			return false;
		}
		if (preset.floorInteriorId != 0 && preset.floorInteriorId != NKMCommonConst.Office.DefaultFloorItem.Id)
		{
			return false;
		}
		if (preset.wallInteriorId != 0 && preset.wallInteriorId != NKMCommonConst.Office.DefaultWallItem.Id)
		{
			return false;
		}
		if (preset.backgroundId != 0 && preset.backgroundId != NKMCommonConst.Office.DefaultBackgroundItem.Id)
		{
			return false;
		}
		return true;
	}

	public static Dictionary<int, long> MakeRequiredFurnitureHaveCountDic(int roomID, NKMOfficePreset preset)
	{
		NKCOfficeData officeData = NKCScenManager.CurrentUserData().OfficeData;
		Dictionary<int, long> dictionary = new Dictionary<int, long>();
		foreach (NKMOfficeFurniture furniture in preset.furnitures)
		{
			int targetID = furniture.itemId;
			if (!dictionary.ContainsKey(targetID))
			{
				long freeInteriorCount = officeData.GetFreeInteriorCount(targetID);
				long num = officeData.GetOfficeRoom(roomID).furnitures.FindAll((NKMOfficeFurniture x) => x.itemId == targetID).Count;
				dictionary.Add(targetID, freeInteriorCount + num);
			}
		}
		return dictionary;
	}

	public static bool CheckFurnitureHaveCount(int roomID, NKMOfficePreset preset)
	{
		Dictionary<int, long> dictionary = MakeRequiredFurnitureHaveCountDic(roomID, preset);
		foreach (NKMOfficeFurniture furniture in preset.furnitures)
		{
			int itemId = furniture.itemId;
			if (!dictionary.TryGetValue(itemId, out var value))
			{
				value = 0L;
			}
			if (value <= 0)
			{
				return false;
			}
			dictionary[itemId] = value - 1;
		}
		return true;
	}

	public static bool IsAllFurniturePlaced(NKMOfficePreset preset, NKMOfficeRoom room)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
		dictionary.Add(preset.floorInteriorId, 1);
		dictionary.Add(preset.wallInteriorId, 1);
		dictionary.Add(preset.backgroundId, 1);
		foreach (NKMOfficeFurniture furniture in preset.furnitures)
		{
			if (dictionary.ContainsKey(furniture.itemId))
			{
				dictionary[furniture.itemId]++;
			}
			else
			{
				dictionary[furniture.itemId] = 1;
			}
		}
		dictionary2.Add(room.floorInteriorId, 1);
		dictionary2.Add(room.wallInteriorId, 1);
		dictionary2.Add(room.backgroundId, 1);
		foreach (NKMOfficeFurniture furniture2 in room.furnitures)
		{
			if (dictionary2.ContainsKey(furniture2.itemId))
			{
				dictionary2[furniture2.itemId]++;
			}
			else
			{
				dictionary2[furniture2.itemId] = 1;
			}
		}
		foreach (KeyValuePair<int, int> item in dictionary)
		{
			if (!dictionary2.ContainsKey(item.Key))
			{
				return false;
			}
			dictionary2[item.Key] -= item.Value;
		}
		foreach (KeyValuePair<int, int> item2 in dictionary2)
		{
			if (item2.Value > 0)
			{
				return false;
			}
		}
		return true;
	}

	public static string GetMyPresetName(int index)
	{
		NKMOfficePreset preset = NKCScenManager.CurrentUserData().OfficeData.GetPreset(index);
		if (preset == null || string.IsNullOrEmpty(preset.name))
		{
			return GetDefaultPresetName(index);
		}
		return preset.name;
	}

	public static string GetDefaultPresetName(int index)
	{
		return NKCStringTable.GetString("SI_PF_OFFICE_DECO_MODE_PRESET_NAME", index + 1);
	}

	public static void BuildSkinInteractionGroup()
	{
		s_dicInteractionSkinGroup = new Dictionary<int, string>();
		foreach (NKCOfficeUnitInteractionTemplet value in NKMTempletContainer<NKCOfficeUnitInteractionTemplet>.Values)
		{
			if (value != null && !string.IsNullOrEmpty(value.InteractionSkinGroup))
			{
				if (value.ActorType != ActTargetType.Skin || value.TargetType != ActTargetType.Skin)
				{
					Log.Error($"[NKCOfficeUnitInteractionTemplet:{value.Key}] InteractionSkinGroup이 존재하나 양쪽 타입이 Skin이 아님", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Office/NKCOfficeManager.cs", 1111);
					continue;
				}
				AddToSkinInteractionGroup(value.Key, value.InteractionSkinGroup, value.hsActorGroup);
				AddToSkinInteractionGroup(value.Key, value.InteractionSkinGroup, value.hsTargetGroup);
			}
		}
	}

	private static void AddToSkinInteractionGroup(int key, string groupID, HashSet<string> hsGroup)
	{
		foreach (string item in hsGroup)
		{
			string value;
			if (!int.TryParse(item, out var result))
			{
				Log.Error($"[NKCOfficeUnitInteractionTemplet:{key}] Skin ID parse error", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Office/NKCOfficeManager.cs", 1126);
			}
			else if (s_dicInteractionSkinGroup.TryGetValue(result, out value))
			{
				if (!groupID.Equals(value))
				{
					Log.Error($"[NKCOfficeUnitInteractionTemplet:{key}] 스킨 {result}가 2개 이상의 그룹에 속함 : {groupID}, {value}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Office/NKCOfficeManager.cs", 1134);
				}
			}
			else
			{
				s_dicInteractionSkinGroup.Add(result, groupID);
			}
		}
	}
}
