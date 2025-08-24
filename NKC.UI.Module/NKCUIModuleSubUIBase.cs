using NKM.Event;
using UnityEngine;

namespace NKC.UI.Module;

public class NKCUIModuleSubUIBase : MonoBehaviour
{
	public int ModuleID = -1;

	public virtual void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
	}

	public virtual void OnOpen(NKMEventTabTemplet eventTabTemplet)
	{
	}

	public virtual void OnClose()
	{
	}

	public virtual void Init()
	{
	}

	public virtual void Refresh()
	{
	}

	public virtual void UnHide()
	{
	}

	public virtual void Hide()
	{
	}

	public virtual bool OnBackButton()
	{
		return false;
	}

	public virtual void PassData(NKCUIModuleHome.EventModuleMessageDataBase data)
	{
	}
}
