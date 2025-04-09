using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelpers;

[HtmlTargetElement("usa-checkbox", TagStructure = TagStructure.WithoutEndTag)]
public class UsaCheckboxTagHelper : TagHelper
{
    private const string AriaAttributeName = "aria";
    private const string DataAttributeName = "all-data-values";
    private const string DataAttributePrefix = "asp-data-";
    private const string ForAttributeName = "for";
    private const string IdAttributeName = "id";
    private const string LabelAttributeName = "label-text";
    private const string LabelDescriptionAttributeName = "label-description";
    private const string ReadOnlyAttributeName = "readonly";
    private IDictionary<string, string>? _ariaValues;

    private readonly IHtmlGenerator _generator;

    public UsaCheckboxTagHelper(IHtmlGenerator generator) =>
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));

    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression? For { get; set; }

    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    [HtmlAttributeName(LabelAttributeName)]
    public string? LabelText { get; set; }

    [HtmlAttributeName(LabelDescriptionAttributeName)]
    public string? LabelDescription { get; set; }

    [HtmlAttributeName(ReadOnlyAttributeName)]
    public bool ReadOnly { get; set; }

    [HtmlAttributeName(AriaAttributeName)]
    public IDictionary<string, string> AriaValues
    {
        get
        {
            _ariaValues ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            return _ariaValues;
        }
        set => _ariaValues = value;
    }

    [HtmlAttributeName(DataAttributeName, DictionaryAttributePrefix = DataAttributePrefix)]
    public IDictionary<string, string> DataValues { get; set; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (For == null)
        {
            throw new InvalidOperationException($"The {nameof(For)} property must be set.");
        }

        var isChecked = false;

        if (For.ModelExplorer.ModelType != typeof(bool)
            && Nullable.GetUnderlyingType(For.ModelExplorer.ModelType) != typeof(bool))
        {
            throw new InvalidOperationException($"The Model for {nameof(UsaCheckboxTagHelper)} must be a bool.");
        }
        else if (bool.TryParse(For.Model?.ToString(), out var isCheck))
        {
            isChecked = isCheck;
        }

        var inputAttributes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "class", "usa-checkbox__input" },
            };
        var labelFor = For.Name;

        if (!string.IsNullOrWhiteSpace(Id))
        {
            inputAttributes[IdAttributeName] = Id;
            labelFor = Id;
        }

        if (ReadOnly)
        {
            inputAttributes["disabled"] = "true";
        }

        var checkbox =
            _generator.GenerateCheckBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                isChecked,
                inputAttributes
                    .Concat(AriaValues.ToDictionary(kvp => "aria-" + kvp.Key, kvp => kvp.Value))
                    .Concat(DataValues.ToDictionary(kvp => "data-" + kvp.Key, kvp => kvp.Value))
                .ToDictionary(kv => kv.Key, kv => kv.Value as object));

        if (isChecked)
        {
            checkbox.Attributes["checked"] = "checked";
        }

        var checkBoxBuilder = output.Content.AppendHtml(checkbox);

        var hasLabel = !string.IsNullOrWhiteSpace(LabelText);
        var label =
            _generator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                labelFor,
                LabelText ?? " ",
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { "class", "usa-checkbox__label" },
                });

        if (!string.IsNullOrWhiteSpace(LabelDescription))
        {
            var span = new TagBuilder("span");
            span.InnerHtml.Append(LabelDescription);
            span.AddCssClass("usa-checkbox__label-description");
            label.InnerHtml.AppendHtml(span);
        }

        checkBoxBuilder.AppendHtml(label);

        var hiddenCheckbox =
            _generator.GenerateHiddenForCheckbox(
                ViewContext,
                For.ModelExplorer,
                For.Name);

        checkBoxBuilder.AppendHtml(hiddenCheckbox);

        var validationMessage =
            _generator.GenerateValidationMessage(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                null,
                null,
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { "class", "usa-error-message" },
                });

        checkBoxBuilder.AppendHtml(validationMessage);

        output.TagMode = TagMode.StartTagAndEndTag;
        output.TagName = "div";

        var outerDiv = new TagBuilder("div");

        IEnumerable<string> outerClasses = ["usa-checkbox"];
        outerClasses = hasLabel
            ? outerClasses
            : outerClasses.Concat(["display-flex", "flex-justify-center", "usa-checkbox--no-text"]);
        outerDiv.MergeAttribute("class", string.Join(" ", outerClasses));
        output.MergeAttributes(outerDiv);
    }
}
