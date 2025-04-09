using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelpers;

[HtmlTargetElement(Attributes = PageAttributeName)]
[HtmlTargetElement(Attributes = ActionsAttributeName)]
[HtmlTargetElement(Attributes = ResourcesAttributeName)]
[HtmlTargetElement(Attributes = ClassAttributeName)]
[HtmlTargetElement(Attributes = DisableRenderActiveAttributeName)]
public class ActiveLinkTagHelper : TagHelper
{
    private const string PageAttributeName = "asp-page";
    private const string ActionsAttributeName = "asp-actions";
    private const string ResourcesAttributeName = "asp-resources";
    private const string ClassAttributeName = "asp-active-class";
    private const string DisableRenderActiveAttributeName = "asp-disable-render-active";

    [HtmlAttributeName(PageAttributeName)]
    public string? Page { get; set; }

    [HtmlAttributeName(ActionsAttributeName)]
    public string? Actions { get; set; }

    [HtmlAttributeName(ResourcesAttributeName)]
    public string? Resources { get; set; }

    [HtmlAttributeName(ClassAttributeName)]
    public string Class { get; set; } = "usa-current";

    [HtmlAttributeName(DisableRenderActiveAttributeName)]
    public bool DisableRenderActive { get; set; }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (DisableRenderActive)
        {
            base.Process(context, output);
            return;
        }

        var routeValues = ViewContext.RouteData.Values;
        var currentPage = routeValues["page"]?.ToString() ?? string.Empty;
        var (currentResource, currentMethod) = GetResourceAndMethod(currentPage);

        if (string.IsNullOrEmpty(Page))
        {
            Page = currentResource;
        }

        var acceptedMethods =
            Actions?.Trim()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray() ?? [];

        var acceptedResources =
            Resources?.Trim()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray() ?? [];

        var (resource, _) = GetResourceAndMethod(Page ?? string.Empty);

        if ((acceptedMethods.Length == 0 || acceptedMethods.Contains(currentMethod))
            && (acceptedResources.Contains(currentResource) || resource == currentResource))
        {
            SetAttribute(output, "class", Class);
        }

        base.Process(context, output);
    }

    private static void SetAttribute(TagHelperOutput output, string attributeName, string value, bool merge = true)
    {
        var v = value;
        if (output.Attributes.TryGetAttribute(attributeName, out var attribute) && merge)
        {
            v = $"{attribute.Value} {value}";
        }
        output.Attributes.SetAttribute(attributeName, v);
    }

    private static (string? resource, string? method) GetResourceAndMethod(string page)
    {
        var segments = page?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];

        return segments.Length switch
        {
            0 => (default, default),
            1 => (segments[0], default),
            _ => (segments[0], segments[1])
        };
    }
}
