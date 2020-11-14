using System;
using System.IO;
using DebuggingLog;
using System.Threading;
using System.Collections.Generic;

namespace IOData
{
    public class Database
    {
        static List<Chunk> Chunks = new List<Chunk>();
        static List<Entry> Entries = new List<Entry>();

        static Chunk FillInChunk(DirectoryInfo di)
        {
            Chunk chunk = new Chunk();
            chunk.FullPath = di.FullName;
            chunk.ChunkName = di.Name;
            ThreadStart fileRef = new ThreadStart(() =>
            {
                foreach (FileInfo f in di.GetFiles())
                {
                    Debugger.PrintToConsole("Creating entry: " + f.FullName, MLevel.Warning);
                    using (StreamReader sr = new StreamReader(f.FullName))
                    {
                        string value = sr.ReadToEnd();
                        Entry entry = new Entry(f.Name, value);
                        chunk.AddEntry(entry);
                        Entries.Add(entry);
                        sr.Close();
                    }
                }
                Debugger.PrintToConsole("Finished filling in entries", MLevel.Success);
            });
            ThreadStart dirRef = new ThreadStart(() =>
            {
                foreach (DirectoryInfo d in di.GetDirectories())
                {
                    Debugger.PrintToConsole("Creating chunk: " + d.FullName, MLevel.Warning);
                    Chunk dirChunk = FillInChunk(d);
                    dirChunk.FullPath = d.FullName;
                    chunk.AddChunk(dirChunk);
                }
                Debugger.PrintToConsole("Finished filling in chunks", MLevel.Success);
            });
            Thread dirThread = new Thread(dirRef);
            if (di.Parent.FullName != Path.GetFullPath(@".\Database"))
            {
                Thread fileThread = new Thread(fileRef);
                fileThread.Start();
            }
            dirThread.Start();
            return chunk;
        }

        public static void Reload()
        {
            // Database Validation
            if (!Directory.Exists("Database"))
                Directory.CreateDirectory("Database");
            // Filling in the Database
            // Entries
            foreach (FileInfo f in new DirectoryInfo("Database").GetFiles())
                f.Delete();
            // Chunks
            ThreadStart chunksRef = new ThreadStart(() =>
            {
                Debugger.PrintToConsole("Filling in database chunks", MLevel.Warning);
                foreach (DirectoryInfo d in new DirectoryInfo("Database").GetDirectories())
                {
                    Chunk chunk = FillInChunk(d);
                    CreateStore(chunk);
                }
                Debugger.PrintToConsole("Successfully filled in database chunks", MLevel.Success);
            });
            Thread chunksThread = new Thread(chunksRef);
            chunksThread.Start();
            Thread.Sleep(1000);
            Console.Clear();
        }

        public static Chunk CreateStore(string StoreName)
        {
            Chunk storeChunk = new Chunk();
            storeChunk.ChunkName = StoreName;
            storeChunk.FullPath = @".\Database\" + StoreName;
            return CreateStore(storeChunk);
        }

        public static Chunk GetStore(string ChunkName)
        {
            try
            {
                foreach (Chunk chunk in Chunks)
                {
                    if (chunk.ChunkName == ChunkName)
                        return chunk;
                }
            }
            catch (Exception ex)
            {
                Debugger.PrintToConsole(ex.Message, MLevel.Error);
            }
            return null;
        }

        public static Chunk CreateStore(Chunk chunk)
        {
            try
            {
                Chunk chunkFound = GetStore(chunk.ChunkName);
                if (chunkFound == null)
                {
                    Chunks.Add(chunk);
                    Directory.CreateDirectory(@".\Database\" + chunk.ChunkName);
                    return chunk;
                }
                else
                    return chunkFound;
            }
            catch (Exception ex)
            {
                Debugger.PrintToConsole(ex.Message, MLevel.Error);
            }
            return null;
        }

        public static Entry GetEntry(string EntryName)
        {
            foreach (Entry entry in Entries)
            {
                if (entry.Name == EntryName)
                    return entry;
            }
            return null;
        }

        public static Chunk GetChunk(string ChunkName)
        {
            foreach (Chunk chunk in Chunks)
            {
                if (chunk.ChunkName == ChunkName)
                    return chunk;
            }
            return null;
        }
    }
}
