using System;
using DebuggingLog;
using IOData;

namespace ClientStudent
{
    class Program
    {
        static Chunk StudentsStore;
        static Chunk MeetingsStore;
        static Chunk CurrentStudent;

        static void PrintCommands()
        {
            string Commands = @"School Systems 1998
Commands:
help : Shows commands
exit : Exits application
clear : Clears console
login : Starts a login attempt
logout : Log out of current account
showdata : Shows student data
changepassword : Changes old password
meetteacher : Books a meeting with a teacher
reloaddatabase : Gets new updates from the database // shouldn't be a thing because a database should be on the server";
            Debugger.PrintToConsole(Commands, MLevel.Success);
        }

        static void Main(string[] args)
        {
            Debugger.TurnLogs(); // Turning on the functionalty to log everything that has happened to file
            Database.Reload(); // Reloading a database is basically initializing it
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
                    else if (cmds[0] == "exit")
                    {
                        Console.Beep();
                        Environment.Exit(0);
                    }
                    else if (cmds[0] == "clear")
                        Console.Clear();
                    else if (cmds[0] == "login")
                    {
                        Console.Clear();
                        if (CurrentStudent == null)
                        {
                            Debugger.PrintToConsole("Username:", MLevel.Warning);
                            string Username = Console.ReadLine();
                            Debugger.PrintToConsole("Password:", MLevel.Warning);
                            string Password = Console.ReadLine();
                            Chunk studentFound = StudentsStore.GetChunk(Username);
                            if (studentFound != null)
                            {
                                string sPassword = (string)studentFound.GetPropertyValue("Password");
                                if (Password == sPassword)
                                {
                                    CurrentStudent = studentFound;
                                    Debugger.PrintToConsole("Login successful: Welcome! " + studentFound.GetPropertyValue("Name"), MLevel.Success);
                                }
                                else
                                    Debugger.PrintToConsole("Invalid credentials", MLevel.Error);
                            }
                            else
                                Debugger.PrintToConsole("Invalid credentials", MLevel.Error);
                        }
                        else
                            Debugger.PrintToConsole("Already logged in", MLevel.Warning);
                    }
                    else if (cmds[0] == "logout")
                    {
                        if (CurrentStudent != null)
                        {
                            CurrentStudent = null;
                            Debugger.PrintToConsole("Successfully logged out of user", MLevel.Success);
                        }
                        else
                            Debugger.PrintToConsole("You are not logged in", MLevel.Error);
                    }
                    else if (cmds[0] == "showdata")
                    {
                        if (CurrentStudent != null)
                        {
                            CurrentStudent.GetEntries().ForEach(entry =>
                            {
                                Debugger.PrintToConsole("[" + entry.Name + "]:[" + entry.Value + "]", MLevel.Warning);
                            });
                        }
                        else
                            Debugger.PrintToConsole("You are not logged in", MLevel.Error);
                    }
                    else if (cmds[0] == "changepassword")
                    {
                        Console.Clear();
                        if (CurrentStudent != null)
                        {
                            Debugger.PrintToConsole("Current Password: ", MLevel.Warning);
                            string currentPassword = Console.ReadLine();
                            string sPassword = (string)CurrentStudent.GetPropertyValue("Password");
                            if (currentPassword == sPassword)
                            {
                                Debugger.PrintToConsole("New Password: ", MLevel.Warning);
                                string newPassword = Console.ReadLine();
                                Student newStudent = new Student((string)CurrentStudent.GetPropertyValue("Name"), (string)CurrentStudent.GetPropertyValue("Age"),
                                    (string)CurrentStudent.GetPropertyValue("Sex"), newPassword);
                                CurrentStudent.EditChunk(CurrentStudent.ChunkName, newStudent);
                                Debugger.PrintToConsole("Successfully changed password", MLevel.Success);
                            }
                            else
                                Debugger.PrintToConsole("Incorrect Password", MLevel.Error);
                        }
                        else
                            Debugger.PrintToConsole("You are not logged in", MLevel.Error);
                    }
                    else if (cmds[0] == "meetteacher")
                    {
                        if (CurrentStudent != null)
                        {
                            Meeting meeting = new Meeting(CurrentStudent);
                            MeetingsStore.AddChunk(meeting);
                            Debugger.PrintToConsole("Meeting request is ready for a response!", MLevel.Success);
                        }
                        else
                            Debugger.PrintToConsole("You are not logged in", MLevel.Error);
                    }
                    else if (cmds[0] == "reloaddatabase")
                        Database.Reload();
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
