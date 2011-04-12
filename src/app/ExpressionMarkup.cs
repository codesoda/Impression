using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using CodeSoda.Impression.Parsers;
using CodeSoda.Impression.Properties;

namespace CodeSoda.Impression
{
	public class ExpressionMarkup : MarkupBase
	{
		// expression type,
		// single data object
		// complex expression

		private IReflector reflector = null;
		private IFilterRunner filterRunner = null;

		public override MarkupType MarkupType
		{
			get { return MarkupType.Expression; }
		}

		internal override void Interpret(IInterpretContext ctx)
		{
			// this is a literal, simply append our content and increment position in parselist
			ctx.Builder.Append(this.EvaluateAsString(ctx));
			ctx.MoveNext();
		}

		public string ObjectName { get; private set; }

		public string Within { get; private set; }

		public string[] Properties { get; private set; }

		public string[] Attributes { get; private set; }

		public ExpressionMarkup(IReflector reflector, IFilterRunner filterRunner, string markup, int lineNumber, int charPos ):base(markup, lineNumber, charPos) {
			
			if (reflector == null)
				throw new ArgumentNullException("reflector");
			this.reflector = reflector;

			if (filterRunner == null)
				throw new ArgumentNullException("filterRunner");
			this.filterRunner = filterRunner;


			// make sure this markup starts with <!-- and ends with -->
			if (!parseMarkupX()) {
				throw new ImpressionParseException("Invalid markup detected, " + markup + "( Line: " + lineNumber + ", Char: " + charPos + " )");
			}
		}

		public ExpressionMarkup(IReflector reflector, IFilterRunner filterRunner, string markup)
			: this(reflector, filterRunner, markup, -1, -1) {}

		public bool EvaluateAsBoolean(IInterpretContext ctx)
		{
			// todo check if there is some kinda default value specified in attributes
			object obj = Evaluate(ctx);

			return reflector.AsBoolean(obj);

		}

		public string EvaluateAsString(IInterpretContext ctx)
		{
			object obj = Evaluate(ctx);

			return obj != null
				? reflector.AsString(obj)
				: (ImpressionEngine.EmptySubs.Contains("{0}")
					? string.Format(ImpressionEngine.EmptySubs, this.Markup)
					: ImpressionEngine.EmptySubs);
		}

		public object Evaluate(IInterpretContext ctx)
		{
			try {
				string completeKey = ObjectName;
				if (this.Properties != null) {
					foreach(string property in Properties)
						completeKey += "." + property;
				}

				Object obj = ctx.Bag.ContainsKey(completeKey)
					? ctx.Bag[completeKey]
					: null;

				if (obj == null) {

					try {

						string key = this.ObjectName;
						obj = ctx.Bag.ContainsKey(key) ? ctx.Bag[key] : null;
						if (obj != null) {
							obj = reflector.Eval(obj, this.Properties);
						}
					}
					catch (Exception ex) {
						throw new Exception(
							"An error occurred looking up key " + completeKey,
							ex
						);
					}
				}

				// loop through all the attributes and process them against the obj
				if (this.Attributes != null) {
					obj = filterRunner.RunFilters(obj, this.Attributes, ctx.Bag, this);
				}

				return obj;
			}
			catch (Exception ex) {
				Trace.WriteLine(ex.ToString());
				throw;
			}
		}

		#region Helpers

		private bool parseMarkupX()
		{
			//Match m = new Regex(@"\[.*\]").Match(this.Markup);
			Match m = ExpressionParserRegex.Match(this.Markup);

			if (m.Success)
			{
				if (m.Groups["object"].Success)
				{
					this.ObjectName = m.Groups["object"].Value.Trim();
				}

				if (m.Groups["within"].Success)
				{
					this.Within = m.Groups["within"].Value.Trim();
				}

				// add properties
				if (m.Groups["properties"].Success)
				{
					CaptureCollection vals = m.Groups["properties"].Captures;
					List<string> props = new List<string>(vals.Count);
					for (int i = 0, len = vals.Count; i < len; i++)
					{
						props.Add(vals[i].Value.Trim());
					}
					this.Properties = props.ToArray();
				}
				else
				{
					this.Properties = null;
				}

				// add attributes
				if (m.Groups["attributes"].Success)
				{
					CaptureCollection vals = m.Groups["attributes"].Captures;

					List<string> atts = new List<string>(vals.Count);
					for (int i = 0, len = vals.Count; i < len; i++)
					{
						atts.Add(vals[i].Value.Trim());
					}
					this.Attributes = atts.ToArray();
				}
				else
				{
					this.Attributes = null;
				}

				return true;
			}
			return false;
		}

		//private void parseMarkup() {

		//   string markupParse = this.Markup;
		//   markupParse = markupParse.Substring( ImpressionEngine.ExpressionOpen.Length ).TrimStart(); // strip the leading "<!--" and spaces
		//   markupParse = markupParse.Substring(0, markupParse.Length - ImpressionEngine.ExpressionOpen.Length).TrimEnd();

		//   // pull apart the declaration
		//   // Object.Accessor.Accesor... | Modifier | Modifier | Modifier

		//   int idx = 0;
		//   string temp;
		//   string[] parts;

		//   if (markupParse.Contains("|")) {

		//      // grab the modifiers off the end
		//      idx = markupParse.IndexOf("|");
		//      parts = markupParse.Substring(idx+1).Split('|');
		//      List<string> atts = new List<string>();

		//      foreach(string val in parts) {
		//         temp = val.Trim();
		//         if (temp.Length > 0) {

		//            // TODO make sure its a supported attribute

		//            atts.Add(temp);
		//         } else {
		//            throw new ImpressionParseException(
		//               string.Format("Invalid Expression encountered \"{0}\"", this.Markup)
		//            );
		//         }
		//      }

		//      this.Attributes = atts.ToArray(); 

		//       // remove the attributes from the markupParse
		//       markupParse = markupParse.Substring(0, idx).TrimEnd();
		//   }

		//   if (markupParse.Contains(".")) {
		//      // grab the accessors off the object
		//       parts = markupParse.Split('.');
		//       this.ObjectName = parts[0];

		//       List<string> props = new List<string>();
		//       for(int i=1,len=parts.Length; i<len; i++)
		//       {
		//           temp = parts[i].Trim();
		//           if (temp.Length > 0)
		//           {
		//               props.Add(temp);
		//           }
		//           else
		//           {
		//               throw new ImpressionParseException(
		//                  string.Format("Invalid Expression encountered \"{0}\"", this.Markup)
		//               );
		//           }
		//       }
		//       this.Properties = props.ToArray();
		//   } else
		//   {
		//       this.ObjectName = markupParse;
		//       this.Properties = null;
		//   }
		//}

		#endregion


		private static Regex expressionParserRegex;
		public static Regex ExpressionParserRegex {
			get {
				if (expressionParserRegex == null) {
					expressionParserRegex = new Regex(
						Resources.ExpressionParserPattern,
						//@"{{[ \t]*((?<within>[a-zA-Z][\w]*)[ \t]+(in|IN)[ \t]+)?(?<object>[a-zA-Z][\w]*)(\.(?<properties>[a-zA-Z][\w]*))*([ \t]*\|[ \t]*(?<attributes>[a-zA-Z][\w]*))*[ \t]*}}",
						//@"{{[ \t]*((?<within>[a-zA-Z][\w]*)[ \t]+(in|IN)[ \t]+)?(?<object>[a-zA-Z][\w]*)(\.(?<properties>[a-zA-Z][\w]*))*([ \t]*\|[ \t]*(?<attributes>[a-zA-Z][\w]*))*[ \t]*}}",
						//@"\[[ \t]*(?<object>[a-zA-Z][\w]*)(\.(?<properties>[a-zA-Z][\w]*))*([ \t]*\|[ \t]*(?<attributes>[a-zA-Z][\w]*))*[ \t]*\]",
						RegexOptions.Compiled | RegexOptions.ExplicitCapture
					);
				}
				return expressionParserRegex;
			}
		}
	}
}
