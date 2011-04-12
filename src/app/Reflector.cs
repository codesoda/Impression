using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CodeSoda.Impression
{
	public interface IReflector {
		string AsString(object obj);
		bool AsBoolean(object obj);
		object Eval(object context, string lookup );
		object Eval(object context, string[] lookups);
		bool TryGetProperty(object context, string propertyName, Type contextType, out object value);
		bool TryGetField(object context, string fieldName, Type contextType, out object value);
		bool TryGetterMethod(object context, string getterName, Type contextType, out object value);
		bool TryGetDictionaryValue(object context, string keyName, Type contextType, out object value);
	}

	public class Reflector : IReflector {

		public string AsString(object obj)
		{
			if (obj == null)
				return "";

			string retVal = obj.ToString();

			// bool
			if (obj is bool)
			{
				retVal = retVal.ToLower();
			}

			return retVal;
		}

		public bool AsBoolean(object obj)
		{
			if (obj == null)
				return false;

			bool retVal = false; // todo check if there is some kinda default value specified in attributes

			// bool
			if (obj is bool)
			{
				retVal = (bool)obj;
			}

			// strings
			else if (obj is string)
			{
				if (!Boolean.TryParse((string)obj, out retVal))
				{
					double tempNumber = 0;
					if (!Double.TryParse((string)obj, out tempNumber))
					{
						retVal = !string.IsNullOrEmpty((string)obj);
					}
					else
					{
						retVal = tempNumber > 0;
					}
				}
			}

			// numbers
			else if (obj is int)
			{
				retVal = (int)obj > 0;
			}
			else if (obj is double)
			{
				retVal = (double)obj > 0;
			}
			else if (obj is float)
			{
				retVal = (float)obj > 0;
			}
			else if (obj is decimal)
			{
				retVal = (decimal)obj > 0;
			}

			// arrays, lists, etc
			else if (obj is IEnumerable)
			{
				retVal = ((IEnumerable)obj).GetEnumerator().MoveNext();
			}

			// convert using dot net
			// TODO move to start (but after bool)
			else if (obj.GetType().GetInterface("IConvertible") != null)
			{
				//try {
				retVal = Convert.ToBoolean(obj);
				//}catch {}
			}

			// catch all, anything not null is true
			else
			{
				retVal = obj != null;
			}
			return retVal;
		}

		private object EvalField(object context, string lookup) {
			if (context == null || string.IsNullOrEmpty(lookup))
				return context;

			Type contextType = context.GetType();
			Object value = null;
			// look up a property on the bag object 

			// try lookup a property
			if (!TryGetProperty(context, lookup, contextType, out value))
			{
				// we couldn't find a property, try a getter
				if (!TryGetterMethod(context, lookup, contextType, out value))
				{
					// we couldn't find a getter, try a field
					if (!TryGetField(context, lookup, contextType, out value))
					{
						// couldn't find a field, try a dictionary
						TryGetDictionaryValue(context, lookup, contextType, out value);
					}
				}
			}

			return value;
		}

		public object Eval(object context, string lookup ) {

			string[] lookups = lookup.Split(new char[] {'.'});

			if (lookups.Length > 1)
				return Eval(context, lookups);
			else
				return EvalField(context, lookup);
		}

		public object Eval(object context, string[] lookups) {

			if (context == null || lookups == null || lookups.Length == 0)
				return context;

			foreach (string lookup in lookups) {
				context = EvalField(context, lookup.Trim());

				if (context == null) break;
			}

			return context;
		}

		public bool TryGetProperty(object context, string propertyName, Type contextType, out object value) {

			bool found = false;
			PropertyInfo propInfo = contextType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly);
			if (propInfo != null) {
				found = true;
                try
                {
                    value = propInfo.GetValue(context, null);
                }
                catch(TargetInvocationException ex)
                {
                    if (ex.InnerException != null)
                        throw ex.InnerException;
                    else
                        throw new ApplicationException("could not lookup property: " + propertyName);
                }
			} else {
				value = null;
			}

			return found;
		}

		public bool TryGetField(object context, string fieldName, Type contextType, out object value)
		{
			bool found = false;
			FieldInfo fieldInfo = contextType.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			if (fieldInfo != null) {
				found = true;
                try
                {
                    value = fieldInfo.GetValue(context);
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException != null)
                        throw ex.InnerException;
                    else
                        throw new ApplicationException("could not lookup field: " + fieldName);
                }
			} else {
				value = null;
			}
			return found;
		}

		public bool TryGetterMethod(object context, string getterName, Type contextType, out object value) {
			bool found = false;
			MethodInfo methodInfo = contextType.GetMethod("get_" + getterName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			if (methodInfo != null) {
				found = true;
                try
                {
                    value = methodInfo.Invoke(context, null);
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException != null)
                        throw ex.InnerException;
                    else
                        throw new ApplicationException("could not lookup getter: " + getterName);
                }
			} else {
				value = null;
			}
			return found;
		}

		public bool TryGetDictionaryValue(object context, string keyName, Type contextType, out object value) {

			keyName = keyName.ToLower();

			value = null;
			bool found = false;
			if (contextType.GetInterface("IDictionary") != null)
			{
				IDictionary dict = (IDictionary) context;
				
				// force lazy dictionary loading
				try
				{
					object obj = dict[keyName];
					if (obj != null)
					{
						value = obj;
						found = true;
					}
				}
				catch {}

				if (!found) {
					string checkKey = keyName.ToLower();
					foreach (object key in dict.Keys)
					{
						if (key.ToString().ToLower() == checkKey)
						{
							value = dict[key];
							found = true;
							break;
						}
					}
				}
			} else if (contextType.GetInterface("IDictionary<>") != null)
			{
				//IDictionary<> dict = (IDictionary<>)context;
				throw new Exception("Cannot read values from Generic Dictionaries.");
			} 

			return found;
		}

		//private Type GetGenericDictionary(Type contextType, object context)
		//{
			
		//}
	}
}
