using UnityEngine;

namespace NKC;

public class NKCUICamFaceBillboard : MonoBehaviour
{
	private void Update()
	{
		base.transform.LookAt(base.transform.position + NKCCamera.GetCamera().transform.rotation * Vector3.forward, NKCCamera.GetCamera().transform.rotation * Vector3.up);
	}
}
