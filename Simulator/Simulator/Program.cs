using System;
using System.Threading;
namespace Simulator
{
    class Program
    {
        private static Mutex CuravailableThreadsMutex;// mutex that helps us to follow the progress of the theards.
        static int availableThreads;
        private static Random random = new Random();
        static string fileLocation = "SpreadSheet.dat";
        static void Main(string[] args)
        {
            CuravailableThreadsMutex = new Mutex();
            int rows = Convert.ToInt32(args[0]);
            int cols = Convert.ToInt32(args[1]);
            int nThreads = Convert.ToInt32(args[2]);
            int nOperations = Convert.ToInt32(args[3]);
            int Sleep = Convert.ToInt32(args[4]);
            availableThreads = nThreads; //helps us to follow the progress of the theards
            // set the number of threads 
            ThreadPool.SetMinThreads(nThreads, 0);
            ThreadPool.SetMaxThreads(nThreads, 0);
            // create the spreadsheet
            SharableSpreadSheet spreadsheet = new SharableSpreadSheet(rows, cols);

            // fill the spreadsheet with Defaultive values
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    spreadsheet.setCell(i, j, "InitialCell" + i + j);
                }
            }
            //starting the theards run.
            for (int i = 0; i < nThreads; i++)
            {
                object[] tuple = { spreadsheet, nOperations, Sleep }; //the argument from the user
                ThreadPool.QueueUserWorkItem(MultiUserSimulator, tuple);// run the simulator
                CuravailableThreadsMutex.WaitOne();
                availableThreads--; //CS
                CuravailableThreadsMutex.ReleaseMutex();
            }

            while (nThreads != availableThreads) ; // wait to all threads done
            spreadsheet.save(fileLocation); // save the file

        }

        private static void MultiUserSimulator(Object Arguments)
        {
            object[] tuple = (object[])Arguments;
            SharableSpreadSheet spreadSheet = (SharableSpreadSheet)(tuple[0]);
            int SleepSeconds = (int)(tuple[2]);
            int nOperations = (int)(tuple[1]);
            int rndChoose; // which operation to make
            Tuple<int, int> spreadSheetSize = spreadSheet.getSize(); //initial size of the Sheet
            int row = spreadSheetSize.Item1;//initial size of the Sheet (rows).
            int col = spreadSheetSize.Item2;//initial size of the Sheet (cols).
            int RandomRow, RandomCol;// random Rol and Col
            String s; // random string    
            String id = "User [" + Thread.CurrentThread.ManagedThreadId + "]: ";
            spreadSheet.setConcurrentSearchLimit(30);
            for (int i = 0; i < nOperations; i++)
            {
                rndChoose = random.Next(1, 14);  // creates a number between 1 and 13
                RandomRow = random.Next(0, row);
                RandomCol = random.Next(0, col);
                switch (rndChoose) // choose random operation and activte it with random variables
                {
                    case 1: // getCell
                        s = spreadSheet.getCell(RandomRow, RandomCol);
                        if (s != null)
                            Console.WriteLine(id + "The String \"" + s + "\" found in cell[" + RandomRow + ", " + RandomCol + "].");
                        else
                            Console.WriteLine(id + "Didn't found any String that maches in cell[" + RandomRow + ", " + RandomCol + "].");
                        break;
                    case 2: // setCell

                        s = "RandomNewCell" + RandomRow + RandomCol;
                        spreadSheet.setCell(RandomRow, RandomCol, s);
                        String Value = spreadSheet.getCell(RandomRow, RandomCol);
                        if (Value == s)
                            Console.WriteLine(id + "Set new string \"" + s + "\" in cell[" + RandomRow + ", " + RandomCol + "].");
                        else
                            Console.WriteLine(id + "Didn't set new string \"" + s + "\" in cell[" + RandomRow + ", " + RandomCol + "].");
                        break;
                    case 3: // searchString
                        s = "InitialCell" + RandomRow + RandomCol;
                        Tuple<int, int> Solution = spreadSheet.searchString(s);
                        if(Solution != null)
                            if (Solution.Item1 == RandomRow && Solution.Item2 == RandomCol)
                                Console.WriteLine(id + "The string \"" + s + "\" has found in the cell[" + RandomRow + ", " + RandomCol + "] .");
                             else
                                Console.WriteLine(id + "The string \"" + s + "\" does not fount in cell[" + RandomRow + ", " + RandomCol + "] .");
                        else
                            Console.WriteLine(id + "The string \"" + s + "\" does not fount in cell[" + RandomRow + ", " + RandomCol + "] .");
                        break;
                    case 4: // exchangeRows
                        int RowToExchange = random.Next(0, row);
                        spreadSheet.exchangeRows(RandomRow, RowToExchange);
                        Console.WriteLine(id + "Rows [" + RandomRow + "] and [" + RowToExchange + "] exchanged successfully.");
                        break;
                    case 5: // exchangeCols
                        int ColToExchange = random.Next(0, col);
                        spreadSheet.exchangeCols(RandomCol, ColToExchange);
                        Console.WriteLine(id + "Cols [" + RandomCol + "] and [" + ColToExchange + "] exchanged successfully.");
                           break;
                    case 6: // searchInRow
                        s = "InitialCell" + RandomRow + RandomCol;
                        int FoundCol = spreadSheet.searchInRow(RandomRow, s);
                        if(FoundCol != -1)
                            Console.WriteLine(id + "The String \"" + s + "\" has found in row[" + RandomRow + "] .");
                        else
                            Console.WriteLine(id + "The String \"" + s + "\" does not found in row[" + RandomRow + "] .");
                        break;
                    case 7: // searchInCol
                        s = "InitialCell" + RandomRow + RandomCol;
                        int FoundRow = spreadSheet.searchInCol(RandomCol, s);
                        if(FoundRow != -1)
                            Console.WriteLine(id + "The String \"" + s + "\" has found in col[" + RandomCol + "] .");
                        else
                            Console.WriteLine(id + "The String \"" + s + "\" does not found in col[" + RandomCol + "] .");
                        break;
                    case 8: // searchInRange
                        int RandomRow2 = random.Next(RandomRow, row);
                        int RandomCol2 = random.Next(RandomCol, col);
                        s = "InitialCell" + random.Next(RandomRow, RandomRow2 + 1) + random.Next(RandomCol, RandomCol2 + 1);
                        Tuple<int,int> RandomCell = spreadSheet.searchInRange(RandomCol, RandomCol2, RandomRow, RandomRow2, s);
                        if(RandomCell != null)
                            Console.WriteLine(id + "The string \"" + s + "\" has founs in range [" + RandomRow + "-" + RandomRow2 + ", " + RandomCol + "-" + RandomCol2 + "] .");
                        else
                            Console.WriteLine(id + "The string \"" + s + "\" does not found in range  [" + RandomRow + "-" + RandomRow2 + ", " + RandomCol + "-" + RandomCol2 + "] .");
                        break;
                    case 9: // addRow
                        spreadSheet.addRow(RandomRow);
                        Console.WriteLine(id + "added new row after row " + RandomRow);
                        break;
                    case 10: // addCol
                        spreadSheet.addCol(RandomCol);
                        Console.WriteLine(id + "added new column after column " + RandomCol);
                        break;
                    case 11: // getSize
                        Tuple<int, int> SolutionSize = spreadSheet.getSize();
                        Console.WriteLine(id + "the size of the table is [" + SolutionSize.Item1 + ", " + SolutionSize.Item2 + "]");
                        break;
                    case 12: //CaseSensitive Test
                        Tuple<int, int>[] tuples = spreadSheet.findAll("TestCellNewRow", false);
                        if(tuples != null)
                            Console.WriteLine(id + "Case Sensitive False Check With TestCellNewRow Found");
                        else
                            Console.WriteLine(id + "Case Sensitive False Check With TestCellNewRow Not Found");
                        break;
                    case 13://CaseSensitive Test
                        Tuple<int, int>[] tuples2 = spreadSheet.findAll("testcellnewrow", true);
                        if (tuples2 == null)
                            Console.WriteLine(id + "Case Sensitive True Check With TestCellNewRow Succeed");
                        break;

                }
                Thread.Sleep(SleepSeconds);
            }
            CuravailableThreadsMutex.WaitOne();
            availableThreads++;
            CuravailableThreadsMutex.ReleaseMutex();
            Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: + FINISHED"); //for the debuging, to make shure the threads finished.

        }
    }

}