using System;
using System.Runtime.InteropServices;

public static class NativeWinAlert
{
	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	public static IntPtr GetWindowHandle()
	{
		return GetActiveWindow();
	}

	[DllImport("user32.dll", SetLastError = true)]
	public static extern int MessageBox(IntPtr hwnd, string lpText, string lpCaption, uint uType);
}
