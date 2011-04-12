
using System;

namespace CodeSoda.Impression
{
    internal class LiteralMarkup : MarkupBase
    {

        public LiteralMarkup(string markup):base(markup, -1, -1)
        {
        }

       internal override void Interpret(IInterpretContext ctx)
       {
          // this is a literal, simply append our content and increment position in parselist
          ctx.Builder.Append(this.Markup);
          ctx.MoveNext();
       }

        public override MarkupType MarkupType
        {
            get
            {
                return MarkupType.Literal;
            }
        }
    }
}
