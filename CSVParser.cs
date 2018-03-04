using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Entities;

namespace MarchMadness2018
{
    public static class CSVParser
    {
        private const string DATA_FOLDER = "data\\";
        public static IEnumerable<T> ParseCsvFile<T>(string fileName) {

            fileName = DATA_FOLDER + fileName;

            if(!fileName.EndsWith(".csv"))
            {
                fileName += ".csv";
            }

            var parsedData = new List<T>();

            try
            {
                using (var streamReader = new StreamReader(Path.GetFullPath(fileName)))
                {
                    string line;
                    string[] propertyNames = new string[]{};
                    string[] row;
                    bool isHeader = true;
                    while ((line = streamReader.ReadLine()) != null)
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

            return parsedData;
        }
    }
}
