using System;
using NKC.ImageEffects;
using NKM;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace NKC;

public class NKCCamera
{
	public enum FitMode
	{
		FitToScreen,
		FitAuto,
		FitToWidth,
		FitToHeight,
		FitIn
	}

	public enum ScaleMode
	{
		Scale,
		RectSize
	}

	private static GameObject m_SCEN_MAIN_Camera = null;

	private static Camera m_SCEN_MAIN_Camera_Camera = null;

	private static GameObject m_SCEN_SUB_UI_LOW_Camera = null;

	private static Camera m_SCEN_SUB_UI_LOW_Camera_Camera = null;

	private static SepiaTone m_SepiaTone_SUB_UI_LOW_CAMERA = null;

	private static GameObject m_SCEN_SUB_UI_Camera = null;

	private static Camera m_SCEN_SUB_UI_Camera_Camera = null;

	private static NKCUIComVideoCamera m_SCEN_SUB_UI_Camera_Video_Player = null;

	private static Camera m_SCEN_BLACK_Camera = null;

	private static Transform m_SCEN_MAIN_Camera_Transform = null;

	private static bool m_bBloomEnable;

	private static bool m_bBloomEnableNow;

	private static bool m_bBloomEnableUI;

	private static NKCBloom m_SCEN_MAIN_Camera_Bloom = null;

	private static NKMTrackingVector3 m_NKMTrackingPos = new NKMTrackingVector3();

	private static NKMTrackingVector3 m_NKMTrackingRotation = new NKMTrackingVector3();

	private static NKMTrackingFloat m_NKMTrackingZoom = new NKMTrackingFloat();

	private static float m_fCameraAspect;

	private static float m_CameraMinX = 0f;

	private static float m_CameraMaxX = 0f;

	private static float m_fCamMinYGap = 0f;

	private static float m_fCamMaxYGap = 0f;

	private static float m_CameraSizeOrg = 0f;

	private static float m_CameraSizeMax = 512f;

	private static CamCrashControl m_CamCrashUp = new CamCrashControl();

	private static CamCrashControl m_CamCrashDown = new CamCrashControl();

	private static CamCrashControl m_CamCrashUpDown = new CamCrashControl();

	private static float m_fDeltaTime = 0f;

	private static Vector3 m_CamPosTemp = default(Vector3);

	private static bool m_BattleCam = false;

	private static int m_BattlePriority = 0;

	private static float m_fBloomIntensityOrg = 0.7f;

	private static float m_fBloomThreshHoldOrg = 0.5f;

	private static CoolMotionBlur m_CoolMotionBlur = null;

	private static Material m_FocusMat = null;

	private static NKMTrackingFloat m_FocusFactor = new NKMTrackingFloat();

	private static float m_fFocusTime = 0f;

	private static BlurOptimized m_BlurOptimized = null;

	public static bool m_bBlur = false;

	private static ScreenWater m_ScreenWater = null;

	private static NKMTrackingFloat m_tracScreenWaterSpeed = new NKMTrackingFloat();

	private static NKMTrackingFloat m_tracScreenWaterIntens = new NKMTrackingFloat();

	private static Vector3 m_Vec3Temp = default(Vector3);

	private static RectTransform m_rectCamMoveBound;

	private static Rect m_rectCamBound;

	public static void SetEnableSepiaToneSubUILowCam(bool bSet)
	{
		m_SepiaTone_SUB_UI_LOW_CAMERA.enabled = bSet;
	}

	public static void SetBlackCameraEnable(bool value)
	{
		if (m_SCEN_BLACK_Camera != null)
		{
			NKCUtil.SetGameobjectActive(m_SCEN_BLACK_Camera.gameObject, value);
		}
	}

	public static Camera GetCamera()
	{
		return m_SCEN_MAIN_Camera_Camera;
	}

	public static Camera GetSubUILowCamera()
	{
		return m_SCEN_SUB_UI_LOW_Camera_Camera;
	}

	public static Camera GetSubUICamera()
	{
		return m_SCEN_SUB_UI_Camera_Camera;
	}

	public static NKCUIComVideoCamera GetSubUICameraVideoPlayer()
	{
		return m_SCEN_SUB_UI_Camera_Video_Player;
	}

	public static bool GetEnableBloom()
	{
		return m_bBloomEnable;
	}

	public static void EnableBloom(bool bEnable)
	{
		m_bBloomEnable = bEnable;
	}

	public static void SetBloomEnableUI(bool bEnable)
	{
		m_bBloomEnableUI = bEnable;
	}

	public static float GetCameraAspect()
	{
		return m_fCameraAspect;
	}

	public static float GetCameraSizeOrg()
	{
		return m_CameraSizeOrg;
	}

	public static float GetBloomIntensityOrg()
	{
		return m_fBloomIntensityOrg;
	}

	public static float GetBloomThreshHoldOrg()
	{
		return m_fBloomThreshHoldOrg;
	}

	public static void SetBoundRectTransform(RectTransform rect)
	{
		m_rectCamMoveBound = rect;
		if (rect != null)
		{
			m_rectCamBound = GetCameraBoundRect(m_rectCamMoveBound, m_CamPosTemp.z);
		}
	}

	public static void Init()
	{
		m_SCEN_MAIN_Camera = GameObject.Find("SCEN_MAIN_Camera");
		m_SCEN_MAIN_Camera_Camera = m_SCEN_MAIN_Camera.GetComponent<Camera>();
		m_SCEN_SUB_UI_LOW_Camera = GameObject.Find("SCEN_SUB_UI_LOW_Camera");
		m_SCEN_SUB_UI_LOW_Camera_Camera = m_SCEN_SUB_UI_LOW_Camera.GetComponent<Camera>();
		m_SepiaTone_SUB_UI_LOW_CAMERA = m_SCEN_SUB_UI_LOW_Camera.GetComponent<SepiaTone>();
		if (m_SepiaTone_SUB_UI_LOW_CAMERA != null)
		{
			m_SepiaTone_SUB_UI_LOW_CAMERA.enabled = false;
		}
		m_SCEN_SUB_UI_Camera = GameObject.Find("SCEN_SUB_UI_Camera");
		m_SCEN_SUB_UI_Camera_Camera = m_SCEN_SUB_UI_Camera.GetComponent<Camera>();
		m_SCEN_SUB_UI_Camera_Video_Player = m_SCEN_SUB_UI_Camera.GetComponent<NKCUIComVideoCamera>();
		GameObject gameObject = new GameObject("SCEN_BLACK_Camera");
		gameObject.transform.parent = m_SCEN_MAIN_Camera.transform.parent;
		m_SCEN_BLACK_Camera = gameObject.AddComponent<Camera>();
		m_SCEN_BLACK_Camera.clearFlags = CameraClearFlags.Color;
		m_SCEN_BLACK_Camera.backgroundColor = Color.black;
		m_SCEN_BLACK_Camera.cullingMask = 0;
		gameObject.SetActive(value: false);
		m_SCEN_MAIN_Camera_Transform = m_SCEN_MAIN_Camera.GetComponent<Transform>();
		m_SCEN_MAIN_Camera_Bloom = m_SCEN_MAIN_Camera.GetComponent<NKCBloom>();
		if (m_SCEN_MAIN_Camera_Bloom != null)
		{
			m_bBloomEnableNow = m_SCEN_MAIN_Camera_Bloom.enabled;
			m_bBloomEnable = m_bBloomEnableNow;
			m_fBloomIntensityOrg = m_SCEN_MAIN_Camera_Bloom.bloomIntensity;
			m_fBloomThreshHoldOrg = m_SCEN_MAIN_Camera_Bloom.bloomThreshold;
		}
		m_CoolMotionBlur = m_SCEN_MAIN_Camera.GetComponent<CoolMotionBlur>();
		m_FocusMat = m_CoolMotionBlur.ScreenMat;
		DisableFocusBlur();
		m_BlurOptimized = m_SCEN_MAIN_Camera.GetComponent<BlurOptimized>();
		if (m_BlurOptimized != null)
		{
			m_BlurOptimized.enabled = false;
		}
		m_ScreenWater = m_SCEN_MAIN_Camera.GetComponent<ScreenWater>();
		if (m_ScreenWater != null)
		{
			m_ScreenWater.enabled = false;
		}
		m_tracScreenWaterSpeed.SetNowValue(0f);
		m_tracScreenWaterIntens.SetNowValue(0f);
		m_fCameraAspect = m_SCEN_MAIN_Camera_Camera.aspect;
	}

	public static void SetCameraRect(Rect rect)
	{
		m_SCEN_MAIN_Camera_Camera.rect = rect;
	}

	public static void InitBattle(float fCamMinX, float fCamMaxX, float fCamMinY, float fCamMaxY, float fCamSize, float fCamSizeMax)
	{
		m_BattleCam = true;
		m_CameraMinX = fCamMinX;
		m_CameraMaxX = fCamMaxX;
		m_fCamMinYGap = fCamMinY;
		m_fCamMaxYGap = fCamMaxY;
		m_CameraSizeOrg = fCamSize;
		m_CameraSizeMax = fCamSizeMax;
		m_NKMTrackingZoom.SetNowValue(m_CameraSizeOrg);
		m_NKMTrackingPos.SetNowValue(m_SCEN_MAIN_Camera_Transform.position.x, m_SCEN_MAIN_Camera_Transform.position.y, m_SCEN_MAIN_Camera_Transform.position.z);
		m_fCameraAspect = m_SCEN_MAIN_Camera_Camera.aspect;
	}

	public static void BattleEnd()
	{
		m_BattleCam = false;
		m_SCEN_MAIN_Camera_Camera.targetTexture = null;
		m_SCEN_SUB_UI_Camera_Camera.targetTexture = null;
		InitCrashCamera();
	}

	public static void BloomProcess()
	{
		if (NKCScenManager.GetScenManager().GetSystemMemorySize() < 3000)
		{
			m_bBloomEnableNow = false;
		}
		else if (m_bBloomEnable)
		{
			if (m_BattleCam)
			{
				if (m_bBloomEnableNow)
				{
					m_bBloomEnableNow = false;
					if (m_SCEN_MAIN_Camera_Bloom != null)
					{
						m_SCEN_MAIN_Camera_Bloom.enabled = m_bBloomEnableNow;
					}
				}
			}
			else if (m_bBloomEnableNow != m_bBloomEnableUI)
			{
				m_bBloomEnableNow = m_bBloomEnableUI;
				if (m_SCEN_MAIN_Camera_Bloom != null)
				{
					m_SCEN_MAIN_Camera_Bloom.enabled = m_bBloomEnableNow;
				}
			}
		}
		else if (m_bBloomEnableNow)
		{
			m_bBloomEnableNow = false;
			if (m_SCEN_MAIN_Camera_Bloom != null)
			{
				m_SCEN_MAIN_Camera_Bloom.enabled = m_bBloomEnableNow;
			}
		}
	}

	private static void UpdateCrashBasic()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null)
		{
			return;
		}
		float num = 0f;
		if (gameOptionData.CameraShakeLevel != NKCGameOptionDataSt.GameOptionCameraShake.None)
		{
			num += m_CamCrashUp.m_fCrashGap;
			num += m_CamCrashDown.m_fCrashGap;
			num = ((!m_CamCrashUpDown.m_bCrashPositive) ? (num - m_CamCrashUpDown.m_fCrashGap) : (num + m_CamCrashUpDown.m_fCrashGap));
			if (gameOptionData.CameraShakeLevel == NKCGameOptionDataSt.GameOptionCameraShake.Low)
			{
				num *= 0.5f;
			}
			m_CamPosTemp.y += num;
		}
	}

	public static void Update(float deltaTime)
	{
		m_fDeltaTime = deltaTime;
		if (m_fFocusTime > 0f)
		{
			m_fFocusTime -= m_fDeltaTime;
			if (m_fFocusTime <= 0f)
			{
				m_fFocusTime = 0f;
				m_FocusFactor.SetTracking(0f, 0.3f, TRACKING_DATA_TYPE.TDT_NORMAL);
			}
		}
		else if (m_FocusFactor.GetNowValue() <= 0f && m_CoolMotionBlur.isActiveAndEnabled)
		{
			DisableFocusBlur();
		}
		m_FocusFactor.Update(m_fDeltaTime);
		m_FocusMat.SetFloat("_SampleDist", m_FocusFactor.GetNowValue());
		m_NKMTrackingPos.Update(m_fDeltaTime);
		if (!m_NKMTrackingPos.IsTracking())
		{
			m_BattlePriority = 0;
		}
		m_NKMTrackingZoom.Update(m_fDeltaTime);
		m_NKMTrackingRotation.Update(m_fDeltaTime);
		m_tracScreenWaterSpeed.Update(m_fDeltaTime);
		m_tracScreenWaterIntens.Update(m_fDeltaTime);
		if (m_ScreenWater != null && m_tracScreenWaterSpeed.IsTracking())
		{
			m_ScreenWater.Speed = m_tracScreenWaterSpeed.GetNowValue();
		}
		if (m_ScreenWater != null && m_tracScreenWaterIntens.IsTracking())
		{
			m_ScreenWater.Intens = m_tracScreenWaterIntens.GetNowValue();
		}
		CamCrashProcessUp();
		CamCrashProcessDown();
		CamCrashProcessUpDown();
		m_CamPosTemp.x = m_NKMTrackingPos.GetNowValueX();
		m_CamPosTemp.y = m_NKMTrackingPos.GetNowValueY();
		m_CamPosTemp.z = m_NKMTrackingPos.GetNowValueZ();
		if (m_BattleCam)
		{
			float cameraSizeNow = GetCameraSizeNow();
			m_SCEN_MAIN_Camera_Camera.orthographicSize = cameraSizeNow;
			float num = cameraSizeNow * m_fCameraAspect * 0.5f;
			if (num * 2f > m_CameraMaxX - m_CameraMinX)
			{
				m_CamPosTemp.x = (m_CameraMinX + m_CameraMaxX) * 0.5f;
				m_NKMTrackingPos.SetNowValue(m_CamPosTemp.x, m_NKMTrackingPos.GetNowValueY(), m_NKMTrackingPos.GetNowValueZ());
			}
			else if (m_CamPosTemp.x < m_CameraMinX + num)
			{
				m_CamPosTemp.x = m_CameraMinX + num;
				m_NKMTrackingPos.SetNowValue(m_CamPosTemp.x, m_NKMTrackingPos.GetNowValueY(), m_NKMTrackingPos.GetNowValueZ());
			}
			else if (m_CamPosTemp.x > m_CameraMaxX - num)
			{
				m_CamPosTemp.x = m_CameraMaxX - num;
				m_NKMTrackingPos.SetNowValue(m_CamPosTemp.x, m_NKMTrackingPos.GetNowValueY(), m_NKMTrackingPos.GetNowValueZ());
			}
			num = m_CameraSizeOrg - cameraSizeNow;
			if (m_CamPosTemp.y > num + m_fCamMaxYGap)
			{
				m_CamPosTemp.y = num + m_fCamMaxYGap;
			}
			else if (m_CamPosTemp.y < 0f - num + m_fCamMinYGap)
			{
				m_CamPosTemp.y = 0f - num + m_fCamMinYGap;
			}
			UpdateCrashBasic();
			num = m_CameraSizeMax - cameraSizeNow;
			if (m_CamPosTemp.y > num + m_fCamMaxYGap)
			{
				m_CamPosTemp.y = num + m_fCamMaxYGap;
			}
			else if (m_CamPosTemp.y < 0f - num + m_fCamMinYGap)
			{
				m_CamPosTemp.y = 0f - num + m_fCamMinYGap;
			}
		}
		else
		{
			UpdateCrashBasic();
		}
		BloomProcess();
		if (m_rectCamMoveBound != null)
		{
			if (m_CamPosTemp.z != m_SCEN_MAIN_Camera_Transform.position.z)
			{
				m_rectCamBound = GetCameraBoundRect(m_rectCamMoveBound, m_CamPosTemp.z);
			}
			m_CamPosTemp.x = Mathf.Clamp(m_CamPosTemp.x, m_rectCamBound.xMin, m_rectCamBound.xMax);
			m_CamPosTemp.y = Mathf.Clamp(m_CamPosTemp.y, m_rectCamBound.yMin, m_rectCamBound.yMax);
		}
		m_SCEN_MAIN_Camera_Transform.position = m_CamPosTemp;
		m_SCEN_MAIN_Camera_Transform.rotation = Quaternion.Euler(m_NKMTrackingRotation.GetNowValueX(), m_NKMTrackingRotation.GetNowValueY(), m_NKMTrackingRotation.GetNowValueZ());
	}

	public static void EnableBlur(bool bEnable, float blurSize = 2f, int blurIteration = 2)
	{
		m_bBlur = bEnable;
		if (m_BlurOptimized != null)
		{
			m_BlurOptimized.blurSize = blurSize;
			m_BlurOptimized.blurIterations = blurIteration;
			m_BlurOptimized.enabled = m_bBlur;
		}
	}

	public static void EnableScreenWater(bool bEnable, float fSpeed, float fIntens, float fTime)
	{
		if (m_ScreenWater != null && bEnable != m_ScreenWater.enabled)
		{
			m_ScreenWater.enabled = bEnable;
		}
		m_tracScreenWaterSpeed.SetTracking(fSpeed, fTime, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_tracScreenWaterIntens.SetTracking(fIntens, fTime, TRACKING_DATA_TYPE.TDT_SLOWER);
		if (!bEnable)
		{
			m_tracScreenWaterSpeed.SetNowValue(0f);
			m_tracScreenWaterIntens.SetNowValue(0f);
			if (m_ScreenWater != null)
			{
				m_ScreenWater.Speed = m_tracScreenWaterSpeed.GetNowValue();
				m_ScreenWater.Intens = m_tracScreenWaterIntens.GetNowValue();
			}
		}
	}

	public static void DisableFocusBlur()
	{
		m_CoolMotionBlur.enabled = false;
	}

	public static void SetFocusBlur(float fFocusTime, float fCenterPosX = 0.5f, float fCenterPosY = 0.5f, float fCenterPosZ = 0f)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null || gameOptionData.UseGameEffect)
		{
			m_CoolMotionBlur.enabled = true;
			if (m_fFocusTime <= 0f)
			{
				m_FocusFactor.SetTracking(0.1f, 0.3f, TRACKING_DATA_TYPE.TDT_NORMAL);
			}
			m_fFocusTime = fFocusTime;
			m_Vec3Temp.Set(fCenterPosX, fCenterPosY, fCenterPosZ);
			m_Vec3Temp = m_SCEN_MAIN_Camera_Camera.WorldToScreenPoint(m_Vec3Temp);
			m_Vec3Temp.x /= Screen.width;
			m_Vec3Temp.y /= Screen.height;
			if (m_Vec3Temp.x < 0f || m_Vec3Temp.x > 1f)
			{
				m_Vec3Temp.x = 0.5f;
			}
			if (m_Vec3Temp.y < 0f || m_Vec3Temp.y > 1f)
			{
				m_Vec3Temp.y = 0.5f;
			}
			m_Vec3Temp.z = 0f;
			m_FocusMat.SetVector("_Center", m_Vec3Temp);
		}
	}

	public static void SetBloomIntensity(float fIntensity)
	{
		if (m_SCEN_MAIN_Camera_Bloom != null)
		{
			m_SCEN_MAIN_Camera_Bloom.bloomIntensity = fIntensity;
		}
	}

	public static void SetBloomThreshHold(float fThreshHold)
	{
		if (m_SCEN_MAIN_Camera_Bloom != null)
		{
			m_SCEN_MAIN_Camera_Bloom.bloomThreshold = fThreshHold;
		}
	}

	public static float GetPosNowX(bool bUseEdge = false)
	{
		if (!bUseEdge)
		{
			return m_NKMTrackingPos.GetNowValue().x;
		}
		return m_SCEN_MAIN_Camera_Transform.position.x;
	}

	public static float GetPosNowY(bool bUseEdge = false)
	{
		if (!bUseEdge)
		{
			return m_NKMTrackingPos.GetNowValue().y;
		}
		return m_SCEN_MAIN_Camera_Transform.position.y;
	}

	public static float GetPosNowZ(bool bUseEdge = false)
	{
		if (!bUseEdge)
		{
			return m_NKMTrackingPos.GetNowValue().z;
		}
		return m_SCEN_MAIN_Camera_Transform.position.z;
	}

	public static NKMTrackingVector3 GetTrackingPos()
	{
		return m_NKMTrackingPos;
	}

	public static NKMTrackingVector3 GetTrackingRotation()
	{
		return m_NKMTrackingRotation;
	}

	public static bool IsTrackingCamera()
	{
		if (m_NKMTrackingPos.IsTracking() || m_NKMTrackingZoom.IsTracking())
		{
			return true;
		}
		return false;
	}

	public static bool IsTrackingCameraPos()
	{
		if (m_NKMTrackingPos.IsTracking())
		{
			return true;
		}
		return false;
	}

	public static bool IsTrackingWater()
	{
		if (m_tracScreenWaterSpeed.IsTracking() || m_tracScreenWaterIntens.IsTracking())
		{
			return true;
		}
		return false;
	}

	public static void StopTrackingCamera()
	{
		m_NKMTrackingPos.StopTracking();
		m_NKMTrackingZoom.SetTracking(m_CameraSizeOrg, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_NKMTrackingRotation.StopTracking();
		m_BattlePriority = 0;
	}

	public static void SetZoom(float fZoomSize, bool bTrackingStop)
	{
		if (bTrackingStop)
		{
			m_NKMTrackingZoom.StopTracking();
		}
		m_NKMTrackingZoom.SetNowValue(fZoomSize);
	}

	public static float GetZoomRate()
	{
		return m_NKMTrackingZoom.GetNowValue() / m_CameraSizeOrg;
	}

	public static float GetCameraSizeNow()
	{
		float num = m_NKMTrackingZoom.GetNowValue();
		if (num > m_CameraSizeMax)
		{
			num = m_CameraSizeMax;
		}
		return num;
	}

	public static float GetScreenRatio(bool bSafeRect)
	{
		if (bSafeRect)
		{
			return Screen.safeArea.width / Screen.safeArea.height;
		}
		return (float)Screen.width / (float)Screen.height;
	}

	public static void SetPos(Vector3 pos, bool bTrackingStop, bool bForce = false)
	{
		if (bTrackingStop)
		{
			m_NKMTrackingPos.StopTracking();
		}
		if (!m_NKMTrackingPos.IsTracking())
		{
			m_NKMTrackingPos.SetNowValue(pos.x, pos.y, pos.z);
		}
		if (bForce)
		{
			m_CamPosTemp.x = m_NKMTrackingPos.GetNowValueX();
			m_CamPosTemp.y = m_NKMTrackingPos.GetNowValueY();
			m_CamPosTemp.z = m_NKMTrackingPos.GetNowValueZ();
			m_SCEN_MAIN_Camera_Transform.position = m_CamPosTemp;
		}
	}

	public static void SetPos(float x = -1f, float y = -1f, float z = -1f, bool bTrackingStop = true, bool bForce = false)
	{
		if (bTrackingStop)
		{
			m_NKMTrackingPos.StopTracking();
		}
		if (!m_NKMTrackingPos.IsTracking())
		{
			if (x != -1f)
			{
				m_CamPosTemp.x = x;
			}
			else
			{
				m_CamPosTemp.x = m_NKMTrackingPos.GetNowValue().x;
			}
			if (y != -1f)
			{
				m_CamPosTemp.y = y;
			}
			else
			{
				m_CamPosTemp.y = m_NKMTrackingPos.GetNowValue().y;
			}
			if (z != -1f)
			{
				m_CamPosTemp.z = z;
			}
			else
			{
				m_CamPosTemp.z = m_NKMTrackingPos.GetNowValue().z;
			}
			m_NKMTrackingPos.SetNowValue(m_CamPosTemp.x, m_CamPosTemp.y, m_CamPosTemp.z);
		}
		if (bForce)
		{
			m_CamPosTemp.x = m_NKMTrackingPos.GetNowValueX();
			m_CamPosTemp.y = m_NKMTrackingPos.GetNowValueY();
			m_CamPosTemp.z = m_NKMTrackingPos.GetNowValueZ();
			m_SCEN_MAIN_Camera_Transform.position = m_CamPosTemp;
		}
	}

	public static void SetPosRel(Vector3 pos, bool bTrackingStop)
	{
		if (bTrackingStop)
		{
			m_NKMTrackingPos.StopTracking();
		}
		if (!m_NKMTrackingPos.IsTracking())
		{
			m_NKMTrackingPos.SetNowValue(m_NKMTrackingPos.GetNowValueX() + pos.x, m_NKMTrackingPos.GetNowValueY() + pos.y, m_NKMTrackingPos.GetNowValueZ() + pos.z);
		}
	}

	public static void SetPosRel(Vector2 pos, bool bTrackingStop = true)
	{
		if (bTrackingStop)
		{
			m_NKMTrackingPos.StopTracking();
		}
		if (!m_NKMTrackingPos.IsTracking())
		{
			m_CamPosTemp.Set(pos.x, pos.y, m_SCEN_MAIN_Camera_Transform.position.z);
			m_NKMTrackingPos.SetNowValue(m_SCEN_MAIN_Camera_Transform.position.x + m_CamPosTemp.x, m_SCEN_MAIN_Camera_Transform.position.y + m_CamPosTemp.y, m_SCEN_MAIN_Camera_Transform.position.z + m_CamPosTemp.z);
		}
	}

	public static void SetPosRel(float x, float y, float z, bool bTrackingStop = true)
	{
		if (bTrackingStop)
		{
			m_NKMTrackingPos.StopTracking();
		}
		if (!m_NKMTrackingPos.IsTracking())
		{
			m_CamPosTemp.x = x;
			m_CamPosTemp.y = y;
			m_CamPosTemp.z = z;
			m_NKMTrackingPos.SetNowValue(m_NKMTrackingPos.GetNowValueX() + m_CamPosTemp.x, m_NKMTrackingPos.GetNowValueY() + m_CamPosTemp.y, m_NKMTrackingPos.GetNowValueZ() + m_CamPosTemp.z);
		}
	}

	public static float GetDist(NKMUnit cNKMUnit)
	{
		return Mathf.Abs(m_NKMTrackingPos.GetNowValue().x - cNKMUnit.GetUnitSyncData().m_PosX);
	}

	public static float GetDist(NKMDamageEffect cNKMDamageEffect)
	{
		return Mathf.Abs(m_NKMTrackingPos.GetNowValue().x - cNKMDamageEffect.GetDEData().m_PosX);
	}

	public static void CamCrashProcessUp()
	{
		if (m_CamCrashUp.m_fCrashGap >= 0f && m_CamCrashUp.m_fCrashAccel != 0f)
		{
			m_CamCrashUp.m_fCrashGap += m_CamCrashUp.m_fCrashSpeed * m_fDeltaTime;
			m_CamCrashUp.m_fCrashSpeed += m_CamCrashUp.m_fCrashAccel * m_fDeltaTime;
		}
		else
		{
			m_CamCrashUp.m_fCrashGap = 0f;
			m_CamCrashUp.m_fCrashSpeed = 0f;
			m_CamCrashUp.m_fCrashAccel = 0f;
		}
	}

	public static void CamCrashProcessDown()
	{
		if (m_CamCrashDown.m_fCrashGap <= 0f && m_CamCrashDown.m_fCrashAccel != 0f)
		{
			m_CamCrashDown.m_fCrashGap += m_CamCrashDown.m_fCrashSpeed * m_fDeltaTime;
			m_CamCrashDown.m_fCrashSpeed += m_CamCrashDown.m_fCrashAccel * m_fDeltaTime;
		}
		else
		{
			m_CamCrashDown.m_fCrashGap = 0f;
			m_CamCrashDown.m_fCrashSpeed = 0f;
			m_CamCrashDown.m_fCrashAccel = 0f;
		}
	}

	public static void CamCrashProcessUpDown()
	{
		if (m_CamCrashUpDown.m_fCrashTime > 0f)
		{
			m_CamCrashUpDown.m_fCrashTime -= m_fDeltaTime;
			m_CamCrashUpDown.m_fCrashTimeGap -= m_fDeltaTime;
			if (m_CamCrashUpDown.m_fCrashTimeGap <= 0f)
			{
				m_CamCrashUpDown.m_fCrashTimeGap = m_CamCrashUpDown.m_fCrashTimeGapOrg;
				m_CamCrashUpDown.m_bCrashPositive = !m_CamCrashUpDown.m_bCrashPositive;
			}
		}
		else
		{
			m_CamCrashUpDown.m_fCrashTime = 0f;
			m_CamCrashUpDown.m_fCrashGap = 0f;
		}
	}

	public static void UpCrashCamera(float fSpeed = 100f, float fAccel = -1500f)
	{
		m_CamCrashUp.m_fCrashGap = 0f;
		m_CamCrashUp.m_fCrashSpeed = fSpeed;
		m_CamCrashUp.m_fCrashAccel = fAccel;
	}

	public static void DownCrashCamera(float fSpeed = -100f, float fAccel = 1500f)
	{
		m_CamCrashDown.m_fCrashGap = 0f;
		m_CamCrashDown.m_fCrashSpeed = fSpeed;
		m_CamCrashDown.m_fCrashAccel = fAccel;
	}

	public static void UpDownCrashCamera(float fGap = 10f, float fTime = 0.2f, float fCrashTimeGapOrg = 0.05f)
	{
		if (!(m_CamCrashUpDown.m_fCrashTime > 0f) || !(fGap < m_CamCrashUpDown.m_fCrashGap))
		{
			m_CamCrashUpDown.m_fCrashGap = fGap;
			m_CamCrashUpDown.m_fCrashTime = fTime;
			m_CamCrashUpDown.m_fCrashTimeGap = fCrashTimeGapOrg;
			m_CamCrashUpDown.m_fCrashTimeGapOrg = fCrashTimeGapOrg;
		}
	}

	public static void TurnOffCrashUpDown()
	{
		m_CamCrashUpDown.m_fCrashGap = 0f;
		m_CamCrashUpDown.m_fCrashTime = 0f;
		m_CamCrashUpDown.m_fCrashTimeGap = 0f;
	}

	public static void UpDownCrashCameraNoReset(float fGap = 10f, float fTime = 0.2f)
	{
		m_CamCrashUpDown.m_fCrashGap = fGap;
		m_CamCrashUpDown.m_fCrashTime = fTime;
		m_CamCrashUpDown.m_fCrashTimeGapOrg = 0.05f;
	}

	private static void InitCrashCamera()
	{
		m_CamCrashDown.m_fCrashGap = 0f;
		m_CamCrashDown.m_fCrashSpeed = 0f;
		m_CamCrashDown.m_fCrashAccel = 0f;
		m_CamCrashUp.m_fCrashGap = 0f;
		m_CamCrashUp.m_fCrashSpeed = 0f;
		m_CamCrashUp.m_fCrashAccel = 0f;
		m_CamCrashUpDown.m_fCrashTime = 0f;
		m_CamCrashUpDown.m_fCrashGap = 0f;
	}

	public static void TrackingPos(float fTrackingTime, float fX = -1f, float fY = -1f, float fZ = -1f, int priority = 0)
	{
		if (m_BattlePriority <= priority || !m_NKMTrackingPos.IsTracking())
		{
			m_BattlePriority = priority;
			if (fX == -1f)
			{
				fX = m_NKMTrackingPos.GetNowValue().x;
			}
			if (fY == -1f)
			{
				fY = m_NKMTrackingPos.GetNowValue().y;
			}
			if (fZ == -1f)
			{
				fZ = m_NKMTrackingPos.GetNowValue().z;
			}
			m_NKMTrackingPos.SetTracking(fX, fY, fZ, fTrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
	}

	public static void TrackingPosRel(float fTrackingTime, float fX, float fY, float fZ)
	{
		fX = m_NKMTrackingPos.GetNowValue().x + fX;
		fY = m_NKMTrackingPos.GetNowValue().y + fY;
		fZ = m_NKMTrackingPos.GetNowValue().z + fZ;
		m_NKMTrackingPos.SetTracking(fX, fY, fZ, fTrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	public static void TrackingZoom(float fTrackingTime, float fZoomSize)
	{
		m_NKMTrackingZoom.SetTracking(fZoomSize, fTrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	public static void GetScreenPosToWorldPos(out Vector3 worldPos, float fScreenX, float fScreenY)
	{
		m_CamPosTemp.Set(fScreenX, fScreenY, m_SCEN_MAIN_Camera_Camera.nearClipPlane);
		worldPos = m_SCEN_MAIN_Camera_Camera.ScreenToWorldPoint(m_CamPosTemp);
	}

	public static void GetWorldPosToScreenPos(out Vector3 screenPos, float fWorldX, float fWorldY, float fWorldZ)
	{
		m_CamPosTemp.Set(fWorldX, fWorldY, fWorldZ);
		screenPos = m_SCEN_SUB_UI_Camera_Camera.ScreenToWorldPoint(m_SCEN_MAIN_Camera_Camera.WorldToScreenPoint(m_CamPosTemp));
	}

	public static Rect GetCameraBoundRect(RectTransform rect, float CameraZPos)
	{
		Vector3 CenterWorldPos;
		Rect worldRect = GetWorldRect(rect, out CenterWorldPos);
		Vector3 vector = new Vector3(CenterWorldPos.x, CenterWorldPos.y, CameraZPos);
		Rect result = default(Rect);
		if (m_SCEN_MAIN_Camera_Camera.orthographic)
		{
			result.height = worldRect.height - m_SCEN_MAIN_Camera_Camera.orthographicSize * 2f;
			float num = GetScreenRatio(bSafeRect: false) * m_SCEN_MAIN_Camera_Camera.orthographicSize * 2f;
			result.width = worldRect.width - num;
		}
		else
		{
			float num2 = Mathf.Abs(CenterWorldPos.z - CameraZPos);
			float num3 = Mathf.Tan((float)Math.PI / 180f * m_SCEN_MAIN_Camera_Camera.fieldOfView * 0.5f) * num2 * 2f;
			float num4 = GetScreenRatio(bSafeRect: false) * num3;
			result.height = worldRect.height - num3;
			result.width = worldRect.width - num4;
		}
		if (result.height < 0f)
		{
			result.height = 0f;
		}
		if (result.width < 0f)
		{
			result.width = 0f;
		}
		result.center = vector;
		return result;
	}

	public static void FitCameraToRect(Camera camera, RectTransform rect)
	{
		Vector3[] array = new Vector3[4];
		rect.GetWorldCorners(array);
		FitCameraToWorldRect(camera, array);
	}

	public static void FitCameraToWorldRect(Camera camera, Vector3[] WorldCornerPosArray)
	{
		Vector3 vector = WorldCornerPosArray[1];
		Vector3 vector2 = WorldCornerPosArray[2];
		Vector3 vector3 = WorldCornerPosArray[3];
		Vector3 vector4 = (vector + vector3) * 0.5f;
		if (camera.orthographic)
		{
			camera.transform.position = new Vector3(vector4.x, vector4.y, Mathf.Min(vector.z, vector3.z) - 10f);
			float num = vector2.x - vector.x;
			float num2 = vector2.y - vector3.y;
			camera.orthographicSize = (vector2.y - vector3.y) / 2f;
			camera.aspect = num / num2;
		}
		else
		{
			Vector3 normalized = Vector3.Cross(vector - vector4, vector2 - vector4).normalized;
			float num3 = (vector.y - vector4.y) / Mathf.Tan((float)Math.PI / 180f * camera.fieldOfView * 0.5f);
			Vector3 position = vector4 + normalized * num3;
			float num4 = vector2.x - vector.x;
			float num5 = vector2.y - vector3.y;
			camera.aspect = num4 / num5;
			camera.transform.position = position;
			camera.transform.LookAt(vector4);
		}
	}

	public static void FitRectToCamera(RectTransform rect)
	{
		float screenRatio = GetScreenRatio(bSafeRect: false);
		if (screenRatio > 1f)
		{
			rect.SetWidth(screenRatio * rect.GetHeight());
		}
		Vector3[] array = new Vector3[4];
		rect.GetWorldCorners(array);
		Vector3 vector = array[0];
		float num = (array[1].y - vector.y) * 0.5f / Mathf.Tan((float)Math.PI / 180f * m_SCEN_MAIN_Camera_Camera.fieldOfView * 0.5f);
		rect.position = m_SCEN_MAIN_Camera_Camera.transform.position + m_SCEN_MAIN_Camera_Camera.transform.forward * num;
	}

	public static Rect GetCameraRectOnRectTransform(RectTransform rect, Camera Camera, float CameraWorldZPos)
	{
		Rect result = default(Rect);
		if (rect == null)
		{
			return result;
		}
		GetWorldRect(rect, out var CenterWorldPos);
		if (Camera.orthographic)
		{
			result.height = Camera.orthographicSize * 2f;
			result.width = GetScreenRatio(bSafeRect: false) * result.height;
			result.center = CenterWorldPos;
		}
		else
		{
			float num = Mathf.Abs(CenterWorldPos.z - CameraWorldZPos);
			float num2 = Mathf.Tan((float)Math.PI / 180f * Camera.fieldOfView * 0.5f) * num * 2f;
			float width = GetScreenRatio(bSafeRect: false) * num2;
			result.width = width;
			result.height = num2;
			result.center = CenterWorldPos;
		}
		return result;
	}

	private static Rect GetWorldRect(RectTransform rect, out Vector3 CenterWorldPos)
	{
		if (rect == null)
		{
			CenterWorldPos = Vector3.zero;
			return default(Rect);
		}
		Vector3[] array = new Vector3[4];
		rect.GetWorldCorners(array);
		Vector3 vector = array[1];
		Vector3 vector2 = array[2];
		Vector3 vector3 = array[3];
		CenterWorldPos = (vector + vector3) * 0.5f;
		float width = Mathf.Abs(vector2.x - vector.x);
		float height = Mathf.Abs(vector2.y - vector3.y);
		return new Rect
		{
			width = width,
			height = height,
			center = CenterWorldPos
		};
	}

	public static void RescaleRectToCameraFrustrum(RectTransform rect, Camera targetCamera, Vector2 CameraMoveRectSize, float farthestZPosition, FitMode fitMode = FitMode.FitAuto, ScaleMode scaleMode = ScaleMode.Scale)
	{
		if (rect == null)
		{
			return;
		}
		Vector3 CenterWorldPos;
		Rect worldRect = GetWorldRect(rect, out CenterWorldPos);
		Rect cameraRectOnRectTransform = GetCameraRectOnRectTransform(rect, targetCamera, farthestZPosition);
		Vector2 vector = new Vector2
		{
			x = cameraRectOnRectTransform.width + CameraMoveRectSize.x,
			y = cameraRectOnRectTransform.height + CameraMoveRectSize.y
		};
		Vector2 newSize = default(Vector2);
		switch (fitMode)
		{
		case FitMode.FitToScreen:
			newSize.x = vector.x;
			newSize.y = vector.y;
			break;
		case FitMode.FitToWidth:
		{
			float num4 = worldRect.width / worldRect.height;
			newSize.x = vector.x;
			newSize.y = newSize.x / num4;
			break;
		}
		case FitMode.FitToHeight:
		{
			float num2 = worldRect.width / worldRect.height;
			newSize.y = vector.y;
			newSize.x = newSize.y * num2;
			break;
		}
		case FitMode.FitIn:
		{
			float num3 = worldRect.width / worldRect.height;
			if (worldRect.width / vector.x > worldRect.height / vector.y)
			{
				newSize.x = vector.x;
				newSize.y = newSize.x / num3;
			}
			else
			{
				newSize.y = vector.y;
				newSize.x = newSize.y * num3;
			}
			break;
		}
		default:
		{
			float num = worldRect.width / worldRect.height;
			if (worldRect.width / vector.x > worldRect.height / vector.y)
			{
				newSize.y = vector.y;
				newSize.x = newSize.y * num;
			}
			else
			{
				newSize.x = vector.x;
				newSize.y = newSize.x / num;
			}
			break;
		}
		}
		switch (scaleMode)
		{
		case ScaleMode.Scale:
			rect.localScale = new Vector3
			{
				x = newSize.x / rect.GetWidth(),
				y = newSize.y / rect.GetHeight(),
				z = rect.localScale.z
			};
			break;
		case ScaleMode.RectSize:
			rect.SetSize(newSize);
			break;
		}
	}

	public static Vector3 CameraPositionOfFitRectHeight(RectTransform rect, Camera targetCamera)
	{
		float num = rect.GetHeight() * rect.lossyScale.y;
		float num2 = Mathf.Tan(targetCamera.fieldOfView * 0.5f * ((float)Math.PI / 180f));
		float f = rect.eulerAngles.x * ((float)Math.PI / 180f);
		float num3 = -0.5f * num * Mathf.Cos(f) / num2;
		float num4 = -0.5f * num * Mathf.Sin(f) * num2;
		return new Vector3(rect.position.x, rect.position.y + num4, rect.position.z + num3);
	}
}
