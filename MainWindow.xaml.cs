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
using Microsoft.Win32;

namespace LCEOptionsEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow() { InitializeComponent(); this.Title = $"{App.Name}: Select a file"; }

        private void ChooseFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                new OptionsWindow(path, int.Parse(OffsetTextBox.Text)).Show();
                this.Close();
            }
        }
        
        private void OffsetTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}
