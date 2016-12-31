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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace TwitchAuthWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            //Set the DataContext so we can update the TextBox Text data
            DataContext = this;
            

        }

       

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //By Default the WebBrowser control manages cookies, while debugging this is a real problem as
            //you don;t go through the complete end to end user experience so this call suppresses them
            //Do not use unless debugging
            //NativeMethods.SuppressCookiePersistence();
            //When the button is clicked send the user to the Twitch Auth url as documented here:
            //https://dev.twitch.tv/docs/v5/guides/authentication/

            myBrowser.Navigate("https://api.twitch.tv/kraken/oauth2/authorize?response_type=code&client_id=t6ma91fizbc7hudx6n5z60u9vb9h42&redirect_uri=http://localhost:8080/twitch/callback&scope=user_read&state=123456");
        }

        //Code to handle updating the TextBox Text data
        //TextBox.Text is bound to textBoxContent, Text="{Binding Path=textBoxContent}"

        public event PropertyChangedEventHandler PropertyChanged;

        private string _textBoxContent;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public string textBoxContent
        {
            get { return _textBoxContent; }
            set
            {
                if (value != _textBoxContent)
                {
                    _textBoxContent = value;
                    OnPropertyChanged("textBoxContent");
                }
            }
        }
    }

    
    //For debugging purposes I wanted to supress cookies so I'd go through the whole process each time.
    //This can be removed when not debugging as it will affect the user experience

    public static partial class NativeMethods
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        private const int INTERNET_OPTION_SUPPRESS_BEHAVIOR = 81;
        private const int INTERNET_SUPPRESS_COOKIE_PERSIST = 3;

        public static void SuppressCookiePersistence()
        {
            var lpBuffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
            Marshal.StructureToPtr(INTERNET_SUPPRESS_COOKIE_PERSIST, lpBuffer, true);

            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SUPPRESS_BEHAVIOR, lpBuffer, sizeof(int));

            Marshal.FreeCoTaskMem(lpBuffer);
        }
    }
}
