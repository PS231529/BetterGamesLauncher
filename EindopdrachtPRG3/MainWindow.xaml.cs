using EindopdrachtPRG3.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EindopdrachtPRG3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameDB _GameDB = new GameDB();
        MySqlConnection _connection = new MySqlConnection("Server=localhost;Database=prg3_eindopdracht;Uid=root;Pwd=;");


        public MainWindow()
        {
            InitializeComponent();
            //FillDataGrid();

            myFunction();

        }

        private void SelectFavourite()
        {

            DataTable games = _GameDB.SelectFavourite();

            try
            {
                int x = 0;
                int y = 0;
                _connection.Open();

                Main.Children.Clear();

                for (int i = 0; i < games.Rows.Count; i++)
                {



                    MySqlCommand command = _connection.CreateCommand();

                    command.CommandText = "SELECT * FROM games WHERE favourite = 1";

                    MySqlDataReader reader = command.ExecuteReader();
                    games.Load(reader);

                    byte[] bytes = (byte[])games.Rows[i]["Image"];

                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();

                    Stream stream = new MemoryStream(bytes);
                    bitmap.StreamSource = stream;

                    bitmap.EndInit();

                    Button btn = new Button();

                    btn.Content = games.Rows[i]["game"];
                    btn.Height = 125;
                    btn.Width = 100;
                    btn.Background = new ImageBrush(bitmap);
                    btn.Tag = games.Rows[i]["directory"];

                    btn.Click += new RoutedEventHandler(btn_Click);

                    //if we are at the end of a row
                    if (i % 5 == 0 && i != 0)
                    {
                        x++;
                        y = 0;
                    }
                    Grid.SetRow(btn, x);
                    Grid.SetColumn(btn, y);
                    Main.Children.Add(btn);

                    y++;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _connection.Close();
            }

        }

        private void myFunction()
        {

            DataTable games = _GameDB.SelectStudents();

            try
            {
                int x = 0;
                int y = 0;
                _connection.Open();
                for (int i = 0; i < games.Rows.Count; i++)
                {



                    MySqlCommand command = _connection.CreateCommand();

                    command.CommandText = "SELECT * FROM games";

                    MySqlDataReader reader = command.ExecuteReader();
                    games.Load(reader);

                    byte[] bytes = (byte[])games.Rows[i]["Image"];

                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();

                    Stream stream = new MemoryStream(bytes);
                    bitmap.StreamSource = stream;

                    bitmap.EndInit();

                    Button btn = new Button();
                    Button btn2 = new Button();

                    btn.Content = games.Rows[i]["game"];
                    btn.Height = 125;
                    btn.Width = 100;
                    btn.BorderBrush = Brushes.Transparent;
                    btn.Background = new ImageBrush(bitmap);
                    btn.Tag = games.Rows[i]["directory"];


                    btn2.Height = 25;
                    btn2.Width = 25;

                    BitmapImage btm = new BitmapImage(new Uri("/Assets/favorite.png", UriKind.Relative));
                    Image img = new Image();
                    img.Source = btm;
                    img.Stretch = Stretch.Fill;
                    btn2.Content = img;
                    btn2.Background = Brushes.Transparent;
                    btn2.BorderBrush = Brushes.Transparent;
                    btn2.VerticalAlignment = VerticalAlignment.Top;
                    btn2.HorizontalAlignment = HorizontalAlignment.Right;
                    //btn2.Tag = games.Rows[i]["favourite"];
                    btn2.Tag = games.Rows[i]["id"];

                    //get the value of the favourite column
                    int favourite = Convert.ToInt32(games.Rows[i]["favourite"]);
                    // if the value is 1, then the game is favourite and we need to change the image
                    if (favourite == 1)
                    {
                        btm = new BitmapImage(new Uri("/Assets/star.png", UriKind.Relative));
                        img = new Image();
                        img.Source = btm;
                        img.Stretch = Stretch.Fill;
                        btn2.Content = img;
                    }

                    btn.Click += new RoutedEventHandler(btn_Click);
                    btn2.Click += new RoutedEventHandler(btn2_Click);



                    //if we are at the end of a row
                    if (i % 5 == 0 && i != 0)
                    {
                        x++;
                        y = 0;
                    }
                    Grid.SetRow(btn, x);
                    Grid.SetColumn(btn, y);
                    Grid.SetRow(btn2, x);
                    Grid.SetColumn(btn2, y);
                    Main.Children.Add(btn);
                    Main.Children.Add(btn2);
                    y++;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _connection.Close();
            }

        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button test = (Button)sender;
            System.Diagnostics.Process.Start(test.Tag.ToString());
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            DataTable games = _GameDB.SelectStudents();
            
            Button test = (Button)sender;


            int id = Convert.ToInt32(test.Tag);

            _GameDB.UpdateFavourite(id);

            for (int i = 0; i < games.Rows.Count; i++)
            {
                int favourite = Convert.ToInt32(games.Rows[i]["favourite"]);

                //if backgroundimage is star.png , then change to favorite.png
                if (favourite == 1)
                {
                    BitmapImage btm = new BitmapImage(new Uri("/Assets/favorite.png", UriKind.Relative));
                    Image img = new Image();
                    img.Source = btm;
                    img.Stretch = Stretch.Fill;
                    test.Content = img;
                }

                if (favourite == 0)
                {
                    BitmapImage btm = new BitmapImage(new Uri("/Assets/star.png", UriKind.Relative));
                    Image img = new Image();
                    img.Source = btm;
                    img.Stretch = Stretch.Fill;
                    test.Content = img;
                }
            }


        }

        private void Button_All(object sender, RoutedEventArgs e)
        {
            myFunction();
        }

        private void Button_Fav(object sender, RoutedEventArgs e)
        {
            SelectFavourite();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            string search = searchBox.Text;
            
            DataTable games = _GameDB.ZoekStudents(search);

            Main.Children.Clear();

            try
            {
                int x = 0;
                int y = 0;

                _connection.Open();



                for (int i = 0; i < games.Rows.Count; i++)
                {

                    MySqlCommand command = _connection.CreateCommand();

                    command.CommandText = "SELECT * FROM games WHERE game LIKE '%" + search + "%'";

                    MySqlDataReader reader = command.ExecuteReader();
                    games.Load(reader);

                    byte[] bytes = (byte[])games.Rows[i]["Image"];

                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();

                    Stream stream = new MemoryStream(bytes);
                    bitmap.StreamSource = stream;

                    bitmap.EndInit();

                    Button btn = new Button();

                    btn.Content = games.Rows[i]["game"];
                    btn.Height = 125;
                    btn.Width = 100;
                    btn.BorderBrush = Brushes.Transparent;
                    btn.Background = new ImageBrush(bitmap);
                    btn.Tag = games.Rows[i]["directory"];

                    btn.Content = games.Rows[i]["game"];

                    if (i % 5 == 0 && i != 0)
                    {
                        x++;
                        y = 0;
                    }
                    Grid.SetRow(btn, x);
                    Grid.SetColumn(btn, y);
                    Main.Children.Add(btn);
                    y++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _connection.Close();
            }

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}    




