using System;
using System.IO;
using ClientPacket.Pvp;
using Cs.Engine.Network.Buffer;
using Cs.Engine.Util;
using Cs.Logging;
using Cs.Protocol;

namespace Cs.GameServer.Replay;

internal static class ReplayLoader
{
	public static ReplayData Load(string fullPath)
	{
		if (!File.Exists(fullPath))
		{
			Log.Error("[ReplayData] file not exist. path:" + fullPath, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Replay/ReplayLoader.cs", 17);
			return null;
		}
		try
		{
			long bytes = 0L;
			ZeroCopyBuffer zeroCopyBuffer = null;
			using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
			{
				bytes = fileStream.Length;
				zeroCopyBuffer = Lz4Util.Decompress(fileStream);
			}
			long bytes2 = zeroCopyBuffer.CalcTotalSize();
			Log.Debug("[ReplayData] fileSize:" + bytes.ToByteFormat() + " decompressed:" + bytes2.ToByteFormat(), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Replay/ReplayLoader.cs", 32);
			using (zeroCopyBuffer.Hold())
			{
				using PacketReader packetReader = new PacketReader(zeroCopyBuffer.GetReader());
				string text = packetReader.GetString();
				if (text != "RV006")
				{
					Log.Warn("[ReplayData] version mismatched. current:RV006 saved:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Replay/ReplayLoader.cs", 40);
					return null;
				}
				ReplayData replayData = new ReplayData();
				packetReader.GetWithoutNullBit(replayData);
				Log.Debug($"[ReplayData] syncCount:{replayData.syncList.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Replay/ReplayLoader.cs", 46);
				return replayData;
			}
		}
		catch (Exception ex)
		{
			Log.Error("[ReplayData] load failed. path:" + fullPath + " exception:" + ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Replay/ReplayLoader.cs", 52);
			return null;
		}
	}
}
