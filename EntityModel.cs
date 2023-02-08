public abstract class BaseModel
    {
        private ConnectionBase conn = new ConnectionBase();
        public List<PushData> data = new List<PushData>();
        public string nameTable;
        public BaseModel()
        {
            Clean();
        }
        public bool Insert()
        {
            UpdateData();
            if (verification())
            {
                if (data.Count != 0)
                {
                    string query = "INSERT INTO " + nameTable + " (";
                    data.ForEach(x => {
                        query += x.value == "" ? "" : x.parameter + ",";
                    });
                    query = query.Remove(query.Length - 1, 1);
                    query += ") VALUES (";
                    data.ForEach(x =>
                    {
                        query += x.value == "" ? "" : "'" + x.value + "',";
                    });
                    query = query.Remove(query.Length - 1, 1);
                    query += ");";

                    return conn.excecuteQuery(query);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool Update(PushData DataWhere)
        {
            UpdateData();
            string query = "UPDATE " + nameTable + " SET";
            data.ForEach(x =>
            {
                query += x.value == "" ? "" : " " + x.parameter + " = '" + x.value + "',";
            });
            query = query.Remove(query.Length - 1, 1);
            query += " WHERE " + DataWhere.parameter + " = '" + DataWhere.value + "';";

            return conn.excecuteQuery(query);
        }
        public abstract void UpdateData();
        public bool verification()
        {
            var listVerification = MandatoryValues();
            bool valueVerification = true;

            for (int i = 0; i < data.Count; i++)
            {

                for (int j = 0; j < listVerification.Count; j++)
                {
                    if (data[i].parameter == listVerification[j])
                    {
                        if (DataIsNull(data[i].value))
                        {
                            valueVerification = false;
                            break;
                        }
                    }
                }
                if (!valueVerification)
                {
                    break;
                }

            }

            return valueVerification;
        }
        public abstract List<string> MandatoryValues();
        public void CreateList(string _parameter, string _value)
        {
            if (DataIsNull(_value))
            {
                data.Add(new PushData { parameter = _parameter, value = "" });
            }
            else
            {
                data.Add(new PushData { parameter = _parameter, value = _value });
            }
        }
        private bool DataIsNull(string value)
        {
            if ((value == null) || (value == "0") || (value == "") || (value == "0001/01/01 00:00:00.000") || (value == "1E-05"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public abstract void Clean();
    }
    public struct PushData
    {
        public string parameter;
        public string value;
    }
    //MODELOS
    public class Aseguradora : BaseModel
    {
        public Aseguradora() : base()
        {
            nameTable = "[dbo].[Catalogo_Aseguradora]";
        }
        public override void Clean()
        {
            ASEGURADORA = null;  
            NO_POLIZA = null;
            MONTO_ASEGURADO = 0.00001;
            VIGENCIA = DateTime.MinValue;
            TELEFONO = null;
        }
        public override void UpdateData()
        {
            CreateList("ASEGURADORA", ASEGURADORA);
            CreateList("NO_POLIZA", NO_POLIZA);
            CreateList("MONTO_ASEGURADO", MONTO_ASEGURADO.ToString());
            CreateList("VIGENCIA", VIGENCIA.ToString("yyyy/MM/dd HH:mm:ss.fff"));
            CreateList("TELEFONO", TELEFONO);
        }
        public override List<string> MandatoryValues()
        {
            return new List<string> { };
        }
        public string ASEGURADORA { get; set; }
        public string NO_POLIZA { get; set; }
        public double MONTO_ASEGURADO { get; set; }
        public DateTime VIGENCIA { get; set; }
        public string TELEFONO { get; set; }
       
    }
