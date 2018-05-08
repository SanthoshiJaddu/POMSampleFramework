using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.Smart.EnumsAndInterfaces
{
    public interface PageAccessInterfaces
    {
        bool FindElement();
        string GetWindowTitle();
        bool SelectOptionFromDropdown();
    }
}

