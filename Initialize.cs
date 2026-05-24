#!/usr/bin/dotnet run

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

Console.WriteLine("Jellyfin Plugin Initializer");
Console.WriteLine("===========================");

// Gather Inputs
Console.Write("Plugin Name (e.g. MyAwesomePlugin): ");
string? pluginName = Console.ReadLine();
if (string.IsNullOrWhiteSpace(pluginName)) { Console.WriteLine("Name required."); return; }

string defaultNamespace = $"Jellyfin.Plugin.{pluginName}";
Console.Write($"Namespace [{defaultNamespace}]: ");
string ns = Console.ReadLine() ?? "";
if (string.IsNullOrWhiteSpace(ns)) ns = defaultNamespace;

Console.Write("Owner [jellyfin]: ");
string owner = Console.ReadLine() ?? "jellyfin";
if (string.IsNullOrWhiteSpace(owner)) owner = "jellyfin";

Console.Write("Description [A Jellyfin plugin.]: ");
string description = Console.ReadLine() ?? "A Jellyfin plugin.";
if (string.IsNullOrWhiteSpace(description)) description = "A Jellyfin plugin.";

Console.Write("Long Description [A detailed description of the plugin.]: ");
string longDescription = Console.ReadLine() ?? "";
if (string.IsNullOrWhiteSpace(longDescription)) longDescription = description;

Console.Write("Manifest Repository (e.g. username/repo): ");
string manifestRepo = Console.ReadLine() ?? "YOUR_USERNAME/YOUR_MANIFEST_REPO";

string newGuid = Guid.NewGuid().ToString();

Console.WriteLine($"\nGUID: {newGuid}");
Console.Write("Proceed? (y/N): ");
if (Console.ReadKey().Key != ConsoleKey.Y) { Console.WriteLine("\nAborted."); return; }

// Constants
const string oldNamespace = "Jellyfin.Plugin.Template";
const string oldGuid = "eb5d7894-8eef-4b36-aa6f-5d124e828ce1";

// Process Files
var files = Directory.EnumerateFiles(".", "*", SearchOption.AllDirectories)
    .Where(f => !f.Contains("/.git/") && !f.Contains("/bin/") && !f.Contains("/obj/") && !f.Contains("/node_modules/"));

foreach (var file in files)
{
    string relativePath = Path.GetRelativePath(Environment.CurrentDirectory, file);
    
    // Skip this script itself
    if (relativePath == "Initialize.cs") continue;

    string content = File.ReadAllText(file);
    bool changed = false;

    if (content.Contains(oldNamespace)) { content = content.Replace(oldNamespace, ns); changed = true; }
    if (content.Contains(oldGuid)) { content = content.Replace(oldGuid, newGuid); changed = true; }
    
    // Specific updates
    if (relativePath == "build.yaml") {
        content = Regex.Replace(content, "name: \".*\"", $"name: \"{pluginName}\"");
        content = Regex.Replace(content, "owner: \".*\"", $"owner: \"{owner}\"");
        content = Regex.Replace(content, "overview: \".*\"", $"overview: \"{description}\"");
        changed = true;
    }

    if (relativePath == ".github/workflows/release.yaml") {
        content = Regex.Replace(content, "MANIFEST_REPO: '.*'", $"MANIFEST_REPO: '{manifestRepo}'");
        content = Regex.Replace(content, "PLUGIN_GUID: '.*'", $"PLUGIN_GUID: '{newGuid}'");
        content = Regex.Replace(content, "PLUGIN_NAME: '.*'", $"PLUGIN_NAME: '{pluginName}'");
        changed = true;
    }

    if (relativePath == "docker-compose.yml") {
        // Convert PascalCase/TitleCase to kebab-case
        string kebabName = Regex.Replace(pluginName, @"([a-z0-9])([A-Z])", "$1-$2").ToLower();
        content = Regex.Replace(content, "jellyfin-plugin-template", kebabName);
        changed = true;
    }

    if (changed) File.WriteAllText(file, content);
}

// Rename Folders and Files
if (Directory.Exists(oldNamespace)) Directory.Move(oldNamespace, ns);
string oldCsproj = Path.Combine(ns, $"{oldNamespace}.csproj");
if (File.Exists(oldCsproj)) File.Move(oldCsproj, Path.Combine(ns, $"{ns}.csproj"));
if (File.Exists($"{oldNamespace}.sln")) File.Move($"{oldNamespace}.sln", $"{ns}.sln");

Console.WriteLine("\nDone! Initialize repository complete.");

Console.Write("\nDisplay git diff of all changes? (y/N): ");
if (Console.ReadKey().Key == ConsoleKey.Y) {
    Console.WriteLine("\nStaging and displaying changes...");
    System.Diagnostics.Process.Start("git", "add -A").WaitForExit();
    System.Diagnostics.Process.Start("git", "--no-pager diff --cached").WaitForExit();

    Console.WriteLine("\nRestoring staging area...");
    System.Diagnostics.Process.Start("git", "restore --staged .").WaitForExit();
}

Console.Write("\nCommit initialization changes? (y/N): ");
if (Console.ReadKey().Key == ConsoleKey.Y) {
    Console.WriteLine("\nStaging and committing changes...");
    System.Diagnostics.Process.Start("git", "add -A").WaitForExit();
    System.Diagnostics.Process.Start("git", "commit -m \"feat: initialize plugin repository\"").WaitForExit();
    Console.WriteLine("Changes committed.");
}
