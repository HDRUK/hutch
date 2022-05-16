using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LinkLiteManager.Extensions;

public static class ModelStateDictionaryExtensions
{
  /// <summary>
  /// Collapses a ModelStateDictionary's Errors to a simple list
  /// containing the key (if any) and error message of each item only.
  /// </summary>
  /// <param name="modelState"></param>
  /// <returns></returns>
  public static List<string> CollapseErrors(this ModelStateDictionary modelState)
    => modelState.Keys
        .SelectMany(k => modelState[k]?.Errors
          .Select(x => !string.IsNullOrWhiteSpace(k)
            ? $"{k}: {x.ErrorMessage}"
            : x.ErrorMessage) ?? new List<string>())
        .ToList();
}
