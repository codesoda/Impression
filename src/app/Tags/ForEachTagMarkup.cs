using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace CodeSoda.Impression
{

	public enum ForEachTagType
	{
		ForEach,
		Next
	}

	/// <summary>
	/// An ImpressionTag represents a functional piece of markup within a template
	/// </summary>
	public class ForEachTagMarkup : TagMarkupBase
	{
		#region Members

		public ForEachTagType TagType;

		#endregion

		#region Constructors

		public ForEachTagMarkup(ForEachTagType tagType, ExpressionMarkup expression, string markup, int lineNumber, int charPos)
			: base(expression, markup, lineNumber, charPos) {
			this.TagType = tagType;
		}

		#endregion

		internal override void Interpret(IInterpretContext ctx)
		{
			Stack<object> CurrentTag = ctx.CurrentTag;
			ForEachTagType? currentForEachTagType = CurrentTag.Count > 0 && CurrentTag.Peek() is ForEachTagType
				? (ForEachTagType?)CurrentTag.Peek()
				: null;

			if (!ctx.State.ContainsKey("CurrentForEach_EnumeratorKey"))
			{
				ctx.State.Add("CurrentForEach_EnumeratorKey", new Stack<String>());
			}
			Stack<String> currentEnumeratorKey = (Stack<String>)ctx.State["CurrentForEach_EnumeratorKey"];

			if (!ctx.State.ContainsKey("CurrentForEach_Enumerator"))
			{
				ctx.State.Add("CurrentForEach_Enumerator", new Stack<IEnumerator>());
			}
			Stack<IEnumerator> currentEnumerator = (Stack<IEnumerator>)ctx.State["CurrentForEach_Enumerator"];

			if (!ctx.State.ContainsKey("CurrentForEach_EnumeratorIndex"))
			{
				ctx.State.Add("CurrentForEach_EnumeratorIndex", new Stack<int>());
			}
			Stack<int> currentEnumeratorIndex = (Stack<int>)ctx.State["CurrentForEach_EnumeratorIndex"];

			if (!ctx.State.ContainsKey("CurrentForEach_EnumeratorStartIndex"))
			{
				ctx.State.Add("CurrentForEach_EnumeratorStartIndex", new Stack<int>());
			}
			Stack<int> currentEnumeratorStartIndex = (Stack<int>)ctx.State["CurrentForEach_EnumeratorStartIndex"];


			switch (this.TagType)
			{
				case ForEachTagType.ForEach:

					// lookup the object we are foreach-ing
					object obj = this.Expression.Evaluate(ctx);

					// make sure the obj inherits IEnumerable
					if (obj != null && obj.GetType().GetInterface("IEnumerable") != null)
					{
						// get an enumerator to loop with
						IEnumerator en = ((IEnumerable)obj).GetEnumerator();

						// move to first item
						if (en.MoveNext())
						{
							// add the current iteration to the bag as "Within"
							ctx.Bag[this.Expression.Within] = en.Current;

							// save the current within
							currentEnumeratorKey.Push(this.Expression.Within);
							currentEnumeratorIndex.Push(1);
							ctx.Bag[currentEnumeratorKey.Peek() + ".Position"] = 1;

							// add the enumerator to the stack
							currentEnumerator.Push(en);

							// save the position this enumeration starts at
							ctx.MoveNext();
							currentEnumeratorStartIndex.Push(ctx.ListPosition);
							CurrentTag.Push(ForEachTagType.ForEach);
							return;
						}

						//// add the enumerator to the stack
						//currentEnumerator.Push(en);

						//// save the position this enumeration starts at
						//ctx.MoveNext();
						//currentEnumeratorStartIndex.Push(ctx.ListPosition);
						//CurrentTag.Push(ForEachTagType.ForEach);
						//return;
						
					}

					// not enumerable, null enumerator or unable to move next indicates no items in data
					// find the next "NEXT" but track how many foreach's there were
					int toFind = 1;
					int found = 0;
					do
					{
						ctx.MoveNext();
						MarkupBase m = ctx.CurrentMarkup;
						if (m is ForEachTagMarkup) 
						{
							ForEachTagType loopForEachTagType = ((ForEachTagMarkup)m).TagType;
							if (loopForEachTagType == ForEachTagType.Next)
							{
								found++;
							}
							else if (loopForEachTagType == ForEachTagType.ForEach)
							{
								toFind++;
							}
						}
					} while (found < toFind || ctx.ListPosition >= ctx.ParseList.Count);
					ctx.MoveNext();
					break;


				case ForEachTagType.Next:
					
					IEnumerator enumerator = (currentEnumerator.Count > 0 ? currentEnumerator.Peek() : null);
					string key = (currentEnumeratorKey.Count > 0 ? currentEnumeratorKey.Peek() : null);

					// make sure that the current tag is a foreach
					if (currentForEachTagType == null || currentForEachTagType != ForEachTagType.ForEach) {
						throw new ImpressionInterpretException("NEXT tag detected without a corresponding FOREACH Tag", this);
					}
					
					// get the current enumerator
					if (enumerator != null)
					{
						if (enumerator.MoveNext())
						{
							// there is another record in the enumerator
							ctx.Bag[currentEnumeratorKey.Peek()] = enumerator.Current;
							int position = currentEnumeratorIndex.Pop() + 1;
							currentEnumeratorIndex.Push(position);
							ctx.Bag[currentEnumeratorKey.Peek() + ".Position"] = position;
							//ctx.Bag["Position"] = position;
							ctx.MoveTo(currentEnumeratorStartIndex.Peek());
						}
						else
						{
							// we're finished the enumeration, clean up
							// but make sure there is something on the stacks first, a list of
							// nothing will not get a position or key pushed onto stack
							if (currentEnumeratorKey.Count > 0)
							{
								string currentKey = currentEnumeratorKey.Peek();
								string positionKey = currentKey + ".Position";
								ctx.Bag.Remove(positionKey);
								ctx.Bag.Remove(currentKey);
								currentEnumeratorIndex.Pop();
								currentEnumeratorKey.Pop();
							}

							currentEnumerator.Pop();
							currentEnumeratorStartIndex.Pop();
							CurrentTag.Pop();
							ctx.MoveNext();
						}
					} else {
						throw new ImpressionInterpretException("Next tag detected without a corresponding Enumerator", this);
					}

					break;
			}
		}
	}
}
