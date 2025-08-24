using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleMail : NKCUIDevConsoleContentBase
{
	[Serializable]
	public class PostItem
	{
		public Dropdown ddType;

		public NKCUIComStateButton btnSearch;

		public Text txtName;

		public InputField ifCount;
	}

	public Dropdown m_ddType;

	public InputField m_ifTitle;

	public InputField m_ifDesc;

	public InputField m_ifExpiration;

	public List<PostItem> m_itemList;

	public NKCUIComStateButton m_btnOK;
}
