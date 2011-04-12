
namespace CodeSoda.Impression.Cache
{
	public interface ITemplateCache
	{
		// Methods
		void Add(string key, ParseList parseList);
		void Clear();
		ParseList Get(string key);
	}
}
