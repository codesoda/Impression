using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CodeSoda.Impression.Cache;

namespace CodeSoda.Impression.LoadTests
{
	public class LoadTests
	{

		public void Stress()
		{
			//TimeTemplateParsingWithoutCache();
			TimeTemplateParsingWithCache();
		}

		public void StressHashtableCache()
		{
			int maxItems = 1000000;
			int maxHits = 1000000;

			Random random = new Random();

			var cache = new HashtableTemplateCache(maxItems);
			cache.Clear();

			for (int i =0;i < maxItems; i++)
				cache.Add("key" + i, new ParseList());

			for( int i=0; i < maxHits; i++)
			{
				cache.Get("key" + random.Next(maxItems));
			}
			
		}

		public void TimeTemplateParsingWithoutCache()
		{
			const int totalRuns = 100;
			DateTime startedAll = DateTime.Now;

			for (int i = 0; i < totalRuns; i++)
			{
				DateTime started = DateTime.Now;
				RunStressTests(null);
				Console.WriteLine("Iteration {0} of {1} completed in {2}", i+1, totalRuns, (DateTime.Now - started).Milliseconds);
			}

			TimeSpan ts = DateTime.Now - startedAll;
			Console.WriteLine("Execution completed in {0}, average = {1}", ts.TotalMilliseconds, ts.TotalMilliseconds / totalRuns);
			Console.ReadLine();
		}

		public void TimeTemplateParsingWithCache()
		{
			ITemplateCache cache = new HashtableTemplateCache();

			const int totalRuns = 100;
			DateTime startedAll = DateTime.Now;
			
			for (int i = 0; i < totalRuns; i++)
			{
				DateTime started = DateTime.Now;
				RunStressTests(cache);
				Console.WriteLine("Iteration {0} of {1} completed in {2}", i+1, totalRuns, (DateTime.Now - started).Milliseconds);
			}

			TimeSpan ts = DateTime.Now - startedAll;
			Console.WriteLine("Execution completed in {0}, average = {1}", ts.TotalMilliseconds, ts.TotalMilliseconds / totalRuns);
			Console.ReadLine();
		}

		private long RunStressTests(ITemplateCache templateCache)
		{
			long started = DateTime.Now.Ticks;

			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../hbmx/storefront.template.html");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath, new PropertyBag(), templateCache);

			templatePath = Path.Combine(currentFolder, "../hbmx/contact.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag(), templateCache);

			templatePath = Path.Combine(currentFolder, "../hbmx/account.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag(), templateCache);

			templatePath = Path.Combine(currentFolder, "../hbmx/productlist-category.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag(), templateCache);

			templatePath = Path.Combine(currentFolder, "../hbmx/custompage.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag(), templateCache);

			templatePath = Path.Combine(currentFolder, "../hbmx/product.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag(), templateCache);

			templatePath = Path.Combine(currentFolder, "../hbmx/cart.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag(), templateCache);

			templatePath = Path.Combine(currentFolder, "../hbmx/yourinfo.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag(), templateCache);

			templatePath = Path.Combine(currentFolder, "../hbmx/payment.template.html");
			ie = ImpressionEngine.Create(templatePath, new PropertyBag(), templateCache);

			long elapsed = DateTime.Now.Ticks - started;
			return elapsed;
		}
	}
}
