using System;
using System.Collections.Generic;
using UtinyRipper;
using UtinyRipper.AssetExporters;

using Object = UtinyRipper.Classes.Object;

namespace UtinyRipperFull.Exporters
{
	public class EngineAssetExporter : IAssetExporter
	{
		public bool IsHandle(Object asset)
		{
			return EngineExportCollection.IsEngineAsset(asset);
		}

		public IExportCollection CreateCollection(Object asset)
		{
			return new EngineExportCollection(asset);
		}

		public void Export(IExportContainer container, Object asset, string path)
		{
			throw new NotSupportedException();
		}

		public void Export(IExportContainer container, Object asset, string path, Action<IExportContainer, Object, string> callback)
		{
			throw new NotSupportedException();
		}

		public void Export(IExportContainer container, IEnumerable<Object> assets, string path)
		{
			throw new NotSupportedException();
		}

		public void Export(IExportContainer container, IEnumerable<Object> assets, string path, Action<IExportContainer, Object, string> callback)
		{
			throw new NotSupportedException();
		}

		public AssetType ToExportType(Object asset)
		{
			return AssetType.Internal;
		}

		public bool ToUnknownExportType(ClassIDType classID, out AssetType assetType)
		{
			assetType = AssetType.Internal;
			return false;
		}
	}
}
