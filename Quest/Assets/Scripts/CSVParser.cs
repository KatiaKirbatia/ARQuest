using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


    public static class CSVParser
    {
        /// <summary>
        /// Load CSV data from specified path.
        /// </summary>
        /// <param name="path">CSV file path.</param>
        /// <param name="delimiter">Delimiter.</param>
        /// <param name="encoding">Type of text encoding. (default UTF-8)</param>
        /// <returns>Nested list that CSV parsed.</returns>
        public static List<List<string>> LoadFromPath(string path,  Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            var data = File.ReadAllText(path, encoding);
            return Parse(data);
        }

        /// <summary>
        /// Load CSV data asynchronously from specified path.
        /// </summary>
        /// <param name="path">CSV file path.</param>
        /// <param name="delimiter">Delimiter.</param>
        /// <param name="encoding">Type of text encoding. (default UTF-8)</param>
        /// <returns>Nested list that CSV parsed.</returns>
        public static async Task<List<List<string>>> LoadFromPathAsync(string path, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            using (var reader = new StreamReader(path, encoding))
            {
                var data = await reader.ReadToEndAsync();
                return Parse(data);
            }
        }

        /// <summary>
        /// Load CSV data from string.
        /// </summary>
        /// <param name="data">CSV string</param>
        /// <param name="delimiter">Delimiter.</param>
        /// <returns>Nested list that CSV parsed.</returns>
        public static List<List<string>> LoadFromString(string data)
        {
           
            return Parse(data);
        }

        static List<List<string>> Parse(string data)
        {
            ConvertToCrlf(ref data);

            var sheet = new List<List<string>>();
            var row = new List<string>();
            var cell = new StringBuilder();
            var insideQuoteCell = false;
            var start = 0;

            var delimiterSpan = ','.ToString().AsSpan();
            var crlfSpan = "\r\n".AsSpan();
            var oneDoubleQuotSpan = "\"".AsSpan();
            var twoDoubleQuotSpan = "\"\"".AsSpan();

            while (start < data.Length)
            {
                var length = start <= data.Length - 2 ? 2 : 1;
                var span = data.AsSpan(start, length);

                if (span.StartsWith(delimiterSpan))
                {
                    if (insideQuoteCell)
                    {
                        cell.Append(',');
                    }
                    else
                    {
                        AddCell(row, cell);
                    }

                    start += 1;
                }
                else if (span.StartsWith(crlfSpan))
                {
                    if (insideQuoteCell)
                    {
                        cell.Append("\r\n");
                    }
                    else
                    {
                        AddCell(row, cell);
                        AddRow(sheet, ref row);
                    }

                    start += 2;
                }
                else if (span.StartsWith(twoDoubleQuotSpan))
                {
                    cell.Append("\"");
                    start += 2;
                }
                else if (span.StartsWith(oneDoubleQuotSpan))
                {
                    insideQuoteCell = !insideQuoteCell;
                    start += 1;
                }
                else
                {
                    cell.Append(span[0]);
                    start += 1;
                }
            }

            if (row.Count > 0)
            {
                AddCell(row, cell);
                AddRow(sheet, ref row);
            }

            return sheet;
        }

        static void AddCell(List<string> row, StringBuilder cell)
        {
            row.Add(cell.ToString());
            cell.Length = 0; // Old C#.
        }

        static void AddRow(List<List<string>> sheet, ref List<string> row)
        {
            sheet.Add(row);
            row = new List<string>();
        }

        static void ConvertToCrlf(ref string data)
        {
            data = Regex.Replace(data, @"\r\n|\r|\n", "\r\n");
        }
    }
