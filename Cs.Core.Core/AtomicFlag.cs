using System.Diagnostics;
using System.Threading;

namespace Cs.Core.Core;

[DebuggerDisplay("value = {value}")]
public sealed class AtomicFlag
{
	private volatile int value = 1;

	public bool IsOn => value == 1;

	public AtomicFlag(bool initialValue)
	{
		value = (initialValue ? 1 : 0);
	}

	public bool On()
	{
		return Interlocked.CompareExchange(ref value, 1, 0) == 0;
	}

	public bool Off()
	{
		return Interlocked.CompareExchange(ref value, 0, 1) == 1;
	}
}
