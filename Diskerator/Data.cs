using DebuggingLog;
using System.Collections.Generic;
using System.IO;

namespace IOData
{
    public class Entry
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public object Value { get; set; }
        public Entry(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }

    public class Chunk
    {
        public string ChunkName { get; set; }
        public string FullPath { get; set; }
        List<Chunk> Chunks = new List<Chunk>();
        List<Entry> Entries = new List<Entry>();
        public virtual List<string> InitialProperties { get; set; }

        public Entry GetEntry(string EntryName)
        {
            foreach(Entry entry in Entries)
            {
                if (entry.Name == EntryName)
                    return entry;
            }
            return null;
        }

        public Entry AddEntry(Entry entry)
        {
            Entry entryInData = GetEntry(entry.Name);
            if (entryInData == null)
            {
                entry.FullPath = FullPath + @"\" + entry.Name;
                Entries.Add(entry);
                if(!File.Exists(entry.FullPath))
                {
                    using (StreamWriter sw = new StreamWriter(entry.FullPath))
                    {
                        string valueToWrite = string.Empty;
                        if (entry.Value.GetType() == typeof(Chunk))
                        {
                            Chunk valueChunk = (Chunk)entry.Value;
                            valueToWrite = (string)valueChunk.GetPropertyValue("Name");
                        }
                        else
                            valueToWrite = (string)entry.Value;
                        sw.Write(valueToWrite);
                        sw.Close();
                    }
                }
                Debugger.PrintToConsole("Successfully created [Entry]:[" + entry.Name + "]", MLevel.Success);
            }
            return entryInData;
        }

        public object GetPropertyValue(string PropertyName)
        {
            Entry entry = GetEntry(PropertyName);
            if (entry == null)
            {
                Debugger.PrintToConsole("Property not found: " + PropertyName, MLevel.Error);
                return null;
            }
            return entry.Value;
        }

        public Chunk EditChunk(string ChunkName, Chunk newChunk)
        {
            Chunk chunk = GetChunk(ChunkName);
            if (chunk != null)
            {
                // Removing old chunk
                GetChunks().Remove(chunk);
                Directory.Delete(chunk.FullPath, true);

                // Adding new chunk
                AddChunk(newChunk);
                string path = new DirectoryInfo(chunk.FullPath).Parent.FullName + @"\" + newChunk.ChunkName;
                newChunk.FullPath = path;
                if (!Directory.Exists(newChunk.FullPath))
                {
                    Directory.CreateDirectory(newChunk.FullPath);
                    Debugger.PrintToConsole("Successfully created [Chunk]:[" + newChunk.ChunkName + "]", MLevel.Success);
                    if (newChunk.InitialProperties.Count > 0)
                    {
                        foreach (string Prop in newChunk.InitialProperties)
                        {
                            object PropValue = newChunk.GetType().GetProperty(Prop).GetValue(newChunk, null);
                            Entry entry = new Entry(Prop, PropValue);
                            newChunk.AddEntry(entry);
                        }
                    }
                }
                return newChunk;
            }
            else
                Debugger.PrintToConsole("Cannot create [Chunk]:[" + newChunk.ChunkName + "] there's already a chunk with that name", MLevel.Warning);
            return null;
        }

        public void DeleteChunk(string ChunkName)
        {
            Chunk chunk = GetChunk(ChunkName);
            if(chunk != null)
            {
                GetChunks().Remove(chunk);
                Directory.Delete(chunk.FullPath, true);
            }
        }

        public void AddEntries(List<Entry> entries)
        {
            entries.ForEach(entry =>
            {
                entry.FullPath = FullPath + @"\" + entry.Name;
                Entries.Add(entry);
                if(!File.Exists(entry.FullPath))
                {
                    using (StreamWriter sw = new StreamWriter(entry.FullPath))
                    {
                        string valueToWrite = string.Empty;
                        if (entry.Value.GetType() == typeof(Chunk))
                        {
                            Chunk valueChunk = (Chunk)entry.Value;
                            valueToWrite = (string)valueChunk.GetPropertyValue("Name");
                        }
                        else
                            valueToWrite = (string)entry.Value;
                        sw.Write(valueToWrite);
                        sw.Close();
                    }
                }
                Debugger.PrintToConsole("Successfully created [Entry]:[" + entry.Name + "]", MLevel.Success);
            });
        }

        public Chunk GetChunk(string ChunkName)
        {
            foreach (Chunk chunk in Chunks)
            {
                if (chunk.ChunkName == ChunkName)
                    return chunk;
            }
            return null;
        }

        public List<Chunk> GetChunks()
        {
            return Chunks;
        }

        public List<Entry> GetEntries()
        {
            return Entries;
        }

        public void AddChunk(Chunk chunk)
        {
            if(!Chunks.Contains(chunk))
            {
                Chunks.Add(chunk);
                chunk.FullPath = FullPath + @"\" + chunk.ChunkName;
                if (!Directory.Exists(chunk.FullPath))
                {
                    Directory.CreateDirectory(chunk.FullPath);
                    Debugger.PrintToConsole("Successfully created [Chunk]:[" + chunk.ChunkName + "]", MLevel.Success);
                    if(chunk.InitialProperties.Count > 0)
                    {
                        foreach (string Prop in chunk.InitialProperties)
                        {
                            /*string PropValue = (string)chunk.GetType().GetProperty(Prop).GetValue(chunk, null);*/
                            object PropValue = chunk.GetType().GetProperty(Prop).GetValue(chunk, null);
                            Entry entry = new Entry(Prop, PropValue);
                            chunk.AddEntry(entry);
                        }
                    }
                }
            }
            else
            {
                Debugger.PrintToConsole("Cannot create [Chunk]:[" + chunk.ChunkName + "] there's already a chunk with that name", MLevel.Warning);
            }
        }
    }
}
