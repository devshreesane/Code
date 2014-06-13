using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCode
{
    class FinStats
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\devsane\Downloads\FinStats.csv";
            string[][] rows = File.ReadAllLines(filePath)
                .Select(x => x.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
                .ToArray();
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = rows[i].Select(x => x.Split(new char[] {'"'})[1]).ToArray();
            }

            if (rows.Length >= 2)
            {
                string[] header = rows[0];
                Dictionary<string, int> headers = new Dictionary<string,int>();
                for(int i=0;i<header.Length;i++)
                {
                    headers.Add(header[i],i);
                }

                Dictionary<int, BiWeeklyExpense> expenses = new Dictionary<int, BiWeeklyExpense>();
                int dateHeaderIndex = headers["Transaction_Date"];
                int amtHeaderIndex = headers["Amount"];
                for (int i = 1; i < rows.Length; i++)
                {
                    
                    int dateHash = FinStats.GetDateHash(rows[i][dateHeaderIndex]);
                    if(!expenses.ContainsKey(dateHash))
                    {
                        expenses.Add(dateHash, new BiWeeklyExpense());
                    }
                    BiWeeklyExpense bwe = expenses[dateHash];
                    bwe.AddExpense(double.Parse(rows[i][amtHeaderIndex]));

                }
                Console.WriteLine("BiWeek    Credit    Debit");
                for (int i = 0; i < 24; i++)
                {
                    if (expenses.ContainsKey(i) && expenses[i].TransactionCount > 0)
                    {
                        Console.WriteLine(i + "     " + expenses[i].Credit + "    " + expenses[i].Debit);



                    }
                }

            }
            else
            {
                throw new InvalidDataException("Empty expenses file");
            }
            
        }

        public static int GetDateHash(string date)
        {
            DateTime dt = DateTime.Parse(date, CultureInfo.CreateSpecificCulture("en-US"));
            int month = dt.Month-1;
            int firstHalf = dt.Day <= 15 ? 0 : 1;
            return 2 * month + firstHalf;
        }

        public class BiWeeklyExpense
        {
            double credit = 0;

            public double Credit { get { return credit; } }

            double debit = 0;

            public double Debit { get { return debit; } }

            int numTransactions = 0;

            public int TransactionCount { get { return numTransactions; } }

            public void AddExpense(double value)
            {
                if (value < 0)
                {
                    debit += Math.Abs(value);
                }
                else
                {
                    credit += value;
                }
                numTransactions++;
            }
        }
    }
}
