using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace MoveDronePicture
{
	public class ComMethod
	{
		/// <summary>
		/// 緯度経度からUTM座標，ゾーン及び偏位角を取得
		/// (北緯の東経地域に限る。原点緯度は北緯0度。
		/// 数式は以下を参照。
		/// /https://vldb.gsi.go.jp/sokuchi/surveycalc/surveycalc/algorithm/xy2bl/xy2bl.htm
		/// </summary>
		/// <param name="lat_">緯度</param>
		/// <param name="lon_">経度</param>
		/// <param name="zone_">ゾーン</param>
		/// <param name="E_"></param>
		/// <param name="N_"></param>
		/// <param name="gam_">偏位角</param>
		public static void DesToUTM(double lat_, double lon_, out double zone_, out double E_, out double N_, out double gam_) {
			double f, r, n, Am, n1, n2, n3, n4, n5, n6;
			double[] A = new double[6];
			double[] a = new double[6];
			double lon0, lon, lat0, lat, gd, ed, E0, N0, k0, t, tm;
			double tau, del, lamc, lams, Smlat, rk0;
			int i;

			zone_ = 0;
			E_= 0;
			N_= 0;
			gam_ = 0;

			f = 1.0 / (298.257223563);//WGS84 地球の扁平率
			r = 6378137.00000000; //GRS80 & WGS84 地球長辺半径

			E0 = 500 * 1000;// 経度オフセット(500km)
			N0 = 0.0;// 北半球オフセット
					 //	N0 = 10000.0*1000.0;// 南半球オフセット(10000km)
			k0 = 0.9996;//UTM座標系の場合
						//	k0 = 0.9999;//国土地理院

			n = f / (2.0 - f);
			n1 = n;
			n2 = n1 * n;
			n3 = n2 * n;
			n4 = n3 * n;
			n5 = n4 * n;
			n6 = n5 * n;
			A[0] = 1 + n2 / 4.0 + n4 / 64.0;
			rk0 = r * k0 / (1 + n1);
			Am = rk0 * A[0];

			A[1] = -3.0 / 2.0 * (n1 - n3 / 8.0 - n5 / 64.0);
			A[2] = 15.0 / 16.0 * (n2 - n4 / 4.0);
			A[3] = -35.0 / 48.0 * (n3 - 5.0 / 16.0 * n5);
			A[4] = 315.0 / 512.0 * n4;
			A[5] = -693.0 / 1280.0 * n5;

			a[1] = 1.0 / 2.0 * n1 - 2.0 / 3.0 * n2 + 5.0 / 16.0 * n3 + 41.0 / 180.0 * n4 - 127.0 / 288.0 * n5;
			a[2] = 13.0 / 48.0 * n2 - 3.0 / 5.0 * n3 + 557.0 / 1440.0 * n4 + 281.0 / 630.0 * n5;
			a[3] = 61.0 / 240.0 * n3 - 103.0 / 140.0 * n4 + 15061.0 / 26880.0 * n5;
			a[4] = 49561.0 / 161280.0 * n4 - 179.0 / 168.0 * n5;
			a[5] = 34729.0 / 80640.0 * n5;

			zone_ = (int)(lon_ / 6.0) + 31;// 統計のUTMザーン番号
			lon0 = (double)(int)(lon_ / 6.0) * 6.0 + 3.0; // UTMゾーンでの中心経度
			lat0 = 0.0;//赤道固定

			lon0 = lon0 * Math.PI / 180.0;
			lat0 = lat0 * Math.PI / 180.0;

			lon = lon_ * Math.PI / 180.0;
			lat = lat_ * Math.PI / 180.0;


			//緯度原点の調整分の計算
			Smlat = Am * lat0;
			for (i = 1; i <= 5; i++) {
				Smlat = Smlat + rk0 * A[i] * Math.Sin(2.0 * ((double)i) * lat0);
			}

			t = Math.Sinh(Atanh(Math.Sin(lat)) - 2 * Math.Sqrt(n) / (1.0 + n) * Atanh(2 * Math.Sqrt(n) / (1.0 + n) * Math.Sin(lat)));
			tm = Math.Sqrt(1 + t * t);
			gd = Math.Atan(t / (Math.Cos(lon - lon0)));
			ed = Atanh(Math.Sin(lon - lon0) / Math.Sqrt(1 + t * t));

			E_ = E0 + Am * ed;
			N_ = N0 + Am * gd;

			for (i = 1; i <= 5; i++) {
				E_ = E_ + Am * a[i] * Math.Cos(2.0 * ((double)i) * gd) * Sinh(2.0 * ((double)i) * ed);
				N_ = N_ + Am * a[i] * Math.Sin(2.0 * ((double)i) * gd) * Cosh(2.0 * ((double)i) * ed);
			}
			N_ = N_ - Smlat;// 緯度原点の調整

			// 真北方向角(分)を求める。(プラスは真北より西，マイナスは東)

			del = 1.0;
			tau = 0.0;

			for (i = 1; i <= 5; i++) {
				del = del + 2.0 * ((double)i) * a[i] * Math.Cos(2.0 * ((double)i) * gd) * Cosh(2.0 * ((double)i) * ed);
				tau = tau + 2.0 * ((double)i) * a[i] * Math.Sin(2.0 * ((double)i) * gd) * Sinh(2.0 * ((double)i) * ed);
			}

			lamc = Math.Cos(lon - lon0);
			lams = Math.Sin(lon - lon0);

			gam_ = -Math.Atan((tau * tm * lamc + del * t * lams) / (del * tm * lamc - tau * t * lams)) / Math.PI * 60.0;

		}

		private static double Atanh(double x) {
			return (Math.Log((1 + x) / (1 - x)) / 2);
		}

		private static double Sinh(double x) {
			return ((Math.Exp(x) - Math.Exp(-x)) / 2);
		}

		private static double Cosh(double x) {
			return ((Math.Exp(x) + Math.Exp(-x)) / 2);
		}
	}
}
