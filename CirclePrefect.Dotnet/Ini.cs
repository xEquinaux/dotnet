using System;
using System.IO;

namespace CirclePrefect.Dotnet;

public class Ini
{
	public string path;

	public const string ext = ".ini";

	public string[] setting;

	public static bool IniExists(string path)
	{
		return File.Exists(path);
	}

	public void AddSetting(string key, string value)
	{
		if (key == null || value == null)
		{
			return;
		}
		if (!File.Exists(path))
		{
			MakeFile();
		}
		int num = 1;
		string[] array = new string[num];
		if (setting != null)
		{
			array = new string[setting.Length];
		}
		string[] array2;
		array = new string[(array2 = ReadFile()).Length + 1];
		int num2 = 0;
		foreach (string text in array2)
		{
			if (text != null && !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
			{
				array[num2++] = text;
			}
		}
		array[num2] = key + "=" + value;
		setting = array;
		WriteValues();
	}

	public void ModifySetting(string key, string value)
	{
		setting = ReadFile();
		if (key == null || value == null || setting == null || setting.Length == 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < setting.Length; i++)
		{
			if (setting[i].StartsWith(key))
			{
				num = i;
				break;
			}
		}
		if (!File.Exists(path))
		{
			MakeFile();
		}
		if (!setting[num].Contains("=") && setting[num].Substring(key.Length, 1) != "=")
		{
			setting[num] = setting[num].Insert(key.Length, "=");
		}
		setting[num] = setting[num].Substring(0, key.Length + 1);
		setting[num] += value;
		WriteValues();
	}

	private void MakeFile()
	{
		FileStream fileStream = File.Create(path);
		fileStream.Close();
		fileStream.Dispose();
	}

	[Obsolete("Do not include this with the initializer for Ini objects anymore as it resorts to removing all data in the config file.")]
	public void WriteFile(object[] text)
	{
		if (text == null)
		{
			return;
		}
		for (int i = 0; i < setting.Length; i++)
		{
			setting[i] += "=";
		}
		if (!File.Exists(path))
		{
			MakeFile();
		}
		using StreamWriter streamWriter = new StreamWriter(path);
		streamWriter.NewLine = "\n";
		for (int j = 0; j < setting.Length; j++)
		{
			if (j < text.Length && j < setting.Length)
			{
				streamWriter.Write(setting[j]);
				streamWriter.WriteLine(text[j]);
			}
		}
	}

	private void WriteValues()
	{
		if (!File.Exists(path))
		{
			MakeFile();
		}
		using StreamWriter streamWriter = new StreamWriter(path);
		streamWriter.NewLine = "\n";
		for (int i = 0; i < setting.Length; i++)
		{
			streamWriter.WriteLine(setting[i]);
		}
	}

	public string[] ReadFile()
	{
		if (!File.Exists(path))
		{
			MakeFile();
		}
		string[] result = null;
		using (StreamReader streamReader = new StreamReader(path))
		{
			result = streamReader.ReadToEnd().Split(new char[1] { '\n' });
		}
		return result;
	}

	public static bool TryParse(string setting, out string output)
	{
		if (setting.Contains("="))
		{
			output = setting.Substring(setting.IndexOf('=') + 1);
			return true;
		}
		output = string.Empty;
		return false;
	}
}
