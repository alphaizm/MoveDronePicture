// INotifyPropertyChanged notifCies the View of property changes, so that Bindings are updated.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace MoveDronePicture
{
	/// <summary>
	/// 写真ファイル通知用クラスの管理
	/// </summary>
	public sealed class cCtrlPicData
	{
		public ObservableCollection<cPicData> _PicData { get; }

		private ProgressBar _bar;
		private Label _label;
		private Button _btnRead;
		private Button _btnMove;
		private Button _btnCsv;

		public cCtrlPicData() {
			_PicData = new ObservableCollection<cPicData>();
			// 複数スレッドからコレクション操作できるようにする
			BindingOperations.EnableCollectionSynchronization(_PicData, new object());
		}

		public void SetProgressBar(ProgressBar bar_) {
			_bar = bar_;
		}
		public void SetLabel(Label label_) {
			_label = label_;
		}
		public void SetButton(Button btnRead_, Button btnRMove_, Button btnCsv_) {
			_btnRead = btnRead_;
			_btnMove = btnRMove_;
			_btnCsv = btnCsv_;
		}

		public async void AddPicData(
										string[] ary_pathes_,
										string str_dst_dir_,
										Dictionary<string, GoogleMap> dic_maps
									) {

			_PicData.Clear();

			// ボタン無効
			_btnRead.IsEnabled = false;
			_btnMove.IsEnabled = false;
			_btnCsv.IsEnabled = false;

			_bar.Minimum = 0;
			_bar.Value = 0;
			_bar.Maximum = ary_pathes_.Length;

			int digit = Digit(ary_pathes_.Length);

			//  画像読み取り
			for (int idx = 0; idx < ary_pathes_.Length; idx++) {
				//  プログレスバー、ラベル更新
				int cnt = idx + 1;
				_bar.Value = cnt;
				_label.Content = cnt.ToString("D" + digit) + " / " + ary_pathes_.Length.ToString("D");

				//  非同期処理で読み取り
				var path = ary_pathes_[idx];
				await Task.Run(() => _PicData.Add(new cPicData(path)));

				foreach (KeyValuePair<string, GoogleMap> map in dic_maps) {
					bool chk = await map.Value.chkInsideArea(_PicData[idx].Lat, _PicData[idx].Lon);
					if (chk) {
						map.Value.addMarker(_PicData[idx].Lat, _PicData[idx].Lon);
						_PicData[idx].SetCopyPath(str_dst_dir_, map.Key);
					}
				}
			}

			// ボタン有効
			_btnRead.IsEnabled = true;
			_btnMove.IsEnabled = true;
			_btnCsv.IsEnabled = true;
		}

		private int Digit(int num_) {
			return (0 == num_) ? 1 : ((int)Math.Log10(num_) + 1);
		}
	}

	/// <summary>
	/// 写真ファイル通知用クラス
	/// </summary>
	public sealed class cPicData : INotifyPropertyChanged
	{
		private string _ImgPath;
		private string _CopyPath;
		private string _ImgName;
		private string _ImgDate;
		private double _Lat;
		private double _Lon;
		private double _Height;

		const int ID_LAT = 0x0002;
		const int ID_LON = 0x0004;
		const int ID_HEIGHT = 0x0006;
		const int ID_DATE = 0x9004;

		public cPicData(string str_path_) {
			_ImgPath = str_path_;
			_Lat = 0;
			_Lon = 0;
			_Height = 0;
			_CopyPath = "";

			_ImgName = Path.GetFileName(str_path_);
			using (Bitmap bmp = new Bitmap(str_path_)) {
				foreach (var prop in bmp.PropertyItems) {
					switch (prop.Id) {
						case ID_DATE:
							_ImgDate = Encoding.UTF8.GetString(prop.Value);
							break;
						case ID_LAT:
							_Lat = GetDecLatLon(prop.Value);
							break;
						case ID_LON:
							_Lon = GetDecLatLon(prop.Value);
							break;
						case ID_HEIGHT:
							_Height = BitConverter.ToUInt32(prop.Value, 0);
							break;
						default:
							break;
					}
				}
			}
		}

		public void SetCopyPath(string str_dir_path_, string str_copy_folder_) {
			string str_year = ImgDate.Substring(0, 4);
			string str_month = ImgDate.Substring(5, 2);
			string str_day = ImgDate.Substring(8, 2);

			_CopyPath = Path.Combine(str_dir_path_, str_copy_folder_, str_year + str_month + str_day);
		}

		public string ImgName {
			get { return _ImgName; }
		}

		public string ImgDate {
			get { return _ImgDate; }
		}

		public string Lat {
			get { return _Lat.ToString(); }
		}

		public string Lon {
			get { return _Lon.ToString(); }
		}

		public string Height {
			get { return _Height.ToString(); }
		}

		public string CopyPath {
			get { return _CopyPath; }
		}

		private double GetDecLatLon(byte[] ary_value_) {
			uint deg_numerator = BitConverter.ToUInt32(ary_value_, 0);
			uint deg_denominator = BitConverter.ToUInt32(ary_value_, 4);

			double deg = 0;
			if (0 != deg_denominator) {
				deg = (double)deg_numerator / (double)deg_denominator;
			}

			uint min_numerator = BitConverter.ToUInt32(ary_value_, 8);
			uint min_denominator = BitConverter.ToUInt32(ary_value_, 12);

			double min = 0;
			if (0 != min_denominator) {
				min = (double)min_numerator / (double)min_denominator;
			}

			uint sec_numerator = BitConverter.ToUInt32(ary_value_, 16);
			uint sec_denominator = BitConverter.ToUInt32(ary_value_, 20);

			double sec = 0;
			if (0 != sec_denominator) {
				sec = (double)sec_numerator / (double)sec_denominator;
			}

			double ret = 0;
			ret += sec;
			ret /= 60;
			ret += min;
			ret /= 60;
			ret += deg;

			return ret;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string property_name_) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name_));
		}
	}
}
