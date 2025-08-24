using System.Collections;
using UnityEngine;

namespace NKC.AppTracking;

public class iOSAppTrackingChecker : MonoBehaviour
{
	public delegate void CallBack(bool userAccepted);

	public void StartIDFA(CallBack callback = null)
	{
		StartCoroutine(StartIDFACoroutine(callback));
	}

	public IEnumerator StartIDFACoroutine(CallBack onFinished)
	{
		yield return null;
		onFinished?.Invoke(userAccepted: true);
	}
}
