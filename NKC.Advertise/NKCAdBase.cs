using UnityEngine;

namespace NKC.Advertise;

public abstract class NKCAdBase : MonoBehaviour
{
	public enum NKC_ADMOB_ERROR_CODE
	{
		NARC_FAILED_TO_LOAD,
		NARC_FAILED_TO_SHOW
	}

	public enum AD_TYPE
	{
		CREDIT,
		ETERNIUM,
		UNIT_INV,
		EQUIP_INV,
		SHIP_INV,
		OPERATOR_INV
	}

	public delegate bool OnUserEarnedReward();

	public delegate void OnAdFailedToShowAd(NKC_ADMOB_ERROR_CODE resultCode, string message);

	private static GameObject objInstance;

	private static NKCAdBase adInstance;

	public static NKCAdBase Instance
	{
		get
		{
			if (adInstance == null)
			{
				CreateInstance();
			}
			return adInstance;
		}
	}

	private static void CreateInstance()
	{
		if (objInstance == null)
		{
			objInstance = new GameObject("NKCAdvertise");
			Object.DontDestroyOnLoad(objInstance);
		}
		adInstance = objInstance.AddComponent<NKCAdNone>();
	}

	public static void InitInstance(bool bAskUserConsent)
	{
		Instance.Initialize(bAskUserConsent);
	}

	public virtual bool IsAdvertiseEnabled()
	{
		return false;
	}

	public virtual void ShowPrivacyOptionsForm()
	{
	}

	public virtual bool IsPrivacyOptionsRequired()
	{
		return false;
	}

	public virtual void ConsentReset()
	{
	}

	public abstract void Initialize(bool bAskUserConsent);

	public abstract void WatchRewardedAd(AD_TYPE adType, OnUserEarnedReward onUserEarnedReward, OnAdFailedToShowAd onFailedToShowAd);
}
