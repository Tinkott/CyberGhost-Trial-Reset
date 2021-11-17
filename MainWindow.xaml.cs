using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.ComponentModel;

namespace CG_TrialReset
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    // Через этот класс портируем функцию GetUserTilePath
    // и через метод GetUserTile возвращаем саму картинку
    class GetImageAccount
    {
        [DllImport("shell32.dll", EntryPoint = "#261",
               CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(
          string username,
          UInt32 whatever, // 0x80000000
          StringBuilder picpath, int maxLength);

        public static string GetUserTilePath(string username)
        {
            var sb = new StringBuilder(1000);
            GetUserTilePath(username, 0x80000000, sb, sb.Capacity);
            return sb.ToString();
        }

        public static BitmapImage GetUserTile(string username)
        {
            return new BitmapImage(new Uri(GetUserTilePath(username)));
        }
    }

    public partial class MainWindow : Window 
    {
        public static void SearchKillProcess(string procname)
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName(procname))
                {
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public MainWindow()
        {

            InitializeComponent();

            mediaElement.Play();

            var _username = Environment.UserName;
            var baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Meepure_Files/";

            if (Directory.Exists(baseFolder) == false)
            {
                Directory.CreateDirectory(baseFolder);
            }

            userAccountImage.ImageSource = GetImageAccount.GetUserTile(Environment.UserName);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            ProgramStateLabel.Foreground = Brushes.Gray;
            ProgramStateLabel.Text = "Working... Please Wait...";

            var backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (s, e) =>
            {

                Thread.Sleep(1000);

                var baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Meepure_Files/";
                if (Process.GetProcessesByName("Dashboard").Any())
                {
                    try
                    {
                        foreach (Process proc in Process.GetProcessesByName("Dashboard"))
                        {
                            proc.Kill();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                String local = Environment.CurrentDirectory;
                File.WriteAllBytes(baseFolder + "resource.exe", Resource.resource_0);

                var startInfo = new ProcessStartInfo
                {
                    FileName = baseFolder + "resource.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process.Start(startInfo);

                Thread.Sleep(100);

                SearchKillProcess("resource");

                Thread.Sleep(100);

                try
                {
                    File.WriteAllBytes(baseFolder + "resource.exe", Resource.resource_1);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            };

            backgroundWorker.RunWorkerCompleted += (s, e) =>
            {
                ProgramStateLabel.Foreground = Brushes.Green;
                ProgramStateLabel.Text = "Trial successfully resetted!";
            };

            backgroundWorker.RunWorkerAsync();
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Position = TimeSpan.Zero;
            mediaElement.Play();
        }

        private void Qiwi_social_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", "http://qiwi.com/n/TINKOTT");
        }

        private void Vk_social_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", "https://lolz.guru/neoandertal/");
        }
    }


}
