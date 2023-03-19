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
		ObservableCollection<GuiItem> _ObjItems { get; set; }
		Dictionary<string, GoogleMap> _DicGoogleMap = new Dictionary<string, GoogleMap>();

		public MainWindow() {
			InitializeComponent();

			_ObjCtrlPicData = new cCtrlPicData();
			m_lstVw_FilesSrc.DataContext = _ObjCtrlPicData._PicData;

			_ObjItems = new ObservableCollection<GuiItem>();
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
				m_txtBox_DirDstServer.Text = _ObjJson.DirDstServer;
				m_txtBox_DirDstLocal.Text = _ObjJson.DirDstLocal;

				if (Directory.Exists(m_txtBox_DirSrc.Text)) {
					m_btn_ReadSrc.IsEnabled = true;
				}

				for (int blk_idx = 0; blk_idx < _ObjJson.LstBlocks.Count; blk_idx++) {
					var block = _ObjJson.LstBlocks[blk_idx];

					var gbi = new GroupBoxItem() {
						X = 5,
						Y = (125 * blk_idx),
						W = 250,
						H = 120,
						BtnContent = block.HeaderName,
					};

					gbi.TabItems = new ObservableCollection<TabItemData>();

					//-----------------------------------------------------
					//  フォルダー
					var tab_folders = new ObservableCollection<TabContentsData>();
					var folders = block.LstFolders;
					for(int fld_idx = 0; fld_idx < folders.Count; fld_idx++) {
						var folder = folders[fld_idx];
						StringBuilder content = new StringBuilder();
						content.Append(folder.Name);
						content.Append(" ： ");
						content.Append(folder.Height.ToString());
						content.Append(" ± ");
						content.Append(folder.Offset.ToString());
						tab_folders.Add(new TabContentsData() { TabContent = content.ToString() });
					}

					gbi.TabItems.Add(new TabItemData() { TabHeader = "フォルダー", TabContents = tab_folders });

					//-----------------------------------------------------
					//  領域ポリゴン用ポイント
					var tab_poly_points = new ObservableCollection<TabContentsData>();
					var poly_points = block.LstPolyPoints;
					foreach (var point in poly_points) {
						string content = point.Lat.ToString() + "／" + point.Lon.ToString();
						tab_poly_points.Add(new TabContentsData() { TabContent = content });
					}

					gbi.TabItems.Add(new TabItemData() { TabHeader = "領域座標", TabContents = tab_poly_points });

					//-----------------------------------------------------
					//  GCP用ポイント
					var tab_gcp_points = new ObservableCollection<TabContentsData>();
					var gcp_points = block.LstGcpPoints;
					foreach (cGcp gcp in gcp_points) {
						string content = gcp.Name + "：" +gcp.Lat.ToString() + "／" + gcp.Lon.ToString();
						tab_gcp_points.Add(new TabContentsData() { TabContent = content });
					}

					gbi.TabItems.Add(new TabItemData() { TabHeader = "GCP座標", TabContents = tab_gcp_points });

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
			_ObjJson.DirDstServer = m_txtBox_DirDstServer.Text;
			_ObjJson.DirDstLocal = m_txtBox_DirDstLocal.Text;
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

		private void m_btn_DirUpdate_Click(object sender, RoutedEventArgs e) {
			if (Directory.Exists(m_txtBox_DirSrc.Text)) {
				m_btn_ReadSrc.IsEnabled = true;
			}
			else {
				m_btn_ReadSrc.IsEnabled = false;
				MessageBox.Show("コピー元ディレクトリは存在していません。");
			}
		}

		private void m_btn_ReadSrc_Click(object sender, RoutedEventArgs e) {
			var ary_file = Directory.GetFiles(m_txtBox_DirSrc.Text, "*.JPG", SearchOption.AllDirectories);

			_ObjCtrlPicData.SetProgressBar(m_progressBar_FilesSrc);
			_ObjCtrlPicData.SetLabel(m_lbl_ProgressBar);
			_ObjCtrlPicData.SetButton(m_btn_ReadSrc, m_btn_CopyFile, m_btn_MoveFile, m_btn_OutputCsv);
			_ObjCtrlPicData.SetFileNameRep(_ObjJson.FileNameRep);
			_ObjCtrlPicData.AddPicData(
											ary_file,
											m_txtBox_DirDstServer.Text,
											m_txtBox_DirDstLocal.Text,
											_DicGoogleMap
									);
		}

		private void m_btn_CopyFile_Click(object sender, RoutedEventArgs e) {
			_ObjCtrlPicData.CopyPicData2Server();
		}

		private void m_btn_MoveFile_Click(object sender, RoutedEventArgs e) {
			_ObjCtrlPicData.CopyPicData2Local();
		}

		private void m_btn_OutputCsv_Click(object sender, RoutedEventArgs e) {
			// ドローン撮影画像の緯度経度データ出力
			outoutCsv_DroneData();

			// 中央点、各範囲点の緯度経度データ出力
			ouputCsv_JsonData();

			MessageBox.Show("CSVファイル出力完了");
		}

		private void outoutCsv_DroneData() {

			Dictionary<string, StringBuilder> dicCsv = new Dictionary<string, StringBuilder>();

			for (int idx = 0; idx < _ObjCtrlPicData._PicData.Count; idx++) {
				var data = _ObjCtrlPicData._PicData[idx];
				string date = data.YYYYMMDD;
				if (!dicCsv.ContainsKey(date)){
					dicCsv.Add(date, new StringBuilder());
					dicCsv[date].Append(getCsv_Header());
					dicCsv[date].Append(",");
					dicCsv[date].Append("altitude");
					dicCsv[date].AppendLine();
				}

				dicCsv[date].Append(data.ImgName);
				dicCsv[date].Append(",");
				dicCsv[date].Append(data.Lat);
				dicCsv[date].Append(",");
				dicCsv[date].Append(data.Lon);
				dicCsv[date].Append(",");
				dicCsv[date].Append(data.Height);
				dicCsv[date].AppendLine();
			}

			// 日付分でループ
			foreach (KeyValuePair<string, StringBuilder> pair in dicCsv) {
				string str_file = System.IO.Path.Combine(m_txtBox_DirDstLocal.Text, "_" + pair.Key + "_drone_latlon.csv");
				File.WriteAllText(str_file, pair.Value.ToString());
			}
		}

		private void ouputCsv_JsonData() {
			StringBuilder str_output_poly_points = new StringBuilder();
			StringBuilder str_output_gcp_points = new StringBuilder();
			StringBuilder str_output_center = new StringBuilder();

			str_output_poly_points.Append(getCsv_Header());
			str_output_poly_points.AppendLine();
			str_output_gcp_points.Append(getCsv_Header());
			str_output_gcp_points.AppendLine();
			str_output_center.Append(getCsv_Header());
			str_output_center.AppendLine();

			for (int blk_idx = 0; blk_idx < _ObjJson.LstBlocks.Count; blk_idx++) {
				var block = _ObjJson.LstBlocks[blk_idx];
				string str_title = block.HeaderName;

				// 中央点
				str_output_center.Append(str_title);
				str_output_center.Append(",");
				str_output_center.Append(block.Center.Lat.ToString());
				str_output_center.Append(",");
				str_output_center.Append(block.Center.Lon.ToString());
				str_output_center.AppendLine();

				// 領域エリア点
				for(int pnt_idx = 0; pnt_idx < block.LstPolyPoints.Count; pnt_idx++) {
					var point = block.LstPolyPoints[pnt_idx];
					str_output_poly_points.Append(str_title + "_" + pnt_idx.ToString());
					str_output_poly_points.Append(",");
					str_output_poly_points.Append(point.Lat.ToString());
					str_output_poly_points.Append(",");
					str_output_poly_points.Append(point.Lon.ToString());
					str_output_poly_points.AppendLine();
				}

				// GCP点
				for (int pnt_idx = 0; pnt_idx < block.LstGcpPoints.Count; pnt_idx++) {
					var gcp = block.LstGcpPoints[pnt_idx];
					str_output_gcp_points.Append(gcp.Name);
					str_output_gcp_points.Append(",");
					str_output_gcp_points.Append(gcp.Lat.ToString());
					str_output_gcp_points.Append(",");
					str_output_gcp_points.Append(gcp.Lon.ToString());
					str_output_gcp_points.AppendLine();
				}
			}

			string str_center_file = System.IO.Path.Combine(m_txtBox_DirDstLocal.Text, "_centers_latlon.csv");
			File.WriteAllText(str_center_file, str_output_center.ToString());

			string str_poly_point_file = System.IO.Path.Combine(m_txtBox_DirDstLocal.Text, "_poly_points_latlon.csv");
			File.WriteAllText(str_poly_point_file, str_output_poly_points.ToString());

			string str_gcp_point_file = System.IO.Path.Combine(m_txtBox_DirDstLocal.Text, "_gcp_points_latlon.csv");
			File.WriteAllText(str_gcp_point_file, str_output_gcp_points.ToString());
		}

		private string getCsv_Header() {
			StringBuilder str_output = new StringBuilder();
			str_output.Append("file name");
			str_output.Append(",");
			str_output.Append("latitude");
			str_output.Append(",");
			str_output.Append("longitude");

			return str_output.ToString();
		}

		private void GrpBxHdBtn_Click(object sender, RoutedEventArgs e) {
			var btn = (Button)sender;

			// 押下グループボックスの登録ブロック取得
			cBlock blk_target = null;
			for (int blk_idx = 0; blk_idx < _ObjJson.LstBlocks.Count; blk_idx++) {
				cBlock blk_search = _ObjJson.LstBlocks[blk_idx];
				if (btn.Content.ToString() == blk_search.HeaderName) {
					blk_target = blk_search;
					break;
				}
			}

			// 表示なしの場合 -> 表示
			if (!_DicGoogleMap.ContainsKey(blk_target.HeaderName)) {
				var page = new GoogleMap(blk_target, DelegateGoogleMapClosing);
				page.Show();

				_DicGoogleMap.Add(blk_target.HeaderName, page);
			}
		}
		

		private void DelegateGoogleMapClosing(string str_target_key_) {
			if (_DicGoogleMap.ContainsKey(str_target_key_)) {
				_DicGoogleMap.Remove(str_target_key_);
			}
		}

		private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			var lstvwiitem = sender as ListViewItem;
			var pic_data = lstvwiitem.Content as cPicData;

			//	ToDo：暫定でブロック３を指定、後でブロックを指定
			var page = new GcpEditor(pic_data.ImgPath, _ObjJson.LstBlocks[3], null);
			page.Show();
		}
	}
}
