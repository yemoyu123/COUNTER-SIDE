using UnityEngine;

namespace NKC.UI.Tooltip;

public abstract class NKCUITooltipBase : MonoBehaviour
{
	public abstract void Init();

	public abstract void SetData(NKCUITooltip.Data data);

	public virtual void SetData(string title, string desc)
	{
	}
}
