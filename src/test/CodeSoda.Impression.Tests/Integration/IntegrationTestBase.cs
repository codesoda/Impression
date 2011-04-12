
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Integration
{

	public abstract class IntegrationTestBase
	{
		protected PropertyBag Bag;
		protected ImpressionEngine Engine;

		virtual public void Setup() {
			Bag = new PropertyBag();
			Engine = ImpressionEngine.Create(Bag);
		}
	}
}
