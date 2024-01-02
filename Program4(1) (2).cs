using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static List<string> studentNames = new List<string>();
    static List<string> subjectNames = new List<string>();

    static object lockObject = new object();

    static void Main()
    {
        // Створюємо два окремих потоки для генерації імен студентів та предметів
        Thread studentThread = new Thread(GenerateStudents);
        Thread subjectThread = new Thread(GenerateSubjects);

        // Запускаємо потоки
        studentThread.Start();
        subjectThread.Start();

        // Очікуємо завершення роботи обох потоків
        studentThread.Join();
        subjectThread.Join();

        // Зберігаємо згенеровані сутності в базі даних
        SaveToDatabase(studentNames, "Students");
        SaveToDatabase(subjectNames, "Subjects");

        // Виводимо дані з бази даних
        DisplayDataFromDatabase("Students");
        DisplayDataFromDatabase("Subjects");
    }

    // Метод для генерації імен студентів
    static void GenerateStudents()
    {
        for (int i = 1; i <= 10; i++)
        {
            string studentName = GenerateName("Студент", i);
            lock (lockObject)
            {
                studentNames.Add(studentName);
            }
            Thread.Sleep(100); // Імітуємо виконання деякої роботи
        }
    }

    // Метод для генерації імен предметів
    static void GenerateSubjects()
    {
        for (int i = 1; i <= 10; i++)
        {
            string subjectName = GenerateName("Предмет", i);
            lock (lockObject)
            {
                subjectNames.Add(subjectName);
            }
            Thread.Sleep(100); // Імітуємо виконання деякої роботи
        }
    }

    // Метод для генерації унікальних імен
    static string GenerateName(string prefix, int index)
    {
        return $"{prefix}{index}";
    }

    // Метод для збереження даних в базі даних
    static void SaveToDatabase(List<string> data, string tableName)
    {
        using (var dbContext = new MyDbContext())
        {
            if (tableName == "Students")
            {
                foreach (var studentName in data)
                {
                    dbContext.Students.Add(new Student { Name = studentName });
                }
            }
            else if (tableName == "Subjects")
            {
                foreach (var subjectName in data)
                {
                    dbContext.Subjects.Add(new Subject { Name = subjectName });
                }
            }

            dbContext.SaveChanges();
        }
    }
    // Метод для виведення даних з бази даних
    static void DisplayDataFromDatabase(string tableName)
    {
        using (var dbContext = new MyDbContext())
        {
            if (tableName == "Students")
            {
                var students = dbContext.Students.ToList();
                foreach (var student in students)
                {
                    Console.WriteLine($"Student Id: {student.Id}, Name: {student.Name}");
                }
            }
            else if (tableName == "Subjects")
            {
                var subjects = dbContext.Subjects.ToList();
                foreach (var subject in subjects)
                {
                    Console.WriteLine($"Subject Id: {subject.Id}, Name: {subject.Name}");
                }
            }
        }
    }
}   
