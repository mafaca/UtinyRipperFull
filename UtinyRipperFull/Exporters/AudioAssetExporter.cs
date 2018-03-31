using FMOD;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UtinyRipper;
using UtinyRipper.AssetExporters;
using UtinyRipper.Classes;
using UtinyRipper.Classes.AudioClips;

using Object = UtinyRipper.Classes.Object;

namespace UtinyRipperFull.Exporters
{
	public class AudioAssetExporter : AssetExporter
	{
		public override IExportCollection CreateCollection(Object @object)
		{
			return new AssetExportCollection(this, @object);
		}

		public override bool Export(IAssetsExporter exporter, IExportCollection collection, string dirPath)
		{
			AssetExportCollection asset = (AssetExportCollection)collection;
			AudioClip audioClip = (AudioClip)asset.Asset;
			exporter.File = audioClip.File;
			
			string subFolder = audioClip.ClassID.ToString();
			string subPath = Path.Combine(dirPath, subFolder);
			string fileName = GetUniqueFileName(audioClip, subPath);
			if (IsSupported(audioClip))
			{
				fileName = $"{Path.GetFileNameWithoutExtension(fileName)}.wav";
			}
			string filePath = Path.Combine(subPath, fileName);

			if(!Directory.Exists(subPath))
			{
				Directory.CreateDirectory(subPath);
			}

			exporter.File = audioClip.File;

			using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
			{
				if (IsSupported(audioClip))
				{
					ExportAudioClip(exporter, fileStream, audioClip);
				}
				else
				{
					audioClip.ExportBinary(exporter, fileStream);
				}
			}

			ExportMeta(exporter, asset, filePath);
			return true;
		}
		
		public override AssetType ToExportType(ClassIDType classID)
		{
			return AssetType.Meta;
		}

		private bool IsSupported(AudioClip audioClip)
		{
			if (AudioClip.IsReadType(audioClip.File.Version))
			{
				switch (audioClip.Type)
				{
					case FMODSoundType.AIFF:
					case FMODSoundType.IT:
					case FMODSoundType.MOD:
					case FMODSoundType.S3M:
					case FMODSoundType.XM:
					case FMODSoundType.XMA:
					case FMODSoundType.VAG:
					case FMODSoundType.AUDIOQUEUE:
						return true;
					default:
						return false;
				}
			}
			else
			{
				switch (audioClip.CompressionFormat)
				{
					case AudioCompressionFormat.PCM:
					case AudioCompressionFormat.Vorbis:
					case AudioCompressionFormat.ADPCM:
					case AudioCompressionFormat.MP3:
					case AudioCompressionFormat.VAG:
					case AudioCompressionFormat.HEVAG:
					case AudioCompressionFormat.XMA:
					case AudioCompressionFormat.GCADPCM:
					case AudioCompressionFormat.ATRAC9:
						return true;
					default:
						return false;
				}
			}
		}

		private string GetAudioType(AudioClip audioClip)
		{
			if (AudioClip.IsReadType(audioClip.File.Version))
			{
				return audioClip.Type.ToString();
			}
			else
			{
				return audioClip.CompressionFormat.ToString();
			}
		}
		
		private void ExportAudioClip(IAssetsExporter exporter, FileStream fileStream, AudioClip clip)
		{
			CREATESOUNDEXINFO exinfo = new CREATESOUNDEXINFO();
			FMOD.System system = null;
			Sound sound = null;
			Sound subsound = null;

			try
			{
				RESULT result = Factory.System_Create(out system);
				if (result != RESULT.OK)
				{
					Logger.Instance.Log(LogType.Error, LogCategory.Export, $"Can't create factory for AudioClip {clip.Name}");
					return;
				}

				result = system.init(1, INITFLAGS.NORMAL, IntPtr.Zero);
				if (result != RESULT.OK)
				{
					Logger.Instance.Log(LogType.Error, LogCategory.Export, $"Can't init system for AudioClip {clip.Name}");
					return;
				}

				byte[] data;
				using (MemoryStream memStream = new MemoryStream())
				{
					clip.ExportBinary(exporter, memStream);
					data = memStream.ToArray();
				}
				if (data.Length == 0)
				{
					return;
				}
			
				exinfo.cbsize = Marshal.SizeOf(exinfo);
				exinfo.length = (uint)data.Length;
				result = system.createSound(data, MODE.OPENMEMORY, ref exinfo, out sound);
				if (result != RESULT.OK)
				{
					Logger.Instance.Log(LogType.Error, LogCategory.Export, $"Can't create sound for AudioClip {clip.Name}");
					return;
				}

				result = sound.getSubSound(0, out subsound);
				if (result != RESULT.OK)
				{
					Logger.Instance.Log(LogType.Error, LogCategory.Export, $"Can't get subsound for AudioClip {clip.Name}");
					return;
				}

				result = subsound.getFormat(out SOUND_TYPE type, out SOUND_FORMAT format, out int numChannels, out int bitsPerSample);
				if (result != RESULT.OK)
				{
					Logger.Instance.Log(LogType.Error, LogCategory.Export, $"Can't get format for AudioClip {clip.Name}");
					return;
				}

				result = subsound.getDefaults(out float frequency, out int priority);
				if (result != RESULT.OK)
				{
					Logger.Instance.Log(LogType.Error, LogCategory.Export, $"Can't get defaults for AudioClip {clip.Name}");
					return;
				}

				int sampleRate = (int)frequency;
				result = subsound.getLength(out uint length, TIMEUNIT.PCMBYTES);
				if (result != RESULT.OK)
				{
					Logger.Instance.Log(LogType.Error, LogCategory.Export, $"Can't get length for AudioClip {clip.Name}");
					return;
				}

				result = subsound.@lock(0, length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2);
				if (result != RESULT.OK)
				{
					Logger.Instance.Log(LogType.Error, LogCategory.Export, $"Can't lock for AudioClip {clip.Name}");
					return;
				}

				using (BinaryWriter writer = new BinaryWriter(fileStream))
				{
					writer.Write(Encoding.UTF8.GetBytes("RIFF"));
					writer.Write(len1 + 36);
					writer.Write(Encoding.UTF8.GetBytes("WAVEfmt "));
					writer.Write(16);
					writer.Write((short)1);
					writer.Write((short)numChannels);
					writer.Write(sampleRate);
					writer.Write(sampleRate * numChannels * bitsPerSample / 8);
					writer.Write((short)(numChannels * bitsPerSample / 8));
					writer.Write((short)bitsPerSample);
					writer.Write(Encoding.UTF8.GetBytes("data"));
					writer.Write(len1);

					for (int i = 0; i < len1; i++)
					{
						byte value = Marshal.ReadByte(ptr1, i);
						writer.Write(value);
					}
				}

				result = subsound.unlock(ptr1, ptr2, len1, len2);
				if (result != RESULT.OK)
				{
					Logger.Instance.Log(LogType.Error, LogCategory.Export, $"Can't unlock for AudioClip {clip.Name}");
				}
			}
			finally
			{
				if (subsound != null)
				{
					subsound.release();
				}
				if (sound != null)
				{
					sound.release();
				}
				if (system != null)
				{
					system.release();
				}
			}
		}
	}
}
