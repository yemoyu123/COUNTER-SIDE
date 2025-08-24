using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BehaviorDesigner.Runtime;

public abstract class BTSharedNKCValue : SharedVariable
{
	public abstract bool TryParse(string paramater);

	public static string ToDebugString<T>(IEnumerable<T> target)
	{
		if (target == null)
		{
			return "null";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(target.GetType().ToString());
		stringBuilder.Append("[");
		bool flag = false;
		foreach (T item in target)
		{
			stringBuilder.Append(item.ToString());
			stringBuilder.Append(", ");
			flag = true;
		}
		if (flag)
		{
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
public abstract class BTSharedNKCValue<T> : BTSharedNKCValue
{
	private Func<T> mGetter;

	private Action<T> mSetter;

	[SerializeField]
	protected T mValue;

	public T Value
	{
		get
		{
			if (mGetter != null)
			{
				return mGetter();
			}
			return mValue;
		}
		set
		{
			if (mSetter != null)
			{
				mSetter(value);
			}
			else
			{
				mValue = value;
			}
		}
	}

	public override void InitializePropertyMapping(BehaviorSource behaviorSource)
	{
		if (!BehaviorManager.IsPlaying || !(behaviorSource.Owner.GetObject() is Behavior) || string.IsNullOrEmpty(base.PropertyMapping))
		{
			return;
		}
		string[] array = base.PropertyMapping.Split('/');
		GameObject gameObject = null;
		try
		{
			gameObject = (object.Equals(base.PropertyMappingOwner, null) ? (behaviorSource.Owner.GetObject() as Behavior).gameObject : base.PropertyMappingOwner);
		}
		catch (Exception)
		{
			Behavior behavior = behaviorSource.Owner.GetObject() as Behavior;
			if (behavior != null && behavior.AsynchronousLoad)
			{
				Debug.LogError("Error: Unable to retrieve GameObject. Properties cannot be mapped while using asynchronous load.");
				return;
			}
		}
		if (gameObject == null)
		{
			Debug.LogError("Error: Unable to find GameObject on " + behaviorSource.behaviorName + " for property mapping with variable " + base.Name);
			return;
		}
		Component component = gameObject.GetComponent(TaskUtility.GetTypeWithinAssembly(array[0]));
		if (component == null)
		{
			Debug.LogError("Error: Unable to find component on " + behaviorSource.behaviorName + " for property mapping with variable " + base.Name);
			return;
		}
		PropertyInfo property = component.GetType().GetProperty(array[1]);
		if ((object)property != null)
		{
			MethodInfo getMethod = property.GetGetMethod();
			if ((object)getMethod != null)
			{
				mGetter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), component, getMethod);
			}
			getMethod = property.GetSetMethod();
			if ((object)getMethod != null)
			{
				mSetter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), component, getMethod);
			}
		}
	}

	public override object GetValue()
	{
		return Value;
	}

	public override void SetValue(object value)
	{
		if (mSetter != null)
		{
			mSetter((T)value);
		}
		else
		{
			mValue = (T)value;
		}
	}

	public override string ToString()
	{
		if (Value == null)
		{
			return "(null)";
		}
		return Value.ToString();
	}
}
