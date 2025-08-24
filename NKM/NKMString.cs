using System.Text;
using System.Threading;

namespace NKM;

public static class NKMString
{
	private static readonly ThreadLocal<StringBuilder> perThreadBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder());

	public static StringBuilder GetBuilder()
	{
		StringBuilder value = perThreadBuilder.Value;
		value.Clear();
		return value;
	}
}
