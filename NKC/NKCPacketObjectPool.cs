using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ClientPacket.Game;
using ClientPacket.Service;
using NKM;

namespace NKC;

public static class NKCPacketObjectPool
{
	private sealed class TypeElement
	{
		public readonly Action<object> cleaner;

		public readonly ConcurrentQueue<object> objects = new ConcurrentQueue<object>();

		public TypeElement(Action<object> cleaner)
		{
			this.cleaner = cleaner;
		}

		public bool TryPop(out object data)
		{
			return objects.TryDequeue(out data);
		}

		public void Push(object data)
		{
			objects.Enqueue(data);
		}
	}

	private static bool bUsePool = true;

	private static readonly Dictionary<Type, TypeElement> typePools = new Dictionary<Type, TypeElement>();

	public static void SetUsePool(bool bUse)
	{
		bUsePool = bUse;
	}

	public static void Init()
	{
		typePools.Clear();
		typePools.Add(typeof(NKMPacket_HEART_BIT_ACK), new TypeElement(Open_NKMPacket_HEART_BIT_ACK));
		typePools.Add(typeof(NKMPacket_SERVER_TIME_ACK), new TypeElement(Open_NKMPacket_SERVER_TIME_ACK));
		typePools.Add(typeof(NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT), new TypeElement(Open_NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT));
		typePools.Add(typeof(NKMGameSyncDataPack), new TypeElement(Open_NKMGameSyncDataPack));
		typePools.Add(typeof(NKMGameSyncData_Base), new TypeElement(Open_NKMGameSyncData_Base));
		typePools.Add(typeof(NKMGameSyncData_GamePoint), new TypeElement(Open_NKMGameSyncData_GamePoint));
		typePools.Add(typeof(NKMGameSyncData_DieUnit), new TypeElement(Open_NKMGameSyncData_DieUnit));
		typePools.Add(typeof(NKMGameSyncData_Unit), new TypeElement(Open_NKMGameSyncData_Unit));
		typePools.Add(typeof(NKMGameSyncDataSimple_Unit), new TypeElement(Open_NKMGameSyncDataSimple_Unit));
		typePools.Add(typeof(NKMDamageData), new TypeElement(Open_NKMDamageData));
		typePools.Add(typeof(NKMBuffSyncData), new TypeElement(Open_NKMBuffSyncData));
		typePools.Add(typeof(NKMUnitSyncData), new TypeElement(Open_NKMUnitSyncData));
		typePools.Add(typeof(NKMGameSyncData_ShipSkill), new TypeElement(Open_NKMGameSyncData_ShipSkill));
		typePools.Add(typeof(NKMGameSyncData_Deck), new TypeElement(Open_NKMGameSyncData_Deck));
		typePools.Add(typeof(NKMGameSyncData_DeckAssist), new TypeElement(Open_NKMGameSyncData_DeckAssist));
		typePools.Add(typeof(NKMGameSyncData_GameState), new TypeElement(Open_NKMGameSyncData_GameState));
		typePools.Add(typeof(NKMGameSyncData_DungeonEvent), new TypeElement(Open_NKMGameSyncData_DungeonEvent));
		typePools.Add(typeof(NKMGameSyncData_GameEvent), new TypeElement(Open_NKMGameSyncData_GameEvent));
		typePools.Add(typeof(NKMUnitStatusTimeSyncData), new TypeElement(Open_NKMStatusTimeSyncData));
		foreach (KeyValuePair<Type, TypeElement> typePool in typePools)
		{
			for (int i = 0; i < 10000; i++)
			{
				Type key = typePool.Key;
				typePool.Value.objects.Enqueue(Activator.CreateInstance(key));
			}
		}
	}

	public static bool IsManagedType(Type type)
	{
		return typePools.ContainsKey(type);
	}

	public static void Open_Packet(object obj)
	{
	}

	public static void Open_NKMPacket_HEART_BIT_ACK(object obj)
	{
	}

	public static void Open_NKMPacket_SERVER_TIME_ACK(object obj)
	{
	}

	public static void Open_NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT(object obj)
	{
		if (obj != null)
		{
			NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT obj2 = (NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT)obj;
			CloseObject(obj2.gameSyncDataPack);
			obj2.gameSyncDataPack = null;
		}
	}

	public static void Open_NKMGameSyncDataPack(object obj)
	{
		if (obj != null)
		{
			NKMGameSyncDataPack nKMGameSyncDataPack = (NKMGameSyncDataPack)obj;
			for (int i = 0; i < nKMGameSyncDataPack.m_listGameSyncData.Count; i++)
			{
				CloseObject(nKMGameSyncDataPack.m_listGameSyncData[i]);
			}
			nKMGameSyncDataPack.m_listGameSyncData.Clear();
		}
	}

	public static void Open_NKMGameSyncData_Base(object obj)
	{
		if (obj != null)
		{
			NKMGameSyncData_Base nKMGameSyncData_Base = (NKMGameSyncData_Base)obj;
			for (int i = 0; i < nKMGameSyncData_Base.m_NKMGameSyncData_DieUnit.Count; i++)
			{
				CloseObject(nKMGameSyncData_Base.m_NKMGameSyncData_DieUnit[i]);
			}
			nKMGameSyncData_Base.m_NKMGameSyncData_DieUnit.Clear();
			for (int j = 0; j < nKMGameSyncData_Base.m_NKMGameSyncData_Unit.Count; j++)
			{
				CloseObject(nKMGameSyncData_Base.m_NKMGameSyncData_Unit[j]);
			}
			nKMGameSyncData_Base.m_NKMGameSyncData_Unit.Clear();
			for (int k = 0; k < nKMGameSyncData_Base.m_NKMGameSyncDataSimple_Unit.Count; k++)
			{
				CloseObject(nKMGameSyncData_Base.m_NKMGameSyncDataSimple_Unit[k]);
			}
			nKMGameSyncData_Base.m_NKMGameSyncDataSimple_Unit.Clear();
			for (int l = 0; l < nKMGameSyncData_Base.m_NKMGameSyncData_ShipSkill.Count; l++)
			{
				CloseObject(nKMGameSyncData_Base.m_NKMGameSyncData_ShipSkill[l]);
			}
			nKMGameSyncData_Base.m_NKMGameSyncData_ShipSkill.Clear();
			for (int m = 0; m < nKMGameSyncData_Base.m_NKMGameSyncData_Deck.Count; m++)
			{
				CloseObject(nKMGameSyncData_Base.m_NKMGameSyncData_Deck[m]);
			}
			nKMGameSyncData_Base.m_NKMGameSyncData_Deck.Clear();
			for (int n = 0; n < nKMGameSyncData_Base.m_NKMGameSyncData_DeckAssist.Count; n++)
			{
				CloseObject(nKMGameSyncData_Base.m_NKMGameSyncData_DeckAssist[n]);
			}
			nKMGameSyncData_Base.m_NKMGameSyncData_DeckAssist.Clear();
			for (int num = 0; num < nKMGameSyncData_Base.m_NKMGameSyncData_GameState.Count; num++)
			{
				CloseObject(nKMGameSyncData_Base.m_NKMGameSyncData_GameState[num]);
			}
			nKMGameSyncData_Base.m_NKMGameSyncData_GameState.Clear();
			for (int num2 = 0; num2 < nKMGameSyncData_Base.m_NKMGameSyncData_DungeonEvent.Count; num2++)
			{
				CloseObject(nKMGameSyncData_Base.m_NKMGameSyncData_DungeonEvent[num2]);
			}
			nKMGameSyncData_Base.m_NKMGameSyncData_DungeonEvent.Clear();
			for (int num3 = 0; num3 < nKMGameSyncData_Base.m_NKMGameSyncData_GameEvent.Count; num3++)
			{
				CloseObject(nKMGameSyncData_Base.m_NKMGameSyncData_GameEvent[num3]);
			}
			nKMGameSyncData_Base.m_NKMGameSyncData_GameEvent.Clear();
			CloseObject(nKMGameSyncData_Base.m_NKMGameSyncData_GamePoint);
			nKMGameSyncData_Base.m_NKMGameSyncData_GamePoint = null;
		}
	}

	public static void Open_NKMGameSyncData_GamePoint(object obj)
	{
	}

	public static void Open_NKMGameSyncData_DieUnit(object obj)
	{
	}

	public static void Open_NKMGameSyncData_Unit(object obj)
	{
		if (obj != null)
		{
			NKMGameSyncData_Unit obj2 = (NKMGameSyncData_Unit)obj;
			CloseObject(obj2.m_NKMGameUnitSyncData);
			obj2.m_NKMGameUnitSyncData = null;
		}
	}

	public static void Open_NKMGameSyncDataSimple_Unit(object obj)
	{
		if (obj == null)
		{
			return;
		}
		NKMGameSyncDataSimple_Unit nKMGameSyncDataSimple_Unit = (NKMGameSyncDataSimple_Unit)obj;
		foreach (KeyValuePair<short, NKMBuffSyncData> dicBuffDatum in nKMGameSyncDataSimple_Unit.m_dicBuffData)
		{
			CloseObject(dicBuffDatum.Value);
		}
		nKMGameSyncDataSimple_Unit.m_dicBuffData.Clear();
	}

	public static void Open_NKMDamageData(object obj)
	{
	}

	public static void Open_NKMBuffSyncData(object obj)
	{
	}

	public static void Open_NKMStatusTimeSyncData(object obj)
	{
	}

	public static void Open_NKMUnitSyncData(object obj)
	{
		if (obj == null)
		{
			return;
		}
		NKMUnitSyncData nKMUnitSyncData = (NKMUnitSyncData)obj;
		for (int i = 0; i < nKMUnitSyncData.m_listDamageData.Count; i++)
		{
			CloseObject(nKMUnitSyncData.m_listDamageData[i]);
		}
		nKMUnitSyncData.m_listDamageData.Clear();
		foreach (KeyValuePair<short, NKMBuffSyncData> dicBuffDatum in nKMUnitSyncData.m_dicBuffData)
		{
			CloseObject(dicBuffDatum.Value);
		}
		nKMUnitSyncData.m_dicBuffData.Clear();
		foreach (NKMUnitStatusTimeSyncData listStatusTimeDatum in nKMUnitSyncData.m_listStatusTimeData)
		{
			CloseObject(listStatusTimeDatum);
		}
		nKMUnitSyncData.m_listStatusTimeData.Clear();
	}

	public static void Open_NKMGameSyncData_ShipSkill(object obj)
	{
		if (obj != null)
		{
			NKMGameSyncData_ShipSkill obj2 = (NKMGameSyncData_ShipSkill)obj;
			CloseObject(obj2.m_NKMGameUnitSyncData);
			obj2.m_NKMGameUnitSyncData = null;
		}
	}

	public static void Open_NKMGameSyncData_Deck(object obj)
	{
	}

	public static void Open_NKMGameSyncData_DeckAssist(object obj)
	{
	}

	public static void Open_NKMGameSyncData_GameState(object obj)
	{
	}

	public static void Open_NKMGameSyncData_DungeonEvent(object obj)
	{
	}

	public static void Open_NKMGameSyncData_GameEvent(object obj)
	{
	}

	public static object OpenObject(Type type)
	{
		if (!bUsePool)
		{
			return Activator.CreateInstance(type);
		}
		if (!typePools.TryGetValue(type, out var value))
		{
			return Activator.CreateInstance(type);
		}
		if (!value.TryPop(out var data))
		{
			return Activator.CreateInstance(type);
		}
		return data;
	}

	public static T OpenObject<T>() where T : class, new()
	{
		if (!bUsePool)
		{
			return new T();
		}
		Type typeFromHandle = typeof(T);
		if (!typePools.TryGetValue(typeFromHandle, out var value))
		{
			return new T();
		}
		if (!value.TryPop(out var data))
		{
			data = Activator.CreateInstance(typeFromHandle);
		}
		return data as T;
	}

	public static void CloseObject(object obj)
	{
		if (bUsePool && obj != null)
		{
			Type type = obj.GetType();
			if (typePools.TryGetValue(type, out var value))
			{
				value.cleaner(obj);
				value.Push(obj);
			}
		}
	}
}
