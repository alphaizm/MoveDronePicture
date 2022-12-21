using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveDronePicture
{
	public class cJsonBase
	{
		public string DirSrc { get; set; }
		public string DirDst { get; set; }
		public List<cBlock> LstBlocks { get; set; }

		public cJsonBase() {
			DirSrc = "";
			DirDst = "";
			LstBlocks = new List<cBlock>();
		}

		public cJsonBase(string str_dir_src_, string str_dir_dst_, List<cBlock> lst_blocks_) {
			DirSrc = str_dir_src_;
			DirDst = str_dir_dst_;
			LstBlocks = lst_blocks_;
		}

	}

	public class cBlock
	{
		public string Name { get; set; }
		public Dictionary<string, double> DicFolders { get; set; }
		public List<cPoint> LstPoints { get; set; }
		public cPoint Center { get; set; }

		public cBlock() {
			Name = "";
			DicFolders = new Dictionary<string, double>();
			LstPoints = new List<cPoint>();
			Center = new cPoint();
		}

		public cBlock(string str_name_, Dictionary<string, double> dic_folders_, List<cPoint> lst_points_, cPoint center_) {
			Name = str_name_;
			DicFolders = dic_folders_;
			LstPoints = lst_points_;
			Center = center_;
		}

	}

	public class cPoint
	{
		public double Lat { get; set; }
		public double Lon { get; set; }

		public cPoint() {
			Lat = 0;
			Lon = 0;
		}

		public cPoint(double lat_, double lon_) {
			Lat = lat_;
			Lon = lon_;
		}
	}
}
