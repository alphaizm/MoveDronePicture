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
using System.Collections.ObjectModel;

namespace MoveDronePicture
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const string JSON_FILE = "Setting.json";
        cJsonBase _ObjJson;
        cCtrlPicData _ObjCtrlPicData;
        ObservableCollection<Item> _ObjItems { get; set; }

        public MainWindow() {
            InitializeComponent();

            _ObjCtrlPicData = new cCtrlPicData();
            m_lstVw_FilesSrc.DataContext = _ObjCtrlPicData.PicData;

            _ObjItems = new ObservableCollection<Item>();
            m_itemCtrl.DataContext = _ObjItems;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            if (File.Exists(JSON_FILE)) {
                try {
                    string str_json = File.ReadAllText(JSON_FILE);
                    _ObjJson = JsonSerializer.Deserialize<cJsonBase>(str_json);

                    m_txtBox_DirSrc.Text = _ObjJson.DirSrc;
                    m_txtBox_DirDst.Text = _ObjJson.DirDst;

                    if (Directory.Exists(m_txtBox_DirSrc.Text)) {
                        m_btn_ReadSrc.IsEnabled = true;
                    }

                    for ( int blk_idx = 0; blk_idx < _ObjJson.LstBlocks.Count; blk_idx++ ) {
                        var block = _ObjJson.LstBlocks[blk_idx];

                        var gbi = new GroupBoxItem() {
                            X = 0,
                            Y = (100 * blk_idx),
                            W = 230,
                            H = 100,
                            Header = block.Name,
                        };

                        gbi.TabItems = new ObservableCollection<TabItemData>();

                        //-----------------------------------------------------
                        //  フォルダー
                        var tab_folders = new ObservableCollection<TabContentsData>();
                        var folders = block.DicFolders;
                        foreach(KeyValuePair<string, double> folder in folders) {
                            string content = folder.Key + " ： " + folder.Value.ToString();
                            tab_folders.Add(new TabContentsData() { TabContent = content });
                        }

                        gbi.TabItems.Add(new TabItemData() { TabHeader = "フォルダー", TabContents = tab_folders });

                        //-----------------------------------------------------
                        //  ポイント
                        var tab_points = new ObservableCollection<TabContentsData>();
                        var points = block.LstPoints;
                        foreach (var point in points) {
                            string content = point.Lat.ToString() + "／" + point.Lon.ToString();
                            tab_points.Add(new TabContentsData() { TabContent = content });
                        }

                        gbi.TabItems.Add(new TabItemData() { TabHeader = "登録座標", TabContents = tab_points });

                        _ObjItems.Add(gbi);
                    }

                    
                }
                catch {
                    //  nop
                }
            }
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
#else
            if(null == _ObjJson) {
                _ObjJson = new cJsonBase();
            }
            _ObjJson.DirSrc = m_txtBox_DirSrc.Text;
            _ObjJson.DirDst = m_txtBox_DirDst.Text;
#endif
            if (null != _ObjJson) {
                var opt = new JsonSerializerOptions {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                    WriteIndented = true
                };

                string str_json = JsonSerializer.Serialize<cJsonBase>(_ObjJson, opt);
                File.WriteAllText(JSON_FILE, str_json);
            }
        }

        private void m_btn_ReadSrc_Click(object sender, RoutedEventArgs e) {
            var ary_file = Directory.GetFiles(m_txtBox_DirSrc.Text, "*.JPG", SearchOption.AllDirectories);
            _ObjCtrlPicData.AddPicData(ary_file, m_progressBar_FilesSrc, m_lbl_ProgressBar);
        }
    }
}
