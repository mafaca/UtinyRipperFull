using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DotNetDxc;
using UtinyRipper;
using UtinyRipper.Classes.Shaders.Exporters;

namespace UtinyRipperFull.Exporters
{
	public class ShaderDXExporter : ShaderTextExporter
	{
		static ShaderDXExporter()
		{
			HlslDxcLib.DxcCreateInstanceFn = DefaultDxcLib.GetDxcCreateInstanceFn();
		}

		public override void Export(byte[] shaderData, TextWriter writer)
		{
			int offset = 0;
			int fourCC = BitConverter.ToInt32(shaderData, 6);
			if(fourCC == DXBCFourCC)
			{
				offset = 6;
			}

			int length = shaderData.Length - offset;
			IntPtr unmanagedPointer = Marshal.AllocHGlobal(length);
			Marshal.Copy(shaderData, offset, unmanagedPointer, length);

			unsafe
			{
				D3DCompiler.D3DCompiler.D3DDisassemble(unmanagedPointer, (uint)length, 0, null, out IDxcBlob disassembly);
				string disassemblyText = GetStringFromBlob(disassembly);
				byte[] textBytes = Encoding.UTF8.GetBytes(disassemblyText);
				using (MemoryStream memStream = new MemoryStream(textBytes))
				{
					using (BinaryReader reader = new BinaryReader(memStream))
					{
						Export(reader, writer);
					}
				}
			}

			Marshal.FreeHGlobal(unmanagedPointer);
		}

		internal static string GetStringFromBlob(IDxcLibrary library, IDxcBlob blob)
		{
			unsafe
			{
				blob = library.GetBlobAstUf16(blob);
				return new string(blob.GetBufferPointer(), 0, (int)(blob.GetBufferSize() / 2) - 1);
			}
		}

		private string GetStringFromBlob(IDxcBlob blob)
		{
			return GetStringFromBlob(Library, blob);
		}

		internal IDxcLibrary Library
		{
			get { return (library ?? (library = HlslDxcLib.CreateDxcLibrary())); }
		}

		/// <summary>
		/// 'DXBC' ascii
		/// </summary>
		private const int DXBCFourCC = 0x43425844;

		private IDxcLibrary library;
	}
}
