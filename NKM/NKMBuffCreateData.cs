namespace NKM;

public struct NKMBuffCreateData
{
	public string m_buffID;

	public byte m_buffStatLevel;

	public byte m_buffTimeLevel;

	public short m_masterGameUnitUID;

	public bool m_bUseMasterStat;

	public bool m_bRangeSon;

	public bool m_stateEndRemove;

	public byte m_overlapCount;

	public NKMBuffCreateData(string _buffID, byte _buffStatLevel, byte _buffTimeLevel, short _masterGameUnitUID, bool _bUseMasterStat, bool _bRangeSon, bool _stateEndRemove, byte _overlapCount)
	{
		m_buffID = _buffID;
		m_buffStatLevel = _buffStatLevel;
		m_buffTimeLevel = _buffTimeLevel;
		m_masterGameUnitUID = _masterGameUnitUID;
		m_bUseMasterStat = _bUseMasterStat;
		m_bRangeSon = _bRangeSon;
		m_stateEndRemove = _stateEndRemove;
		m_overlapCount = _overlapCount;
	}
}
