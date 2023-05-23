// INotifyPropertyChanged notifCies the View of property changes, so that Bindings are updated.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace MoveDronePicture
{
	public sealed class cCtrlGcpItem
	{
		public ObservableCollection<cGcpItem> _GcpItem { get; }

		public cCtrlGcpItem() {
			_GcpItem = new ObservableCollection<cGcpItem>();
			// 複数スレッドからコレクション操作できるようにする
			BindingOperations.EnableCollectionSynchronization(_GcpItem, new object());
		}

		public bool ContainName(string name_) {
			foreach(var item in _GcpItem) {
				if (item.Name == name_) {
					return true;
				}
			}

			return false;
		}
	}

	public sealed class cGcpItem : INotifyPropertyChanged
	{
		private string _PointName;
		private double _Lat;
		private double _Lon;
		private double _Height;
		private int _ImgX;
		private int _ImgY;

		public cGcpItem() {
			// 入力値はデバッグ値
			_PointName = "Test";
			_Lat = 1.0;
			_Lon = 2.0;
			_Height = 3.0;
			_ImgX = 5;
			_ImgY = 6;
		}

		public cGcpItem(string name_, double lat_, double lon_, int x_, int y_) {
			_PointName = name_;
			_Lat = lat_;
			_Lon = lon_;
			_Height = 0;
			_ImgX = x_;
			_ImgY = y_;
		}

		public string Name { get { return _PointName; } }

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
		/// 選択画像のX座標
		/// </summary>
		public int ImgX { get { return _ImgX; } }

		/// <summary>
		/// 選択画像のY座標
		/// </summary>
		public int ImgY { get { return _ImgY; } }

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string property_name_) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name_));
		}
	}

	public class cGcpRegistered
	{
		private string _ImgName;
		private double _Lat;
		private double _Lon;
		private double _Height;
		private int _ImgX;
		private int _ImgY;

		public cGcpRegistered() {
			// 入力値はデバッグ値
			_ImgName = "Test";
			_Lat = 1.0;
			_Lon = 2.0;
			_Height = 3.0;
			_ImgX = 5;
			_ImgY = 6;
		}

		public cGcpRegistered(string name_, double lat_, double lon_, double hgt_, int x_, int y_) {
			_ImgName = name_;
			_Lat = lat_;
			_Lon = lon_;
			_Height = hgt_;
			_ImgX = x_;
			_ImgY = y_;
		}

		public string ImgName { get { return _ImgName; } }

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
		public double GcpZ { get { return _Height; } }

		/// <summary>
		/// 選択画像のX座標
		/// </summary>
		public int ImgX { get { return _ImgX; } }

		/// <summary>
		/// 選択画像のY座標
		/// </summary>
		public int ImgY { get { return _ImgY; } }
	}
}
