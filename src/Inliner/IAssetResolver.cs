using System.Collections.Generic;

namespace Inliner
{
    internal interface IAssetResolver
    {
        /// <summary>
        /// Resolve all virtual paths.
        /// </summary>
        /// <param name="paths">List of requested virtual paths.</param>
        /// <returns>List of Assets</returns>
        IEnumerable<Asset> ResolveUrls(string[] paths);
    }
}
