using System.Collections.Generic;
using System.IO;

namespace CiviCRMToMYOB
{
    public interface ITransactionWriter
    {
        TextWriter WriteToString(TextWriter stream, List<List<MyobTransaction>> transactions);
    }
}