using System;

namespace OOP_KupetsOleksii.lab2v
{
    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Student(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return $"{Name}, Age: {Age}";
        }
    }

    public class StudentGroup
    {
        private Student[] students;
        private int count;

        public StudentGroup(int capacity)
        {
            students = new Student[capacity];
            count = 0;
        }

        public Student this[int index]
        {
            get
            {
                if (index >= 0 && index < count)
                {
                    return students[index];
                }
                else
                {
                    throw new IndexOutOfRangeException("Student index is out of range.");
                }
            }
        }

        public static StudentGroup operator +(StudentGroup group, Student student)
        {
            if (group.count < group.students.Length)
            {
                group.students[group.count++] = student;
            }
            else
            {
                Console.WriteLine("Group is full. Cannot add more students.");
            }
            return group;
        }

        public static StudentGroup operator -(StudentGroup group, Student student)
        {
            int index = Array.IndexOf(group.students, student);
            if (index >= 0 && index < group.count)
            {
                for (int i = index; i < group.count - 1; i++)
                {
                    group.students[i] = group.students[i + 1];
                }
                group.students[--group.count] = null;
            }
            else
            {
                Console.WriteLine("Student not found in the group.");
            }
            return group;
        }

        public void DisplayGroup()
        {
            Console.WriteLine("Student Group:");
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i]}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StudentGroup group = new StudentGroup(5);

            Student student1 = new Student("Oleksii Kupets", 20);
            Student student2 = new Student("Ivan Ivanov", 22);
            Student student3 = new Student("Oksana Petrova", 21);

            group = group + student1;
            group = group + student2;
            group = group + student3;

            group.DisplayGroup();

            group = group - student2;

            Console.WriteLine("\nAfter removing a student:");
            group.DisplayGroup();

            Console.WriteLine($"\nStudent at index 0: {group[0]}");
        }
    }
}
