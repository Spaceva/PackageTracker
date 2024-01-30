using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework.Extensions;
internal static class FuncExtensions
{
    public static Func<Application, bool> CombineOr(this IEnumerable<Func<Application, bool>> functions)
    {
        ArgumentNullException.ThrowIfNull(functions);
        if (!functions.Any())
        {
            throw new ArgumentException("No function provided", nameof(functions));
        }


        Func<Application, bool> finalFunction = functions.First();
        foreach (var function in functions.Skip(1))
        {
            finalFunction = x => finalFunction(x) || function(x);
        }
        return finalFunction;
    }
}
