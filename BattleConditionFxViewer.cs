using System;
using System.IO;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;

public class BattleConditionFxViewer : MonoBehaviour
{
	public int Index;

	public BattleMapList BML;

	public GameObject MapText;

	public GameObject HitBoxText;

	public bool ShowHitBox;

	[Space]
	public Transform MapRoot;

	public GameObject UIRoot;

	public Transform UnitRoot;

	[Space]
	public Camera MainCam;

	public GameObject RaidMap;

	[Space]
	public float FadeInTime = 2f;

	public float FadeOutTime = 2f;

	[Space]
	public int NormalViewSize = 500;

	public int RaidViewSize = 750;

	[Space]
	public Vector3 NormalViewUnitPos;

	public Vector3 RaidViewUnitPos;

	private GameObject currentMap;

	private TextMeshProUGUI HitBoxTmp;

	private TextMeshProUGUI MapTmp;

	private DrawHitBox[] hixBoxes;

	private bool isRaid;

	private bool isFade;

	private object tween;

	private const string SCREENSHOT_DIR = "ScreenShot/";

	private const string FILEPATH_PREFIX = "CounterSide-";

	private const string FILENAME_DATE_FORMAT = "yyyy-MM-dd HH-mm-ss";

	private const string FILE_EXTENSION = ".png";

	private void Start()
	{
		isFade = false;
		isRaid = false;
		if (UnitRoot == null)
		{
			UnitRoot = GameObject.Find("POS_UNIT").transform;
		}
		if (UnitRoot != null)
		{
			hixBoxes = UnitRoot.GetComponentsInChildren<DrawHitBox>(includeInactive: true);
		}
		if (MainCam == null)
		{
			MainCam = Camera.main;
			MainCam.backgroundColor = Color.black;
		}
		if (UIRoot == null)
		{
			UIRoot = GameObject.Find("Canvas");
		}
		if (UIRoot != null)
		{
			if (HitBoxText != null)
			{
				HitBoxTmp = UnityEngine.Object.Instantiate(HitBoxText, UIRoot.transform).GetComponent<TextMeshProUGUI>();
				HitBoxTmp.rectTransform.localPosition = Vector3.zero;
				HitBoxTmp.rectTransform.anchoredPosition = new Vector2(0f, 0f);
				SetHitBox();
			}
			if (MapText != null)
			{
				MapTmp = UnityEngine.Object.Instantiate(MapText, UIRoot.transform).GetComponent<TextMeshProUGUI>();
				MapTmp.rectTransform.localPosition = Vector3.zero;
				MapTmp.rectTransform.anchoredPosition = new Vector2(-25.18f, 4.3f);
			}
		}
		ClearCurrentMap();
		MakeCurrentMap();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Renderer[] componentsInChildren = currentMap.GetComponentsInChildren<Renderer>();
			if (tween != null && DOTween.IsTweening(tween))
			{
				return;
			}
			if (!isFade)
			{
				isFade = true;
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					SpriteRenderer component = componentsInChildren[i].GetComponent<SpriteRenderer>();
					if (component != null)
					{
						tween = component.material.DOColor(Color.black, FadeOutTime).target;
					}
					else if (componentsInChildren[i].GetComponent<SkeletonAnimation>() != null)
					{
						tween = componentsInChildren[i].material.DOColor(Color.black, FadeOutTime).target;
					}
				}
			}
			else
			{
				isFade = false;
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					SpriteRenderer component = componentsInChildren[j].GetComponent<SpriteRenderer>();
					if (component != null)
					{
						tween = component.material.DOColor(Color.white, FadeInTime).target;
					}
					else if (componentsInChildren[j].GetComponent<SkeletonAnimation>() != null)
					{
						tween = componentsInChildren[j].material.DOColor(Color.white, FadeInTime).target;
					}
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			ShowHitBox = !ShowHitBox;
			SetHitBox();
		}
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (isRaid)
			{
				isRaid = false;
				ClearCurrentMap();
				MakeCurrentMap();
				MainCam.orthographicSize = NormalViewSize;
				UnitRoot.localPosition = NormalViewUnitPos;
			}
			else
			{
				isRaid = true;
				ClearCurrentMap();
				currentMap = UnityEngine.Object.Instantiate(RaidMap, MapRoot.transform);
				MapTmp.text = "(Raid) : " + RaidMap.name;
				MapTmp.GetComponent<DOTweenAnimation>().DORestart();
				MainCam.orthographicSize = RaidViewSize;
				UnitRoot.localPosition = RaidViewUnitPos;
			}
		}
		if (isRaid)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			Index--;
			if (0 > Index)
			{
				Index = Mathf.Clamp(BML.Maps.Length, 0, BML.Maps.Length - 1);
			}
			ClearCurrentMap();
			MakeCurrentMap();
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			Index++;
			if (Mathf.Clamp(BML.Maps.Length, 0, BML.Maps.Length - 1) < Index)
			{
				Index = 0;
			}
			ClearCurrentMap();
			MakeCurrentMap();
		}
		if (Input.GetKeyUp(KeyCode.F12))
		{
			CaptureScreen();
		}
	}

	private void SetHitBox()
	{
		if (ShowHitBox)
		{
			HitBoxTmp.text = "HIT BOX ON";
		}
		else
		{
			HitBoxTmp.text = "HIT BOX OFF";
		}
		HitBoxTmp.GetComponent<DOTweenAnimation>().DORestart();
		if (hixBoxes == null || hixBoxes.Length == 0)
		{
			return;
		}
		for (int i = 0; i < hixBoxes.Length; i++)
		{
			if (hixBoxes[i] != null)
			{
				hixBoxes[i].ShowHitBox(ShowHitBox);
			}
		}
	}

	private void ClearCurrentMap()
	{
		int childCount = MapRoot.transform.childCount;
		GameObject[] array = new GameObject[childCount];
		for (int i = 0; i < childCount; i++)
		{
			array[i] = MapRoot.transform.GetChild(i).gameObject;
		}
		if (array.Length != 0)
		{
			GameObject[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				UnityEngine.Object.DestroyImmediate(array2[j]);
			}
		}
	}

	private void MakeCurrentMap()
	{
		if (!(MapTmp != null))
		{
			return;
		}
		for (int i = 0; i < BML.Maps.Length; i++)
		{
			if (Index == i)
			{
				currentMap = UnityEngine.Object.Instantiate(BML.Maps[i], MapRoot.transform);
				MapTmp.text = "(" + i + ") : " + BML.Maps[i].name;
				break;
			}
		}
		MapTmp.GetComponent<DOTweenAnimation>().DORestart();
	}

	public static bool CaptureScreen()
	{
		if (!Directory.Exists("ScreenShot/"))
		{
			Directory.CreateDirectory("ScreenShot/");
		}
		string text = MakeCaptureFileName();
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		ScreenCapture.CaptureScreenshot(text);
		Debug.Log("Screencapture : " + text);
		return true;
	}

	private static string MakeCaptureFileName()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo("ScreenShot/");
		if (directoryInfo == null || !directoryInfo.Exists)
		{
			return null;
		}
		string text = directoryInfo.FullName + "CounterSide-" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
		string text2 = text + ".png";
		if (!File.Exists(text2))
		{
			return text2;
		}
		int num = 60;
		for (int i = 1; i < num; i++)
		{
			text2 = text + $" ({i})" + ".png";
			if (!File.Exists(text2))
			{
				return text2;
			}
		}
		return null;
	}
}
