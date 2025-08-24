public class CallBackMethod
{
	public delegate void VoidDelegate();

	public delegate void NKAServiceDelegate<T>(T info) where T : class;

	public delegate void NKAServiceReadOnlyDelegate<T>(in T readonlyInfo) where T : struct;
}
