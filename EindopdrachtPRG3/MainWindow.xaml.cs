using EindopdrachtPRG3.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace EindopdrachtPRG3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameDB _GameDB = new GameDB();

        MySqlConnection _connection = new MySqlConnection("Server=localhost;Database=BetterGamesLauncher;Uid=root;Pwd=;");
        MySqlConnection _checkconnection = new MySqlConnection("Server=localhost;Database=information_schema;Uid=root;Pwd=;");


        public MainWindow()
        {
            InitializeComponent();
            //FillDataGrid();
            bool dbexists = CheckDatabaseExists(_checkconnection, "bettergameslauncher");
            MessageBox.Show(dbexists.ToString());
            myFunction();
            uitlezen();
        }

        #region OnStartup
        static void createDatabase(string dbname)
        {
            MySqlConnection mainConn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;");

            using (mainConn)
            {
                using (MySqlCommand _maincommand = new MySqlCommand("CREATE DATABASE IF NOT EXISTS " + dbname + ";", mainConn))
                {
                    mainConn.Open();
                    _maincommand.ExecuteScalar();
                    mainConn.Close();
                }
            }
            migrateTables();
        }

        static void migrateTables()
        {
            MySqlConnection _connection = new MySqlConnection("Server=localhost;Database=BetterGamesLauncher;Uid=root;Pwd=;");
            //beautiful code
            using (_connection)
            {
                using (MySqlCommand _maincommand = new MySqlCommand("CREATE TABLE `games` (`id` INT(12) NULL AUTO_INCREMENT,`game` VARCHAR(255) NULL,`description` VARCHAR(535) NULL DEFAULT NULL,`image` BLOB NOT NULL,`directory` VARCHAR(255) NOT NULL,`favourite` INT(1) NOT NULL DEFAULT '0',PRIMARY KEY(`id`)) ENGINE = InnoDB; ", _connection))
                {
                    _connection.Open();
                    _maincommand.ExecuteScalar();
                    _connection.Close();
                }
            }

        }

        private static bool CheckDatabaseExists(MySqlConnection tmpConn, string databaseName)
        {
            string sqlCreateDBQuery;
            bool result = false;


            try
            {

                sqlCreateDBQuery = string.Format("SELECT SCHEMA_NAME FROM SCHEMATA WHERE SCHEMA_NAME = '{0}'", databaseName);

                using (tmpConn)
                {
                    using (MySqlCommand sqlCmd = new MySqlCommand(sqlCreateDBQuery, tmpConn))
                    {
                        tmpConn.Open();

                        object resultObj = sqlCmd.ExecuteScalar();

                        if (resultObj == null)
                        {

                            result = true;
                            createDatabase("bettergameslauncher");
                        }

                        tmpConn.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        #endregion

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

        private void SelectExe(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Executable"; // Default file name
            dialog.DefaultExt = ".exe"; // Default file extension
            dialog.Filter = "Executables (.exe)|*.exe"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
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

        private void uitlezen()
        {
            //string[] dirs = Directory.GetFiles("C:\\Program Files (x86)\\Steam\\steamapps\\common", "*.*", SearchOption.TopDirectoryOnly);
            //foreach (string dir in dirs)
            //{
            //    MessageBox.Show(dirs.ToString());
            //}

            var files = Directory.GetDirectories("C:\\Program Files (x86)\\Steam\\steamapps\\common", "*.*", SearchOption.TopDirectoryOnly);
           
            int xindex = 0;
            int yindex = 0;
            var rsrc = this.FindResource("GameContainerStyle") as Style;
            foreach (string file in files)
            {
                if (yindex == 3) { return; }
                
                //MessageBox.Show(file.ToString());
                var map = file.ToString().Split('\\');
                StackPanel gamens = new StackPanel();
                gamens.Style = rsrc;
                //gamens onclick run function
                string balls = map[5].ToString();
                gamens.MouseLeftButtonDown += new MouseButtonEventHandler(getExe);
                gamens.Tag = balls;


                Border border = new Border();
                TextBlock textBlock = new TextBlock();

<<<<<<< HEAD
                textBlock.Text = map[5];
=======
                if (map[5].Length >= 20) {
                    textBlock.Text = map[5].Substring(0, 20) + "...";
                }
                else
                {
                    textBlock.Text = map[5];
                }

>>>>>>> 9c4b73f9adb6ede24fdf037f1832021b1d4dd058
                gamens.Children.Add(border);
                gamens.Children.Add(textBlock);
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(gamens, 0+yindex);
                Grid.SetColumn(gamens, 0 + xindex);
                Main.Children.Add(gamens);
                xindex++;
<<<<<<< HEAD


=======
>>>>>>> 9c4b73f9adb6ede24fdf037f1832021b1d4dd058

                if (xindex == 5)
                {
                    xindex = 0;
                    yindex++;
                }
            }
        }
        
        private void getExe(object sender, MouseButtonEventArgs e)
        {
            var gamedir = ((StackPanel)sender).Tag.ToString();
            var path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\" + ((StackPanel)sender).Tag.ToString();
            
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                if (file.Contains(".exe"))
                {
                    MessageBox.Show(file);
                    launchGame(file.ToString());
                }
            }
            
        }

        private void launchGame(string exepath)
        {
            Process process = Process.Start(exepath);
            int id = process.Id;
            Process tempProc = Process.GetProcessById(id);
            tempProc.WaitForExit();
        }

    }
}




