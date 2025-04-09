using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace TagHelpers.UnitTests;

[TestClass]
internal class ActiveLinkTag
{
    [TestMethod]
    [DataRow("Ctrl", "Request", "Index", "Index", "")]
    [DataRow("Request", "Request", "Index", "Index", "usa-current")]
    [DataRow("Request", "Request", "", "Index", "usa-current")]
    [DataRow("Request", "Request", "Index,Create", "Create", "usa-current")]
    [DataRow("Request", "Request", "Index,Create", "Edit", "")]
    public async Task OutputCreatedGivenAttributes(
        string controllerAttributeValue,
        string currentController,
        string actions,
        string currentAction,
        string expected)
    {
        var output = CreateOutput("a");
        var context = CreateContext();
        var viewContext = CreateViewContext(currentController, currentAction);
        var helper = new ActiveLinkTagHelper
        {
            Page = controllerAttributeValue,
            Actions = actions,
            ViewContext = viewContext
        };

        await helper.ProcessAsync(context, output);

        Assert.AreEqual(expected, string.Join(" ", output.Attributes.Select(a => a.Value)));
    }

    private static TagHelperContext CreateContext() =>
        new(
            "a",
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

    private static ViewContext CreateViewContext(string controllerName, string actionName)
    {
        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

        var routeData = new RouteData();
        routeData.Values.Add("page", controllerName + "/" + actionName);

        return new ViewContext(
            new ActionContext(new DefaultHttpContext(), routeData, new ActionDescriptor()),
            NullView.Instance,
            viewData,
            Mock.Of<ITempDataDictionary>(),
            TextWriter.Null,
            new HtmlHelperOptions());
    }
}
