using System.Linq;
using CiviCRMToMYOB;
using Fclp;

namespace CiviCRMToMYOBConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var transactionFileName = "";
            var gstFileName = "";
            var outputFileName = "";
            var gstAccount = "";
            var nonGstAccount = "";
            var gstForCreditAccount = "";

            var parser = new FluentCommandLineParser();
            parser.Setup<string>('t').Callback(val => transactionFileName = val);
            parser.Setup<string>('a').Callback(val => gstFileName = val);
            parser.Setup<string>('o').Callback(val => outputFileName = val);
            parser.Setup<string>('g').Callback(val => gstAccount = val);
            parser.Setup<string>('n').Callback(val => nonGstAccount = val);
            parser.Setup<string>('c').Callback(val => gstForCreditAccount = val);
            parser.Parse(args);

            var civiFileReader = new CiviFileReader();
            var transactions = civiFileReader.Read(System.IO.File.ReadAllText(transactionFileName));
            var gsts = civiFileReader.Read(System.IO.File.ReadAllText(gstFileName));

            var outfile = new System.IO.StreamWriter(outputFileName);
            new TransactionWriter().WriteToString(outfile, (new Converter(gstAccount.Split(',').ToList(), nonGstAccount.Split(',').ToList(), gstForCreditAccount.Split(',').ToList())).Convert(transactions, gsts));
            outfile.Flush();
        }
    }
}
