using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NKC;

public class NKCMultiClientPrevent : MonoBehaviour
{
	public static class Import
	{
		public const int ERROR_ALREADY_EXISTS = 183;

		public const int ERROR_INVALID_HANDLE = 6;

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateMutexW(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

		[DllImport("kernel32.dll")]
		public static extern bool ReleaseMutex(IntPtr hMutex);

		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);
	}

	private IntPtr m_hMutex = IntPtr.Zero;

	private void Start()
	{
		Debug.Log("NKCMultiClientPrevent Start");
		m_hMutex = Import.CreateMutexW(IntPtr.Zero, bInitialOwner: false, "CounterSideNxkPC");
		int lastWin32Error = Marshal.GetLastWin32Error();
		if (m_hMutex == IntPtr.Zero || (m_hMutex != IntPtr.Zero && (lastWin32Error == 183 || lastWin32Error == 6)))
		{
			Application.Quit();
		}
		else
		{
			Debug.Log("Import.GetLastError() : " + lastWin32Error);
		}
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		Debug.Log("NKCMultiClientPrevent OnDestroy");
		if (m_hMutex != IntPtr.Zero)
		{
			Import.ReleaseMutex(m_hMutex);
		}
		if (m_hMutex != IntPtr.Zero)
		{
			Import.CloseHandle(m_hMutex);
		}
		m_hMutex = IntPtr.Zero;
	}
}
