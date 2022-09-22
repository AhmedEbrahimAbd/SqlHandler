namespace Utilities.MySQL
{
    using Enumeration;

    using MySql.Data.MySqlClient;

    using System;
    using System.Collections.Generic;
    using System.Data;

    public unsafe class MySqlReader : IDisposable
    {
        private readonly DataSet _dataset;
        private DataRow _datarow;
        private int _row;
        private const string Table = "table";

        public MySqlReader(MySqlCommand command)
        {
            if (command.Type == MySqlCommandType.SELECT)
            {
                _dataset = new DataSet();
                _row = 0;
                using (MySql.Data.MySqlClient.MySqlConnection conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (MySqlDataAdapter? DataAdapter = new MySqlDataAdapter(command.Command, conn))
                    {
                        DataAdapter.SelectCommand.CommandTimeout = 0;
                        DataAdapter.Fill(_dataset, Table);
                    }
                    ((IDisposable)command).Dispose();
                }
            }
        }

        ~MySqlReader()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }
        void IDisposable.Dispose()
        {
            if (_dataset != null)
            {
                _dataset.Dispose();
            }
        }

        public void Dispose()
        {
            if (_dataset != null)
            {
                _dataset.Clear();
                _dataset.Dispose();
            }
        }
        public bool Read()
        {
            if (_dataset == null)
            {
                return false;
            }

            if (_dataset.Tables.Count == 0)
            {
                return false;
            }

            if (_dataset.Tables[Table].Rows.Count > _row)
            {
                _datarow = _dataset.Tables[Table].Rows[_row];
                _row++;
                return true;
            }
            if (_row == null)
            {
                _row++;
                return true;
            }
            _row++;
            return false;
        }
        public bool CanRead()
        {
            if (_dataset == null)
            {
                return false;
            }

            if (_dataset.Tables.Count == 0)
            {
                return false;
            }

            if (_dataset.Tables[Table].Rows.Count > _row)
            {
                return true;
            }
            return false;
        }
        public int NumberOfRows
        {
            get
            {
                if (_dataset == null)
                {
                    return 0;
                }

                if (_dataset.Tables.Count == 0)
                {
                    return 0;
                }

                return _dataset.Tables[Table].Rows.Count;
            }
        }

        public sbyte ReadSByte(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(sbyte);
            }

            sbyte.TryParse(_datarow[columnName].ToString(), out sbyte result);
            return result;
        }

        public byte ReadByte(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(byte);
            }

            byte.TryParse(_datarow[columnName].ToString(), out byte result);
            return result;
        }

        public short ReadInt16(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(short);
            }

            short.TryParse(_datarow[columnName].ToString(), out short result);
            return result;
        }

        public ushort ReadUInt16(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(ushort);
            }

            ushort.TryParse(_datarow[columnName].ToString(), out ushort result);
            return result;
        }

        public int ReadInt32(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(int);
            }

            int.TryParse(_datarow[columnName].ToString(), out int result);
            return result;
        }

        public uint ReadUInt32(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(uint);
            }

            uint.TryParse(_datarow[columnName].ToString(), out uint result);
            return result;
        }
        public long ReadInt64(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(long);
            }

            long.TryParse(_datarow[columnName].ToString(), out long result);
            return result;
        }

        public ulong ReadUInt64(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(ulong);
            }

            ulong.TryParse(_datarow[columnName].ToString(), out ulong result);
            return result;
        }
        public float ReadFloat(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(float);
            }

            float.TryParse(_datarow[columnName].ToString(), out float result);
            return result;
        }
        public double ReadDouble(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(double);
            }

            double.TryParse(_datarow[columnName].ToString(), out double result);
            return result;
        }

        public ulong ReadUInt128(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return default(ulong);
            }

            ulong.TryParse(_datarow[columnName].ToString(), out ulong result);
            return result;
        }

        public string ReadString(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return "";
            }

            string data = _datarow[columnName].ToString();
            return data;
        }

        public bool ReadBoolean(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return false;
            }

            string str = _datarow[columnName].ToString();
            if (str == "")
            {
                return false;
            }

            if (str[0] == '1')
            {
                return true;
            }

            if (str[0] == '0')
            {
                return false;
            }

            bool.TryParse(str, out bool result);
            return result;
        }

        public byte[] ReadBlob(string columnName)
        {
            if (_datarow.IsNull(columnName))
            {
                return new byte[0];
            }

            return (byte[])_datarow[columnName];
        }

        public byte[] ReadByteArray(string columnName, string[] separator)
        {
            if (_datarow.IsNull(columnName))
            {
                return new byte[0];
            }

            string[]? str = _datarow[columnName].ToString().Split(separator, StringSplitOptions.RemoveEmptyEntries);
            byte[] array = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                byte.TryParse(str[i], out byte result);
                array[i] = result;
            }
            return array;
        }
        public float[] ReadFloatArray(string columnName, string[] separator)
        {
            if (_datarow.IsNull(columnName))
            {
                return new float[0];
            }

            string[]? str = _datarow[columnName].ToString().Split(separator, StringSplitOptions.RemoveEmptyEntries);
            float[] array = new float[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                float.TryParse(str[i], out float result);
                array[i] = result;
            }
            return array;
        }
        public uint[] ReadUIntArray(string columnName, string[] separator)
        {
            if (_datarow.IsNull(columnName))
            {
                return new uint[0];
            }

            string[]? str = _datarow[columnName].ToString().Split(separator, StringSplitOptions.RemoveEmptyEntries);
            uint[] array = new uint[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                uint.TryParse(str[i], out uint result);
                array[i] = result;
            }
            return array;
        }
        public double[] ReadDoubleArray(string columnName, string[] separator)
        {
            if (_datarow.IsNull(columnName))
            {
                return new double[0];
            }

            string[]? str = _datarow[columnName].ToString().Split(separator, StringSplitOptions.RemoveEmptyEntries);
            double[] array = new double[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                double.TryParse(str[i], out double result);
                array[i] = result;
            }
            return array;
        }

        public List<byte> ReadListArray(string columnName, string[] separator)
        {
            if (_datarow.IsNull(columnName))
            {
                return new List<byte> { 0 };
            }

            string[]? str = _datarow[columnName].ToString().Split(separator, StringSplitOptions.RemoveEmptyEntries);
            List<byte> array = new List<byte>(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                array.Add(byte.Parse(str[i]));
            }
            return array;
        }
        //public MySqlDateTime ReadDate(string columnName)
        //{
        //    if (_datarow.IsNull(columnName)) return new MySqlDateTime(2011, 1, 1, 1, 1, 1);
        //    return (MySqlDateTime)_datarow[columnName];
        //}
    }
}