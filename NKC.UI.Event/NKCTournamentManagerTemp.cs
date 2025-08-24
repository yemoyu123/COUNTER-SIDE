using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Game;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCTournamentManagerTemp
{
	private static int tournamentId = 101;

	private static NKMTournamentState state;

	private static List<NKMTournamentInfo> tournamentInfo;

	private static List<NKMTournamentInfo> tournamentInfoPredict;

	public static int TournamentId => tournamentId;

	public static NKMTournamentState State => state;

	public static List<NKMTournamentInfo> TournamentInfo => tournamentInfo;

	public static List<NKMTournamentInfo> TournamentInfoPredict => tournamentInfoPredict;

	public static void SetTournamentPredictInfo(List<NKMTournamentInfo> predictInfo)
	{
		tournamentInfoPredict = predictInfo;
	}

	public static void SetTournamentPredictInfo(List<long> cheeringUIdList, NKMTournamentGroups group)
	{
		NKMTournamentInfo nKMTournamentInfo = tournamentInfo?.Find((NKMTournamentInfo e) => (int)e.groupIndex % 10 == (int)group % 10);
		if (nKMTournamentInfo == null)
		{
			Debug.LogError("NKMTournamentInfo not exist");
			return;
		}
		NKMTournamentInfo predictInfo = new NKMTournamentInfo();
		predictInfo.groupIndex = group;
		cheeringUIdList.ForEach(delegate(long e)
		{
			predictInfo.slotUserUid.Add(e);
		});
		if (tournamentInfoPredict == null)
		{
			tournamentInfoPredict = new List<NKMTournamentInfo>();
		}
		int count = cheeringUIdList.Count;
		for (int num = 0; num < count; num++)
		{
			long num2 = cheeringUIdList[num];
			if (num2 > 0)
			{
				nKMTournamentInfo.userInfo.TryGetValue(num2, out var value);
				if (value == null)
				{
					Debug.LogError($"NKMTournamentProfileData (UID:{num2}) not exist");
				}
				if (!predictInfo.userInfo.ContainsKey(num2))
				{
					predictInfo.userInfo.Add(num2, value);
				}
			}
		}
		int num3 = tournamentInfoPredict.FindIndex((NKMTournamentInfo e) => (int)e.groupIndex % 10 == (int)group % 10);
		if (num3 < 0)
		{
			tournamentInfoPredict.Add(predictInfo);
		}
		else
		{
			tournamentInfoPredict[num3] = predictInfo;
		}
	}

	public static NKMTournamentInfo GetTournamentInfo(NKMTournamentGroups group)
	{
		if (tournamentInfo == null)
		{
			tournamentInfo = new List<NKMTournamentInfo>();
		}
		NKMTournamentInfo nKMTournamentInfo = tournamentInfo.Find((NKMTournamentInfo e) => (int)e.groupIndex % 10 == (int)group % 10);
		if (nKMTournamentInfo != null)
		{
			return nKMTournamentInfo;
		}
		NKMTournamentInfo nKMTournamentInfo2 = new NKMTournamentInfo();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		nKMTournamentInfo2.groupIndex = group;
		if (group != NKMTournamentGroups.Finals && group != NKMTournamentGroups.GlobalFinals)
		{
			int num = 7;
			for (int num2 = 0; num2 < num; num2++)
			{
				nKMTournamentInfo2.slotUserUid.Add(0L);
			}
			for (int num3 = 1; num3 <= 8; num3++)
			{
				NKMTournamentProfileData nKMTournamentProfileData = new NKMTournamentProfileData();
				nKMTournamentProfileData.commonProfile.userUid = nKMUserData.UserProfileData.commonProfile.userUid;
				nKMTournamentProfileData.commonProfile.friendCode = nKMUserData.UserProfileData.commonProfile.friendCode;
				nKMTournamentProfileData.commonProfile.level = nKMUserData.UserProfileData.commonProfile.level;
				nKMTournamentProfileData.commonProfile.nickname = num3.ToString();
				nKMTournamentProfileData.commonProfile.titleId = nKMUserData.UserProfileData.commonProfile.titleId;
				nKMTournamentProfileData.commonProfile.mainUnitId = nKMUserData.UserProfileData.commonProfile.mainUnitId;
				nKMTournamentProfileData.commonProfile.mainUnitSkinId = nKMUserData.UserProfileData.commonProfile.mainUnitSkinId;
				nKMTournamentProfileData.commonProfile.mainUnitTacticLevel = nKMUserData.UserProfileData.commonProfile.mainUnitTacticLevel;
				nKMTournamentProfileData.commonProfile.frameId = nKMUserData.UserProfileData.commonProfile.frameId;
				nKMTournamentProfileData.guildData = nKMUserData.UserProfileData.guildData;
				nKMTournamentProfileData.deck = nKMUserData.UserProfileData.defenceDeck;
				if (num3 > 1)
				{
					nKMTournamentProfileData.commonProfile.userUid = num3;
				}
				nKMTournamentInfo2.userInfo.Add(nKMTournamentProfileData.commonProfile.userUid, nKMTournamentProfileData);
				nKMTournamentInfo2.slotUserUid.Add(nKMTournamentProfileData.commonProfile.userUid);
			}
			nKMTournamentInfo2.slotUserUid[0] = nKMTournamentInfo2.slotUserUid[7];
			nKMTournamentInfo2.slotUserUid[1] = nKMTournamentInfo2.slotUserUid[7];
			nKMTournamentInfo2.slotUserUid[2] = nKMTournamentInfo2.slotUserUid[11];
			nKMTournamentInfo2.slotUserUid[3] = nKMTournamentInfo2.slotUserUid[7];
			nKMTournamentInfo2.slotUserUid[4] = nKMTournamentInfo2.slotUserUid[9];
			nKMTournamentInfo2.slotUserUid[5] = nKMTournamentInfo2.slotUserUid[11];
			nKMTournamentInfo2.slotUserUid[6] = nKMTournamentInfo2.slotUserUid[13];
		}
		else
		{
			int num4 = 3;
			for (int num5 = 0; num5 < num4; num5++)
			{
				nKMTournamentInfo2.slotUserUid.Add(0L);
			}
			for (int num6 = 1; num6 <= 4; num6++)
			{
				NKMTournamentProfileData nKMTournamentProfileData2 = new NKMTournamentProfileData();
				nKMTournamentProfileData2.commonProfile.userUid = nKMUserData.UserProfileData.commonProfile.userUid;
				nKMTournamentProfileData2.commonProfile.friendCode = nKMUserData.UserProfileData.commonProfile.friendCode;
				nKMTournamentProfileData2.commonProfile.level = nKMUserData.UserProfileData.commonProfile.level;
				nKMTournamentProfileData2.commonProfile.nickname = num6.ToString();
				nKMTournamentProfileData2.commonProfile.titleId = nKMUserData.UserProfileData.commonProfile.titleId;
				nKMTournamentProfileData2.commonProfile.mainUnitId = nKMUserData.UserProfileData.commonProfile.mainUnitId;
				nKMTournamentProfileData2.commonProfile.mainUnitSkinId = nKMUserData.UserProfileData.commonProfile.mainUnitSkinId;
				nKMTournamentProfileData2.commonProfile.mainUnitTacticLevel = nKMUserData.UserProfileData.commonProfile.mainUnitTacticLevel;
				nKMTournamentProfileData2.commonProfile.frameId = nKMUserData.UserProfileData.commonProfile.frameId;
				nKMTournamentProfileData2.guildData = nKMUserData.UserProfileData.guildData;
				nKMTournamentProfileData2.deck = nKMUserData.UserProfileData.defenceDeck;
				if (num6 > 1)
				{
					nKMTournamentProfileData2.commonProfile.userUid = num6;
				}
				nKMTournamentInfo2.userInfo.Add(nKMTournamentProfileData2.commonProfile.userUid, nKMTournamentProfileData2);
				nKMTournamentInfo2.slotUserUid.Add(nKMTournamentProfileData2.commonProfile.userUid);
			}
			nKMTournamentInfo2.slotUserUid[0] = nKMTournamentInfo2.slotUserUid[3];
			nKMTournamentInfo2.slotUserUid[1] = nKMTournamentInfo2.slotUserUid[3];
			nKMTournamentInfo2.slotUserUid[2] = nKMTournamentInfo2.slotUserUid[5];
			nKMTournamentInfo2.slotUserUid.Add(nKMTournamentInfo2.slotUserUid[4]);
		}
		int num7 = tournamentInfo.FindIndex((NKMTournamentInfo e) => (int)e.groupIndex % 10 == (int)group % 10);
		if (num7 < 0)
		{
			tournamentInfo.Add(nKMTournamentInfo2);
		}
		else
		{
			tournamentInfo[num7] = nKMTournamentInfo2;
		}
		SetTournamentPredictInfo(nKMTournamentInfo2.slotUserUid, group);
		return nKMTournamentInfo2;
	}

	public static NKMTournamentInfo GetTournamentPredictInfo(NKMTournamentGroups group)
	{
		return tournamentInfoPredict?.Find((NKMTournamentInfo e) => (int)e.groupIndex % 10 == (int)group % 10);
	}

	public static NKMTournamentTemplet GetTournamentTemplet()
	{
		return NKMTournamentTemplet.Find(tournamentId);
	}
}
