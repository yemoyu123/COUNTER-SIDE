namespace NKC.Patcher;

public class InnerPatchInfoController
{
	public NKCPatchInfo ObbPatchInfo { get; private set; }

	public NKCPatchInfo FullBuildPatchInfo { get; private set; }

	public void LoadFullBuildManifest()
	{
		FullBuildPatchInfo = PatchManifestManager.LoadManifest(PatchManifestPath.PatchType.FullBuildManifest);
	}

	public void LoadObbManifest()
	{
		ObbPatchInfo = PatchManifestManager.LoadManifest(PatchManifestPath.PatchType.ObbManifest);
	}
}
