using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper.Configuration;

namespace CiviCRMToMYOB
{
    public class CiviFileReader : ICiviFileReader
    {
        private Configuration _configuration = new Configuration
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            HeaderValidated = (opt1, opt2, opt3, opt4) => { },
            TrimOptions = TrimOptions.Trim
        };

        public List<CiviCrmFormat> Read(string text)
        {
            return ReadString(text, _configuration);
        }

        private static List<CiviCrmFormat> ReadString(string civiFormat, Configuration configuration)
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
    }
}