using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NKC_FX_SCENE_MANAGER : MonoBehaviour
{
	[SerializeField]
	[Range(0f, 1f)]
	private float globalTransparancy = 1f;

	public GameObject UIRoot;

	public GameObject SceneDropDown;

	private GameObject sceneDropDown;

	private Dropdown dropdown;

	private Slider vfxOpacitySlider;

	private string path = string.Empty;

	private string sceneName = string.Empty;

	private string sceneList = string.Empty;

	private List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();

	private bool isHideHud;

	public float GlobalTransparancy
	{
		get
		{
			return globalTransparancy;
		}
		set
		{
			globalTransparancy = value;
			SetGlobalTransparency(globalTransparancy);
		}
	}

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Slider_VfxOpacity");
		if (gameObject != null)
		{
			vfxOpacitySlider = gameObject.GetComponent<Slider>();
			vfxOpacitySlider.onValueChanged.AddListener(delegate
			{
				SetGlobalTransparency(vfxOpacitySlider.value);
			});
		}
		if (UIRoot == null)
		{
			UIRoot = GameObject.Find("Canvas");
		}
		if (UIRoot != null && SceneDropDown != null)
		{
			sceneDropDown = Object.Instantiate(SceneDropDown, UIRoot.transform);
			sceneDropDown.transform.localPosition = new Vector3(-50f, 300f, 0f);
			dropdown = sceneDropDown.GetComponent<Dropdown>();
			InitializeDropdown();
			dropdown.onValueChanged.AddListener(delegate
			{
				OnDropDownChanged(dropdown);
			});
			sceneDropDown.SetActive(value: false);
		}
	}

	private void InitializeDropdown()
	{
		if (SceneManager.sceneCountInBuildSettings > 0)
		{
			dropdown.ClearOptions();
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				path = SceneUtility.GetScenePathByBuildIndex(i);
				sceneName = Path.GetFileNameWithoutExtension(path);
				optionDatas.Add(new Dropdown.OptionData(sceneName));
				sceneList = sceneList + SceneUtility.GetScenePathByBuildIndex(i) + "\n";
			}
			dropdown.AddOptions(optionDatas);
			Scene activeScene = SceneManager.GetActiveScene();
			dropdown.value = activeScene.buildIndex;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			sceneDropDown.SetActive(!sceneDropDown.activeSelf);
		}
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			isHideHud = !isHideHud;
			HideHud(isHideHud);
		}
	}

	private void OnDropDownChanged(Dropdown _dropDown)
	{
		LoadScene(_dropDown.value);
	}

	private void LoadScene(int _index)
	{
		path = SceneUtility.GetScenePathByBuildIndex(_index);
		SceneManager.LoadScene(path, LoadSceneMode.Single);
	}

	private void HideHud(bool _toggle)
	{
		int childCount = UIRoot.transform.childCount;
		int instanceID = sceneDropDown.gameObject.GetInstanceID();
		for (int i = 0; i < childCount; i++)
		{
			if (UIRoot.transform.GetChild(i).gameObject.GetInstanceID() != instanceID)
			{
				UIRoot.transform.GetChild(i).gameObject.SetActive(_toggle);
			}
		}
	}

	public void SetGlobalTransparency(float _factor)
	{
		Shader.SetGlobalFloat("_FxGlobalTransparency", _factor);
		Shader.SetGlobalFloat("_FxGlobalTransparencyEnemy", _factor);
	}
}
