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

			ulong exportID = GetExportIDSafe(asset);
			if(exportID == 0)
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
		
		private static EngineGUID GetGUIDForAsset(Object asset)
		{
			EngineGUID guid = GetGUIDForFile(asset.File.Name);
			if(guid != default)
			{
				return guid;
			}

			return FGUID;
		}

		private static ulong GetExportIDSafe(Object asset)
		{
			switch (asset.ClassID)
			{
				case ClassIDType.Material:
					{
						Material material = (Material)asset;
						switch (material.Name)
						{
							case "Default-Particle":
								return 10301;
							case "Default-Diffuse":
								return 10302;
							case "Default-Material":
								return 10303;
							case "Default-Skybox":
								return 10304;
							case "Default-Line":
								return 10306;
							case "Sprites-Default":
								return 10754;
							case "Sprites-Mask":
								return 10758;
							case "FrameDebuggerRenderTargetDisplay":
								return 10756;
							case "SpatialMappingOcclusion":
								return 15302;
							case "SpatialMappingWireframe":
								return 15303;
						}
					}
					break;

				case ClassIDType.Texture2D:
					{
						Texture2D texture = (Texture2D)asset;
						switch (texture.Name)
						{
							case "Soft":
								return 10001;
							case "Default-Particle":
								return 10300;
							case "Default-Checker":
								return 10305;
							case "Checkmark":
								return 10900;
							case "UISprite":
								return 10904;
							case "Background":
								return 10906;
							case "InputFieldBackground":
								return 10910;
							case "Knob":
								return 10912;
							case "DropdownArrow":
								return 10914;
							case "UIMask":
								return 10916;
						}
					}
					break;

				case ClassIDType.Mesh:
					{
						Mesh mesh = (Mesh)asset;
						switch (mesh.Name)
						{
							case "Cube":
								return 10202;
							case "Cylinder":
								return 10206;
							case "Sphere":
								return 10207;
							case "Capsule":
								return 10208;
							case "Plane":
								return 10209;
							case "Quad":
								return 10210;
						}
					}
					break;

				case ClassIDType.Shader:
					{
						Shader shader = (Shader)asset;
						switch (shader.ShaderName)
						{
							case "Normal-DiffuseFast":
							case "Legacy Shaders/Diffuse Fast":
								return 1;
							case "Legacy Shaders/Bumped":
							case "Legacy Shaders/Bumped Diffuse":
								return 2;
							case "Normal-Glossy":
							case "Legacy Shaders/Specular":
								return 3;
							case "Normal-BumpSpec":
							case "Legacy Shaders/Bumped Specular":
								return 4;
							case "Normal-DiffuseDetail":
							case "Legacy Shaders/Diffuse Detail":
								return 5;
							case "Normal-VertexLit":
							case "Legacy Shaders/VertexLit":
								return 6;
							case "Normal-Diffuse":
							case "Legacy Shaders/Diffuse":
								return 7;
							case "Normal-Parallax":
							case "Legacy Shaders/Parallax Diffuse":
								return 8;
							case "Normal-ParallaxSpec":
							case "Legacy Shaders/Parallax Specular":
								return 9;
							case "Illumin-Diffuse":
							case "Legacy Shaders/Self-Illumin/Diffuse":
								return 10;
							case "Illumin-Bumped":
							case "Legacy Shaders/Self-Illumin/Bumped Diffuse":
								return 11;
							case "Illumin-Glossy":
							case "Legacy Shaders/Self-Illumin/Specular":
								return 12;
							case "Illumin-BumpSpec":
							case "Legacy Shaders/Self-Illumin/Bumped Specular":
								return 13;
							case "Illumin-VertexLit":
							case "Legacy Shaders/Self-Illumin/VertexLit":
								return 14;
							case "Illumin-Parallax":
							case "Legacy Shaders/Self-Illumin/Parallax Diffuse":
								return 15;
							case "Illumin-ParallaxSpec":
							case "Legacy Shaders/Self-Illumin/Parallax Specular":
								return 16;
							case "Reflect-Diffuse":
							case "Legacy Shaders/Reflective/Diffuse":
								return 20;
							case "Reflect-Bumped":
							case "Legacy Shaders/Reflective/Bumped Diffuse":
								return 21;
							case "Reflect-Glossy":
							case "Legacy Shaders/Reflective/Specular":
								return 22;
							case "Reflect-BumpSpec":
							case "Legacy Shaders/Reflective/Bumped Specular":
								return 23;
							case "Reflect-VertexLit":
							case "Legacy Shaders/Reflective/VertexLit":
								return 24;
							case "Reflect-BumpNolight":
							case "Legacy Shaders/Reflective/Bumped Unlit":
								return 25;
							case "Reflect-BumpVertexLit":
							case "Legacy Shaders/Reflective/Bumped VertexLit":
								return 26;
							case "Reflect-Parallax":
							case "Legacy Shaders/Reflective/Parallax Diffuse":
								return 27;
							case "Reflect-ParallaxSpec":
							case "Legacy Shaders/Reflective/Parallax Specular":
								return 28;
							case "Alpha-Diffuse":
							case "Legacy Shaders/Transparent/Diffuse":
								return 30;
							case "Alpha-Bumped":
							case "Legacy Shaders/Transparent/Bumped Diffuse":
								return 31;
							case "Alpha-Glossy":
							case "Legacy Shaders/Transparent/Specular":
								return 32;
							case "Alpha-BumpSpec":
							case "Legacy Shaders/Transparent/Bumped Specular":
								return 33;
							case "Alpha-VertexLit":
							case "Legacy Shaders/Transparent/VertexLit":
								return 34;
							case "Alpha-Parallax":
							case "Legacy Shaders/Transparent/Parallax Diffuse":
								return 35;
							case "Alpha-ParallaxSpec":
							case "Legacy Shaders/Transparent/Parallax Specular":
								return 36;
							case "Lightmap-VertexLit":
							case "Legacy Shaders/Lightmapped/VertexLit":
								return 40;
							case "Lightmap-Diffuse":
							case "Legacy Shaders/Lightmapped/Diffuse":
								return 41;
							case "Lightmap-Bumped":
							case "Legacy Shaders/Lightmapped/Bumped Diffuse":
								return 42;
							case "Lightmap-Glossy":
							case "Legacy Shaders/Lightmapped/Specular":
								return 43;
							case "Lightmap-BumpSpec":
							case "Legacy Shaders/Lightmapped/Bumped Specular":
								return 44;
							case "AlphaTest-VertexLit":
							case "Legacy Shaders/Transparent/Cutout/VertexLit":
								return 50;
							case "AlphaTest-Diffuse":
							case "Legacy Shaders/Transparent/Cutout/Diffuse":
								return 51;
							case "AlphaTest-Bumped":
							case "Legacy Shaders/Transparent/Cutout/Bumped Diffuse":
								return 52;
							case "AlphaTest-Glossy":
							case "Legacy Shaders/Transparent/Cutout/Specular":
								return 53;
							case "AlphaTest-BumpSpec":
							case "Legacy Shaders/Transparent/Cutout/Bumped Specular":
								return 54;
							case "Decal":
							case "Legacy Shaders/Decal":
								return 100;

							case "StandardSpecular":
							case "Standard (Specular setup)":
								return 45;
							case "Standard":
								return 46;
							case "StandardRoughness":
							case "Standard (Roughness setup)":
								return 47;

							case "FX/Flare":
								return 101;

							case "skybox cubed":
							case "Skybox/Cubemap":
								return 103;
							case "Skybox":
							case "Skybox/6 Sided":
								return 104;
							case "Skybox/Procedural":
								return 106;
							case "Skybox/Panoramic":
								return 108;

							case "Particle Add":
							case "Particles/Additive":
								return 200;
							case "Particle AddMultiply":
							case "Particles/~Additive-Multiply":
								return 200;
							case "Particle AddSmooth":
							case "Particles/Additive (Soft)":
								return 202;
							case "Particle Alpha Blend":
							case "Particles/Alpha Blended":
								return 203;
							case "Particle Multiply":
							case "Particles/Multiply":
								return 205;
							case "Particle MultiplyDouble":
							case "Particles/Multiply (Double)":
								return 206;
							case "Particle Premultiply Blend":
							case "Particles/Alpha Blended Premultiply":
								return 207;
							case "Particle VertexLit Blended":
							case "Particles/VertexLit Blended":
								return 208;
							case "Particle Anim Alpha Blend":
							case "Particles/Anim Alpha Blended":
								return 209;
							case "Particle Standard Surface":
							case "Particles/Standard Surface":
								return 210;
							case "Particle Standard Unlit":
							case "Particles/Standard Unlit":
								return 211;

							case "Font":
							case "GUI/Text Shader":
								return 10101;

							case "FirstPass":
							case "Nature/Terrain/Diffuse":
								return 10505;
							case "TreeSoftOcclusionBark":
							case "Nature/Tree Soft Occlusion Bark":
								return 10509;
							case "TreeSoftOcclusionLeaves":
							case "Nature/Tree Soft Occlusion Leaves":
								return 10511;

							case "AlphaTest-SoftEdgeUnlit":
							case "Legacy Shaders/Transparent/Cutout/Soft Edge Unlit":
								return 10512;

							case "TreeCreatorBark":
							case "Nature/Tree Creator Bark":
								return 10600;
							case "TreeCreatorLeaves":
							case "Nature/Tree Creator Leaves":
								return 10601;
							case "TreeCreatorLeavesFast":
							case "Nature/Tree Creator Fast":
								return 10606;
							case "TerrBumpFirstPass":
							case "Nature/Terrain/Specular":
								return 10620;
							case "Nature/Terrain/Standard":
								return 10623;

							case "Mobile-Skybox":
							case "Mobile/Skybox":
								return 10700;
							case "Mobile-VertexLit":
							case "Mobile/VertexLit":
								return 10701;
							case "Mobile-Diffuse":
							case "Mobile/Diffuse":
								return 10703;
							case "Mobile-Bumped":
							case "Mobile/Bumped Diffuse":
								return 10704;
							case "Mobile-BumpSpec":
							case "Mobile/Bumped Specular":
								return 10705;
							case "Mobile-BumpSpec-1DirectionalLight":
							case "Mobile/Bumped Specular (1 Directional Light)":
								return 10706;
							case "Mobile-VertexLit-OnlyDirectionalLights":
							case "Mobile/VertexLit (Only Directional Lights)":
								return 10707;
							case "Mobile-Lightmap-Unlit":
							case "Mobile/Unlit (Supports Lightmap)":
								return 10708;
							case "Mobile-Particle-Add":
							case "Mobile/Particles/Additive":
								return 10720;
							case "Mobile-Particle-Alpha":
							case "Mobile/Particles/Alpha Blended":
								return 10721;
							case "Mobile-Particle-Alpha-VertexLit":
							case "Mobile/Particles/VertexLit Blended":
								return 10722;
							case "Mobile-Particle-Multiply":
							case "Mobile/Particles/Multiply":
								return 10723;

							case "Sprites-Default":
							case "Sprites/Default":
								return 10753;

							case "Unlit-Alpha":
							case "Unlit/Transparent":
								return 10750;
							case "Unlit-AlphaTest":
							case "Unlit/Transparent Cutout":
								return 10751;
							case "Unlit-Normal":
							case "Unlit/Texture":
								return 10752;
							case "Unlit-Color":
							case "Unlit/Color":
								return 10755;

							case "Sprites-Mask":
							case "Sprites/Mask":
								return 10757;

							case "UI-Unlit-Transparent":
							case "UI/Unlit/Transparent":
								return 10760;
							case "UI-Unlit-Detail":
							case "UI/Unlit/Detail":
								return 10761;
							case "UI-Unlit-Text":
							case "UI/Unlit/Text":
								return 10762;
							case "UI-Unlit-TextDetail":
							case "UI/Unlit/Text Detail":
								return 10763;
							case "UI-Lit-Bumped":
							case "UI/Lit/Bumped":
								return 10765;
							case "UI-Lit-Transparent":
							case "UI/Lit/Transparent":
								return 10764;
							case "UI-Lit-Detail":
							case "UI/Lit/Detail":
								return 10766;
							case "UI-Lit-Refraction":
							case "UI/Lit/Refraction":
								return 10767;
							case "UI-Lit-RefractionDetail":
							case "UI/Lit/Refraction Detail":
								return 10768;
							case "UI-Default":
							case "UI/Default":
								return 10770;
							case "UI-DefaultFont":
							case "UI/Default Font":
								return 10782;
							case "UI-DefaultETC1":
							case "UI/DefaultETC1":
								return 10783;

							case "Sprites-Diffuse":
							case "Sprites/Diffuse":
								return 10800;

							case "SpeedTree":
							case "Nature/SpeedTree":
								return 14000;
							case "SpeedTreeBillboard":
							case "Nature/SpeedTree Billboard":
								return 14001;

							case "VR/SpatialMapping/Occlusion":
								return 15300;
							case "VR/SpatialMapping/Wireframe":
								return 15301;

							case "AR/TangoARRender":
								return 15401;
						}
					}
					break;

				case ClassIDType.Sprite:
					{
						Sprite sprite = (Sprite)asset;
						switch (sprite.Name)
						{
							case "Checkmark":
								return 10901;
							case "UISprite":
								return 10905;
							case "Background":
								return 10907;
							case "InputFieldBackground":
								return 10911;
							case "Knob":
								return 10913;
							case "DropdownArrow":
								return 10915;
							case "UIMask":
								return 10917;
						}
					}
					break;
			}
			return 0;
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
			ulong exportID = GetExportIDSafe(asset);
			if(exportID == 0)
			{
				throw new NotImplementedException($"Unknown ExportID for asset {asset.ToLogString()} from file {asset.File.Name}");
			}
			return exportID;
		}

		public ExportPointer CreateExportPointer(Object asset, bool isLocal)
		{
#if DEBUG
			if(isLocal)
			{
				throw new NotSupportedException();
			}
#endif
			ulong exportID = GetExportID(asset);
			EngineGUID guid = GetGUIDForAsset(asset);
			return new ExportPointer(exportID, guid, AssetType.Internal);
		}

		public ISerializedFile File { get; }
		public IEnumerable<Object> Assets => m_assets;
		public string Name => "Engine 2017.3.0f3";

		private static readonly EngineGUID EGUID;
		private static readonly EngineGUID FGUID;

		private readonly HashSet<Object> m_assets = new HashSet<Object>();

		private readonly static Regex m_sharedName = new Regex("sharedassets[0-9]+.assets");
	}
}
