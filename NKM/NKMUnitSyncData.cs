using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMUnitSyncData : ISerializable
{
	public struct InvokedTriggerInfo : ISerializable
	{
		public short masterUnit;

		public int triggerId;

		public InvokedTriggerInfo(short _masterUnitID, int _triggerId)
		{
			masterUnit = _masterUnitID;
			triggerId = _triggerId;
		}

		public InvokedTriggerInfo(NKMUnit _unit, int _triggerId)
		{
			masterUnit = _unit.GetUnitDataGame().m_GameUnitUID;
			triggerId = _triggerId;
		}

		public void Serialize(IPacketStream stream)
		{
			stream.PutOrGet(ref masterUnit);
			stream.PutOrGet(ref triggerId);
		}
	}

	public struct ReactionSync : ISerializable
	{
		public short masterUnitID;

		public int ID;

		public int m_currentCount;

		public float m_fTimeLeft;

		public bool Remove => m_currentCount < 0;

		public ReactionSync(NKMUnit.ReactionInstance fbInstance)
		{
			masterUnitID = fbInstance.masterUnitID;
			ID = fbInstance.ID;
			m_currentCount = ((!fbInstance.m_bFinished) ? fbInstance.m_currentCount : (-1));
			m_fTimeLeft = fbInstance.m_fTimeLeft;
		}

		public void SetData(NKMUnit.ReactionInstance fbInstance)
		{
			masterUnitID = fbInstance.masterUnitID;
			ID = fbInstance.ID;
			m_currentCount = ((!fbInstance.m_bFinished) ? fbInstance.m_currentCount : (-1));
			m_fTimeLeft = fbInstance.m_fTimeLeft;
		}

		public void Serialize(IPacketStream stream)
		{
			stream.PutOrGet(ref masterUnitID);
			stream.PutOrGet(ref ID);
			stream.PutOrGet(ref m_currentCount);
			stream.PutOrGet(ref m_fTimeLeft);
		}
	}

	private byte m_DataEncryptSeed;

	public NKM_UNIT_PLAY_STATE m_NKM_UNIT_PLAY_STATE;

	public List<NKMUnitEventSyncData> m_listNKM_UNIT_EVENT_MARK = new List<NKMUnitEventSyncData>();

	public bool m_bRespawnThisFrame;

	public bool m_bRespawnUsedRollback;

	public short m_GameUnitUID;

	public short m_TargetUID;

	public short m_SubTargetUID;

	private float m_fHP;

	public float m_PosX;

	public float m_PosZ;

	public float m_JumpYPos;

	public ushort m_usSpeedX;

	public ushort m_usSpeedY;

	public ushort m_usSpeedZ;

	public bool m_bRight = true;

	public byte m_StateID;

	public sbyte m_StateChangeCount;

	public bool m_bDamageSpeedXNegative;

	public bool m_bAttackerZUp;

	public ushort m_usDamageSpeedX;

	public ushort m_usDamageSpeedZ;

	public ushort m_usDamageSpeedJumpY;

	public ushort m_usDamageSpeedKeepTimeX;

	public ushort m_usDamageSpeedKeepTimeZ;

	public ushort m_usDamageSpeedKeepTimeJumpY;

	public ushort m_usSkillCoolTime;

	public ushort m_usHyperSkillCoolTime;

	public short m_CatcherGameUnitUID;

	public List<NKMDamageData> m_listDamageData = new List<NKMDamageData>();

	public Dictionary<short, NKMBuffSyncData> m_dicBuffData = new Dictionary<short, NKMBuffSyncData>();

	public List<NKMUnitStatusTimeSyncData> m_listStatusTimeData = new List<NKMUnitStatusTimeSyncData>();

	public List<InvokedTriggerInfo> m_listInvokedTrigger = new List<InvokedTriggerInfo>();

	public Dictionary<string, int> m_dicEventVariables = new Dictionary<string, int>();

	public List<ReactionSync> m_listUpdatedReaction = new List<ReactionSync>();

	public float m_fSavedPosX;

	public float m_fSavedPosY;

	public NKMUnitSyncData()
	{
		m_DataEncryptSeed = (byte)NKMRandom.Range(10, 100);
		SetHP(0f);
	}

	public void RespawnInit(bool bUsedRollback)
	{
		m_bRespawnThisFrame = true;
		m_bRespawnUsedRollback = bUsedRollback;
		m_TargetUID = 0;
		m_SubTargetUID = 0;
		m_usSpeedX = 0;
		m_usSpeedY = 0;
		m_usSpeedZ = 0;
		m_usDamageSpeedX = 0;
		m_usDamageSpeedZ = 0;
		m_usDamageSpeedJumpY = 0;
		m_usDamageSpeedKeepTimeX = 0;
		m_usDamageSpeedKeepTimeZ = 0;
		m_usDamageSpeedKeepTimeJumpY = 0;
		m_usSkillCoolTime = 0;
		m_usHyperSkillCoolTime = 0;
		m_StateID = 0;
		m_CatcherGameUnitUID = 0;
		m_listDamageData.Clear();
		m_listStatusTimeData.Clear();
		m_listUpdatedReaction.Clear();
		m_fSavedPosX = 0f;
		m_fSavedPosY = 0f;
	}

	public void Encrypt()
	{
		float hP = GetHP();
		m_DataEncryptSeed = (byte)NKMRandom.Range(10, 100);
		SetHP(hP);
	}

	public float GetHP()
	{
		return m_fHP - (float)(int)m_DataEncryptSeed;
	}

	public void SetHP(float fHP)
	{
		m_fHP = fHP + (float)(int)m_DataEncryptSeed;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_DataEncryptSeed);
		stream.PutOrGetEnum(ref m_NKM_UNIT_PLAY_STATE);
		stream.PutOrGet(ref m_listNKM_UNIT_EVENT_MARK);
		stream.PutOrGet(ref m_bRespawnThisFrame);
		stream.PutOrGet(ref m_bRespawnUsedRollback);
		stream.PutOrGet(ref m_GameUnitUID);
		stream.PutOrGet(ref m_TargetUID);
		stream.PutOrGet(ref m_SubTargetUID);
		stream.PutOrGet(ref m_fHP);
		stream.PutOrGet(ref m_PosX);
		stream.PutOrGet(ref m_PosZ);
		stream.PutOrGet(ref m_JumpYPos);
		stream.PutOrGet(ref m_usSpeedX);
		stream.PutOrGet(ref m_usSpeedY);
		stream.PutOrGet(ref m_usSpeedZ);
		stream.PutOrGet(ref m_bRight);
		stream.PutOrGet(ref m_StateID);
		stream.PutOrGet(ref m_StateChangeCount);
		stream.PutOrGet(ref m_bDamageSpeedXNegative);
		stream.PutOrGet(ref m_bAttackerZUp);
		stream.PutOrGet(ref m_usDamageSpeedX);
		stream.PutOrGet(ref m_usDamageSpeedZ);
		stream.PutOrGet(ref m_usDamageSpeedJumpY);
		stream.PutOrGet(ref m_usDamageSpeedKeepTimeX);
		stream.PutOrGet(ref m_usDamageSpeedKeepTimeZ);
		stream.PutOrGet(ref m_usDamageSpeedKeepTimeJumpY);
		stream.PutOrGet(ref m_usSkillCoolTime);
		stream.PutOrGet(ref m_usHyperSkillCoolTime);
		stream.PutOrGet(ref m_CatcherGameUnitUID);
		stream.PutOrGet(ref m_listDamageData);
		stream.PutOrGet(ref m_dicBuffData);
		stream.PutOrGet(ref m_listStatusTimeData);
		stream.PutOrGet(ref m_listInvokedTrigger);
		stream.PutOrGet(ref m_dicEventVariables);
		stream.PutOrGet(ref m_listUpdatedReaction);
		stream.PutOrGet(ref m_fSavedPosX);
		stream.PutOrGet(ref m_fSavedPosY);
	}

	public void DeepCopyWithoutDamageAndMarkFrom(NKMUnitSyncData source)
	{
		m_DataEncryptSeed = source.m_DataEncryptSeed;
		m_NKM_UNIT_PLAY_STATE = source.m_NKM_UNIT_PLAY_STATE;
		m_bRespawnThisFrame = source.m_bRespawnThisFrame;
		m_bRespawnUsedRollback = source.m_bRespawnUsedRollback;
		m_GameUnitUID = source.m_GameUnitUID;
		m_TargetUID = source.m_TargetUID;
		m_SubTargetUID = source.m_SubTargetUID;
		m_fHP = source.m_fHP;
		m_PosX = source.m_PosX;
		m_PosZ = source.m_PosZ;
		m_JumpYPos = source.m_JumpYPos;
		m_usSpeedX = source.m_usSpeedX;
		m_usSpeedY = source.m_usSpeedY;
		m_usSpeedZ = source.m_usSpeedZ;
		m_bRight = source.m_bRight;
		m_StateID = source.m_StateID;
		m_StateChangeCount = source.m_StateChangeCount;
		m_bDamageSpeedXNegative = source.m_bDamageSpeedXNegative;
		m_bAttackerZUp = source.m_bAttackerZUp;
		m_usDamageSpeedX = source.m_usDamageSpeedX;
		m_usDamageSpeedZ = source.m_usDamageSpeedZ;
		m_usDamageSpeedJumpY = source.m_usDamageSpeedJumpY;
		m_usDamageSpeedKeepTimeX = source.m_usDamageSpeedKeepTimeX;
		m_usDamageSpeedKeepTimeZ = source.m_usDamageSpeedKeepTimeZ;
		m_usDamageSpeedKeepTimeJumpY = source.m_usDamageSpeedKeepTimeJumpY;
		m_CatcherGameUnitUID = source.m_CatcherGameUnitUID;
		m_usSkillCoolTime = source.m_usSkillCoolTime;
		m_usHyperSkillCoolTime = source.m_usHyperSkillCoolTime;
		m_fSavedPosX = source.m_fSavedPosX;
		m_fSavedPosY = source.m_fSavedPosY;
	}
}
