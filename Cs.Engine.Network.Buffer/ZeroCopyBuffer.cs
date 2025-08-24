using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cs.Engine.Network.Buffer.Detail;
using Cs.Logging;
using LZ4;

namespace Cs.Engine.Network.Buffer;

public sealed class ZeroCopyBuffer
{
	private sealed class Cleaner : IDisposable
	{
		private readonly ZeroCopyBuffer buffer;

		public Cleaner(ZeroCopyBuffer buffer)
		{
			this.buffer = buffer;
		}

		public void Dispose()
		{
			foreach (TailBuffer tailBuffer in buffer.tailBuffers)
			{
				tailBuffer.ToRecycleBin();
			}
			buffer.tailBuffers.Clear();
			buffer.last = null;
		}
	}

	private readonly Queue<TailBuffer> tailBuffers = new Queue<TailBuffer>();

	private TailBuffer last;

	public int SegmentCount => tailBuffers.Count;

	public int CalcTotalSize()
	{
		int num = 0;
		foreach (TailBuffer tailBuffer in tailBuffers)
		{
			num += tailBuffer.Offset;
		}
		return num;
	}

	public BinaryWriter GetWriter()
	{
		return new BinaryWriter(new ZeroCopyOutputStream(this));
	}

	public BinaryReader GetReader()
	{
		return new BinaryReader(new ZeroCopyInputStream(this));
	}

	public Stream GetOutputStream()
	{
		return new ZeroCopyOutputStream(this);
	}

	public IDisposable Hold()
	{
		return new Cleaner(this);
	}

	public void Lz4Compress()
	{
		TailBuffer[] array = Move();
		using ZeroCopyOutputStream innerStream = new ZeroCopyOutputStream(this);
		using LZ4Stream lZ4Stream = new LZ4Stream(innerStream, LZ4StreamMode.Compress);
		TailBuffer[] array2 = array;
		foreach (TailBuffer tailBuffer in array2)
		{
			lZ4Stream.Write(tailBuffer.Data, 0, tailBuffer.Offset);
			tailBuffer.ToRecycleBin();
		}
	}

	public void Encrypt()
	{
		if (tailBuffers.Count > 1)
		{
			throw new Exception($"[ZeroCopyBuffer] encryption only support single tail. #tail:{tailBuffers.Count}");
		}
		if (tailBuffers.Count != 0)
		{
			int maskIndex = 0;
			Crypto.Encrypt(last.Data, last.Offset, ref maskIndex);
		}
	}

	internal TailBuffer Peek()
	{
		return tailBuffers.Peek();
	}

	internal void PopHeadSegment()
	{
		tailBuffers.Dequeue().ToRecycleBin();
		if (tailBuffers.Count == 0)
		{
			last = null;
		}
	}

	public bool WriteToFile(string filePath, string fileName)
	{
		Log.Info("[WriteToFile] WriteToFile - " + filePath + " : " + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Buffer/ZeroCopyBuffer.cs", 83);
		if (!Directory.Exists(filePath))
		{
			Log.Info("[WriteToFile] CreateDirectory - " + filePath, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Buffer/ZeroCopyBuffer.cs", 86);
			Directory.CreateDirectory(filePath);
		}
		string text = Path.Combine(filePath, fileName);
		try
		{
			using (FileStream fileStream = File.OpenWrite(text))
			{
				Log.Info("[WriteToFile] OpenWrite - " + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Buffer/ZeroCopyBuffer.cs", 96);
				foreach (TailBuffer tailBuffer in tailBuffers)
				{
					fileStream.Write(tailBuffer.Data, 0, tailBuffer.Offset);
				}
				fileStream.Flush();
				Log.Info("[WriteToFile] Flush", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Buffer/ZeroCopyBuffer.cs", 104);
			}
			return true;
		}
		catch (Exception ex)
		{
			Log.Error("[WriteToFile] exception:" + ex.Message + " filePath:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Buffer/ZeroCopyBuffer.cs", 111);
		}
		return false;
	}

	public async Task<bool> WriteToFileAsync(string filePath, string fileName)
	{
		Log.Info("[WriteToFile] WriteToFile - " + filePath + " : " + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Buffer/ZeroCopyBuffer.cs", 119);
		if (!Directory.Exists(filePath))
		{
			Log.Info("[WriteToFile] CreateDirectory - " + filePath, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Buffer/ZeroCopyBuffer.cs", 122);
			Directory.CreateDirectory(filePath);
		}
		string fullPath = Path.Combine(filePath, fileName);
		try
		{
			using (FileStream fileStream = File.OpenWrite(fullPath))
			{
				Log.Info("[WriteToFile] OpenWrite - " + fullPath, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Buffer/ZeroCopyBuffer.cs", 132);
				foreach (TailBuffer tailBuffer in tailBuffers)
				{
					await fileStream.WriteAsync(tailBuffer.Data, 0, tailBuffer.Offset);
				}
				await fileStream.FlushAsync();
				Log.Info("[WriteToFile] Flush", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Buffer/ZeroCopyBuffer.cs", 140);
			}
			return true;
		}
		catch (Exception ex)
		{
			Log.Error("[WriteToFile] exception:" + ex.Message + " filePath:" + fullPath, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Network/Buffer/ZeroCopyBuffer.cs", 147);
		}
		return false;
	}

	internal void Write(byte[] buffer, int offset, int count)
	{
		while (count > 0)
		{
			if (last == null || last.IsFull)
			{
				last = TailBuffer.Create();
				tailBuffers.Enqueue(last);
			}
			int num = last.AddData(buffer, offset, count);
			offset += num;
			count -= num;
		}
	}

	internal TailBuffer[] Move()
	{
		TailBuffer[] result = tailBuffers.ToArray();
		tailBuffers.Clear();
		last = null;
		return result;
	}

	internal TailBuffer[] GetView()
	{
		return tailBuffers.ToArray();
	}

	public string ToBase64()
	{
		if (tailBuffers.Count > 1)
		{
			throw new Exception($"[ZeroCopyBuffer] ToBase64 only support single tail. #tail:{tailBuffers.Count}");
		}
		return Convert.ToBase64String(last.Data, 0, last.Offset);
	}
}
