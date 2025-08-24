using NKC.UI;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC;

public class NKCSystemEvent : MonoBehaviour
{
	public static void UI_SCEN_BG_DRAG(BaseEventData cBaseEventData)
	{
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			float value = NKCCamera.GetPosNowX() - pointerEventData.delta.x * 10f;
			float value2 = NKCCamera.GetPosNowY() - pointerEventData.delta.y * 10f;
			value = Mathf.Clamp(value, -100f, 100f);
			value2 = Mathf.Clamp(value2, -100f, 100f);
			NKCCamera.TrackingPos(1f, value, value2);
		}
	}

	public void UI_TEAM_SHIP_CLICK()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_TEAM || NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_DUNGEON_ATK_READY)
		{
			NKCScenManager.GetScenManager().Get_SCEN_TEAM().UI_TEAM_SHIP_CLICK();
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY && NKCUIDeckViewer.IsInstanceOpen)
		{
			NKCUIDeckViewer.Instance.DeckViewShipClick();
		}
	}

	public void UI_UNIT_INFO_CLOSE()
	{
		NKCUIUnitInfo.CheckInstanceAndClose();
	}

	public void UI_BACK_TO_SCEN_HOME()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
	}

	public void UI_BASE_HANGER_OPEN()
	{
	}

	public void UI_BASE_FORGE_OPEN()
	{
		NKCScenManager.GetScenManager().Get_SCEN_BASE().OpenFactory();
	}

	public void UI_BASE_HANGER_CLOSE()
	{
		NKCUIUnitInfo.CheckInstanceAndClose();
	}

	public void UI_GAME_CAMERA_DRAG_BEGIN(BaseEventData cBaseEventData)
	{
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_CAMERA_DRAG_BEGIN(pointerEventData.position);
		}
	}

	public void UI_GAME_CAMERA_DRAG(BaseEventData cBaseEventData)
	{
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_CAMERA_DRAG(pointerEventData.delta, pointerEventData.position);
		}
	}

	public void UI_GAME_CAMERA_DRAG_END(BaseEventData cBaseEventData)
	{
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_CAMERA_DRAG_END(pointerEventData.delta, pointerEventData.position);
		}
	}

	public static void UI_HUD_DECK_DOWN(int index)
	{
		NKCScenManager.GetScenManager().GetGameClient().UI_HUD_DECK_DOWN(index);
	}

	public static void UI_HUD_DECK_UP(int index)
	{
		NKCScenManager.GetScenManager().GetGameClient().UI_HUD_DECK_UP(index);
	}

	public static void UI_HUD_DECK_DRAG_BEGIN(BaseEventData cBaseEventData)
	{
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_HUD_DECK_DRAG_BEGIN(pointerEventData.pointerDrag.gameObject, pointerEventData.position);
		}
	}

	public static void UI_HUD_DECK_DRAG(BaseEventData cBaseEventData)
	{
		_ = cBaseEventData.selectedObject;
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_HUD_DECK_DRAG(pointerEventData.pointerDrag.gameObject, pointerEventData.position, pointerEventData.position);
		}
	}

	public static void UI_HUD_DECK_DRAG_END(BaseEventData cBaseEventData)
	{
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_HUD_DECK_DRAG_END(pointerEventData.pointerDrag.gameObject, pointerEventData.position);
		}
	}

	public static void UI_HUD_SHIP_SKILL_DECK_DOWN(int index, BaseEventData cBaseEventData)
	{
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_HUD_SHIP_SKILL_DECK_DOWN(index, pointerEventData.position);
		}
	}

	public static void UI_HUD_SHIP_SKILL_DECK_UP(int index)
	{
		NKCScenManager.GetScenManager().GetGameClient().UI_HUD_SHIP_SKILL_DECK_UP(index);
	}

	public static void UI_HUD_SHIP_SKILL_DECK_DRAG_BEGIN(BaseEventData cBaseEventData)
	{
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_HUD_SHIP_SKILL_DECK_DRAG_BEGIN(pointerEventData.pointerDrag.gameObject, pointerEventData.position);
		}
	}

	public static void UI_HUD_SHIP_SKILL_DECK_DRAG(BaseEventData cBaseEventData)
	{
		_ = cBaseEventData.selectedObject;
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_HUD_SHIP_SKILL_DECK_DRAG(pointerEventData.pointerDrag.gameObject, pointerEventData.position);
		}
	}

	public static void UI_HUD_SHIP_SKILL_DECK_DRAG_END(BaseEventData cBaseEventData)
	{
		if (cBaseEventData is PointerEventData pointerEventData)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_HUD_SHIP_SKILL_DECK_DRAG_END(pointerEventData.pointerDrag.gameObject, pointerEventData.position);
		}
	}

	public void UI_HUD_AUTO_RESPAWN()
	{
		NKCScenManager.GetScenManager().GetGameClient().UI_HUD_AUTO_RESPAWN_TOGGLE();
	}

	public void UI_HUD_ACTION_CAMERA()
	{
		NKCScenManager.GetScenManager().GetGameClient().UI_HUD_ACTION_CAMERA_TOGGLE();
	}

	public void UI_HUD_TRACK_CAMERA()
	{
		NKCScenManager.GetScenManager().GetGameClient().UI_HUD_TRACK_CAMERA_TOGGLE();
	}

	public void UI_GAME_NO_HP_DMG_MODE_TEAM_A()
	{
	}

	public void UI_GAME_NO_HP_DMG_MODE_TEAM_B()
	{
	}

	public void UI_GAME_AI_DISABLE_TEAM_A()
	{
	}

	public void UI_GAME_AI_DISABLE_TEAM_B()
	{
	}

	public void UI_GAME_ALL_KILL()
	{
	}

	public void UI_GAME_ALL_KILL_ENEMY()
	{
	}

	public void UI_GAME_DEV_MENU()
	{
	}

	public void UI_GAME_DEV_MENU_CLOSE()
	{
	}

	public void UI_GAME_DEV_MENU_DUNGEON_LIST_CHANGED(int optionIndex)
	{
	}

	public void UI_GAME_DEV_MENU_DUNGEON_LIST_RELOAD()
	{
	}

	public void UI_GAME_DEV_MENU_SHIP_CHANGE()
	{
	}

	public void UI_GAME_PAUSE()
	{
		NKCScenManager.GetScenManager().GetGameClient().UI_GAME_PAUSE();
	}

	public void UI_GAME_DEV_FRAME_MOVE()
	{
	}

	public void UI_GAME_DEV_SKILL_NORMAL()
	{
	}

	public void UI_GAME_DEV_SKILL_NORMAL_ENEMY()
	{
	}

	public void UI_GAME_DEV_SKILL_SPECIAL()
	{
	}

	public void UI_GAME_DEV_SKILL_SPECIAL_ENEMY()
	{
	}

	public void UI_GAME_DEV_MONSTER_AUTO_RESAPWN_TOGGLE(bool bSet)
	{
	}

	public void UI_GAME_DEV_UNIT_REAL_TIME_SPAWN()
	{
	}

	public void UI_GAME_DEV_UNIT_REAL_TIME_SPAWN_ENEMY()
	{
	}

	public void UI_GAME_DEV_RESET()
	{
	}
}
