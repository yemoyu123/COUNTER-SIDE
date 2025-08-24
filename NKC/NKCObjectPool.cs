using Cs.Logging;
using NKM;

namespace NKC;

public class NKCObjectPool : NKMObjectPool
{
	public override void Unload()
	{
		base.Unload();
	}

	protected override NKMObjectPoolData CreateNewObj(NKM_OBJECT_POOL_TYPE typeT, string bundleName = "", string name = "", bool bAsync = false)
	{
		NKMObjectPoolData nKMObjectPoolData = null;
		nKMObjectPoolData = base.CreateNewObj(typeT, bundleName, name, bAsync);
		if (nKMObjectPoolData != null)
		{
			return nKMObjectPoolData;
		}
		switch (typeT)
		{
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCDamageEffect:
			nKMObjectPoolData = new NKCDamageEffect();
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCASMaterial:
			nKMObjectPoolData = new NKCASMaterial(bundleName, name, bAsync);
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCASLensFlare:
			nKMObjectPoolData = new NKCASLensFlare(bundleName, name, bAsync);
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitSprite:
			nKMObjectPoolData = new NKCASUnitSprite(bundleName, name, bAsync);
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitSpineSprite:
			nKMObjectPoolData = new NKCASUnitSpineSprite(bundleName, name, bAsync);
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitViewerSpineSprite:
			nKMObjectPoolData = new NKCASUnitViewerSpineSprite(bundleName, name, bAsync);
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitShadow:
			nKMObjectPoolData = new NKCASUnitShadow(bAsync);
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitMiniMapFace:
			nKMObjectPoolData = new NKCASUnitMiniMapFace(bundleName, name, bAsync);
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCASUISpineIllust:
			nKMObjectPoolData = new NKCASUISpineIllust(bundleName, name, bAsync);
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect:
			nKMObjectPoolData = new NKCASEffect(bundleName, name, bAsync);
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCUnitClient:
			nKMObjectPoolData = new NKCUnitClient();
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCUnitViewer:
			nKMObjectPoolData = new NKCUnitViewer();
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCEffectReserveData:
			nKMObjectPoolData = new NKCEffectReserveData();
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKCASDangerChargeUI:
			nKMObjectPoolData = new NKCASDangerChargeUI(bAsync);
			break;
		default:
			Log.Error($"CreateNewObj null typeT:{typeT}, bundleName:{bundleName}, name:{name}, bAsync:{bAsync}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCObjectPool.cs", 90);
			break;
		}
		return nKMObjectPoolData;
	}

	public override void Update()
	{
		if (!NKCAssetResourceManager.IsLoadEnd())
		{
			return;
		}
		for (int i = 0; i < 100 && m_qAsyncLoadObject.Count != 0 && NKCAssetResourceManager.IsLoadEnd(); i++)
		{
			NKMObjectPoolData nKMObjectPoolData = m_qAsyncLoadObject.Dequeue();
			if (!nKMObjectPoolData.LoadComplete())
			{
				nKMObjectPoolData.Load(bAsync: false);
				if (!nKMObjectPoolData.LoadComplete())
				{
					Log.Info("LoadComplete Fail " + nKMObjectPoolData.m_ObjectPoolName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCObjectPool.cs", 118);
					continue;
				}
			}
			nKMObjectPoolData.m_bIsLoaded = true;
			nKMObjectPoolData.Open();
		}
	}
}
