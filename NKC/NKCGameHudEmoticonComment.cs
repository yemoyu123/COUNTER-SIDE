using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCGameHudEmoticonComment : MonoBehaviour
{
	public Text m_lbComment;

	public Animator m_amtorComment;

	public NKCUIComStateButton m_csbtnComment;

	public NKCUIComRaycastTarget m_crtComment;

	private int m_LastEmoticonID = -1;

	public void Start()
	{
		NKCUtil.SetBindFunction(m_csbtnComment, delegate
		{
			PlayPreview(m_LastEmoticonID);
		});
	}

	public void SetEnableBtn(bool bSet)
	{
		if (m_csbtnComment != null)
		{
			m_csbtnComment.enabled = bSet;
		}
		if (m_crtComment != null)
		{
			m_crtComment.enabled = bSet;
		}
	}

	public void PlayPreview(int emoticonID)
	{
		NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(emoticonID);
		if (nKMEmoticonTemplet != null)
		{
			m_LastEmoticonID = nKMEmoticonTemplet.m_EmoticonID;
			NKCUtil.SetLabelText(m_lbComment, nKMEmoticonTemplet.GetEmoticonName());
			m_amtorComment.Play("FX_UI_HUD_EMOTICON_COMMENT_STOP_" + nKMEmoticonTemplet.m_EmoticonaAnimationName, -1, 0f);
		}
	}

	public void Play(int emoticonID)
	{
		NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(emoticonID);
		if (nKMEmoticonTemplet != null)
		{
			m_LastEmoticonID = nKMEmoticonTemplet.m_EmoticonID;
			NKCUtil.SetLabelText(m_lbComment, nKMEmoticonTemplet.GetEmoticonName());
			m_amtorComment.Play("FX_UI_HUD_EMOTICON_COMMENT_BASE_" + nKMEmoticonTemplet.m_EmoticonaAnimationName, -1, 0f);
		}
	}

	public void Stop()
	{
		m_amtorComment.Play("FX_UI_HUD_EMOTICON_COMMENT_IDLE", -1, 0f);
	}
}
