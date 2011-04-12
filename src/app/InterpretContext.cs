
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeSoda.Impression
{
	public interface IInterpretContext
	{
		IPropertyBag Bag { get; set; }
		ParseList ParseList { get; set; }
		int ListPosition { get; }
		Hashtable State { get; }
		StringBuilder Builder { get; }
		Stack<object> CurrentTag { get; }
		MarkupBase CurrentMarkup { get; }
		bool MoveTo(int i);
		bool MoveNext();
	}

	public class InterpretContext : IInterpretContext
	{
		public IPropertyBag Bag { get; set; }
		public ParseList ParseList { get; set; }
		public int ListPosition { get; private set; }
		public Hashtable State { get; private set; }
		public StringBuilder Builder { get; private set; }
		public Stack<object> CurrentTag { get; private set; }

		public InterpretContext() {
			this.ListPosition = 0;
			this.State = new Hashtable();
			this.Builder = new StringBuilder();
			this.CurrentTag = new Stack<object>();
		}

		public MarkupBase CurrentMarkup {
			get {
				if (this.ParseList != null && this.ListPosition >= 0 && this.ListPosition < this.ParseList.Count) {
					return this.ParseList[this.ListPosition];
				}
				return null;
			}
		}

		public bool MoveTo(int i) {
			if (i > 0 && i < this.ParseList.Count) {
				this.ListPosition = i;
				return true;
			}
			return false;
		}

		public bool MoveNext() {
			if (this.ListPosition <= this.ParseList.Count - 1) {
				this.ListPosition = this.ListPosition + 1;
				return true;
			}
			return false;
		}

	}
}
