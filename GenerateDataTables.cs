class CreateTable{
        private DataTable table = new DataTable();
        public DataRow GetRow()
        {
            return table.NewRow();
        }
        public CreateTable(List<string> listName)
        {
            listName.ForEach((name) =>
            {
                table.Columns.Add(columnString(name));
            });
        }
        public void Update(DataRow newRow)
        {
            table.Rows.Add(newRow);
        }
        public DataTable getTable()
        {
            return table;
        }
        private DataColumn columnString(string columnName)
        {
            return new DataColumn()
            {
                DataType = Type.GetType("System.String"),
                ColumnName = columnName
            };
        }
    }
