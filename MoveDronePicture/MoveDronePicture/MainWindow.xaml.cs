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
        cCtrlPicData _ObjCtrlPicData;

        public MainWindow() {
            InitializeComponent();

            _ObjCtrlPicData = new cCtrlPicData();
            m_lstVw_FilesSrc.DataContext = _ObjCtrlPicData.PicData;
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
            m_progressBar_FilesSrc.Minimum = 0;
            m_progressBar_FilesSrc.Value = 0;
            m_progressBar_FilesSrc.Maximum = ary_file.Length;
            _ObjCtrlPicData.AddPicData(ary_file, m_progressBar_FilesSrc, m_lbl_ProgressBar);
        }
    }
}
