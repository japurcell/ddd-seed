using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelpers.UnitTests;

[TestClass]
internal class UsaCheckboxTagHelperTests
{
    [TestMethod]
    [DataRow(true, "<input checked=\"checked\" class=\"usa-checkbox__input\" id=\"text\" name=\"text\" type=\"checkbox\" value=\"true\" /><label class=\"usa-checkbox__label\" for=\"text\"> </label><input name=\"text\" type=\"hidden\" value=\"false\" /><span class=\"usa-error-message field-validation-valid\" data-valmsg-for=\"text\" data-valmsg-replace=\"true\"></span>")]
    [DataRow(false, "<input class=\"usa-checkbox__input\" id=\"text\" name=\"text\" type=\"checkbox\" value=\"true\" /><label class=\"usa-checkbox__label\" for=\"text\"> </label><input name=\"text\" type=\"hidden\" value=\"false\" /><span class=\"usa-error-message field-validation-valid\" data-valmsg-for=\"text\" data-valmsg-replace=\"true\"></span>")]
    public async Task CorrectOutputIsRendered(bool isChecked, string expectedHtml)
    {
        var metadataProvider = new EmptyModelMetadataProvider();
        var containerType = typeof(bool);
        var containerExplorer = metadataProvider.GetModelExplorerForType(containerType, isChecked);
        var propertyMetadata = metadataProvider.GetMetadataForType(containerType);
        var modelExplorer = containerExplorer.GetExplorerForExpression(propertyMetadata, isChecked);
        var modelExpression = new ModelExpression("text", modelExplorer);

        var urlHelper = new Mock<IUrlHelper>();
        var generator = new TestableHtmlGenerator(metadataProvider, urlHelper.Object);

        var context = CreateContext();
        var output = CreateOutput("usa-checkbox");

        var viewContext = TestableHtmlGenerator.GetViewContext(
            model: null,
            metadataProvider: metadataProvider);

        var helper = new UsaCheckboxTagHelper(generator)
        {
            For = modelExpression,
            ViewContext = viewContext
        };

        await helper.ProcessAsync(context, output);

        Assert.AreEqual(expectedHtml, output.Content.GetContent());
    }

    private static TagHelperContext CreateContext() =>
        new(
            "usa-checkbox",
            [],
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

    private static TagHelperOutput CreateOutput(
        string tagName,
        TagHelperAttributeList? attributes = null,
        string? childContent = null)
    {
        attributes ??= [];

        return new TagHelperOutput(
            tagName,
            attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(childContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });
    }
}
