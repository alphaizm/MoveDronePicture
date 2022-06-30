// INotifyPropertyChanged notifCies the View of property changes, so that Bindings are updated.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Data;

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

        public cPicData(string str_path_) {
            _Path = str_path_;
            _Lat = 0;
            _Lon = 0;
            _Height = 0;

            var start = _Path.LastIndexOf(@"\") + 1;
            _FileName = _Path.Substring(start, _Path.Length - start);

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property_name_) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name_));
        }
    }
}
