using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EindopdrachtPRG3.Classes
{
    class GameDB
    {
        #region fields
        MySqlConnection _connection = new MySqlConnection("Server=localhost;Database=prg3_eindopdracht;Uid=root;Pwd=;");
        #endregion

        #region methods/functions
        public DataTable SelectStudents()
        {
            DataTable result = new DataTable();
            try
            {
                _connection.Open();
                MySqlCommand command = _connection.CreateCommand();
                command.CommandText = "SELECT * FROM games;";
                MySqlDataReader reader = command.ExecuteReader();
                result.Load(reader);
            }
            catch (Exception)
            {
                //Problem with the database
            }
            finally
            {
                _connection.Close();
            }
            return result;
        }

        public DataTable ZoekStudents(string search)
        {
            DataTable result = new DataTable();
            try
            {
                _connection.Open();
                MySqlCommand command = _connection.CreateCommand();
                command.CommandText = "SELECT * FROM games where game like '%" + search + "%'";
                MySqlDataReader reader = command.ExecuteReader();
                result.Load(reader);
            }
            catch (Exception)
            {
                //Problem with the database
            }
            finally
            {
                _connection.Close();
            }
            return result;
        }

        public DataTable SelectFavourite()
        {
            
            DataTable result = new DataTable();
            try
            {
                _connection.Open();
                MySqlCommand command = _connection.CreateCommand();
                command.CommandText = "SELECT * FROM games WHERE favourite = '1';";
                MySqlDataReader reader = command.ExecuteReader();
                result.Load(reader);
            }
            catch (Exception)
            {
                //Problem with the database
            }
            finally
            {
                _connection.Close();
            }
            return result;
        }

        public void UpdateFavourite(int id)
        {
            try
            {
                _connection.Open();
                MySqlCommand command = _connection.CreateCommand();
                command.CommandText = "UPDATE games SET favourite = !favourite WHERE id = @id ";
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteReader();
            }
            catch (Exception)
            {
                //Problem with the database
            }
            finally
            {
                _connection.Close();
            }
        }
        #endregion
    }

}
