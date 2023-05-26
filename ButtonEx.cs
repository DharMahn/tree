using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trees
{
    public class ButtonEx : Button
    {
        public ButtonEx()
        {
            SetStyle(ControlStyles.Selectable, false);
        }
    }
}
