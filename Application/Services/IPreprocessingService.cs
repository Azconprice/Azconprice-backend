using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IPreprocessingService
    {
        string Canon(string input);
        string? ExtractMaterial(string input);
        List<string> Tokenize(string canonText);
    }

}
