using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CodeSoda.Impression.Cache;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	[TestFixture]
	public class ImpressionCachingComparisonTests
	{

		[Test]
		public void TimeTemplateParsingWithoutCache()
		{
			int totalRuns = 100;
			DateTime started = DateTime.Now;

			for (int i = 0; i < totalRuns; i++)
			{
				RunStressTests(null);
			}

			Debug.WriteLine((DateTime.Now - started).TotalMilliseconds / totalRuns);
		}

		[Test]
		public void TimeTemplateParsingWithCache()
		{
			ITemplateCache cache = new HashtableTemplateCache();

			int totalRuns = 100;
			DateTime started = DateTime.Now;

			for (int i = 0; i < totalRuns; i++)
			{
				RunStressTests(cache);
			}

			Debug.WriteLine((DateTime.Now - started).TotalMilliseconds / totalRuns);
		}

		private long RunStressTests(ITemplateCache templateCache)
		{
			long started = DateTime.Now.Ticks;

			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/hbmx/storefront.template.html");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath, new PropertyBag(), templateCache);

			templatePath = Path.Combine(currentFolder, "../templates/hbmx/contact.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag());

			templatePath = Path.Combine(currentFolder, "../templates/hbmx/account.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag());

			templatePath = Path.Combine(currentFolder, "../templates/hbmx/productlist-category.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag());

			templatePath = Path.Combine(currentFolder, "../templates/hbmx/custompage.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag());

			templatePath = Path.Combine(currentFolder, "../templates/hbmx/product.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag());

			templatePath = Path.Combine(currentFolder, "../templates/hbmx/cart.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag());

			templatePath = Path.Combine(currentFolder, "../templates/hbmx/yourinfo.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag());

			templatePath = Path.Combine(currentFolder, "../templates/hbmx/payment.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag());

			long elapsed = DateTime.Now.Ticks - started;
			return elapsed;
		}
	}
}
