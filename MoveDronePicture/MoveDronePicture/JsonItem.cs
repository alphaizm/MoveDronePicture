using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MoveDronePicture
{
	public class cJsonBase
	{
		public string DirSrc { get; set; }
		public string DirDstServer { get; set; }
		public string DirDstLocal { get; set; }
		public string FileNameRep { get; set; }
		public List<cBlock> LstBlocks { get; set; }

		public cJsonBase() {
			DirSrc = "";
			DirDstServer = "";
			DirDstLocal = "";
			FileNameRep = "";
			LstBlocks = new List<cBlock>();
		}

		public cJsonBase(string str_dir_src_, string str_dir_dst_, string str_file_rep_, List<cBlock> lst_blocks_) {
			DirSrc = str_dir_src_;
			DirDstServer = str_dir_dst_;
			DirDstLocal = str_dir_dst_;
			FileNameRep = str_file_rep_;
			LstBlocks = lst_blocks_;
		}
	}

	public class cBlock
	{
		public string HeaderName { get; set; }
		public string NasFolder { get; set; }
		public string TargetFileName { get; set; }
		public cWindow Window { get; set; }

		public List<cFolder> LstFolders { get; set; }
		public List<cPoint> LstPolyPoints { get; set; }
		public List<cGcp> LstGcpPoints { get; set; }
		public cPoint Center { get; set; }

		public cBlock() {
			HeaderName = "";
			NasFolder = "";
			TargetFileName = "";
			Window = new cWindow();
			LstFolders = new List<cFolder>();
			LstPolyPoints = new List<cPoint>();
			LstGcpPoints = new List<cGcp>();
			Center = new cPoint();
		}

		public cBlock(string str_name_, List<cFolder> lst_folders_, List<cPoint> lst_points_, cPoint center_) {
			HeaderName = str_name_;
			NasFolder = str_name_;
			TargetFileName = str_name_; ;
			Window = new cWindow();
			LstFolders = lst_folders_;
			LstPolyPoints = lst_points_;
			LstGcpPoints = new List<cGcp>();
			Center = center_;
		}
	}

	public class cWindow
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double W { get; set; }
		public double H { get; set; }

		public cWindow() {
			X = 0;
			Y = 0;
			W = 0;
			H = 0;
		}
	}

	public class cFolder
	{
		public string Name { get; set; }
		public double Height { get; set; }
		public double Offset { get; set; }

		public cFolder() {
			Name = "";
			Height = 0;
			Offset = 0;
		}

		public cFolder(string str_name_, double height_, double offset_) {
			Name = str_name_;
			Height = height_;
			Offset = offset_;
		}
	}

	public class cGcp {
		public string Name { get; set; }
		public double Lat { get; set; }
		public double Lon { get; set; }

		public double Hgt { get; set; }

		public cGcp() {
			Name = "";
			Lat = 0;
			Lon = 0;
			Hgt = 0;
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
