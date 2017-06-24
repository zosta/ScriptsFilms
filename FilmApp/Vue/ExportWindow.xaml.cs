using FilmApp.Model;
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

namespace WpfScriptFilms.Vue
{
    /// <summary>
    /// Logique d'interaction pour ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        public ExportWindow()
        {
            InitializeComponent();
        }

        private void btn_export_html_Click_1(object sender, RoutedEventArgs e)
        {
            //Bibliotheque.Instance.exporterListeFilms("html");

            Bibliotheque.Instance.exporterListeFilms();
        }

        private void btn_export_txt_Click(object sender, RoutedEventArgs e)
        {
            Bibliotheque.Instance.exporterListeFilms();
        }
    }
}
