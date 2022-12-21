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
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.IO.IsolatedStorage;

namespace MoveDronePicture
{
	/// <summary>
	/// GoogleMap.xaml の相互作用ロジック
	/// </summary>
	public partial class GoogleMap : Window
	{
		const string API_KEY = ".\\Assets\\api_key";
		const string WEB_TEMPLATE = ".\\Assets\\template.html";
		const string TAG_TITLE = "_TITLE_";
		const string TAG_LAT_ = "_LAT_";
		const string TAG_LNG_ = "_LNG_";
		const string TAG_API_KEY_ = "_API_KEY_";
		private readonly DockPanel _dockpanel = new DockPanel();
		private readonly WebView2Controller _webController = new WebView2Controller();

		public GoogleMap() {
			InitializeComponent();
		}

		public GoogleMap(cBlock blk_target_) {

			this._dockpanel.Children.Add(this._webController.GetWebView2());
			this.AddChild(this._dockpanel);

			using (StreamReader api_key = new StreamReader(API_KEY)) {
				string str_key = api_key.ReadToEnd();
				using (StreamReader web_template = new StreamReader(WEB_TEMPLATE)) {
					string str_html = web_template.ReadToEnd();
					str_html = str_html.Replace(TAG_API_KEY_, str_key);
					str_html = str_html.Replace(TAG_TITLE, blk_target_.Name);
					str_html = str_html.Replace(TAG_LAT_, blk_target_.LstPoints[0].Lat.ToString());
					str_html = str_html.Replace(TAG_LNG_, blk_target_.LstPoints[0].Lon.ToString());

					_webController.NavigateToString(str_html);
				}
			}

			InitializeComponent();

			this.Title = blk_target_.Name;
		}
	}

	public class WebView2Controller
	{
		private readonly WebView2 _webView2 = new WebView2();
		private string _strSelectedTxtScriptId;

		public WebView2Controller() {
			this._webView2.CoreWebView2InitializationCompleted += CoreWebView2Initialization;
		}

		private void CoreWebView2Initialization(object sender, CoreWebView2InitializationCompletedEventArgs e) {
			if (e.IsSuccess) {
				//this._webView2.CoreWebView2.WebMessageReceived += this.WebMessageProcessor;
				this.SelectedTextInitialize();
			}

			//this._webView2.CoreWebView2.OpenDevToolsWindow();
		}

		public async void NavigateToString(string htrml) {
			if (null == this._webView2.CoreWebView2) {
				await this._webView2.EnsureCoreWebView2Async(null);
			}

			this._webView2.CoreWebView2.NavigateToString(htrml);
		}

		public WebView2 GetWebView2() {
			return this._webView2;
		}

		//private void WebMessageProcessor(object sender, CoreWebView2WebMessageReceivedEventArgs e) {
		//	Debug.WriteLine(e.WebMessageAsJson);
		//}

		private async void SelectedTextInitialize() {
			if (null == this._strSelectedTxtScriptId) {
				this._strSelectedTxtScriptId = await this._webView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"document.addEventListener('DOMContentLoaded',new class{constructor(){document.addEventListener('mouseup',this.selectEvent)}selectEvent(event){if(event.isTrusted&&event.button===0){const selection=getSelection();if(!selection.isCollapsed&&selection.type==='Range'){console.log(`SelectedText: '${selection.toString()}'`);try{chrome.webview.postMessage({Type:'SelectedText',Text:selection.toString()})}catch(error){console.error(`'chrome.webview' is undefined.(${error.message})`)}}}}})");
			}
		}
	}
}
