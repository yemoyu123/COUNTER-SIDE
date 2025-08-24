namespace AssetBundles;

public struct BundleNameInfo
{
	public string bundleName;

	public string variant;

	public BundleNameInfo(string _bundlename, string _variant)
	{
		bundleName = _bundlename;
		variant = _variant;
	}
}
