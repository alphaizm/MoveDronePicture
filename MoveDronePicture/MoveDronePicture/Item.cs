using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public bool IsExpanded { get; set; }
    }

    public class GroupBoxItem : Item
    {
        public string BtnContent { get; set; }

        public ObservableCollection<TabItemData> TabItems { get; set; }
    }

    public class TabItemData
    {
        public string TabHeader { get; set; }

        public ObservableCollection<TabContentsData> TabContents { get; set; }
    }

    public class TabContentsData
    {
        public string TabContent { get; set; }
    }

    public class LabelItem : Item
    {
        public string Content { get; set; }
    }
}
