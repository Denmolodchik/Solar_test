using System;
using System.Threading;

namespace Solar
{
    class Program
    {
        static void Main(string[] args)
        {

            bool process = true;
            while (process)
            {
                Console.Clear();
                Console.Write(" Меню:\n  1: Создать событие.\n" +
                    "  2: Просмотреть Все события.\n" +
                    "  3: Удаление\n" +
                    "  4: Редактирование\n\n" +
                    "  Backspace: Выход  \n");
                var choice = Console.ReadKey().Key;
                switch (choice)
                {
                    // Создать событие
                    case ConsoleKey.D1:
                        using (Scheduler db = new Scheduler())
                        {
                            db.Elements.Add(db.CreateElement());
                            db.SaveChanges();
                        }

                        Console.WriteLine("\n\n\t\tГотово!");
                        Thread.Sleep(800);
                        break;

                    // Просмотреть Все события.
                    case ConsoleKey.D2:
                        bool sortProcess = true;
                        Console.Clear();
                        using (Scheduler db = new Scheduler())
                        {
                            db.PrintElements();
                            while (sortProcess)
                            {
                                string menu = "\n\nShift: По возрастанию. Alt: По убыванию.\n" +
                                    " 1: Дата начала;\t 2: Дата конца\n" +
                                    " Alt + 3: События, которые будут, либо идут;\n Alt + 4: События, которые прошли\n" +
                                    " Backspace: Выход.";
                                Console.WriteLine(menu);

                                var keypress = Console.ReadKey();


                                if (keypress.Key == ConsoleKey.Backspace)
                                {
                                    break;
                                }

                                else if ((ConsoleModifiers.Shift & keypress.Modifiers) != 0)
                                {
                                    switch (keypress.Key)
                                    {
                                        case ConsoleKey.D1: // shift 2
                                            Console.Clear();
                                            db.PrintElementsOrderByStartAsc();
                                            break;

                                        case ConsoleKey.D2: // shift 3
                                            Console.Clear();
                                            db.PrintElementsOrderByFinishAsc();
                                            break;

                                        default:
                                            Console.Clear();
                                            db.PrintElements();
                                            break;
                                    }
                                }
                                else if ((ConsoleModifiers.Alt & keypress.Modifiers) != 0)
                                {
                                    switch (keypress.Key)
                                    {
                                        case ConsoleKey.D1: // alt 1
                                            Console.Clear();
                                            db.PrintElementsOrderByStartDesc();
                                            break;

                                        case ConsoleKey.D2: // alt 2
                                            Console.Clear();
                                            db.PrintElementsOrderByFinishDesc();
                                            break;

                                        case ConsoleKey.D3: // alt 3
                                            Console.Clear();
                                            db.PrintElementsExpired();
                                            break;

                                        case ConsoleKey.D4: // alt 4
                                            Console.Clear();
                                            db.PrintElementsComing();
                                            break;

                                        default:
                                            Console.Clear();
                                            db.PrintElements();
                                            break;
                                    }
                                }

                            }

                        }
                        break;

                    // Удаление
                    case ConsoleKey.D3:

                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("Удаление, для выхода введите *");

                            using (Scheduler db = new Scheduler())
                            {
                                db.PrintElements();
                            }
                            Console.Write("\n\nID поля: ");
                            string inputDel = Console.ReadLine();
                            if (inputDel != "*")
                            {
                                int delId = Convert.ToInt32(inputDel);
                                Element toDelete = new Element { Id = delId };
                                using (Scheduler db = new Scheduler())
                                {
                                    db.Elements.Attach(toDelete);
                                    db.Elements.Remove(toDelete);
                                    db.SaveChanges();
                                }

                            }
                            else
                            {
                                break;
                            }

                        }
                        break;

                    // Редактирование
                    case ConsoleKey.D4:
                        Console.Clear();
                        Console.WriteLine("Редактирование, для выхода введите *:");
                        using (Scheduler db = new Scheduler())
                        {
                            db.PrintElements();
                        }
                        Console.Write("\n\nID элемента: ");
                        string inputEdit = Console.ReadLine();
                        if (inputEdit != "*")
                        {
                            int edit = Convert.ToInt32(inputEdit);
                            using (Scheduler db = new Scheduler())
                            {
                                db.Elements.Update(db.CreateElement(db.GetElementById(edit)));
                                db.SaveChanges();
                            }

                            Console.WriteLine("\n\n\t\tИзменено!");
                            Thread.Sleep(800);

                        }

                        break;

                    // Выход 
                    case ConsoleKey.Backspace:
                        process = false;
                        break;
                }

            }

        }
    }
}