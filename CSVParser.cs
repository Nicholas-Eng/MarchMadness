using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MarchMadness
{
    public static class CSVParser
    {
        public static IEnumerable<T> ParseCsvFile<T>(string fileName)
        {
            var parsedData = new List<T>();
            string filePath = GetDataFilePath(fileName);

            if(filePath != null) 
            {
                try
                {
                    using (var streamReader = new StreamReader(filePath))
                    {
                        string line;
                        string[] propertyNames = new string[]{};
                        string[] row;
                        bool isHeader = true;
                        while (!string.IsNullOrWhiteSpace(line = streamReader.ReadLine()))
                        {
                            if(isHeader)
                            {
                                propertyNames = line.Split(',');
                                isHeader = false;
                            }
                            else
                            {
                                row = line.Split(',');

                                if(row.Length == propertyNames.Length)
                                {
                                    var instance = (T)Activator.CreateInstance(typeof(T));

                                    for(int i = 0, j = row.Length; i < j; i++)
                                    {
                                        PropertyInfo propertyInfo = instance.GetType().GetProperty(propertyNames[i]);
                                        propertyInfo.SetValue(instance, Convert.ChangeType(row[i], propertyInfo.PropertyType), null);
                                    }

                                    parsedData.Add(instance);
                                }
                                else
                                {
                                    Logger.Error("CSV data invalid. File: " + fileName);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to parse CSV file:" + fileName);
                    Logger.Error(ex.Message);
                }
            }

            return parsedData;
        }

        private static string GetDataFilePath(string fileName)
        {
            if(!fileName.EndsWith(".csv"))
            {
                fileName += ".csv";
            }

            // Directly in VS Code
            string path1 = Path.GetFullPath(Constants.DATA_FOLDER + Path.DirectorySeparatorChar + fileName);
            // Debug bin
            string path2 = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\" + Constants.DATA_FOLDER + Path.DirectorySeparatorChar + fileName);
            // Exe
            string path3 = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\..\\" + Constants.DATA_FOLDER + Path.DirectorySeparatorChar + fileName);

            if(File.Exists(path1))
            {
                return path1;
            }
            else if(File.Exists(path2))
            {
                return path2;
            }
            else if(File.Exists(path3))
            {
                return path3;
            }
            else
            {
                Logger.Error("Failed to find data diretory path");
                return null;
            }
        }
    }
}
