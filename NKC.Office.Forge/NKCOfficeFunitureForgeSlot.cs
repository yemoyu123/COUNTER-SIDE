using NKC.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.Office.Forge;

public class NKCOfficeFunitureForgeSlot : NKCOfficeSpineFurniture
{
	[Header("제작 관련")]
	public Text m_lbTime;

	public NKCUISlot m_Slot;

	[Header("모드별 오브젝트")]
	public GameObject m_objDisable;

	public GameObject m_objOpen;

	public GameObject m_objCompleted;

	public GameObject m_objBusy;
}
