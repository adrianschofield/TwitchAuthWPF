using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TwitchAuthWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Start my MainWindow here so that I have a reference to it.
            Window myMain = new MainWindow();

            //Show the MainWindow
            Application.Current.MainWindow = myMain;
            Application.Current.MainWindow.Show();

            //Start the actual work, create the object and update the main property
            //so we can update the TextBox with data

            Program myProgram = new Program();
            myProgram.main = (TwitchAuthWPF.MainWindow)myMain;

            //Start the actual work using a Task

            Task.Run(() => myProgram.DoWork());
            
        }
    }
}
