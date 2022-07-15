using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveDronePicture
{
    public class Item
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double W { get; set; }
        public double H { get; set; }
    }

    public class ExpanderItem : Item
    {
        public string Header { get; set; }
    }

    public class LabelItem : Item
    {
        public string Content { get; set; }
    }
}
