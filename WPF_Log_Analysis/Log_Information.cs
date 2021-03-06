﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;



namespace WPF_Log_Analysis
{
    class Log_Information
    {
        List<System.Windows.Point> points = new List<System.Windows.Point>();
        Node head;
        public Log_Information()
        {
            head = new Node("Side Menu");
            head.count = 4;
        }

        public int run(string file, DateTime start, DateTime end)
        {
            System.IO.StreamReader fh = new System.IO.StreamReader(file);
            if (fh == null)
            {
                return -1;
            }
            string line;
            string[] array;
            while ((line = fh.ReadLine()) != null)
            {
                
                array = line.Split(';');
                if (array.Length < 2)
                {
                    continue;
                }

                DateTime time = Pull_Date(array[0]);
                if (start.CompareTo(time) > 0 || DateTime.Today.AddDays(1).CompareTo(time) == 0)
                {
                    continue;
                }
                if (end.CompareTo(time) < 0)
                {
                    return 1;
                }

                List<string> menus;
                if (array.Length == 4)//interacted with
                {

                    menus = Pull_Menu(array[2], array[3]);
                    if (menus == null)
                    {
                        continue;
                    }

                    head.Add(menus);


                    char[] delim = { ',', '=' };
                    string[] xy = array[1].Split(delim),y;
                    
                    delim[0]=' ';
                    y = xy[3].Split();
                    points.Add(new System.Windows.Point(Convert.ToDouble(xy[1]), Convert.ToDouble(y[0])));

                }
                if (array.Length == 2)//from history
                {
                    menus = from_History(array[1]);
                    if (menus == null)
                    {
                        continue;
                    }
                    // head.Add(menus);
                }


            }

            return 1;
        }



        /*return the date held in the sting if the date is of the form 
         * "Day/month/year hour:min:sec AM"
         * if the format is wrong tomorrows date is returned because no analysis will happen on tomorrws date
         */
        private DateTime Pull_Date(string date)
        {
            char[] array = { '/', ' ', ':' };
            string[] split = date.Split(array);
            if (split.Length == 7)
            {
                try
                {
                    if (split[6] == "AM")
                    {
                        return new DateTime(Convert.ToInt32(split[2]), Convert.ToInt32(split[1]), Convert.ToInt32(split[0]), Convert.ToInt32(split[3]), Convert.ToInt32(split[4]), Convert.ToInt32(split[5]));
                    }
                    else if (split[6] == "PM")
                    {
                        int hour=Convert.ToInt32(split[3]);
                        if(hour<12){
                            hour+=12;
                        }
                        return new DateTime(Convert.ToInt32(split[2]), Convert.ToInt32(split[1]), Convert.ToInt32(split[0]),hour , Convert.ToInt32(split[4]), Convert.ToInt32(split[5]));
                    }
                }
                catch (FormatException e)
                {
                    return DateTime.Today.AddDays(1);
                }
                return DateTime.Today.AddDays(1);
            }
            return DateTime.Today.AddDays(1);

        }

        //returns null if line is not to be used
        //returns array of strings each string is a level in the heirachy
        List<string> Pull_Menu(string menu, string square)
        {
            string[] splitMenu, splitSquare;
            char[] delim = new char[2];
            delim[0] = ':';
            splitMenu = menu.Split(delim);
            delim[1] = ',';
            splitSquare = square.Split(delim);

            if (splitMenu[0].Contains("TreeMenu"))
            {

                delim = new char[1];
                delim[0] = '/';
                splitMenu[1] = splitMenu[1].Trim();
                string[] treeMenu = splitMenu[1].Split(delim);
                List<string> hr = treeMenu.ToList();

                hr.Insert(hr.Count(), splitSquare[1].Trim());
                return hr;
            }//now it is a side menu
            else if (splitMenu[1].Contains("Animating"))
            {
                return null;//not an actual interaction
            }
            else
            {//actual selection
                List<string> hr = new List<string>();
                hr.Add(splitSquare[1].Trim());
                return hr;
            }
        }

        public List<string> from_History(string menu)
        {
            string[] splitMenu;
            char[] delim = { ':' };
            splitMenu = menu.Split(delim);
            splitMenu[2] = splitMenu[2].Trim();
            delim[0] = '/';
            splitMenu = splitMenu[2].Split(delim);
            return splitMenu.ToList();
        }

        public void print(string file)
        {
            
            System.IO.StreamWriter fh = new System.IO.StreamWriter(file);
            fh.WriteLine("Main Menu");
            head.print(1, fh);
            fh.Close();
        }
        public string GetInfo()
        {
            string Information="Main Menu\n";
            Information += head.GetInfo(1);
            return Information;

        }

        public DrawingImage Picture(double radi,double thickness)
        {

            GeometryGroup bGroup = new GeometryGroup();
            //bGroup.Children.Add(new RectangleGeometry(new Rect(540, 540, 860, 0)));
            bGroup.Children.Add( new LineGeometry(new Point(0, 540), new Point(1920, 540)));
            GeometryDrawing bDrawingP = new GeometryDrawing();
            bDrawingP.Geometry = bGroup;
            bDrawingP.Pen = new Pen(Brushes.Black, 1080);

            GeometryGroup rGroup = new GeometryGroup();
            rGroup.Children.Add(new RectangleGeometry(new Rect(10, 10, 1900, 1060)));
            GeometryDrawing rDrawingP = new GeometryDrawing();
            rDrawingP.Geometry = rGroup;
            rDrawingP.Pen = new Pen(Brushes.Blue, 20);

            GeometryGroup group=new GeometryGroup();
            foreach (Point p in points)
            {
                group.Children.Add(new EllipseGeometry(p, radi, radi));
            }
            GeometryDrawing gDrawingP = new GeometryDrawing();
            gDrawingP.Geometry = group;
            SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
            redBrush.Opacity = 1;
            

            gDrawingP.Pen = new Pen(redBrush,thickness);

            DrawingGroup dGroup=new DrawingGroup();
            dGroup.Children.Add(bDrawingP);//background
            dGroup.Children.Add(rDrawingP);//border
            dGroup.Children.Add(gDrawingP);//dots

            DrawingImage dImage = new DrawingImage();
            dImage.Drawing = dGroup;
           
            return dImage;
        }
       

    }
}
