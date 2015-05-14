﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wyam.Modules.Razor
{
    // From https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNet.Mvc.Razor.Host/ViewHierarchyUtility.cs
    /// <summary>
    /// Contains methods to locate <c>_ViewStart.cshtml</c> and <c>_GlobalImport.cshtml</c>
    /// </summary>
    public static class ViewHierarchyUtility
    {
        private const string ViewStartFileName = "_ViewStart.cshtml";
        private const string GlobalImportFileName = "_GlobalImport.cshtml";

        /// <summary>
        /// Gets the view start locations that are applicable to the specified path.
        /// </summary>
        /// <param name="applicationRelativePath">The application relative path of the file to locate
        /// <c>_ViewStart</c>s for.</param>
        /// <returns>A sequence of paths that represent potential view start locations.</returns>
        /// <remarks>
        /// This method returns paths starting from the directory of <paramref name="applicationRelativePath"/> and
        /// moves upwards until it hits the application root.
        /// e.g.
        /// /Views/Home/View.cshtml -> [ /Views/Home/_ViewStart.cshtml, /Views/_ViewStart.cshtml, /_ViewStart.cshtml ]
        /// </remarks>
        public static IEnumerable<string> GetViewStartLocations(string applicationRelativePath)
        {
            return GetHierarchicalPath(applicationRelativePath, ViewStartFileName);
        }

        /// <summary>
        /// Gets the locations for <c>_GlobalImport</c>s that are applicable to the specified path.
        /// </summary>
        /// <param name="applicationRelativePath">The application relative path of the file to locate
        /// <c>_GlobalImport</c>s for.</param>
        /// <returns>A sequence of paths that represent potential <c>_GlobalImport</c> locations.</returns>
        /// <remarks>
        /// This method returns paths starting from the directory of <paramref name="applicationRelativePath"/> and
        /// moves upwards until it hits the application root.
        /// e.g.
        /// /Views/Home/View.cshtml -> [ /Views/Home/_GlobalImport.cshtml, /Views/_GlobalImport.cshtml,
        ///                              /_GlobalImport.cshtml ]
        /// </remarks>
        public static IEnumerable<string> GetGlobalImportLocations(string applicationRelativePath)
        {
            return GetHierarchicalPath(applicationRelativePath, GlobalImportFileName);
        }

        private static IEnumerable<string> GetHierarchicalPath(string relativePath, string fileName)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return Enumerable.Empty<string>();
            }

            if (relativePath.StartsWith("~/", StringComparison.Ordinal))
            {
                relativePath = relativePath.Substring(2);
            }

            if (relativePath.StartsWith("/", StringComparison.Ordinal))
            {
                relativePath = relativePath.Substring(1);
            }

            if (Path.IsPathRooted(relativePath))
            {
                // If the path looks like it's not app relative, don't attempt to construct paths.
                return Enumerable.Empty<string>();
            }

            if (string.Equals(Path.GetFileName(relativePath), fileName, StringComparison.OrdinalIgnoreCase))
            {
                // If the specified path is for the file hierarchy being constructed, then the first file that applies
                // to it is in a parent directory.
                relativePath = Path.GetDirectoryName(relativePath);
                if (string.IsNullOrEmpty(relativePath))
                {
                    return Enumerable.Empty<string>();
                }
            }

            var locations = new List<string>();
            while (!string.IsNullOrEmpty(relativePath))
            {
                relativePath = Path.GetDirectoryName(relativePath);
                var path = Path.Combine(relativePath, fileName);
                locations.Add(path);
            }

            return locations;
        }
    }
}