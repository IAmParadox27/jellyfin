using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace Emby.Server.Implementations.Plugins;

/// <summary>
/// A custom <see cref="AssemblyLoadContext"/> for loading Jellyfin plugins.
/// </summary>
public class PluginGroupLoadContext : AssemblyLoadContext
{
    private readonly List<AssemblyDependencyResolver> _resolvers;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginGroupLoadContext"/> class.
    /// </summary>
    public PluginGroupLoadContext() : base(true)
    {
        _resolvers = new List<AssemblyDependencyResolver>();
    }

    /// <summary>
    /// Adds an assembly dependency resolver for the given path.
    /// </summary>
    /// <param name="path">The path of the plugin assembly being registered.</param>
    public void RegisterResolverForPath(string path)
    {
        _resolvers.Add(new AssemblyDependencyResolver(path));
    }

    /// <inheritdoc />
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        foreach (var resolver in _resolvers)
        {
            var assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath is not null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
        }

        return null;
    }
}
