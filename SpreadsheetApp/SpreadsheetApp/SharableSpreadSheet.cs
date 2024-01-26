using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace SpreadsheetApp
{
    class SharableSpreadSheet
    {
        private static Semaphore BlockWriter; // Enable/Disable Writing.
        private static Semaphore SearchingSemaphore; // limit the number of searchers
        private static Mutex CurReaderMutex; //Only One allowed perform CS of Incrementing / Decrementing Current Readers.
        private static Mutex CurSearchingMutex; //Only One allowed perform CS of Incrementing / Decremting Current Searchers. 
        private string[,] SpreadSheetMatrix;
        private int rows;
        private int cols;
        // nUsers used for setConcurrentSearchLimit, -1 mean no limit.
        private int CurrnUsers; //limit the number of users that can perform the search
        private int CurrReaders;
        private int CurrSearching;
        public SharableSpreadSheet(int nRows, int mCols, int nUsers = -1)
        {
            BlockWriter = new Semaphore(1, 2);//Maybe(0,1)??
            if (nUsers != -1)
                SearchingSemaphore = new Semaphore(1, nUsers);//NOTE******
            else
                SearchingSemaphore = new Semaphore(0, Int32.MaxValue);
            CurReaderMutex = new Mutex();
            CurSearchingMutex = new Mutex();
            rows = nRows;
            cols = mCols;
            SpreadSheetMatrix = new string[rows, cols];
            CurrnUsers = nUsers; //no limit defult
            CurrReaders = 0;
            CurrSearching = 0;

        }

        public void BeforeSearch()
        {
            //CS - Incrementing CurrReaders
            CurReaderMutex.WaitOne();
            CurrReaders++;
            //If The Current Thread is the First Thread to Read, than Block the Writing Option.
            if (CurrReaders == 1)
                BlockWriter.WaitOne();
            CurReaderMutex.ReleaseMutex();
            //End Of CS-Incrementing CurrReaders

            //CS - SearchingSemaphore - Making sure we dont pass through the nUsers limit. 
            CurSearchingMutex.WaitOne();
            CurrSearching++;
            if (CurrnUsers != -1)
                SearchingSemaphore.WaitOne();
            CurSearchingMutex.ReleaseMutex();
            //End of CS 
        }

        public void AfterSearch()
        {
            //CS - Decrementing CurrReaders
            CurReaderMutex.WaitOne();
            CurrReaders--;
            //CS - CurSearching Decrementing 
            CurSearchingMutex.WaitOne();
            CurrSearching--;
            if (CurrnUsers != -1)
                SearchingSemaphore.Release();
            CurSearchingMutex.ReleaseMutex();
            
            //If The Current Thread is the Last Thread to Read, than Unlock the Writing Option.
            if (CurrReaders == 0)
                BlockWriter.Release();
            CurReaderMutex.ReleaseMutex();
            //End Of CS-Decrementing CurrReaders
        }
        public string getCell(int row, int col)
        {
            // return the string at [row,col]
            if (row >= 0 && row < rows && col >= 0 && col < cols)
                return SpreadSheetMatrix[row, col];
            return null;
        }
        public void setCell(int row, int col, string str)
        {
            if (row >= 0 && row < rows && col >= 0 && col < cols)
            {
                // set the string at [row,col]
                BlockWriter.WaitOne();
                //CS
                SpreadSheetMatrix[row, col] = str;
                //END OF CS
                BlockWriter.Release();
            }

        }

        public Tuple<int, int> searchString(string str)
        {
            BeforeSearch();
            Tuple<int, int> Solution = null;
            bool found = false;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (SpreadSheetMatrix[i, j] == str)
                    {
                        Solution = new Tuple<int, int>(i, j);
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            // return first cell indexes that contains the string (search from first row to the last row)
            AfterSearch();
            return Solution;
        }
        public void exchangeRows(int row1, int row2)
        {
            if (row1 >= 0 && row1 < rows && row2 >= 0 && row2 < rows)
            {
                string temp;
                // exchange the content of row1 and row2
                BlockWriter.WaitOne();
                //CS
                for (int j = 0; j < cols; j++)
                {
                    temp = SpreadSheetMatrix[row1, j];
                    SpreadSheetMatrix[row1, j] = string.Copy(SpreadSheetMatrix[row2, j]);
                    SpreadSheetMatrix[row2, j] = string.Copy(temp);
                }
                //END OF CS
                BlockWriter.Release();
            }
        }
        public void exchangeCols(int col1, int col2)
        {
            // exchange the content of col1 and col2
            if (col1 >= 0 && col1 < cols && col2 >= 0 && col2 < cols)
            {
                //CS
                BlockWriter.WaitOne();
                string temp;
                // exchange the content of row1 and row2
                for (int j = 0; j < rows; j++)
                {
                    temp = SpreadSheetMatrix[j, col1];
                    SpreadSheetMatrix[j, col1] = string.Copy(SpreadSheetMatrix[j, col2]);
                    SpreadSheetMatrix[j, col2] = string.Copy(temp);
                }
                //END OF CS
                BlockWriter.Release();
            }
        }
        public int searchInRow(int row, string str)
        {
            int Solution = -1;
            if (row >= 0 && row < rows)
            {
                BeforeSearch();
                //Returns Col (i) 
                for (int i = 0; i < cols; i++)
                {
                    if (SpreadSheetMatrix[row, i] == str)
                    {
                        Solution = i;
                        break;
                    }
                }
                AfterSearch();
            }
            return Solution;
        }
        public int searchInCol(int col, string str)
        {
            int Solution = -1;
            if (col >= 0 && col < cols)
            {
                BeforeSearch();
                for (int i = 0; i < rows; i++)
                {
                    if (SpreadSheetMatrix[i, col] == str)
                    {
                        Solution = i;
                        break;
                    }
                }
                AfterSearch();
            }
            return Solution;
        }
        public Tuple<int, int> searchInRange(int col1, int col2, int row1, int row2, string str)

        {
            Tuple<int, int> Solution = null;
            if (row1 >= 0 && row1 < rows && row2 >= 0 && row2 < rows && col1 >= 0 && col1 < cols && col2 >= 0 && col2 < cols)
            {
                BeforeSearch();
                bool found = false;
                for (int i = row1; i <= row2; i++)
                {
                    for (int j = col1; j <= col2; j++)
                    {
                        if (SpreadSheetMatrix[i, j] == str)
                        {
                            Solution = new Tuple<int, int>(i, j);
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }
                AfterSearch();
            }
            return Solution;
        }
        public void addRow(int row1)
        {
            //add a row after row1
            if (row1 >= 0 && row1 <= rows)
            {
                //CS
                BlockWriter.WaitOne();
                string[,] newSpreadSheetMatrix = new string[rows + 1, cols];
                int k = 0;
                for (int i = 0; i <= rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        //if i == rows insert new row
                        if (i == row1+1)
                        {
                            newSpreadSheetMatrix[i, j] = "TestCellNewRow";
                            k = -1;
                        }
                        //else copy to the new SpreadSheetMatrix
                        else
                        {
                            newSpreadSheetMatrix[i, j] = string.Copy(SpreadSheetMatrix[k+i, j]);
                        }
                    }
                }
                SpreadSheetMatrix = newSpreadSheetMatrix;
                rows++;
                //END OF CS
                BlockWriter.Release();
            }
        }
        public void addCol(int col1)
        {
            //add a column after col1
            if (col1 >= 0 && col1 <= cols)
            {
                //CS
                BlockWriter.WaitOne();
                string[,] newSpreadSheetMatrix = new string[rows, cols + 1];
                for (int i = 0; i < rows; i++)
                {
                    int k = 0;
                    for (int j = 0; j <= cols; j++)
                    {
                        //if i == cols insert new col
                        if (j == col1+1)
                        {
                            newSpreadSheetMatrix[i, j] = "testcellNewCol";
                        }
                        //else copy to the new SpreadSheetMatrix
                        else
                        {
                            newSpreadSheetMatrix[i, j] = string.Copy(SpreadSheetMatrix[i, k++]);
                        }

                    }
                }
                SpreadSheetMatrix = newSpreadSheetMatrix;
                cols++;
                BlockWriter.Release();
            }
        }
        public Tuple<int, int>[] findAll(string str, bool caseSensitive)
        {
            BeforeSearch();
            Tuple<int, int> Solution = null;
            ArrayList Solutions = new ArrayList();
            Tuple<int, int>[] SolutionArray = null;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (caseSensitive && SpreadSheetMatrix[i, j].Equals(str))
                    {
                        Solution = new Tuple<int, int>(i, j);
                        Solutions.Add(Solution);
                    }
                    else if (SpreadSheetMatrix[i, j] == str && !caseSensitive)
                    {
                        Solution = new Tuple<int, int>(i, j);
                        Solutions.Add(Solution);
                    }
                }
            }
            SolutionArray = (Tuple<int, int>[])Solutions.ToArray(typeof(Tuple<int, int>));
            // return first cell indexes that contains the string (search from first row to the last row)
            AfterSearch();
            return SolutionArray;
            // perform search and return all relevant cells according to caseSensitive param
        }
        public void setAll(string oldStr, string newStr, bool caseSensitive)
        {
            // replace all oldStr cells with the newStr str according to caseSensitive param
            BlockWriter.WaitOne();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (caseSensitive && SpreadSheetMatrix[i, j].Equals(oldStr))
                    {
                        SpreadSheetMatrix[i, j] = newStr;
                    }
                    else if (SpreadSheetMatrix[rows, cols] == oldStr && !caseSensitive)
                    {
                        SpreadSheetMatrix[i, j] = newStr;
                    }
                }
            }
            BlockWriter.Release();
        }
        public Tuple<int, int> getSize()
        {
            Tuple<int, int> Size = new Tuple<int, int>(rows, cols);
            // return the size of the spreadsheet in nRows, nCols
            return Size;
        }
        public void setConcurrentSearchLimit(int nUsers)
        {
            // this function aims to limit the number of users that can perform the search operations concurrently.
            // The default is no limit. When the function is called, the max number of concurrent search operations is set to nUsers. 
            // In this case additional search operations will wait for existing search to finish.
            // This function is used just in the creation
            CurReaderMutex.WaitOne();
            if (nUsers < 1 || nUsers < CurrSearching)
            {
                CurReaderMutex.ReleaseMutex();
                return;
            }
            if (CurrnUsers == -1)
            {
                SearchingSemaphore.Release(nUsers);
            }
            else if (nUsers != CurrnUsers)
            {
                if (nUsers < CurrnUsers)
                {
                    for (int i = nUsers; i < CurrnUsers; i++)
                        SearchingSemaphore.WaitOne();
                }
                else
                {
                    SearchingSemaphore.Release(nUsers - CurrnUsers);
                }
            }
            CurrnUsers = nUsers;
            CurReaderMutex.ReleaseMutex();
        }





        public void save(string fileName)
        {
            // save the spreadsheet to a file fileName.
            // you can decide the format you save the data. There are several options.
            //CS - Incrementing CurrReaders
            CurReaderMutex.WaitOne();
            CurrReaders++;
            //If The Current Thread is the First Thread to Read, than Block the Writing Option.
            if (CurrReaders == 1)
                BlockWriter.WaitOne();
            CurReaderMutex.ReleaseMutex();
            //End Of CS-Incrementing CurrReaders
            //End of CS 
            StreamWriter File = new StreamWriter(fileName);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    File.Write(SpreadSheetMatrix[i, j] + ",");

                }
                File.WriteLine();
            }
            File.Close();
            //CS - Decrementing CurrReaders
            CurReaderMutex.WaitOne();
            CurrReaders--;
            CurReaderMutex.ReleaseMutex();
            //If The Current Thread is the Last Thread to Read, than Unlock the Writing Option.
            if (CurrReaders == 0)
                BlockWriter.Release();
            //End Of CS-Decrementing CurrReaders
        }
        public void load(string fileName)
        {
            // load the spreadsheet from fileName
            // replace the data and size of the current spreadsheet with the loaded data

            string FileTxt = File.ReadAllText(fileName);//getting the content of the file

            //Split into lines.
            FileTxt = FileTxt.Replace('\n', '\r');
            string[] lines = FileTxt.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

            //getting the number rows and coloms from the fileName
            int rowsFile = lines.Length;
            int coldFile = lines[0].Split(',').Length ;

            // Allocate data to mattrix
            string[,] FileMttrix = new string[rowsFile, coldFile];

            //load to the FileMttrix
            for (int i = 0; i < rowsFile; i++)
            {
                string[] line_r = lines[i].Split(',');
                for (int j = 0; j < coldFile; j++)
                {
                    FileMttrix[i, j] = line_r[j];
                }
            }

            SpreadSheetMatrix = FileMttrix;
            rows = rowsFile;
            cols = coldFile;
        }
    }

}
