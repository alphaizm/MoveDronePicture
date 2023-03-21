// INotifyPropertyChanged notifCies the View of property changes, so that Bindings are updated.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MoveDronePicture
{
	public sealed class cCtrlGcpData
	{
		public ObservableCollection<cGcpItem> _GcpItem { get; }

		public cCtrlGcpData() {
			_GcpItem = new ObservableCollection<cGcpItem>();
			// 複数スレッドからコレクション操作できるようにする
			BindingOperations.EnableCollectionSynchronization(_GcpItem, new object());
		}
	}

	public sealed class cGcpItem : INotifyPropertyChanged
	{
		private string _Name;
		private double _Lat;
		private double _Lon;
		private double _Height;
		private short _ImgX;
		private short _ImgY;

		public cGcpItem() {
			// 入力値はデバッグ値
			_Name = "Test";
			_Lat = 1.0;
			_Lon = 2.0;
			_Height = 3.0;
			_ImgX = 5;
			_ImgY = 6;
		}

		public cGcpItem(string name_, double lat_, double lon_, double x_, double y_) {
			_Name = name_;
			_Lat = lat_;
			_Lon = lon_;
			_Height = 0;
			_ImgX = (short)Math.Round(x_, 1);
			_ImgY = (short)Math.Round(y_, 1);
		}

		public string Name { get { return _Name; } }

		/// <summary>
		/// WGS84座標の緯度：latitude
		/// 例）140.85908710956599
		/// </summary>
		public double Lat { get { return _Lon; } }

		/// <summary>
		/// WGS84座標の経度：longitude
		/// 例）38.191641339096201
		/// </summary>
		public double Lon { get { return _Lon; } }

		/// <summary>
		/// WGS84座標の高さ
		/// 例）12.8
		/// </summary>
		public double Height { get { return _Height; } }

		/// <summary>
		/// UTM座標：X
		/// 例）487660.487
		/// </summary>
		public double GcpX { get { return _Lon; } }

		/// <summary>
		/// UTM座標：Y
		/// 例）4227087.754
		/// </summary>
		public double GcpY { get { return _Lat; } }

		/// <summary>
		/// UTM座標：H
		/// 例）12.800
		/// </summary>
		public double GcpH { get { return _Height; } }

		/// <summary>
		/// 選択画像のX座標
		/// </summary>
		public double ImgX { get { return _ImgX; } }

		/// <summary>
		/// 選択画像のY座標
		/// </summary>
		public double ImgY { get { return _ImgY; } }

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string property_name_) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name_));
		}
	}
}
