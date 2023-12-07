using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using MySql.Data;
using MySql.Data.MySqlClient;

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
    public static UserInfo UserInfo { get; set; }

    private static MySqlConnection _sqlConnection;
    private static MySqlDataReader _sqlReader;

    private string _connectionStringPath;

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

        _connectionStringPath = Application.dataPath + Path.DirectorySeparatorChar + "Database";
        if (!TryCreateConnectionString(_connectionStringPath, out string connectionInfo)) return;
        try
        {
            if (string.IsNullOrEmpty(connectionInfo))
            {
                Debug.Log("Error - Empty Connection String");
                return;
            }

            _sqlConnection = new MySqlConnection(connectionInfo);
            
            if (!TryOpen(_sqlConnection))
            {
                Debug.Log($"Error - Failed to open connection.\n{connectionInfo}");
                return;
            }

            Debug.Log($"Connection Success.\n{connectionInfo}");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
    }

    private bool TryCreateConnectionString(string path, out string connectionString)
    {
        connectionString = string.Empty;

        // When file not found. create initial directory.
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(path);
            string jsonString = File.ReadAllText(path + Path.DirectorySeparatorChar + "config.json");

            JsonData itemData = JsonMapper.ToObject(jsonString);
            connectionString = $"Server={itemData[0]["IP"]};" +
                                      $"Database={itemData[0]["TableName"]};" +
                                      $"Uid={itemData[0]["ID"]};" +
                                      $"Pwd={itemData[0]["PW"]};" +
                                      $"Port={itemData[0]["PORT"]};" +
                                      $"CharSet=utf8;";
            return true;
        }

        return false;
    }

    private static bool TryOpen(MySqlConnection connection)
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();

            if (connection.State != ConnectionState.Open)
                return false;
        }

        return true;
    }

    public static bool LogIn(string id, string password)
    {
        try
        {
            if (!TryOpen(_sqlConnection))
                return false;

            string query = @$"SELECT User_Name, User_Password FROM user_info
                              WHERE User_Name = '{id}' AND User_Password = '{password}';";

            using (MySqlCommand cmd = new MySqlCommand(query, _sqlConnection))
            using (_sqlReader = cmd.ExecuteReader())
            {
                _sqlReader = cmd.ExecuteReader();
                if (_sqlReader.HasRows)
                {
                    while (_sqlReader.Read())
                    {
                        string readId = _sqlReader.IsDBNull(0) ? string.Empty : _sqlReader["User_Name"].ToString();
                        string readPassword = _sqlReader.IsDBNull(0) ? string.Empty : _sqlReader["User_Password"].ToString();

                        if (string.IsNullOrEmpty(readId) || string.IsNullOrEmpty(readPassword))
                            break;

                        UserInfo = new UserInfo(readId, readPassword);

                        if (!_sqlReader.IsClosed)
                            _sqlReader.Close();

                        return true;
                    }
                }

                if (!_sqlReader.IsClosed)
                    _sqlReader.Close();

                return false;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);

            if (!_sqlReader.IsClosed)
                _sqlReader.Close();

            return false;
        }
    }
}
