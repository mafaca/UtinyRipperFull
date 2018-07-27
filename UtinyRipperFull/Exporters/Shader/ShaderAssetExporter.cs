using System.Collections.Generic;
using System.IO;
using UtinyRipper;
using UtinyRipper.AssetExporters;
using UtinyRipper.Classes;
using UtinyRipper.Classes.Shaders;
using UtinyRipper.Classes.Shaders.Exporters;

namespace UtinyRipperFull.Exporters
{
	public class ShaderAssetExporter : IAssetExporter
	{
		public bool IsHandle(Object asset)
		{
			return true;
		}

		public void Export(IExportContainer container, Object asset, string path)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
			{
				Shader shader = (Shader)asset;
				shader.ExportBinary(container, fileStream, ShaderExporterInstantiator);
			}
		}

		public void Export(IExportContainer container, IEnumerable<Object> assets, string path)
		{
			foreach (Object asset in assets)
			{
				Export(container, asset, path);
			}
		}

		public IExportCollection CreateCollection(Object asset)
		{
			return new AssetExportCollection(this, asset);
		}

		public AssetType ToExportType(Object asset)
		{
			ToUnknownExportType(asset.ClassID, out AssetType assetType);
			return assetType;
		}

		public bool ToUnknownExportType(ClassIDType classID, out AssetType assetType)
		{
			assetType = AssetType.Meta;
			return true;
		}

		private static ShaderTextExporter ShaderExporterInstantiator(ShaderGpuProgramType programType)
		{
			if(programType.IsDX())
			{
				return new ShaderDXExporter();
			}
			return Shader.DefaultShaderExporterInstantiator(programType);
		}
	}
}
