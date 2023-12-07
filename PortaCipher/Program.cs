﻿using System.Text;

namespace PortaCipher
{
    internal class Program
    {
        static bool run = true;
        static void Main()
        {
            while (run)
            {
                Menu.MenuItem();
                int item;
                Console.Write("Выберите пункт меню: ");
                while (!int.TryParse(Console.ReadLine(), out item))
                {
                    Console.Write("Введите число: ");
                }

                switch (item)
                {
                    case 1:
                        EncryptPort.Encrypt();
                        break;
                    case 2:
                        break;
                    default:
                        run = false;
                        break;

                }
            }
        }
    }

    internal static class EncryptPort
    {
        static readonly string[] separator = { " ", ",", ".", "-" };    // массив разделителей
        static string consoleText = "";                                 // переменная для работы с консолью(для чтения)
        public static void Encrypt()
        {
            Console.WriteLine("Введите фразу:");
            consoleText = Console.ReadLine() ?? "";
            if (consoleText != "")
            {
                string[] plaintext = consoleText.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.Plaintext = string.Join("", plaintext);
            }
            Console.WriteLine("Введите ключ:");
            consoleText = Console.ReadLine() ?? "";
            if (consoleText != "")
            {
                string[] key = consoleText.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.Key = string.Join("", key);
            }
            Console.WriteLine("Введите лозунг:");
            consoleText = Console.ReadLine() ?? "";
            if (consoleText != "")
            {
                string[] slogan = consoleText.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.Slogan = string.Join("", slogan);
            }
            AlgoritmPort.PortaCipher();

        }
    }
    internal static class AlgoritmPort
    {
        static readonly char[] plaintext = Global.Plaintext.ToCharArray();
        static readonly char[] key = Global.Key.ToCharArray();
        static string encryptedText = string.Empty;
        static readonly int[] rowItems = new int[plaintext.Length];       // номера строк, где будут присутствовать буквы ключа
        static readonly int[] colItems = new int[plaintext.Length];        // номера столбцов, где находятся буквы текста

        internal static void PortaCipher()
        {
            CreateTable();
            GetKey();

            for (int i = 0; i < plaintext.Length; i++)
            {
                encryptedText += Global.table[rowItems[i], colItems[i]];
            }
            Global.EncryptedText = encryptedText;
            Console.WriteLine("Зашифрованная фраза: " + encryptedText);
        }
        internal static void CreateTable()              // создание таблицы
        {
            const string path = "alphabet.txt";
            string content = File.ReadAllText(path);    // считываем алфавит
            string[] alphabet = content.Split(' ');     // создаем массив букв алфавита
            string[] s = new string[alphabet.Length];
            Array.Copy(alphabet, s, alphabet.Length);   // создаём копию для дальнейшей работы

            List<string> row = new();                   // список, содержащий начальную строку нашей таблицы
            StringBuilder sb = new(Global.Slogan);      // создаем элемент StringBuilder, помещая в него лозунг
            for (int i = 0; i < sb.Length; i++)         // данный цикл удаляет повторяющиеся буквы в лозунге
            {
                for (int j = 0; j < sb.Length; j++)
                {
                    if (sb[i] == sb[j] && i != j)
                        sb.Remove(j, 1);
                }
            }
            for (int i = 0; i < sb.Length; i++)        // данный цикл удаляет буквы лозунга из алфавита
            {
                s = s.Where(s => s != Convert.ToString(sb[i])).ToArray();
                row.Add(Convert.ToString(sb[i]));      // ... и заполняет первые буквы в список
            }
            for (int i = 0; i < s.Length; i++)         // добавляем недостающие буквы (получаем данный вид:
                row.Add(s[i]);                         // <буквы лозунга><буквы алфавита>

            for (int i = 0; i < row.Count; i++)        // цикл, заполняющий таблицу
            {
                for (int j = 0; j < row.Count; j++)
                {
                    Global.table[i, j] = row[j];
                }
                string deleteElement = row[0];         // сдвиг элементов строки таблицы (удаление первого элемента
                row.RemoveAt(0);                       // ... и запись его в конец
                row.Add(deleteElement);
            }
            Console.WriteLine();
            Print2DArray(Global.table);                // печать таблицы в консоль

            for (int i = 0; i < key.Length; i++)      // цикл, находящий индексы строки в нулевом столбце, для дальнейшего упрощения шифрования
            {                                          // на позициях [j, 0] находятся символы ключа
                for (int j = 0; j < row.Count; j++)
                {
                    if (Global.table[j, 0] == Convert.ToString(key[i]))
                        rowItems[i] = j;
                }
            }
            for (int i = 0; i < plaintext.Length; i++)  // цикл, находящий индексы столбцов в нулевой строке, для дальнейшего упрощения шифрования
            {                                           // на позициях [0, j] находятся символы открытого текста    
                for (int j = 0; j < row.Count; j++)
                {
                    if (Global.table[0, j] == Convert.ToString(plaintext[i]))
                        colItems[i] = j;
                }
            }
        }

        static void GetKey()
        {
            string extendedKey = string.Empty;              // расширенный ключ
            int m = plaintext.Length / key.Length;          // сколько полных ключей запишется над фразой 
            int k = plaintext.Length - key.Length * m;      // к-во недостающих букв
            for (int i = 0; i < m; i++)                     // циклы, которые расширяют ключ для удобства дальнейшей работы
            {
                for (int j = 0; j < key.Length; j++)
                    extendedKey += key[j];
            }
            for (int i = 0; i < k; i++)
            {
                extendedKey += key[i];
            }
            Console.WriteLine(extendedKey);
            Console.WriteLine(plaintext);

            for (int i = key.Length; i < plaintext.Length; i++) // дописываем индексы
            {
                rowItems[i] = rowItems[i - key.Length];
            }
        }

        static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
    internal static class Global
    {
        public static string Plaintext { get; set; } = default!;      // открытый текст
        public static string EncryptedText { get; set; } = default!; // зашифрованный текст
        public static string Key { get; set; } = default!;           // ключ
        public static string Slogan { get; set; } = default!;        // лозунг

        public static string[,] table = new string[33, 33];           // таблица
    }

    internal static class Menu
    {
        internal static void MenuItem()
        {
            Console.WriteLine("\t\tМЕНЮ");
            Console.WriteLine("1. Зашифровать");
            Console.WriteLine("2. Расшифровать");
            Console.WriteLine("3. Выход");
        }
    }

}