using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_MOTIONAFTERIMAGE : NKC_FXM_EVALUATER
{
	[Header("\ufffd\ufffd\ufffd\ufffd")]
	[Tooltip("\ufffd\u073b\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public Renderer m_targetRenderer;

	[Tooltip("\ufffd\u073b\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public float m_fSpawnFinishTime;

	[Tooltip("\ufffd\u05b4\ufffd \ufffd\u073b\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public int m_maxImageCount = 5;

	[Tooltip("\ufffd\u073b\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public float m_fGapTime = 0.1f;

	[Tooltip("\ufffd\u073b\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public Color m_Color;

	[Tooltip("\ufffd\u073b\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdӵ\ufffd")]
	public float m_fFadeSpeed = 1.5f;

	[Tooltip("\ufffd\u073b\ufffd \ufffd\u05b4\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffdŸ\ufffd\ufffd")]
	public float m_fLifeTime = 2f;

	private float timeLeftToSpawn;

	private NKC2DMotionAfterImage m_NKCMotionAfterImage = new NKC2DMotionAfterImage();

	public override void Init()
	{
		if (m_targetRenderer == null)
		{
			init = false;
			return;
		}
		m_NKCMotionAfterImage.Init(m_maxImageCount, m_targetRenderer);
		m_NKCMotionAfterImage.SetGapTime(m_fGapTime);
		m_NKCMotionAfterImage.SetFadeSpeed(m_fFadeSpeed);
		m_NKCMotionAfterImage.SetColor(m_Color);
		m_NKCMotionAfterImage.SetLifeTime(m_fLifeTime);
		init = true;
	}

	protected override void OnStart()
	{
		base.OnStart();
		m_NKCMotionAfterImage.ResetGapTime();
		m_NKCMotionAfterImage.SetEnable(bEnable: true);
		timeLeftToSpawn = m_fSpawnFinishTime;
	}

	protected override void OnExecute(bool _render = true)
	{
		base.OnExecute(_render);
		timeLeftToSpawn -= deltaTime;
		m_NKCMotionAfterImage.Update(deltaTime);
		if (timeLeftToSpawn <= 0f)
		{
			m_NKCMotionAfterImage.SetEnable(bEnable: false);
		}
	}

	protected override void OnComplete()
	{
		base.OnComplete();
		m_NKCMotionAfterImage.StopMotionImage();
	}

	private void OnDisable()
	{
		if (m_NKCMotionAfterImage != null)
		{
			m_NKCMotionAfterImage.StopMotionImage();
		}
	}

	private void OnDestroy()
	{
		if (m_NKCMotionAfterImage != null)
		{
			m_NKCMotionAfterImage.Clear();
			m_NKCMotionAfterImage = null;
		}
	}
}
