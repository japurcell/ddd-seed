using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace TagHelpers.UnitTests;

internal class NullView : IView
{
    public static readonly NullView Instance = new();

    public string Path => string.Empty;

    public Task RenderAsync(ViewContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return Task.CompletedTask;
    }
}
