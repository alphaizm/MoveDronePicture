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

namespace MoveDronePicture
{
	/// <summary>
	/// 写真ファイル通知用クラスの管理
	/// </summary>
	public sealed class cCtrlImgItem
	{
		public ObservableCollection<cImgItem> _ImgItem { get; }

		private ProgressBar _bar;
		private Label _label;
		private Button _btnRead;
		private Button _btnCopy;
		private Button _btnMove;
		private Button _btnCsv;

		private string _fileNameRep;

		public cCtrlImgItem() {
			_ImgItem = new ObservableCollection<cImgItem>();
			// 複数スレッドからコレクション操作できるようにする
			BindingOperations.EnableCollectionSynchronization(_ImgItem, new object());
		}

		public void SetProgressBar(ProgressBar bar_) {
			_bar = bar_;
		}
		public void SetLabel(Label label_) {
			_label = label_;
		}
		public void SetButton(Button btnRead_,Button btnCopy_, Button btnRMove_, Button btnCsv_) {
			_btnRead = btnRead_;
			_btnCopy = btnCopy_;
			_btnMove = btnRMove_;
			_btnCsv = btnCsv_;
		}

		public void SetFileNameRep(string fileNameRep_) {
			_fileNameRep = fileNameRep_;
		}

		public async void AddPicData(
										string[] ary_pathes_,
										string str_dst_copy_server_dir_,
										string str_dst_move_local_dir_,
										Dictionary<string, GoogleMap> dic_maps
									) {

			_ImgItem.Clear();

			// ボタン無効
			UpdateBtnState(false);

			int digit = InitBar(ary_pathes_.Length);

			//  画像読み取り
			for (int img_idx = 0; img_idx < ary_pathes_.Length; img_idx++) {
				//  プログレスバー、ラベル更新
				int cnt = img_idx + 1;
				_bar.Value = cnt;
				_label.Content = cnt.ToString("D" + digit) + " / " + ary_pathes_.Length.ToString("D");

				//  非同期処理で読み取り
				var path = ary_pathes_[img_idx];
				await Task.Run(() => _ImgItem.Add(new cImgItem(path, _fileNameRep)));

				foreach (KeyValuePair<string, GoogleMap> map in dic_maps) {
					bool chk = await map.Value.chkInsideArea(_ImgItem[img_idx].Lat, _ImgItem[img_idx].Lon);
					if (chk) {
						map.Value.addMarker(_ImgItem[img_idx].Lat, _ImgItem[img_idx].Lon, _ImgItem[img_idx].ImgName);
						string str_height = map.Value.getHeightFolder(_ImgItem[img_idx].ChkHeight);
						_ImgItem[img_idx].SetCopyServerPath(str_dst_copy_server_dir_, map.Value.getNasFolder(), str_height, map.Value.getTargetFileName());
						_ImgItem[img_idx].SetMoveLocalPath(str_dst_move_local_dir_, map.Key, str_height, map.Value.getTargetFileName());
						_ImgItem[img_idx].SetField(map.Key);
					}
				}
			}

			// ボタン有効
			UpdateBtnState(true);
		}

		public async void CopyPicData2Server() {
			// ボタン無効
			UpdateBtnState(false);

			int digit = InitBar(_ImgItem.Count);

			for (int img_idx = 0; img_idx < _ImgItem.Count; img_idx++) {
				//  プログレスバー、ラベル更新
				int cnt = img_idx + 1;
				_bar.Value = cnt;
				_label.Content = cnt.ToString("D" + digit) + " / " + _ImgItem.Count.ToString("D");

				var pic_data = _ImgItem[img_idx];
				string path = pic_data.CopyServerPath;
				if ("" != path) {
					Directory.GetParent(path).Create();
					await Task.Run(() => File.Copy(pic_data.ImgPath, path));
				}
			}

			// ボタン有効
			UpdateBtnState(true);
		}


		public async void CopyPicData2Local() {
			// ボタン無効
			UpdateBtnState(false);

			int digit = InitBar(_ImgItem.Count);

			for (int img_idx = 0; img_idx < _ImgItem.Count; img_idx++) {
				//  プログレスバー、ラベル更新
				int cnt = img_idx + 1;
				_bar.Value = cnt;
				_label.Content = cnt.ToString("D" + digit) + " / " + _ImgItem.Count.ToString("D");

				var pic_data = _ImgItem[img_idx];
				if ("" != pic_data.MoveLocalPath) {
					Directory.GetParent(pic_data.MoveLocalPath).Create();
					await Task.Run(() => File.Copy(pic_data.ImgPath, pic_data.MoveLocalPath));
				}
			}

			// ボタン有効
			UpdateBtnState(true);
		}

		private int Digit(int num_) {
			return (0 == num_) ? 1 : ((int)Math.Log10(num_) + 1);
		}

		private void UpdateBtnState(bool state_) {
			_btnRead.IsEnabled = state_;
			_btnCopy.IsEnabled = state_;
			_btnMove.IsEnabled = state_;
			_btnCsv.IsEnabled = state_;
		}

		private int InitBar(int maximum_) {
			_bar.Minimum = 0;
			_bar.Value = 0;
			_bar.Maximum = maximum_;

			return Digit(maximum_);
		}
	}

	/// <summary>
	/// 写真ファイル通知用クラス
	/// </summary>
	public sealed class cImgItem : INotifyPropertyChanged
	{
		private string _ImgPath;
		private string _CopyServerPath;
		private string _MoveLocalPath;
		private string _ImgName;
		private string _ImgDate;
		private double _Lat;
		private double _Lon;
		private double _Height;
		private string _Field;

		private string _StrYYYYMMDD;

		private string _fileNameRep;

		const int ID_LAT = 0x0002;
		const int ID_LON = 0x0004;
		const int ID_HEIGHT = 0x0006;
		const int ID_DATE = 0x9004;

		public cImgItem(string str_path_, string str_file_name_rep_) {
			_ImgPath = str_path_;
			_Lat = 0;
			_Lon = 0;
			_Height = 0;
			_CopyServerPath = "";
			_MoveLocalPath = "";
			_Field = "";

			_fileNameRep = str_file_name_rep_;

			_ImgName = Path.GetFileName(str_path_);
			using (Bitmap bmp = new Bitmap(str_path_)) {
				foreach (var prop in bmp.PropertyItems) {
					switch (prop.Id) {
						case ID_DATE:
							_ImgDate = Encoding.UTF8.GetString(prop.Value);

							string str_year = _ImgDate.Substring(0, 4);
							string str_month = _ImgDate.Substring(5, 2);
							string str_day = _ImgDate.Substring(8, 2);
							_StrYYYYMMDD = str_year + str_month + str_day;

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

		/// <summary>
		/// 指定DIR\圃場\YYYYMMDD\高さ
		///		例）指定DIR\原\20230131\オルソ_20m
		/// </summary>
		/// <param name="str_dir_path_"></param>
		/// <param name="str_copy_folder_"></param>
		/// <param name="str_height_folder_"></param>
		public void SetCopyServerPath(string str_dir_path_, string str_copy_folder_, string str_height_folder_, string str_target_name_) {
			List<string> lst_str = new List<string>() {
				str_dir_path_,
				str_copy_folder_,
				_StrYYYYMMDD,
				str_height_folder_,
				_ImgName.Replace(_fileNameRep, _StrYYYYMMDD + "_" + str_target_name_)
			};

			_CopyServerPath = Path.Combine(lst_str.ToArray());
		}

		/// <summary>
		/// 指定DIR\YYYYMMDD\圃場\高さ
		///		例）指定DIR\20230131\原\オルソ_20m
		/// </summary>
		/// <param name="str_dir_path_"></param>
		/// <param name="str_move_folder_"></param>
		/// <param name="str_height_folder_"></param>
		public void SetMoveLocalPath(string str_dir_path_, string str_move_folder_, string str_height_folder_, string str_target_name_) {
			List<string> lst_str = new List<string>() {
				str_dir_path_,
				_StrYYYYMMDD,
				str_move_folder_,
				str_height_folder_,
				_ImgName.Replace(_fileNameRep, _StrYYYYMMDD + "_" + str_target_name_)
			};


			_MoveLocalPath = Path.Combine(lst_str.ToArray());
		}

		public void SetField(string str_field_) {
			_Field = str_field_;
		}

		public string ImgPath {
			get { return _ImgPath; }
		}

		public string ImgName {
			get { return _ImgName; }
		}

		public string ImgDate {
			get { return _ImgDate; }
		}

		public string YYYYMMDD {
			get { return _StrYYYYMMDD; }
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

		public string Field {
			get { return _Field; }
		}

		public double ChkHeight {
			get { return _Height; }
		}

		public string CopyServerPath {
			get { return _CopyServerPath; }
		}

		public string MoveLocalPath {
			get { return _MoveLocalPath; }
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
