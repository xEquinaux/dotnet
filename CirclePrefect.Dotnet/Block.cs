using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CirclePrefect.Dotnet;

public struct Block : IEnumerable
{
	public int index;
	internal const int Max = 1000;
	public bool active;
	private const string Empty = "0";
	internal const int Zero = 2;
	internal const int NonData = 3;
	private Block Instance => this;
	public DataStore Root { get; internal set; }
	public string Heading { get; internal set; }
	public string[] RawData { get; internal set; }
	internal string[] Data
	{
		get
		{
			if (Root != null)
			{
				Root.block[index] = this;
			}
			return RawData;
		}
	}
	public string[] Contents => Trim(RawData);
	public bool Dynamic { get; internal set; }
	public int Length
	{
		get
		{
			int num = ((RawData != null) ? RawData.Length : 0);
			return Math.Max(num - 3, 0);
		}
	}

	public IEnumerator GetEnumerator()
	{
		return Data.GetEnumerator();
	}

	public void Add(string item)
	{
		throw new NotImplementedException();
	}

	internal void UpdateRoot(DataStore data)
	{
		data.block[index] = this;
	}

	public static void EmptyBlock(Block block)
	{
		block.RawData = new string[0];
	}

	internal string[] Trim(string[] array)
	{
		if (array == null)
		{
			return new string[0];
		}
		string[] array2 = new string[array.Length - 3];
		if (array.Length > 3)
		{
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array[i + 2];
			}
		}
		return array2;
	}

	public string Key(string item)
	{
		if (item.Contains(":"))
		{
			return item.Substring(0, item.IndexOf(":"));
		}
		return string.Empty;
	}

	public string Key(int index)
	{
		string[] array = Trim(RawData);
		if (array != null && array.Length != 0 && index < array.Length)
		{
			return array[index].Substring(0, array[index].IndexOf(":"));
		}
		return string.Empty;
	}

	private string Value(string item)
	{
		if (item.Contains(":"))
		{
			return item.Substring(item.IndexOf(":") + 1);
		}
		return string.Empty;
	}

	private string Value(int index)
	{
		string[] array = Trim(RawData);
		if (array != null && array.Length != 0 && index < array.Length)
		{
			return array[index].Substring(array[index].IndexOf(":"));
		}
		return string.Empty;
	}

	public string[] PairIndexAt(int index)
	{
		string[] result = new string[1] { string.Empty };
		index = Math.Max(0, Math.Min(Length - 1, index));
		if (RawData.Length >= index + 2)
		{
			string[] array = RawData[index + 2].Split(new char[1] { ':' });
			result = new string[2]
			{
				array[0],
				array[1]
			};
		}
		return result;
	}

	public string[] Keys()
	{
		string[] array = Trim(RawData);
		if (array.Length == 0)
		{
			throw new Exception("Block array has no key value pairs.");
		}
		string[] array2 = new string[RawData.Length];
		for (int i = 0; i < RawData.Length; i++)
		{
			if (RawData[i] != null && RawData[i].Contains(":") && array[i] != null && array[i].Length > 0)
			{
				array2[i] = RawData[i].Substring(0, RawData[i].IndexOf(":"));
			}
		}
		return array2;
	}

	public string[] KeysV2()
	{
		string text = string.Empty;
		for (int i = 0; i < RawData.Length; i++)
		{
			if (RawData[i] != null && !string.IsNullOrWhiteSpace(RawData[i]) && !string.IsNullOrEmpty(RawData[i]) && RawData[i].Contains(":"))
			{
				text = text + RawData[i].Substring(0, RawData[i].IndexOf(":")) + ";";
			}
		}
		return text.TrimEnd(new char[1] { ';' }).Split(new char[1] { ';' });
	}

	public string[] Values()
	{
		string[] array = Trim(RawData);
		string[] array2 = new string[array.Length];
		if (array.Length == 0)
		{
			throw new Exception("Block array has no key value pairs.");
		}
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = RawData[i].Substring(RawData[i].IndexOf(":") + 1);
		}
		return array2;
	}

	public bool HasValue(string value)
	{
		for (int i = 0; i < RawData.Length; i++)
		{
			if (!string.IsNullOrEmpty(RawData[i]) && RawData[i].Split(new char[1] { ':' })[1] == value)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasValue(string value, out string key)
	{
		key = "";
		for (int i = 0; i < RawData.Length; i++)
		{
			if (!string.IsNullOrEmpty(RawData[i]) && RawData[i].Split(new char[1] { ':' })[1] == value)
			{
				key = RawData[i].Split(new char[1] { ':' })[0];
				return true;
			}
		}
		return false;
	}

	public bool HasKey(string key)
	{
		for (int i = 0; i < RawData.Length; i++)
		{
			if (!string.IsNullOrEmpty(RawData[i]) && RawData[i].Split(new char[1] { ':' })[0] == key)
			{
				return true;
			}
		}
		return false;
	}

	public string GetValue(string key)
	{
		for (int i = 0; i < RawData.Length; i++)
		{
			if (!string.IsNullOrEmpty(RawData[i]) && RawData[i].StartsWith(key))
			{
				string[] array = RawData[i].Split(new char[1] { ':' });
				if (array.Length == 2)
				{
					return array[1];
				}
			}
		}
		return "0";
	}

	public void GetValue(string key, out string value)
	{
		for (int i = 0; i < RawData.Length; i++)
		{
			if (!string.IsNullOrEmpty(RawData[i]) && RawData[i].StartsWith(key))
			{
				string[] array = RawData[i].Split(new char[1] { ':' });
				if (array.Length == 2)
				{
					value = array[1];
					return;
				}
			}
		}
		value = "0";
	}

	private void ShiftDown(ref string[] input, int amount)
	{
		string[] array = new string[input.Length + amount];
		input.CopyTo(array, 0);
		input = array;
	}

	public void RemovePair(string key, string value)
	{
		bool flag = false;
		int num = 0;
		for (int i = 0; i < RawData.Length; i++)
		{
			string text = RawData[i];
			if (!string.IsNullOrEmpty(text) && text == key + ":" + value)
			{
				num = i;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			throw new Exception("Key not found in the block.");
		}
		RawData[num] = string.Empty;
	}

	public void RemoveItem(string key)
	{
		bool flag = false;
		int num = 0;
		for (int i = 0; i < RawData.Length; i++)
		{
			string text = RawData[i];
			if (!string.IsNullOrEmpty(text) && text.Contains(":") && text.Substring(0, text.IndexOf(":")) == key)
			{
				num = i;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			throw new Exception("Key not found in the block.");
		}
		RawData[num] = string.Empty;
	}

	public void RemoveItemAt(int index)
	{
		index += 2;
		if (index < 2 || index > 1000)
		{
			throw new IndexOutOfRangeException("index");
		}
		if (!string.IsNullOrEmpty(RawData[1000]))
		{
			throw new Exception("Block has reached max entries and declined item adding.");
		}
		if (!string.IsNullOrEmpty(RawData[index]) && RawData[index].Contains(":"))
		{
			RawData[index] = string.Empty;
		}
		for (int i = index; i < 1000; i++)
		{
			string text = "";
			if (!string.IsNullOrEmpty(RawData[i + 1]))
			{
				text = (string)RawData[i + 1].Clone();
			}
			RawData[i] = text;
			if (RawData[i - 2] == "]")
			{
				break;
			}
		}
	}

	public void Clear()
	{
		RawData = new string[3] { Heading, "[", "]" };
	}

	public void InsertItemAt(int index, string key, string value, bool ignore = false)
	{
		index += 2;
		if (index < 2 || index > 1000)
		{
			throw new IndexOutOfRangeException("index");
		}
		if (!string.IsNullOrEmpty(RawData[1000]))
		{
			throw new Exception("Block has reached max entries and declined item adding.");
		}
		if (!ignore)
		{
			string[] array = Keys();
			foreach (string text in array)
			{
				if (text == key)
				{
					throw new Exception("Key already found in the block.");
				}
			}
		}
		bool flag = false;
		int num = 0;
		for (int num2 = 1000; num2 > index; num2--)
		{
			if (!string.IsNullOrEmpty(RawData[num2]))
			{
				string text2 = (string)RawData[num2 - 1].Clone();
				RawData[num2] = text2;
				if (!flag)
				{
					num = num2 + 1;
					flag = true;
				}
			}
		}
		RawData[num] = "]";
		RawData[index] = key + ":" + value;
	}

	public void AddItem(string key, string value, bool ignore = false)
	{
		if (!ignore)
		{
			string[] array = Keys();
			foreach (string text in array)
			{
				if (text == key)
				{
					throw new Exception("Key already found in the block.");
				}
			}
		}
		for (int num = 1000; num >= 2; num--)
		{
			if (num == 1)
			{
				throw new Exception("Block " + Heading + " has reached max entries and declined item adding.");
			}
			if (RawData[num] == "]")
			{
				RawData[num] = key + ":" + value;
				RawData[num + 1] = "]";
				break;
			}
		}
	}

	[Obsolete("Use AddItem(...) instead.")]
	public void AddItemV2(string key, string value)
	{
		if (Contents.Length != 0)
		{
			string[] array = Keys();
			foreach (string text in array)
			{
				if (text == key)
				{
					throw new Exception("Key already found in the block.");
				}
			}
		}
		string[] array2 = new string[RawData.Length + 1];
		RawData.CopyTo(array2, 0);
		int num = 0;
		for (int j = 0; j < array2.Length; j++)
		{
			if (array2[j] == "]")
			{
				num = j;
				break;
			}
		}
		array2[num] = key + ":" + value;
		array2[num + 1] = "]";
		RawData = array2;
	}

	public void WriteValue(string key, object value)
	{
		int num = 0;
		for (int i = 0; i < RawData.Length; i++)
		{
			if (!string.IsNullOrWhiteSpace(RawData[i]))
			{
				if (Key(RawData[i]) == key)
				{
					num = i;
					break;
				}
				if (i == RawData.Length - 1)
				{
					return;
				}
			}
		}
		RawData[num] = key + ":" + value.ToString();
	}

	public void WriteValue(string key, string value, bool flag)
	{
		int num = 0;
		for (int i = 0; i < RawData.Length; i++)
		{
			if (!string.IsNullOrWhiteSpace(RawData[i]))
			{
				if (Key(RawData[i]) == key)
				{
					num = i;
					break;
				}
				if (i == RawData.Length - 1)
				{
					return;
				}
			}
		}
		RawData[num] = key + ":" + value + ":" + flag;
	}

	public void AddValue(string key, char separator, string value)
	{
		int num = 0;
		for (int i = 0; i < RawData.Length; i++)
		{
			if (!string.IsNullOrWhiteSpace(RawData[i]))
			{
				if (Key(RawData[i]) == key)
				{
					num = i;
					break;
				}
				if (i == RawData.Length - 1)
				{
					return;
				}
			}
		}
		if (!RawData[num].StartsWith(key))
		{
			RawData[num] = key + ":" + value + separator;
		}
		else
		{
			RawData[num] += value + separator;
		}
	}

	public int IncreaseValue(string key, int amount)
	{
		int num = 0;
		for (int i = 0; i < RawData.Length; i++)
		{
			if (!string.IsNullOrEmpty(RawData[i]) && Key(RawData[i]) == key)
			{
				num = i;
				break;
			}
			if (i == RawData.Length - 1)
			{
				return amount;
			}
		}
		int result = 0;
		string value = GetValue(RawData[num]);
		if (int.TryParse(value, out result))
		{
			result += amount;
			RawData[num] = key + ":" + result;
		}
		return result + amount;
	}

	public void HasFlag(string key, out bool flag)
	{
		int num = RawData.Length - 1;
		for (int i = 0; i < RawData.Length; i++)
		{
			if (RawData == null)
			{
				flag = false;
				return;
			}
			if (RawData[i] != null && !string.IsNullOrWhiteSpace(RawData[i]) && !string.IsNullOrEmpty(RawData[i]) && RawData[i].StartsWith(key))
			{
				num = i;
				break;
			}
		}
		bool.TryParse(RawData[num].Substring(RawData[num].LastIndexOf(":") + 1), out flag);
	}

	public Dictionary<string, string> KeyValue()
	{
		string[] array = Trim(RawData);
		if (array.Length == 0)
		{
			throw new Exception("Block array has no key value pairs.");
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < array.Length; i++)
		{
			dictionary.Add(Key(array[i]), GetValue(array[i]));
		}
		return dictionary;
	}

	public override bool Equals(object obj)
	{
		return Instance == (Block)obj;
	}

	public void FromDictionary(Dictionary<string, string> pairs)
	{
		if (pairs.Count == 0)
		{
			throw new Exception("Dictionary has no key pair values to read.");
		}
		for (int i = 0; i < pairs.Count; i++)
		{
			string text = pairs.Keys.ElementAt(i);
			if (!Keys().Contains(text))
			{
				AddItem(text, pairs[text]);
			}
		}
	}

	public override int GetHashCode()
	{
		throw new NotImplementedException();
	}

	public static bool operator !=(Block a, Block b)
	{
		return a.RawData == b.RawData;
	}

	public static bool operator ==(Block a, Block b)
	{
		return a.RawData == b.RawData;
	}
}
