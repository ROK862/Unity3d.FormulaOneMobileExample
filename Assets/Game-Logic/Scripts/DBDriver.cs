using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine.UI;
using System.Globalization;
using System.IO;

public class DBDriver : MonoBehaviour
{
    [SerializeField]
    public static decimal ExperiancePoints;
    public static decimal GoldCoin;
    public static decimal SilverCoin;

    private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    private IDataReader reader;
    public InputField teamName;
    public static Image loadingImage;

    public DBPlayerProfile playerProfile;


    string DatabaseName = "unity_formula_one_racer.db";
    // Start is called before the first frame update
    private void Start()
    {
        //Application database Path android
        string filepath = Application.persistentDataPath + "/" + DatabaseName;
        if (!File.Exists(filepath))
        {
            // If not found on android will create Tables and databas
            Debug.LogWarning("File \"" + filepath + "\" does not exist. Attempting to create from \"" +
                             Application.dataPath + "!/assets/unity_formula_one_racer");
            // UNITY_ANDROID
            WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/unity_formula_one_racer.db");
            while (!loadDB.isDone) { }
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDB.bytes);
        }

        conn = "URI=file:" + filepath;

        Debug.Log("Stablishing connection to: " + conn);
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        string query;
        query = "CREATE TABLE finance_module (ID    INTEGER,Currency  TEXT UNIQUE,Value NUMERIC,PRIMARY KEY(ID AUTOINCREMENT))";
        try
        {
            dbcmd = dbconn.CreateCommand(); // create empty command
            dbcmd.CommandText = query; // fill the command
            reader = dbcmd.ExecuteReader(); // execute command which returns a reader
            query = "INSERT INTO 'main'.'finance_module'('ID', 'Currency', 'Value') VALUES('1', 'SilverCoin', '10000')";
            dbcmd = dbconn.CreateCommand(); // create empty command
            dbcmd.CommandText = query; // fill the command
            reader = dbcmd.ExecuteReader(); // execute command which returns a reader
            query = "INSERT INTO 'main'.'finance_module'('ID', 'Currency', 'Value') VALUES('2', 'GoldCoin', '50')";
            dbcmd = dbconn.CreateCommand(); // create empty command
            dbcmd.CommandText = query; // fill the command
            reader = dbcmd.ExecuteReader(); // execute command which returns a reader
            query = "INSERT INTO 'main'.'finance_module'('ID', 'Currency', 'Value') VALUES('2', 'ExperiancePoints', '5')";
            dbcmd = dbconn.CreateCommand(); // create empty command
            dbcmd.CommandText = query; // fill the command
            reader = dbcmd.ExecuteReader(); // execute command which returns a reader
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        InitializeTables();
    }
    // Start is called before the first frame update
    void GetCurrency()
    {
        string conn = "URI=file:" + Application.persistentDataPath + "/unity_formula_one_racer.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT Currency, Value FROM finance_module;";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            string currencyType = reader.GetString(0);
            decimal currencyValue = reader.GetDecimal(1);

            if (currencyType == "SilverCoin")
            {
                DBDriver.SilverCoin = currencyValue;
            }
            else if (currencyType == "GoldCoin")
            {
                DBDriver.GoldCoin = currencyValue;
            } else if (currencyType == "ExperiancePoints") {
                DBDriver.ExperiancePoints = currencyValue;
            }
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public static List<UIDriverIDCard> GetDrivers()
    {
        List<UIDriverIDCard> drivers = new List<UIDriverIDCard>();
        string conn = "URI=file:" + Application.persistentDataPath + "/unity_formula_one_racer.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM driver_register;";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int driverID = reader.GetInt32(0);
            string driverName = reader.GetString(1);
            int driverExperience = reader.GetInt32(2);
            int driverRaceCraft = reader.GetInt32(3);
            int driverAwareness = reader.GetInt32(4);
            int driverPace = reader.GetInt32(5);
            string driverTeam = reader.GetString(6);
            double driverPrice = reader.GetDouble(7);
            double driverSalary = reader.GetDouble(8);
            double driverBonus = reader.GetDouble(9);

            drivers.Add(new UIDriverIDCard(driverID, driverName, driverExperience, driverRaceCraft, driverAwareness, driverPace, driverTeam, driverPrice, driverSalary, driverBonus));
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        return drivers;
    }

    static void Execute(string query) {
        if (loadingImage != null)
            loadingImage.gameObject.SetActive(true);
        try
        {
            string conn = "URI=file:" + Application.persistentDataPath + "/unity_formula_one_racer.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = query;
            dbcmd.ExecuteReader();
            dbconn.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        if (loadingImage != null)
            loadingImage.gameObject.SetActive(false);
    }


    public static DBPlayerProfile GetPlayerProfile() {
        string conn = "URI=file:" + Application.persistentDataPath + "/unity_formula_one_racer.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM player_profile WHERE ID = '1';";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        DBPlayerProfile profile = new DBPlayerProfile();
        while (reader.Read())
        {
            //( string _GoogleAccount, string _FacebookAccount, string _ImageURL, string _BillableAccount, string _AccountKey, string _AccountSecret)
            profile = new DBPlayerProfile(
                reader.GetInt32(0), 
                reader.GetString(1), 
                reader.GetString(2), 
                reader.GetString(3), 
                reader.GetString(4), 
                reader.GetDouble(5), 
                reader.GetDouble(6),
                reader.GetString(7),
                reader.GetString(8),
                reader.GetString(9),
                reader.GetString(10),
                reader.GetString(11),
                reader.GetString(12));
            break;
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        return profile;
    }
    static void InitializeTables()
    {
        try
        {
            Execute("DROP TABLE driver_register;");
            Execute("CREATE TABLE 'driver_register' ('ID'	INTEGER,'DriveName'	TEXT NOT NULL UNIQUE,'Experiance'	INTEGER,'RaceCraft'	INTEGER,'Awareness'	INTEGER,'Pace'	INTEGER,'Team'	INTEGER,'MarketPrice'	NUMERIC,'MarketSalary'	NUMERIC,'MarketBonus'	NUMERIC,PRIMARY KEY('ID' AUTOINCREMENT))");
            Execute("CREATE TABLE 'player_profile' ('ID'	INTEGER,'FirstName'	TEXT,'LastName'	TEXT,'TeamName'	TEXT UNIQUE,'LastAccessed'	TEXT,'AccessCount'	INTEGER,'AverageGameplayDuration'	NUMERIC,'GoogleAccount'	TEXT,'FacebookAccount'	TEXT,'ImageURL'	TEXT,'BillableAccount'	INTEGER,'AccountKey'	TEXT,'AccountSecret'	TEXT,PRIMARY KEY('ID' AUTOINCREMENT))");
            Execute("CREATE TABLE 'myteam_drivers' ('ID'	INTEGER,'DriverName'	TEXT,'DriverBonus'	INTEGER,'DriverSalary'	NUMERIC,'DriverContructCost'	NUMERIC,'DriverContractEnd'	TEXT,'DriverContructStatus'	TEXT DEFAULT 'released','DriverWins'	INTEGER,PRIMARY KEY('ID' AUTOINCREMENT))");
        }
        catch (Exception ex)
        {
            Debug.Log("driver_register already exist!");
        }

        try
        {
            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('1', 'Lewis Hamilton', '89', '93', '95', '96', 'Mercedes-AMG Petronas', '19000000', '13000000', '6500000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('2', 'Max Verstappen', '66', '94', '85', '96', 'Red Bull', '10500000', '7000000', '3500000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('3', 'Mick Schumacher', '76', '85', '91', '89', 'HAAS', '6500000', '4350000', '1850000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('4', 'Antonio Giovinazzi', '49', '73', '71', '79', 'Alfa Romeo Racing ORLEN', '4500000', '3150000', '1250000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('5', 'Carlos Sainz', '67', '87', '83', '88', 'McLaren F1 Team', '6000000', '4000000', '2000000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('6', 'Charles Leclerc', '59', '92', '88', '92', 'Shuderia Ferrari', '7500000', '5000000', '2500000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('7', 'Daniel Ricciardo', '77', '89', '87', '91', 'Renault DP World F1 Team', '9000000', '6000000', '3000000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('8', 'Esteban Ocon', '58', '91', '88', '79', 'Renault DP World F1 Team', '6000000', '4000000', '2000000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('9', 'Fernando Alonso', '79', '84', '91', '91', 'Renault DP World F1 Team', '6000000', '4000000', '2000000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('10', 'George Russel', '55', '65', '82', '83', 'Williams Racing', '4500000', '3150000', '1850000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('11', 'Kimi Raikkonen', '97', '85', '92', '81', 'Alfa Romeo Racing ORLEN', '9000000', '6000000', '3500000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('12', 'Lance Stroll', '54', '90', '84', '85', 'McLaren F1 Team', '6000000', '4350000', '2000000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('13', 'Lando Norris', '57', '90', '94', '81', 'McLaren F1 Team', '6500000', '4000000', '2000000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('14', 'Nicholas Latifi', '48', '70', '69', '74', 'Williams Racing', '2250000', '1500000', '750000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('15', 'Nikita Mazepin', '44', '60', '81', '82', 'HAAS', '2250000', '1500000', '750000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('16', 'Pierre Gasly', '55', '84', '90', '85', 'Scuderia AlphaTauri Honda', '6000000', '4000000', '2000000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('17', 'Sebastian Vettel', '57', '70', '69', '74', 'Aston Martin', '6500000', '4000000', '2000000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('18', 'Sergio Perez', '79', '92', '80', '87', 'BWT Racing Point F1 Team', '7500000', '5000000', '2500000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('19', 'Valtteri Bottas', '71', '88', '99', '93', 'Mercedes-AMG Petronas', '10500000', '7000000', '3500000');");

            Execute("INSERT INTO 'main'.'driver_register' ('ID', 'DriveName', 'Experiance', 'RaceCraft', 'Awareness', 'Pace', 'Team', 'MarketPrice', 'MarketSalary', 'MarketBonus') VALUES ('20', 'Yuki Tsunoda', '57', '90', '84', '85', 'Scuderia AlphaTauri Honda', '6000000', '4000000', '2000000');");
        }
        catch (Exception ex2)
        {
            Debug.Log(ex2);
        }
    }

    public void JoinTeam(string _teamName)
    {
        Execute("INSERT INTO 'main'.'player_profile' ('ID', 'FirstName', 'LastName', 'TeamName', 'LastAccessed', 'AccessCount', 'AverageGameplayDuration', 'GoogleAccount', 'FacebookAccount', 'ImageURL', 'BillableAccount', 'AccountKey', 'AccountSecret') VALUES ('1', 'John', 'Doe', 'Team-Name', '2021-02-07', '159', '23', 'NONE', 'NONE', 'NONE', 'NO', 'NONE', 'NONE');");
        Execute("UPDATE 'main'.'player_profile' SET 'TeamName' = '" + _teamName + "' WHERE ID = '1';");
        teamName.text = _teamName;
        if (playerProfile)
            playerProfile.InitializePlayer();
    }

    public void CreateTeam()
    {
        Execute("INSERT INTO 'main'.'player_profile' ('ID', 'FirstName', 'LastName', 'TeamName', 'LastAccessed', 'AccessCount', 'AverageGameplayDuration', 'GoogleAccount', 'FacebookAccount', 'ImageURL', 'BillableAccount', 'AccountKey', 'AccountSecret') VALUES ('1', 'John', 'Doe', 'Team-Name', '2021-02-07', '159', '23', 'NONE', 'NONE', 'NONE', 'NO', 'NONE', 'NONE');");
        Execute("UPDATE 'main'.'player_profile' SET 'TeamName' = '" + teamName.text + "' WHERE ID = '1';");
        if (playerProfile)
            playerProfile.InitializePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrency();

        var silverCoins = GameObject.FindGameObjectsWithTag("db_silver_coin");
        foreach (GameObject obj in silverCoins)
        {
            var text = obj.GetComponent<Text>();
            if (text)
            {
                text.text = "" + DBDriver.SilverCoin.ToString("C", CultureInfo.CurrentCulture);
            }
        }

        var goldCoins = GameObject.FindGameObjectsWithTag("db_gold_coin");
        foreach (GameObject obj in goldCoins)
        {
            var text = obj.GetComponent<Text>();
            if (text)
            {
                text.text = DBDriver.GoldCoin.ToString();
            }
        }

        var exPoints = GameObject.FindGameObjectsWithTag("db_experiance_points");
        foreach (GameObject obj in exPoints)
        {
            var text = obj.GetComponent<Text>();
            if (text)
            {
                text.text = DBDriver.ExperiancePoints.ToString();
            }
        }
    }
}
