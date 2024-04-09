using PackageTracker.Presentation.MVCApp.Models;
using System.Collections.Frozen;
using System.Reflection;
using System.Text;

namespace PackageTracker.Presentation.MVCApp.Modules;

public static class CsvExporter
{
    private static readonly FrozenSet<PropertyInfo> baseApplicationsHeader = typeof(ApplicationDetailViewModel).GetProperties().Where(p => !p.Name.Equals(nameof(ApplicationDetailViewModel.Packages))).ToFrozenSet();
    private static readonly FrozenSet<PropertyInfo> packageApplicationsHeader = typeof(ApplicationDetailViewModel.ApplicationPackage).GetProperties().ToFrozenSet();

    public static string Export(IReadOnlyCollection<ApplicationDetailViewModel> rows)
    {
        var csvContent = new StringBuilder();
        csvContent.AppendLine(string.Join(";", baseApplicationsHeader.Union(packageApplicationsHeader).Select(h => $"\"{h.Name}\"")));
        foreach (var row in rows)
        {
            var startLine = string.Join(";", baseApplicationsHeader.Select(p => $"\"{p.GetValue(row)}\""));
            if (row.Packages.Count == 0)
            {
                var endLine = string.Join(string.Empty, packageApplicationsHeader.SkipLast(1).Select(p => ";"));
                csvContent.AppendLine($"{startLine};{endLine}");
                continue;
            }
            foreach (var package in row.Packages)
            {
                var endLine = string.Join(";", packageApplicationsHeader.Select(p => $"\"{p.GetValue(package)}\""));
                csvContent.AppendLine($"{startLine};{endLine}");
            }
        }

        return csvContent.ToString();
    }
}
