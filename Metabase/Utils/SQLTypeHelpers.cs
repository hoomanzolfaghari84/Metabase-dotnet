using System.Collections;
using System.Data;
using System.Text;

namespace Metabase.Utils
{
    public static class SQLTypeHelpers
    {
        public static string GetSqlValueLiteral(this SqlDbType type, byte[] bytes) => type switch
        {
            SqlDbType.Int => BitConverter.ToInt32(bytes).ToString(),
            SqlDbType.BigInt => BitConverter.ToInt64(bytes).ToString(),
            SqlDbType.Binary => bytes.ToString() ?? "null",
            SqlDbType.Bit => new BitArray(bytes)[0] ? "0" : "1",
            SqlDbType.Char => "'" + BitConverter.ToChar(bytes).ToString() + "'",
            SqlDbType.DateTime => new DateTime(BitConverter.ToInt64(bytes)).ToString(),
            SqlDbType.Decimal => throw new NotImplementedException(),
            SqlDbType.Float => throw new NotImplementedException(),
            SqlDbType.Image => throw new NotImplementedException(),
            SqlDbType.Money => throw new NotImplementedException(),
            SqlDbType.NChar => throw new NotImplementedException(),
            SqlDbType.NText => throw new NotImplementedException(),
            SqlDbType.NVarChar => throw new NotImplementedException(),
            SqlDbType.Real => throw new NotImplementedException(),
            SqlDbType.UniqueIdentifier => throw new NotImplementedException(),
            SqlDbType.SmallDateTime => throw new NotImplementedException(),
            SqlDbType.SmallInt => throw new NotImplementedException(),
            SqlDbType.SmallMoney => throw new NotImplementedException(),
            SqlDbType.Text => throw new NotImplementedException(),
            SqlDbType.Timestamp => throw new NotImplementedException(),
            SqlDbType.TinyInt => throw new NotImplementedException(),
            SqlDbType.VarBinary => throw new NotImplementedException(),
            SqlDbType.VarChar => "'" + Encoding.UTF8.GetString(bytes) + "'",
            SqlDbType.Variant => throw new NotImplementedException(),
            SqlDbType.Xml => throw new NotImplementedException(),
            SqlDbType.Udt => throw new NotImplementedException(),
            SqlDbType.Structured => throw new NotImplementedException(),
            SqlDbType.Date => throw new NotImplementedException(),
            SqlDbType.Time => throw new NotImplementedException(),
            SqlDbType.DateTime2 => throw new NotImplementedException(),
            SqlDbType.DateTimeOffset => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };

        public static byte[] ConvertSQLStringValueToByteArray(string v, SqlDbType type) => type switch
        {
            SqlDbType.BigInt => BitConverter.GetBytes(long.Parse(v)),
            SqlDbType.Binary => throw new NotImplementedException(),
            SqlDbType.Bit => throw new NotImplementedException(),
            SqlDbType.Char => BitConverter.GetBytes(v.ToCharArray()[0]),
            SqlDbType.DateTime => BitConverter.GetBytes(DateTime.Parse(v).ToBinary()),
            SqlDbType.Decimal => throw new NotImplementedException(),
            SqlDbType.Float => throw new NotImplementedException(),
            SqlDbType.Image => throw new NotImplementedException(),
            SqlDbType.Int => BitConverter.GetBytes(int.Parse(v)),
            SqlDbType.Money => throw new NotImplementedException(),
            SqlDbType.NChar => throw new NotImplementedException(),
            SqlDbType.NText => throw new NotImplementedException(),
            SqlDbType.NVarChar => throw new NotImplementedException(),
            SqlDbType.Real => throw new NotImplementedException(),
            SqlDbType.UniqueIdentifier => throw new NotImplementedException(),
            SqlDbType.SmallDateTime => throw new NotImplementedException(),
            SqlDbType.SmallInt => throw new NotImplementedException(),
            SqlDbType.SmallMoney => throw new NotImplementedException(),
            SqlDbType.Text => throw new NotImplementedException(),
            SqlDbType.Timestamp => throw new NotImplementedException(),
            SqlDbType.TinyInt => throw new NotImplementedException(),
            SqlDbType.VarBinary => throw new NotImplementedException(),
            SqlDbType.VarChar => Encoding.UTF8.GetBytes(v),
            SqlDbType.Variant => throw new NotImplementedException(),
            SqlDbType.Xml => throw new NotImplementedException(),
            SqlDbType.Udt => throw new NotImplementedException(),
            SqlDbType.Structured => throw new NotImplementedException(),
            SqlDbType.Date => throw new NotImplementedException(),
            SqlDbType.Time => throw new NotImplementedException(),
            SqlDbType.DateTime2 => throw new NotImplementedException(),
            SqlDbType.DateTimeOffset => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };

        public static string GetSQLTypeLiteral(SqlDbType type)
        {
            return type switch { SqlDbType.VarChar => "VARCHAR(MAX)" };
        }


        public static SqlDbType GetSqlTypeFromString(string v)
        {
            return v switch
            {
                "System.Int32" => SqlDbType.Int,
                "System.String" => SqlDbType.VarChar,
                _ => throw new Exception("type not supported" + v)
            };
        }
    }
}
