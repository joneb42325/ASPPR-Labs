using System;

namespace SimplexMethodLab
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n╔════════════ ГОЛОВНЕ МЕНЮ (Частина Б) ════════════╗");
                Console.WriteLine("║ 1. Розв'язати ЗЛП (Симплекс-метод)               ║");
                Console.WriteLine("║ 0. Вихід                                         ║");
                Console.WriteLine("╚══════════════════════════════════════════════════╝");
                Console.Write("Оберіть пункт: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": TaskSimplex(); break;
                    case "0": isRunning = false; break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        // ==========================================
        // ЗАВДАННЯ: Симплекс-метод
        // ==========================================
        static void TaskSimplex()
        {
            Console.WriteLine("\n=== Пошук розв'язку ЗЛП ===");
            Console.WriteLine("Оберіть тип задачі:\n1 - Максимізація (MAX)\n2 - Мінімізація (MIN)");
            Console.Write("Ваш вибір: ");

            bool isMax;
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "1") { isMax = true; break; }
                if (input == "2") { isMax = false; break; }
                Console.Write("Помилка. Введіть 1 або 2: ");
            }

            Console.Write("\nВведіть кількість обмежень (рядків без Z): ");
            int m = GetValidInt();
            Console.Write("Введіть кількість змінних X (стовпців без вільних членів): ");
            int n = GetValidInt();

            Console.WriteLine("\nЗаповнення початкової симплекс-таблиці.");
            Console.WriteLine("Увага: останній рядок - це функція Z, останній стовпець - вектор вільних членів (ВЧ).");

            int rows = m + 1;
            int cols = n + 1;
            double[,] table = ReadMatrix(rows, cols);

            string[] rowLabels = new string[rows];
            string[] colLabels = new string[cols];

            for (int i = 0; i < m; i++) rowLabels[i] = $"y{i + 1}";
            rowLabels[m] = "Z";

            for (int j = 0; j < n; j++) colLabels[j] = $"x{j + 1}";
            colLabels[n] = "ВЧ";

            Console.WriteLine("\nПочаткова симплекс-таблиця:");
            PrintSimplexTable(table, rowLabels, colLabels);


            // --- ФАЗА 1: Пошук опорного розв'язку ---
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n--- ФАЗА 1: Пошук опорного розв'язку ---");
            Console.ResetColor();

            while (true)
            {
                int pivotRow = -1;
                double minRhs = -1e-9;


                for (int i = 0; i < m; i++)
                {
                    if (table[i, n] < minRhs)
                    {
                        minRhs = table[i, n];
                        pivotRow = i;
                    }
                }

                if (pivotRow == -1) break;

                int pivotCol = -1;

                for (int j = 0; j < n; j++)
                {
                    if (table[pivotRow, j] < -1e-9)
                    {
                        pivotCol = j;
                        break;
                    }
                }

                if (pivotCol == -1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nСистема обмежень суперечлива. Розв'язку не існує.");
                    Console.ResetColor();
                    return;
                }



                Console.WriteLine($"\nКрок МЖВ: рядок {rowLabels[pivotRow]}, стовпець {colLabels[pivotCol]}");
                table = MjeStep(table, pivotRow, pivotCol);

                string temp = rowLabels[pivotRow];
                rowLabels[pivotRow] = colLabels[pivotCol];
                colLabels[pivotCol] = temp;

                PrintSimplexTable(table, rowLabels, colLabels);
            }

            // --- ФАЗА 2: Пошук оптимального розв'язку ---
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n--- ФАЗА 2: Пошук оптимального розв'язку ---");
            Console.ResetColor();

            while (true)
            {
                int pivotCol = -1;
                double mostNeg = -1e-9;

                for (int j = 0; j < n; j++)
                {
                    if (table[m, j] < mostNeg)
                    {
                        mostNeg = table[m, j];
                        pivotCol = j;
                    }
                }

                if (pivotCol == -1) break;

                int pivotRow = -1;
                double minRatio = double.MaxValue;

                // Шукаємо мінімальне відношення (ВЧ / додатний елемент стовпця)
                for (int i = 0; i < m; i++)
                {
                    if (table[i, pivotCol] > 1e-9)
                    {
                        double ratio = table[i, n] / table[i, pivotCol];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nЦільова функція необмежена. Оптимального розв'язку не існує.");
                    Console.ResetColor();
                    return;
                }

                Console.WriteLine($"\nКрок МЖВ: рядок {rowLabels[pivotRow]}, стовпець {colLabels[pivotCol]}");
                table = MjeStep(table, pivotRow, pivotCol);

                string temp = rowLabels[pivotRow];
                rowLabels[pivotRow] = colLabels[pivotCol];
                colLabels[pivotCol] = temp;

                PrintSimplexTable(table, rowLabels, colLabels);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=================================");
            Console.WriteLine(" ОПТИМАЛЬНИЙ РОЗВ'ЯЗОК ЗНАЙДЕНО! ");
            Console.WriteLine("=================================");
            Console.ResetColor();

            for (int i = 1; i <= n; i++)
            {
                string varName = $"x{i}";
                int rowIndex = Array.IndexOf(rowLabels, varName);
                double value = (rowIndex >= 0) ? table[rowIndex, n] : 0.0;
                Console.WriteLine($"{varName} = {value:F3}");
            }

            double zVal = table[m, n];
            if (!isMax) zVal = -zVal;
            Console.WriteLine($"\nZ = {zVal:F3}\n");
        }

        // ==========================================
        // Модифіковані Жорданові Виключення (МЖВ)
        // ==========================================
        static double[,] MjeStep(double[,] matrix, int r, int s)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double pivot = matrix[r, s];
            double[,] nextMatrix = new double[rows, cols];



            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i == r && j == s)
                        nextMatrix[i, j] = 1.0 / pivot;
                    else if (i == r)
                        nextMatrix[i, j] = matrix[i, j] / pivot;
                    else if (j == s)
                        nextMatrix[i, j] = -matrix[i, j] / pivot;
                    else
                        nextMatrix[i, j] = matrix[i, j] - (matrix[i, s] * matrix[r, j]) / pivot; // Правило прямокутника
                }
            }
            return nextMatrix;
        }

        // ==========================================
        // Допоміжні методи для вводу та виводу
        // ==========================================
        static double[,] ReadMatrix(int rows, int cols)
        {
            double[,] matrix = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"Елемент [{i + 1},{j + 1}]: ");
                    matrix[i, j] = GetValidDouble();
                }
            }
            return matrix;
        }

        static void PrintSimplexTable(double[,] matrix, string[] rowLabels, string[] colLabels)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            Console.Write("\t");
            for (int j = 0; j < cols; j++) Console.Write($"{colLabels[j],8} ");
            Console.WriteLine();

            for (int i = 0; i < rows; i++)
            {
                Console.Write($"{rowLabels[i]}\t");
                for (int j = 0; j < cols; j++)
                {
                    double val = Math.Abs(matrix[i, j]) < 1e-9 ? 0.0 : matrix[i, j];
                    Console.Write($"{val,8:F2} ");
                }
                Console.WriteLine();
            }
        }

        static int GetValidInt()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int result) && result > 0)
                    return result;
                Console.Write("Помилка. Введіть ціле додатне число: ");
            }
        }

        static double GetValidDouble()
        {
            while (true)
            {
                string input = Console.ReadLine().Replace('.', ',');
                if (double.TryParse(input, out double result))
                    return result;
                Console.Write("Помилка. Введіть число: ");
            }
        }
    }
}

