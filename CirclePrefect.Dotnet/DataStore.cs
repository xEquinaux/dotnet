using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace CirclePrefect.Dotnet;

public class DataStore
{
	public const string ext = ".dat";
	private IList<string> array = new List<string>();
	private string fileName;
	private string fileNoExt;
	public IList<Block> block = new List<Block>();
	public IList<string> RawDB => array;

	public DataStore()
	{
	}

	public DataStore(string fileNoExt)
	{
		this.fileNoExt = fileNoExt;
		fileName = fileNoExt + ".dat";
		Initialize();
	}

	private void Log(object text)
	{
		Console.WriteLine(text);
	}

	private void Log(string[] array)
	{
		foreach (string text in array)
		{
			if (text != null && NotEmpty(text))
			{
				Console.WriteLine(text);
			}
		}
	}

	private bool NotEmpty(string text)
	{
		return text != string.Empty && text != "\n" && text != "\r" && text != "\n\r" && text != "\r\n" && !string.IsNullOrWhiteSpace(text);
	}

	public void WriteToFile()
	{
		using StreamWriter streamWriter = new StreamWriter(fileName);
		streamWriter.NewLine = "\n";
		for (int i = 0; i < block.Count; i++)
		{
			if (block[i].Data == null)
			{
				continue;
			}
			int num = 0;
			IList<string> array = this.array;
			foreach (string text in array)
			{
				if (text == block[i].Heading)
				{
					num++;
				}
			}
			if (num >= 2 && BlockExists(block[i].Heading))
			{
				continue;
			}
			for (int k = 0; k < block[i].Data.Length; k++)
			{
				if (NotEmpty(block[i].Data[k]))
				{
					streamWriter.WriteLine(block[i].Data[k]);
				}
			}
		}
	}

	private void WriteToStream(Stream stream)
	{
		using StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.NewLine = "\n";
		for (int i = 0; i < block.Count; i++)
		{
			if (block[i].Data == null)
			{
				continue;
			}
			int num = 0;
			IList<string> array = this.array;
			foreach (string text in array)
			{
				if (text == block[i].Heading)
				{
					num++;
				}
			}
			if (num >= 2 && BlockExists(block[i].Heading))
			{
				continue;
			}
			for (int k = 0; k < block[i].Data.Length; k++)
			{
				if (NotEmpty(block[i].Data[k]))
				{
					streamWriter.WriteLine(block[i].Data[k]);
				}
			}
		}
	}

	public void WriteToFile(string fileName)
	{
		string text = "";
		using FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
		for (int i = 0; i < block.Count; i++)
		{
			if (block[i].Data == null)
			{
				continue;
			}
			int num = 0;
			IList<string> array = this.array;
			foreach (string text2 in array)
			{
				if (text2 == block[i].Heading)
				{
					num++;
				}
			}
			if (num >= 2 && BlockExists(block[i].Heading))
			{
				continue;
			}
			for (int k = 0; k < block[i].Data.Length; k++)
			{
				if (NotEmpty(block[i].Data[k]))
				{
					text = text + block[i].Data[k] + "\n";
				}
			}
		}
		byte[] bytes = Encoding.ASCII.GetBytes(text.TrimEnd(new char[1] { '\n' }));
		fileStream.Write(bytes, 0, bytes.Length);
	}

	public void Dispose(bool clearAll, Block[] keep = null)
	{
		for (int i = 0; i < block.Count; i++)
		{
			if (keep != null)
			{
				for (int j = 0; j < keep.Length && !(block[i].Heading == keep[j].Heading); j++)
				{
					if (j == keep.Length - 1)
					{
						//_ = ref block[i];
						if (true)
						{
							Block.EmptyBlock(block[i]);
						}
					}
				}
			}
			else
			{
				Block.EmptyBlock(block[i]);
			}
		}
		if (clearAll)
		{
			for (int k = 0; k < array.Count; k++)
			{
				array[k] = null;
			}
		}
	}

	public void Initialize()
	{
		if (fileName.Contains("\\") && !Directory.Exists(fileName.Remove(fileName.LastIndexOf("\\"))))
		{
			Directory.CreateDirectory(fileName.Remove(fileName.LastIndexOf("\\")));
		}
		if (!File.Exists(fileName))
		{
			FileStream fileStream = File.Create(fileName);
			fileStream.Close();
			fileStream.Dispose();
			return;
		}
		using (StreamReader streamReader = new StreamReader(fileName))
		{
			array = streamReader.ReadToEnd().Split(new char[1] { '\n' });
		}
		Rewrite();
	}

	private void Rewrite()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 1; i < array.Count; i++)
		{
			if (!string.IsNullOrWhiteSpace(array[i]))
			{
				int num3 = 0;
				int num4 = 0;
				if (array[i].StartsWith("["))
				{
					block.Add(new Block
					{
						Heading = array[i - 1],
						RawData = new string[1001],
						active = true
					});
					block[num].RawData[0] = array[i - 1];
					block[num].RawData[1] = "[";
				}
				else if (array[i].StartsWith("]"))
				{
					block[num].RawData[num2 + 2] = "]";
					num++;
					num2 = 0;
					continue;
				}
				if (array[i].Contains(":"))
				{
					block[num].RawData[num2 + 2] = array[i];
				}
				num2++;
			}
		}
	}

	public int GetIndex(Block b)
	{
		return block.ToList().IndexOf(b);
	}

	private int NumBlocks()
	{
		int num = 0;
		for (int i = 0; i < block.Count; i++)
		{
			//_ = ref block[i];
			if (block[i].Data != null)
			{
				num++;
			}
		}
		return num;
	}

	public Block NewBlock(string[] array, string heading)
	{
		IList<Block> array2 = block;
		for (int i = 0; i < array2.Count; i++)
		{
			Block result = array2[i];
			if (result.Heading == heading)
			{
				return result;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			if (!array[j].Contains(":"))
			{
				array[j] += ":0";
			}
		}
		int num = NumBlocks();
		ShiftDown(ref array, 2);
		array[0] = heading;
		array[1] = "[";
		array[array.Length - 1] = "]";
		block.Add(new Block
		{
			Heading = heading,
			RawData = new string[1001],
			index = num,
			Root = this,
			active = true
		});
		for (int k = 0; k < array.Length; k++)
		{
			if (!string.IsNullOrEmpty(array[k]))
			{
				block[num].RawData[k] = array[k];
			}
		}
		return block[num];
	}

	public Block NewBlock(string[] array, object[] values, string heading)
	{
		IList<Block> array2 = block;
		for (int i = 0; i < array2.Count; i++)
		{
			Block result = array2[i];
			if (result.Heading == heading)
			{
				return result;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			if (!array[j].Contains(":"))
			{
				ref string reference = ref array[j];
				reference = reference + ":" + values[j].ToString();
			}
		}
		int num = NumBlocks();
		ShiftDown(ref array, 2);
		array[0] = heading;
		array[1] = "[";
		array[array.Length - 1] = "]";
		block.Add(new Block
		{
			Heading = heading,
			RawData = new string[1001],
			index = num,
			Root = this,
			active = true
		});
		for (int k = 0; k < array.Length; k++)
		{
			if (!string.IsNullOrEmpty(array[k]))
			{
				block[num].RawData[k] = array[k];
			}
		}
		return block[num];
	}

	private void ShiftDown(ref string[] input, int amount)
	{
		string[] array = new string[input.Length + amount + 1];
		input.CopyTo(array, amount);
		input = array;
	}

	private bool IsClose(string item)
	{
		return item.StartsWith("]");
	}

	public Block GetBlock(string heading)
	{
		IList<Block> array = block;
		for (int i = 0; i < array.Count; i++)
		{
			Block result = array[i];
			if (result.Data != null && result.Heading == heading)
			{
				return result;
			}
		}
		return block[0];
	}

	public bool BlockExists(string heading)
	{
		IList<Block> array = this.block;
		foreach (Block block in array)
		{
			if (block.Heading == heading)
			{
				return true;
			}
		}
		return false;
	}

	public bool BlockExists(string heading, out Block item)
	{
		IList<Block> array = this.block;
		for (int i = 0; i < array.Count; i++)
		{
			Block block = array[i];
			if (block.Heading == heading)
			{
				item = block;
				return true;
			}
		}
		item = default(Block);
		return false;
	}
}
