using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIFierceBattleNoticeSlot : MonoBehaviour
{
	public Image m_BossImage;

	public Text m_BossName;

	public void SetData(string bossFaceCardName, string bossName)
	{
		if (!string.IsNullOrEmpty(bossFaceCardName))
		{
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_unit_face_card", bossFaceCardName);
			NKCUtil.SetImageSprite(m_BossImage, orLoadAssetResource);
		}
		NKCUtil.SetLabelText(m_BossName, bossName);
	}
}
