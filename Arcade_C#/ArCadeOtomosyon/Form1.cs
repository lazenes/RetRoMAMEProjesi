using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Diagnostics;

namespace ArCadeOtomosyon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var MyIni = new IniFile(Path.Combine(Environment.CurrentDirectory, "ayarlar.ini"));

            string url = MyIni.Read("url", "ayarlar");
            webBrowser1.Navigate(url);
           
        }
        public class OyunBilgi
        {
            public string dosya { get; set; }
            public string isim { get; set; }
        }

        public class Root
        {
            public List<OyunBilgi> oyunBilgi { get; set; }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //label2.Text=webBrowser1.Document.Body.InnerText;
            var  RootObjects = JsonConvert.DeserializeObject<List<OyunBilgi>>(webBrowser1.Document.Body.InnerText);

           

            foreach (var rootObject in RootObjects)
            {
                
                label2.Text = rootObject.isim;
               
            }
            
        }

        private void WebBrowser1_ProgressChanged(Object sender,
                                           WebBrowserProgressChangedEventArgs e)
        {
            progressBar1.Maximum = (int)e.MaximumProgress;
            progressBar1.Value = (int)e.CurrentProgress;
          
           
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
            pictureBox1.Visible = true;
          
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            webBrowser1.Refresh(WebBrowserRefreshOption.Completely);
            //label2.Text=webBrowser1.Document.Body.InnerText;
            var RootObjects = JsonConvert.DeserializeObject<List<OyunBilgi>>(webBrowser1.Document.Body.InnerText);



            foreach (var rootObject in RootObjects)
            {

                
                if (label2.Text != rootObject.isim) { 
                    
                    label2.Text = rootObject.isim;
                    String dizin = Path.Combine(Environment.CurrentDirectory, "mame.exe");
                    //, strMsgReceive

                    foreach (var processKill in Process.GetProcessesByName("mame"))
                    {
                        processKill.Kill();
                    }
                    Process process = new Process();
                    process.StartInfo.FileName = dizin;
                    process.StartInfo.Arguments = rootObject.dosya + ".zip";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                }
            }
           
        }
    }
}
