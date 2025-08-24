using UnityEngine;

namespace NKC.UI.Option;

public abstract class NKCUIGameOptionContentBase : MonoBehaviour
{
	public abstract void Init();

	public abstract void SetContent();

	public virtual bool Processhotkey(HotkeyEventType eventType)
	{
		return false;
	}
}
