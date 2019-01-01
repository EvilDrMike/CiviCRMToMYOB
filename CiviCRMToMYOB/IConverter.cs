using System.Collections.Generic;

namespace CiviCRMToMYOB
{
    public interface IConverter
    {
        List<List<MyobTransaction>> Convert(List<CiviCrmFormat> transactions,
            List<CiviCrmFormat> gstTransactions);
    }
}