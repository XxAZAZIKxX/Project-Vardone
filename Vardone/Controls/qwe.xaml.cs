﻿using System;
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

namespace Vardone.Controls
{
    /// <summary>
    /// Interaction logic for qwe.xaml
    /// </summary>
    public partial class qwe : Page
    {
        public static string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static string directory = System.IO.Path.GetDirectoryName(path);
        public qwe()
        {
            InitializeComponent();

            ServersAvatar my = new ServersAvatar();
          //  MessageBox.Show(directory + @"\resources\va.ico");
            //my.image.Source = new BitmapImage(new Uri(@directory + @"\resources\va.ico"));

           // DataGSA.Items.Add(my);


        }
        public class ServersAvatar
        {
            public Image image { get; set; }
        }
    }
}
