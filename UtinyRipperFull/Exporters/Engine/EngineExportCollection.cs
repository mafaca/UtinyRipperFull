using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UtinyRipper;
using UtinyRipper.AssetExporters;
using UtinyRipper.AssetExporters.Classes;
using UtinyRipper.Classes;
using UtinyRipper.SerializedFiles;

using Object = UtinyRipper.Classes.Object;

namespace UtinyRipperFull.Exporters
{
	public class EngineExportCollection : IExportCollection
	{
		static EngineExportCollection()
		{
			EGUID = new EngineGUID(0x00000000, 0xE0000000, 0x00000000, 0x00000000);
			FGUID = new EngineGUID(0x00000000, 0xF0000000, 0x00000000, 0x00000000);
		}

		public EngineExportCollection(Object asset)
		{
			if (asset == null)
			{
				throw new ArgumentNullException(nameof(asset));
			}

			File = asset.File;
			if (IsEngineFile(asset.File.Name))
			{
				foreach (Object otherAsset in File.FetchAssets())
				{
					m_assets.Add(otherAsset);
				}
			}
			else
			{
				m_assets.Add(asset);
			}
		}
				
		public static bool IsEngineAsset(Object asset)
		{
			if (IsEngineFile(asset.File.Name))
			{
				return true;
			}

			EngineBuiltInAsset engineAsset = GetEngineBuildInAsset(asset);
			if(!engineAsset.IsValid)
			{
				return false;
			}

			switch(asset.ClassID)
			{
				case ClassIDType.Material:
				case ClassIDType.Shader:
					break;

				default:
					return false;
			}
			
			switch(asset.ClassID)
			{
				case ClassIDType.Material:
					{
						Material meterial = (Material)asset;
						Shader shader = meterial.Shader.FindObject(meterial.File);
						if(shader == null)
						{
							return true;
						}
						return IsEngineAsset(shader);
					}

				default:
					return true;
			}
		}

		private static bool IsEngineFile(string fileName)
		{
			EngineGUID guid = GetGUIDForFile(fileName);
			return guid != default;
		}

		private static EngineGUID GetGUIDForFile(string fileName)
		{
			switch (fileName)
			{
				case "unity default resources":
				case "library/unity default resources":
					return EGUID;

				case "unity_builtin_extra":
				case "resources/unity_builtin_extra":
					return FGUID;

				default:
					return default;
			}
		}

		private static EngineBuiltInAsset GetEngineBuildInAsset(Object asset)
		{
			switch (asset.ClassID)
			{
				case ClassIDType.Material:
					{
						Material material = (Material)asset;
						if(EngineBuiltInAssets.Materials.TryGetValue(material.Name, out EngineBuiltInAsset engineAsset))
						{
							return engineAsset;
						}
					}
					break;

				case ClassIDType.Texture2D:
					{
						Texture2D texture = (Texture2D)asset;
						if (EngineBuiltInAssets.Textures.TryGetValue(texture.Name, out EngineBuiltInAsset engineAsset))
						{
							return engineAsset;
						}
					}
					break;

				case ClassIDType.Mesh:
					{
						Mesh mesh = (Mesh)asset;
						if (EngineBuiltInAssets.Meshes.TryGetValue(mesh.Name, out EngineBuiltInAsset engineAsset))
						{
							return engineAsset;
						}
					}
					break;

				case ClassIDType.Shader:
					{
						Shader shader = (Shader)asset;
						if (EngineBuiltInAssets.Shaders.TryGetValue(shader.ShaderName, out EngineBuiltInAsset engineAsset))
						{
							return engineAsset;
						}
					}
					break;

				case ClassIDType.Font:
					{
						Font font = (Font)asset;
						if (EngineBuiltInAssets.Fonts.TryGetValue(font.Name, out EngineBuiltInAsset engineAsset))
						{
							return engineAsset;
						}
					}
					break;

				case ClassIDType.Sprite:
					{
						Sprite sprite = (Sprite)asset;
						if (EngineBuiltInAssets.Sprites.TryGetValue(sprite.Name, out EngineBuiltInAsset engineAsset))
						{
							return engineAsset;
						}
					}
					break;

				case ClassIDType.LightmapParameters:
					{
						LightmapParameters lightParams = (LightmapParameters)asset;
						if (EngineBuiltInAssets.LightmapParams.TryGetValue(lightParams.Name, out EngineBuiltInAsset engineAsset))
						{
							return engineAsset;
						}
					}
					break;
			}
			return default;
		}

		public bool Export(ProjectAssetContainer container, string dirPath)
		{
			return false;
		}

		public bool IsContains(Object asset)
		{
			return m_assets.Contains(asset);
		}

		public ulong GetExportID(Object asset)
		{
			EngineBuiltInAsset engneAsset = GetEngineBuildInAsset(asset);
			if(!engneAsset.IsValid)
			{
				throw new NotImplementedException($"Unknown ExportID for asset {asset.ToLogString()} from file {asset.File.Name}");
			}
			return engneAsset.ExportID;
		}

		public ExportPointer CreateExportPointer(Object asset, bool isLocal)
		{
#if DEBUG
			if(isLocal)
			{
				throw new NotSupportedException();
			}
#endif
			EngineBuiltInAsset engneAsset = GetEngineBuildInAsset(asset);
			ulong exportID = engneAsset.ExportID;
			EngineGUID guid = engneAsset.IsDefault ? FGUID : EGUID;
			return new ExportPointer(exportID, guid, AssetType.Internal);
		}

		public ISerializedFile File { get; }
		public IEnumerable<Object> Assets => m_assets;
		public string Name => "Engine 2017.3.0f3";

		private static readonly EngineGUID EGUID;
		private static readonly EngineGUID FGUID;

		private readonly HashSet<Object> m_assets = new HashSet<Object>();
	}
}
