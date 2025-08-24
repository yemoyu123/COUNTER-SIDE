using System;
using UnityEngine;

namespace NKC.UI;

[Serializable]
public class BaseSceneMenuBtn
{
	[Serializable]
	public class BaseSceneMenuSubBtn
	{
		public NKC_SCEN_BASE.eUIOpenReserve Type;

		public NKCUIComStateButton Btn;

		public NKCUIComStateButton LockedBtn;

		public GameObject m_objEvent;
	}

	[Header("서브 메뉴별 설정")]
	public NKCUIBaseSceneMenu.BaseSceneMenuType Type;

	public GameObject obj;

	public Animator animator;

	public Sprite spBackground;

	public BaseSceneMenuSubBtn[] subBtn;
}
