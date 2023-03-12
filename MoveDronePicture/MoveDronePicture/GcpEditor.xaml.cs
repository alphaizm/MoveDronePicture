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
	}
}
