using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace NetworkRoom
{
    public class UserInfo
    {
        public string ID { get; private set; }
        public string Password { get; private set; }

        public UserInfo(string id, string password)
        {
            ID = id;
            Password = password;
        }
    }

    public class SQLManager : MonoBehaviour
    {
        public UserInfo userInfo;

        public MySqlConnection connection;
        public MySqlDataReader reader;

        private string path;

        public static SQLManager Instance { get; private set; } = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

#if !UNITY_SERVER
            path = Application.dataPath + Path.DirectorySeparatorChar + "Database";
            string connectionInfo = SetConnectionString(path);
            try
            {
                if (string.IsNullOrEmpty(connectionInfo))
                {
                    Debug.Log("[SQLManager] Connection String Error");
                    return;
                }

                connection = new MySqlConnection(connectionInfo);
                connection.Open();
                Debug.Log($"[SQLManager] Successfully Connected.\n{connectionInfo}");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
#endif
        }

        private string SetConnectionString(string path)
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
                string jsonString = File.ReadAllText(path + Path.DirectorySeparatorChar + "config.json");

                JsonData itemData = JsonMapper.ToObject(jsonString);
                string connectionString = $"Server={itemData[0]["IP"]};" +
                                          $"Database={itemData[0]["TableName"]};" +
                                          $"Uid={itemData[0]["ID"]};" +
                                          $"Pwd={itemData[0]["PW"]};" +
                                          $"Port={itemData[0]["PORT"]};" +
                                          $"CharSet=utf8;";
                return connectionString;
            }

            return null;
        }

        private bool IsValidConnection(MySqlConnection connection)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();

                if (connection.State != System.Data.ConnectionState.Open)
                    return false;
            }

            return true;
        }

        public bool LogIn(string id, string password)
        {
            try
            {
                if (!IsValidConnection(connection))
                    return false;

                string query = @$"SELECT User_Name, User_Password FROM user_info
                                  WHERE User_Name = '{id}' AND User_Password = '{password}';";

                MySqlCommand cmd = new MySqlCommand(query, connection);

                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string readId = reader.IsDBNull(0) ? string.Empty : reader["User_Name"].ToString();
                        string readPassword = reader.IsDBNull(0) ? string.Empty : reader["User_Password"].ToString();

                        if (string.IsNullOrEmpty(readId) || string.IsNullOrEmpty(readPassword))
                            break;

                        userInfo = new UserInfo(readId, readPassword);

                        if (!reader.IsClosed)
                            reader.Close();

                        return true;
                    }
                }

                if (!reader.IsClosed)
                    reader.Close();

                return false;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);

                if (!reader.IsClosed)
                    reader.Close();

                return false;
            }
        }
    }
}
