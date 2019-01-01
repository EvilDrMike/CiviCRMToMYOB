using System.Collections.Generic;

namespace CiviCRMToMYOB
{
    public interface ICiviFileReader
    {
        List<CiviCrmFormat> Read(string text);
    }
}