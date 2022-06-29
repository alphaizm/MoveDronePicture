#if DEBUG
    //#define OUBPUT_JSON
#endif

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
using System.IO;

namespace MoveDronePicture
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const string JSON_FILE = "Setting.json";
        cJsonBase _ObjJson;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

#if OUBPUT_JSON
            var _ObjJson = new cJsonBase(
                                            @"E:\DCIM",
                                            @"C:\_nakada\agri\2022\drone",
                                            new List<cBlock>() {
                                                new cBlock(
                                                    "麦口(林)",
                                                    new Dictionary<string, double>() {
                                                        {"雑草診断_10m",10.00 },
                                                        {"オルソ_20m",20.00 }
                                                    },
                                                    new List<cPoint> {
                                                        new cPoint(36.374585508, 136.546502806),
                                                        new cPoint(36.374818851, 136.546537629),
                                                    }
                                                ),
                                                new cBlock(
                                                    "麦口(中村)",
                                                    new Dictionary<string, double>() {
                                                        {"雑草診断_10m",10.00 },
                                                        {"オルソ_20m",20.00 }
                                                    },
                                                    new List<cPoint> {
                                                        new cPoint(36.374585508, 136.546502806),
                                                        new cPoint(36.374818851, 136.546537629),
                                                    }
                                                ),
                                                new cBlock(
                                                    "麦口(中田)",
                                                    new Dictionary<string, double>() {
                                                        {"雑草診断_10m",10.00 },
                                                        {"オルソ_20m",20.00 }
                                                    },
                                                    new List<cPoint> {
                                                        new cPoint(36.374585508, 136.546502806),
                                                        new cPoint(36.374818851, 136.546537629),
                                                    }
                                                ),
                                                new cBlock(
                                                    "原",
                                                    new Dictionary<string, double>() {
                                                        {"雑草診断_10m",10.00 },
                                                        {"オルソ_20m",20.00 }
                                                    },
                                                    new List<cPoint> {
                                                        new cPoint(36.374585508, 136.546502806),
                                                        new cPoint(36.374818851, 136.546537629),
                                                    }
                                                )
                                            }
                                        );
#endif

            var opt = new JsonSerializerOptions {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                WriteIndented = true
            };

            string str_json = JsonSerializer.Serialize<cJsonBase>(_ObjJson, opt);
            File.WriteAllText(JSON_FILE, str_json);
        }
    }

    public class cJsonBase
    {
        public string SrcDir { get; set; }
        public string DstDir { get; set; }
        public List<cBlock> LstBlocks { get; set; }

        public cJsonBase() {
            SrcDir = "";
            DstDir = "";
            LstBlocks = new List<cBlock>();
        }

        public cJsonBase(string str_src_dir_, string str_dst_dir, List<cBlock> lst_blocks_) {
            SrcDir = str_src_dir_;
            DstDir = str_dst_dir;
            LstBlocks = lst_blocks_;
        }

    }

    public class cBlock
    {
        public string Name { get; set; }
        public Dictionary<string, double> DicFolders { get; set; }
        public List<cPoint> LstPoints { get; set; }

        public cBlock() {
            Name = "";
            DicFolders = new Dictionary<string, double>();
            LstPoints = new List<cPoint>();
        }

        public cBlock(string str_name_, Dictionary<string, double> dic_folders_, List<cPoint> lst_points_) {
            Name = str_name_;
            DicFolders = dic_folders_;
            LstPoints = lst_points_;
        }

    }

    public class cPoint
    {
        public double Lat { get; set; }
        public double Lon { get; set; }

       public cPoint() {
            Lat = 0;
            Lon = 0;
        }

        public cPoint(double lat_, double lon_) {
            Lat = lat_;
            Lon = lon_;
        }
    }
}
