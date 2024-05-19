using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using Online_Library.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Library.ViewModel
{
    
    internal partial class MainWindow_ViewModel : ObservableObject,INotifyPropertyChanged
    {

        [ObservableProperty]
        ObservableCollection<MainWindow_Model> list;

        [ObservableProperty]
        MainWindow_Model item;

        [RelayCommand]
        void Add()
        {
            if (Item == null)
                return;

            List.Add(Item);

        }

        MainWindow_ViewModel() 
        {
            list = new ObservableCollection<MainWindow_Model>();
            item = new MainWindow_Model();
        }

        string server = "localhost";
        string database = "online_library";
        string uid = "root";
        string password = "love4904@";
        

        public void SqlConnection()
        {
            string connectionString = $"server={server};uid=root;pwd={password};database={database}";
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            mySqlConnection.Open();
            string query = "select * from student";
            MySqlCommand mySqlCommand = new MySqlCommand(query,mySqlConnection);
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
        }

    }
}
