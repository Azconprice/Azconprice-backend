using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface INumericExtractor
    {
        List<(double Number, string Unit)> Extract(string input);
    }
}
