using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace App14
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnScriptNotify(object sender, NotifyEventArgs e)
        {
            if (e.Value.Contains(NotifyScriptTypes.__KEYBOARD_EVENT.ToString()))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<ScriptNotifyMessage<KeyDownScriptNotifyMessage>>(e.Value);
                    if (result?.Message != null)
                    {
                        var keyDown = new KeyDownMessage(result.Message);
                        if (keyDown.KeyCode == VirtualKey.Back)
                        {
                            RemoveInternalHtmlContent();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("OnScriptNotify __KEYBOARD_EVENT - " + ex.Message);
                }
            }
        }

        private async void OnloadCompleted(object sender, NavigationEventArgs e)
        {
            var w = (WebView)sender;
            await ScriptManager.RunScriptAsync(w, "jQuery");
            await ScriptManager.RunScriptAsync(w, "event");
        }

        private async void RemoveInternalHtmlContent()
        {
            string[] arguments = new string[] { @"document.getSelection().deleteFromDocument();" };
            await webview.InvokeScriptAsync("eval", arguments);
        }


        public enum NotifyScriptTypes
        {
            __MOUSE_EVENT,
            __KEYBOARD_EVENT,
            __URL,
            __SIGNATURE,
            __NOTIFIER,
            __SEARCH,
            __SHARE
        }
        public class ScriptNotifyMessage<T>
        {
            public string Type { get; set; }
            public string Error { get; set; }
            public T Message { get; set; }
        }

        public class KeyDownScriptNotifyMessage
        {
            public bool Ctrl { get; set; }
            public bool Alt { get; set; }
            public bool Shift { get; set; }
            public string Char { get; set; }
            public string Type { get; set; }
            public string Key { get; set; }
            public VirtualKey KeyCode { get; set; }
        }

        public class KeyDownMessage
        {
            public bool Ctrl { get { return NotifireMessage.Ctrl; } }
            public bool Alt { get { return NotifireMessage.Alt; } }
            public bool Shift { get { return NotifireMessage.Shift; } }
            public string Char { get { return NotifireMessage.Char; } }
            public string Type { get { return NotifireMessage.Type; } }
            public string Key { get { return NotifireMessage.Key; } }
            public VirtualKey KeyCode { get { return NotifireMessage.KeyCode; } }

            private KeyDownScriptNotifyMessage NotifireMessage { get; set; }
            public KeyDownMessage(KeyDownScriptNotifyMessage notifireMessage)
            {
                if (notifireMessage == null)
                    throw new ArgumentNullException("notifireMessage can't be null");
                else
                    NotifireMessage = notifireMessage;
            }
        }

        private async void webview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                //await ScriptManager.RunScriptAsync(sender, "event");
            }
        }
    }



    public static class ScriptManager
    {
        private const string CONTENT = "__CONTENT";
        //private static ITrackService TrackService = App.Locator.Resolve<ITrackService>();

        #region public 

        public static async Task RunScriptAsync(WebView webView, string scriptName)
        {
            try
            {
                var script = await ScriptProvider.ReadScriptAsync(scriptName);
                await webView.InvokeScriptAsync("eval", new[] { script });
            }
            catch (Exception ex)
            {
                //TrackService.TrackEventMessage(error.Message);
            }
        }

        public static async void RunScriptAsync(WebView webView, IEnumerable<string> script)
        {
            try
            {
                await webView.InvokeScriptAsync("eval", script);
            }
            catch (Exception ex)
            {
                //TrackService.TrackEventMessage(ex.Message);
            }
        }

        public static async void RunScriptAsync(WebView webView, string scriptName, string content)
        {
            try
            {
                var script = await ScriptProvider.ReadScriptAsync(scriptName);
                if (script.Contains(CONTENT))
                    script = script.Replace(CONTENT, content);

                await webView.InvokeScriptAsync("eval", new[] { script });

            }
            catch (Exception ex)
            {
                //TrackService.TrackEventMessage(ex.Message);
            }
        }

        #endregion
    }

    internal static class ScriptProvider
    {
        internal static async Task<string> ReadScriptAsync(string scriptName)
        {
            var appUri = new Uri($"ms-appx:///Assets/{scriptName}.js");
            var file = await StorageFile.GetFileFromApplicationUriAsync(appUri);
            if (file == null)
            {
                return null;
            }
            var script = "";

            using (var stream = await file.OpenReadAsync())
            using (var streamReader = stream.AsStreamForRead())
            using (var dataStream = new StreamReader(streamReader))
            {
                script = dataStream.ReadToEnd();
            }

            return script;
        }
    }
}
