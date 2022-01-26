using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class EventManager
{
	private static Dictionary<string, List<FuncRef>> listeners = new Dictionary<string, List<FuncRef>>();

	public static void ListenEvent(string @event, FuncRef callback)
	{
		if (!listeners.ContainsKey(@event))
			listeners[@event] = new List<FuncRef>();

		listeners[@event].Add(callback);
	}

	public static void IgnoreEvent(string @event, FuncRef callback)
	{
		if (listeners.ContainsKey(@event))
		{
			listeners[@event].Remove(callback);
		}
	}

	public static void RaiseEvent(string @event, params object[] args)
	{
		if (listeners.ContainsKey(@event))
		{
			foreach (FuncRef callback in listeners[@event])
			{
				callback.CallFunc(args);
			}
		}
	}

	public static void ClearEvents()
	{
		listeners.Clear();
	}
}
