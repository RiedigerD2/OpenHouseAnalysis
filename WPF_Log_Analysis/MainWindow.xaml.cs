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
using System.ComponentModel;
using System.IO;

namespace WPF_Log_Analysis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.OpenFileDialog dialog;
        System.Windows.Forms.SaveFileDialog saveDialogT;
        System.Windows.Forms.SaveFileDialog saveDialogP;
        Log_Information info;
        DateTime startDate;
        DateTime endDate;
        string longFile, shortFile;
        private static BackgroundWorker worker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();

            dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.FileOk += new System.ComponentModel.CancelEventHandler(dialog_FileOk);

            saveDialogT = new System.Windows.Forms.SaveFileDialog();
            saveDialogP = new System.Windows.Forms.SaveFileDialog();

            saveDialogT.FileOk += new CancelEventHandler(saveDialogT_FileOk);
            saveDialogT.Filter = "Text file (.txt)|*.txt";
            saveDialogP.FileOk += new CancelEventHandler(saveDialogP_FileOk);
            saveDialogP.Filter = "PNG Image (.png)|*.png";

            worker.DoWork += Analyse_file;
            worker.RunWorkerCompleted += WorkerCompleted;

            worker.WorkerSupportsCancellation = true;

        }

        void saveDialogP_FileOk(object sender, CancelEventArgs e)
        {
            Window wind = new Window();
            Image image2 = new Image();
            wind.Width = 1000;
            wind.Height = 600;
            image2.HorizontalAlignment = HorizontalAlignment.Left;
            image2.VerticalAlignment = VerticalAlignment.Top;
            image2.Width = 960;
            image2.Height = 540;
            image2.Source = info.Picture();
            wind.Content = image2;
            wind.Show();
           
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(1000,
                                                                               560,
                                                                               100, 100, PixelFormats.Default);
               
                renderTargetBitmap.Render(image2);


                PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
                pngBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                using (FileStream fileStream = new FileStream(saveDialogP.FileName, FileMode.Create))
                {
                    pngBitmapEncoder.Save(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }
               
                wind.Close();
        }

        void saveDialogT_FileOk(object sender, CancelEventArgs e)
        {

            info.print(saveDialogT.FileName);
        }

        void dialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            fileText.Text = dialog.FileName;
            longFile = shortFile = null;//now user can't save old data
            SavePictureButton.Visibility = Visibility.Hidden;
            SaveTextButton.Visibility = Visibility.Hidden;
            image.Visibility = Visibility.Hidden;
            TextOutPut.Visibility = Visibility.Hidden;
            Process.Content = null;
            scrolly.Visibility = Visibility.Hidden;
        }


        private void Browse_Click(object sender, RoutedEventArgs e)
        {
           
            dialog.ShowDialog();
        }

        private void Analyse_Click(object sender, RoutedEventArgs e)
        {
            Date_error.Content = null;
            File_error.Content = null;
            info = new Log_Information();
            SavePictureButton.Visibility = Visibility.Hidden;
            SaveTextButton.Visibility = Visibility.Hidden;
            image.Visibility = Visibility.Hidden;
            TextOutPut.Visibility = Visibility.Hidden;
            scrolly.Visibility = Visibility.Hidden;
            Process.Content = null;

            if (dialog.FileName == "")
            {
                File_error.Content = "Choose a file";
                return;
            }
            if (datePicker_Start.SelectedDate.HasValue && datePicker_End.SelectedDate.HasValue)
            {
                startDate = datePicker_Start.SelectedDate.Value;
                endDate = datePicker_End.SelectedDate.Value;
                if (startDate.CompareTo(DateTime.Today) > 0 || endDate.CompareTo(DateTime.Today) > 0)
                {
                    Date_error.Content = "Dates must be before todays date";
                    return;
                }
                if (startDate.CompareTo(endDate) < 0)
                {
                    Process.Content = "Processing...";
                    UpdateLayout();
                    longFile = dialog.FileName;
                    shortFile = dialog.SafeFileName;
                    if (worker.IsBusy)
                    {
                        worker.CancelAsync();
                    }
                    worker.RunWorkerAsync();
                    return;
                }
                else
                {
                    Date_error.Content = "Start date must be before end date";
                    return;
                }
            }
            else
            {
                Date_error.Content = "Please enter start and end date.";
                return;
            }
        }
        void Analyse_file(object sender, DoWorkEventArgs e)
        {
            e.Result = info.run(longFile, startDate, endDate);
        }
        void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((int)e.Result < 0)
            {
                return;
            }
            Process.Content = "Done!";
            TextOutPut.Text = info.GetInfo();
            image.Source = info.Picture();

            TextOutPut.Visibility = Visibility.Visible;
            image.Visibility = Visibility.Visible;
            SavePictureButton.Visibility = Visibility.Visible;
            SaveTextButton.Visibility = Visibility.Visible;
            scrolly.Visibility = Visibility.Visible;

            
            return;

        }
        private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            DateLabel.Content = datePicker_Start.SelectedDate.ToString() + " -- " + datePicker_End.SelectedDate.ToString();
            SavePictureButton.Visibility = Visibility.Hidden;
            SaveTextButton.Visibility = Visibility.Hidden;
            image.Visibility = Visibility.Hidden;
            TextOutPut.Visibility = Visibility.Hidden;
            scrolly.Visibility = Visibility.Hidden;
            Process.Content = null;
        }

        private void SavePicture_Click(object sender, RoutedEventArgs e)
        {
            saveDialogP.ShowDialog();

        }
      

        private void SaveText_Click(object sender, RoutedEventArgs e)
        {
            saveDialogT.ShowDialog();           
        }
    }
}
