using OpenCvSharp;
using OpenCvSharp.Features2D;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Documents;
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
		private int _width = 0;
		private int _height = 0;
		private double _minScale = 0;
		private double _maxScalet = 0;
		private double _scale = 0;
		private bool _isMouseDown = false;
		private System.Windows.Point _startPoint;
		private System.Windows.Point _crrntPoint;

		private List<string> _lstEntry;
		private Mat _orginPng;
		private Mat _prossPng;

		public cCtrlGcpItem _objCtrlGcpItem;

		public GcpEditor() {
			InitializeComponent();
		}

		public GcpEditor(string file_path_, cBlock blk_target_, Callback callback_) {
			InitializeComponent();
			_lstEntry = new List<string>();
			_objCtrlGcpItem = new cCtrlGcpItem();
			_callback = callback_;
			_target_key = System.IO.Path.GetFileName(file_path_);
			m_lstVw_GcpPoint.ItemsSource = blk_target_.LstGcpPoints;
			m_lstVw_GcpList.DataContext = _objCtrlGcpItem._GcpItem;

			_prossPng = null;
			_orginPng = new Mat(file_path_);
			var bmp_source = WriteableBitmapConverter.ToWriteableBitmap(_orginPng);
			bmp_source.Freeze();
			m_img_png.Source = bmp_source;

			_height = _orginPng.Height;
			_width = _orginPng.Width;

			this.Title = "GcpEditor：" + _target_key;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			// 拡大上限、縮小下限の設定
			_minScale = 0.2;
			_maxScalet = 5.0;
			_scale = _minScale;

			m_img_png.Height = _height * _scale;
			m_img_png.Width = _width * _scale;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

			if (null != _orginPng) { _orginPng.Dispose(); }
			if (null != _prossPng) { _prossPng.Dispose(); }

			_callback(_target_key);
		}

		private void m_scrlVw_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			e.Handled = true;              // 後続のイベントを実行しないための処理

			double scale = _scale;
			if (0 < e.Delta) {
				scale -= 0.05;  // 10%の倍率で縮小
				if (scale < _minScale) {
					scale = _minScale;
				}
			}
			else {
				scale += 0.05;  // 10%の倍率で拡大
				if (_maxScalet < scale) {
					scale = _maxScalet;
				}
			}

			// 画像の拡大縮小
			m_img_png.Height = _height * scale;
			m_img_png.Width = _width * scale;

			// マウス位置が中心になるようにスクロールバーの位置を調整
			// TODO：一定以上のズームになると、スクロールが右下に向かってしまう問題あり
			System.Windows.Point mouse_point = e.GetPosition(m_scrlVw);
			double x_bar_offset = (m_scrlVw.HorizontalOffset + mouse_point.X) * scale - mouse_point.X;
			m_scrlVw.ScrollToHorizontalOffset(x_bar_offset);

			double y_bar_offset = (m_scrlVw.VerticalOffset + mouse_point.Y) * scale - mouse_point.Y;
			m_scrlVw.ScrollToVerticalOffset(y_bar_offset);

			_scale = scale;
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
			// リスト選択なし
			if (null == obj_point) { return; }

			// 右クリック位置取得
			var pos = e.GetPosition(m_img_png);
			int img_x = (int)Math.Round((pos.X / _scale), 0);
			int img_y = (int)Math.Round((pos.Y / _scale), 0);

			string str_name = obj_point.Name;

			// 出力リストに登録登録されてない場合、登録へ
			if (!_lstEntry.Contains(str_name)) {
				_objCtrlGcpItem._GcpItem.Add(new cGcpItem(str_name, obj_point.Lat, obj_point.Lon, img_x, img_y));
				_lstEntry.Add(str_name);

				// 選択場所にポイント登録
				UpdatePngEntry();
			}
		}

		private void m_lstVw_GcpList_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			cGcpItem obj_item = (cGcpItem)m_lstVw_GcpList.SelectedItem;

			//+++++++++++++++++++++++++++++++++++++++++++++
			// リスト選択なし
			if (null == obj_item) { return; }

			// 出力リストに登録されている場合、削除へ
			if (_lstEntry.Contains(obj_item.Name)) {
				_objCtrlGcpItem._GcpItem.Remove(obj_item);
				_lstEntry.Remove(obj_item.Name);

				// 残りのポイント登録
				UpdatePngEntry();
			}
		}

		private void m_btn_output_Click(object sender, RoutedEventArgs e) {

		}

		private void UpdatePngEntry() {
			if (null != _prossPng) { _prossPng.Dispose(); }
			_prossPng = _orginPng.Clone();

			for (int idx = 0; idx < _objCtrlGcpItem._GcpItem.Count; idx++) {
				var item = _objCtrlGcpItem._GcpItem[idx];

				Cv2.Circle(
							_prossPng,
							item.ImgX, item.ImgY,			// center – 円の中心座標
							40,								// radius – 円の半径
							new Scalar(0, 0, 0),			// color – 円の色
							30,								// thickness – 円の枠線の太さ．負の値の場合，円が塗りつぶされます
							0								// lineType – 円の枠線の種類
						);

				Cv2.PutText(
							_prossPng,
							item.Name,							// text – 描かれる文字列
							new OpenCvSharp.Point((item.ImgX + 50), (item.ImgY + 30)),        // org – 文字列の左下角の，画像中の座標
							HersheyFonts.HersheySimplex,          // fontFace – フォントの種類
							5,                                  // fontScale – フォントのスケールファクタ
							new Scalar(0, 0, 0),                // color – 文字列の色
							10,                                  // thickness – フォントの描画に利用される線の太さ
							LineTypes.AntiAlias,                // lineType – 線の種類
							false                               // bottomLeftOrigin – true の場合は画像データの原点が左下，そうでない場合は左上
						);
			}


			var bmp_source = WriteableBitmapConverter.ToWriteableBitmap(_prossPng);
			bmp_source.Freeze();
			m_img_png.Source = bmp_source;
		}

		
	}
}
