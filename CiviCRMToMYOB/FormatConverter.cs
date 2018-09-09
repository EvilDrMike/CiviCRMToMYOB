using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper.Configuration;

namespace CiviCRMToMYOB
{
    public class FormatConverter : IFormatConverter
    {
        public string Convert(string civiFormat)
        {
            var configuration = GetConfiguration();

            var list = ReadString(civiFormat, configuration);

            return WriteString(list);
        }

        private static string WriteString(IEnumerable<CiviCrmFormat> list)
        {
            var writer = new StringWriter();

            writer.WriteLine("{},,,,,,,,,");
            writer.WriteLine("Journal Number,Date,Memo,Account Number,Is Credit,Amount,Job,Allocation Memo,Category,Is Year-End Adjustment");

            var previousTransactionNumber = (string)null;
            foreach (var row in list)
            {
                if (string.IsNullOrEmpty(row.TransNumber))
                {
                    row.TransNumber = $"{row.ContactName} at {row.Date}";
                }

                if (previousTransactionNumber != null && string.Compare(row.TransNumber, previousTransactionNumber, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    writer.WriteLine(",,,,,,,,,");
                }

                writer.WriteLine(ToDebitLine(row));
                writer.WriteLine(ToCreditLine(row));

                previousTransactionNumber = row.TransNumber;
            }

            return writer.ToString();
        }

        private static string ToCreditLine(CiviCrmFormat item)
        {
            return $",{item.Date},\"{item.TransNumber}\",{item.FinancialAccountCodeCredit},Y,{item.Amount},,,,";
        }

        private static ReadOnlySpan<char> ToDebitLine(CiviCrmFormat item)
        {
            return $",{item.Date},\"{item.TransNumber}\",{item.FinancialAccountCodeDebit},N,{item.Amount},,,,";
        }

        private List<CiviCrmFormat> ReadString(string civiFormat, Configuration configuration)
        {
            using (var reader = new StringReader(civiFormat))
            {
                var csv = new CsvHelper.CsvReader(reader, configuration);
                csv.Configuration.RegisterClassMap(new CiviCrmFormatMap());
                csv.Configuration.MissingFieldFound = null;
                var records = csv.GetRecords<CiviCrmFormat>();
                return records.ToList();
            }
        }

        private static Configuration GetConfiguration()
        {
            var configuration = new Configuration
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                HeaderValidated = (opt1, opt2, opt3, opt4) => { },
                TrimOptions = TrimOptions.Trim
            };
            return configuration;
        }
    }
}
