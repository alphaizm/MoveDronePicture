using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
		private int _width = 0;
		private int _height = 0;
		private double _minScale = 0;
		private double _maxScalet = 0;
		private double _scale = 0;
		private bool _isMouseDown = false;
		private System.Windows.Point _startPoint;

		private Mat _orginPng;
		private Mat _prossPng;

		private string _pathGcpList;
		private string _localImgName;

		public cCtrlGcpItem _objCtrlGcpItem;
		private Dictionary<string, List<cGcpRegistered>> _dicGcpRegistered;

		public GcpEditor() {
			InitializeComponent();
		}

		public GcpEditor(cImgItem img_item_, cBlock blk_target_, Callback callback_) {
			InitializeComponent();
			_objCtrlGcpItem = new cCtrlGcpItem();
			_dicGcpRegistered = new Dictionary<string, List<cGcpRegistered>>();
			_callback = callback_;
			_target_key = System.IO.Path.GetFileName(img_item_.ImgPath);
			m_lstVw_GcpPoint.ItemsSource = blk_target_.LstGcpPoints;
			m_lstVw_GcpList.DataContext = _objCtrlGcpItem._GcpItem;

			_prossPng = null;
			_orginPng = new Mat(img_item_.ImgPath);
			var bmp_source = WriteableBitmapConverter.ToWriteableBitmap(_orginPng);
			bmp_source.Freeze();
			m_img_png.Source = bmp_source;

			_height = _orginPng.Height;
			_width = _orginPng.Width;

			_pathGcpList = Path.Combine(
											Path.GetDirectoryName(img_item_.MoveLocalPath),
											"_" + img_item_.YYYYMMDD + "_" + blk_target_.TargetFileName + "_GcpList.txt"
										);
			_localImgName = Path.GetFileName(img_item_.MoveLocalPath);

			this.Title = "GcpEditor：" + _target_key;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			// 拡大上限、縮小下限の設定
			_minScale = 0.2;
			_maxScalet = 5.0;
			_scale = _minScale;

			SetPngSiz(_scale);

			// PreviewKeyDown発生用にフォーカス
			m_scrlVw.Focus();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

			if (null != _orginPng) { _orginPng.Dispose(); _orginPng = null; }
			if (null != _prossPng) { _prossPng.Dispose(); _prossPng = null; }
			GC.Collect();

			_callback(_target_key);
		}

		private void m_scrlVw_GotFocus(object sender, RoutedEventArgs e) {
			m_brd_img.BorderThickness = new Thickness(10);
		}

		private void m_scrlVw_LostFocus(object sender, RoutedEventArgs e) {
			m_brd_img.BorderThickness = new Thickness(0);
		}

		private void m_scrlVw_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			e.Handled = true;              // 後続のイベントを実行しないための処理

			_scale = UpdateScale(e.Delta);

			// マウス位置が中心になるようにスクロールバーの位置を調整
			// TODO：一定以上のズームになると、スクロールが右下に向かってしまう問題あり
			System.Windows.Point mouse_point = e.GetPosition(m_scrlVw);
			double x_bar_offset = (m_scrlVw.HorizontalOffset + mouse_point.X) * _scale - mouse_point.X;
			m_scrlVw.ScrollToHorizontalOffset(x_bar_offset);

			double y_bar_offset = (m_scrlVw.VerticalOffset + mouse_point.Y) * _scale - mouse_point.Y;
			m_scrlVw.ScrollToVerticalOffset(y_bar_offset);
		}

		private void m_scrlVw_PreviewKeyDown(object sender, KeyEventArgs e) {
			switch (e.Key) {
				case Key.Enter:
					MessageBox.Show("test");
					break;
				case Key.PageUp:
					_scale = UpdateScale(0);    // 拡大
					break;
				case Key.PageDown:
					_scale = UpdateScale(1);    // 縮小
					break;
				case Key.Space:
					// Ctrl + Space		-> 表示位置、表示サイズ初期化
					if (Keyboard.Modifiers == ModifierKeys.Control) {
						_scale = _minScale;
						SetPngSiz(_scale);

						System.Windows.Point pos_zero = new System.Windows.Point(0, 0);
						var offset = m_cnvs.TranslatePoint(pos_zero, m_img_png);
						SetPngPos(offset);
						_startPoint = pos_zero;
					}
					break;
				default:
					break;
			}
		}

		private void m_img_png_SizeChanged(object sender, SizeChangedEventArgs e) {
			m_cnvs.Height = e.NewSize.Height;
			m_cnvs.Width = e.NewSize.Width;
		}

		/// <summary>
		/// 【イベント】【キャンバス】左クリック押下
		/// -> 画像移動開始
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_img_png_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			_isMouseDown = true;  // 押下中

			// PreviewKeyDown発生用にフォーカス
			m_scrlVw.Focus();

			// GetPositionメソッドで現在のマウス座標を取得し、マウス移動開始点を更新
			// （マウス座標は、キャンバスからの相対的な位置とする）
			_startPoint = e.GetPosition(m_cnvs);

			e.Handled = true;   // イベントを処理済みとする（当イベントがこの先伝搬されるのを止めるため）
		}

		/// <summary>
		/// 【イベント】【キャンバス】左クリック離す
		/// -> 画像移動終了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_img_png_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			_isMouseDown = false;  // 離す
			e.Handled = true;

		}

		/// <summary>
		/// 【イベント】【キャンバス】キャンバス外にマウス移動
		/// -> 画像移動終了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_img_png_MouseLeave(object sender, MouseEventArgs e) {
			_isMouseDown = false;  // 離す
			e.Handled = true;
		}

		/// <summary>
		/// 【イベント】【キャンバス】マウス移動
		/// -> 左クリック中は画像移動
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_img_png_MouseMove(object sender, MouseEventArgs e) {
			// 非押下中は処理なし
			if (!_isMouseDown) { return; }

			// マウスの現在位置座標を取得（OperationAreaからの相対位置）し、移動量に応じて画像移動
			MovePng(e.GetPosition(m_cnvs));

			e.Handled = true;
		}

		/// <summary>
		/// 【イベント】【キャンバス】右クリック離す
		/// -> リスト選択中は、GCPリストに登録
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
			if (!_objCtrlGcpItem.ContainName(str_name)) {
				_objCtrlGcpItem._GcpItem.Add(new cGcpItem(str_name, obj_point.Lat, obj_point.Lon, img_x, img_y));

				cGcpRegistered gcp_regist = new cGcpRegistered(
														_localImgName,
														obj_point.Lat,
														obj_point.Lon,
														obj_point.Hgt,
														img_x,
														img_y
													);

				// 出力データに登録
				if (!_dicGcpRegistered.ContainsKey(str_name)) {
					_dicGcpRegistered.Add(str_name, new List<cGcpRegistered>() { gcp_regist });
				}
				else {
					_dicGcpRegistered[str_name].Add(gcp_regist);
				}

				// 選択場所にポイント登録
				UpdatePngEntry();
			}
		}

		/// <summary>
		/// 【イベント】【GCPリスト】右クリック離す
		/// -> 登録されているGCPポイントをGCPリストから削除
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_lstVw_GcpList_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			cGcpItem obj_item = (cGcpItem)m_lstVw_GcpList.SelectedItem;

			//+++++++++++++++++++++++++++++++++++++++++++++
			// リスト選択なし
			if (null == obj_item) { return; }

			string str_name = obj_item.Name;

			// 出力リストに登録されている場合、削除へ
			if (_objCtrlGcpItem.ContainName(str_name)) {
				_objCtrlGcpItem._GcpItem.Remove(obj_item);

				// 出力データから削除
				var obj = _dicGcpRegistered[str_name].FindAll(x => x.ImgName == _localImgName)[0];
				_dicGcpRegistered[str_name].Remove(obj);

				// 残りのポイント登録
				UpdatePngEntry();
			}
		}

		private void m_btn_output_Click(object sender, RoutedEventArgs e) {
			StringBuilder str_output = new StringBuilder();

			str_output.AppendLine("EPSG:32653");
			str_output.AppendLine("#X Y Z pixcelX pixcelY file");
			foreach (KeyValuePair<string, List<cGcpRegistered>> regist_data in _dicGcpRegistered) {
				str_output.AppendLine("#" + regist_data.Key.ToString());

				List<cGcpRegistered> lst_gcp_info = regist_data.Value;
				for (int idx = 0; idx < lst_gcp_info.Count; idx++) {
					cGcpRegistered gcp_info = lst_gcp_info[idx];
					StringBuilder str_row = new StringBuilder();
					str_row.Append(gcp_info.GcpX.ToString());
					str_row.Append(" ");
					str_row.Append(gcp_info.GcpY.ToString());
					str_row.Append(" ");
					str_row.Append(gcp_info.GcpZ.ToString());
					str_row.Append(" ");
					str_row.Append(gcp_info.ImgX.ToString());
					str_row.Append(" ");
					str_row.Append(gcp_info.ImgY.ToString());
					str_row.Append(" ");
					str_row.Append(gcp_info.ImgName);

					str_output.AppendLine(str_row.ToString());
				}
			}

			File.WriteAllText(_pathGcpList, str_output.ToString());
		}

		private double UpdateScale(int chg_) {
			double scale = _scale;
			if (0 < chg_) {
				scale -= 0.05;  // 5%の倍率で縮小
				if (scale < _minScale) {
					scale = _minScale;
				}
			}
			else {
				scale += 0.05;  // 5%の倍率で拡大
				if (_maxScalet < scale) {
					scale = _maxScalet;
				}
			}

			// 画像の拡大縮小
			SetPngSiz(scale);

			return scale;
		}

		private void SetPngSiz(double scale_) {
			m_img_png.Height = _height * scale_;
			m_img_png.Width = _width * scale_;
		}

		private void MovePng(System.Windows.Point crrntPoint_) {
			//移動開始点と現在位置の差から、MouseMoveイベント1回分の移動量を算出
			double offsetX = crrntPoint_.X - _startPoint.X;
			double offsetY = crrntPoint_.Y - _startPoint.Y;

			SetPngPos(new System.Windows.Point(offsetX, offsetY));

			// 移動開始点を現在位置で更新する
			// （今回の現在位置が次回のMouseMoveイベントハンドラで使われる移動開始点となる）
			_startPoint = crrntPoint_;
		}

		private void SetPngPos(System.Windows.Point offset_) {
			// 動かす対象の図形からMatrixオブジェクトを取得
			// このMatrixオブジェクトを用いて図形を描画上移動させる
			Matrix matrix = ((MatrixTransform)m_img_png.RenderTransform).Matrix;

			// TranslateメソッドにX方向とY方向の移動量を渡し、移動後の状態を計算
			matrix.Translate(offset_.X, offset_.Y);

			// 移動後の状態を計算したMatrixオブジェクトを描画に反映する
			m_img_png.RenderTransform = new MatrixTransform(matrix);
		}

		private void UpdatePngEntry() {
			if (null != _prossPng) { _prossPng.Dispose(); }
			_prossPng = null;
			GC.Collect();
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
