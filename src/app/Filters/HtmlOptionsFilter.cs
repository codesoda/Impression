
using System.Collections;
using System.Text;
using IEnumerable=System.Collections.IEnumerable;

namespace CodeSoda.Impression.Filters
{
	public class HtmlOptionsFilter: FilterBase
	{
		public override string Keyword
		{
			get { return "html_options"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters == null || (parameters.Length < 2 || parameters.Length > 3))
				throw new ImpressionInterpretException("Filter " + Keyword + " expects 2 or 3 parameters.", markup);

			// text field to bind to
			// selected value

			// text field to bind to
			// value field to bind to
			// selected value

			if (obj == null)
				return null;

			if (obj.GetType().GetInterface("IEnumerable") != null) {

				IReflector reflector = new Reflector();

				bool textOnly = parameters.Length == 2;
				string textField = parameters[0];
				string selectedField = textOnly ? parameters[1] : parameters[2];

				StringBuilder optionBuilder = new StringBuilder();
				IEnumerator en = ((IEnumerable) obj).GetEnumerator();

				// &#34; for quotes
				// &#39; for apostrophes

				// lookup the selected value
				object selectedObject = bag == null ? null : reflector.Eval(bag, selectedField);
				string selectedValue = selectedObject != null ? selectedObject.ToString() : null;

				if (textOnly) {
					while (en.MoveNext()) {
						object current = en.Current;
						object textObject = reflector.Eval(current, textField);
						string textString = textObject != null ? textObject.ToString() : null;
						string selected = (textString == selectedValue) ? " selected=\"true\"" : "";
						optionBuilder.AppendFormat("<option{1}>{0}</option>", textString, selected);
					}
				} else {
					string valueField = parameters[1];
					
					while(en.MoveNext()) {
						object current = en.Current;
						object textObject = reflector.Eval(current, textField);
						string textString = textObject != null ? textObject.ToString() : null;
						object valueObject = reflector.Eval(current, valueField);
						string valueString = valueObject != null ? valueObject.ToString() : null;

						string selected = (valueString == selectedValue) ? " selected=\"true\"" : "";
						optionBuilder.AppendFormat("<option value=\"{0}\"{2}>{1}</option>", valueString, textString, selected);
					}
				}
				return optionBuilder.ToString();
			}

			return null;


		}

	}
}
