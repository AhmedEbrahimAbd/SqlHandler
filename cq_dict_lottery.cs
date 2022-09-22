using Enumeration;
using Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utilities.MySQL;

namespace MsgServer.Database
{
    public class cq_dict_lottery
    {
        public static Dictionary<uint, cq_dict_lottery_fields> dict_lottery = new Dictionary<uint, cq_dict_lottery_fields>();
        public class cq_dict_lottery_fields
        {
            public uint id { get; set; }
            public uint type { get; set; }
            public uint value1 { get; set; }
            public uint value2 { get; set; }
            public uint value3 { get; set; }
            public uint rate { get; set; }
        }
        [DBAttribute("cq_dict_lottery")]
        private static unsafe void Process(MySqlCommand cmd, MySqlReader reader)
        {
            using (cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("cq_dict_lottery"))
            using (reader = cmd.CreateReader())
            {
                if (reader.CanRead())
                {
                    while (reader.Read())
                    {

                        cq_dict_lottery_fields data = new cq_dict_lottery_fields()
                        {
                            id = reader.ReadUInt32("id"),
                            type = reader.ReadUInt32("type"),
                            value1 = reader.ReadUInt32("value1"),
                            value2 = reader.ReadUInt32("value2"),
                            value3 = reader.ReadUInt32("value3"),
                            rate = reader.ReadUInt32("rate"),
                        };
                        if (!dict_lottery.ContainsKey(data.id))
                        {
                            dict_lottery.Add(data.id, data);
                        }
                    }
                }
                else
                {
                    Server.EmptyTable++;
                    MyConsole.WriteLine("Table cq_dict_lottery is empty.", ConsoleColor.DarkYellow);
                }
            }
            cmd.Dispose();
            reader.Dispose();
        }
    }
}
