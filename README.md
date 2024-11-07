# dotnet
#### version 2
<hr>
This was written for small applications that needed persistent values to be saved without something like WPF's resources and EXE property storage. In effect this is a string-based storage system. It is very simple to learn to be quaint.
<br>
<br>
It works basically like this.
<br>
<br>
To create a database object and write values to it:
<br>

```cs
int num0 = -1;
DataStore data = new DataStore("database_file");
Block _block = data.NewBlock([ "Key1", "Key2", "Key3" ], "Chunk");
_block.WriteValue("Key1", num0);

data.WriteToFile();
```

To read from a database file:
<br>

```cs
DataStore data = new DataStore("database_file");
Block _block = data.GetBlock("Chunk");
int num0 = int.Parse(_block.GetValue("Key1"));
```