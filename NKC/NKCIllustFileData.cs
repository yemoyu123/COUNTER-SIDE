namespace NKC;

public struct NKCIllustFileData
{
	public readonly string m_BGThumbnailFileName;

	public readonly string m_BGFileName;

	public readonly string m_GameObjectBGAniName;

	public NKCIllustFileData(string BGThumbnailFileName, string BGFileName, string BGAniName)
	{
		m_BGThumbnailFileName = BGThumbnailFileName;
		m_BGFileName = BGFileName;
		m_GameObjectBGAniName = BGAniName;
	}
}
