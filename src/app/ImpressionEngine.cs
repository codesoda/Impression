using System;
using System.IO;
using System.Data;
using CodeSoda.Impression.Cache;
using CodeSoda.Impression.Parsers;

namespace CodeSoda.Impression
{
	public class ImpressionEngine: IDisposable
	{
		private ITemplateParser templateParser = null;
		private ITemplateCache templateCache = null;
		private IReflector reflector = null;

		public static string EmptySubs = "";
		public static string LongDateTimeFormat = "dddd, dd MMMMM yyyy hh:mm tt";
		public static string LongDateFormat = "dddd, dd MMMM yyyy";
		public static string DateTimeFormat = "dd-MMM-yyyy hh:mm tt";
		public static string DateFormat = "dd-MMM-yyyy";
		public static string TimeFormat = "hh:mm tt";
		public static string MoneyFormat = "0.00";

		private IPropertyBag bag;
		public IPropertyBag Bag {
			get {
				if (bag == null)
					bag = new PropertyBag();

				return bag;
			}
			set {
				bag = value;
			}
		}
		private string _templatePath;
		private ParseList _parseList;

		public static ImpressionEngine Create(string templatePath, IPropertyBag bag)
		{
			return Create(templatePath, bag, null);
		}

		public static ImpressionEngine Create(string templatePath, IPropertyBag bag, ITemplateCache templateCache) {

			// create container and register it, in itself
			IContainer container = new Container();
			container.Register<IContainer>(delegate { return container; });

			// singleton objects, only create once
			IReflector reflector = null;
			IFilterRunner filterRunner = null;
			ITagFactory tagFactory = null;

			container.Register<ITemplateCache>(delegate { return templateCache; });
			container.Register<IReflector>( delegate { if (reflector == null) reflector = new Reflector(); return reflector; } );
			container.Register<IFilterRunner>(delegate { if (filterRunner == null) filterRunner = new FilterRunner(); return filterRunner; });
			container.Register<ITagFactory>(delegate { if (tagFactory == null) tagFactory = container.Resolve<TagFactory>(); return tagFactory; });
			
			// instance scoped, create new each time
			container.Register<ITemplateParser, TemplateParser>();

			ImpressionEngine ie = container.Resolve<ImpressionEngine>();
			if (!string.IsNullOrEmpty(templatePath)) {
				ie.LoadTemplate(templatePath, true);
			}

			if (bag != null) {
				ie.LoadBag(bag);
			}

			return ie;
		}

		public static ImpressionEngine Create(string templatePath) {
			return Create(templatePath, null);
		}

		public static ImpressionEngine Create(IPropertyBag bag) {
			return Create(null, bag);
		}



		public ImpressionEngine(ITemplateParser templateParser, IReflector reflector, ITemplateCache templateCache) {
			if (templateParser == null)
				throw new ArgumentNullException("templateParser");
			if (reflector == null)
				throw new ArgumentNullException("reflector");

			// save a local pointer
			this.templateParser = templateParser;
			this.reflector = reflector;
			this.templateCache = templateCache;
		}

		private void LoadTemplate(string templatePath, bool parseTemplate)
		{
			if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
				throw new ArgumentException("The template specified is invalid or could not be found", "templatePath");

			// save the template path
			this._templatePath = templatePath;

			// check for a cached version of this template
			if (templateCache != null && (_parseList = templateCache.Get(templatePath)) != null)
				return;

			string templateContent = File.ReadAllText(_templatePath);
			string templateFolder = Path.GetDirectoryName(_templatePath);

			templateContent = templateParser.PreProcessTemplate(templateContent, templateFolder);

			// parse the template ready for running
			_parseList = templateParser.ParseTemplate(templateContent);

			// check if we need to cache this template
			if (templateCache != null && _parseList != null)
				templateCache.Add(templatePath, _parseList);
		}

		private void LoadBag(IPropertyBag bag) {
			// grab the bag
			this.Bag = bag != null ? new PropertyBag(bag) : new PropertyBag();
		}

		public ImpressionEngine AddItem(string key, object value) {
			Bag.Add(key, value);
			return this;
		}

		public ImpressionEngine AddList(string listname, params object[] columns) {
			// grab the list from the bag
			DataSet ds = this.Bag[listname] as DataSet;

			// make sure dataset exists, if not add it
			if (ds == null) {
				ds = new DataSet();
				this.Bag[listname] = ds;
			}

			// make sure a datatable exists
			if (ds.Tables[listname] == null) {
				DataTable dt = new DataTable(listname);
				foreach (string colname in columns) {
					dt.Columns.Add(colname);
				}
				ds.Tables.Add(dt);
			}

			return this;
		}

		public ImpressionEngine AddListItem(string listname, params object[] values) {
			DataSet ds = this.Bag[listname] as DataSet;

			if (ds != null && ds.Tables[listname] == null) {
				ds.Tables[listname].Rows.Add(values);
				return this;
			}
			else {
				throw new ArgumentException("There is no list with the name \"" + listname +
											"\", call AddList to add the list first");
			}
		}

		public string Run() {
			// make sure the current template is loaded and parsed
			if (_templatePath == null) {
				// TODO this should be some other type of exception
				throw new Exception("Cannot run without a template");
			}
			else if (_parseList == null) {
				// TODO this should be some other type of exception
				throw new Exception("Cannot run without a parseList");
			}

			// run the impression against the internal parse and bag
			return this.Run(_parseList, this.Bag);
		}

		public string Run(IPropertyBag bag) {
			// make sure the current template is loaded and parsed
			if (_templatePath == null) {
				// TODO this should be some other type of exception
				throw new Exception("Cannot run without a template");
			}
			else if (_parseList == null) {
				// TODO this should be some other type of exception
				throw new Exception("Cannot run without a parseList");
			}

			return this.Run(_parseList, bag);
		}

		public string RunString(string template) {
			ParseList parseList = templateParser.ParseTemplate(template);
			return this.Run(parseList, this.Bag);
		}

		public string Run(string templatePath) {
			// run against the specified template
			if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath)) {
				throw new ArgumentException("The template specified is invalid or could not be found", "templatePath");
			}

			LoadTemplate(templatePath, true);

			//ParseList parseList = templateParser.ParseTemplate(File.ReadAllText(templatePath), Path.GetDirectoryName(templatePath));

			return this.Run(this._parseList, this.Bag);
		}

		private string Run(ParseList parseList, IPropertyBag bag) {

			IInterpretContext ctx = new InterpretContext();
			ctx.Bag = bag;
			ctx.ParseList = parseList;

			int listLength = ctx.ParseList.Count;
			while (ctx.ListPosition < listLength) {

				// go through the parseList and act on each item we find
				MarkupBase markupItem = parseList[ctx.ListPosition];
				markupItem.Interpret(ctx);
			}

			return ctx.Builder.ToString();
		}

		//private bool ReplaceToBoolean(string key) {
		//    return ReplaceToBoolean(key, false);
		//}

		//private bool ReplaceToBoolean(string key, bool defaultValue) {
		//    bool retVal = defaultValue;
		//    if (Bag != null && Bag.Count > 0 && Bag.ContainsKey(key)) {
		//        Object obj = Bag[key];
		//        if (obj is string) {
		//            Boolean.TryParse(obj as string, out retVal);
		//        }
		//        else {
		//            try {
		//                retVal = Convert.ToBoolean(obj);
		//            }
		//            catch {
		//            }
		//        }
		//    }
		//    return retVal;
		//}

		//private string ReplaceToString(string key) {
		//    return ReplaceToString(key, "");
		//}

		//private string ReplaceToString(string key, string defaultValue) {
		//    string retVal = defaultValue;
		//    if (Bag != null && Bag.Count > 0 && Bag.ContainsKey(key)) {
		//        Object obj = Bag[key];
		//        if (obj is string) {
		//            retVal = obj as string;
		//        }
		//        else {
		//            try {
		//                retVal = obj.ToString();
		//            }
		//            catch {
		//            }
		//        }
		//    }
		//    return retVal;
		//}

		//private string Replace(
		//    string original,
		//    string pattern,
		//    string replacement) {
		//    return Replace(original, pattern, replacement, StringComparison.CurrentCulture);
		//}

		//private string Replace(
		//    string original,
		//    string pattern,
		//    string replacement,
		//    StringComparison comparisonType) {
		//    if (original == null) {
		//        return null;
		//    }

		//    if (String.IsNullOrEmpty(pattern)) {
		//        return original;
		//    }

		//    int lenPattern = pattern.Length;
		//    int idxPattern = -1;
		//    int idxLast = 0;
		//    StringBuilder result = new StringBuilder();
		//    while (true) {
		//        idxPattern = original.IndexOf(pattern, idxPattern + 1, comparisonType);
		//        if (idxPattern < 0) {
		//            result.Append(original, idxLast, original.Length - idxLast);
		//            break;
		//        }
		//        result.Append(original, idxLast, idxPattern - idxLast);
		//        result.Append(replacement);
		//        idxLast = idxPattern + lenPattern;
		//    }
		//    return result.ToString();
		//}

		//public static bool ContainsTemplate(string templateText) {
		//    // if string contains any impression tags
		//    return false;
		//}



        #region IDisposable Members

        public void Dispose()
        {
            if (this.bag != null)
            {
                if (this.bag.Count > 0)
                {
                    foreach(string key in bag.Keys)
                    {
                        object obj = bag[key];
                        if (obj.GetType().IsAssignableFrom(typeof(IDisposable)))
                        {
                            ((IDisposable)obj).Dispose();
                        }
                    }
                    
                }

                bag = null;
            }
        }

        #endregion
    }
}

/*
OTHER SYNTAXS

 * <!--#include virtual="insertthisfile.html" --> 

 * 
 * {$title|default:"no title"}
 * 
<html>
<head>
<title>User Info</title>
</head>
<body>
User Information:<p>
Name: {$name|capitalize}<br>
Addr: {$address|escape}<br>
Date: {$smarty.now|date_format:"%Y-%m-%d"}<br>
</body>
</html>




// string template
using Antlr.StringTemplate;
StringTemplate hello = new StringTemplate("Hello, $name$");
hello.SetAttribute("name", "World");
Console.Out.WriteLine(hello.ToString());

for expressions maybe try {$field}

expression attributes
 * formatas_date(dateformat e.g. dd-MMM-yyyy)
 * formatas_currency
 * formatas_number (number format)
 
template api's
 * if, elseif, else, endif - conditional ouput
 * include - include another template
 * escape - html output wrapper
 * date - date output wrapper
controller api's
 * 




specify a seperator for when something has multiple values
$values; separator=", "$ // from stringtemplate

specify a value to be substitued for when the value is null
$values; null="-1", separator=", "$


property references
e.g. $person.name$ or $person.email$
a C# property (i.e. a non-indexed CLR property) named name 
A method named get_name() 
A method named Getname() 
A method named Isname() 
A method named getname() 
A method named isname() 
A field named name 
A C# indexer (i.e. a CLR indexed property) that accepts a single string parameter - this["name"]
If found, a return value is obtained via reflection. The person.email expression is resolved in a similar manner. 
As shown above, if the property is not accessible as a C# property, StringTemplate attempts to find a field with the same name as the property. In the above example, StringTemplate would look for fields name and email without the capitalization typically used with property access methods. 



/*
Class1 c = new Class1();
FieldInfo[] fis = typeof(Class1).GetFields(BindingFlags.Instance |
BindingFlags.Public | BindingFlags.NonPublic);
foreach (FieldInfo fi in fis)
Console.WriteLine(fi.Name + " = " + fi.GetValue(c));
Console.ReadLine();
}
}

If you want set the field value call SetValue instead of GetValue.
If you want to get/set properties, use PropertyInfo[] pis = ...GetProperties
*/

/*
private static string ReplaceEx(string original, 
                    string pattern, string replacement)
{
    int count, position0, position1;
    count = position0 = position1 = 0;
    string upperString = original.ToUpper();
    string upperPattern = pattern.ToUpper();
    int inc = (original.Length/pattern.Length) * 
              (replacement.Length-pattern.Length);
    char [] chars = new char[original.Length + Math.Max(0, inc)];
    while( (position1 = upperString.IndexOf(upperPattern, 
                                      position0)) != -1 )
    {
        for ( int i=position0 ; i < position1 ; ++i )
            chars[count++] = original[i];
        for ( int i=0 ; i < replacement.Length ; ++i )
            chars[count++] = replacement[i];
        position0 = position1+pattern.Length;
    }
    if ( position0 == 0 ) return original;
    for ( int i=position0 ; i < original.Length ; ++i )
        chars[count++] = original[i];
    return new string(chars, 0, count);
}


static public string Replace(string original, string pattern, string replacement, StringComparison comparisonType)        {            if (original == null)            {                return null;            }            if (String.IsNullOrEmpty(pattern))            {                return original;            }            int lenPattern = pattern.Length;            int idxPattern = -1;            int idxLast = 0;                        StringBuilder result = new StringBuilder();            while (true)            {                idxPattern = original.IndexOf(pattern, idxPattern + 1, comparisonType);                if (idxPattern < 0)                {                    result.Append(original, idxLast, original.Length - idxLast);                                        break;                }                result.Append(original, idxLast, idxPattern - idxLast);                result.Append(replacement);                idxLast = idxPattern + lenPattern;            }            return result.ToString();        }

*/

/*
// the template (our formatting expression)
var myTemplate = new Template('The TV show #{title} was created by #{author}.');
// our data to be formatted by the template
var show = {title: 'The Simpsons', author: 'Matt Groening', network: 'FOX' };
// let's format our data
myTemplate.evaluate(show);
// -> The TV show The Simpsons was created by Matt Groening.

// note: you're seeing two backslashes here because the backslash is also a // escaping character in JavaScript strings var t = new Template('in #{lang} we also use the \\#{variable} syntax for templates.'); var data = {lang:'Ruby', variable: '(not used)'}; t.evaluate(data); // -> in Ruby we also use the #{variable} syntax for templates. 

var syntax = /(^|.|\r|\n)(\<%=\s*(\w+)\s*%\>)/; //matches symbols like '<%= field %>' var t = new Template('<div>Name: <b><%= name %></b>, Age: <b><%=age%></b></div>', syntax); t.evaluate( {name: 'John Smith', age: 26} ); // -> <div>Name: <b>John Smith</b>, Age: <b>26</b></div> 
*/