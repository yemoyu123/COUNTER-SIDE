using ClientPacket.WorldMap;

namespace NKM;

public static class NKMWorldMapEventGroupEx
{
	public static void Clear(this NKMWorldMapEventGroup data)
	{
		data.worldmapEventID = 0;
		data.eventUid = 0L;
	}
}
