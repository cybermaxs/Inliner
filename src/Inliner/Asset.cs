namespace Inliner
{
    internal struct Asset
    {
        public string VirtualPath { get; private set; }
        public AssetType Type {get; private set; }

        public Asset(string virtualPath, AssetType type)
        {
            VirtualPath = virtualPath;
            Type = type;
        }
    }
}
