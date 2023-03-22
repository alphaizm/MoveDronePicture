using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace MoveDronePicture
{
	/// <summary>
	/// GcpEditor.xaml の相互作用ロジック
	/// </summary>
	public partial class GcpEditor : System.Windows.Window
	{
		public delegate void Callback(string str_target_key_);
		private Callback _callback;
		private string _target_key;
		private double _minHeight = 0;
		private double _maxHeight = 0;
		private bool _isMouseDown = false;
		private System.Windows.Point _startPoint;
		private System.Windows.Point _crrntPoint;

		public cCtrlGcpItem _ObjCtrlGcpItem;

		public GcpEditor() {
			InitializeComponent();
		}

		public GcpEditor(string file_path_, cBlock blk_target_, Callback callback_) {
			InitializeComponent();
			_ObjCtrlGcpItem = new cCtrlGcpItem();
			_callback = callback_;
			_target_key = System.IO.Path.GetFileName(file_path_);
			m_lstVw_GcpPoint.ItemsSource = blk_target_.LstGcpPoints;
			m_lstVw_GcpList.DataContext = _ObjCtrlGcpItem._GcpItem;

			using (Mat png = new Mat(file_path_)) {
				m_img_png.Source = WriteableBitmapConverter.ToWriteableBitmap(png);
			}

			this.Title = "GcpEditor：" + _target_key;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			// 拡大上限、縮小下限の設定
			_minHeight = m_cnvs.Height * 0.7;
			_maxHeight = m_cnvs.Height * 20.0;

			m_img_png.Height = m_img_png.ActualHeight * 0.77;
			m_img_png.Width = m_img_png.ActualWidth * 0.77;
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
			if ((_minHeight <= height) && (height <= _maxHeight)) {
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

		private void m_img_png_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			_isMouseDown = true;  // 押下中

			// GetPositionメソッドで現在のマウス座標を取得し、マウス移動開始点を更新
			// （マウス座標は、キャンバスからの相対的な位置とする）
			_startPoint = e.GetPosition(m_cnvs);

			e.Handled = true;   // イベントを処理済みとする（当イベントがこの先伝搬されるのを止めるため）
		}

		private void m_img_png_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			_isMouseDown = false;  // 離す
			e.Handled = true;

		}

		private void m_img_png_MouseLeave(object sender, MouseEventArgs e) {
			_isMouseDown = false;  // 離す
			e.Handled = true;
		}

		private void m_img_png_MouseMove(object sender, MouseEventArgs e) {
			// 非押下中は処理なし
			if (!_isMouseDown) { return; }

			// マウスの現在位置座標を取得（OperationAreaからの相対位置）
			_crrntPoint = e.GetPosition(m_cnvs);

			//移動開始点と現在位置の差から、MouseMoveイベント1回分の移動量を算出
			double offsetX = _crrntPoint.X - _startPoint.X;
			double offsetY = _crrntPoint.Y - _startPoint.Y;

			// 動かす対象の図形からMatrixオブジェクトを取得
			// このMatrixオブジェクトを用いて図形を描画上移動させる
			Matrix matrix = ((MatrixTransform)m_img_png.RenderTransform).Matrix;

			// TranslateメソッドにX方向とY方向の移動量を渡し、移動後の状態を計算
			matrix.Translate(offsetX, offsetY);

			// 移動後の状態を計算したMatrixオブジェクトを描画に反映する
			m_img_png.RenderTransform = new MatrixTransform(matrix);

			// 移動開始点を現在位置で更新する
			// （今回の現在位置が次回のMouseMoveイベントハンドラで使われる移動開始点となる）
			_startPoint = _crrntPoint;

			e.Handled = true;
		}

		private void m_img_png_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			cGcp obj_point = (cGcp)m_lstVw_GcpPoint.SelectedItem;

			//+++++++++++++++++++++++++++++++++++++++++++++
			// ポイント選択なし
			if (null == obj_point) { return; }

			// 右クリック位置取得
			var pos = e.GetPosition(m_img_png);
			_ObjCtrlGcpItem._GcpItem.Add(new cGcpItem(obj_point.Name, obj_point.Lat, obj_point.Lon, pos.X, pos.Y));
			//_ObjCtrlGcpData._GcpItem.Add(new cGcpItem());
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			_callback(_target_key);
		}
	}
}
