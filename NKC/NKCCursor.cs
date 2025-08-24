using Cs.Logging;
using UnityEngine;

namespace NKC;

public class NKCCursor : MonoBehaviour
{
	public Texture2D m_cursorDefault;

	public Texture2D m_cursorLarge;

	public void SetMouseCursor()
	{
		switch (NKCScenManager.GetScenManager().GetGameOptionData().eMouseCursorState)
		{
		case NKCGameOptionDataSt.GameOptionMouseCursorState.Default:
			if (m_cursorDefault == null)
			{
				Log.Error("Default cursor texture is null.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCursor.cs", 21);
			}
			Cursor.SetCursor(m_cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
			break;
		case NKCGameOptionDataSt.GameOptionMouseCursorState.Large:
			if (m_cursorLarge == null)
			{
				Log.Error("Large cursor texture is null.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCursor.cs", 28);
			}
			Cursor.SetCursor(m_cursorLarge, Vector2.zero, CursorMode.ForceSoftware);
			break;
		default:
			Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
			break;
		}
	}
}
