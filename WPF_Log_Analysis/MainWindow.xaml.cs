using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Log_Analysis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.OpenFileDialog dialog;
        Log_Information info;
        public MainWindow()
        {
            InitializeComponent();
            info = new Log_Information();
            dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.FileOk += new System.ComponentModel.CancelEventHandler(dialog_FileOk);
        }

        void dialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            fileText.Text = dialog.FileName;
           
        }


        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            dialog.ShowDialog();
        }

        private void Analyse_Click(object sender, RoutedEventArgs e)
        {
            Date_error.Content = null;
            File_error.Content = null;

          

            if (dialog.FileName == "")
            {
                File_error.Content = "Choose a file";
                return;
            }
            if (datePicker_Start.SelectedDate.HasValue && datePicker_End.SelectedDate.HasValue)
            {
                DateTime startDate = datePicker_Start.SelectedDate.Value;
                DateTime endDate = datePicker_End.SelectedDate.Value;
                if (startDate.CompareTo(endDate) < 0)
                {   
                    Process.Content="Processing...";
                    UpdateLayout();
                    info.run(dialog.FileName, startDate, endDate);
                    Process.Content = "Done!";
                    
                   
                    info.print(dialog.FileName.Remove(dialog.FileName.Length - dialog.SafeFileName.Length)+"Log_Analysis_"+startDate.ToLongDateString()+"--"+endDate.ToLongDateString()+".txt");


                    image.Source = info.Picture();
                }
                else
                {
                    Date_error.Content = "Start date must be before end date";
                }
            }
            else
            {
                Date_error.Content = "Please enter start and end date.";
            }

        }

        private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime startDate=new DateTime(), endDate;
            if(datePicker_Start.SelectedDate.HasValue)
                 startDate = datePicker_Start.SelectedDate.Value;
            if(datePicker_End.SelectedDate.HasValue)
                endDate = datePicker_End.SelectedDate.Value;
            // DateTime time = startDate.Value;
            DateLabel.Content = datePicker_Start.SelectedDate.ToString() + " -- " + datePicker_End.SelectedDate.ToString();
            DateTime comparble = new DateTime(2014,01,30,23,0,0);//year month day hour minute second
            
        }
    }
}
