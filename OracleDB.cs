using System;
using System.Data;
using System.IO;
using System.Text.Json;
using Oracle.ManagedDataAccess.Client;
    
namespace GetOracleData
{
    class OracleDB
    {
        private readonly string connectionString;
        private readonly OracleConnection connection;
        private OracleTransaction transaction;

        /// <summary>
        /// コンストラクタ：設定ファイルからDB接続情報を取得
        /// </summary>
        public OracleDB()
        {
            try
            {
                string basePath = AppContext.BaseDirectory;
                string jsonPath = Path.Combine(basePath, "appsettings.json");

                if (!File.Exists(jsonPath))
                {
                    throw new FileNotFoundException($"設定ファイルが見つかりません: {jsonPath}");
                }

                // JSONファイルを読み込む
                string json = File.ReadAllText(jsonPath);

                // JSONをDatabaseConfigオブジェクトに変換
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // JSONのキー名を大文字小文字を無視
                    ReadCommentHandling = JsonCommentHandling.Skip, // コメントをスキップ
                    AllowTrailingCommas = true // 末尾のカンマを許容
                };

                DatabaseConfig config = JsonSerializer.Deserialize<DatabaseConfig>(json, options);
                if (config == null)
                {
                    throw new InvalidOperationException("JSONのデシリアライズに失敗しました。");
                }

                // Oracle接続文字列を生成
                connectionString = $"User ID={config.UserId}; Password={config.Password}; Data Source={config.DataSource};";

                // 接続オブジェクトを作成
                connection = new OracleConnection(connectionString);
            }
            catch(Exception ex)
            {
                logger.Error(ex, true);
                throw;
            }
        }

        /// <summary>
        /// データベース接続を開く
        /// </summary>
        public bool Open()
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    return true;
                }
                return connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// データベース接続を閉じる
        /// </summary>
        public bool Close()
        {
            try
            {
                if (transaction != null)
                {

                }

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    return true;
                }
                return connection.State == ConnectionState.Closed;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// データベースのトランザクション開始
        /// </summary>
        public void BeginTransaction()
        {
            transaction = connection.BeginTransaction();
        }

        /// <summary>
        /// データベースのコミット
        /// </summary>
        public void Commit()
        {
            if (transaction != null)
            {
                transaction.Commit();
                transaction = null;
            }
        }

        /// <summary>
        /// データベースのロールバック
        /// </summary>
        public void Rollback()
        {
            if (transaction != null)
            {
                transaction.Rollback();
                transaction = null;
            }
        }

        /// <summary>
        /// テーブルデータの取得
        /// </summary>
        /// <param name="sqlQuery">SQL文</param>
        /// <returns>データテーブル</returns>
        public bool Execute(string sqlQuery, out DataTable dataTable)
        {
            dataTable = new DataTable();

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("データベースが開かれていません。");
                }

                using (var cmd = new OracleCommand(sqlQuery, connection))
                {
                    using (var adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// テーブルデータ更新
        /// </summary>
        /// <param name="sqlQuery">SQL文</param>
        /// <returns>取得行数(</returns>
        public bool Execute(string sqlQuery)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("データベースが開かれていません。");
                }

                if (transaction == null)
                {
                    throw new InvalidOperationException("トランザクションが開始されていません。");
                }

                int affrctedRows = 0;
                using (var cmd = new OracleCommand(sqlQuery, connection))
                {
                    cmd.Transaction = transaction;
                    affrctedRows = cmd.ExecuteNonQuery();
                }

                if (affrctedRows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
    }
}
