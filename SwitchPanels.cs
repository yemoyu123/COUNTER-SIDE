using UnityEngine;
using UnityEngine.UI;

public class SwitchPanels : MonoBehaviour
{
	public GameObject Menu;

	public GameObject Panel;

	private void Awake()
	{
		GetComponent<Toggle>().onValueChanged.AddListener(OnToggleClick);
	}

	public void OnToggleClick(bool isActive)
	{
		Menu.SetActive(isActive);
		Panel.SetActive(!isActive);
	}
}
