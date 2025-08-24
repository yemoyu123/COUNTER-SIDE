using System;
using DG.Tweening;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCCutTemplet
{
	public enum eCutsceneAction
	{
		NONE,
		MARK,
		JUMP,
		SELECT,
		PLAY_MUSIC
	}

	private const float UNIT_MOVE_DIST = 1400f;

	private const float UNIT_MOVE_DIST_FOR_CENTER = 1800f;

	private const float UNIT_MOVE_TIME_FOR_CENTER = 0.7f;

	private const float UNIT_MOVE_TIME_TO_DOWN = 0.9f;

	private const float UNIT_MOVE_TIME = 0.6f;

	public bool m_bWaitClick = true;

	public float m_fWaitTime;

	public bool m_bTitleClear;

	public bool m_bTitleFadeOut;

	public float m_fTitleFadeOutTime;

	public string m_Title = "";

	public float m_fTitleTalkTime = 0.15f;

	public string m_SubTitle = "";

	public float m_fSubTitleTalkTime = 0.15f;

	public bool m_bTalkCenterFadeIn;

	public bool m_bTalkCenterFadeOut;

	public float m_bTalkCenterFadeTime;

	public NKC_CUTSCEN_FILTER_TYPE m_FilterType;

	public float m_fBGFadeInTime;

	public Ease m_easeBGFadeIn = Ease.Linear;

	public Color m_colBGFadeInStart = new Color(1f, 1f, 1f, 1f);

	public Color m_colBGFadeIn = new Color(1f, 1f, 1f, 1f);

	public float m_fBGFadeOutTime;

	public Ease m_easeBGFadeOut = Ease.Linear;

	public Color m_colBGFadeOut = new Color(1f, 1f, 1f, 1f);

	public bool m_bGameObjectBGType;

	public string m_BGFileName = "";

	public string m_GameObjectBGAniName = "";

	public bool m_bGameObjectBGLoop = true;

	public bool m_bNoWaitBGAni;

	public int m_BGCrash;

	public float m_fBGCrashTime;

	public float m_fBGAnITime;

	public bool m_bBGAniPos;

	public Vector2 m_BGOffsetPose = new Vector2(0f, 0f);

	public TRACKING_DATA_TYPE m_tdtBGPos = TRACKING_DATA_TYPE.TDT_NORMAL;

	public bool m_bBGAniScale;

	public Vector3 m_BGOffsetScale = new Vector3(1f, 1f, 1f);

	public TRACKING_DATA_TYPE m_tdtBGScale = TRACKING_DATA_TYPE.TDT_NORMAL;

	public bool m_bLooseShake;

	public bool m_bFadeIn;

	public float m_fFadeTime;

	public bool m_bFadeWhite;

	public float m_fFlashBangTime;

	public string m_StartBGMFileName = "";

	public string m_EndBGMFileName = "";

	public NKC_CUTSCEN_SOUND_CONTROL m_StartFXSoundControl = NKC_CUTSCEN_SOUND_CONTROL.NCSC_ONE_TIME_PLAY;

	public string m_StartFXSoundName = "";

	public NKC_CUTSCEN_SOUND_CONTROL m_EndFXSoundControl = NKC_CUTSCEN_SOUND_CONTROL.NCSC_ONE_TIME_PLAY;

	public string m_EndFXSoundName = "";

	public string m_VoiceFileName = "";

	public string m_CharStrID = "";

	public bool m_bClear;

	public bool m_bFlip;

	public string m_Pos = "";

	public Vector2 m_CharOffSet = Vector2.zero;

	public Vector2 m_StartPos = Vector2.zero;

	public Vector2 m_TargetPos = Vector2.zero;

	public bool m_bMoveVertical;

	public float m_TrackingTime = 0.6f;

	public CutUnitPosType m_StartPosType = CutUnitPosType.NONE;

	public int m_BounceCount;

	public float m_BounceTime;

	public bool m_bCharHologramEffect;

	public bool m_bCharPinup;

	public float m_bCharPinupEasingTime;

	public bool m_bCharFadeOut;

	public bool m_bCharFadeIn;

	public Vector2 m_CharScale;

	public float m_CharScaleTime;

	public string m_Face = "";

	public bool m_bFaceLoop = true;

	public int m_Crash;

	public string m_Talk = "";

	public float m_fTalkTime;

	public bool m_bTalkAppend;

	public bool m_CloseTalkBox;

	public string m_ImageName = "";

	public float m_fImageScale = 1f;

	public Vector2 m_ImageOffsetPos = new Vector2(0f, 0f);

	public bool m_bMovieSkipEnable = true;

	public string m_MovieName = "";

	private CutUnitPosType m_cutUnitPosType = CutUnitPosType.NONE;

	private CutUnitPosAction m_cutUnitPosAction = CutUnitPosAction.NONE;

	public eCutsceneAction m_Action;

	public string m_ActionStrKey;

	public static readonly char[] Seperator = new char[4] { ',', ' ', '\t', '\n' };

	public CutUnitPosType CutUnitPos => m_cutUnitPosType;

	public CutUnitPosAction CutUnitAction => m_cutUnitPosAction;

	public string GetActionFirstToken()
	{
		string[] actionTokens = GetActionTokens();
		if (actionTokens.Length != 0)
		{
			return actionTokens[0];
		}
		return "";
	}

	public string[] GetActionTokens()
	{
		return m_ActionStrKey.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
	}

	public bool LoadFromLUA(NKMLua cNKMLua, string cutSceneStrID, int index)
	{
		string nationalPostfix = NKCStringTable.GetNationalPostfix(NKCStringTable.GetNationalCode());
		cNKMLua.GetData("m_bWaitClick", ref m_bWaitClick);
		cNKMLua.GetData("m_fWaitTime", ref m_fWaitTime);
		cNKMLua.GetData("m_bTitleClear", ref m_bTitleClear);
		cNKMLua.GetData("m_bTitleFadeOut", ref m_bTitleFadeOut);
		cNKMLua.GetData("m_fTitleFadeOutTime", ref m_fTitleFadeOutTime);
		cNKMLua.GetData("m_Title" + nationalPostfix, ref m_Title);
		cNKMLua.GetData("m_fTitleTalkTime", ref m_fTitleTalkTime);
		cNKMLua.GetData("m_SubTitle" + nationalPostfix, ref m_SubTitle);
		cNKMLua.GetData("m_fSubTitleTalkTime", ref m_fSubTitleTalkTime);
		cNKMLua.GetData("m_bTalkCenterFadeIn", ref m_bTalkCenterFadeIn);
		cNKMLua.GetData("m_bTalkCenterFadeOut", ref m_bTalkCenterFadeOut);
		cNKMLua.GetData("m_bTalkCenterFadeTime", ref m_bTalkCenterFadeTime);
		cNKMLua.GetData("m_FilterType", ref m_FilterType);
		cNKMLua.GetData("m_fBGFadeInTime", ref m_fBGFadeInTime);
		cNKMLua.GetData("m_easeBGFadeIn", ref m_easeBGFadeIn);
		if (cNKMLua.OpenTable("m_colBGFadeInStart"))
		{
			cNKMLua.GetData(1, ref m_colBGFadeInStart.r);
			cNKMLua.GetData(2, ref m_colBGFadeInStart.g);
			cNKMLua.GetData(3, ref m_colBGFadeInStart.b);
			cNKMLua.GetData(4, ref m_colBGFadeInStart.a);
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_colBGFadeIn"))
		{
			cNKMLua.GetData(1, ref m_colBGFadeIn.r);
			cNKMLua.GetData(2, ref m_colBGFadeIn.g);
			cNKMLua.GetData(3, ref m_colBGFadeIn.b);
			cNKMLua.GetData(4, ref m_colBGFadeIn.a);
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_fBGFadeOutTime", ref m_fBGFadeOutTime);
		cNKMLua.GetData("m_easeBGFadeOut", ref m_easeBGFadeOut);
		if (cNKMLua.OpenTable("m_colBGFadeOut"))
		{
			cNKMLua.GetData(1, ref m_colBGFadeOut.r);
			cNKMLua.GetData(2, ref m_colBGFadeOut.g);
			cNKMLua.GetData(3, ref m_colBGFadeOut.b);
			cNKMLua.GetData(4, ref m_colBGFadeOut.a);
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_bGameObjectBGType", ref m_bGameObjectBGType);
		cNKMLua.GetData("m_BGFileName", ref m_BGFileName);
		cNKMLua.GetData("m_GameObjectBGAniName", ref m_GameObjectBGAniName);
		cNKMLua.GetData("m_bGameObjectBGLoop", ref m_bGameObjectBGLoop);
		cNKMLua.GetData("m_bNoWaitBGAni", ref m_bNoWaitBGAni);
		cNKMLua.GetData("m_BGCrash", ref m_BGCrash);
		cNKMLua.GetData("m_fBGCrashTime", ref m_fBGCrashTime);
		cNKMLua.GetData("m_fBGAnITime", ref m_fBGAnITime);
		cNKMLua.GetData("m_bBGAniPos", ref m_bBGAniPos);
		if (cNKMLua.OpenTable("m_BGOffsetPose"))
		{
			cNKMLua.GetData(1, ref m_BGOffsetPose.x);
			cNKMLua.GetData(2, ref m_BGOffsetPose.y);
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_tdtBGPos", ref m_tdtBGPos);
		cNKMLua.GetData("m_bBGAniScale", ref m_bBGAniScale);
		if (cNKMLua.OpenTable("m_BGOffsetScale"))
		{
			cNKMLua.GetData(1, ref m_BGOffsetScale.x);
			cNKMLua.GetData(2, ref m_BGOffsetScale.y);
			cNKMLua.GetData(3, ref m_BGOffsetScale.z);
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_tdtBGScale", ref m_tdtBGScale);
		cNKMLua.GetData("m_bLooseShake", ref m_bLooseShake);
		cNKMLua.GetData("m_bFadeIn", ref m_bFadeIn);
		cNKMLua.GetData("m_fFadeTime", ref m_fFadeTime);
		cNKMLua.GetData("m_bFadeWhite", ref m_bFadeWhite);
		cNKMLua.GetData("m_fFlashBangTime", ref m_fFlashBangTime);
		cNKMLua.GetData("m_StartBGMFileName", ref m_StartBGMFileName);
		cNKMLua.GetData("m_EndBGMFileName", ref m_EndBGMFileName);
		if (!cNKMLua.GetDataEnum<NKC_CUTSCEN_SOUND_CONTROL>("m_StartFXSoundControl", out m_StartFXSoundControl))
		{
			m_StartFXSoundControl = NKC_CUTSCEN_SOUND_CONTROL.NCSC_ONE_TIME_PLAY;
		}
		cNKMLua.GetData("m_StartFXSoundName", ref m_StartFXSoundName);
		if (!cNKMLua.GetDataEnum<NKC_CUTSCEN_SOUND_CONTROL>("m_EndFXSoundControl", out m_EndFXSoundControl))
		{
			m_EndFXSoundControl = NKC_CUTSCEN_SOUND_CONTROL.NCSC_ONE_TIME_PLAY;
		}
		cNKMLua.GetData("m_EndFXSoundName", ref m_EndFXSoundName);
		cNKMLua.GetData("m_VoiceFileName", ref m_VoiceFileName);
		cNKMLua.GetData("m_CharStrID", ref m_CharStrID);
		cNKMLua.GetData("m_bClear", ref m_bClear);
		cNKMLua.GetData("m_bFlip", ref m_bFlip);
		cNKMLua.GetData("m_Pos", ref m_Pos);
		ParsePosCommand(cutSceneStrID, index);
		if (cNKMLua.OpenTable("m_CharOffSet"))
		{
			cNKMLua.GetData(1, ref m_CharOffSet.x);
			cNKMLua.GetData(2, ref m_CharOffSet.y);
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_Bounce"))
		{
			cNKMLua.GetData(1, ref m_BounceCount);
			cNKMLua.GetData(2, ref m_BounceTime);
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_TrackingTime", ref m_TrackingTime);
		cNKMLua.GetData("m_bCharHologramEffect", ref m_bCharHologramEffect);
		cNKMLua.GetData("m_bCharPinup", ref m_bCharPinup);
		cNKMLua.GetData("m_bCharPinupEasingTime", ref m_bCharPinupEasingTime);
		cNKMLua.GetData("m_bCharFadeIn", ref m_bCharFadeIn);
		cNKMLua.GetData("m_bCharFadeOut", ref m_bCharFadeOut);
		if (cNKMLua.OpenTable("m_CharScale"))
		{
			cNKMLua.GetData(1, ref m_CharScale.x);
			cNKMLua.GetData(2, ref m_CharScale.y);
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_CharScaleTime", ref m_CharScaleTime);
		cNKMLua.GetData("m_Face", ref m_Face);
		if (Enum.TryParse<NKCASUIUnitIllust.eAnimation>(m_Face, out var result))
		{
			m_Face = NKCASUIUnitIllust.GetAnimationName(result);
		}
		cNKMLua.GetData("m_bFaceLoop", ref m_bFaceLoop);
		cNKMLua.GetData("m_Crash", ref m_Crash);
		cNKMLua.GetData("m_Talk" + nationalPostfix, ref m_Talk);
		cNKMLua.GetData("m_fTalkTime", ref m_fTalkTime);
		cNKMLua.GetData("m_bTalkAppend", ref m_bTalkAppend);
		cNKMLua.GetData("m_ImageName", ref m_ImageName);
		cNKMLua.GetData("m_fImageScale", ref m_fImageScale);
		cNKMLua.GetData("m_CloseTalkBox", ref m_CloseTalkBox);
		if (cNKMLua.OpenTable("m_ImageOffsetPos"))
		{
			cNKMLua.GetData(1, ref m_ImageOffsetPos.x);
			cNKMLua.GetData(2, ref m_ImageOffsetPos.y);
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_bMovieSkipEnable", ref m_bMovieSkipEnable);
		cNKMLua.GetData("m_MovieName", ref m_MovieName);
		if (cNKMLua.GetDataEnum<eCutsceneAction>("m_Action", out m_Action))
		{
			cNKMLua.GetData("m_ActionStrKey", ref m_ActionStrKey);
		}
		return true;
	}

	public void ParsePosCommand(string cutSceneStrID, int index)
	{
		if (string.IsNullOrEmpty(m_Pos))
		{
			return;
		}
		string[] array = m_Pos.Split('_');
		float num = 0f;
		int num2 = 0;
		switch (array[0])
		{
		case "L":
			m_cutUnitPosType = CutUnitPosType.LEFT;
			num = 1400f;
			break;
		case "R":
			m_cutUnitPosType = CutUnitPosType.RIGHT;
			num = 1400f;
			break;
		case "C":
			m_cutUnitPosType = CutUnitPosType.CENTER;
			num = 1800f;
			break;
		default:
			m_cutUnitPosType = CutUnitPosType.NONE;
			Debug.LogError($"Invalid m_Pos Command. First token must be 'L', 'R', 'C'. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
			return;
		}
		if (array.Length == 1)
		{
			m_cutUnitPosAction = CutUnitPosAction.PLACE;
			return;
		}
		switch (array[1])
		{
		case "D":
			if (array.Length != 2)
			{
				Debug.LogError($"Invalid m_Pos Command. 'D' Command must have only 2 token. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
				return;
			}
			m_cutUnitPosAction = CutUnitPosAction.DARK;
			break;
		case "L":
		case "R":
			if (array.Length != 4)
			{
				Debug.LogError($"Invalid m_Pos Command. 'C_L_M', 'C_R_M' Command must have only 4 token. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
				return;
			}
			if (array[2] != "M")
			{
				Debug.LogError($"Invalid m_Pos Command. Token next to 'C_L', 'C_R' must be 'M'. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
				return;
			}
			num2 = 2;
			switch (array[3])
			{
			case "I":
				m_cutUnitPosAction = CutUnitPosAction.IN;
				break;
			case "O":
				m_cutUnitPosAction = CutUnitPosAction.OUT;
				break;
			default:
				Debug.LogError($"Invalid m_Pos Command. Token next to 'M' must be 'I' or 'O'. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
				return;
			}
			break;
		case "M":
			if (array.Length < 3)
			{
				Debug.LogError($"Invalid m_Pos Command. 'M' Command must greater than 3. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
				return;
			}
			num2 = 1;
			if (array[2] == "I")
			{
				if (array.Length > 3)
				{
					Debug.LogError($"Invalid m_Pos Command. 'I' Command must less than 3. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
					return;
				}
				m_cutUnitPosAction = CutUnitPosAction.IN;
				break;
			}
			if (array[2] == "O")
			{
				if (array.Length > 4)
				{
					Debug.LogError($"Invalid m_Pos Command. 'O' Command must less than 4. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
					return;
				}
				m_cutUnitPosAction = CutUnitPosAction.OUT;
				if (array.Length == 4)
				{
					if (array[3] != "DOWN")
					{
						Debug.LogError($"Invalid m_Pos Command. Token next to 'O' must be 'DOWN'. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
						return;
					}
					m_bMoveVertical = true;
				}
				break;
			}
			Debug.LogError($"Invalid m_Pos Command. Token next to 'M' must be 'I' or 'O'. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
			return;
		case "F":
			if (array.Length != 3)
			{
				Debug.LogError($"Invalid m_Pos Command. 'F' Command must have only 3 token. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
				return;
			}
			m_cutUnitPosAction = CutUnitPosAction.MOVE;
			switch (array[2])
			{
			case "L":
				m_StartPosType = CutUnitPosType.LEFT;
				break;
			case "R":
				m_StartPosType = CutUnitPosType.RIGHT;
				break;
			case "C":
				m_StartPosType = CutUnitPosType.CENTER;
				break;
			default:
				Debug.LogError($"Invalid m_Pos Command. Token next to 'F' must be 'L' or 'R' or 'C'. m_Pos [{m_Pos}], CutSceneStrID [{cutSceneStrID}], CutIndex [{index}]");
				return;
			}
			break;
		}
		if (m_cutUnitPosAction == CutUnitPosAction.IN)
		{
			if (array[num2 - 1] == "L")
			{
				m_StartPos.x = 0f - num;
			}
			else
			{
				m_StartPos.x = num;
			}
		}
		else if (m_cutUnitPosAction == CutUnitPosAction.OUT)
		{
			if (m_bMoveVertical)
			{
				m_TargetPos.y = 0f - num;
			}
			else if (array[num2 - 1] == "L")
			{
				m_TargetPos.x = 0f - num;
			}
			else
			{
				m_TargetPos.x = num;
			}
		}
	}
}
