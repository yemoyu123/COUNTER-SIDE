using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public interface INKCUIStageViewer
{
	bool UseNormalizedPos();

	Vector2 SetData(bool bUseEpSlot, int EpisodeID, int ActID, EPISODE_DIFFICULTY Difficulty, IDungeonSlot.OnSelectedItemSlot onSelectedSlot, EPISODE_SCROLL_TYPE scrollType);

	void ResetPosition(Transform parent);

	int GetActCount(EPISODE_DIFFICULTY difficulty);

	void SetActive(bool bValue);

	void SetSelectNode(NKMStageTempletV2 stageTemplet);

	void RefreshData();

	int GetCurActID();

	Vector2 GetTargetPos(int slotIndex, EPISODE_SCROLL_TYPE scrollType, bool bUseNormalized);
}
