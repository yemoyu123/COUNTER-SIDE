using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using ClientPacket.Common;
using ClientPacket.Raid;
using ClientPacket.Warfare;
using ClientPacket.WorldMap;
using Cs.Logging;
using Cs.Math;
using NKC.PacketHandler;
using NKC.Publisher;
using NKC.Trim;
using NKC.UI;
using NKC.UI.Component;
using NKC.UI.Result;
using NKM;
using NKM.Guild;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC;

public class NKCUtil
{
	public enum ButtonColor
	{
		BC_NONE = -1,
		BC_GRAY,
		BC_YELLOW,
		BC_BLUE,
		BC_RED,
		BC_LOGIN_BLUE,
		BC_LOGIN_YELLOW,
		BC_COMMON_ENABLE,
		BC_COMMON_DISABLE
	}

	public delegate bool Factory<T>(string source, out T value);

	private static string m_PatchVersion = "";

	private static string m_PatchVersionEA = "";

	private static StringBuilder newFullTextBuilder = new StringBuilder();

	private static StringBuilder subTextBuilder = new StringBuilder();

	private static StringBuilder tempTextBuilder = new StringBuilder();

	public static Color EP_ACHIEVE_COLOR = new Color(1f, 74f / 85f, 0f);

	public static Color EP_NO_ACHIEVE_COLOR = new Color(37f / 85f, 37f / 85f, 37f / 85f);

	public static HashSet<int> m_sHsFirstClearDungeon = new HashSet<int>();

	public static HashSet<int> m_sHsFirstClearWarfare = new HashSet<int>();

	public static string PatchVersion
	{
		get
		{
			return m_PatchVersion;
		}
		set
		{
			m_PatchVersion = value;
		}
	}

	public static string PatchVersionEA
	{
		get
		{
			return m_PatchVersionEA;
		}
		set
		{
			m_PatchVersionEA = value;
		}
	}

	public static string GetExtraDownloadPath()
	{
		if (Application.isEditor)
		{
			return Environment.CurrentDirectory.Replace("\\", "/") + "/ExtraAsset/";
		}
		if (NKCDefineManager.DEFINE_PC_EXTRA_DOWNLOAD_IN_EXE_FOLDER())
		{
			return Application.dataPath + "/../ExtraAsset/";
		}
		return Application.persistentDataPath + "/ExtraAsset/";
	}

	public static bool CheckFinalCaptionEnabled()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.NPC_SUBTITLE))
		{
			return false;
		}
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null || !gameOptionData.UseNpcSubtitle)
		{
			return false;
		}
		return true;
	}

	public static void WindowMsgBox(string msg)
	{
		NativeWinAlert.MessageBox(NativeWinAlert.GetWindowHandle(), msg, "Log", 16u);
	}

	public static string LoadFileString(string fileName, bool bUsePersistentDataPath = false)
	{
		string path = ((!bUsePersistentDataPath) ? (Application.dataPath + "/" + fileName) : (Application.persistentDataPath + "/" + fileName));
		if (File.Exists(path))
		{
			FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
			StreamReader streamReader = new StreamReader(fileStream);
			string result = streamReader.ReadLine();
			streamReader.Close();
			fileStream.Close();
			return result;
		}
		return "";
	}

	public static string LoadFileFullString(string fileName, bool bUsePersistentDataPath = false)
	{
		string path = ((!bUsePersistentDataPath) ? (Application.dataPath + "/" + fileName) : (Application.persistentDataPath + "/" + fileName));
		if (File.Exists(path))
		{
			FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
			TextReader textReader = new StreamReader(fileStream);
			StringBuilder builder = NKMString.GetBuilder();
			string text = textReader.ReadLine();
			while (text != null)
			{
				builder.Append(text);
				text = textReader.ReadLine();
				if (text != null)
				{
					builder.Append(Environment.NewLine);
				}
			}
			textReader.Close();
			fileStream.Close();
			return builder.ToString();
		}
		return "";
	}

	public static void SaveFileString(string fileName, string text, bool bUsePersistentDataPath = false, bool bForceWriteReadOnlyFile = false)
	{
		string text2 = ((!bUsePersistentDataPath) ? (Application.dataPath + "/" + fileName) : (Application.persistentDataPath + "/" + fileName));
		FileStream fileStream;
		if (!File.Exists(text2))
		{
			fileStream = new FileStream(text2, FileMode.Create, FileAccess.Write, FileShare.Read);
		}
		else
		{
			if (bForceWriteReadOnlyFile)
			{
				FileInfo fileInfo = new FileInfo(text2);
				if (fileInfo.IsReadOnly)
				{
					fileInfo.Attributes = FileAttributes.Normal;
				}
			}
			fileStream = File.Open(text2, FileMode.Truncate, FileAccess.Write, FileShare.Read);
		}
		StreamWriter streamWriter = new StreamWriter(fileStream);
		streamWriter.WriteLine(text);
		streamWriter.Close();
		fileStream.Close();
	}

	public static void SaveFileBinary(string fileName, byte[] bytes)
	{
		string path = Application.persistentDataPath + "/" + fileName;
		FileStream fileStream = (File.Exists(path) ? File.Open(path, FileMode.Truncate, FileAccess.Write, FileShare.Read) : new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read));
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(bytes, 0, bytes.Length);
		binaryWriter.Close();
		fileStream.Close();
	}

	public static void LoadFileBinary(string fileName, out byte[] bytes)
	{
		string path = Application.persistentDataPath + "/" + fileName;
		if (File.Exists(path))
		{
			FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			bytes = binaryReader.ReadBytes((int)fileStream.Length);
			binaryReader.Close();
			fileStream.Close();
		}
		else
		{
			bytes = null;
		}
	}

	public static void SetGameObjectPos(GameObject go, float x, float y, float z)
	{
		Vector3 position = new Vector3(x, y, z);
		go.transform.position = position;
	}

	public static void SetGameObjectLocalPos(GameObject go, float x, float y, float z)
	{
		Vector3 localPosition = new Vector3(x, y, z);
		go.transform.localPosition = localPosition;
	}

	public static void SetRectTransformLocalRotate(RectTransform rt, float x, float y, float z)
	{
		Quaternion localRotation = rt.localRotation;
		Vector3 eulerAngles = localRotation.eulerAngles;
		eulerAngles.Set(x, y, z);
		localRotation.eulerAngles = eulerAngles;
		rt.localRotation = localRotation;
	}

	public static void SetGameObjectLocalScale(GameObject go, float x = -1f, float y = -1f, float z = -1f)
	{
		Vector3 localScale = go.transform.localScale;
		if (x != -1f)
		{
			localScale.x = x;
		}
		if (y != -1f)
		{
			localScale.y = y;
		}
		if (z != -1f)
		{
			localScale.z = z;
		}
		go.transform.localScale = localScale;
	}

	public static void SetGameObjectLocalScale(RectTransform rt, float x, float y, float z)
	{
		Vector3 localScale = new Vector3(x, y, z);
		rt.localScale = localScale;
	}

	public static void SetGameObjectLocalScaleRel(GameObject go, float x, float y, float z)
	{
		Vector3 localScale = go.transform.localScale;
		localScale.Set(localScale.x + x, localScale.y + y, localScale.z + z);
		go.transform.localScale = localScale;
	}

	public static void SetGameObjectLocalScaleRel(GameObject go, float fFactor)
	{
		Vector3 localScale = go.transform.localScale;
		localScale.Set(localScale.x * fFactor, localScale.y * fFactor, localScale.z);
		go.transform.localScale = localScale;
	}

	public static void SetRectTranformSizeDelta(RectTransform rt, float x, float y)
	{
		Vector2 sizeDelta = new Vector2(x, y);
		rt.sizeDelta = sizeDelta;
	}

	public static void SetSpriteRendererSortOrder(SpriteRenderer[] aSpriteRenderer, Material cMaterial = null, float fOffsetZ = 0f)
	{
		for (int i = 0; i < aSpriteRenderer.Length; i++)
		{
			_ = aSpriteRenderer[i].gameObject;
			if (cMaterial != null)
			{
				aSpriteRenderer[i].material = cMaterial;
			}
		}
	}

	public static void SetParticleSystemRendererSortOrder(ParticleSystemRenderer[] aParticleSystemRenderer, bool bEnable = true)
	{
		for (int i = 0; i < aParticleSystemRenderer.Length; i++)
		{
			_ = aParticleSystemRenderer[i].gameObject.transform.position;
			if (!bEnable)
			{
				aParticleSystemRenderer[i].enabled = false;
			}
			else
			{
				aParticleSystemRenderer[i].enabled = true;
			}
		}
	}

	public static void SetScrollSize(Text scrollText, RectTransform scrollTextRect, float scrollYOrg)
	{
		if (scrollYOrg < scrollText.preferredHeight)
		{
			Vector2 sizeDelta = scrollTextRect.sizeDelta;
			sizeDelta.y = scrollText.preferredHeight;
			scrollTextRect.sizeDelta = sizeDelta;
		}
		else
		{
			Vector2 sizeDelta2 = scrollTextRect.sizeDelta;
			sizeDelta2.y = scrollYOrg;
			scrollTextRect.sizeDelta = sizeDelta2;
		}
		Vector2 anchoredPosition = scrollTextRect.anchoredPosition;
		anchoredPosition.y = 0f;
		scrollTextRect.anchoredPosition = anchoredPosition;
	}

	public static int GetStarCntByUnitGrade(NKMUnitTempletBase templetBase)
	{
		int result = 1;
		if (templetBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_N)
		{
			result = 1;
		}
		else if (templetBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_R)
		{
			result = 1;
		}
		else if (templetBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SR)
		{
			result = 2;
		}
		else if (templetBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR)
		{
			result = 3;
		}
		return result;
	}

	public static void SetStarRank(List<GameObject> lstStar, NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		switch (unitTempletBase.m_NKM_UNIT_TYPE)
		{
		case NKM_UNIT_TYPE.NUT_SHIP:
			SetStarRank(lstStar, unitData.GetStarGrade(unitTempletBase), 6);
			break;
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			SetStarRank(lstStar, -1, -1);
			break;
		default:
			SetStarRank(lstStar, unitData.GetStarGrade(unitTempletBase), unitTempletBase.m_StarGradeMax);
			break;
		}
	}

	public static void SetStarRank(List<GameObject> lstStar, int starRank, int maxStarRank)
	{
		for (int i = 0; i < lstStar.Count; i++)
		{
			if (!(lstStar[i] == null))
			{
				Transform transform = lstStar[i].transform.Find("ON");
				Transform transform2 = lstStar[i].transform.Find("OFF");
				if (transform != null && transform2 != null)
				{
					transform.gameObject.SetActive(i < starRank);
					transform2.gameObject.SetActive(i >= starRank && i < maxStarRank);
					lstStar[i].gameObject.SetActive(i < maxStarRank);
				}
				else
				{
					lstStar[i].SetActive(i < starRank);
				}
			}
		}
	}

	public static void SetSkillUnlockStarRank(List<GameObject> lstStar, NKMUnitSkillTemplet skillTemplet, int unitStarGradeMax)
	{
		int num = unitStarGradeMax - 3;
		SetStarRank(lstStar, NKMUnitSkillManager.GetUnlockReqUpgradeFromSkillId(skillTemplet.m_ID) + num, unitStarGradeMax);
	}

	public static Color GetUITextColor(bool bActive = true)
	{
		if (bActive)
		{
			return new Color(0.345f, 0.141f, 0.09f);
		}
		return new Color(0.133f, 0.133f, 0.133f);
	}

	public static Color GetButtonUIColor(bool Active = true)
	{
		if (Active)
		{
			return new Color(0.345098f, 0.1568628f, 0.09019608f);
		}
		return new Color(0.13f, 0.13f, 0.1333333f);
	}

	public static Color GetBonusColor(float bonus)
	{
		if (bonus > 0f)
		{
			return new Color(26f / 85f, 0.7607843f, 81f / 85f);
		}
		if (bonus < 0f)
		{
			return new Color(1f, 0f, 0f);
		}
		return Color.white;
	}

	public static void SetGameobjectActive(GameObject targetObj, bool bValue)
	{
		if (targetObj != null && targetObj.activeSelf != bValue)
		{
			targetObj.SetActive(bValue);
		}
	}

	public static void SetGameobjectActive(Transform targetTransform, bool bValue)
	{
		if (targetTransform != null && targetTransform.gameObject.activeSelf != bValue)
		{
			targetTransform.gameObject.SetActive(bValue);
		}
	}

	public static void SetGameobjectActive(MonoBehaviour targetMono, bool bValue)
	{
		if (targetMono != null && targetMono.gameObject.activeSelf != bValue)
		{
			targetMono.gameObject.SetActive(bValue);
		}
	}

	public static void SetGameobjectActive(NKCUISlot slot, bool bValue)
	{
		if (slot != null)
		{
			slot.SetActive(bValue);
		}
	}

	public static void SetCanvasGroupAlpha(CanvasGroup canvasGroup, float fValue)
	{
		if (canvasGroup != null)
		{
			canvasGroup.alpha = fValue;
		}
	}

	public static void SetLabelText(TextMeshProUGUI label, string msg)
	{
		if (!(label != null))
		{
			return;
		}
		if (label.fontSharedMaterial == null)
		{
			UnityEngine.Debug.LogWarning("TMP ShardMatrial Null from object " + label.gameObject.name + ". try default");
			label.fontSharedMaterial = TMP_Settings.defaultFontAsset.material;
		}
		if (!string.IsNullOrEmpty(msg))
		{
			label.SetText(msg);
			if (msg.Contains("<link="))
			{
				label.raycastTarget = true;
				if (label.spriteAsset == null)
				{
					UnityEngine.Debug.LogError("TMP using Link, but SpriteAsset Null. objectname : " + label.gameObject.name);
				}
			}
		}
		else
		{
			label.text = "";
		}
		label.SetLayoutDirty();
	}

	public static void SetLabelText(Text label, string msg)
	{
		if (label != null)
		{
			label.text = msg;
			label.SetLayoutDirty();
		}
	}

	public static void SetLabelText(TMP_Text label, string msg)
	{
		if (!(label != null))
		{
			return;
		}
		if (label.fontSharedMaterial == null)
		{
			UnityEngine.Debug.LogWarning("TMP ShardMatrial Null from object " + label.gameObject.name + ". try default");
			label.fontSharedMaterial = TMP_Settings.defaultFontAsset.material;
		}
		if (!string.IsNullOrEmpty(msg))
		{
			label.SetText(msg);
			if (msg.Contains("<link="))
			{
				label.raycastTarget = true;
				if (label.spriteAsset == null)
				{
					UnityEngine.Debug.LogError("TMP using Link, but SpriteAsset Null. objectname : " + label.gameObject.name);
				}
			}
		}
		else
		{
			label.text = "";
		}
		label.SetLayoutDirty();
	}

	public static void SetLabelText(TextMeshProUGUI label, string msg, params object[] args)
	{
		if (!(label != null))
		{
			return;
		}
		if (label.fontSharedMaterial == null)
		{
			UnityEngine.Debug.LogWarning("TMP ShardMatrial Null from object " + label.gameObject.name + ". try default");
			label.fontSharedMaterial = TMP_Settings.defaultFontAsset.material;
		}
		if (!string.IsNullOrEmpty(msg))
		{
			label.SetText(string.Format(msg, args));
			if (msg.Contains("<link="))
			{
				label.raycastTarget = true;
				if (label.spriteAsset == null)
				{
					UnityEngine.Debug.LogError("TMP using Link, but SpriteAsset Null. objectname : " + label.gameObject.name);
				}
			}
		}
		else
		{
			label.text = "";
		}
		label.SetLayoutDirty();
	}

	public static void SetLabelText(Text label, string msg, params object[] args)
	{
		if (label != null)
		{
			label.text = string.Format(msg, args);
		}
	}

	public static void SetLabelKey(TextMeshProUGUI label, string key, params object[] param)
	{
		if (label != null)
		{
			string msg = NKCStringTable.GetString(key, param);
			SetLabelText(label, msg);
		}
	}

	public static void SetLabelKey(Text label, string key, params object[] param)
	{
		if (label != null)
		{
			string text = NKCStringTable.GetString(key, param);
			label.text = text;
			label.SetLayoutDirty();
		}
	}

	public static void SetLabelKey(TMP_Text label, string key, params object[] param)
	{
		if (label != null)
		{
			string msg = NKCStringTable.GetString(key, param);
			SetLabelText(label, msg);
		}
	}

	public static void SetLabelTextColor(Text label, Color col)
	{
		if (label != null)
		{
			label.color = col;
		}
	}

	public static void SetLabelTextColor(TMP_Text label, Color col)
	{
		if (label != null)
		{
			label.color = col;
		}
	}

	public static void SetLabelWidthScale(ref Text label)
	{
		if (!(label == null))
		{
			RectTransform component = label.GetComponent<RectTransform>();
			float width = component.GetWidth();
			Font font = label.font;
			float num = 0f;
			string text = label.text;
			foreach (char ch in text)
			{
				font.GetCharacterInfo(ch, out var info, label.fontSize, label.fontStyle);
				num += (float)info.advance;
			}
			if (width >= num)
			{
				component.localScale = new Vector3(component.localScale.y, component.localScale.y, component.localScale.y);
				return;
			}
			float num2 = width / num;
			component.localScale = new Vector3(component.localScale.y * num2, component.localScale.y, component.localScale.y);
		}
	}

	public static string RemoveLabelCharText(string str, string removeChar)
	{
		if (str == null)
		{
			return "";
		}
		return str.Replace(removeChar, "");
	}

	public static string LabelLongTextCut(Text label)
	{
		if (label == null)
		{
			return "";
		}
		string text = label.text.Replace("\n", " ");
		float num = 0f;
		float width = label.GetComponent<RectTransform>().GetWidth();
		int fontSize = label.fontSize;
		Font font = label.font;
		FontStyle fontStyle = label.fontStyle;
		int num2 = 0;
		string text2 = text;
		for (int i = 0; i < text2.Length; i++)
		{
			char ch = text2[i];
			if (!font.GetCharacterInfo(ch, out var info, fontSize, fontStyle))
			{
				font.RequestCharactersInTexture(ch.ToString(), fontSize, fontStyle);
				if (!font.GetCharacterInfo(ch, out info, fontSize, fontStyle))
				{
					UnityEngine.Debug.LogWarning("Text CharInfo is null: " + ch);
				}
			}
			num += (float)info.advance;
			if (num >= width)
			{
				int num3 = 3;
				int num4 = num2 - num3;
				if (num4 > 0)
				{
					return text.Remove(num4) + "...";
				}
			}
			num2++;
		}
		return text;
	}

	public static void SetImageSprite(Image image, Sprite sp, bool bDisableIfSpriteNull = false)
	{
		if (image != null)
		{
			image.sprite = sp;
		}
		if (bDisableIfSpriteNull)
		{
			SetGameobjectActive(image, sp != null);
		}
	}

	public static void SetImageColor(Image image, Color color)
	{
		if (image != null)
		{
			image.color = color;
		}
	}

	public static void SetImageFillAmount(Image image, float value)
	{
		if (image != null)
		{
			image.fillAmount = value;
		}
	}

	public static void SetImageMaterial(Image image, Material mat)
	{
		if (image != null)
		{
			image.material = mat;
		}
	}

	public static void SetUnityEvent<T>(UnityEvent<T> unityEvent, UnityAction<T> call)
	{
		if (unityEvent != null)
		{
			unityEvent.RemoveAllListeners();
			unityEvent.AddListener(call);
		}
	}

	public static void SetButtonClickDelegate(NKCUIComStateButton button, UnityAction call)
	{
		if (!(button == null))
		{
			button.PointerClick.RemoveAllListeners();
			button.PointerClick.AddListener(call);
		}
	}

	public static void SetButtonPointerDownDelegate(NKCUIComButton button, UnityAction<PointerEventData> call)
	{
		if (!(button == null))
		{
			button.PointerDown.RemoveAllListeners();
			button.PointerDown.AddListener(call);
		}
	}

	public static void SetButtonPointerDownDelegate(NKCUIComStateButton button, UnityAction<PointerEventData> call)
	{
		if (!(button == null))
		{
			button.PointerDown.RemoveAllListeners();
			button.PointerDown.AddListener(call);
		}
	}

	public static void SetButtonClickDelegate(NKCUIComStateButton button, UnityAction<int> call)
	{
		if (!(button == null))
		{
			if (button.PointerClickWithData == null)
			{
				button.PointerClickWithData = new NKCUnityEventInt();
			}
			button.PointerClickWithData.RemoveAllListeners();
			button.PointerClickWithData.AddListener(call);
		}
	}

	public static void SetButtonClickDelegate(NKCUIComButton button, UnityAction call)
	{
		if (!(button == null))
		{
			button.PointerClick.RemoveAllListeners();
			button.PointerClick.AddListener(call);
		}
	}

	public static void SetButtonClickDelegate(NKCUIComToggle toggle, UnityAction<bool> call)
	{
		SetToggleValueChangedDelegate(toggle, call);
	}

	public static void SetToggleValueChangedDelegate(NKCUIComToggle toggle, UnityAction<bool> call)
	{
		if (!(toggle == null))
		{
			toggle.OnValueChanged.RemoveAllListeners();
			toggle.OnValueChanged.AddListener(call);
		}
	}

	public static void SetButtonLock(NKCUIComStateButton button, bool value, bool bForce = false)
	{
		if (button != null)
		{
			button.SetLock(value, bForce);
		}
	}

	public static void SetButtonLock(NKCUIComButton button, bool value)
	{
		if (button != null)
		{
			if (value)
			{
				button.Lock();
			}
			else
			{
				button.UnLock();
			}
		}
	}

	public static void SetButtonLock(NKCUIComToggle toggle, bool value, bool bForce = false)
	{
		if (toggle != null)
		{
			toggle.SetLock(value, bForce);
		}
	}

	public static void SetSliderValueChangedDelegate(Slider slider, UnityAction<float> call)
	{
		if (!(slider == null))
		{
			slider.onValueChanged.RemoveAllListeners();
			slider.onValueChanged.AddListener(call);
		}
	}

	public static void SetDropdownValueChanged(Dropdown dropdown, UnityAction<int> call)
	{
		if (!(dropdown == null))
		{
			dropdown.onValueChanged.RemoveAllListeners();
			dropdown.onValueChanged.AddListener(call);
		}
	}

	public static void SetEventTriggerDelegate(EventTrigger evtTrigger, UnityAction call)
	{
		SetEventTriggerDelegate(evtTrigger, delegate
		{
			call();
		}, EventTriggerType.PointerClick, bInit: true);
	}

	public static void SetEventTriggerDelegate(EventTrigger evtTrigger, UnityAction<BaseEventData> call, EventTriggerType type = EventTriggerType.PointerClick, bool bInit = true)
	{
		if (!(evtTrigger == null) && call != null)
		{
			if (bInit)
			{
				evtTrigger.triggers.Clear();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = type;
			entry.callback.AddListener(call);
			evtTrigger.triggers.Add(entry);
		}
	}

	public static void SetSliderValue(Slider slider, float value)
	{
		if (slider != null)
		{
			slider.value = value;
		}
	}

	public static void SetSliderMinMax(Slider slider, float min, float max)
	{
		if (slider != null)
		{
			slider.minValue = min;
			slider.maxValue = max;
		}
	}

	public static void SetScrollHotKey(LoopScrollRect loopScroll, NKCUIBase uiRoot = null)
	{
		NKCUIComLoopScrollHotkey.AddHotkey(loopScroll, uiRoot);
	}

	public static void SetScrollHotKey(ScrollRect sr, NKCUIBase uiRoot = null)
	{
		NKCUIComScrollRectHotkey.AddHotkey(sr, uiRoot);
	}

	public static Color GetColorForGrade(NKMSkinTemplet.SKIN_GRADE skinGrade)
	{
		switch (skinGrade)
		{
		case NKMSkinTemplet.SKIN_GRADE.SG_VARIATION:
			return new Color(0.6705883f, 0.6705883f, 0.6705883f, 1f);
		case NKMSkinTemplet.SKIN_GRADE.SG_NORMAL:
			return new Color(0f, 48f / 85f, 1f, 1f);
		case NKMSkinTemplet.SKIN_GRADE.SG_RARE:
			return new Color(64f / 85f, 0f, 1f, 1f);
		case NKMSkinTemplet.SKIN_GRADE.SG_PREMIUM:
			return new Color(1f, 64f / 85f, 0.003921569f, 1f);
		case NKMSkinTemplet.SKIN_GRADE.SG_SPECIAL:
		{
			ColorUtility.TryParseHtmlString("#B0C9FC", out var color);
			return color;
		}
		default:
			return new Color(1f, 1f, 1f, 1f);
		}
	}

	public static string GetStringForGrade(NKMSkinTemplet.SKIN_GRADE skinGrade)
	{
		return skinGrade switch
		{
			NKMSkinTemplet.SKIN_GRADE.SG_VARIATION => NKCUtilString.GET_STRING_SKIN_GRADE_VARIATION, 
			NKMSkinTemplet.SKIN_GRADE.SG_RARE => NKCUtilString.GET_STRING_SKIN_GRADE_RARE, 
			NKMSkinTemplet.SKIN_GRADE.SG_PREMIUM => NKCUtilString.GET_STRING_SKIN_GRADE_PREMIUM, 
			NKMSkinTemplet.SKIN_GRADE.SG_SPECIAL => NKCUtilString.GET_STRING_SKIN_GRADE_SPECIAL, 
			_ => NKCUtilString.GET_STRING_SKIN_GRADE_NORMAL, 
		};
	}

	public static NKM_ITEM_GRADE ConvertUnitGradeToItemGrade(NKM_UNIT_GRADE unitGrade)
	{
		return unitGrade switch
		{
			NKM_UNIT_GRADE.NUG_SSR => NKM_ITEM_GRADE.NIG_SSR, 
			NKM_UNIT_GRADE.NUG_SR => NKM_ITEM_GRADE.NIG_SR, 
			NKM_UNIT_GRADE.NUG_R => NKM_ITEM_GRADE.NIG_R, 
			NKM_UNIT_GRADE.NUG_N => NKM_ITEM_GRADE.NIG_N, 
			NKM_UNIT_GRADE.NUG_COUNT => NKM_ITEM_GRADE.NIG_COUNT, 
			_ => NKM_ITEM_GRADE.NIG_N, 
		};
	}

	public static Color GetColorForUnitGrade(NKM_UNIT_GRADE eNKM_UNIT_GRADE)
	{
		return eNKM_UNIT_GRADE switch
		{
			NKM_UNIT_GRADE.NUG_N => new Color(0.6705883f, 0.6705883f, 0.6705883f, 1f), 
			NKM_UNIT_GRADE.NUG_R => new Color(0f, 48f / 85f, 1f, 1f), 
			NKM_UNIT_GRADE.NUG_SR => new Color(64f / 85f, 0f, 1f, 1f), 
			NKM_UNIT_GRADE.NUG_SSR => new Color(1f, 64f / 85f, 0.003921569f, 1f), 
			_ => new Color(1f, 1f, 1f, 1f), 
		};
	}

	public static Color GetColorForItemGrade(NKM_ITEM_GRADE eNKM_ITEM_GRADE)
	{
		return eNKM_ITEM_GRADE switch
		{
			NKM_ITEM_GRADE.NIG_N => new Color(0.6705883f, 0.6705883f, 0.6705883f, 1f), 
			NKM_ITEM_GRADE.NIG_R => new Color(0f, 48f / 85f, 1f, 1f), 
			NKM_ITEM_GRADE.NIG_SR => new Color(64f / 85f, 0f, 1f, 1f), 
			NKM_ITEM_GRADE.NIG_SSR => new Color(1f, 64f / 85f, 0.003921569f, 1f), 
			_ => new Color(1f, 1f, 1f, 1f), 
		};
	}

	public static string GetColorTagForUnitGrade(NKM_UNIT_GRADE eNKM_UNIT_GRADE)
	{
		return "<color=#" + GetColorCodeForUnitGrade(eNKM_UNIT_GRADE) + ">";
	}

	public static string GetColorCodeForUnitGrade(NKM_UNIT_GRADE eNKM_UNIT_GRADE)
	{
		return eNKM_UNIT_GRADE switch
		{
			NKM_UNIT_GRADE.NUG_N => "CFCFCFFF", 
			NKM_UNIT_GRADE.NUG_R => "008FFFFF", 
			NKM_UNIT_GRADE.NUG_SR => "FF00FEFF", 
			NKM_UNIT_GRADE.NUG_SSR => "FFF000FF", 
			_ => "FFFFFFFF", 
		};
	}

	public static HashSet<long> GetEquipsBeingUsed(Dictionary<long, NKMUnitData> _dicUnit, NKMInventoryData cNKMInventoryData)
	{
		HashSet<long> hashSet = new HashSet<long>();
		foreach (KeyValuePair<long, NKMUnitData> item in _dicUnit)
		{
			NKMUnitData value = item.Value;
			for (int i = 0; i < 4; i++)
			{
				long equipUid = value.GetEquipUid((ITEM_EQUIP_POSITION)i);
				if (equipUid > 0)
				{
					NKMEquipItemData itemEquip = cNKMInventoryData.GetItemEquip(equipUid);
					if (itemEquip != null)
					{
						hashSet.Add(itemEquip.m_ItemUid);
					}
				}
			}
		}
		return hashSet;
	}

	public static string GetEquipTypeIconIMGName(NKMEquipTemplet equipTemplet)
	{
		ITEM_EQUIP_POSITION itemEquipPosition = equipTemplet.m_ItemEquipPosition;
		NKM_UNIT_STYLE_TYPE equipUnitStyleType = equipTemplet.m_EquipUnitStyleType;
		string text = "";
		switch (equipUnitStyleType)
		{
		case NKM_UNIT_STYLE_TYPE.NUST_COUNTER:
			switch (itemEquipPosition)
			{
			case ITEM_EQUIP_POSITION.IEP_ACC:
			case ITEM_EQUIP_POSITION.IEP_ACC2:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_COUNTER_ACC";
			case ITEM_EQUIP_POSITION.IEP_DEFENCE:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_COUNTER_DEFENCE";
			case ITEM_EQUIP_POSITION.IEP_WEAPON:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_COUNTER_WEAPON";
			case ITEM_EQUIP_POSITION.IEP_ENCHANT:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_ENCHANT";
			default:
				return "";
			}
		case NKM_UNIT_STYLE_TYPE.NUST_MECHANIC:
			switch (itemEquipPosition)
			{
			case ITEM_EQUIP_POSITION.IEP_ACC:
			case ITEM_EQUIP_POSITION.IEP_ACC2:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_MECHANIC_ACC";
			case ITEM_EQUIP_POSITION.IEP_DEFENCE:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_MECHANIC_DEFENCE";
			case ITEM_EQUIP_POSITION.IEP_WEAPON:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_MECHANIC_WEAPON";
			case ITEM_EQUIP_POSITION.IEP_ENCHANT:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_ENCHANT";
			default:
				return "";
			}
		case NKM_UNIT_STYLE_TYPE.NUST_SOLDIER:
			switch (itemEquipPosition)
			{
			case ITEM_EQUIP_POSITION.IEP_ACC:
			case ITEM_EQUIP_POSITION.IEP_ACC2:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_SOLDIER_ACC";
			case ITEM_EQUIP_POSITION.IEP_DEFENCE:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_SOLDIER_DEFENCE";
			case ITEM_EQUIP_POSITION.IEP_WEAPON:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_SOLDIER_WEAPON";
			case ITEM_EQUIP_POSITION.IEP_ENCHANT:
				return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_ENCHANT";
			default:
				return "";
			}
		case NKM_UNIT_STYLE_TYPE.NUST_ENCHANT:
			return text + "AB_UI_ITEM_EQUIP_SLOT_ITEM_TYPE_ENCHANT";
		default:
			return "";
		}
	}

	public static Color GetColor(string hexRGB)
	{
		Color color = Color.white;
		ColorUtility.TryParseHtmlString(hexRGB, out color);
		return color;
	}

	public static Color GetPotentialOptionColor(int precision)
	{
		if (precision >= 91)
		{
			return GetColor("#ff7f01");
		}
		if (precision >= 81)
		{
			return GetColor("#ffc001");
		}
		if (precision >= 61)
		{
			return GetColor("#ffe493");
		}
		if (precision >= 41)
		{
			return GetColor("#f6f6f6");
		}
		return GetColor("#82838d");
	}

	public static Sprite GetButtonSprite(ButtonColor type)
	{
		string text = "";
		string bundleName = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE";
		switch (type)
		{
		case ButtonColor.BC_BLUE:
			text = "NKM_UI_POPUP_BLUEBUTTON";
			break;
		case ButtonColor.BC_GRAY:
			text = "NKM_UI_POPUP_BUTTON_02";
			break;
		case ButtonColor.BC_RED:
			text = "NKM_UI_POPUP_BUTTON_03";
			break;
		case ButtonColor.BC_YELLOW:
			text = "NKM_UI_POPUP_BUTTON_01";
			break;
		case ButtonColor.BC_LOGIN_BLUE:
			text = "AB_UI_LOGIN_BUTTON1";
			bundleName = "AB_UI_LOGIN_SPRITE";
			break;
		case ButtonColor.BC_LOGIN_YELLOW:
			text = "AB_UI_LOGIN_BUTTON2";
			bundleName = "AB_UI_LOGIN_SPRITE";
			break;
		case ButtonColor.BC_COMMON_ENABLE:
			text = "NKM_UI_COMMON_BTN_02";
			break;
		case ButtonColor.BC_COMMON_DISABLE:
			text = "NKM_UI_COMMON_BTN_01";
			break;
		default:
			UnityEngine.Debug.LogError("unknown GetButtonSprite type");
			return null;
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(bundleName, text);
		if (orLoadAssetResource == null)
		{
			UnityEngine.Debug.LogError($"UnitSprite {text}(From ButtonColor_TYPE {type.ToString()}) not found");
		}
		return orLoadAssetResource;
	}

	public static Sprite GetSpriteCommonIConStar(int starLv)
	{
		if (starLv < 1 || starLv > 6)
		{
			return null;
		}
		string text = "";
		string bundleName = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE";
		switch (starLv)
		{
		case 1:
			text = "NKM_UI_COMMON_ICON_STAR_1";
			break;
		case 2:
			text = "NKM_UI_COMMON_ICON_STAR_2";
			break;
		case 3:
			text = "NKM_UI_COMMON_ICON_STAR_3";
			break;
		case 4:
			text = "NKM_UI_COMMON_ICON_STAR_4";
			break;
		case 5:
			text = "NKM_UI_COMMON_ICON_STAR_5";
			break;
		case 6:
			text = "NKM_UI_COMMON_ICON_STAR_6";
			break;
		default:
			UnityEngine.Debug.LogError("unknown GetButtonSprite type");
			return null;
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(bundleName, text);
		if (orLoadAssetResource == null)
		{
			UnityEngine.Debug.LogError($"UnitSprite {text}(From GetSpriteCommonIConStar {starLv}) not found");
		}
		return orLoadAssetResource;
	}

	public static Sprite GetSpriteUnitGrade(NKM_UNIT_GRADE grade)
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_ICON", grade switch
		{
			NKM_UNIT_GRADE.NUG_R => "NKM_UI_COMMON_RANK_R", 
			NKM_UNIT_GRADE.NUG_SR => "NKM_UI_COMMON_RANK_SR", 
			NKM_UNIT_GRADE.NUG_SSR => "NKM_UI_COMMON_RANK_SSR", 
			_ => "NKM_UI_COMMON_RANK_N", 
		});
	}

	public static Sprite GetSpriteBattleConditionICon(NKMBattleConditionTemplet templet)
	{
		if (templet != null)
		{
			return GetSpriteBattleConditionICon(templet.BattleCondInfoIcon);
		}
		return null;
	}

	public static Sprite GetSpriteBattleConditionICon(string name)
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_BC", name);
	}

	public static Sprite GetSpriteFierceBattleBackgroud(bool bNightMareMode = false)
	{
		string assetName = (bNightMareMode ? "NKM_UI_FIERCE_BATTLE_BG_2" : "NKM_UI_FIERCE_BATTLE_BG");
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_fierce_battle_bg", assetName);
	}

	public static Sprite GetSpriteEquipSetOptionIcon(NKMItemEquipSetOptionTemplet templet)
	{
		if (templet != null)
		{
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_EQUIP_SET_ICON", templet.m_EquipSetIcon);
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_EQUIP_SET_ICON", "ICON_SET_NONE");
	}

	public static Sprite GetSpriteOperatorBG(NKM_UNIT_GRADE grade)
	{
		string text = "";
		string bundleName = "ab_ui_nkm_ui_operator_deck_sprite";
		switch (grade)
		{
		case NKM_UNIT_GRADE.NUG_SSR:
			text = "NKM_UI_OPERATOR_DECK_SLOT_RARE_SSR";
			break;
		case NKM_UNIT_GRADE.NUG_SR:
			text = "NKM_UI_OPERATOR_DECK_SLOT_RARE_SR";
			break;
		case NKM_UNIT_GRADE.NUG_R:
			text = "NKM_UI_OPERATOR_DECK_SLOT_RARE_R";
			break;
		case NKM_UNIT_GRADE.NUG_N:
			text = "NKM_UI_OPERATOR_DECK_SLOT_RARE_N";
			break;
		default:
			UnityEngine.Debug.LogError("unknown GetButtonSprite type");
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>(bundleName, text);
	}

	public static Color GetSkillTypeColor(NKM_SKILL_TYPE type)
	{
		Color color = Color.black;
		switch (type)
		{
		case NKM_SKILL_TYPE.NST_PASSIVE:
			ColorUtility.TryParseHtmlString("#FED427", out color);
			break;
		case NKM_SKILL_TYPE.NST_ATTACK:
		case NKM_SKILL_TYPE.NST_SKILL:
			ColorUtility.TryParseHtmlString("#4EC2F2", out color);
			break;
		case NKM_SKILL_TYPE.NST_HYPER:
		case NKM_SKILL_TYPE.NST_SHIP_ACTIVE:
			ColorUtility.TryParseHtmlString("#D600D4", out color);
			break;
		case NKM_SKILL_TYPE.NST_LEADER:
			ColorUtility.TryParseHtmlString("#FED427", out color);
			break;
		default:
			UnityEngine.Debug.LogError("Unknown skill type");
			break;
		}
		return color;
	}

	public static Sprite GetSkillIconSprite(NKMUnitSkillTemplet unitskillTemplet)
	{
		if (unitskillTemplet == null)
		{
			return null;
		}
		string unitSkillIcon = unitskillTemplet.m_UnitSkillIcon;
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_UNIT_SKILL_ICON", unitSkillIcon);
		if (orLoadAssetResource == null)
		{
			UnityEngine.Debug.LogError($"SkillSprite {unitSkillIcon}(From UnitSkillTemplet StrID {unitskillTemplet.m_strID}) not found");
		}
		return orLoadAssetResource;
	}

	public static Sprite GetSkillIconSprite(NKMOperatorSkillTemplet skillTemplet)
	{
		if (skillTemplet == null)
		{
			return null;
		}
		string operSkillIcon = skillTemplet.m_OperSkillIcon;
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_TACTICAL_COMMAND_ICON", operSkillIcon);
		if (orLoadAssetResource == null)
		{
			UnityEngine.Debug.LogError($"SkillSprite {operSkillIcon}(From UnitSkillTemplet StrID {skillTemplet.m_OperSkillNameStrID}) not found");
		}
		return orLoadAssetResource;
	}

	public static Sprite GetSkillIconSprite(NKMShipSkillTemplet shipSkillTemplet)
	{
		if (shipSkillTemplet == null)
		{
			return null;
		}
		string shipSkillIcon = shipSkillTemplet.m_ShipSkillIcon;
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_SHIP_SKILL_ICON", shipSkillIcon);
		if (orLoadAssetResource == null)
		{
			UnityEngine.Debug.LogError($"SkillSprite {shipSkillIcon}(From ShipSkillTemplet StrID {shipSkillTemplet.m_ShipSkillStrID}) not found");
		}
		return orLoadAssetResource;
	}

	public static Sprite GetMoveTypeImg(bool bAirUnit)
	{
		if (bAirUnit)
		{
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_ICON", "UI_COMMON_UNIT_TYPE_AIR");
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_ICON", "UI_COMMON_UNIT_TYPE_LAND");
	}

	public static Sprite GetMissionThumbnailSprite(NKMMissionTemplet missionTemplet)
	{
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId);
		if (missionTabTemplet == null)
		{
			return null;
		}
		if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.GROWTH_COMPLETE)
		{
			if (NKMMissionManager.GetMissionTemplet(missionTemplet.m_MissionRequire) != null)
			{
				return GetMissionThumbnailSprite(NKMMissionManager.GetMissionTemplet(missionTemplet.m_MissionRequire).m_MissionTabId);
			}
			return null;
		}
		return GetMissionThumbnailSprite(missionTemplet.m_MissionTabId);
	}

	public static Sprite GetMissionThumbnailSprite(int tabID)
	{
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(tabID);
		if (missionTabTemplet == null)
		{
			return null;
		}
		string bundleName = "AB_UI_NKM_UI_MISSION_TEXTURE";
		if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
		{
			bundleName = "ui_mission_guide_texture";
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>(bundleName, missionTabTemplet.m_SlotBannerName);
	}

	public static Sprite GetGrowthMissionHamburgerIconSprite(NKMMissionTemplet missionTemplet)
	{
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			if (missionTabTemplet.m_MissionType != NKM_MISSION_TYPE.GROWTH_COMPLETE)
			{
				return GetGrowthMissionHamburgerIconSprite(missionTemplet.m_MissionTabId);
			}
			if (NKMMissionManager.GetMissionTemplet(missionTemplet.m_MissionRequire) != null)
			{
				return GetGrowthMissionHamburgerIconSprite(NKMMissionManager.GetMissionTemplet(missionTemplet.m_MissionRequire).m_MissionTabId);
			}
		}
		return null;
	}

	public static Sprite GetGrowthMissionHamburgerIconSprite(int tabID)
	{
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(tabID);
		if (missionTabTemplet == null)
		{
			return null;
		}
		string arg = missionTabTemplet.m_MissionTabIconName.Substring(missionTabTemplet.m_MissionTabIconName.Length - 2);
		string assetName = "";
		if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.GROWTH)
		{
			assetName = $"NKM_UI_COMMON_ICON_GROWTH_{arg}";
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", assetName);
	}

	public static Sprite GetCompanyBuffIconSprite(NKMCompanyBuffData buff)
	{
		string text = "";
		NKMCompanyBuffTemplet nKMCompanyBuffTemplet = NKMTempletContainer<NKMCompanyBuffTemplet>.Find(buff.Id);
		text = ((nKMCompanyBuffTemplet == null || string.IsNullOrEmpty(nKMCompanyBuffTemplet.m_CompanyBuffIcon)) ? "EVENTBUFF_SAMPLE" : nKMCompanyBuffTemplet.m_CompanyBuffIcon);
		return GetCompanyBuffIconSprite(text);
	}

	public static Sprite GetCompanyBuffIconSprite(string iconName)
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_EVENTBUFF_ICON", iconName);
	}

	public static Sprite GetShopSprite(string spritePath)
	{
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_SHOP_SKIN_SPRITE", spritePath);
		if (orLoadAssetResource != null)
		{
			return orLoadAssetResource;
		}
		return null;
	}

	public static void SetShopReddotImage(ShopReddotType reddotType, GameObject objReddotRoot, GameObject objRed, GameObject objYellow)
	{
		if (!(objReddotRoot == null))
		{
			SetGameobjectActive(objReddotRoot, reddotType != ShopReddotType.NONE);
			SetGameobjectActive(objRed, reddotType == ShopReddotType.REDDOT_PURCHASED);
			SetGameobjectActive(objYellow, reddotType == ShopReddotType.REDDOT_CHECKED);
		}
	}

	public static void SetShopReddotLabel(ShopReddotType reddotType, Text lbReddot, int reddotCount)
	{
		if (lbReddot == null)
		{
			return;
		}
		if (reddotCount > 0)
		{
			if (reddotCount > 99)
			{
				SetLabelText(lbReddot, "...");
			}
			else
			{
				SetLabelText(lbReddot, reddotCount.ToString());
			}
		}
		else
		{
			SetLabelText(lbReddot, "");
		}
	}

	public static string GetCompanyBuffDesc(int buffID)
	{
		StringBuilder stringBuilder = new StringBuilder();
		NKMCompanyBuffTemplet nKMCompanyBuffTemplet = NKMTempletContainer<NKMCompanyBuffTemplet>.Find(buffID);
		if (nKMCompanyBuffTemplet != null)
		{
			for (int i = 0; i < nKMCompanyBuffTemplet.m_CompanyBuffInfoList.Count && nKMCompanyBuffTemplet.m_CompanyBuffInfoList[i].m_CompanyBuffType != NKMConst.Buff.BuffType.NONE; i++)
			{
				if (nKMCompanyBuffTemplet.m_CompanyBuffInfoList[i].m_CompanyBuffType != NKMConst.Buff.BuffType.WARFARE_DUNGEON_REWARD_DROP_BONUS)
				{
					if (i != 0 && stringBuilder.Length > 0)
					{
						stringBuilder.Append("\n");
					}
					switch (nKMCompanyBuffTemplet.m_CompanyBuffInfoList[i].m_CompanyBuffType)
					{
					case NKMConst.Buff.BuffType.NONE:
						return string.Empty;
					case NKMConst.Buff.BuffType.WARFARE_DUNGEON_REWARD_CREDIT:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_RWDBOUNS_CREDIT);
						break;
					case NKMConst.Buff.BuffType.WARFARE_DUNGEON_REWARD_EXP_COMPANY:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_RWDBOUNS_EXP_PLAYER);
						break;
					case NKMConst.Buff.BuffType.WARFARE_DUNGEON_REWARD_EXP_UNIT:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_RWDBOUNS_EXP_UNIT);
						break;
					case NKMConst.Buff.BuffType.WARFARE_ETERNIUM_DISCOUNT:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_WARFARE_ETNM_DISCOUNT);
						break;
					case NKMConst.Buff.BuffType.WARFARE_DUNGEON_ETERNIUM_DISCOUNT:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_WARFARE_DUNGEON_ETNM_DISCOUNT);
						break;
					case NKMConst.Buff.BuffType.PVP_POINT_CHARGE:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_PVP_POINT_CHARGE);
						break;
					case NKMConst.Buff.BuffType.ALL_PVP_POINT_REWARD:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_PVP_POINT_REWARD);
						break;
					case NKMConst.Buff.BuffType.WORLDMAP_MISSION_COMPLETE_RATIO_BONUS:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_CITY_MISSION_WMMR_S_UP);
						break;
					case NKMConst.Buff.BuffType.BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_NEGOTIATION_CREDIT_DISCOUNT);
						break;
					case NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_FACTORY_CRAFT_CREDIT_DISCOUNT);
						break;
					case NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT);
						break;
					case NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_COST_DISCOUNT:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_COST_DISCOUNT);
						break;
					case NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SSR:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SSR);
						break;
					case NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SR:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SR);
						break;
					case NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_R:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_R);
						break;
					case NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N);
						break;
					case NKMConst.Buff.BuffType.BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT:
						stringBuilder.Append(NKCUtilString.GET_EVENT_BUFF_TYPE_BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT);
						break;
					}
					stringBuilder.Append(" ");
					if (nKMCompanyBuffTemplet.m_CompanyBuffInfoList[i].m_CompanyBuffType == NKMConst.Buff.BuffType.WARFARE_ETERNIUM_DISCOUNT)
					{
						stringBuilder.AppendFormat("-{0}%", nKMCompanyBuffTemplet.m_CompanyBuffInfoList[i].m_CompanyBuffRatio);
					}
					else
					{
						stringBuilder.AppendFormat("+{0}%", nKMCompanyBuffTemplet.m_CompanyBuffInfoList[i].m_CompanyBuffRatio);
					}
				}
			}
		}
		return stringBuilder.ToString();
	}

	public static Sprite GetBounsTypeIcon(RewardTuningType type, bool big = true)
	{
		string text = "";
		switch (type)
		{
		case RewardTuningType.Credit:
			text = ((!big) ? "NKM_UI_OPERATION_BONUSTYPE_ICON_CREDIT_TEXT" : "NKM_UI_OPERATION_BONUSTYPE_ICON_CREDIT");
			break;
		case RewardTuningType.Eternium:
			text = ((!big) ? "NKM_UI_OPERATION_BONUSTYPE_ICON_ETERNIUM_TEXT" : "NKM_UI_OPERATION_BONUSTYPE_ICON_ETERNIUM");
			break;
		case RewardTuningType.UnitExp:
			text = ((!big) ? "NKM_UI_OPERATION_BONUSTYPE_ICON_UNIT_EXP_TEXT" : "NKM_UI_OPERATION_BONUSTYPE_ICON_UNIT_EXP");
			break;
		case RewardTuningType.UserExp:
			text = ((!big) ? "NKM_UI_OPERATION_BONUSTYPE_ICON_USER_EXP_TEXT" : "NKM_UI_OPERATION_BONUSTYPE_ICON_USER_EXP");
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_common_operation_bonustype", text);
	}

	public static float TrackValue(TRACKING_DATA_TYPE type, float beginValue, float endValue, float currentTime, float totalTime)
	{
		float progress = NKMTrackingFloat.TrackRatio(type, currentTime, totalTime);
		return Lerp(beginValue, endValue, progress);
	}

	public static Vector2 TrackValue(TRACKING_DATA_TYPE type, Vector2 beginValue, Vector2 endValue, float currentTime, float totalTime)
	{
		float progress = NKMTrackingFloat.TrackRatio(type, currentTime, totalTime);
		return Lerp(beginValue, endValue, progress);
	}

	public static Vector3 TrackValue(TRACKING_DATA_TYPE type, Vector3 beginValue, Vector3 endValue, float currentTime, float totalTime)
	{
		float progress = NKMTrackingFloat.TrackRatio(type, currentTime, totalTime);
		return Lerp(beginValue, endValue, progress);
	}

	public static float Lerp(float beginValue, float endValue, float progress)
	{
		return beginValue + (endValue - beginValue) * progress;
	}

	public static Vector2 Lerp(Vector2 beginValue, Vector2 endValue, float progress)
	{
		return beginValue + (endValue - beginValue) * progress;
	}

	public static Vector3 Lerp(Vector3 beginValue, Vector3 endValue, float progress)
	{
		return beginValue + (endValue - beginValue) * progress;
	}

	public static int GetItemStarCount(NKM_ITEM_GRADE grade)
	{
		return grade switch
		{
			NKM_ITEM_GRADE.NIG_N => 2, 
			NKM_ITEM_GRADE.NIG_R => 3, 
			NKM_ITEM_GRADE.NIG_SR => 4, 
			NKM_ITEM_GRADE.NIG_SSR => 5, 
			_ => 1, 
		};
	}

	public static Sprite GetShipGradeSprite(NKM_UNIT_GRADE grade)
	{
		string text = "ab_ui_ship_slot_card_sprite";
		string assetName = "";
		switch (grade)
		{
		case NKM_UNIT_GRADE.NUG_SSR:
			assetName = "NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_SSR";
			break;
		case NKM_UNIT_GRADE.NUG_SR:
			assetName = "NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_SR";
			break;
		case NKM_UNIT_GRADE.NUG_R:
			assetName = "NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_R";
			break;
		case NKM_UNIT_GRADE.NUG_N:
			assetName = "NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_N";
			break;
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(text, assetName);
		if (orLoadAssetResource == null)
		{
			UnityEngine.Debug.LogError($"Fail - Get Ship Grade Sprite, not found {grade} - {text}");
		}
		return orLoadAssetResource;
	}

	public static int GetShipStartCost(int shipLevel)
	{
		return Math.Min(10, 4 + shipLevel);
	}

	public static bool ProcessDeckErrorMsg(NKM_ERROR_CODE errorCode)
	{
		if (errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(errorCode);
			return false;
		}
		return true;
	}

	public static void SetLayer(Transform trans, int layer)
	{
		if (trans == null)
		{
			return;
		}
		trans.gameObject.layer = layer;
		foreach (Transform tran in trans)
		{
			SetLayer(tran, layer);
		}
	}

	public static bool CheckExistRewardType(List<int> groupIds, NKM_REWARD_TYPE rewardType)
	{
		if (rewardType == NKM_REWARD_TYPE.RT_MISC && (groupIds.Contains(1031) || groupIds.Contains(1032) || groupIds.Contains(1033)))
		{
			return true;
		}
		for (int i = 0; i < groupIds.Count; i++)
		{
			NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(groupIds[i]);
			if (rewardGroup == null)
			{
				continue;
			}
			for (int j = 0; j < rewardGroup.List.Count; j++)
			{
				if (rewardGroup.List[j].m_eRewardType == rewardType)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool CheckEventDropReward(NKMStageTempletV2 stageTemplet, List<int> groupIds, NKM_REWARD_TYPE rewardType)
	{
		if (stageTemplet == null)
		{
			return false;
		}
		if (stageTemplet.DungeonTempletBase == null)
		{
			return false;
		}
		if (stageTemplet.DungeonTempletBase.m_EventRewardRateDate == null)
		{
			return false;
		}
		if (!stageTemplet.DungeonTempletBase.m_EventRewardRateDate.IsValidTime(NKCSynchronizedTime.ServiceTime))
		{
			return false;
		}
		for (int i = 0; i < groupIds.Count; i++)
		{
			NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(groupIds[i]);
			if (rewardGroup == null)
			{
				continue;
			}
			for (int j = 0; j < rewardGroup.List.Count; j++)
			{
				if (rewardGroup.List[j].m_eRewardType == rewardType && stageTemplet.DungeonTempletBase.ViewEventTagRewardGroup.Contains(rewardGroup.GroupId))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static HashSet<int> GetRewardIDs(List<int> groupIds, NKM_REWARD_TYPE rewardType)
	{
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < groupIds.Count; i++)
		{
			NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(groupIds[i]);
			if (rewardGroup != null)
			{
				for (int j = 0; j < rewardGroup.List.Count; j++)
				{
					if (rewardGroup.List[j].m_eRewardType == rewardType)
					{
						hashSet.Add(rewardGroup.List[j].m_RewardID);
					}
				}
			}
			if (rewardType == NKM_REWARD_TYPE.RT_MISC && (groupIds[i] == 1031 || groupIds[i] == 1032 || groupIds[i] == 1033))
			{
				hashSet.Add(groupIds[i]);
			}
		}
		return hashSet;
	}

	public static int GetMaxGradeInRewardGroups(List<int> groupIds, NKM_REWARD_TYPE rewardType)
	{
		int num = -1;
		for (int i = 0; i < groupIds.Count; i++)
		{
			if (rewardType == NKM_REWARD_TYPE.RT_MISC)
			{
				if (groupIds[i] == 1031)
				{
					num = 0;
					continue;
				}
				if (groupIds[i] == 1032)
				{
					num = 1;
					continue;
				}
				if (groupIds[i] == 1033)
				{
					num = 2;
					continue;
				}
			}
			NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(groupIds[i]);
			if (rewardGroup == null)
			{
				continue;
			}
			for (int j = 0; j < rewardGroup.List.Count; j++)
			{
				if (rewardGroup.List[j].m_eRewardType == rewardType)
				{
					int rewardGrade = GetRewardGrade(rewardGroup.List[j].m_RewardID, rewardType);
					if (num < rewardGrade)
					{
						num = rewardGrade;
					}
				}
			}
		}
		return num;
	}

	public static int GetMaxGradeInRewardGroups(int rewardGroupID)
	{
		NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(rewardGroupID);
		if (rewardGroup == null)
		{
			return -1;
		}
		int num = -1;
		for (int i = 0; i < rewardGroup.List.Count; i++)
		{
			int rewardGrade = GetRewardGrade(rewardGroup.List[i].m_RewardID, rewardGroup.List[i].m_eRewardType);
			if (num < rewardGrade)
			{
				num = rewardGrade;
			}
		}
		return num;
	}

	public static int GetRewardGrade(int rewardID, NKM_REWARD_TYPE rewardType)
	{
		int result = -1;
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_EQUIP:
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(rewardID);
			if (equipTemplet != null)
			{
				result = (int)equipTemplet.m_NKM_ITEM_GRADE;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MISC:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(rewardID);
			if (itemMiscTempletByID != null)
			{
				result = (int)itemMiscTempletByID.m_NKM_ITEM_GRADE;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(rewardID);
			if (unitTempletBase != null)
			{
				result = (int)unitTempletBase.m_NKM_UNIT_GRADE;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MOLD:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(rewardID);
			if (itemMoldTempletByID != null)
			{
				result = (int)itemMoldTempletByID.m_Grade;
			}
			break;
		}
		default:
			UnityEngine.Debug.LogError("not yet implemented type : " + rewardType);
			return 0;
		}
		return result;
	}

	public static string GetRewardStrID(int rewardID, NKM_REWARD_TYPE rewardType)
	{
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_EQUIP:
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(rewardID);
			if (equipTemplet != null)
			{
				return equipTemplet.m_ItemEquipStrID;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MISC:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(rewardID);
			if (itemMiscTempletByID != null)
			{
				return itemMiscTempletByID.m_ItemMiscStrID;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(rewardID);
			if (unitTempletBase != null)
			{
				return unitTempletBase.m_UnitStrID;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MOLD:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(rewardID);
			if (itemMoldTempletByID != null)
			{
				return itemMoldTempletByID.m_MoldStrID;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(rewardID);
			if (skinTemplet == null)
			{
				return string.Empty;
			}
			return skinTemplet.m_SkinStrID;
		}
		default:
			UnityEngine.Debug.LogError("not yet implemented type : " + rewardType);
			return string.Empty;
		}
		return string.Empty;
	}

	public static void PlayStartCutscenAndStartGame(NKMGameData cNKMGameData)
	{
		bool flag = false;
		if (cNKMGameData != null && !cNKMGameData.m_bLocal)
		{
			bool flag2 = true;
			if (NKCScenManager.CurrentUserData() != null)
			{
				flag2 = NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene;
			}
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			int dungeonID = cNKMGameData.m_DungeonID;
			NKMStageTempletV2 stageTemplet = ((!NKCPhaseManager.IsCurrentPhaseDungeon(dungeonID)) ? NKMDungeonManager.GetDungeonTempletBase(cNKMGameData.m_DungeonID).StageTemplet : NKCPhaseManager.GetStageTemplet());
			bool flag3 = false;
			if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_WARFARE)
			{
				flag3 = NKCScenManager.CurrentUserData().m_UserOption.m_bAutoWarfare;
			}
			else if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_DIVE)
			{
				flag3 = NKCScenManager.CurrentUserData().m_UserOption.m_bAutoDive;
			}
			bool isOnGoing = NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing();
			bool flag4 = false;
			if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_TRIM)
			{
				if (NKCTrimManager.TrimModeState != null)
				{
					flag4 = NKCTrimManager.WillPlayTrimDungeonCutscene(NKCTrimManager.TrimModeState.trimId, dungeonID, NKCTrimManager.TrimModeState.trimLevel);
				}
			}
			else
			{
				flag4 = dungeonID > 0 && !flag3 && (!myUserData.CheckStageCleared(stageTemplet) || (flag2 && !isOnGoing));
			}
			if (flag4)
			{
				NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
				if (dungeonTempletBase != null)
				{
					NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(dungeonTempletBase.m_CutScenStrIDBefore);
					if (cutScenTemple != null)
					{
						NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(cutScenTemple.m_CutScenStrID, delegate
						{
							NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
						}, dungeonTempletBase.m_DungeonStrID);
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
						flag = true;
					}
				}
			}
		}
		if (!flag)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
		}
		if (cNKMGameData == null || cNKMGameData.m_WarfareID != 0 || cNKMGameData.m_DungeonID <= 0)
		{
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase2 = NKMDungeonManager.GetDungeonTempletBase(cNKMGameData.m_DungeonID);
		if (dungeonTempletBase2 != null)
		{
			string key = $"{NKCScenManager.CurrentUserData().m_UserUID}_{dungeonTempletBase2.m_DungeonStrID}";
			if (!PlayerPrefs.HasKey(key) && !NKCScenManager.CurrentUserData().CheckWarfareClear(dungeonTempletBase2.m_DungeonStrID))
			{
				PlayerPrefs.SetInt(key, 0);
			}
		}
	}

	public static bool IsCanStartEterniumStage(NKMStageTempletV2 stageTemplet, bool bCallLackPopup = false)
	{
		if (stageTemplet != null)
		{
			return IsCanStartEterniumStage(stageTemplet.m_StageReqItemID, stageTemplet.m_StageReqItemCount, bCallLackPopup);
		}
		return true;
	}

	public static bool IsCanStartEterniumStage(int reqItemID, int reqItemCount, bool bCallLackPopup = false)
	{
		if (reqItemID == 2)
		{
			NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref reqItemCount);
			if (!NKCScenManager.CurrentUserData().CheckPrice(reqItemCount, 2))
			{
				if (bCallLackPopup)
				{
					if (!NKCAdManager.IsAdRewardItem(2))
					{
						NKCShopManager.OpenItemLackPopup(2, reqItemCount);
					}
					else
					{
						NKCPopupItemLack.Instance.OpenItemLackAdRewardPopup(2, delegate
						{
							NKCShopManager.OpenItemLackPopup(2, reqItemCount);
						});
					}
				}
				return false;
			}
		}
		return true;
	}

	public static void ChangeEquip(long unitUID, ITEM_EQUIP_POSITION equipPos, NKCUISlotEquip.OnSelectedEquipSlot _OnClickEmptySlot = null, long selectedItemUID = 0L, bool bShowFierceUI = false)
	{
		NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(unitUID);
		if (unitFromUID == null)
		{
			return;
		}
		if (unitFromUID.IsSeized)
		{
			NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED);
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
		if (unitTempletBase == null)
		{
			return;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMUnitManager.IsUnitBusy(NKCScenManager.GetScenManager().GetMyUserData(), unitFromUID, ignoreWorldmapState: true);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(nKM_ERROR_CODE.ToString()));
			return;
		}
		NKCUIInventory.EquipSelectListOptions options = new NKCUIInventory.EquipSelectListOptions(NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL, _bMultipleSelect: false);
		options.m_dOnClickEmptySlot = _OnClickEmptySlot;
		options.m_NKC_INVENTORY_OPEN_TYPE = NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT;
		options.lastSelectedItemUID = selectedItemUID;
		options.m_EquipListOptions.iTargetUnitID = unitFromUID.m_UnitID;
		options.bShowFierceUI = bShowFierceUI;
		options.lEquipOptionCachingByUnitUID = unitUID;
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
		if (unitTempletBase2 != null)
		{
			options.strUpsideMenuName = string.Format(NKCUtilString.GET_STRING_CHOICE_ONE_PARAM, NKCUtilString.GetEquipPosSimpleStrByUnitStyle(unitTempletBase2.m_NKM_UNIT_STYLE_TYPE, equipPos));
		}
		options.setFilterOption = new HashSet<NKCEquipSortSystem.eFilterOption> { NKCEquipSortSystem.GetFilterOptionByEquipPosition(equipPos) };
		if (unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_COUNTER)
		{
			options.setFilterOption.Add(NKCEquipSortSystem.eFilterOption.Equip_Counter);
		}
		else if (unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_SOLDIER)
		{
			options.setFilterOption.Add(NKCEquipSortSystem.eFilterOption.Equip_Soldier);
		}
		else if (unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_MECHANIC)
		{
			options.setFilterOption.Add(NKCEquipSortSystem.eFilterOption.Equip_Mechanic);
		}
		if (equipPos == ITEM_EQUIP_POSITION.IEP_ACC || equipPos == ITEM_EQUIP_POSITION.IEP_ACC2)
		{
			options.setExcludeEquipUID = new HashSet<long>();
			if (unitFromUID.GetEquipUid(ITEM_EQUIP_POSITION.IEP_ACC) > 0)
			{
				options.setExcludeEquipUID.Add(unitFromUID.GetEquipUid(ITEM_EQUIP_POSITION.IEP_ACC));
			}
			if (unitFromUID.GetEquipUid(ITEM_EQUIP_POSITION.IEP_ACC2) > 0)
			{
				options.setExcludeEquipUID.Add(unitFromUID.GetEquipUid(ITEM_EQUIP_POSITION.IEP_ACC2));
			}
		}
		else if (unitFromUID.GetEquipUid(equipPos) > 0)
		{
			options.setExcludeEquipUID = new HashSet<long> { unitFromUID.GetEquipUid(equipPos) };
		}
		options.equipChangeTargetPosition = equipPos;
		options.lstSortOption = new List<NKCEquipSortSystem.eSortOption>
		{
			NKCEquipSortSystem.eSortOption.Equipped_Last,
			NKCEquipSortSystem.eSortOption.Enhance_High,
			NKCEquipSortSystem.eSortOption.Tier_High,
			NKCEquipSortSystem.eSortOption.Rarity_High,
			NKCEquipSortSystem.eSortOption.UnitType_First,
			NKCEquipSortSystem.eSortOption.EquipType_FIrst,
			NKCEquipSortSystem.eSortOption.ID_First,
			NKCEquipSortSystem.eSortOption.UID_First
		};
		options.m_EquipListOptions.setExcludeFilterOption = new HashSet<NKCEquipSortSystem.eFilterOption> { NKCEquipSortSystem.eFilterOption.Equip_Enchant };
		options.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_EQUIP_TO_CHANGE;
		options.m_ButtonMenuType = NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_CHANGE;
		NKCUIInventory.Instance.Open(options, null, unitUID);
	}

	public static void ChangePresetEquip(long unitUId, int presetIndex, long equipUId, List<long> presetEquipUId, ITEM_EQUIP_POSITION equipPos, NKM_UNIT_STYLE_TYPE unitStyleType, bool bShowFierceUI = false, NKCUISlotEquip.OnSelectedEquipSlot _OnClickEmptySlot = null)
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(unitUId) == null)
		{
			return;
		}
		NKCUIInventory.EquipSelectListOptions options = new NKCUIInventory.EquipSelectListOptions(NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL, _bMultipleSelect: false);
		options.m_dOnClickEmptySlot = _OnClickEmptySlot;
		options.m_NKC_INVENTORY_OPEN_TYPE = NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT;
		options.lastSelectedItemUID = equipUId;
		options.bShowFierceUI = bShowFierceUI;
		options.iPresetIndex = presetIndex;
		options.lEquipOptionCachingByUnitUID = unitUId;
		options.setFilterOption = new HashSet<NKCEquipSortSystem.eFilterOption> { NKCEquipSortSystem.GetFilterOptionByEquipPosition(equipPos) };
		options.presetUnitStyeType = unitStyleType;
		switch (unitStyleType)
		{
		case NKM_UNIT_STYLE_TYPE.NUST_COUNTER:
			options.setFilterOption.Add(NKCEquipSortSystem.eFilterOption.Equip_Counter);
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SOLDIER:
			options.setFilterOption.Add(NKCEquipSortSystem.eFilterOption.Equip_Soldier);
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_MECHANIC:
			options.setFilterOption.Add(NKCEquipSortSystem.eFilterOption.Equip_Mechanic);
			break;
		}
		if (equipPos == ITEM_EQUIP_POSITION.IEP_ACC || equipPos == ITEM_EQUIP_POSITION.IEP_ACC2)
		{
			options.setExcludeEquipUID = new HashSet<long>();
			int num = 2;
			if (presetEquipUId.Count > num && presetEquipUId[num] > 0)
			{
				options.setExcludeEquipUID.Add(presetEquipUId[num]);
			}
			num = 3;
			if (presetEquipUId.Count > num && presetEquipUId[num] > 0)
			{
				options.setExcludeEquipUID.Add(presetEquipUId[num]);
			}
		}
		else if (presetEquipUId.Count > (int)equipPos && presetEquipUId[(int)equipPos] > 0)
		{
			options.setExcludeEquipUID = new HashSet<long> { presetEquipUId[(int)equipPos] };
		}
		options.equipChangeTargetPosition = equipPos;
		options.lstSortOption = new List<NKCEquipSortSystem.eSortOption>
		{
			NKCEquipSortSystem.eSortOption.Equipped_Last,
			NKCEquipSortSystem.eSortOption.Enhance_High,
			NKCEquipSortSystem.eSortOption.Tier_High,
			NKCEquipSortSystem.eSortOption.Rarity_High,
			NKCEquipSortSystem.eSortOption.UnitType_First,
			NKCEquipSortSystem.eSortOption.EquipType_FIrst,
			NKCEquipSortSystem.eSortOption.ID_First,
			NKCEquipSortSystem.eSortOption.UID_First
		};
		options.m_EquipListOptions.setExcludeFilterOption = new HashSet<NKCEquipSortSystem.eFilterOption> { NKCEquipSortSystem.eFilterOption.Equip_Enchant };
		options.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_EQUIP_TO_CHANGE;
		options.m_ButtonMenuType = NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_PRESET_CHANGE;
		NKCUIInventory.Instance.Open(options, null, unitUId);
	}

	public static bool IsPrivateEquipAlreadyEquipped(IReadOnlyList<long> lstEquipResult)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null || lstEquipResult == null)
		{
			return false;
		}
		int count = lstEquipResult.Count;
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < count; i++)
		{
			long itemUid = lstEquipResult[i];
			NKMEquipItemData itemEquip = nKMUserData.m_InventoryData.GetItemEquip(itemUid);
			if (itemEquip == null)
			{
				continue;
			}
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet != null && equipTemplet.IsPrivateEquip())
			{
				int maxUpgradedEquip = NKMItemEquipUpgradeTemplet.GetMaxUpgradedEquip(equipTemplet.m_ItemEquipID);
				if (hashSet.Contains(maxUpgradedEquip))
				{
					return true;
				}
				hashSet.Add(maxUpgradedEquip);
			}
		}
		return false;
	}

	public static bool ProcessWFExpireTime()
	{
		if (NKCScenManager.GetScenManager().WarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_EXPIRED));
		}
		return true;
	}

	public static bool ProcessDiveExpireTime()
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().m_DiveGameData == null)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_DIVE_EXPIRED));
		}
		return true;
	}

	public static void SetDiveTargetEventID()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool flag = false;
		if (nKMUserData != null && nKMUserData.m_DiveGameData != null && nKMUserData.m_DiveGameData.Floor.Templet.IsEventDive)
		{
			int cityIDByEventData = nKMUserData.m_WorldmapData.GetCityIDByEventData(NKM_WORLDMAP_EVENT_TYPE.WET_DIVE, nKMUserData.m_DiveGameData.DiveUid);
			if (cityIDByEventData != -1)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE_READY().SetTargetEventID(cityIDByEventData, nKMUserData.m_DiveGameData.Floor.Templet.StageID);
				flag = true;
			}
		}
		if (!flag)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE_READY().SetTargetEventID(0, 0);
		}
	}

	public static int GetEquipCreatableCount(NKMMoldItemData cNKMMoldItemData, NKMInventoryData cNKMInventoryData)
	{
		if (cNKMMoldItemData == null || cNKMInventoryData == null)
		{
			return 0;
		}
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(cNKMMoldItemData.m_MoldID);
		int num = 10;
		if (itemMoldTempletByID != null)
		{
			NKM_CRAFT_TAB_TYPE tabType = itemMoldTempletByID.m_TabType;
			num = ((tabType != NKM_CRAFT_TAB_TYPE.MT_EQUIP && (uint)(tabType - 8) > 3u) ? 999 : 10);
			if (!itemMoldTempletByID.m_bPermanent && num > cNKMMoldItemData.m_Count)
			{
				num = (int)cNKMMoldItemData.m_Count;
			}
			long num2 = 0L;
			for (int i = 0; i < itemMoldTempletByID.m_MaterialList.Count; i++)
			{
				NKMItemMoldMaterialData nKMItemMoldMaterialData = itemMoldTempletByID.m_MaterialList[i];
				if (nKMItemMoldMaterialData.m_MaterialType == NKM_REWARD_TYPE.RT_MISC)
				{
					int credit = itemMoldTempletByID.m_MaterialList[i].m_MaterialValue;
					if (nKMItemMoldMaterialData.m_MaterialID == 1)
					{
						NKCCompanyBuff.SetDiscountOfCreditInCraft(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
					}
					num2 = cNKMInventoryData.GetCountMiscItem(nKMItemMoldMaterialData.m_MaterialID) / credit;
					if (num > num2)
					{
						num = (int)num2;
					}
				}
			}
			int num3 = 0;
			num3 = ((!NKMItemManager.IsStackMoldItem(itemMoldTempletByID)) ? NKMItemManager.GetRemainResetCount(itemMoldTempletByID.m_ResetGroupId) : NKMItemManager.GetRemainResetCountStack(itemMoldTempletByID));
			if (num3 >= 0)
			{
				num = Math.Min(num, num3);
			}
			return num;
		}
		return 0;
	}

	public static Dictionary<string, string> ParseStringTable(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return new Dictionary<string, string>();
		}
		char[] separator = new char[1] { ';' };
		char[] separator2 = new char[1] { '=' };
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			if (!string.IsNullOrEmpty(text))
			{
				string[] array2 = text.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length != 2)
				{
					UnityEngine.Debug.LogError("Table parse error : 2 or more = in single statement");
					continue;
				}
				string key = array2[0].Trim();
				string value = array2[1].Trim();
				dictionary.Add(key, value);
			}
		}
		return dictionary;
	}

	public static int GetIntValue(Dictionary<string, string> dicParamTable, string key, int defaultValue = -1)
	{
		if (dicParamTable == null)
		{
			return defaultValue;
		}
		if (!dicParamTable.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		if (!int.TryParse(value, out var result))
		{
			return defaultValue;
		}
		return result;
	}

	public static Dictionary<int, string> ParseIntKeyTable(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return new Dictionary<int, string>();
		}
		char[] separator = new char[1] { ';' };
		char[] separator2 = new char[3] { ',', '=', ' ' };
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		string[] array = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			if (!string.IsNullOrEmpty(text))
			{
				string[] array2 = text.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length != 2)
				{
					UnityEngine.Debug.LogError("Table parse error : 2 or more , in single statement");
					continue;
				}
				if (!int.TryParse(array2[0].Trim(), out var result))
				{
					UnityEngine.Debug.LogError("Table parse error : Key parse failed");
					continue;
				}
				string value = array2[1].Trim();
				dictionary.Add(result, value);
			}
		}
		return dictionary;
	}

	public static Dictionary<TKey, TValue> ParseStringTable<TKey, TValue>(string input, Factory<TKey> keyFactory, Factory<TValue> valueFactory)
	{
		if (string.IsNullOrEmpty(input))
		{
			return new Dictionary<TKey, TValue>();
		}
		char[] separator = new char[1] { ';' };
		char[] separator2 = new char[2] { '=', ' ' };
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		string[] array = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			string[] array2 = text.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length != 2)
			{
				UnityEngine.Debug.LogError("Table parse error : 2 or more , in single statement");
				continue;
			}
			string source = array2[0].Trim();
			if (!keyFactory(source, out var value))
			{
				UnityEngine.Debug.LogError("Table parse error : Key parse failed");
				continue;
			}
			string source2 = array2[1].Trim();
			if (!valueFactory(source2, out var value2))
			{
				UnityEngine.Debug.LogError("Table parse error : value parse failed");
			}
			else
			{
				dictionary.Add(value, value2);
			}
		}
		return dictionary;
	}

	public static Dictionary<string, TValue> ParseStringTable<TValue>(string input, Factory<TValue> valueFactory)
	{
		if (string.IsNullOrEmpty(input))
		{
			return new Dictionary<string, TValue>();
		}
		char[] separator = new char[1] { ';' };
		char[] separator2 = new char[2] { '=', ' ' };
		Dictionary<string, TValue> dictionary = new Dictionary<string, TValue>();
		string[] array = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			string[] array2 = text.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length != 2)
			{
				UnityEngine.Debug.LogError("Table parse error : 2 or more , in single statement");
				continue;
			}
			string key = array2[0].Trim();
			string source = array2[1].Trim();
			if (!valueFactory(source, out var value))
			{
				UnityEngine.Debug.LogError("Table parse error : value parse failed");
			}
			else
			{
				dictionary.Add(key, value);
			}
		}
		return dictionary;
	}

	public static Sprite GetLeaderBoardPointIcon(LeaderBoardType boardType, int criteria = 0, LEAGUE_TIER_ICON tier = LEAGUE_TIER_ICON.LTI_NONE)
	{
		switch (boardType)
		{
		case LeaderBoardType.BT_ACHIEVE:
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_ITEM_MISC_SMALL", "AB_INVEN_ICON_ITEM_MISC_RESOURCE_ACHIEVE_POINT");
		case LeaderBoardType.BT_SHADOW:
		case LeaderBoardType.BT_TIMEATTACK:
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_SPRITE", "AB_UI_NKM_UI_LEADER_BOARD_TIME_ICON");
		case LeaderBoardType.BT_FIERCE:
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_SPRITE", "AB_UI_NKM_UI_LEADER_BOARD_FIERCE_BATTLE_SUPPORT_REWARD");
		case LeaderBoardType.BT_DEFENCE:
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_SPRITE", "AB_UI_NKM_UI_LEADER_BOARD_ICON_DEF");
		case LeaderBoardType.BT_PVP_RANK:
			return GetTierIcon(tier);
		case LeaderBoardType.BT_GUILD:
			if (criteria == 1)
			{
				return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_SPRITE", "AB_UI_NKM_UI_LEADER_BOARD_MENU_ICON_EXP");
			}
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_SPRITE", "AB_UI_NKM_UI_LEADER_BOARD_MENU_ICON_SHADOW");
		default:
			return null;
		}
	}

	public static Sprite GetRankIcon(int rank)
	{
		return rank switch
		{
			1 => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_SPRITE", "Rank_01"), 
			2 => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_SPRITE", "Rank_02"), 
			3 => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_SPRITE", "Rank_03"), 
			_ => null, 
		};
	}

	public static Sprite GetTierIcon(LEAGUE_TIER_ICON tier)
	{
		string text = "";
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_GAUNTLET_TIER", tier switch
		{
			LEAGUE_TIER_ICON.LTI_BRONZE => "AB_UI_NKM_UI_COMMON_GAUNTLET_TIER_BRONZE_SIMPLE", 
			LEAGUE_TIER_ICON.LTI_SILVER => "AB_UI_NKM_UI_COMMON_GAUNTLET_TIER_SILVER_SIMPLE", 
			LEAGUE_TIER_ICON.LTI_GOLD => "AB_UI_NKM_UI_COMMON_GAUNTLET_TIER_GOLD_SIMPLE", 
			LEAGUE_TIER_ICON.LTI_PLATINUM => "AB_UI_NKM_UI_COMMON_GAUNTLET_TIER_PLATINUM_SIMPLE", 
			LEAGUE_TIER_ICON.LTI_DIAMOND => "AB_UI_NKM_UI_COMMON_GAUNTLET_TIER_DIAMOND_SIMPLE", 
			LEAGUE_TIER_ICON.LTI_MASTER => "AB_UI_NKM_UI_COMMON_GAUNTLET_TIER_MASTER_SIMPLE", 
			LEAGUE_TIER_ICON.LTI_CHALLENGER => "AB_UI_NKM_UI_COMMON_GAUNTLET_TIER_CHALLENGER_SIMPLE", 
			LEAGUE_TIER_ICON.LTI_GRANDMASTER => "AB_UI_NKM_UI_COMMON_GAUNTLET_TIER_GRANDMASTER_SIMPLE", 
			_ => "", 
		});
	}

	public static Sprite GetTierIconBig(LEAGUE_TIER_ICON tier)
	{
		string assetName = "";
		switch (tier)
		{
		case LEAGUE_TIER_ICON.LTI_BRONZE:
			assetName = "AB_UI_NKM_UI_GAUNTLET_TIER_BIG_BRONZE";
			break;
		case LEAGUE_TIER_ICON.LTI_SILVER:
			assetName = "AB_UI_NKM_UI_GAUNTLET_TIER_BIG_SILVER";
			break;
		case LEAGUE_TIER_ICON.LTI_GOLD:
			assetName = "AB_UI_NKM_UI_GAUNTLET_TIER_BIG_GOLD";
			break;
		case LEAGUE_TIER_ICON.LTI_PLATINUM:
			assetName = "AB_UI_NKM_UI_GAUNTLET_TIER_BIG_PLATINUM";
			break;
		case LEAGUE_TIER_ICON.LTI_DIAMOND:
			assetName = "AB_UI_NKM_UI_GAUNTLET_TIER_BIG_DIAMOND";
			break;
		case LEAGUE_TIER_ICON.LTI_MASTER:
			assetName = "AB_UI_NKM_UI_GAUNTLET_TIER_BIG_MASTER";
			break;
		case LEAGUE_TIER_ICON.LTI_GRANDMASTER:
			assetName = "AB_UI_NKM_UI_GAUNTLET_TIER_BIG_GRANDMASTER";
			break;
		case LEAGUE_TIER_ICON.LTI_CHALLENGER:
			assetName = "AB_UI_NKM_UI_GAUNTLET_TIER_BIG_CHALLENGER";
			break;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_gauntlet_texture", assetName);
	}

	public static int CalculateNormalizedIndex(int index, int maxCount)
	{
		if (index < 0)
		{
			index = maxCount + index % maxCount;
		}
		if (index >= maxCount)
		{
			index %= maxCount;
		}
		return index;
	}

	public static int FindPVPSeasonIDForRank(DateTime nowUTC)
	{
		int result = 0;
		foreach (KeyValuePair<int, NKMPvpRankSeasonTemplet> item in NKCPVPManager.dicPvpRankSeasonTemplet)
		{
			if (item.Value.CheckSeasonForRank(nowUTC))
			{
				result = item.Key;
				break;
			}
		}
		return result;
	}

	public static int FindPVPSeasonIDForAsync(DateTime nowUTC)
	{
		int result = 0;
		foreach (KeyValuePair<int, NKMPvpRankSeasonTemplet> item in NKCPVPManager.dicAsyncPvpSeasonTemplet)
		{
			if (item.Value.CheckSeasonForRank(nowUTC))
			{
				result = item.Key;
				break;
			}
		}
		return result;
	}

	public static int FindPVPSeasonIDForLeague(DateTime nowUTC)
	{
		int result = 0;
		foreach (NKMLeaguePvpRankSeasonTemplet value in NKMLeaguePvpRankSeasonTemplet.Values)
		{
			if (value.CheckSeasonForRank(nowUTC))
			{
				result = value.Key;
				break;
			}
		}
		return result;
	}

	public static int FindPVPSeasonIDForEvent()
	{
		int result = 0;
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet != null)
		{
			result = eventPvpSeasonTemplet.SeasonId;
		}
		return result;
	}

	public static int GetScoreBySeason(int currentSeasonID, int lastestPlaySeasonID, int score, NKM_GAME_TYPE gameType)
	{
		if (currentSeasonID != lastestPlaySeasonID)
		{
			score = NKCPVPManager.GetResetScore(currentSeasonID, score, gameType);
		}
		return score;
	}

	public static int CalcAddScore(LEAGUE_TYPE leagueType, int myScore, int targetScore)
	{
		int num = 25;
		switch (leagueType)
		{
		case LEAGUE_TYPE.LEAGUE_TYPE_START:
		case LEAGUE_TYPE.LEAGUE_TYPE_NEWBIE:
			return num;
		case LEAGUE_TYPE.LEAGUE_TYPE_NORMAL:
		{
			int num2 = num + (targetScore - myScore) / 12;
			if (num2 < 5)
			{
				num2 = 5;
			}
			else if (num2 > 45)
			{
				num2 = 45;
			}
			return num2;
		}
		default:
			return 0;
		}
	}

	public static bool CheckInvalidStringName(string text)
	{
		if (text == null)
		{
			return false;
		}
		for (int i = 0; i < text.Length; i++)
		{
			if ((NKCStringTable.GetNationalCode() != NKM_NATIONAL_CODE.NNC_KOREA || text[i] < '가' || text[i] > '힣') && (text[i] < '0' || text[i] > '9') && (text[i] < 'A' || text[i] > 'Z') && (text[i] < 'a' || text[i] > 'z'))
			{
				_ = text[i];
				_ = 32;
				return false;
			}
		}
		return true;
	}

	public static T GetNextEnum<T>(T enumValue) where T : Enum
	{
		T[] array = (T[])Enum.GetValues(typeof(T));
		int num = Array.IndexOf(array, enumValue) + 1;
		if (array.Length != num)
		{
			return array[num];
		}
		return array[0];
	}

	public static T GetPrevEnum<T>(T enumValue) where T : Enum
	{
		T[] array = (T[])Enum.GetValues(typeof(T));
		int num = Array.IndexOf(array, enumValue) - 1;
		if (num >= 0)
		{
			return array[num];
		}
		return array[array.Length - 1];
	}

	public static void ClearGauntletCacheData(NKCScenManager cNKCScenManager)
	{
		if (!(cNKCScenManager == null))
		{
			cNKCScenManager.Get_NKC_SCEN_GAUNTLET_INTRO()?.ClearCacheData();
			cNKCScenManager.Get_NKC_SCEN_GAUNTLET_LOBBY()?.ClearCacheData();
			cNKCScenManager.Get_NKC_SCEN_GAUNTLET_MATCH()?.ClearCacheData();
			if (NKCUIManager.NKCUIGauntletResult != null)
			{
				NKCUIManager.NKCUIGauntletResult.CloseInstance();
				NKCUIManager.NKCUIGauntletResult = null;
			}
		}
	}

	public static int GetFinalPVPScore(PvpState cNKMPVPData, NKM_GAME_TYPE gameType)
	{
		if (cNKMPVPData == null)
		{
			return 0;
		}
		int num = NKCPVPManager.FindPvPSeasonID(gameType, NKCSynchronizedTime.GetServerUTCTime());
		if (cNKMPVPData.SeasonID != num)
		{
			return NKCPVPManager.GetResetScore(cNKMPVPData.SeasonID, cNKMPVPData.Score, gameType);
		}
		return cNKMPVPData.Score;
	}

	public static bool IsPVPDemotionAlert(NKMPvpRankTemplet cNKMPvpRankTempletByScore, NKMPvpRankTemplet cNKMPvpRankTempletByTier, int pvpScore)
	{
		if (cNKMPvpRankTempletByScore == null || cNKMPvpRankTempletByTier == null)
		{
			return false;
		}
		if (cNKMPvpRankTempletByScore.LeagueTier < cNKMPvpRankTempletByTier.LeagueTier && NKMPvpCommonConst.Instance.DEMOTION_SCORE <= cNKMPvpRankTempletByTier.LeaguePointReq - pvpScore)
		{
			return true;
		}
		return false;
	}

	public static bool IsPVPDemotionAlert(NKMLeaguePvpRankTemplet cNKMPvpRankTempletByScore, NKMLeaguePvpRankTemplet cNKMPvpRankTempletByTier, int pvpScore)
	{
		if (cNKMPvpRankTempletByScore == null || cNKMPvpRankTempletByTier == null)
		{
			return false;
		}
		if (cNKMPvpRankTempletByScore.LeagueTier < cNKMPvpRankTempletByTier.LeagueTier && NKMPvpCommonConst.Instance.LEAGUE_PVP_DEMOTION_SCORE <= cNKMPvpRankTempletByTier.LeaguePointReq - pvpScore)
		{
			return true;
		}
		return false;
	}

	public static bool IsPVPDemotionAlert(NKM_GAME_TYPE gameType, PvpState cNKMPVPData)
	{
		if (cNKMPVPData == null)
		{
			return false;
		}
		int finalPVPScore = GetFinalPVPScore(cNKMPVPData, gameType);
		int num = NKCPVPManager.FindPvPSeasonID(gameType, NKCSynchronizedTime.GetServerUTCTime());
		if (gameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE || gameType == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(num);
			if (nKMLeaguePvpRankSeasonTemplet != null)
			{
				NKMLeaguePvpRankTemplet byScore = nKMLeaguePvpRankSeasonTemplet.RankGroup.GetByScore(cNKMPVPData.Score);
				NKMLeaguePvpRankTemplet byTier = nKMLeaguePvpRankSeasonTemplet.RankGroup.GetByTier(cNKMPVPData.LeagueTierID);
				if (IsPVPDemotionAlert(byScore, byTier, finalPVPScore))
				{
					return true;
				}
			}
			return false;
		}
		NKMPvpRankTemplet pvpRankTempletByTier = NKCPVPManager.GetPvpRankTempletByTier(num, cNKMPVPData.LeagueTierID);
		if (IsPVPDemotionAlert(NKCPVPManager.GetPvpRankTempletByScore(num, finalPVPScore), pvpRankTempletByTier, finalPVPScore))
		{
			return true;
		}
		return false;
	}

	public static void OpenPurchasePopupNotInShop(int itemID, int shopID, int priceItemID)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID != null)
		{
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(shopID);
			if (shopItemTemplet != null)
			{
				NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
				if (myUserData == null)
				{
					return;
				}
				if (shopItemTemplet.m_QuantityLimit > 0 && shopItemTemplet.m_QuantityLimit <= myUserData.m_ShopData.GetPurchasedCount(shopItemTemplet))
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_SHOP_FAIL_PURCHASE_COUNT.ToString()));
					return;
				}
				int realPrice = myUserData.m_ShopData.GetRealPrice(shopItemTemplet);
				NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_PURCHASE_POPUP_TITLE, string.Format(NKCUtilString.GET_STRING_PURCHASE_POPUP_DESC, itemMiscTempletByID.GetItemName()), priceItemID, realPrice, delegate
				{
					NKCPacketSender.Send_NKMPacket_SHOP_FIX_SHOP_BUY_REQ(shopID, 1);
				});
			}
			else
			{
				Log.Warn($"상점ID로 ShopItemTemplet을 찾을 수 없음. 상점ID : {shopID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCUtil.cs", 3302);
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_INVALID_SHOP_ID));
			}
		}
		else
		{
			Log.Warn($"비품 아이템을 찾을 수 없습니다. 비품 아이템 아이디 : {itemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCUtil.cs", 3308);
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_INVALID_MISC_ITEM_ID));
		}
	}

	public static float HalfToFloat(ushort us)
	{
		if (us == 0)
		{
			return 0f;
		}
		object obj = new Half
		{
			value = us
		};
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		return ((IConvertible)obj).ToSingle(invariantCulture);
	}

	public static void MakeUILoyaltyValue(int realBefore, int realGain, out int textBefore, out int textAfter, out int textGain)
	{
		textAfter = Math.Min((realBefore + realGain) / 100, 100);
		textBefore = realBefore / 100;
		textGain = textAfter - textBefore;
	}

	public static void SetBindFunction(NKCUIComStateButton btn, UnityAction bindFunc = null)
	{
		if (!(btn == null))
		{
			btn.PointerClick.RemoveAllListeners();
			if (bindFunc != null)
			{
				btn.PointerClick.AddListener(bindFunc);
			}
		}
	}

	public static void SetBindFunction(NKCUIComButton btn, UnityAction bindFunc = null)
	{
		if (!(btn == null))
		{
			btn.PointerClick.RemoveAllListeners();
			if (bindFunc != null)
			{
				btn.PointerClick.AddListener(bindFunc);
			}
		}
	}

	public static void SetHotkey(NKCUIComToggle tgl, HotkeyEventType hotkeyEvent, NKCUIBase uiBase = null, bool bUpDownEvent = false)
	{
		if (!(tgl == null))
		{
			tgl.SetHotkey(hotkeyEvent, uiBase, bUpDownEvent);
			tgl.m_bSelectByClick = true;
		}
	}

	public static void SetHotkey(NKCUIComStateButtonBase btn, HotkeyEventType hotkeyEvent, NKCUIBase uiBase = null, bool bUpDownEvent = false)
	{
		if (!(btn == null))
		{
			btn.SetHotkey(hotkeyEvent, uiBase, bUpDownEvent);
		}
	}

	public static void SetHotkey(NKCUIComButton btn, HotkeyEventType hotkeyEvent)
	{
		if (!(btn == null))
		{
			btn.m_HotkeyEventType = hotkeyEvent;
		}
	}

	public static bool CalculateContentRectSize(LoopScrollRect loopScrollRect, GridLayoutGroup grid_layout_group, int minColumn, Vector2 cellSize, Vector2 spacing, bool recalcCellSize = false)
	{
		loopScrollRect.ResetContentSpacing();
		RectTransform component = loopScrollRect.GetComponent<RectTransform>();
		RectTransform viewRect = loopScrollRect.viewRect;
		float num = cellSize.x * (float)minColumn + spacing.x * (float)(minColumn - 1);
		if (viewRect.GetWidth() < num)
		{
			UnityEngine.Debug.Log($"Content Rect Size ... viewRect : {viewRect.GetWidth()}, minWidth : {num}");
			float num2 = (viewRect.GetWidth() - spacing.x * (float)(minColumn - 1)) / (float)minColumn;
			float num3 = num2 / cellSize.x;
			grid_layout_group.cellSize = new Vector2(num2, cellSize.y * num3);
		}
		else
		{
			grid_layout_group.cellSize = cellSize;
		}
		grid_layout_group.spacing = spacing;
		int num4 = (int)((viewRect.GetWidth() + spacing.x) / (cellSize.x + spacing.x));
		if (recalcCellSize)
		{
			cellSize.x = (viewRect.GetWidth() - (float)(num4 - 1) * spacing.x) / (float)num4;
			grid_layout_group.cellSize = cellSize;
		}
		bool result = loopScrollRect.ContentConstraintCount != num4;
		grid_layout_group.constraintCount = num4;
		loopScrollRect.ContentConstraintCount = num4;
		float num5 = cellSize.x * (float)num4 + spacing.x * (float)(num4 - 1);
		float num6 = (component.GetWidth() - num5) / 2f;
		grid_layout_group.padding.left = (int)num6;
		grid_layout_group.padding.right = (int)num6;
		UnityEngine.Debug.Log($"CellSize : {grid_layout_group.cellSize}, rectContentWidth : {viewRect.GetWidth()}, scrollRectWidth : {component.GetWidth()}, padding : {grid_layout_group.padding.left}");
		return result;
	}

	public static bool CalculateContentRectSizeHorizontal(LoopScrollRect loopScrollRect, GridLayoutGroup grid_layout_group, int minRow, Vector2 cellSize, Vector2 spacing, bool recalcCellSize = false)
	{
		loopScrollRect.ResetContentSpacing();
		RectTransform component = loopScrollRect.GetComponent<RectTransform>();
		RectTransform viewRect = loopScrollRect.viewRect;
		float num = cellSize.y * (float)minRow + spacing.y * (float)(minRow - 1);
		if (viewRect.GetHeight() < num)
		{
			UnityEngine.Debug.Log($"Content Rect Size ... viewRect : {viewRect.GetHeight()}, minHeight : {num}");
			float num2 = (viewRect.GetHeight() - spacing.y * (float)(minRow - 1)) / (float)minRow;
			float num3 = num2 / cellSize.y;
			grid_layout_group.cellSize = new Vector2(cellSize.x * num3, num2);
		}
		else
		{
			grid_layout_group.cellSize = cellSize;
		}
		grid_layout_group.spacing = spacing;
		int num4 = (int)((viewRect.GetHeight() + spacing.y) / (cellSize.y + spacing.y));
		if (recalcCellSize)
		{
			cellSize.y = (viewRect.GetHeight() - (float)(num4 - 1) * spacing.y) / (float)num4;
			grid_layout_group.cellSize = cellSize;
		}
		bool result = loopScrollRect.ContentConstraintCount != num4;
		grid_layout_group.constraintCount = num4;
		loopScrollRect.ContentConstraintCount = num4;
		float num5 = cellSize.y * (float)num4 + spacing.y * (float)(num4 - 1);
		float num6 = (component.GetHeight() - num5) / 2f;
		grid_layout_group.padding.left = (int)num6;
		grid_layout_group.padding.right = (int)num6;
		UnityEngine.Debug.Log($"CellSize : {grid_layout_group.cellSize}, rectContentHeight : {viewRect.GetHeight()}, scrollRectHeight : {component.GetHeight()}, padding : {grid_layout_group.padding.left}");
		return result;
	}

	public static string TextSplitLine(string fullText, Text cUIText, float fForceRectWidth = 0f)
	{
		RectTransform component = cUIText.gameObject.GetComponent<RectTransform>();
		if (fForceRectWidth == 0f)
		{
			fForceRectWidth = component.rect.width;
		}
		int num = 0;
		bool bExistEmpty = true;
		for (int i = 0; i < fullText.Length; i++)
		{
			if (fullText[i] == ' ')
			{
				num++;
			}
		}
		if (num < 2)
		{
			bExistEmpty = false;
		}
		newFullTextBuilder.Clear();
		subTextBuilder.Clear();
		foreach (char c in fullText)
		{
			subTextBuilder.Append(c);
			if (c == '\n')
			{
				TextSplitLine(subTextBuilder, cUIText, fForceRectWidth, newFullTextBuilder, bExistEmpty);
				subTextBuilder.Clear();
			}
		}
		TextSplitLine(subTextBuilder, cUIText, fForceRectWidth, newFullTextBuilder, bExistEmpty);
		cUIText.text = "";
		return newFullTextBuilder.ToString();
	}

	public static string TextSplitLine(string fullText, TextMeshProUGUI cUIText, float fForceRectWidth = 0f)
	{
		RectTransform component = cUIText.gameObject.GetComponent<RectTransform>();
		if (fForceRectWidth == 0f)
		{
			fForceRectWidth = component.rect.width;
		}
		int num = 0;
		bool bExistEmpty = true;
		for (int i = 0; i < fullText.Length; i++)
		{
			if (fullText[i] == ' ')
			{
				num++;
			}
		}
		if (num < 2)
		{
			bExistEmpty = false;
		}
		newFullTextBuilder.Clear();
		subTextBuilder.Clear();
		foreach (char c in fullText)
		{
			subTextBuilder.Append(c);
			if (c == '\n')
			{
				TextSplitLine(subTextBuilder, cUIText, fForceRectWidth, newFullTextBuilder, bExistEmpty);
				subTextBuilder.Clear();
			}
		}
		TextSplitLine(subTextBuilder, cUIText, fForceRectWidth, newFullTextBuilder, bExistEmpty);
		cUIText.text = "";
		return newFullTextBuilder.ToString();
	}

	private static void RemoveTextTag(StringBuilder destTextBuilder, StringBuilder srcTextBuilder)
	{
		destTextBuilder.Remove(0, destTextBuilder.Length);
		bool flag = false;
		for (int i = 0; i < srcTextBuilder.Length; i++)
		{
			char c = srcTextBuilder[i];
			if (c == '<')
			{
				flag = true;
				continue;
			}
			if (i > 0)
			{
				c = srcTextBuilder[i - 1];
			}
			if (c == '>')
			{
				flag = false;
			}
			if (!flag)
			{
				destTextBuilder.Append(srcTextBuilder[i]);
			}
		}
	}

	private static bool TextSplitLine(StringBuilder subTextBuilder, Text cUIText, float fForceRectWidth, StringBuilder newFullTextBuilder, bool bExistEmpty)
	{
		string text = subTextBuilder.ToString();
		RemoveTextTag(tempTextBuilder, subTextBuilder);
		cUIText.text = tempTextBuilder.ToString();
		bool flag = false;
		if (cUIText.preferredWidth >= fForceRectWidth - (float)cUIText.fontSize * 1.1f && (double)cUIText.preferredWidth < (double)fForceRectWidth * 1.5)
		{
			int num = 0;
			bool flag2 = false;
			for (int i = 0; i < text.Length / 2; i++)
			{
				char num2 = text[i];
				if (num2 == '<')
				{
					flag2 = true;
				}
				if (num2 == '>')
				{
					flag2 = false;
				}
				if (flag2)
				{
					num++;
				}
			}
			bool flag3 = false;
			int num3 = (int)((double)num * 0.8) + text.Length / 2;
			for (int j = 0; j < num3; j++)
			{
				char num4 = text[j];
				if (j > 0 && text[j - 1] == '>')
				{
					flag3 = false;
				}
				if (num4 == '<')
				{
					flag3 = true;
				}
			}
			for (int k = num3; k < text.Length; k++)
			{
				char c = text[k];
				if (k > 0 && text[k - 1] == '>')
				{
					flag3 = false;
				}
				if (c == '<')
				{
					flag3 = true;
				}
				if (flag3)
				{
					continue;
				}
				bool flag4 = false;
				if (bExistEmpty)
				{
					if (c == ' ')
					{
						flag4 = true;
					}
				}
				else
				{
					flag4 = true;
				}
				if (flag4)
				{
					string text2 = text.Substring(0, k);
					cUIText.text = text2;
					if (!(cUIText.preferredWidth >= fForceRectWidth - (float)cUIText.fontSize * 1.1f))
					{
						int num5 = k;
						if (IsSpacing(text, k) && num5 + 1 < text.Length)
						{
							num5++;
						}
						text = text.Insert(num5, "\n");
						newFullTextBuilder.Append(text);
						text = "";
						flag = true;
						break;
					}
					bool flag5 = false;
					for (k = 0; k < (int)((double)num * 0.7) + text.Length / 2; k++)
					{
						c = text[k];
						if (k > 0 && text[k - 1] == '>')
						{
							flag5 = false;
						}
						if (c == '<')
						{
							flag5 = true;
						}
					}
					for (k = (int)((double)num * 0.7) + text.Length / 2; k < text.Length; k++)
					{
						c = text[k];
						if (k > 0 && text[k - 1] == '>')
						{
							flag5 = false;
						}
						if (c == '<')
						{
							flag5 = true;
						}
						if (flag5)
						{
							continue;
						}
						bool flag6 = false;
						if (bExistEmpty)
						{
							if (c == ' ')
							{
								flag6 = true;
							}
						}
						else
						{
							flag6 = true;
						}
						if (flag6)
						{
							int num6 = k;
							if (IsSpacing(text, k) && num6 + 1 < text.Length)
							{
								num6++;
							}
							text = text.Insert(num6, "\n");
							newFullTextBuilder.Append(text);
							text = "";
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		if (!flag)
		{
			newFullTextBuilder.Append(text);
		}
		return flag;
	}

	private static bool TextSplitLine(StringBuilder subTextBuilder, TextMeshProUGUI cUIText, float fForceRectWidth, StringBuilder newFullTextBuilder, bool bExistEmpty)
	{
		string text = subTextBuilder.ToString();
		RemoveTextTag(tempTextBuilder, subTextBuilder);
		cUIText.text = tempTextBuilder.ToString();
		bool flag = false;
		if (cUIText.preferredWidth >= fForceRectWidth - cUIText.fontSize * 1.1f && (double)cUIText.preferredWidth < (double)fForceRectWidth * 1.5)
		{
			int num = 0;
			bool flag2 = false;
			for (int i = 0; i < text.Length / 2; i++)
			{
				char num2 = text[i];
				if (num2 == '<')
				{
					flag2 = true;
				}
				if (num2 == '>')
				{
					flag2 = false;
				}
				if (flag2)
				{
					num++;
				}
			}
			bool flag3 = false;
			int num3 = (int)((double)num * 0.8) + text.Length / 2;
			for (int j = 0; j < num3; j++)
			{
				char num4 = text[j];
				if (j > 0 && text[j - 1] == '>')
				{
					flag3 = false;
				}
				if (num4 == '<')
				{
					flag3 = true;
				}
			}
			for (int k = num3; k < text.Length; k++)
			{
				char c = text[k];
				if (k > 0 && text[k - 1] == '>')
				{
					flag3 = false;
				}
				if (c == '<')
				{
					flag3 = true;
				}
				if (flag3)
				{
					continue;
				}
				bool flag4 = false;
				if (bExistEmpty)
				{
					if (c == ' ')
					{
						flag4 = true;
					}
				}
				else
				{
					flag4 = true;
				}
				if (flag4)
				{
					string text2 = text.Substring(0, k);
					cUIText.text = text2;
					if (!(cUIText.preferredWidth >= fForceRectWidth - cUIText.fontSize * 1.1f))
					{
						int num5 = k;
						if (IsSpacing(text, k) && num5 + 1 < text.Length)
						{
							num5++;
						}
						text = text.Insert(num5, "\n");
						newFullTextBuilder.Append(text);
						text = "";
						flag = true;
						break;
					}
					bool flag5 = false;
					for (k = 0; k < (int)((double)num * 0.7) + text.Length / 2; k++)
					{
						c = text[k];
						if (k > 0 && text[k - 1] == '>')
						{
							flag5 = false;
						}
						if (c == '<')
						{
							flag5 = true;
						}
					}
					for (k = (int)((double)num * 0.7) + text.Length / 2; k < text.Length; k++)
					{
						c = text[k];
						if (k > 0 && text[k - 1] == '>')
						{
							flag5 = false;
						}
						if (c == '<')
						{
							flag5 = true;
						}
						if (flag5)
						{
							continue;
						}
						bool flag6 = false;
						if (bExistEmpty)
						{
							if (c == ' ')
							{
								flag6 = true;
							}
						}
						else
						{
							flag6 = true;
						}
						if (flag6)
						{
							int num6 = k;
							if (IsSpacing(text, k) && num6 + 1 < text.Length)
							{
								num6++;
							}
							text = text.Insert(num6, "\n");
							newFullTextBuilder.Append(text);
							text = "";
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		if (!flag)
		{
			newFullTextBuilder.Append(text);
		}
		return flag;
	}

	private static bool IsSpacing(string text, int charIndex)
	{
		if (charIndex < text.Length)
		{
			return text[charIndex] == ' ';
		}
		return false;
	}

	public static NKM_ERROR_CODE CheckCommonStartCond(NKMUserData cNKMUserData)
	{
		if (cNKMUserData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_USER_DATA_NULL;
		}
		if (!cNKMUserData.m_ArmyData.CanGetMoreUnit(1))
		{
			return NKM_ERROR_CODE.NEC_FAIL_ARMY_FULL;
		}
		if (!cNKMUserData.m_ArmyData.CanGetMoreShip(0))
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_FULL;
		}
		if (!cNKMUserData.m_InventoryData.CanGetMoreEquipItem(1))
		{
			return NKM_ERROR_CODE.NEC_FAIL_EQUIP_ITEM_FULL;
		}
		if (!cNKMUserData.m_ArmyData.CanGetMoreOperator(1))
		{
			return NKM_ERROR_CODE.NEC_FAIL_OPERATOR_FULL;
		}
		if (!cNKMUserData.m_ArmyData.CanGetMoreTrophy(1))
		{
			return NKM_ERROR_CODE.NEC_FAIL_TROPHY_FULL;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static void OnExpandInventoryPopup(NKM_ERROR_CODE error_code)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKM_INVENTORY_EXPAND_TYPE inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_NONE;
		string title = "";
		NKCPopupInventoryAdd.SliderInfo sliderInfo = default(NKCPopupInventoryAdd.SliderInfo);
		int requiredItemCount = 0;
		switch (error_code)
		{
		case NKM_ERROR_CODE.NEC_FAIL_ARMY_FULL:
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT;
			title = NKCUtilString.GET_STRING_INVENTORY_UNIT;
			sliderInfo.increaseCount = 5;
			sliderInfo.maxCount = 1100;
			sliderInfo.currentCount = myUserData.m_ArmyData.m_MaxUnitCount;
			requiredItemCount = 100;
			break;
		case NKM_ERROR_CODE.NEC_FAIL_SHIP_FULL:
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP;
			title = NKCUtilString.GET_STRING_INVENTORY_SHIP;
			sliderInfo.increaseCount = 1;
			sliderInfo.maxCount = 60;
			sliderInfo.currentCount = myUserData.m_ArmyData.m_MaxShipCount;
			requiredItemCount = 100;
			break;
		case NKM_ERROR_CODE.NEC_FAIL_EQUIP_ITEM_FULL:
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP;
			title = NKCUtilString.GET_STRING_INVENTORY_EQUIP;
			sliderInfo.increaseCount = 5;
			sliderInfo.maxCount = 2000;
			sliderInfo.currentCount = myUserData.m_InventoryData.m_MaxItemEqipCount;
			requiredItemCount = 50;
			break;
		case NKM_ERROR_CODE.NEC_FAIL_OPERATOR_FULL:
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_OPERATOR;
			title = NKCUtilString.GET_STRING_INVEITORY_OPERATOR_TITLE;
			sliderInfo.increaseCount = 5;
			sliderInfo.maxCount = 500;
			sliderInfo.currentCount = myUserData.m_ArmyData.m_MaxOperatorCount;
			requiredItemCount = 100;
			break;
		case NKM_ERROR_CODE.NEC_FAIL_TROPHY_FULL:
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_TROPHY;
			title = NKCUtilString.GET_STRING_TROPHY_UNIT;
			sliderInfo.increaseCount = 10;
			sliderInfo.maxCount = 2000;
			sliderInfo.currentCount = myUserData.m_ArmyData.m_MaxTrophyCount;
			requiredItemCount = 50;
			break;
		}
		sliderInfo.inventoryType = inventoryType;
		string expandDesc = NKCUtilString.GetExpandDesc(inventoryType, isFullMsg: true);
		int count = 1;
		if (!NKMInventoryManager.IsValidExpandType(inventoryType))
		{
			return;
		}
		int resultCount;
		bool flag = !NKCAdManager.IsAdRewardInventory(inventoryType) || !NKMInventoryManager.CanExpandInventoryByAd(inventoryType, myUserData, count, out resultCount);
		if (!NKMInventoryManager.CanExpandInventory(inventoryType, myUserData, count, out resultCount) && flag)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCPacketHandlers.GetErrorMessage(error_code));
			return;
		}
		NKCPopupInventoryAdd.Instance.Open(title, expandDesc, sliderInfo, requiredItemCount, 101, delegate(int value)
		{
			NKCPacketSender.Send_NKMPacket_INVENTORY_EXPAND_REQ(inventoryType, value);
		});
	}

	public static bool IsNullObject<T>(T obj, string msg = "")
	{
		if (obj == null)
		{
			StackTrace stackTrace = new StackTrace();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Null Object Exception\ntarget Type : " + typeof(T).FullName + ", ");
			stringBuilder.Append("\nException message : " + msg).Append(" ");
			stringBuilder.Append("\nSTACK TRACE - START").Append(" ");
			for (int i = 1; i < stackTrace.FrameCount; i++)
			{
				MethodBase method = stackTrace.GetFrame(i).GetMethod();
				if (null != method.ReflectedType)
				{
					stringBuilder.AppendFormat("\n{0}.{1}", method.ReflectedType.ToString(), method.Name);
				}
			}
			stringBuilder.Append("\nSTACK TRACE - END");
			UnityEngine.Debug.LogWarning(stringBuilder.ToString());
			return true;
		}
		return false;
	}

	public static bool CheckPossibleShowBan(NKCUIDeckViewer.DeckViewerMode eDeckViewerMode)
	{
		switch (eDeckViewerMode)
		{
		case NKCUIDeckViewer.DeckViewerMode.PrivatePvPReady:
			return NKCPrivatePVPRoomMgr.PrivatePVPLobbyBanUpState;
		case NKCUIDeckViewer.DeckViewerMode.UnlimitedDeck:
			return false;
		default:
		{
			bool flag = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_NEW_MODE) && eDeckViewerMode != NKCUIDeckViewer.DeckViewerMode.AsyncPvpDefenseDeck;
			if (!NKCUIDeckViewer.IsPVPSyncMode(eDeckViewerMode) && flag)
			{
				return false;
			}
			if (NKCScenManager.GetScenManager() == null)
			{
				return false;
			}
			if (NKCScenManager.GetScenManager().GetMyUserData() == null)
			{
				return false;
			}
			PvpState pvpState = null;
			pvpState = ((eDeckViewerMode != NKCUIDeckViewer.DeckViewerMode.AsyncPvpDefenseDeck || !NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_NEW_MODE)) ? NKCScenManager.GetScenManager().GetMyUserData().m_PvpData : NKCScenManager.GetScenManager().GetMyUserData().m_AsyncData);
			if (pvpState == null)
			{
				return false;
			}
			if (!pvpState.IsBanPossibleScore())
			{
				return false;
			}
			return true;
		}
		}
	}

	public static bool CheckPossibleShowUpUnit(NKCUIDeckViewer.DeckViewerMode eDeckViewerMode)
	{
		switch (eDeckViewerMode)
		{
		case NKCUIDeckViewer.DeckViewerMode.PrivatePvPReady:
			return NKCPrivatePVPRoomMgr.PrivatePVPLobbyBanUpState;
		case NKCUIDeckViewer.DeckViewerMode.UnlimitedDeck:
			return false;
		default:
		{
			bool flag = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_NEW_MODE) && eDeckViewerMode != NKCUIDeckViewer.DeckViewerMode.AsyncPvpDefenseDeck;
			if (!NKCUIDeckViewer.IsPVPSyncMode(eDeckViewerMode) && flag)
			{
				return false;
			}
			return true;
		}
		}
	}

	public static NKMUnitData MakeDummyUnit(int unitID, int unitLv = 1, short unitLimitBreakLv = 0, int tacticLv = 0, int reactorLv = 0)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase == null)
		{
			UnityEngine.Debug.LogError($"NKMUnitTempletBase : 데이터를 찾을 수 없습니다 - 유닛ID: {unitID}");
			return null;
		}
		NKMUnitData nKMUnitData = new NKMUnitData();
		nKMUnitData.m_UnitID = unitID;
		nKMUnitData.m_UnitUID = unitID;
		nKMUnitData.m_UserUID = 0L;
		nKMUnitData.m_UnitLevel = unitLv;
		nKMUnitData.m_iUnitLevelEXP = 0;
		nKMUnitData.m_LimitBreakLevel = unitLimitBreakLv;
		nKMUnitData.tacticLevel = tacticLv;
		nKMUnitData.reactorLevel = reactorLv;
		bool flag = unitLv >= 100;
		if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			int skillCount = unitTempletBase.GetSkillCount();
			for (int i = 0; i < skillCount; i++)
			{
				string skillStrID = unitTempletBase.GetSkillStrID(i);
				if (flag)
				{
					nKMUnitData.m_aUnitSkillLevel[i] = NKMUnitSkillManager.GetMaxSkillLevel(skillStrID);
				}
				else
				{
					nKMUnitData.m_aUnitSkillLevel[i] = 1;
				}
			}
		}
		for (int j = 0; j <= 5; j++)
		{
			if (flag)
			{
				nKMUnitData.m_listStatEXP[j] = NKMEnhanceManager.CalculateMaxEXP(nKMUnitData, (NKM_STAT_TYPE)j);
			}
			else
			{
				nKMUnitData.m_listStatEXP[j] = 0;
			}
		}
		nKMUnitData.m_bLock = false;
		return nKMUnitData;
	}

	public static void SetAwakenFX(Animator anim, NKMUnitTempletBase templetBase)
	{
		if (anim == null)
		{
			return;
		}
		anim.keepAnimatorControllerStateOnDisable = true;
		if (templetBase != null && templetBase.m_bAwaken)
		{
			switch (templetBase.m_NKM_UNIT_GRADE)
			{
			default:
				anim.SetInteger("Grade", 0);
				break;
			case NKM_UNIT_GRADE.NUG_SR:
				anim.SetInteger("Grade", 1);
				break;
			case NKM_UNIT_GRADE.NUG_SSR:
				anim.SetInteger("Grade", 2);
				break;
			}
		}
		else
		{
			anim.SetInteger("Grade", 0);
		}
	}

	public static float GetStatPercentage(NKM_STAT_TYPE eStatType, float number)
	{
		if (number == 0f)
		{
			return 0f;
		}
		float num = 0f;
		switch (eStatType)
		{
		case NKM_STAT_TYPE.NST_EVADE:
			num = NKMUnitStatManager.m_fConstEvade;
			break;
		case NKM_STAT_TYPE.NST_DEF:
			num = NKMUnitStatManager.m_fConstDef;
			break;
		case NKM_STAT_TYPE.NST_HIT:
			num = NKMUnitStatManager.m_fConstHit;
			break;
		case NKM_STAT_TYPE.NST_CRITICAL:
			return (number / NKMUnitStatManager.m_fConstCritical).Clamp(0f, 0.85f) * 100f;
		case NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_CONFLICT_G4:
		case NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_STABLE_G4:
		case NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_LIBERAL_G4:
			num = NKMUnitStatManager.m_fConstSourceAttack;
			return number / (num - number * number / (num + number)) * 100f;
		case NKM_STAT_TYPE.NST_DEFEND_VS_SOURCE_CONFLICT_G4:
		case NKM_STAT_TYPE.NST_DEFEND_VS_SOURCE_STABLE_G4:
		case NKM_STAT_TYPE.NST_DEFEND_VS_SOURCE_LIBERAL_G4:
			num = NKMUnitStatManager.m_fConstSourceDefend;
			break;
		}
		if (num != 0f)
		{
			return number / (number + num) * 100f;
		}
		return 0f;
	}

	public static Sprite GetGuildArtifactBgProbImage(GuildDungeonArtifactTemplet.ArtifactProbType imageName)
	{
		return imageName switch
		{
			GuildDungeonArtifactTemplet.ArtifactProbType.LOW => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_UNIT_SLOT_CARD_SPRITE", "NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_BG_R"), 
			GuildDungeonArtifactTemplet.ArtifactProbType.MIDDLE => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_UNIT_SLOT_CARD_SPRITE", "NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_BG_SR"), 
			GuildDungeonArtifactTemplet.ArtifactProbType.HIGH => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_UNIT_SLOT_CARD_SPRITE", "NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_BG_SSR"), 
			_ => null, 
		};
	}

	public static bool IsValidReward(NKM_REWARD_TYPE rewardType, int rewardValueID)
	{
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(rewardValueID);
			if (unitTempletBase == null || !unitTempletBase.CollectionEnableByTag)
			{
				return false;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MOLD:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(rewardValueID);
			if (itemMoldTempletByID == null || !itemMoldTempletByID.EnableByTag)
			{
				return false;
			}
			break;
		}
		}
		return true;
	}

	public static bool IsJPNPolicyRelatedItem(int itemId)
	{
		if (!NKCPublisherModule.InAppPurchase.IsJPNPaymentPolicy())
		{
			return false;
		}
		if (itemId != 101)
		{
			return itemId == 102;
		}
		return true;
	}

	public static AnimationClip GetAnimationClip(Animator animator, string name)
	{
		if (animator == null || animator.runtimeAnimatorController == null)
		{
			return null;
		}
		AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
		foreach (AnimationClip animationClip in animationClips)
		{
			if (animationClip.name == name)
			{
				return animationClip;
			}
		}
		return null;
	}

	public static void OpenRewardPopup(NKMArmyData armyData, NKMRewardData rewardData, NKMAdditionalReward additionalReward, string Title, string subTitle = "", NKCUIResult.OnClose onClose = null)
	{
		if (rewardData.UnitDataList.Count > 0 || rewardData.SkinIdList.Count > 0)
		{
			NKCUIResult.Instance.OpenRewardGain(armyData, rewardData, additionalReward, Title, subTitle, onClose);
		}
		else
		{
			NKCPopupMessageToastSimple.Instance.Open(rewardData, additionalReward, onClose);
		}
	}

	public static string GetPotentialSocketStatText(bool isPercentStat, float statValue, bool bShowDetail = false)
	{
		if (isPercentStat)
		{
			decimal num = new decimal(statValue);
			if (bShowDetail)
			{
				num = Math.Round(num * 10000m) / 10000m;
				return $"{num:P2}" ?? "";
			}
			num = Math.Round(num * 1000m) / 1000m;
			return $"{num:P1}" ?? "";
		}
		if (statValue > 0f)
		{
			return $"{statValue}" ?? "";
		}
		return "0";
	}

	public static string GetPotentialStatText(NKMEquipItemData cNKMEquipItemData, int potentialIndex = 0, bool bShowName = true, bool bShowDetail = false)
	{
		int num = 0;
		if (potentialIndex < cNKMEquipItemData.potentialOptions.Count && cNKMEquipItemData.potentialOptions[potentialIndex] != null)
		{
			float num2 = 0f;
			int num3 = cNKMEquipItemData.potentialOptions[potentialIndex].sockets.Length;
			for (int i = 0; i < num3; i++)
			{
				if (cNKMEquipItemData.potentialOptions[potentialIndex].sockets[i] != null)
				{
					num++;
					num2 += cNKMEquipItemData.potentialOptions[potentialIndex].sockets[i].statValue;
				}
			}
			if (num > 0)
			{
				bool isPercentStat = NKMUnitStatManager.IsPercentStat(cNKMEquipItemData.potentialOptions[potentialIndex].statType);
				string statShortName;
				if (NKCUtilString.IsNameReversedIfNegative(cNKMEquipItemData.potentialOptions[potentialIndex].statType) && num2 < 0f)
				{
					statShortName = NKCUtilString.GetStatShortName(cNKMEquipItemData.potentialOptions[potentialIndex].statType, bNegative: true);
					num2 = Mathf.Abs(num2);
				}
				else
				{
					statShortName = NKCUtilString.GetStatShortName(cNKMEquipItemData.potentialOptions[potentialIndex].statType);
				}
				if (bShowName)
				{
					return statShortName + " " + GetPotentialSocketStatText(isPercentStat, num2, bShowDetail);
				}
				return GetPotentialSocketStatText(isPercentStat, num2, bShowDetail) ?? "";
			}
		}
		return NKCUtilString.GET_STRING_EQUIP_POTENTIAL_OPEN_REQUIRED;
	}

	public static void SetRaidEventPoint(Image img, NKMRaidTemplet raidTemplet)
	{
		if (!(img == null) && raidTemplet != null)
		{
			SetImageSprite(img, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_RENEWAL_EVENT_THUMBNAIL", raidTemplet.EventPointColorName));
		}
	}

	public static void SetDiveEventPoint(Image img, bool bIsSpecial)
	{
		if (!(img == null))
		{
			if (bIsSpecial)
			{
				SetImageSprite(img, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_RENEWAL_EVENT_THUMBNAIL", "EVENT_THUMBNAIL_POINT_DIVE_SPECIAL"));
			}
			else
			{
				SetImageSprite(img, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_RENEWAL_EVENT_THUMBNAIL", "EVENT_THUMBNAIL_POINT_DIVE"));
			}
		}
	}

	public static bool IsUsingSuperUserFunction()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && nKMUserData.IsSuperUser())
		{
			return true;
		}
		return false;
	}

	public static bool IsUsingAdminUserFunction()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && nKMUserData.IsAdminUser())
		{
			return true;
		}
		return false;
	}

	public static NKMEventDeckData GetAutoPreparedDeckData(NKMDungeonEventDeckTemplet eventDeckTemplet, NKMUserData userData)
	{
		return new NKMEventDeckData();
	}

	public static bool IsUnitObtainedAtLeastOnce(NKM_UNIT_TYPE type, int UnitID = 0)
	{
		bool result = false;
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (type == NKM_UNIT_TYPE.NUT_SHIP)
		{
			int num = 1000;
			int num2 = UnitID / num;
			int num3 = UnitID % num;
			int num4 = 6;
			while (num4 > 0)
			{
				int unitID = num2 * num + num3;
				if (armyData.IsCollectedUnit(unitID))
				{
					result = true;
					break;
				}
				num4--;
				num2--;
			}
		}
		else
		{
			result = armyData.IsCollectedUnit(UnitID);
		}
		return result;
	}

	public static bool CanRaidCoop()
	{
		bool result = false;
		NKMWorldMapData worldmapData = NKCScenManager.CurrentUserData().m_WorldmapData;
		if (worldmapData != null)
		{
			foreach (KeyValuePair<int, NKMWorldMapCityData> item in worldmapData.worldMapCityDataMap)
			{
				NKMWorldMapCityData value = item.Value;
				if (value.worldMapEventGroup == null || value.worldMapEventGroup.worldmapEventID == 0)
				{
					continue;
				}
				NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(value.worldMapEventGroup.worldmapEventID);
				if (nKMWorldMapEventTemplet != null && nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
				{
					NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(value.worldMapEventGroup.eventUid);
					DateTime finishTimeUTC = new DateTime(nKMRaidDetailData.expireDate);
					NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
					if (nKMRaidDetailData != null && !nKMRaidDetailData.isCoop && nKMRaidDetailData.curHP > 0f && !NKCSynchronizedTime.IsFinished(finishTimeUTC) && nowSeasonTemplet != null && nKMRaidDetailData.seasonID == nowSeasonTemplet.RaidSeasonId)
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	public static void SetStringKey(string key, string value)
	{
		if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(key))
		{
			Log.Error("key/value must exist", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCUtil.cs", 4507);
			return;
		}
		PlayerPrefs.SetString($"{key}_{NKCScenManager.CurrentUserData().m_UserUID}", value);
		PlayerPrefs.Save();
	}

	public static string GetStringKey(string key, string defaultValue = "")
	{
		return PlayerPrefs.GetString($"{key}_{NKCScenManager.CurrentUserData().m_UserUID}", defaultValue);
	}
}
