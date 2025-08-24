using UnityEngine;

namespace NKC;

public interface INKCUICutScenTalkBoxMgr
{
	GameObject MyGameObject { get; }

	bool IsFinished { get; }

	NKCUICutScenTalkBoxMgr.TalkBoxType MyBoxType { get; }

	void SetPause(bool bPause);

	void ResetTalkBox();

	void Finish();

	void Open(string _TalkerName, string _Talk, float fCoolTime, bool bWaitClick, bool _bTalkAppend);

	void StartFadeIn(float fadeTime);

	void FadeOutBooking(float fadeTime);

	void ClearTalk();

	void Close();

	void OnChange();

	bool UsingTMPText();
}
