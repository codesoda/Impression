//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Web;

//namespace CodeSoda.Impression
//{

//    public static class Formatters
//    {
//        private static Random random = new Random();

//        private static IReflector reflector = new Reflector();

//        #region AsBoolean

//        public static bool AsBoolean(object obj) {

//            if (obj == null)
//                return false;

//            bool retVal = false; // todo check if there is some kinda default value specified in attributes

//            // bool
//            if (obj is bool)
//            {
//                retVal = (bool)obj;
//            }

//            // strings
//            else if (obj is string)
//            {
//                if (!Boolean.TryParse((string)obj, out retVal))
//                {
//                    double tempNumber = 0;
//                    if (!Double.TryParse((string)obj, out tempNumber)) {
//                        retVal = !string.IsNullOrEmpty((string) obj);
//                    } else {
//                        retVal = tempNumber > 0;
//                    }
//                }
//            }

//            // numbers
//            else if (obj is int)
//            {
//                retVal = (int) obj > 0;
//            }
//            else if (obj is double)
//            {
//                retVal = (double)obj > 0;
//            }
//            else if (obj is float)
//            {
//                retVal = (float)obj > 0;
//            }
//            else if (obj is decimal)
//            {
//                retVal = (decimal)obj > 0;
//            }

//            // arrays, lists, etc
//            else if(obj is IEnumerable)
//            {
//                retVal = ((IEnumerable) obj).GetEnumerator().MoveNext();
//            }

//            // convert using dot net
//            else if (obj.GetType().GetInterface("IConvertible") != null)
//            {
//                //try {
//                retVal = Convert.ToBoolean(obj);
//                //}catch {}
//            }
			
//            // catch all, anything not null is true
//            else
//            {
//                retVal = obj != null;
//            }
//            return retVal;
//        }

//        #endregion

//        /// <summary>
//        /// Checks if an attribute can be broken into a parameterized attribute call e.g. attribute(val2,val2...etc)
//        /// </summary>
//        /// <param name="attribute">
//        /// the full attribute with parameters <see cref="System.String"/>
//        /// </param>
//        /// <param name="parameters">
//        /// an array of the parameters for the attribute <see cref="System.String"/>
//        /// </param>
//        /// <returns>
//        /// string - the name of the parameterized attribute <see cref="System.String"/>
//        /// </returns>
//        public static string ParseParameterizedAttribute(string attribute, out string[] parameters) {
			
//            // check if the attribute can be broken into a parameterized attribute call
//            // eg. attribute(val1,val2...etc)
			
//            parameters = null;
//            string methodName = null;

//            Match m = Parsers.AttributeParserRegex.Match(attribute);

//            if (m.Success) {
//                // attribute method name
//                if (m.Groups["method"].Success) {
//                    methodName = m.Groups["method"].Value;
//                }

//                // parameters to the attribute method
//                if (m.Groups["param"].Success) {
//                    CaptureCollection vals = m.Groups["param"].Captures;
//                    List<string> parms = new List<string>(vals.Count);
//                    for (int i = 0, len = vals.Count; i < len; i++) {
//                        parms.Add(vals[i].Value);
//                    }
//                    parameters = parms.ToArray();
//                }
//            } else {
//                methodName = attribute;
//            }
			
//            return methodName;
//        }

//        #region FormatObject

//        public static object FormatObject(object obj, string[] attributes, PropertyBag bag, IMarkupBase markup)
//        {
//            foreach (string attribute in attributes)
//            {
//                string[] parameters = null;
//                string attr = ParseParameterizedAttribute(attribute, out parameters);
				

//                switch (attr.ToLower())
//                {

//            //////// default values
//                    case "default":
//                        obj = Default(obj, parameters, bag, markup);
//                        break;

//                    case "empty_true":
//                        obj = Default(obj, true);
//                        break;

//                    case "empty_false":
//                        obj = Default(obj, false);
//                        break;

//            //////// boolean

//                    case "not":
//                        obj = Not(obj);
//                        break;

//                    case "if":
//                        obj = IIf(obj, parameters, bag, markup);
//                        break;

//            //////// formatting types as strings

//                    case "format_money":
//                        obj = FormatMoney(obj, ImpressionEngine.MoneyFormat);
//                        break;

//                    case "format_longdatetime":
//                        obj = FormatDate(obj, ImpressionEngine.LongDateTimeFormat);
//                        break;

//                    case "format_longdate":
//                        obj = FormatDate(obj, ImpressionEngine.LongDateFormat);
//                        break;

//                    case "format_datetime":
//                        obj = FormatDate(obj, ImpressionEngine.DateTimeFormat);
//                        break;

//                    case "format_date":
//                        obj = FormatDate(obj, ImpressionEngine.DateFormat);
//                        break;

//                    case "lowercase":
//                        obj = LowerCase(obj);
//                        break;

//                    case "uppercase":
//                        obj = UpperCase(obj);
//                        break;


//            /////////// List operations

//                    case "shuffle":
//                        // randomize the order
//                        obj = Shuffle(obj);
//                        break;

//                    case "pick":
//                        obj = Pick(obj, parameters);
//                        break;

//                    case "pick_first":
//                        obj = First(obj);
//                        break;

//                    case "pick_last":
//                        obj = Last(obj);
//                        break;

//                    case "random":
//                        obj = Random(obj);
//                        break;

//                    case "count":
//                        obj = Count(obj);
//                        break;

//                    case "join_comma":
//                        obj = Join(obj, ",");
//                        break;

//                    case "join_space":
//                        obj = Join(obj, " ");
//                        break;

//                    case "join_commaspace":
//                        obj = Join(obj, ", ");
//                        break;

//                    case "html_options":
//                        obj = FormatHtmlOptions(obj, parameters, bag, markup);
//                        break;

//                    case "html_checked":
//                        obj = AsBoolean(obj) ? "checked" : "";
//                        break;

//                    case "html_encode":
//                        obj = HttpUtility.HtmlEncode(obj.ToString());
//                        break;
//                }
//            }
//            return obj;
//        }

//        #endregion

//        #region Literal operations

//        public static bool IsLiteral(string value)
//        {
//            return
//                !string.IsNullOrEmpty(value)
//                && (
//                    (value.StartsWith("\"") && value.EndsWith("\""))
//                    || (value.StartsWith("'") && value.EndsWith("'"))
//                );
//        }

//        public static string GetLiteral(string value)
//        {
//            return GetLiteral(value, false);
//        }

//        public static string GetLiteral(string value, bool forceLiteral)
//        {
//            if (forceLiteral || IsLiteral(value))
//            {
//                return value.Trim('\'', '\"');
//            }
//            else
//                return null;
//        }

//        #endregion

//        #region Default methods

//        public static object Default(object obj, string[] parms, PropertyBag bag, IMarkupBase markup) {
//            if (parms == null || parms.Length != 1)
//            {
//                throw new ImpressionParseException("default expects 1 parameter", markup);
//            }

//            bool hasValue = (obj is string && !string.IsNullOrEmpty((string)obj)) || (obj != null);

//            if (!hasValue)
//            {
//                if (IsLiteral(parms[0]))
//                {
//                    obj = GetLiteral(parms[0]);
//                }
//                else
//                {
//                    // lookup property on the bag
//                    //obj = bag[parms[0]];
//                    obj = reflector.Eval(bag, parms[0]);
//                }
//            }

//            return obj;
//        }

//        public static object Default(object obj, object def)
//        {
//            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
//            {
//                obj = def;
//            }
//            return obj;
//        }

//        #endregion

//        #region Boolean methods

//        public static object Not(object obj)
//        {
//            return !AsBoolean(obj);
//        }

//        public static object IIf(object obj, string[] parms, PropertyBag bag, IMarkupBase markup) {

//            if (parms == null || parms.Length != 2)
//            {
//                throw new ImpressionParseException("if expects 2 parameters", markup);
//            }

//            bool isTrue = AsBoolean(obj);

//            int index = isTrue ? 0 : 1;
			
//            if (IsLiteral(parms[index])) {
//                obj = GetLiteral(parms[index]);
//            } else {
//                // lookup property on the bag
//                //obj = bag[parms[index]];
//                obj = reflector.Eval(bag, parms[0]);
//            }

//            return obj;

//        }

//        #endregion

//        #region Pick (take first X elements from a list)

//        public static object Pick(object obj, string[] parameters)
//        {
//            return obj;
//        }

//        public static string Pick(string s, string[] parameters)
//        {
//            return s;
//        }

//        public static IEnumerable Pick(IEnumerable obj, string[] parms, IMarkupBase markup)
//        {

//            if (obj == null)
//                return null;

//            if (parms == null || parms.Length != 1)
//            {
//                throw new ImpressionParseException("pick expects 1 number as a parameter", markup);
//            }

//            int number;
//            if (!Int32.TryParse(GetLiteral(parms[0], true), out number))
//            {
//                throw new ImpressionParseException("pick expects 1 number as a parameter", markup);
//            }

//            ArrayList list = new ArrayList();
//            IEnumerator en = obj.GetEnumerator();
//            for (int i = 0; i < number; i++)
//            {
//                if (en.MoveNext())
//                    list.Add(en.Current);
//            }

//            return list;
//        } 

//        public static IEnumerable<T> Pick<T>(IEnumerable<T> obj, string[] parms, IMarkupBase markup) {
			
//            if (obj == null)
//                return null;

//            if (parms == null || parms.Length != 1)
//            {
//                throw new ImpressionParseException("pick expects 1 number as a parameter", markup);
//            }

//            int number;
//            if (!Int32.TryParse(GetLiteral(parms[0], true), out number))
//            {
//                throw new ImpressionParseException("pick expects 1 number as a parameter", markup);
//            }

//            List<T> list = new List<T>();
//            IEnumerator en = ((IEnumerable)obj).GetEnumerator();
//            for(int i=0; i<number; i++)
//            {
//                if (en.MoveNext())
//                    list.Add((T)en.Current);
//            }

//            return list;
//        }

//        #endregion

//        #region Format

//        public static object FormatMoney(object obj, string format)
//        {
//            if (obj == null)
//            {
//                obj = 0.0;
//            }

//            double val = 0;
//            if (Double.TryParse(obj.ToString(), out val))
//            {
//                obj = val.ToString(format);
//            }

//            //List<Type> numberTypes = new List<Type>();
//            //numberTypes.Add( typeof(double) );
//            //numberTypes.Add( typeof(int) );
//            //numberTypes.Add( typeof(long) );
//            //numberTypes.Add( typeof(float) );

//            //if (numberTypes.Contains(type)) {
//            //   obj = ((double)obj).ToString("c");
//            //}

//            return obj;
//        }

//        public static
//           object FormatDate(object obj, string format)
//        {
//            if (obj != null)
//            {
//                if (obj is DateTime)
//                {
//                    obj = ((DateTime)obj).ToString(format);
//                }
//                else
//                {
//                    DateTime val;
//                    if (DateTime.TryParse(obj.ToString(), out val))
//                    {
//                        obj = val.ToString(format);
//                    }
//                }
//            }
//            return obj;
//        }

//        #endregion

//        #region string operations

//        public static object LowerCase(object obj)
//        {
//            return (obj != null ? obj.ToString().ToLower() : obj);
//        }

//        public static object UpperCase(object obj)
//        {
//            return (obj != null ? obj.ToString().ToUpper() : obj);
//        }

//        public static object Join(object obj, string seperator)
//        {
//            if (obj != null && obj.GetType().GetInterface("IEnumerable") != null)
//            {
//                // if its not a string array
//                if (obj.GetType() != typeof(string[]))
//                {
//                    List<string> ar = new List<string>();
//                    IEnumerator en = ((IEnumerable)obj).GetEnumerator();
//                    while (en.MoveNext())
//                        ar.Add(en.Current.ToString());
//                    obj = ar.ToArray();
//                }
//                obj = string.Join(seperator, (string[])obj);
//            }
//            return obj;
//        }

//        #endregion

//        #region Shuffle

//        public static string Shuffle(string s)
//        {
//            return s;
//        }

//        // for cases when the parameter is null, or some other data type
//        // basically jsut ignore
//        public static object Shuffle(object obj)
//        {
//            return obj;
//        }

//        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> enumerable) {

//            if (enumerable == null)
//                return null;

//            List<T> list = new List<T>(enumerable);
//            //list.Sort((a, b) => Guid.NewGuid().CompareTo(Guid.NewGuid()));
//            if (list.Count <= 1)
//            {
//                return list; // nothing to do
//            }

//            for (int i = 0; i < list.Count; i++)
//            {
//                int newIndex = random.Next(0, list.Count);

//                // swap the two elements over 
//                T x = list[i];
//                list[i] = list[newIndex];
//                list[newIndex] = x;
//            }
			
//            return list;
//        }

//        public static IEnumerable Shuffle(IEnumerable enumerable)
//        {

//            if (enumerable == null)
//                return null;

//            ArrayList list = new ArrayList();
//            IEnumerator en = enumerable.GetEnumerator();
//            while (en.MoveNext())
//                list.Add(en.Current);

//            for (int i = 0; i < list.Count; i++)
//            {
//                int r = random.Next(0, list.Count);

//                object tmp = list[i];
//                list[i] = list[r];
//                list[r] = tmp;
//            }

//            return list;
//        }

//        #endregion

//        #region List operations

//        public static object Random(object obj)
//        {
//            return obj;
//        }

//        public static string Random(string obj)
//        {
//            return obj;
//        }

//        public static object Random(IEnumerable obj)
//        {
//            if (obj == null)
//                return null;

//            ArrayList list = new ArrayList();
//            IEnumerator en = obj.GetEnumerator();
//            while (en.MoveNext())
//                list.Add(en.Current);

//            return list.Count == 0
//                    ? null
//                    : (list.Count == 1
//                        ? list[0]
//                        : list[random.Next(0, list.Count)]);
//        }

//        public static T Random<T>(IEnumerable<T> obj)
//        {
//            if (obj == null)
//                return default(T);

//            List<T> list = new List<T>();
//            IEnumerator en = obj.GetEnumerator();
//            while (en.MoveNext())
//                list.Add((T)en.Current);

//            return list.Count == 0
//                    ? default(T)
//                    : (list.Count == 1
//                        ? list[0]
//                        : list[random.Next(0, list.Count)]);
//        }

//        public static object First(object obj)
//        {
//            if (obj != null)
//            {
//                if (obj.GetType().GetInterface("IEnumerable") != null)
//                {
//                    IEnumerator en = ((IEnumerable)obj).GetEnumerator();
//                    obj = en.MoveNext() ? en.Current : null;
//                }

//                // if its not enumerable, then leave alone the first item
//                // from something which isnt a list or array is itself
//            }
//            return obj;
//        }

//        public static object Last(object obj)
//        {
//            if (obj != null)
//            {
//                if (obj.GetType().GetInterface("IEnumerable") != null)
//                {
//                    IEnumerator en = ((IEnumerable)obj).GetEnumerator();
//                    object last = null;
//                    while (en.MoveNext()) last = en.Current;
//                    obj = last;
//                }

//                // if its not enumerable, then leave alone the last item
//                // from something which isnt a list or array is itself
//            }
//            return obj;
//        }

//        public static object Count(object obj)
//        {
//            if (obj != null)
//            {
//                if (obj.GetType().GetInterface("IEnumerable") != null)
//                {
//                    IEnumerator en = ((IEnumerable)obj).GetEnumerator();
//                    int count = 0;
//                    while (en.MoveNext()) count++;
//                    obj = count;
//                }

//                // if its not enumerable, but its not null, then return 1;
//                obj = 1;

//            }
//            else
//            {
//                // if the object is null then return 0
//                obj = 0;
//            }
//            return obj;
//        }

//        #endregion

//        #region html operations

//        public static object FormatHtmlOptions(object obj, string[] parameters, PropertyBag bag, IMarkupBase markup)
//        {
			

//            if (parameters == null || (parameters.Length < 2 || parameters.Length > 3))
//                throw new ImpressionInterpretException("Formatter html_options expects 2 or 3 parameters.", markup);

//            if (obj == null)
//                return null;

//            // text field to bind to
//            // selected value

//            // text field to bind to
//            // value field to bind to
//            // selected value

//            if (obj.GetType().GetInterface("IEnumerable") != null)
//            {

//                bool textOnly = parameters.Length == 2;
//                string selectedField = parameters[0];
//                string textField = parameters[1];

//                StringBuilder optionBuilder = new StringBuilder();
//                IEnumerator en = ((IEnumerable)obj).GetEnumerator();

//                // &#34; for quotes
//                // &#39; for apostrophes

//                // lookup the selected value
//                string selectedValue = bag == null ? null : (reflector.Eval(bag, selectedField) ?? "").ToString();

//                if (textOnly)
//                {
//                    while (en.MoveNext())
//                    {
//                        object current = en.Current;
//                        string text = ((reflector.Eval(current, textField)) ?? "").ToString();
//                        string selected = (text == selectedValue) ? " selected=\"true\"" : "";
//                        optionBuilder.AppendFormat("<option{1}>{0}</option>", text, selected);
//                    }
//                }
//                else
//                {
//                    string valueField = parameters[2];

//                    while (en.MoveNext())
//                    {
//                        object current = en.Current;
//                        string text = ((reflector.Eval(current, textField)) ?? "").ToString();
//                        string value = ((reflector.Eval(current, valueField)) ?? "").ToString();
//                        string selected = (value == selectedValue) ? " selected=\"true\"" : "";
//                        optionBuilder.AppendFormat("<option value=\"{0}\"{2}>{1}</option>", value, text, selected);
//                    }
//                }
//                return optionBuilder.ToString();
//            }

//            return null;


//        }

//        #endregion

//    }

//}

