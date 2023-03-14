﻿using System;
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
using System.Windows.Shapes;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;


namespace MoveDronePicture
{
	/// <summary>
	/// GcpEditor.xaml の相互作用ロジック
	/// </summary>
	public partial class GcpEditor : System.Windows.Window
	{
		public delegate void Callback(string str_target_key_);
		private Callback _callback;
		private double _min_height = 0;
		private double _max_height = 0;
		public GcpEditor() {
			InitializeComponent();
		}

		public GcpEditor(string file_path_, cBlock blk_target_, Callback callback_) {
			InitializeComponent();
			_callback = callback_;
			m_lstVw_GcpPoint.ItemsSource = blk_target_.LstGcpPoints;

			using (Mat png = new Mat(file_path_)) {
				m_img_png.Source = WriteableBitmapConverter.ToWriteableBitmap(png);
				
			}

			this.Title = "GcpEditor：" + System.IO.Path.GetFileName(file_path_);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			_min_height = m_cnvs.Height * 0.8;
			_max_height = m_cnvs.Height * 20.0;
		}

		private void m_scrlVw_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			e.Handled = true;              // 後続のイベントを実行しないための処理

			double scale;
			if (0 < e.Delta) {
				scale = 0.9;  // 10%の倍率で縮小
			}
			else {
				scale = 1.1;  // 10%の倍率で拡大
			}

			double height = m_img_png.ActualHeight * scale;

			// 指定縮尺時のみ有効
			if ((_min_height <= height) && (height <= _max_height)) {
				// 画像の拡大縮小
				m_img_png.Height = m_img_png.ActualHeight * scale;
				m_img_png.Width = m_img_png.ActualWidth * scale;

				// マウス位置が中心になるようにスクロールバーの位置を調整
				System.Windows.Point mouse_point = e.GetPosition(m_scrlVw);
				double x_bar_offset = (m_scrlVw.HorizontalOffset + mouse_point.X) * scale - mouse_point.X;
				m_scrlVw.ScrollToHorizontalOffset(x_bar_offset);

				double y_bar_offset = (m_scrlVw.VerticalOffset + mouse_point.Y) * scale - mouse_point.Y;
				m_scrlVw.ScrollToVerticalOffset(y_bar_offset);
			}
		}

		private void m_img_png_SizeChanged(object sender, SizeChangedEventArgs e) {
			m_cnvs.Height = e.NewSize.Height;
			m_cnvs.Width = e.NewSize.Width;
		}
	}
}
