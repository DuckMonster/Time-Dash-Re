using System;
using System.Collections.Generic;

class Garbage
{
	static List<IDisposable> disposeList = new List<IDisposable>();

	public static void Dispose()
	{
		foreach (IDisposable disp in disposeList) if (disp != null) disp.Dispose();
	}

	public static void Add(IDisposable disp)
	{
		disposeList.Add(disp);
	}
}