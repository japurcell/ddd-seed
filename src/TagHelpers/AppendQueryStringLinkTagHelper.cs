using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelpers;

[HtmlTargetElement(Attributes = PageAttributeName)]
[HtmlTargetElement(Attributes = AppendQueryStringToRouteValuesName)]
public class AppendQueryStringLinkTagHelper : TagHelper
{
    private const string PageAttributeName = "asp-page";
    private const string AppendQueryStringToRouteValuesName = "asp-append-query-params";
    private const string RouteValuesDictionaryName = "asp-all-route-data";
    private const string RouteValuesPrefix = "asp-route-";

    [HtmlAttributeName(PageAttributeName)]
    public string? Page { get; set; }

    [HtmlAttributeName(AppendQueryStringToRouteValuesName)]
    public bool AppendQueryStringToRouteValues { get; set; }

    [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
    public IDictionary<string, string> RouteValues { get; set; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (AppendQueryStringToRouteValues)
        {
            ViewContext.HttpContext.Request.Query
                .ToList()
                .ForEach(kvp =>
            {
                if (!(RouteValues?.ContainsKey(kvp.Key) ?? false))
                {
                    SetAttribute(output, "href", $"{kvp.Key}={kvp.Value}");
                }
            });
        }

        base.Process(context, output);
    }

    private static void SetAttribute(TagHelperOutput output, string attributeName, string value)
    {
        var v = value;
        if (output.Attributes.TryGetAttribute(attributeName, out var attribute))
        {
            v = $"{attribute.Value}&{value}";
        }
        output.Attributes.SetAttribute(attributeName, v);
    }
}
