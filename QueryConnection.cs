public class ConnectionBase
    {
        public bool excecuteQuery(string query)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(GetStringConnection()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    Print(System.Reflection.MethodBase.GetCurrentMethod().Name, query);
                    return true;
                }
            }
            catch (Exception e)
            {
                Print(e, System.Reflection.MethodBase.GetCurrentMethod().Name, query);
                return false;
            }

        }
        public double getScalar(string query)
        {

            double result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(GetStringConnection()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result = Convert.ToDouble(reader[0]);
                    }
                    reader.Close();
                    conn.Close();
                }
                Print(System.Reflection.MethodBase.GetCurrentMethod().Name, query);
            }
            catch (Exception e)
            {
                Print(e, System.Reflection.MethodBase.GetCurrentMethod().Name, query);
            }
            return result;

        }
        public string getText(string query)
        {

            string result = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(GetStringConnection()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result = reader[0].ToString();
                    }
                    reader.Close();
                    conn.Close();
                    Print(System.Reflection.MethodBase.GetCurrentMethod().Name, query);
                }
            }
            catch (Exception e)
            {
                Print(e, System.Reflection.MethodBase.GetCurrentMethod().Name, query);
            }
            return result;

        }
        public DataSet getDataSet(string query)
        {

            DataSet data = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(GetStringConnection()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(data);
                    conn.Close();
                }
                Print(System.Reflection.MethodBase.GetCurrentMethod().Name, query);
            }
            catch (Exception e)
            {
                Print(e, System.Reflection.MethodBase.GetCurrentMethod().Name, query);
            }
            return data;

        }
        public DataTable getTable(string query)
        {

            DataTable table = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(GetStringConnection()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);
                    conn.Close();
                }
                Print(System.Reflection.MethodBase.GetCurrentMethod().Name, query);
            }
            catch (Exception e)
            {
                Print(e, System.Reflection.MethodBase.GetCurrentMethod().Name, query);
            }
            return table;

        }
        public List<string> getColum(string query)
        {

            List<string> column = new List<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(GetStringConnection()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        column.Add(reader[0].ToString());
                    }
                }
                Print(System.Reflection.MethodBase.GetCurrentMethod().Name, query);
            }
            catch (Exception e)
            {
                Print(e, System.Reflection.MethodBase.GetCurrentMethod().Name, query);
            }
            return column;

        }
        private void Print(Exception e, string nameFunction, string query)
        {
            Debug.WriteLine("### ERROR [ " + nameFunction + " / " + query + " ] : [ " + e + " ]");
        }
        private void Print(string nameFunction, string query)
        {
            Debug.WriteLine("### SUCCESS [ " + nameFunction + " / " + query + " ] ");
        }
        private string GetStringConnection()
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();
            connectionString.DataSource = @"localhost\SQLEXPRESS";
            //connectionString.UserID = "sa";
            //connectionString.Password = "12345";
            connectionString.IntegratedSecurity = true;
            connectionString.InitialCatalog = "****";
            return connectionString.ConnectionString;
        }
    }
