using System.IO;
using UtinyRipper.AssetExporters;
using UtinyRipper.Classes;

namespace UtinyRipperFull.Exporters
{
	public class AudioExportCollection : AssetExportCollection
	{
		public AudioExportCollection(IAssetExporter assetExporter, AudioClip asset) :
			base(assetExporter, asset)
		{
		}

		protected override string ExportInner(ProjectAssetContainer container, string filePath)
		{
			AudioClip audioClip = (AudioClip)Asset;
			if (AudioAssetExporter.IsSupported(audioClip))
			{
				string dir = Path.GetDirectoryName(filePath);
				string newName = $"{Path.GetFileNameWithoutExtension(filePath)}.wav";
				filePath = Path.Combine(dir, newName);
			}

			AssetExporter.Export(container, Asset, filePath);
			return filePath;
		}
	}
}
