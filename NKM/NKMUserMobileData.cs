using Cs.Protocol;

namespace NKM;

public sealed class NKMUserMobileData : ISerializable
{
	public const string NxPcMarketId = "WINDOWS";

	public static readonly NKMUserMobileData DevDefault;

	public string m_MarketId;

	public string m_Country;

	public string m_Language;

	public string m_AuthPlatform;

	public string m_Platform;

	public string m_OsVersion;

	public string m_AdId;

	public string m_ClientVersion;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_MarketId);
		stream.PutOrGet(ref m_Country);
		stream.PutOrGet(ref m_Language);
		stream.PutOrGet(ref m_AuthPlatform);
		stream.PutOrGet(ref m_Platform);
		stream.PutOrGet(ref m_OsVersion);
		stream.PutOrGet(ref m_AdId);
		stream.PutOrGet(ref m_ClientVersion);
	}

	public short GetMarketID()
	{
		return m_MarketId switch
		{
			"Google Play Store" => 1, 
			"Apple App Store" => 2, 
			"One Store" => 3, 
			"WINDOWS" => 200, 
			_ => 0, 
		};
	}

	public static string GetMarketStr(short index)
	{
		return index switch
		{
			1 => "Google Play Store", 
			2 => "Apple App Store", 
			3 => "One Store", 
			200 => "WINDOWS", 
			_ => "Unknown", 
		};
	}

	public ClientOsType GetOsType()
	{
		switch (m_MarketId)
		{
		case "Google Play Store":
			return ClientOsType.Android;
		case "Apple App Store":
			return ClientOsType.iOS;
		case "One Store":
			return ClientOsType.Android;
		case "Steam":
		case "WINDOWS":
			return ClientOsType.Windows;
		default:
			return ClientOsType.Unknown;
		}
	}

	public ClientOsNxLogType GetNxLogOsType()
	{
		return m_MarketId switch
		{
			"Google Play Store" => ClientOsNxLogType.Android, 
			"Apple App Store" => ClientOsNxLogType.iOS, 
			"One Store" => ClientOsNxLogType.Android, 
			"WINDOWS" => ClientOsNxLogType.Windows, 
			_ => ClientOsNxLogType.Unknown, 
		};
	}

	public string GetMarketStrID()
	{
		return m_MarketId switch
		{
			"Google Play Store" => "GPS", 
			"Apple App Store" => "AAS", 
			"One Store" => "ONE", 
			_ => string.Empty, 
		};
	}

	public byte GetAuthPlatformID()
	{
		byte result = 0;
		switch (m_AuthPlatform)
		{
		case "NexonPlay":
			result = 1;
			break;
		case "KaKao":
			result = 2;
			break;
		case "NPA":
			result = 3;
			break;
		case "Nexon.com":
			result = 4;
			break;
		case "inner":
			result = 5;
			break;
		}
		return result;
	}

	public string GetPlatformStrID()
	{
		return m_Platform switch
		{
			"Android" => "A", 
			"IPhonePlayer" => "I", 
			_ => string.Empty, 
		};
	}

	public string GetPlatformStrIDForNGSM()
	{
		return m_Platform switch
		{
			"Android" => "AOS", 
			"IPhonePlayer" => "IOS", 
			_ => string.Empty, 
		};
	}

	public string GetCountryCode()
	{
		switch (m_Country)
		{
		case "KO_KR":
		case "Korea":
			return "KR";
		default:
			return m_Country;
		}
	}

	static NKMUserMobileData()
	{
		DevDefault = new NKMUserMobileData
		{
			m_MarketId = "DevDefault",
			m_Country = "DevDefault",
			m_Language = "DevDefault",
			m_AuthPlatform = "DevDefault",
			m_Platform = "DevDefault",
			m_OsVersion = "DevDefault",
			m_AdId = "DevDefault",
			m_ClientVersion = "DevDefault"
		};
	}
}
