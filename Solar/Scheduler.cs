using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Solar
{
    class Scheduler : DbContext
    {
        public DbSet<Element> Elements { get; set; }

        public Scheduler()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server= (localdb)\\mssqllocaldb;Database=SchedulerDb;Trusted_Connection=True;");
        }

        //Поиск элемента по Id
        public Element GetElementById(int id)
        {
            return Elements.Where(p => p.Id == id).ToList()[0];
        }

        //Вывести значения из БД
        private void PrintList(List<Element> el)
        {
            Console.WriteLine("Список объектов:\n ID\t|\t Название\t|\t Описание\t|\t Дата начала\t|\t Дата окончания\n");
            foreach (Element e in el)
            {
                Console.WriteLine($" {e.Id}\t|\t " +
                    $"{e.Name,8}\t|\t " +
                    $"{e.Description,8}\t|\t " +
                    $"{e.start:d}\t|\t   " +
                    $"{e.finish:d} ");
            }

        }

        public void PrintElements()
        {
            PrintList(Elements.ToList());
        }

        //Создание и изменение записи из БД

        public Element CreateElement(Element el = null)
        {
            if (el == null)
                el = new Element();

            for (int i = 0; i < 4; i++)
            {
                string eventMenu = $"Событие:\n" +
                    $" 1 Название: \t\t{el.Name}\n" +
                    $" 2 Описание: \t\t{el.Description}\n" +
                    $" 3 Дата начала события: \t{el.start}\n" +
                    $" 4 Дата окончания события: \t{el.finish}\n";
                Console.Clear();
                Console.Write(eventMenu);
                string input = null;
                DateTime date = new DateTime();
                if (i < 2)
                {
                    Console.Write($" \n\n Ввод {i + 1}: ");
                    input = Console.ReadLine().ToString();
                }
                else
                {
                    int y = 2000, m = 12, d=12;
                    for (int j = 0; j < 3; j++)
                    {
                        Console.Clear();
                        Console.Write(eventMenu);
                        using (Scheduler db = new Scheduler())
                        {
                            switch (j)
                            {
                                case 0:
                                    Console.Write("\nДень: ");
                                    d = Convert.ToInt32(Console.ReadLine());
                                    break;
                                    if (d > 31 || d < 1)
                                    {
                                        Console.Clear();
                                        db.Elements.Add(el);
                                        Console.WriteLine("\n\n\t\tНекорректный день!");
                                        Thread.Sleep(800);
                                        j--;
                                    }
                                case 1:
                                    Console.Write("\nМесяц: ");
                                    m = Convert.ToInt32(Console.ReadLine());
                                    if (m > 12 || m < 1)
                                    {
                                        Console.Clear();
                                        db.Elements.Add(el);
                                        Console.WriteLine("\n\n\t\tНекорректный номер месяца!");
                                        Thread.Sleep(800);
                                        j--;
                                    }
                                    break;

                                case 2:
                                    Console.Write("\nГод: ");
                                    y = Convert.ToInt32(Console.ReadLine());
                                    if (new DateTime(y,m,d)<DateTime.Now.Date)
                                    {
                                        Console.Clear();
                                        db.Elements.Add(el);
                                        Console.WriteLine("\n\n\t\tЭта дата уже прошла");
                                        Thread.Sleep(800);
                                        j = -1;
                                    }
                                    if ((i == 3) && (new DateTime(y, m, d) < el.start))
                                    {
                                        Console.Clear();
                                        db.Elements.Add(el);
                                        Console.WriteLine("\n\n\t\tНельзя завершить дело раньше, чем начать");
                                        Thread.Sleep(800);
                                        j = -1;
                                    }
                                    break;

                            }
                        }
                    }
                    date = new DateTime(y, m, d);
                }

                switch (i)
                {
                    case 0:
                        el.Name = input;
                        break;
                    case 1:
                        el.Description = input;
                        break;
                    case 2:
                        el.start = date;
                        break;
                    case 3:
                        el.finish = date;
                        Console.Clear();
                        break;
                }
            }
            return el;
        }

        // Сортировка по Дате начала
        public void PrintElementsOrderByStartAsc() // Сортировка по возрастанию
        {
            PrintList(Elements.OrderBy(p => p.start).ToList());
        }

        public void PrintElementsOrderByStartDesc() // Сортировка по убыванию
        {
            PrintList(Elements.OrderByDescending(p => p.start).ToList());
        }

        // Сортировка по Дате окончания
        public void PrintElementsOrderByFinishAsc() // Сортировка по возрастанию
        {
            PrintList(Elements.OrderBy(p => p.finish).ToList());
        }

        public void PrintElementsOrderByFinishDesc() // Сортировка по убыванию
        {
            PrintList(Elements.OrderByDescending(p => p.finish).ToList());
        }

        // Дела, которые ещё предстоят
        public void PrintElementsComing() 
        {
            PrintList(Elements.Where(p => p.finish < DateTime.Now.Date).ToList());
        }

        // Дела, которые уже просрочены
        public void PrintElementsExpired() 
        {
            PrintList(Elements.Where(p => p.finish > DateTime.Now.Date).ToList());

        }
    }
}
