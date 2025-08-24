using UnityEngine;

namespace NKC;

public abstract class NKCUICharacterViewEffectBase : MonoBehaviour
{
	public virtual Color EffectColor { get; } = Color.white;

	public abstract void SetEffect(NKCASUIUnitIllust unitIllust, Transform trOriginalRoot);

	public abstract void CleanUp(NKCASUIUnitIllust unitIllust, Transform trOriginalRoot);

	public abstract void SetColor(Color color);

	public abstract void SetColor(float fR = -1f, float fG = -1f, float fB = -1f, float fA = -1f);
}
