using TMPro;
using UnityEngine;

namespace NKC.UI.HUD;

public class NKCUIHudRespawnCostAddEvent : MonoBehaviour
{
	public TextMeshProUGUI lbCost;

	public Animator animator;

	public bool Idle => !base.gameObject.activeInHierarchy;

	public void Play(float value)
	{
		base.gameObject.SetActive(value: true);
		base.transform.localPosition = Vector3.zero;
		NKCUtil.SetLabelText(lbCost, "{0:+0.##;-0.##;0}", value);
		if (animator != null)
		{
			animator.Rebind();
			animator.Update(0f);
		}
	}

	private void Update()
	{
		if (animator == null)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
