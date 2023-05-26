using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trees
{
    public class CheckBoxEx : CheckBox
    {
        public CheckBoxEx()
        {
            SetStyle(ControlStyles.Selectable, false);
        }
    }
}
