﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Wyam.Common;
using IDocument = Wyam.Common.IDocument;

namespace Wyam.Modules.Html
{
    public class Excerpt : IModule
    {
        private string _querySelector;
        private string _metadataKey;

        public Excerpt(string querySelector = "p", string metadataKey = "Excerpt")
        {
            _querySelector = querySelector;
            _metadataKey = metadataKey;
        }

        public Excerpt SetMatadataKey(string metadataKey)
        {
            _metadataKey = metadataKey;
            return this;
        }

        public Excerpt SetQuerySelector(string querySelector)
        {
            _querySelector = querySelector;
            return this;
        }

        public IEnumerable<IDocument> Execute(IReadOnlyList<IDocument> inputs, IExecutionContext context)
        {
            HtmlParser parser = new HtmlParser();
            return inputs.Select(x =>
            {
                try
                {
                    IHtmlDocument htmlDocument = parser.Parse(x.Stream);
                    IElement element = htmlDocument.QuerySelector(_querySelector);
                    if (element != null)
                    {
                        return x.Clone(new Dictionary<string, object>()
                        {
                            {_metadataKey, element.OuterHtml}
                        });
                    }
                    return x;
                }
                catch (Exception ex)
                {
                    context.Trace.Warning("Exception while parsing HTML for {0}: {1}", x.Source, ex.Message);
                    return x;
                }
            });
        }
    }
}