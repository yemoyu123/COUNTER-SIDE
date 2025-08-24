using ClientPacket.Office;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public interface IOfficeMinimap
{
	GameObject GetGameObject();

	ScrollRect GetScrollRect();

	Transform GetScrollTargetTileTransform(int sectionId);

	Transform GetRightEndTileTransform();

	RectTransform GetTileRectTransform(int roomId);

	float GetScrollRectContentOriginalWidth();

	void SetActive(bool value);

	void UpdateRoomStateAll();

	void UpdateRoomState(NKMOfficeRoomTemplet.RoomType roomType);

	void UpdateRoomStateInSection(int sectionId);

	void UpdateRoomInfo(NKMOfficeRoom officeRoom);

	void UpdatePurchasedRoom(NKMOfficeRoom officeRoom);

	void LockRoomsInSection(int sectionId);

	void ExpandScrollRectRange();

	void RevertScrollRectRange();

	void UpdateCameraPosition();

	bool IsRedDotOn();
}
