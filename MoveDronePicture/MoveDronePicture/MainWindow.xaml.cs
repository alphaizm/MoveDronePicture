using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;

namespace MoveDronePicture
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const string _JsonFile = "Setting.json";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cJsonBase json = new cJsonBase() {
                SrcDir = @"E:\DCIM",
                DstDir = @"C:\_nakada\agri\2022\drone",
                Blocks = new List<cBlock>() {
                    new cBlock(
                        Name = "麦口(林)",
                        Folders = new List<string>() {
                            "オルソ_20m",
                            "雑草診断_10m"
                        },
                        Points = new List<cPoint>() {
                            new cPoint(){Lat = (long)36.374585508, Lon =(long)136.546502806},
                            new cPoint(){Lat = (long)36.374818851, Lon=(long)136.546537629}
                        }
                        )
                }

            };
        }
    }

    public class cJsonBase
    {
        public string SrcDir;
        public string DstDir;
        public List<cBlock> Blocks;

    }

    public class cBlock
    {
        public string Name;
        public List<string> Folders;
        public List<cPoint> Points;

    }

    public class cPoint
    {
        public long Lat;
        public long Lon;
    }
}
