using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace CodeSoda.Impression
{
    /// <summary>
    /// TagMarkupBase represents a base which Tags can be created from
    /// </summary>
    public abstract class TagMarkupBase : MarkupBase
    {

        #region Properties

        /// <summary>
        /// Indicates the type of markup that this Object contains
        /// </summary>
        public override MarkupType MarkupType
        {
            get { return MarkupType.Tag; }
        }

		///// <summary>
		///// Indicates the type of tag that this class represents
		///// </summary>
		//public virtual TagMarkupType TagType
		//{
		//    get;
		//    protected set;
		//}

        /// <summary>
        /// The expression component of this tag
        /// </summary>
        public virtual ExpressionMarkup Expression
        {
            get;
            protected set;
        }

		#endregion

        #region Constructors

        public TagMarkupBase(ExpressionMarkup expression, string markup, int lineNumber, int charPos):base(markup,lineNumber, charPos)
        {
			//this.TagType = tagType;
            this.Expression = expression;
        }

        #endregion
    }
}
