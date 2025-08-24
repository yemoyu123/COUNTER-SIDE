using System.IO;
using System.Threading.Tasks;
using ClientPacket.Game;
using ClientPacket.Pvp;
using Cs.Engine.Network.Buffer;
using Cs.Engine.Util;
using Cs.Logging;
using Cs.Protocol;
using NKC;
using NKM;

namespace Cs.GameServer.Replay;

internal sealed class ReplayRecorder
{
	public static string ReplaySavePath = Path.Combine(NKCLogManager.GetSavePath(), "Replay");

	private readonly ReplayData replayData = new ReplayData();

	public ReplayRecorder(string name, NKMGameData _gameData, NKMGameRuntimeData _runtimeData)
	{
		replayData = new ReplayData
		{
			replayName = name,
			replayVersion = "RV006",
			dateTime = NKCSynchronizedTime.ServiceTime,
			protocolVersion = 960,
			streamID = -1,
			gameData = _gameData.DeepCopy(),
			gameRuntimeData = _runtimeData.DeepCopy()
		};
		Log.Debug($"[Replay] GameType Debug  ReplayGameData[{replayData.gameData.GetGameType()}] OrgGameData[{_gameData.GetGameType()}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Replay/ReplayRecorder.cs", 33);
	}

	public void AddEmoticonData(ReplayData.EmoticonData data)
	{
		replayData.emoticonList.Add(data);
		Log.Debug($"[Replay] record emoticon data... count:{replayData.emoticonList.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Replay/ReplayRecorder.cs", 39);
	}

	public void AddSyncData(NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT data)
	{
		replayData.syncList.Add(data);
	}

	public void SetGameResult(PVP_RESULT pvpResult, float gameEndTime, NKMGameRecord gameRecord)
	{
		replayData.pvpResult = pvpResult;
		replayData.gameEndTime = gameEndTime;
		replayData.gameRecord = gameRecord;
	}

	public static void WriteReplayDataToFile(string userUIDString, ReplayData replayData)
	{
		ZeroCopyBuffer zeroCopyBuffer = new ZeroCopyBuffer();
		using (PacketWriter packetWriter = new PacketWriter(zeroCopyBuffer.GetWriter()))
		{
			packetWriter.PutString("RV006");
			packetWriter.PutWithoutNullBit(replayData);
		}
		using (zeroCopyBuffer.Hold())
		{
			int bytes = zeroCopyBuffer.CalcTotalSize();
			zeroCopyBuffer.Lz4Compress();
			int bytes2 = zeroCopyBuffer.CalcTotalSize();
			Log.Debug($"[ReplayRecoder] totalSize:{bytes.ToByteFormat()} compressedSize:{bytes2.ToByteFormat()} #syncData:{replayData.syncList.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Replay/ReplayRecorder.cs", 69);
			string filePath = Path.Combine(ReplaySavePath, userUIDString);
			zeroCopyBuffer.WriteToFile(filePath, replayData.replayName + ".replay");
		}
	}

	public async Task FinishAsync(string userUIDString)
	{
		ZeroCopyBuffer zeroCopyBuffer = new ZeroCopyBuffer();
		using (PacketWriter packetWriter = new PacketWriter(zeroCopyBuffer.GetWriter()))
		{
			packetWriter.PutString("RV006");
			packetWriter.PutWithoutNullBit(replayData);
		}
		using (zeroCopyBuffer.Hold())
		{
			int bytes = zeroCopyBuffer.CalcTotalSize();
			zeroCopyBuffer.Lz4Compress();
			int bytes2 = zeroCopyBuffer.CalcTotalSize();
			Log.Debug($"[ReplayRecoder] totalSize:{bytes.ToByteFormat()} compressedSize:{bytes2.ToByteFormat()} #syncData:{replayData.syncList.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Replay/ReplayRecorder.cs", 90);
			string filePath = Path.Combine(ReplaySavePath, userUIDString);
			await zeroCopyBuffer.WriteToFileAsync(filePath, replayData.replayName + ".replay");
		}
		CloseSyncNotPackets();
	}

	private void CloseSyncNotPackets()
	{
		foreach (NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT sync in replayData.syncList)
		{
			NKCPacketObjectPool.CloseObject(sync);
		}
	}
}
