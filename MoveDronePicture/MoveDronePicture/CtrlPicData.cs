﻿// INotifyPropertyChanged notifCies the View of property changes, so that Bindings are updated.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.Drawing;

namespace MoveDronePicture
{
    /// <summary>
    /// 写真ファイル通知用クラスの管理
    /// </summary>
    public sealed class cCtrlPicData
    {
        public ObservableCollection<cPicData> PicData { get; }

        public cCtrlPicData() {
            PicData = new ObservableCollection<cPicData>();
            // 複数スレッドからコレクション操作できるようにする
            BindingOperations.EnableCollectionSynchronization(PicData, new object());
        }

        public void AddPicData(string[] ary_pathes_) {
            PicData.Clear();
            foreach ( var path in ary_pathes_ ) {
                PicData.Add(new cPicData(path));
            }
        }
    }

    /// <summary>
    /// 写真ファイル通知用クラス
    /// </summary>
    public sealed class cPicData : INotifyPropertyChanged
    {
        private string _Path;
        private string _FileName;
        private double _Lat;
        private double _Lon;
        private double _Height;

        const int ID_LAT = 0x0002;
        const int ID_LON = 0x0004;
        const int ID_HEIGHT = 0x0006;
        const int ID_DATE = 0x9004;

        public cPicData(string str_path_) {
            _Path = str_path_;
            _Lat = 0;
            _Lon = 0;
            _Height = 0;

            _FileName = Path.GetFileName(str_path_);
            //var file = await Window.Storage.strageFile.GetFileFromPathAsynck(str_path_);
            Bitmap bmp = new Bitmap(str_path_);
            foreach (var prop in bmp.PropertyItems) {
                switch (prop.Id) {
                    case ID_DATE:
                        var str_time = Encoding.UTF8.GetString(prop.Value);
                        break;
                    case ID_LAT:
                        _Lat = GetDecLatLon(prop.Value);
                        break;
                    case ID_LON:
                        _Lon = GetDecLatLon(prop.Value);
                        break;
                    case ID_HEIGHT:
                        break;
                    default:
                        break;
                }
            }
            //  todo    上記から必要なデータを読みだして各プロパティに入れる
        }

        public string FileName {
            get { return _FileName; }
        }

        public string Lat {
            get { return _Lat.ToString(); }
        }

        public string Lon {
            get { return _Lon.ToString(); }
        }

        public string Height {
            get { return _Height.ToString(); }
        }

        private double GetDecLatLon(byte[] ary_value_) {
            uint deg_numerator = BitConverter.ToUInt32(ary_value_, 0);
            //uint deg_denominator = BitConverter.ToUInt32(ary_value_, 4);

            uint min_numerator = BitConverter.ToUInt32(ary_value_, 8);
            //uint min_denominator = BitConverter.ToUInt32(ary_value_, 12);

            uint sec_numerator = BitConverter.ToUInt32(ary_value_, 16);
            uint sec_denominator = BitConverter.ToUInt32(ary_value_, 20);

            double sec = 0;
            if (0 != sec_denominator) {
                sec = (double)sec_numerator / (double)sec_denominator;
            }

            double ret = deg_numerator + min_numerator / 60 + sec / 3600;
            return ret;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property_name_) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name_));
        }
    }
}
