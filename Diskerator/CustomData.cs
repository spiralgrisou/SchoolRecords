using System;
using System.Collections.Generic;
using System.Text;

namespace IOData
{
    public class Student : Chunk
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string Password { get; set; }

        public Student(string Name, string Age, string Sex, string Password)
        {
            ChunkName = Name;
            this.Name = Name;
            this.Age = Age;
            this.Sex = Sex;
            this.Password = Password;
        }

        public override List<string> InitialProperties
        {
            get
            {
                List<string> Props = new List<string>();
                Props.Add("Name");
                Props.Add("Age");
                Props.Add("Sex");
                Props.Add("Password");
                return Props;
            }
            set
            {
                InitialProperties = value;
            }
        }
    }

    public class Meeting : Chunk
    {
        public Chunk WantingStudent { get; set; }
        
        public Meeting(Chunk WantingStudent)
        {
            ChunkName = WantingStudent.ChunkName + "Meeting";
            this.WantingStudent = WantingStudent;
        }

        public override List<string> InitialProperties
        { 
            get
            {
                List<string> Props = new List<string>();
                Props.Add("WantingStudent");
                return Props;
            }
            set
            {
                InitialProperties = value;
            }
        }
    }
}
