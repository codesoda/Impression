using System;
using System.Collections.Generic;
using System.Text;
using CodeSoda.Impression.Cache;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
    [TestFixture]
    public class HashTemplateCacheTests
    {
        [Test]
        public void SimpleTest()
        {
            var cache = new HashtableTemplateCache(3);
            
            Assert.IsNull(cache.Get("anything"));

            cache.Clear();

            cache.Add("key1", new ParseList());
            cache.Add("key2", new ParseList());
            cache.Add("key3", new ParseList());

            Assert.IsNotNull(cache.Get("key1"));
            Assert.IsNotNull(cache.Get("key2"));
            Assert.IsNotNull(cache.Get("key3"));

            cache.Add("key4", new ParseList());
        	object key1Object = cache.Get("key1");
            Assert.IsNull(key1Object);
        }

    }
}
