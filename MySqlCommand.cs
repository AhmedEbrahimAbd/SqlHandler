namespace Utilities.MySQL
{
    using Enumeration;

    using Extensions;

    using System;
    using System.Text;

    using MYSQLCOMMAND = MySql.Data.MySqlClient.MySqlCommand;
    using MYSQLCONNECTION = MySql.Data.MySqlClient.MySqlConnection;

    public unsafe class MySqlCommand : IDisposable
    {
        private MySqlCommandType _type;

        public MySqlCommandType Type
        {
            get => _type;
            set => _type = value;
        }

        protected StringBuilder _command;

        public string Command
        {
            get
            {
                if (_command != null)
                {
                    return _command.ToString();
                }
                else
                {
                    return "";
                }
            }
            set => _command = new StringBuilder(value);
        }

        private bool firstPart = true;
        private readonly bool IsDisposed = false;

        private readonly SafeDictionary<byte, string> insertFields;
        private readonly SafeDictionary<byte, string> insertValues;
        private byte lastpair;

        public MySqlCommand(MySqlCommandType Type)
        {
            this.Type = Type;
            switch (Type)
            {
                case MySqlCommandType.SELECT: _command = new StringBuilder("SELECT * FROM <R>"); break;
                case MySqlCommandType.UPDATE: _command = new StringBuilder("UPDATE <R> SET "); break;
                case MySqlCommandType.INSERT:
                    {
                        insertFields = new SafeDictionary<byte, string>();
                        insertValues = new SafeDictionary<byte, string>();
                        lastpair = 0;
                        _command = new StringBuilder("INSERT INTO <R> (<F>) VALUES (<V>)");
                        break;
                    }
                case MySqlCommandType.DELETE: _command = new StringBuilder("DELETE FROM <R> WHERE <C> = <V>"); break;
                case MySqlCommandType.COUNT: _command = new StringBuilder("SELECT count(<V>) FROM <R>"); break;
            }
        }
        ~MySqlCommand()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }
        public void Dispose()
        {
            if (!IsDisposed)
            {
                if (insertValues != null)
                {
                    insertValues.Clear();
                    insertFields.Clear();
                }
                _command = null;
            }


            using (MYSQLCONNECTION? conn = DataHolder.MySqlConnection)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public int Execute2()
        {
            if (Type == MySqlCommandType.INSERT)
            {
                string fields = "";
                string values = "";
                byte x;
                for (x = 0; x < lastpair; x++)
                {
                    bool comma = (x + 1) == lastpair ? false : true;

                    #region Fields

                    if (comma)
                    {
                        fields += "`" + insertFields[x] + "`,";
                    }
                    else
                    {
                        fields += "`" + insertFields[x] + "`";
                    }

                    #endregion Fields

                    #region Values

                    if (comma)
                    {
                        values += "'" + insertValues[x] + "'" + ",";
                    }
                    else
                    {
                        values += "'" + insertValues[x] + "'";
                    }

                    #endregion Values
                }
                _command = _command.Replace("<F>", fields);
                _command = _command.Replace("<V>", values);
            }
            using (MYSQLCONNECTION? conn = DataHolder.MySqlConnection)
            {
                //conn.ConnectionString = "Server=localhost;Port=3306;Database=tq;Uid=root;Password=ahmed010930;Persist Security Info=True;Pooling=true; Min Pool Size = 32;  Max Pool Size = 300;";
                conn.Open();
                MYSQLCOMMAND cmd = new MYSQLCOMMAND(Command + ";", conn);
                return cmd.ExecuteNonQuery();
            }
        }

        private bool Comma()
        {
            if (firstPart)
            {
                firstPart = false;
                return false;
            }
            string command = _command.ToString();
            if (command[command.Length - 1] == ',' || command[command.Length - 2] == ',' || command[command.Length - 3] == ',')
            {
                return false;
            }

            return true;
        }

        #region Select

        public MySqlCommand Select(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }

        #endregion Select

        #region Count

        public MySqlCommand Count(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }

        #endregion Count

        #region Delete

        public MySqlCommand Delete(string table, string column, string value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", "'" + value.MySqlEscape() + "'");
            return this;
        }

        public MySqlCommand Delete(string table, string column, long value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", value.ToString());
            return this;
        }

        public MySqlCommand Delete(string table, string column, ulong value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", value.ToString());
            return this;
        }

        public MySqlCommand Delete(string table, string column, bool value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", (value ? "1" : "0"));
            return this;
        }

        #endregion Delete

        #region Update

        public MySqlCommand Update(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }

        public MySqlCommand Set(string column, long value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                {
                    _command = _command.Append(",`" + column + "` = " + value.ToString() + " ");
                }
                else
                {
                    _command = _command.Append("`" + column + "` = " + value.ToString() + " ");
                }
            }
            return this;
        }

        public MySqlCommand Set(string column, double value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                {
                    _command = _command.Append(",`" + column + "` = " + value.ToString() + " ");
                }
                else
                {
                    _command = _command.Append("`" + column + "` = " + value.ToString() + " ");
                }
            }
            return this;
        }

        public MySqlCommand Set(string column, ulong value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                {
                    _command = _command.Append(",`" + column + "` = " + value.ToString() + " ");
                }
                else
                {
                    _command = _command.Append("`" + column + "` = " + value.ToString() + " ");
                }
            }
            return this;
        }

        public MySqlCommand Set(string column, string value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                {
                    _command = _command.Append(",`" + column + "` = '" + value + "' ");
                }
                else
                {
                    _command = _command.Append("`" + column + "` = '" + value.MySqlEscape() + "' ");
                }
            }
            return this;
        }

        public MySqlCommand Set(string column, bool value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                {
                    _command = _command.Append(",`" + column + "` = " + (value ? "1" : "0") + " ");
                }
                else
                {
                    _command = _command.Append("`" + column + "` = " + (value ? "1" : "0") + " ");
                }
            }
            return this;
        }

        public MySqlCommand Set<T>(string column, T value)
        {
            if (value is bool)
            {
                Set(column, value);
            }
            else
            {
                Set(column, value.ToString());
            }

            return this;
        }

        #endregion Update

        #region Insert

        public MySqlCommand Insert(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }

        public MySqlCommand Insert(string field, long value)
        {
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, value.ToString());
            lastpair++;
            return this;
        }

        public MySqlCommand Insert(string field, ulong value)
        {
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, value.ToString());
            lastpair++;
            return this;
        }

        public MySqlCommand Insert(string field, bool value)
        {
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, (value ? 1 : 0).ToString());
            lastpair++;
            return this;
        }

        public MySqlCommand Insert(string field, string value)
        {
            char[]? array = value.ToCharArray();
            string str = Encoding.Default.GetString(Encoding.Unicode.GetBytes(array, 0, array.Length));
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, value.MySqlEscape());
            lastpair++;
            return this;
        }

        #endregion Insert

        #region Where

        public MySqlCommand Where(string column, long value)
        {
            _command = _command.Append("WHERE `" + column + "` = " + value);
            return this;
        }

        public MySqlCommand Where(string column, long value, bool greater)
        {
            if (greater)
            {
                _command = _command.Append("WHERE `" + column + "` > " + value);
            }
            else
            {
                _command = _command.Append("WHERE `" + column + "` < " + value);
            }

            return this;
        }

        public MySqlCommand Where(string column, ulong value)
        {
            _command = _command.Append("WHERE `" + column + "` = " + value);
            return this;
        }

        public MySqlCommand Where(string column, string value)
        {
            _command = _command.Append("WHERE `" + column + "` = '" + value.MySqlEscape() + "'");
            return this;
        }

        public MySqlCommand Where(string column, bool value)
        {
            _command = _command.Append("WHERE `" + column + "` = " + (value ? "1" : "0"));
            return this;
        }

        #endregion Where

        #region And

        public MySqlCommand And(string column, long value)
        {
            _command = _command.Append(" AND `" + column + "` = " + value);
            return this;
        }

        public MySqlCommand And(string column, ulong value)
        {
            _command = _command.Append(" AND `" + column + "` = " + value);
            return this;
        }

        public MySqlCommand And(string column, string value)
        {
            _command = _command.Append(" AND `" + column + "` = '" + value.MySqlEscape() + "'");
            return this;
        }

        public MySqlCommand And(string column, bool value)
        {
            _command = _command.Append(" AND `" + column + "` = " + (value ? "1" : "0"));
            return this;
        }

        public MySqlCommand And(string column, long value, bool greater)
        {
            if (greater)
            {
                _command = _command.Append(" AND `" + column + "` > " + value);
            }
            else
            {
                _command = _command.Append(" AND `" + column + "` < " + value);
            }

            return this;
        }

        #endregion And

        #region Or

        public MySqlCommand Or(string column, long value)
        {
            _command = _command.Append(" Or `" + column + "` = " + value);
            return this;
        }

        public MySqlCommand Or(string column, ulong value)
        {
            _command = _command.Append(" Or `" + column + "` = " + value);
            return this;
        }

        public MySqlCommand Or(string column, string value)
        {
            _command = _command.Append(" Or `" + column + "` = '" + value.MySqlEscape() + "'");
            return this;
        }

        public MySqlCommand Or(string column, bool value)
        {
            _command = _command.Append(" Or `" + column + "` = " + (value ? "1" : "0"));
            return this;
        }

        #endregion Or

        #region Order

        public MySqlCommand Order(string column)
        {
            _command = _command.Append("ORDER BY " + column + "");
            return this;
        }

        #endregion Order

        public int Execute()
        {
            using (MYSQLCONNECTION? conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                return Execute(conn);
            }
        }

        public int Execute(MYSQLCONNECTION conn)
        {
            if (Type == MySqlCommandType.INSERT)
            {
                string fields = "";
                string values = "";
                byte x;
                for (x = 0; x < lastpair; x++)
                {
                    bool comma = (x + 1) == lastpair ? false : true;

                    #region Fields

                    if (comma)
                    {
                        fields += "`" + insertFields[x] + "`,";
                    }
                    else
                    {
                        fields += "`" + insertFields[x] + "`";
                    }

                    #endregion Fields

                    #region Values

                    if (comma)
                    {
                        values += "'" + insertValues[x] + "'" + ",";
                    }
                    else
                    {
                        values += "'" + insertValues[x] + "'";
                    }

                    #endregion Values
                }
                _command = _command.Replace("<F>", fields);
                _command = _command.Replace("<V>", values);
            }

            MYSQLCOMMAND cmd = new MYSQLCOMMAND(Command, conn);
            return cmd.ExecuteNonQuery();
        }

        public MySqlReader CreateReader()
        {
            return new MySqlReader(this);
        }

        void IDisposable.Dispose()
        {
            if (insertValues != null)
            {
                insertValues.Clear();
                insertFields.Clear();
            }
            _command = null;
        }
    }
}