using ClientPacket.Common;
using ClientPacket.Game;

namespace NKM;

public static class NKMPacketExt
{
	public static NKMPacket_GAME_END_NOT ConvertToGameEndNot(NKMGameEndData gameEndData)
	{
		if (gameEndData == null)
		{
			return null;
		}
		return new NKMPacket_GAME_END_NOT
		{
			win = gameEndData.win,
			giveup = gameEndData.giveup,
			restart = gameEndData.restart,
			dungeonClearData = gameEndData.dungeonClearData,
			deckIndex = gameEndData.deckIndex,
			gameRecord = gameEndData.gameRecord,
			updatedUnits = gameEndData.updatedUnits,
			costItemDataList = gameEndData.costItemDataList,
			killCountData = gameEndData.killCountData,
			killCountDelta = gameEndData.killCountDelta,
			totalPlayTime = gameEndData.totalPlayTime
		};
	}

	public static NKMGameEndData ConvertToGameEndData(NKMPacket_GAME_END_NOT gameEndNot)
	{
		if (gameEndNot == null)
		{
			return null;
		}
		return new NKMGameEndData
		{
			win = gameEndNot.win,
			giveup = gameEndNot.giveup,
			restart = gameEndNot.restart,
			dungeonClearData = gameEndNot.dungeonClearData,
			deckIndex = gameEndNot.deckIndex,
			gameRecord = gameEndNot.gameRecord,
			updatedUnits = gameEndNot.updatedUnits,
			costItemDataList = gameEndNot.costItemDataList,
			killCountData = gameEndNot.killCountData,
			killCountDelta = gameEndNot.killCountDelta,
			totalPlayTime = gameEndNot.totalPlayTime
		};
	}
}
