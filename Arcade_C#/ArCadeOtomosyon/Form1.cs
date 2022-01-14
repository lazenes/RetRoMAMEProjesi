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
using System.Management;
using System.IO.Ports;

namespace ArCadeOtomosyon
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
       
        String url = "";
        int jeton = 0;      
        SerialPort serialPort1;
        bool sayfa = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            //webBrowser1.Document.Cookie.Remove(0, webBrowser1.Document.Cookie.Length);
           

            Control.CheckForIllegalCrossThreadCalls = false;
            var MyIni = new IniFile(Path.Combine(Environment.CurrentDirectory, "ayarlar.ini"));
            webBrowser2.Navigate("https://arcade.enesbiber.com.tr/cmd.php?online=Online&durum=1");
            url = MyIni.Read("url", "ayarlar");
            string sonRom = MyIni.Read("SonRom", "ayarlar");
            string Oyun = MyIni.Read("Oyun", "ayarlar");
            string port = MyIni.Read("PORT", "ayarlar");
            serialPort1 = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(SeriPort_VeriOKU);
            serialPort1.DtrEnable = true;
            serialPort1.Open();
            webBrowser1.Navigate(url);           
            pictureBox1.Visible = true;
            if (!string.IsNullOrEmpty(sonRom)) {
                timer1.Start();
                label2.Text = Oyun;
                String dizin = Path.Combine(Environment.CurrentDirectory, "mame.exe");
                foreach (var processKill in Process.GetProcessesByName("mame"))
                {
                    processKill.Kill();
                }
               
                Process process = new Process();
                process.StartInfo.FileName = dizin;
                process.StartInfo.Arguments = sonRom + ".zip";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = false;
                process.Start();
            }
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


        private void WebBrowser1_ProgressChanged(Object sender,
                                           WebBrowserProgressChangedEventArgs e)
        {
            progressBar1.Maximum = (int)e.MaximumProgress;
            progressBar1.Value = (int)e.CurrentProgress;
          
           
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            pictureBox1.Visible = true;
            button1.Visible = false;
          
        }
        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool CheckNet()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            try
            {
                sayfa = true;
                webBrowser1.Refresh(WebBrowserRefreshOption.Completely);
              //  webBrowser1.Navigate(url);

               // label3.Text = CheckNet().ToString();
            if (CheckNet())
            {
                    label3.ForeColor = Color.Green;

                    label3.Text = "Bağlantı Sinyali: ONLİNE";


                    if (webBrowser1.ReadyState == WebBrowserReadyState.Complete)
                    {

                        var RootObjects = JsonConvert.DeserializeObject<List<OyunBilgi>>(webBrowser1.Document.Body.InnerText);

                        foreach (var rootObject in RootObjects)
                        {

                            var MyIni = new IniFile(Path.Combine(Environment.CurrentDirectory, "ayarlar.ini"));
                            string Oyun = MyIni.Read("Oyun", "ayarlar");
                            if (Oyun != rootObject.isim)
                            {

                                label2.Text = rootObject.isim;
                                String dizin = Path.Combine(Environment.CurrentDirectory, "mame.exe");
                                if (rootObject.dosya != "kapat" && rootObject.dosya != "Arcade_Kapat" && rootObject.dosya != "Restart" && rootObject.dosya != "jeton" && rootObject.dosya != "dur")
                                {
                                    foreach (var processKill in Process.GetProcessesByName("mame"))
                                    {
                                        processKill.Kill();
                                    }

                                    MyIni.Write("SonRom", rootObject.dosya, "ayarlar");
                                    MyIni.Write("Oyun", rootObject.isim, "ayarlar");

                                    txtAlinan.Text = "\n" + txtAlinan.Text + rootObject.isim + "\n";
                                    Process process = new Process();
                                    process.StartInfo.FileName = dizin;
                                    process.StartInfo.Arguments = rootObject.dosya + ".zip";
                                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                    process.StartInfo.UseShellExecute = false;
                                    process.Start();
                                }

                                else
                                {
                                    if (rootObject.dosya == "jeton")
                                    {
                                        jeton++;

                                        ServisMod(rootObject.dosya);
                                        webBrowser2.Navigate("https://arcade.enesbiber.com.tr/cmd.php?dosya=dur&isim=Servis Modu jeton");
                                    }
                                    else
                                    {
                                        jeton = 0;
                                        ServisMod(rootObject.dosya);
                                    }
                                }

                            }
                        }
                    } 
            
                }
                else
                {
                    label3.ForeColor = Color.Red;
                    label3.Text = "Bağlantı Sinyali: OFFLİNE";


                }
            } 
            catch (NullReferenceException err)
            {
                Console.WriteLine("Hiçbir Bir şey olmasa da birşeyler olabilir");
                Console.WriteLine(err.Message);
            }
        }
         void ServisMod(String komut) {

            switch (komut)
            {
                case    "kapat":
                    // Mame Uygulaması Kapatılır
                    foreach (var processKill in Process.GetProcessesByName("mame"))
                    {
                        processKill.Kill();
                    }
                    break;
                case "Arcade_Kapat":
                    //Bilgisayarı kapatır
                    webBrowser2.Navigate("https://arcade.enesbiber.com.tr/cmd.php?dosya=dur&isim=Servis Modu Kapat");
                    ArcadeOfflineMOd();
                    Shutdown(1);
                    break;
                case "Restart":
                    // bilgisayarı yeniden başlatır
                    webBrowser2.Navigate("https://arcade.enesbiber.com.tr/cmd.php?dosya=dur&isim=Servis Modu Restart");
                    ArcadeOfflineMOd();
                    Shutdown(2);
                    break;
                case "jeton":
                    // oyuna jeton atar
                    serialPort1.Write("jeton");
                   
                        break;
                default:
                    // code block
                    
                    break;
            }
        }
     

void Shutdown(int flg)
    {
        ManagementBaseObject mboShutdown = null;
        ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
        mcWin32.Get();

        // You can't shutdown without security privileges
        mcWin32.Scope.Options.EnablePrivileges = true;
        ManagementBaseObject mboShutdownParams =
                 mcWin32.GetMethodParameters("Win32Shutdown");

        // Flag 1 means we want to shut down the system. Use "2" to reboot.
        mboShutdownParams["Flags"] = flg;
        mboShutdownParams["Reserved"] = "0";
        foreach (ManagementObject manObj in mcWin32.GetInstances())
        {
            mboShutdown = manObj.InvokeMethod("Win32Shutdown",
                                           mboShutdownParams, null);
        }
    }

        private void SeriPort_VeriOKU(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
           // SeriPortVeri = serialPort1.ReadExisting();
           txtAlinan.Text = "\n"+ txtAlinan.Text + serialPort1.ReadExisting() + "\n";
           txtAlinan.Select(txtAlinan.Text.Length, 0);
            // MessageBox.Show("vERİ gELDİ");
           
        }

        private void txtAlinan_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Write(textBox1.Text);
        }
     

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                ArcadeOfflineMOd();
            }
           


        }
        void ArcadeOfflineMOd() {

            String dizin = Path.Combine(Environment.CurrentDirectory, "offline.exe");

            Process process = new Process();
            process.StartInfo.FileName = dizin;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            sayfa = true;
            timer1.Start();
        }
    }
}
