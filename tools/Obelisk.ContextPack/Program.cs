using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

return ContextPackCli.Run(args);

internal static partial class ContextPackCli
{
    public static int Run(string[] args)
    {
        if (args.Length == 0)
        {
            WriteUsage();
            return 1;
        }

        var command = args[0];
        var options = ParseOptions(args.Skip(1).ToArray());

        try
        {
            return command switch
            {
                "generate-template" => GenerateTemplate(options),
                "generate-brownfield" => GenerateBrownfield(options),
                _ => Fail($"Unknown command '{command}'.")
            };
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static int GenerateTemplate(IReadOnlyDictionary<string, string> options)
    {
        var repoRoot = RequireDirectory(options, "repo");
        var facts = RepositoryFacts.Load(repoRoot);

        TemplateOutputs.Write(facts);

        Console.WriteLine($"Generated Context Pack facts for '{facts.RepositoryName}'.");
        return 0;
    }

    private static int GenerateBrownfield(IReadOnlyDictionary<string, string> options)
    {
        var repoRoot = RequireDirectory(options, "repo");
        var baselineRoot = RequireDirectory(options, "baseline");

        var repoFacts = RepositoryFacts.Load(repoRoot);
        var baselineFacts = RepositoryFacts.Load(baselineRoot);

        TemplateOutputs.Write(repoFacts);
        BrownfieldOutputs.Write(repoFacts, baselineFacts);

        Console.WriteLine(
            $"Generated brownfield Context Pack facts for '{repoFacts.RepositoryName}' using baseline '{baselineFacts.RepositoryName}'.");
        return 0;
    }

    private static string RequireDirectory(IReadOnlyDictionary<string, string> options, string key)
    {
        if (!options.TryGetValue(key, out var path) || string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException($"Missing required option '--{key} <path>'.");
        }

        var fullPath = Path.GetFullPath(path);

        if (!Directory.Exists(fullPath))
        {
            throw new InvalidOperationException($"Directory '{fullPath}' does not exist.");
        }

        return fullPath;
    }

    private static Dictionary<string, string> ParseOptions(string[] args)
    {
        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < args.Length; index++)
        {
            var token = args[index];

            if (!token.StartsWith("--", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Unexpected token '{token}'.");
            }

            if (index == args.Length - 1)
            {
                throw new InvalidOperationException($"Option '{token}' is missing a value.");
            }

            options[token[2..]] = args[++index];
        }

        return options;
    }

    private static int Fail(string message)
    {
        Console.Error.WriteLine(message);
        WriteUsage();
        return 1;
    }

    private static void WriteUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project tools/Obelisk.ContextPack -- generate-template --repo <path>");
        Console.WriteLine("  dotnet run --project tools/Obelisk.ContextPack -- generate-brownfield --repo <path> --baseline <path>");
    }
}

internal static class TemplateOutputs
{
    public static void Write(RepositoryFacts facts)
    {
        var outputRoot = facts.GeneratedOutputRoot;
        Directory.CreateDirectory(outputRoot);

        File.WriteAllText(Path.Combine(outputRoot, "repo-map.md"), RenderRepoMap(facts));
        File.WriteAllText(Path.Combine(outputRoot, "dependency-inventory.md"), RenderDependencyInventory(facts));
        File.WriteAllText(Path.Combine(outputRoot, "config-surface.md"), RenderConfigSurface(facts));
        File.WriteAllText(Path.Combine(outputRoot, "auth-surface.md"), RenderAuthSurface(facts));
    }

    private static string RenderRepoMap(RepositoryFacts facts)
    {
        var builder = new StringBuilder();

        builder.AppendLine("# Repository Map");
        builder.AppendLine();
        builder.AppendLine($"Generated from `{facts.RepositoryName}`.");
        builder.AppendLine();
        builder.AppendLine("## Top-level layout");
        builder.AppendLine();
        builder.AppendLine("- `.github/`: CI and repository automation");
        builder.AppendLine("- `devops/`: deployment and infrastructure support assets");
        builder.AppendLine("- `src/`: runtime and test projects");
        builder.AppendLine("- `docs/context/`: Context Pack entrypoint, guidance, and generated fact sheets");
        builder.AppendLine("- `tools/Obelisk.ContextPack/`: generator used to refresh `docs/context/_generated`");
        builder.AppendLine();
        builder.AppendLine("## Solution projects");
        builder.AppendLine();

        foreach (var project in facts.Projects.OrderBy(static item => item.Path, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine(
                $"- `{project.Path}`: `{project.TargetFramework}`; role `{project.Role}`{(project.IsTestProject ? "; test project" : string.Empty)}");
        }

        builder.AppendLine();
        builder.AppendLine("## Recommended editing map");
        builder.AppendLine();
        builder.AppendLine("- Domain commands, entities, and domain events live under `src/OneBeyond.Studio.Obelisk.Domain`.");
        builder.AppendLine("- Query DTOs and handlers live under `src/OneBeyond.Studio.Obelisk.Application`.");
        builder.AppendLine("- Identity and auth primitives live under `src/OneBeyond.Studio.Obelisk.Authentication`.");
        builder.AppendLine("- EF Core data access, seeding, and migrations live under `src/OneBeyond.Studio.Obelisk.Infrastructure`.");
        builder.AppendLine("- HTTP endpoints live under `src/OneBeyond.Studio.Obelisk.WebApi`.");
        builder.AppendLine("- Azure Functions background processing lives under `src/OneBeyond.Studio.Obelisk.Workers`.");
        builder.AppendLine("- Aspire orchestration lives under `src/OneBeyond.Studio.Obelisk.AppHost`.");

        return builder.ToString();
    }

    private static string RenderDependencyInventory(RepositoryFacts facts)
    {
        var builder = new StringBuilder();

        builder.AppendLine("# Dependency Inventory");
        builder.AppendLine();
        builder.AppendLine("## Central package management");
        builder.AppendLine();
        builder.AppendLine("Versions are pinned in `Directory.Packages.props`.");
        builder.AppendLine();

        foreach (var package in facts.CentralPackages)
        {
            builder.AppendLine($"- `{package.Name}`: `{package.Version}`");
        }

        builder.AppendLine();
        builder.AppendLine("## Project package references");
        builder.AppendLine();

        foreach (var project in facts.Projects.OrderBy(static item => item.Path, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"### `{project.Name}`");
            builder.AppendLine();

            if (project.PackageReferences.Count == 0 && project.ProjectReferences.Count == 0)
            {
                builder.AppendLine("- No direct package or project references were discovered.");
                builder.AppendLine();
                continue;
            }

            foreach (var packageReference in project.PackageReferences)
            {
                builder.AppendLine($"- Package: `{packageReference}`");
            }

            foreach (var projectReference in project.ProjectReferences)
            {
                builder.AppendLine($"- Project reference: `{projectReference}`");
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static string RenderConfigSurface(RepositoryFacts facts)
    {
        var builder = new StringBuilder();

        builder.AppendLine("# Configuration Surface");
        builder.AppendLine();
        builder.AppendLine("Generated from `appsettings*.json`, `host.json`, and `local.settings.json`.");
        builder.AppendLine();

        foreach (var configFile in facts.ConfigFiles.OrderBy(static item => item.RelativePath, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"## `{configFile.RelativePath}`");
            builder.AppendLine();

            if (configFile.Entries.Count == 0)
            {
                builder.AppendLine("- No configuration keys discovered.");
                builder.AppendLine();
                continue;
            }

            foreach (var entry in configFile.Entries.OrderBy(static item => item.Key, StringComparer.OrdinalIgnoreCase))
            {
                builder.AppendLine($"- `{entry.Key}`: `{entry.Kind}`");
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static string RenderAuthSurface(RepositoryFacts facts)
    {
        var builder = new StringBuilder();

        builder.AppendLine("# Authentication And Authorization Surface");
        builder.AppendLine();
        builder.AppendLine("## Setup markers");
        builder.AppendLine();

        foreach (var marker in facts.AuthSetupMarkers)
        {
            builder.AppendLine($"- {marker}");
        }

        builder.AppendLine();
        builder.AppendLine("## Controllers");
        builder.AppendLine();

        foreach (var controller in facts.AuthControllers.OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"### `{controller.Name}`");
            builder.AppendLine();
            builder.AppendLine($"- Route prefix: `{controller.RoutePrefix}`");
            builder.AppendLine($"- Class attributes: {FormatInlineList(controller.ClassAttributes)}");

            if (controller.EndpointMarkers.Count == 0)
            {
                builder.AppendLine("- Endpoint markers: none discovered");
                builder.AppendLine();
                continue;
            }

            builder.AppendLine($"- Endpoint markers: {FormatInlineList(controller.EndpointMarkers)}");
            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static string FormatInlineList(IReadOnlyCollection<string> values)
        => values.Count == 0 ? "none" : string.Join(", ", values.OrderBy(static item => item, StringComparer.OrdinalIgnoreCase).Select(static item => $"`{item}`"));
}

internal static class BrownfieldOutputs
{
    public static void Write(RepositoryFacts repoFacts, RepositoryFacts baselineFacts)
    {
        var outputPath = Path.Combine(repoFacts.GeneratedOutputRoot, "baseline-comparison.md");
        File.WriteAllText(outputPath, RenderBaselineComparison(repoFacts, baselineFacts));
    }

    private static string RenderBaselineComparison(RepositoryFacts repoFacts, RepositoryFacts baselineFacts)
    {
        var builder = new StringBuilder();
        var repoProjects = repoFacts.Projects.Select(static item => item.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var baselineProjects = baselineFacts.Projects.Select(static item => item.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var repoPackages = repoFacts.CentralPackages.Select(static item => item.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var baselinePackages = baselineFacts.CentralPackages.Select(static item => item.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var repoConfigKeys = repoFacts.ConfigFiles.SelectMany(static item => item.Entries).Select(static item => item.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var baselineConfigKeys = baselineFacts.ConfigFiles.SelectMany(static item => item.Entries).Select(static item => item.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var repoAuth = repoFacts.AuthControllers.Select(static item => item.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var baselineAuth = baselineFacts.AuthControllers.Select(static item => item.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        builder.AppendLine("# Baseline Comparison");
        builder.AppendLine();
        builder.AppendLine($"Generated by comparing `{repoFacts.RepositoryName}` against baseline `{baselineFacts.RepositoryName}`.");
        builder.AppendLine();
        builder.AppendLine("## Aligned with Obelisk");
        builder.AppendLine();
        WriteValues(builder, repoProjects.Intersect(baselineProjects, StringComparer.OrdinalIgnoreCase).Select(value => $"Shared project: `{value}`"));
        WriteValues(builder, repoPackages.Intersect(baselinePackages, StringComparer.OrdinalIgnoreCase).Select(value => $"Shared package family: `{value}`"));
        WriteValues(builder, repoConfigKeys.Intersect(baselineConfigKeys, StringComparer.OrdinalIgnoreCase).Select(value => $"Shared config key: `{value}`"));
        WriteValues(builder, repoAuth.Intersect(baselineAuth, StringComparer.OrdinalIgnoreCase).Select(value => $"Shared auth/controller surface: `{value}`"));
        builder.AppendLine();
        builder.AppendLine("## Intentional divergence");
        builder.AppendLine();
        builder.AppendLine("- No intentional divergence can be confirmed from repo inspection alone. Human validation is required.");
        builder.AppendLine();
        builder.AppendLine("## Unclear drift");
        builder.AppendLine();
        WriteValues(builder, repoProjects.Except(baselineProjects, StringComparer.OrdinalIgnoreCase).Select(value => $"Project present only in brownfield repo: `{value}`"));
        WriteValues(builder, baselineProjects.Except(repoProjects, StringComparer.OrdinalIgnoreCase).Select(value => $"Project missing compared with baseline: `{value}`"));
        WriteValues(builder, repoPackages.Except(baselinePackages, StringComparer.OrdinalIgnoreCase).Select(value => $"Package family present only in brownfield repo: `{value}`"));
        WriteValues(builder, baselinePackages.Except(repoPackages, StringComparer.OrdinalIgnoreCase).Select(value => $"Package family missing compared with baseline: `{value}`"));
        WriteValues(builder, repoConfigKeys.Except(baselineConfigKeys, StringComparer.OrdinalIgnoreCase).Select(value => $"Config key present only in brownfield repo: `{value}`"));
        WriteValues(builder, baselineConfigKeys.Except(repoConfigKeys, StringComparer.OrdinalIgnoreCase).Select(value => $"Config key missing compared with baseline: `{value}`"));
        WriteValues(builder, repoAuth.Except(baselineAuth, StringComparer.OrdinalIgnoreCase).Select(value => $"Auth/controller surface present only in brownfield repo: `{value}`"));
        WriteValues(builder, baselineAuth.Except(repoAuth, StringComparer.OrdinalIgnoreCase).Select(value => $"Auth/controller surface missing compared with baseline: `{value}`"));
        builder.AppendLine();
        builder.AppendLine("## Approved future pattern pending human confirmation");
        builder.AppendLine();
        builder.AppendLine("- This section is intentionally a placeholder in v1. Use engineering review and chatbot-assisted interviews to turn unclear drift into approved local guidance.");

        return builder.ToString();
    }

    private static void WriteValues(StringBuilder builder, IEnumerable<string> values)
    {
        var wroteAny = false;

        foreach (var value in values.OrderBy(static item => item, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"- {value}");
            wroteAny = true;
        }

        if (!wroteAny)
        {
            builder.AppendLine("- None discovered from repo inspection.");
        }
    }
}

internal sealed partial record RepositoryFacts(
    string RepositoryRoot,
    string RepositoryName,
    string GeneratedOutputRoot,
    IReadOnlyList<ProjectInfo> Projects,
    IReadOnlyList<PackageInfo> CentralPackages,
    IReadOnlyList<ConfigFileInfo> ConfigFiles,
    IReadOnlyList<ControllerInfo> AuthControllers,
    IReadOnlyList<string> AuthSetupMarkers)
{
    public static RepositoryFacts Load(string repositoryRoot)
    {
        var normalizedRoot = Path.GetFullPath(repositoryRoot);
        var repositoryName = Path.GetFileName(normalizedRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var generatedOutputRoot = Path.Combine(normalizedRoot, "docs", "context", "_generated");
        var projects = DiscoverProjects(normalizedRoot);
        var centralPackages = DiscoverCentralPackages(normalizedRoot);
        var configFiles = DiscoverConfigFiles(normalizedRoot);
        var authControllers = DiscoverControllers(normalizedRoot);
        var authSetupMarkers = DiscoverAuthSetupMarkers(normalizedRoot);

        return new RepositoryFacts(
            normalizedRoot,
            repositoryName,
            generatedOutputRoot,
            projects,
            centralPackages,
            configFiles,
            authControllers,
            authSetupMarkers);
    }

    private static IReadOnlyList<ProjectInfo> DiscoverProjects(string repositoryRoot)
    {
        var slnxPath = Directory.EnumerateFiles(repositoryRoot, "*.slnx", SearchOption.TopDirectoryOnly).SingleOrDefault()
            ?? throw new InvalidOperationException($"No .slnx file found in '{repositoryRoot}'.");

        var document = XDocument.Load(slnxPath);

        return document.Descendants("Project")
            .Select(projectElement => (string?)projectElement.Attribute("Path"))
            .Where(static path => !string.IsNullOrWhiteSpace(path))
            .Select(path => ProjectInfo.Load(repositoryRoot, path!))
            .ToList();
    }

    private static IReadOnlyList<PackageInfo> DiscoverCentralPackages(string repositoryRoot)
    {
        var propsPath = Path.Combine(repositoryRoot, "Directory.Packages.props");
        var document = XDocument.Load(propsPath);

        return document.Descendants("PackageVersion")
            .Select(element => new PackageInfo(
                (string?)element.Attribute("Include") ?? string.Empty,
                (string?)element.Attribute("Version") ?? string.Empty))
            .Where(static item => !string.IsNullOrWhiteSpace(item.Name))
            .OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IReadOnlyList<ConfigFileInfo> DiscoverConfigFiles(string repositoryRoot)
    {
        var supportedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "host.json",
            "local.settings.json"
        };

        return Directory.EnumerateFiles(repositoryRoot, "*.json", SearchOption.AllDirectories)
            .Where(path =>
            {
                var fileName = Path.GetFileName(path);
                return fileName.StartsWith("appsettings", StringComparison.OrdinalIgnoreCase) || supportedNames.Contains(fileName);
            })
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .Select(path => ConfigFileInfo.Load(repositoryRoot, path))
            .ToList();
    }

    private static IReadOnlyList<ControllerInfo> DiscoverControllers(string repositoryRoot)
    {
        var controllersRoot = Path.Combine(repositoryRoot, "src", "OneBeyond.Studio.Obelisk.WebApi", "Controllers");
        if (!Directory.Exists(controllersRoot))
        {
            return Array.Empty<ControllerInfo>();
        }

        var defaultRoute = DiscoverDefaultControllerRoute(repositoryRoot);

        return Directory.EnumerateFiles(controllersRoot, "*.cs", SearchOption.TopDirectoryOnly)
            .Select(path => ControllerInfo.Load(path, defaultRoute))
            .Where(static controller => !string.IsNullOrWhiteSpace(controller.Name))
            .ToList();
    }

    private static string DiscoverDefaultControllerRoute(string repositoryRoot)
    {
        var controllerBasePath = Path.Combine(repositoryRoot, "src", "OneBeyond.Studio.Obelisk.WebApi", "Controllers", "ControllerBase.cs");
        if (!File.Exists(controllerBasePath))
        {
            return "unknown";
        }

        var text = File.ReadAllText(controllerBasePath);
        var routeMatch = ControllerRouteRegex().Match(text);
        return routeMatch.Success ? routeMatch.Groups["route"].Value : "unknown";
    }

    private static IReadOnlyList<string> DiscoverAuthSetupMarkers(string repositoryRoot)
    {
        var markers = new List<string>();
        var webApiProgramPath = Path.Combine(repositoryRoot, "src", "OneBeyond.Studio.Obelisk.WebApi", "Program.cs");
        var authExtensionsPath = Path.Combine(
            repositoryRoot,
            "src",
            "OneBeyond.Studio.Obelisk.Authentication",
            "OneBeyond.Studio.Obelisk.Authentication.Application",
            "DependencyInjection",
            "ServiceCollectionExtensions.cs");

        if (File.Exists(webApiProgramPath))
        {
            var text = File.ReadAllText(webApiProgramPath);

            AddMarkerIfPresent(text, "AddApplicationAuthentication", "Web API wires application authentication via `AddApplicationAuthentication`.", markers);
            AddMarkerIfPresent(text, "AddUserStore<AuthUserStore>", "JWT authentication uses `AuthUserStore` as the user store.", markers);
            AddMarkerIfPresent(text, "AddClaimsPrincipalFactory<ApplicationClaimsIdentityFactory>", "Claims principal generation uses `ApplicationClaimsIdentityFactory`.", markers);
            AddMarkerIfPresent(text, "AddOpenApi(\"v1\")", "OpenAPI is registered per API version; v1 is currently enabled.", markers);
            AddMarkerIfPresent(text, "UseAuthentication();", "Web API enables authentication middleware.", markers);
            AddMarkerIfPresent(text, "UseAuthorization();", "Web API enables authorization middleware.", markers);
        }

        if (File.Exists(authExtensionsPath))
        {
            var text = File.ReadAllText(authExtensionsPath);

            AddMarkerIfPresent(text, "AddJwtAuthentication", "JWT bearer authentication is part of the combined auth setup.", markers);
            AddMarkerIfPresent(text, "AddMicrosoftIdentityWebApp", "Azure AD web app authentication is enabled.", markers);
            AddMarkerIfPresent(text, "ConfigureApplicationCookie", "Cookie authentication is configured explicitly for app sessions.", markers);
            AddMarkerIfPresent(text, "UseCombinedScheme", "Authentication uses a combined scheme across identity and JWT.", markers);
        }

        return markers.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(static item => item, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static void AddMarkerIfPresent(string text, string probe, string marker, ICollection<string> markers)
    {
        if (text.Contains(probe, StringComparison.Ordinal))
        {
            markers.Add(marker);
        }
    }

    [GeneratedRegex("\\[Route\\(\"(?<route>[^\"]+)\"\\)\\]")]
    private static partial Regex ControllerRouteRegex();
}

internal sealed record ProjectInfo(
    string Name,
    string Path,
    string TargetFramework,
    string Role,
    bool IsTestProject,
    IReadOnlyList<string> PackageReferences,
    IReadOnlyList<string> ProjectReferences)
{
    public static ProjectInfo Load(string repositoryRoot, string relativePath)
    {
        var fullPath = System.IO.Path.Combine(repositoryRoot, relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));
        var document = XDocument.Load(fullPath);
        var projectName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
        var targetFramework = document.Descendants("TargetFramework").Select(static element => element.Value).FirstOrDefault()
            ?? XDocument.Load(System.IO.Path.Combine(repositoryRoot, "Directory.Build.props"))
                .Descendants("TargetFramework")
                .Select(static element => element.Value)
                .FirstOrDefault()
            ?? "unknown";
        var packageReferences = document.Descendants("PackageReference")
            .Select(element => (string?)element.Attribute("Include"))
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Select(static item => item!)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var projectReferences = document.Descendants("ProjectReference")
            .Select(element => (string?)element.Attribute("Include"))
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Select(static item => item!.Replace('\\', '/'))
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var isTestProject = document.Descendants("IsTestProject").Any(element => string.Equals(element.Value, "true", StringComparison.OrdinalIgnoreCase))
            || projectName.EndsWith(".Tests", StringComparison.OrdinalIgnoreCase);

        return new ProjectInfo(
            projectName,
            relativePath.Replace('\\', '/'),
            targetFramework,
            InferRole(projectName),
            isTestProject,
            packageReferences,
            projectReferences);
    }

    private static string InferRole(string projectName)
    {
        if (projectName.Contains("AppHost", StringComparison.OrdinalIgnoreCase))
        {
            return "Aspire orchestration";
        }

        if (projectName.Contains("WebApi", StringComparison.OrdinalIgnoreCase))
        {
            return "HTTP API";
        }

        if (projectName.Contains("Workers", StringComparison.OrdinalIgnoreCase))
        {
            return "Azure Functions worker";
        }

        if (projectName.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase))
        {
            return "Infrastructure";
        }

        if (projectName.Contains("Application", StringComparison.OrdinalIgnoreCase))
        {
            return "Application";
        }

        if (projectName.Contains("Domain", StringComparison.OrdinalIgnoreCase))
        {
            return "Domain";
        }

        if (projectName.Contains("Authentication", StringComparison.OrdinalIgnoreCase))
        {
            return "Authentication";
        }

        if (projectName.Contains("ServiceDefaults", StringComparison.OrdinalIgnoreCase))
        {
            return "Aspire service defaults";
        }

        return "Project";
    }
}

internal sealed record PackageInfo(string Name, string Version);

internal sealed record ConfigFileInfo(string RelativePath, IReadOnlyList<ConfigEntryInfo> Entries)
{
    public static ConfigFileInfo Load(string repositoryRoot, string fullPath)
    {
        using var document = JsonDocument.Parse(
            File.ReadAllText(fullPath),
            new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            });

        var entries = new List<ConfigEntryInfo>();
        Flatten(entries, document.RootElement, null);

        return new ConfigFileInfo(Path.GetRelativePath(repositoryRoot, fullPath).Replace('\\', '/'), entries);
    }

    private static void Flatten(ICollection<ConfigEntryInfo> entries, JsonElement element, string? prefix)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject().OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase))
                {
                    var key = prefix is null ? property.Name : $"{prefix}:{property.Name}";
                    entries.Add(new ConfigEntryInfo(key, property.Value.ValueKind.ToString()));
                    Flatten(entries, property.Value, key);
                }

                break;

            case JsonValueKind.Array:
                entries.Add(new ConfigEntryInfo(prefix ?? "(root)", $"Array[{element.GetArrayLength()}]"));
                break;
        }
    }
}

internal sealed record ConfigEntryInfo(string Key, string Kind);

internal sealed partial record ControllerInfo(
    string Name,
    string RoutePrefix,
    IReadOnlyList<string> ClassAttributes,
    IReadOnlyList<string> EndpointMarkers)
{
    public static ControllerInfo Load(string fullPath, string defaultRoute)
    {
        var text = File.ReadAllText(fullPath);
        if (text.Contains("abstract class", StringComparison.Ordinal))
        {
            return new ControllerInfo(string.Empty, string.Empty, Array.Empty<string>(), Array.Empty<string>());
        }

        var classMatch = ControllerNameRegex().Match(text);
        var name = classMatch.Groups["name"].Value;
        var explicitRoute = ControllerRouteRegex().Match(text);
        var routePrefix = explicitRoute.Success
            ? explicitRoute.Groups["route"].Value
            : defaultRoute.Replace("[controller]", name.Replace("Controller", string.Empty, StringComparison.Ordinal));
        var classHeader = classMatch.Success ? text[..classMatch.Index] : text;

        var classAttributes = AttributeRegex()
            .Matches(classHeader)
            .Select(match => match.Groups["attribute"].Value.Trim())
            .Where(static value =>
                value.StartsWith("Authorize", StringComparison.Ordinal)
                || value.StartsWith("AllowAnonymous", StringComparison.Ordinal)
                || value.StartsWith("ApiVersion", StringComparison.Ordinal)
                || value.StartsWith("ApiVersionNeutral", StringComparison.Ordinal)
                || value.StartsWith("Produces", StringComparison.Ordinal)
                || value.StartsWith("Route", StringComparison.Ordinal))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var endpointMarkers = HttpAttributeRegex()
            .Matches(text)
            .Select(match =>
            {
                var route = match.Groups["route"].Success ? match.Groups["route"].Value : "/";
                return $"{match.Groups["verb"].Value} {route}";
            })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new ControllerInfo(name, routePrefix, classAttributes, endpointMarkers);
    }

    [GeneratedRegex(@"class\s+(?<name>[A-Za-z0-9_]+)")]
    private static partial Regex ControllerNameRegex();

    [GeneratedRegex(@"\[(?<attribute>[A-Za-z0-9_]+(?:\([^\]]*\))?)\]")]
    private static partial Regex AttributeRegex();

    [GeneratedRegex("\\[(?<verb>Http(?:Get|Post|Put|Delete|Patch))(\\((\"(?<route>[^\"]*)\")?\\))?\\]")]
    private static partial Regex HttpAttributeRegex();

    [GeneratedRegex("\\[Route\\(\"(?<route>[^\"]+)\"\\)\\]")]
    private static partial Regex ControllerRouteRegex();
}
