using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscoket.Models
{
    public static class ConnectToDataBase
    {
        private static string myConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static int CheckMacAdressIsTrackableOrNot(string MacAddress)
        {
            int returnValue = 0;
            using (MySqlConnection myConnection = new MySqlConnection(myConnectionString))
            {
                myConnection.Open();
                MySqlCommand myCommand = myConnection.CreateCommand();
                // Start a local transaction
                myCommand.Connection = myConnection;
                MySqlDataReader dataReader;
                try
                {
                    myCommand.CommandText = "select count(Id) as Count from DeviceAssociateSite as devasssite" + " " +
                                             "join Device as dev" + " " +
                                             "on devasssite.DeviceId = dev.DeviceId" + " " +
                                             "where dev.MacAddress='" + MacAddress + "'" + " " + "and" + " " + "devasssite.IsTrackByRtls=" + true;
                    dataReader = myCommand.ExecuteReader();
                    while (dataReader.Read())
                    {
                        returnValue = Convert.ToInt32(dataReader["Count"]);
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    myConnection.Close();
                }
                return returnValue;
            }
        }
    }
}

