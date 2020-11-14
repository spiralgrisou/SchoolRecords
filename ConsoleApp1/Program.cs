using System;
using DebuggingLog;
using IOData;

namespace ConsoleApp1
{
    class Program
    {
        static Chunk StudentsStore;
        static Chunk SelectedStudent;
        static Chunk MeetingsStore;

        static void PrintCommands()
        {
            string Commands =
                @"School Systems ©1998
Commands:
help : Shows commands list
clear : Clears console
getstudent <StudentName> : Get student information
addstudent <StudentName> <Age> <Sex> <Password> : Add student to database
selectstudent <StudentName> : Selects student
editselected <NewStudentName> <NewAge> <NewSex> <Password> : Edit currently selected student
getstudents : Shows you all signed in student names
clearselected : Clears selected student
showselected : Shows selected student
deleteselected : Deletes selected student from database
reloaddatabase : Reloads database
showmeetings : Shows meetings requests from students
deletemeeting <MeetingName> : Delete meeting from database
exit : Exits console";
            Debugger.PrintToConsole(Commands, MLevel.Success);
        }

        static void Main(string[] args)
        {
            Debugger.TurnLogs();
            Database.Reload();
            StudentsStore = Database.CreateStore("Students");
            MeetingsStore = Database.CreateStore("Meetings");
            PrintCommands();
            while(true)
            {
                Debugger.PrintToConsole("Enter Command: ", MLevel.Warning);
                string[] cmds = Console.ReadLine().Split(" ");
                try
                {
                    if (cmds[0] == "help")
                        PrintCommands();
                    else if (cmds[0] == "clear")
                        Console.Clear();
                    else if (cmds[0] == "addstudent")
                    {
                        Student student = new Student(cmds[1], cmds[2], cmds[3], cmds[4]);
                        StudentsStore.AddChunk(student);
                    }
                    else if (cmds[0] == "getstudent")
                    {
                        Chunk student = StudentsStore.GetChunk(cmds[1]);
                        if(student != null)
                        {
                            student.GetEntries().ForEach(entry =>
                            {
                                Debugger.PrintToConsole("[" + entry.Name + "]: " + entry.Value, MLevel.Warning);
                            });
                        }
                    }
                    else if (cmds[0] == "reloaddatabase")
                        Database.Reload();
                    else if (cmds[0] == "getstudents")
                    {
                        StudentsStore.GetChunks().ForEach(chunk =>
                        {
                            Debugger.PrintToConsole(chunk.ChunkName, MLevel.Warning);
                        });
                    }
                    else if (cmds[0] == "exit")
                    {
                        Console.Beep();
                        Environment.Exit(0);
                    }
                    else if (cmds[0] == "selectstudent")
                        SelectedStudent = StudentsStore.GetChunk(cmds[1]);
                    else if (cmds[0] == "showselected")
                    {
                        if (SelectedStudent != null)
                        {
                            foreach (Entry entry in SelectedStudent.GetEntries())
                            {
                                Debugger.PrintToConsole("[" + entry.Name + "]: " + entry.Value, MLevel.Warning);
                            }
                        }
                        else
                            Debugger.PrintToConsole("There is no selected student!", MLevel.Error);
                    }
                    else if (cmds[0] == "editselected")
                    {
                        
                        if(SelectedStudent != null)
                        {
                            Chunk newChunk = StudentsStore.EditChunk(SelectedStudent.ChunkName, new Student(cmds[1], cmds[2], cmds[3], cmds[4]));
                            Debugger.PrintToConsole("Successfully updated [Chunk]:[" + SelectedStudent.ChunkName + "]", MLevel.Success);
                            SelectedStudent = newChunk;
                        }
                    }
                    else if (cmds[0] == "deleteselected")
                    {
                        if(SelectedStudent != null)
                        {
                            StudentsStore.DeleteChunk(SelectedStudent.ChunkName);
                            Debugger.PrintToConsole("Successfully removed [Chunk]:[" + SelectedStudent.ChunkName + "]", MLevel.Success);
                            SelectedStudent = null;
                        }
                    }
                    else if (cmds[0] == "clearselected")
                        SelectedStudent = null;
                    else if (cmds[0] == "showmeetings")
                    {
                        MeetingsStore.GetChunks().ForEach(meeting =>
                        {
                            // Read chunks from files (in progress)
                            Debugger.PrintToConsole("[" + meeting.ChunkName + "]:[" + meeting.GetPropertyValue("WantingStudent") + "]", MLevel.Warning);
                        });
                    }
                    else if (cmds[0] == "deletemeeting")
                    {
                        MeetingsStore.DeleteChunk(cmds[1]);
                        Debugger.PrintToConsole("Successfully deleted meeting: " + cmds[1], MLevel.Success);
                    }
                    else
                        Debugger.PrintToConsole("Invalid Command", MLevel.Error);
                }
                catch(IndexOutOfRangeException)
                {
                    Debugger.PrintToConsole("Invalid Arguments", MLevel.Error);
                }
            }
        }
    }
}
