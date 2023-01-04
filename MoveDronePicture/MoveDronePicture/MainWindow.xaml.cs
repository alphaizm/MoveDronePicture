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
		Dictionary<string, GoogleMap> _DicGoogleMap = new Dictionary<string, GoogleMap>();

		public MainWindow() {
			InitializeComponent();

			_ObjCtrlPicData = new cCtrlPicData();
			m_lstVw_FilesSrc.DataContext = _ObjCtrlPicData.PicData;

			_ObjItems = new ObservableCollection<Item>();
			m_itemCtrl.DataContext = _ObjItems;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			//+++++++++++++++++++++++++++++++++++++++++++++
			// ファイルなし
			if (!File.Exists(JSON_FILE)) { return; }

			try {
				string str_json = File.ReadAllText(JSON_FILE);
				_ObjJson = JsonSerializer.Deserialize<cJsonBase>(str_json);

				m_txtBox_DirSrc.Text = _ObjJson.DirSrc;
				m_txtBox_DirDst.Text = _ObjJson.DirDst;

				if (Directory.Exists(m_txtBox_DirSrc.Text)) {
					m_btn_ReadSrc.IsEnabled = true;
				}

				for (int blk_idx = 0; blk_idx < _ObjJson.LstBlocks.Count; blk_idx++) {
					var block = _ObjJson.LstBlocks[blk_idx];

					var gbi = new GroupBoxItem() {
						X = 0,
						Y = (105 * blk_idx) + 5,
						W = 230,
						H = 100,
						BtnContent = block.Name,
					};

					gbi.TabItems = new ObservableCollection<TabItemData>();

					//-----------------------------------------------------
					//  フォルダー
					var tab_folders = new ObservableCollection<TabContentsData>();
					var folders = block.DicFolders;
					foreach (KeyValuePair<string, double> folder in folders) {
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

					//-----------------------------------------------------
					//  中央
					var tab_center = new ObservableCollection<TabContentsData>();
					var center = block.Center;
					string str_content = center.Lat.ToString() + "／" + center.Lon.ToString();
					tab_center.Add(new TabContentsData() { TabContent = str_content });

					gbi.TabItems.Add(new TabItemData() { TabHeader = "中心点", TabContents = tab_center });

					_ObjItems.Add(gbi);
				}
			}
			catch {
				//  nop
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
                                                    },
													new cPoint(36.37466547010662, 136.54670838845954),
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
                                                    },
													new cPoint(36.37466547010662, 136.54670838845954),
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
                                                    },
													new cPoint(36.37466547010662, 136.54670838845954),
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
                                                    },
													new cPoint(36.37466547010662, 136.54670838845954),
                                                )
                                            }
                                        );
#else
			if (null == _ObjJson) {
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

			// GoogleMap消去
			var maps = _DicGoogleMap.Values.ToArray();
			for (int idx = 0; idx < maps.Length; idx++) {
				maps[idx].Close();
			}
		}

		private void m_btn_ReadSrc_Click(object sender, RoutedEventArgs e) {
			var ary_file = Directory.GetFiles(m_txtBox_DirSrc.Text, "*.JPG", SearchOption.AllDirectories);
			_ObjCtrlPicData.AddPicData(
											ary_file,
											m_progressBar_FilesSrc,
											m_lbl_ProgressBar,
											m_btn_OutputCsv,
											_DicGoogleMap
									);
		}

		private void m_btn_OutputCsv_Click(object sender, RoutedEventArgs e) {
			StringBuilder str_output = new StringBuilder();
			str_output.Append("latitude");
			str_output.Append(",");
			str_output.Append("longitude");
			str_output.Append(",");
			str_output.Append("altitude");
			str_output.AppendLine();

			for (int idx = 0; idx < _ObjCtrlPicData.PicData.Count; idx++) {
				var data = _ObjCtrlPicData.PicData[idx];
				str_output.Append(data.Lat);
				str_output.Append(",");
				str_output.Append(data.Lon);
				str_output.Append(",");
				str_output.Append(data.Height);
				str_output.AppendLine();
			}

			string str_file = System.IO.Path.Combine(m_txtBox_DirDst.Text, "_drone_latlon.csv");
			File.WriteAllText(str_file, str_output.ToString());
		}

		private void GrpBxHdBtn_Click(object sender, RoutedEventArgs e) {
			var btn = (Button)sender;

			// 押下グループボックスの登録ブロック取得
			cBlock blk_target = null;
			for (int blk_idx = 0; blk_idx < _ObjJson.LstBlocks.Count; blk_idx++) {
				cBlock blk_search = _ObjJson.LstBlocks[blk_idx];
				if (btn.Content.ToString() == blk_search.Name) {
					blk_target = blk_search;
					break;
				}
			}

			// 表示なしの場合 -> 表示
			if (!_DicGoogleMap.ContainsKey(blk_target.Name)) {
				var page = new GoogleMap(blk_target, DelegateGoogleMapClosing);
				page.Show();

				_DicGoogleMap.Add(blk_target.Name, page);
			}
		}

		private void DelegateGoogleMapClosing(string str_target_key_) {
			if (_DicGoogleMap.ContainsKey(str_target_key_)) {
				_DicGoogleMap.Remove(str_target_key_);
			}
		}
	}
}
