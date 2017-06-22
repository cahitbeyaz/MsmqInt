using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsmqInt.Test.Mq
{
    public class Student
    {
        public int age { get; set; }
        public string name { get; set; }

        public Student()
        {
        }

        public Student(int age,string name)
        {
            this.age = age;
            this.name = name;
        }
    }
}
