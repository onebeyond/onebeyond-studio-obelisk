using System;
using System.IO;
using System.Linq;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.WebApi.Helpers;

public static class UIBuildFileHelpers
{
    private const string SCRIPT_FOLDER = "dist";
    private const string WEB_ROOT = "wwwroot";

    public static string GetFileNameFromUIBundle(string expression)
    {
        EnsureArg.IsNotEmptyOrWhiteSpace(expression, nameof(expression));


        var filePathsMatchingExpression = Directory.GetFiles($"{WEB_ROOT}/{SCRIPT_FOLDER}", expression).Select(Path.GetFileName).ToList();

        var filePath = filePathsMatchingExpression.Count == 1
            ? $"{SCRIPT_FOLDER}/{filePathsMatchingExpression.SingleOrDefault()}"
            : throw new FileNotFoundException($"Error retrieving file. There were {filePathsMatchingExpression.Count} files matching the expression {expression}.");

        return filePath;

    }
}
