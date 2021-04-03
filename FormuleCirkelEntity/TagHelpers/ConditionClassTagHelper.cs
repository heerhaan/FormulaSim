using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.TagHelpers
{
    [HtmlTargetElement("div", Attributes = ClassPrefix + "*")]
    public class ConditionClassTagHelper : TagHelper
    {
        private const string ClassPrefix = "condition-class-";
        [HtmlAttributeName("class")]
        public string CssClass { get; set; }

        private IDictionary<string, bool> _classValues;

        [HtmlAttributeName("", DictionaryAttributePrefix = ClassPrefix)]
        public IDictionary<string, bool> ClassValues
        {
            get
            {
                return _classValues ?? (_classValues =
                    new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase));
            }
            set { _classValues = value; }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var items = _classValues.Where(e => e.Value).Select(e => e.Key).ToList();

            if (!string.IsNullOrEmpty(CssClass))
            {
                items.Insert(0, CssClass);
            }

            if (items.Any())
            {
                var classes = string.Join(" ", items.ToArray());
                output.Attributes.Add("class", classes);
            }
        }
    }
}
