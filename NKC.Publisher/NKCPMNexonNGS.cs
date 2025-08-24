using ClientPacket.Account;
using UnityEngine;

namespace NKC.Publisher;

public class NKCPMNexonNGS
{
	private static string s_NPA_code = "";

	public static void SetNpaCode(string data)
	{
		s_NPA_code = data;
	}

	public static string GetNpaCode()
	{
		return s_NPA_code;
	}

	public static void OnRecv(NKMPacket_NEXON_NGS_DATA_NOT cNKMPacket_NEXON_NGS_DATA_NOT)
	{
		Debug.Log("OnRecv NKMPacket_NEXON_NGS_DATA_NOT length : " + cNKMPacket_NEXON_NGS_DATA_NOT.buffer.Length);
	}
}
