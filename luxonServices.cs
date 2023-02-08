namespace Luxon
{
    public partial class Luxon : ServiceBase
    {
        private int eventedId = 1;
        // EL dia de ejecucion es el jueves.      Recuerda agregar coma a los dias correspondientes
        private string dayExecute = "jueves";
        // el intervalo de cada cuando revisa el servicion. 60000 -> 1 minuto
        private int intervaleTime = 60000;
        public Luxon()
        {
            InitializeComponent();
            eventosSistema = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("Luxon"))
            {
                System.Diagnostics.EventLog.CreateEventSource("Luxon", "Application");
            }
            eventosSistema.Source = "Luxon";
            eventosSistema.Log = "Application";
        }
        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            Timer timer = new Timer();
            timer.Interval = intervaleTime;
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
            eventosSistema.WriteEntry("Iniciado servicio de respuesta de mensajes " + "Luxon Sistem");
        }
        protected override void OnStop()
        {
            eventosSistema.WriteEntry("Detenido servicio de respuesta de mensajes " + "Luxon Sistem");
        }
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            eventosSistema.WriteEntry("Monitoring the system" + EventLogEntryType.Information + (eventedId++));
            SendEmails();
        }
        public void SendEmails()
        {
            try
            {
                System.Diagnostics.Debugger.Launch();
                string Registrer = getText("SELECT MAX(Fecha_Envio) FROM [dbo].[Registros_Correo]");
                if (string.IsNullOrEmpty(Registrer)) Registrer = DateTime.MinValue.ToString();
                DateTime date = Convert.ToDateTime(Registrer);
                if(! (date.ToString("MM/dd/yyyy") == DateTime.Now.ToString(("MM/dd/yyyy"))) )
                {
                    if (DateTime.Now.ToString("dddd",new CultureInfo("es-Es")) == dayExecute )
                    {
                        DataTable data = getTable("SELECT distinct Area,Area_especifico,Correo FROM [dbo].[usuarios] WHERE Area_especifico is not null And Puesto = 'Jefe'; ");
                        for (int i = 0; i < data.Rows.Count; i++)
                        {
                            SendEmail(data.Rows[i]["Correo"].ToString(), data.Rows[i]["Area_especifico"].ToString());
                        }
                        excecuteQuery("Insert into [dbo].[Registros_Correo] ([Fecha_Envio],[Areas]) VALUES ('" + DateTime.Now.ToString("MM/dd/yyyy") + "','" + GetAreas() + "')");
                    }
                }
                eventosSistema.WriteEntry("Connection correct:" + eventedId);
            }
            catch (Exception e)
            {
                eventosSistema.WriteEntry("Error conection : " + e);
            }
        }
        private string GetAreas()
        {
            List<string> ListAreas = getColum("SELECT distinct Area FROM[Biometrico_TM].[dbo].[usuarios] WHERE Area_especifico is not null And Puesto = 'Jefe'; ");
            string QueryAreas = "";
            ListAreas.ForEach((area) =>
            {
                QueryAreas += area + ",";
            });
            return QueryAreas;
        }
        public void SendEmail(string Email, string link)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.BodyEncoding = Encoding.GetEncoding(1252);
                message.From = new MailAddress("soportetrenmaya@gmail.com");
                message.To.Add(Email);
                message.Subject = ("MIC-Plataforma control de asistiencia");
                message.Body = string.Format("");
                message.Body = "<html>" +
                    "<head>" +
                    "<style>" +
                        "table, td, th { " +
                          "border: 1px solid black;" +
                          "text-align: center;" +
                        "}" +
                        "table {" +
                          "width: 100%;" +
                          "border-collapse: collapse;" +
                        "}" +
                    "</style>" +
                    "</head>" +
                    "<body>" +
                        @"<h1 style= ""text-align: center; font-family: Arial; color: #006699; font-size: 25px;"">MIC-Plataforma control de asistiencia</h1>" +
                        @"<h2 style=""text-align: center; font-family: Arial; color: #000000; font-size: 18px;"">Favor de entrar al link para autorizar las faltas</h2>" +
                        @"<center><a  style=""text-align: center; font-family: Arial; color: #000000; font-size: 25px;"" href=" + link + " >biometrico</a></center>" +
                        @"<h6 style=""text-align: center; font-family: Arial; color: #000000; font-size:15px;"">Favor de Revisarla en el portal!!!</h6>" +
                        @"<h6 style=""text-align: center; font-family: Arial; color: #000000; font-size:15px;"">Validar antes de las 12:00 pm</h6>" +
                        @"<h3 style=""text-align: center; font-family: Arial; color: #000000; font-size: 15px;"">FAVOR DE NO RESPONDER O REENVIAR ESTE CORREO AL EMISOR</h3>" +
                    "</body>" +
                    "</html>";
                message.IsBodyHtml = true;
                var htmlView = AlternateView.CreateAlternateViewFromString(message.Body, new ContentType("text/html"));
                htmlView.ContentType.CharSet = Encoding.UTF8.WebName;
                message.AlternateViews.Add(htmlView);

                SmtpClient cliente = new SmtpClient();
                cliente.EnableSsl = true;
                cliente.Port = 587;
                cliente.Host = "smtp.gmail.com";
                cliente.Credentials = new NetworkCredential("soportetrenmaya@gmail.com", "wbhxhlmjrfaozscx");
                cliente.Send(message);
            }
            catch
            {

            }
        }
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
            eventosSistema.WriteEntry("### ERROR [ " + nameFunction + " / " + query + " ] : [ " + e + " ]");
        }
        private void Print(string nameFunction, string query)
        {
            eventosSistema.WriteEntry("### SUCCESS [ " + nameFunction + " / " + query + " ] ");
        }
        private string GetStringConnection()
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();
            connectionString.DataSource = @"localhost\SQLEXPRESS";
            connectionString.UserID = "sa";
            connectionString.Password = "12345";
            //connectionString.IntegratedSecurity = true;
            connectionString.InitialCatalog = "****";
            return connectionString.ConnectionString;
        }
    }
    
}
