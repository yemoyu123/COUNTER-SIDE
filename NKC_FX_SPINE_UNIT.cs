using UnityEngine;
using UnityEngine.UI;

public class NKC_FX_SPINE_UNIT : MonoBehaviour
{
	public NKC_FX_SPINE_EVENT Spine_Event;

	public Transform Root;

	public float speed = 1f;

	public string RunAnimation = "RUN";

	public string AstandAnimation = "ASTAND";

	[Space]
	[Header("Key Action")]
	public Button Action_1;

	public Button Action_2;

	public Button Action_3;

	public Button Action_4;

	public Button Action_5;

	public Button Action_6;

	public Button Action_7;

	public Button Action_8;

	public Button Action_9;

	public Button Action_0;

	public Button Action_Q;

	public Button Action_W;

	public Button Action_E;

	public Button Action_R;

	private bool isRunning;

	private bool isAstand;

	private float translation;

	private Vector3 position2D = new Vector3(0f, 0f, 0f);

	private Vector3 front = new Vector3(0f, 0f, 0f);

	private Vector3 back = new Vector3(0f, 180f, 0f);

	private void Start()
	{
		isRunning = false;
	}

	private void Update()
	{
		float axis = Input.GetAxis("Horizontal");
		if (axis > 0f)
		{
			if (Root != null)
			{
				Root.localEulerAngles = front;
			}
			isAstand = false;
			ActionRun();
			translation = 1f * speed * 100f;
			translation *= Time.deltaTime;
			if (Root != null)
			{
				Root.Translate(translation, 0f, 0f);
				position2D.Set(Root.localPosition.x, Root.localPosition.y, 0f);
				Root.localPosition = position2D;
			}
		}
		else if (axis < 0f)
		{
			if (Root != null)
			{
				Root.localEulerAngles = back;
			}
			isAstand = false;
			ActionRun();
			translation = -1f * (0f - speed) * 100f;
			translation *= Time.deltaTime;
			if (Root != null)
			{
				Root.Translate(translation, 0f, 0f);
				position2D.Set(Root.localPosition.x, Root.localPosition.y, 0f);
				Root.localPosition = position2D;
			}
		}
		else
		{
			isRunning = false;
			ActionAstand();
		}
		if (Input.anyKey)
		{
			bool keyDown = Input.GetKeyDown(KeyCode.Alpha0);
			bool keyDown2 = Input.GetKeyDown(KeyCode.Alpha1);
			bool keyDown3 = Input.GetKeyDown(KeyCode.Alpha2);
			bool keyDown4 = Input.GetKeyDown(KeyCode.Alpha3);
			bool keyDown5 = Input.GetKeyDown(KeyCode.Alpha4);
			bool keyDown6 = Input.GetKeyDown(KeyCode.Alpha5);
			bool keyDown7 = Input.GetKeyDown(KeyCode.Alpha6);
			bool keyDown8 = Input.GetKeyDown(KeyCode.Alpha7);
			bool keyDown9 = Input.GetKeyDown(KeyCode.Alpha8);
			bool keyDown10 = Input.GetKeyDown(KeyCode.Alpha9);
			bool keyDown11 = Input.GetKeyDown(KeyCode.Q);
			bool keyDown12 = Input.GetKeyDown(KeyCode.W);
			bool keyDown13 = Input.GetKeyDown(KeyCode.E);
			bool keyDown14 = Input.GetKeyDown(KeyCode.R);
			if (keyDown && Action_0 != null)
			{
				Action_0.onClick.Invoke();
			}
			if (keyDown2 && Action_1 != null)
			{
				Action_1.onClick.Invoke();
			}
			if (keyDown3 && Action_2 != null)
			{
				Action_2.onClick.Invoke();
			}
			if (keyDown4 && Action_3 != null)
			{
				Action_3.onClick.Invoke();
			}
			if (keyDown5 && Action_4 != null)
			{
				Action_4.onClick.Invoke();
			}
			if (keyDown6 && Action_5 != null)
			{
				Action_5.onClick.Invoke();
			}
			if (keyDown7 && Action_6 != null)
			{
				Action_6.onClick.Invoke();
			}
			if (keyDown8 && Action_7 != null)
			{
				Action_7.onClick.Invoke();
			}
			if (keyDown9 && Action_8 != null)
			{
				Action_8.onClick.Invoke();
			}
			if (keyDown10 && Action_9 != null)
			{
				Action_9.onClick.Invoke();
			}
			if (keyDown11 && Action_Q != null)
			{
				Action_Q.onClick.Invoke();
			}
			if (keyDown12 && Action_W != null)
			{
				Action_W.onClick.Invoke();
			}
			if (keyDown13 && Action_E != null)
			{
				Action_E.onClick.Invoke();
			}
			if (keyDown14 && Action_R != null)
			{
				Action_R.onClick.Invoke();
			}
		}
	}

	public void SetButton()
	{
		if (!(Spine_Event != null))
		{
			return;
		}
		Button[] componentsInChildren = Spine_Event.GetComponentsInChildren<Button>();
		Action_1 = null;
		Action_2 = null;
		Action_3 = null;
		Action_4 = null;
		Action_5 = null;
		Action_6 = null;
		Action_7 = null;
		Action_8 = null;
		Action_9 = null;
		Action_0 = null;
		Action_Q = null;
		Action_W = null;
		Action_E = null;
		Action_R = null;
		int num = 0;
		if (componentsInChildren.Length != 0)
		{
			if (componentsInChildren.Length > num)
			{
				Action_1 = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_2 = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_3 = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_4 = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_5 = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_6 = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_7 = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_8 = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_9 = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_0 = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_Q = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_W = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_E = componentsInChildren[num];
			}
			num++;
			if (componentsInChildren.Length > num)
			{
				Action_R = componentsInChildren[num];
			}
			num++;
		}
	}

	private void ActionRun()
	{
		if (!isRunning && Spine_Event != null)
		{
			Spine_Event.SetAnimationName(RunAnimation);
			isRunning = true;
		}
	}

	private void ActionAstand()
	{
		if (!isAstand && Spine_Event != null)
		{
			Spine_Event.SetAnimationName(AstandAnimation);
			isAstand = true;
		}
	}
}
