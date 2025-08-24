using System;
using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public class NKMMiniGameData : ISerializable
{
	public const int MAX_STRING_VALUE_LENGTH = 100;

	public NKM_MINI_GAME_TYPE type;

	public int templetId;

	public long score;

	public string gameInfo;

	public static NKMMiniGameData Create(int templetId, long score = 0L, string gameInfo = "")
	{
		NKMMiniGameData nKMMiniGameData = new NKMMiniGameData();
		NKMMiniGameTemplet templet = NKMMiniGameManager.GetTemplet(templetId);
		if (templet == null)
		{
			return null;
		}
		nKMMiniGameData.type = templet.m_MiniGameType;
		nKMMiniGameData.templetId = templetId;
		nKMMiniGameData.score = score;
		nKMMiniGameData.gameInfo = gameInfo;
		return nKMMiniGameData;
	}

	public NKM_ERROR_CODE CanApplyScoreChange(NKMMiniGameTemplet templet, NKMMiniGameData newData, DateTime current)
	{
		if (templet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MINI_GAME_TEMPLET_IS_NULL;
		}
		if (templetId != newData.templetId && newData.templetId != templet.Key)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MINI_GAME_TEMPLET_IS_NULL;
		}
		if (newData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MINI_GAME_INVALID_GAME_INFO;
		}
		if (newData.gameInfo.Length >= 100)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MINI_GAME_INVALID_GAME_INFO;
		}
		if (newData.score <= 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MINI_GAME_INVALID_SCORE;
		}
		if (score >= newData.score)
		{
			return NKM_ERROR_CODE.NEC_FAIL_MINI_GAME_INVALID_SCORE;
		}
		_ = newData.type;
		_ = 10;
		return NKM_ERROR_CODE.NEC_OK;
	}

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref type);
		stream.PutOrGet(ref templetId);
		stream.PutOrGet(ref score);
		stream.PutOrGet(ref gameInfo);
	}
}
