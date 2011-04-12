using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace CodeSoda.Impression
{

	public enum IfTagType {
		If,
		ElseIf,
		Else,
		EndIf
	}
	/// <summary>
	/// An ImpressionTag represents a functional piece of markup within a template
	/// </summary>
	public class IfTagMarkup : TagMarkupBase
	{
		#region Members

		//private static Stack<bool> CurrentIf = new Stack<bool>();
		private readonly IfTagType TagType;

		#endregion

		#region Methods

		internal override void Interpret(IInterpretContext ctx)
		{
			if (!ctx.State.ContainsKey("CurrentIf")) {
				ctx.State.Add( "CurrentIf", new Stack<bool>() );
			}
			Stack<bool> CurrentIf = (Stack<bool>)ctx.State["CurrentIf"];

			Stack<object> CurrentTag = ctx.CurrentTag;
			IfTagType? currentIfTagType = CurrentTag.Count > 0 && CurrentTag.Peek() is IfTagType
				? (IfTagType?)CurrentTag.Peek()
				: null;

			bool val = false;
			int ifDepth = 0;
			bool found = false;

			switch (this.TagType) {
				case IfTagType.If:

					val = this.Expression.EvaluateAsBoolean(ctx);
					CurrentTag.Push(IfTagType.If);
					CurrentIf.Push(val);

					if (val) {
						ctx.MoveNext();
					}
					else
					{
						// move forward to the next ELSE or ELSE IF or ENDIF
						found = false;
						ifDepth = 0;
						do
						{
							ctx.MoveNext();
							MarkupBase m = ctx.CurrentMarkup;

							if (m is IfTagMarkup)
							{
								IfTagType loopIfTagType = ((IfTagMarkup) m).TagType;
								if (loopIfTagType == IfTagType.If) {
									ifDepth++;
								} else {
									if (ifDepth > 0)
									{
										// ignore tags that are either an elseif or else
										// if its an endif, then decrement the ifDepth
										if (loopIfTagType == IfTagType.EndIf) {
											ifDepth--;
										}
									} else {
										found = (
											loopIfTagType == IfTagType.ElseIf
											|| loopIfTagType == IfTagType.Else
											|| loopIfTagType == IfTagType.EndIf
										);
									}
								}
							}

						} while (!found || ctx.ListPosition >= ctx.ParseList.Count);

					}

				break;

			case IfTagType.ElseIf:

				// make sure we have a logically correct parent
				if (currentIfTagType == null
					|| (
						currentIfTagType != IfTagType.If
						&& currentIfTagType != IfTagType.ElseIf
					)
				) {
					throw new ImpressionInterpretException("Expected an IF or ELSEIF before ELSE", this);
				}

				// if the current IF was false, process this ELSEIF
				if (!CurrentIf.Peek()) {
					val = this.Expression.EvaluateAsBoolean(ctx);

					// remove current tag, add "elseif"
					CurrentTag.Pop();
					CurrentTag.Push(IfTagType.ElseIf);

					if (val) {
						// add us to the if
						CurrentIf.Pop();
						CurrentIf.Push(val);
						ctx.MoveNext();
						return;
					}
				}

				// move forward to the next ELSE IF or ENDIF
				found = false;
				ifDepth = 0;
				do {
					ctx.MoveNext();
					MarkupBase m = ctx.CurrentMarkup;
					if (m is IfTagMarkup) {
						IfTagType loopIfTagType = ((IfTagMarkup) m).TagType;

						// if its an if, then increment the ifDepth
						if (loopIfTagType == IfTagType.If)
						{
							ifDepth++;
						}
						else
						{
							if (ifDepth > 0)
							{
								// ignore tags that are either an elseif or else
								// if its an endif, then decrement the ifDepth
								if (loopIfTagType == IfTagType.EndIf)
								{
									ifDepth--;
								}
							}
							else
							{
								found = (loopIfTagType == IfTagType.ElseIf ||
										loopIfTagType == IfTagType.Else ||
										loopIfTagType == IfTagType.EndIf);
							}
						}
					}
				} while (!found || ctx.ListPosition >= ctx.ParseList.Count);

				break;

			case IfTagType.Else:

				// make sure we have a logically correct parent
				if (currentIfTagType == null ||
						(currentIfTagType != IfTagType.If
						&& currentIfTagType != IfTagType.ElseIf)) {
					throw new ImpressionInterpretException("Expected an IF or ELSEIF before ELSE", this);
				}

				// if the last if wasn't true then check this otherwise, just skip
				if (!CurrentIf.Peek()) {
					// rejig the if
					CurrentIf.Push(!CurrentIf.Pop());

					// remove current tag, add "else"
					CurrentTag.Pop();
					CurrentTag.Push(IfTagType.Else);

					// move to the next markup
					ctx.MoveNext();
				}
				else {
					// move to the next markup
					//ctx.MoveNext();

					// move forward to the next ENDIF at this level 
					found = false;
					ifDepth = 0;
					do {
						ctx.MoveNext();
						MarkupBase m = ctx.CurrentMarkup;

						if (m is IfTagMarkup) {
							IfTagType loopIfTagType = ((IfTagMarkup)m).TagType;
							// if its an if, then increment the ifDepth
							if (loopIfTagType == IfTagType.If) {
								ifDepth++;
							}
							else {
								if (ifDepth > 0) {
									// ignore tags that are either an elseif or else
									// if its an endif, then decrement the ifDepth
									if (loopIfTagType == IfTagType.EndIf) {
										ifDepth--;
									}
								}
								else {
									found = loopIfTagType == IfTagType.EndIf;
								}
							}
						}
					} while (!found || ctx.ListPosition >= ctx.ParseList.Count);
				}

				break;

			case IfTagType.EndIf:

					// make sure we have a logically correct parent
					if (currentIfTagType == null ||
						(currentIfTagType != IfTagType.If
						&& currentIfTagType != IfTagType.ElseIf
						&& currentIfTagType != IfTagType.Else)) {
							throw new ImpressionInterpretException("Expected an IF, ELSEIF or ELSE before ENDIF", this);
					}

					// rejig the if
					CurrentIf.Pop();

					// remove current tag
					CurrentTag.Pop();

					ctx.MoveNext();
				break;
			}
		}

		#endregion

		#region Constructors

		public IfTagMarkup(IfTagType tagType, ExpressionMarkup expression, string markup, int lineNumber, int charPos)
			: base(expression, markup, lineNumber, charPos)
		{
			this.TagType = tagType;
		}

		#endregion
	}
}
