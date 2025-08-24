using System;
using DG.Tweening;
using UnityEngine.UI;

namespace NKC;

[Serializable]
public struct TargetStringInfoToChange
{
	public string m_Key;

	public Text m_lbText;

	public DOTweenAnimation m_doTweenAni;

	public InputField m_inputField;
}
