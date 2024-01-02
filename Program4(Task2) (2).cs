using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static List<Sportswear> sportswearItems = new List<Sportswear>();
    static List<Footwear> footwearItems = new List<Footwear>();

    static async Task Main()
    {
        // Створюємо завдання для генерації спортивного одягу та взуття
        Task sportswearTask = Task.Run(() => GenerateSportswear());
        Task footwearTask = Task.Run(() => GenerateFootwear());

        // Очікуємо завершення обох завдань
        await Task.WhenAll(sportswearTask, footwearTask);

        // Зберігаємо згенеровані сутності в базі даних
        await SaveToDatabaseAsync(sportswearItems, "Sportswear");
        await SaveToDatabaseAsync(footwearItems, "Footwear");

        // Виводимо дані з бази даних
        await DisplayDataFromDatabaseAsync("Sportswear");
        await DisplayDataFromDatabaseAsync("Footwear");
    }

    // Метод для генерації спортивного одягу
    static void GenerateSportswear()
    {
        for (int i = 1; i <= 10; i++)
        {
            Sportswear item = new Sportswear
            {
                Name = $"Sportswear{i}",
                Size = $"Size{i}",
                SportType = $"SportType{i}"
            };

            lock (sportswearItems)
            {
                sportswearItems.Add(item);
            }
            Task.Delay(100).Wait(); // Імітуємо виконання деякої роботи
        }
    }

    // Метод для генерації взуття
    static void GenerateFootwear()
    {
        for (int i = 1; i <= 10; i++)
        {
            Footwear item = new Footwear
            {
                Name = $"Footwear{i}",
                Size = $"Size{i}",
                ShoeType = $"ShoeType{i}"
            };

            lock (footwearItems)
            {
                footwearItems.Add(item);
            }
            Task.Delay(100).Wait(); // Імітуємо виконання деякої роботи
        }
    }

    // Метод для збереження даних в базі даних (асинхронний варіант)
    static async Task SaveToDatabaseAsync<T>(List<T> data, string tableName) where T : Product
    {
        using (var context = new AppDbContext())
        {
            if (tableName == "Sportswear")
            {
                context.Sportswears.AddRange(data.Cast<Sportswear>());
            }
            else if (tableName == "Footwear")
            {
                context.Footwears.AddRange(data.Cast<Footwear>());
            }

            await context.SaveChangesAsync();
        }
    }


    // Метод для виведення даних з бази даних (асинхронний варіант)
    static async Task DisplayDataFromDatabaseAsync(string tableName)
    {
        using (var context = new AppDbContext())
        {
            if (tableName == "Sportswear")
            {
                var sportswears = await context.Sportswears.ToListAsync();
                foreach (var sportswear in sportswears)
                {
                    Console.WriteLine($"Sportswear: {sportswear.Name}, Size: {sportswear.Size}, SportType: {sportswear.SportType}");
                }
            }
            else if (tableName == "Footwear")
            {
                var footwears = await context.Footwears.ToListAsync();
                foreach (var footwear in footwears)
                {
                    Console.WriteLine($"Footwear: {footwear.Name}, Size: {footwear.Size}, ShoeType: {footwear.ShoeType}");
                }
            }
        }
    }
}

// Базовий клас для всіх товарів
class Product
{
    public string Name { get; set; }
    public string Size { get; set; }
}

// Клас для представлення спортивного одягу
class Sportswear : Product
{
    public string SportType { get; set; }
}

// Клас для представлення взуття
class Footwear : Product
{
    public string ShoeType { get; set; }
}
