using System;

namespace JordanEliminationsLab
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n╔════════════ ГОЛОВНЕ МЕНЮ ════════════╗");
                Console.WriteLine("║ 1. Обернена матриця                  ║");
                Console.WriteLine("║ 2. Ранг матриці                      ║");
                Console.WriteLine("║ 3. Розв'язання СЛАР (Ax - B = 0)     ║");
                Console.WriteLine("║ 0. Вихід                             ║");
                Console.WriteLine("╚══════════════════════════════════════╝");
                Console.Write("Оберіть пункт: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": TaskInverseMatrix(); break;
                    case "2": TaskRankMatrix(); break;
                    case "3": TaskSolveSLE(); break;
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
        // ЗАВДАННЯ 1: Обернена матриця
        // ==========================================
        static void TaskInverseMatrix()
        {
            Console.Write("\nВведіть розмірність квадратної матриці (n): ");
            int n = GetValidInt();
            double[,] matrix = ReadMatrix(n, n);

            double[,] inverse = matrix;
            for (int i = 0; i < n; i++)
            {
                if (Math.Abs(inverse[i, i]) < 1e-9)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nПомилка: Діагональний елемент [{i},{i}] дорівнює нулю. Для простого алгоритму ЗЖВ матриця не підходить або є виродженою.");
                    Console.ResetColor();
                    return;
                }
                inverse = JordanStep(inverse, i, i);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n--- Обернена матриця ---");
            Console.ResetColor();
            PrintMatrix(inverse);
        }

        // ==========================================
        // ЗАВДАННЯ 2: Ранг матриці
        // ==========================================
        static void TaskRankMatrix()
        {
            Console.Write("\nВведіть кількість рядків: ");
            int rows = GetValidInt();
            Console.Write("Введіть кількість стовпців: ");
            int cols = GetValidInt();

            double[,] matrix = ReadMatrix(rows, cols);
            bool[] usedRows = new bool[rows];
            bool[] usedCols = new bool[cols];

            int rank = 0;
            int maxSteps = Math.Min(rows, cols);

            for (int step = 0; step < maxSteps; step++)
            {
                double maxPivot = 0;
                int pivotRow = -1;
                int pivotCol = -1;

                for (int i = 0; i < rows; i++)
                {
                    if (usedRows[i]) continue;
                    for (int j = 0; j < cols; j++)
                    {
                        if (usedCols[j]) continue;
                        if (Math.Abs(matrix[i, j]) > Math.Abs(maxPivot))
                        {
                            maxPivot = matrix[i, j];
                            pivotRow = i;
                            pivotCol = j;
                        }
                    }
                }

                if (Math.Abs(maxPivot) < 1e-9) break; 

                matrix = JordanStep(matrix, pivotRow, pivotCol);
                usedRows[pivotRow] = true;
                usedCols[pivotCol] = true;
                rank++;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nРанг матриці: {rank}");
            Console.ResetColor();
        }

        // ==========================================
        // ЗАВДАННЯ 4: СЛАР методом Ax - B = 0
        // ==========================================
        static void TaskSolveSLE()
        {
            Console.Write("\nВведіть кількість невідомих (n): ");
            int n = GetValidInt();

            Console.WriteLine("\nВведення матриці коефіцієнтів A:");
            double[,] A = ReadMatrix(n, n);

            Console.WriteLine("\nВведення вектора вільних членів B:");
            double[,] B = ReadMatrix(n, 1);

            double[,] M = new double[n, n + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    M[i, j] = A[i, j];
                }
                M[i, n] = -B[i, 0]; 
            }

            for (int i = 0; i < n; i++)
            {
                if (Math.Abs(M[i, i]) < 1e-9)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nПомилка: Нульовий розв'язувальний елемент [{i},{i}].");
                    Console.ResetColor();
                    return;
                }
                M = JordanStep(M, i, i);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n--- Розв'язок системи ---");
            Console.ResetColor();

            for (int i = 0; i < n; i++)
            {
                double xValue = M[i, n];
                Console.WriteLine($"X[{i + 1}] = {xValue:F3}");
            }
        }

        // ==========================================
        // Базовий алгоритм Звичайних Жорданових Виключень
        // ==========================================
        static double[,] JordanStep(double[,] matrix, int r, int s)
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
                        nextMatrix[i, j] = -matrix[i, j] / pivot;
                    else if (j == s)
                        nextMatrix[i, j] = matrix[i, j] / pivot;
                    else
                        nextMatrix[i, j] = matrix[i, j] - (matrix[i, s] * matrix[r, j]) / pivot;
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

        static void PrintMatrix(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"{matrix[i, j],8:F2} ");
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