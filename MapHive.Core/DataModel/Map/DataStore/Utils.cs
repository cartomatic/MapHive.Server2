using Cartomatic.Utils.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel.Map
{
    public partial class DataStore
    {
        /// <summary>
        /// Extracts first zip archive found in the path
        /// </summary>
        /// <param name="path"></param>
        protected static void ExtractZip(string path)
        {
            var zip = Directory.GetFiles(path, "*.zip").FirstOrDefault();
            if (!string.IsNullOrEmpty(zip))
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(zip, path);
            }
        }

        /// <summary>
        /// normalizes a table or column name and returns one that should be safe to use in sql
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected internal static string GetSafeDbObjectName(string input)
        {
            return input.Replace(" ", "_").Replace("-", "_").Replace(".", "_").ToLowerInvariant();
        }

        protected static string[] SqlBlackList = { ";", "--", "drop", "create", "alter", "select", "update" };

        /// <summary>
        /// Whether or not a string seems to be safe from the sql injection point of view
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected internal static bool CheckIfObjectSafe(string input)
        {
            return input.ToLowerInvariant().Split(' ').All(s => !SqlBlackList.Contains(s));
        }

        /// <summary>
        /// returns db sql type for column data type 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        protected internal static string ColumnDataTypeToDbType(ColumnDataType ct)
        {
            switch (ct)
            {
                case ColumnDataType.Integer:
                    return "integer";
                case ColumnDataType.Long:
                    return "bigint";
                case ColumnDataType.Number:
                    return "numeric";
                case ColumnDataType.Date:
                    return "timestamp without time zone";
                case ColumnDataType.Bool:
                    return "boolean";
                case ColumnDataType.String:
                    return "text";
                case ColumnDataType.Geom:
                    return "geometry";
                default:
                    throw new Exception($"Unknown column data type: {ct}");
            }
        }

        /// <summary>
        /// returns a npgsql db type for column type
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static NpgsqlDbType ColumnDataTypeToNpgsqlDbType(ColumnDataType ct)
        {
            switch (ct)
            {
                case ColumnDataType.Integer:
                    return NpgsqlDbType.Integer;
                case ColumnDataType.Long:
                    return NpgsqlDbType.Bigint;
                case ColumnDataType.Number:
                    return NpgsqlDbType.Numeric;
                case ColumnDataType.Date:
                    return NpgsqlDbType.Timestamp; //"timestamp without time zone";
                case ColumnDataType.Bool:
                    return NpgsqlDbType.Boolean;
                case ColumnDataType.String:
                    return NpgsqlDbType.Text;
                default:
                    throw new Exception($"Unknown NpgsqlDbType data type: {ct}");
            }
        }

        /// <summary>
        /// returns a null value db param for column
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        protected internal static NpgsqlParameter GetDbNullValueParamForColumn(string pName, Column c)
        {
            var p = new NpgsqlParameter(pName, ColumnDataTypeToNpgsqlDbType(c.Type)) { Value = DBNull.Value };

            return p;
        }

        /// <summary>
        /// parses a string value to column type value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        protected internal static object ParseToColumnDataType(string value, ColumnDataType ct)
        {
            if (value == null)
                return null;

            object output = null;
            switch (ct)
            {
                case ColumnDataType.Integer:
                    if (int.TryParse(value, out var parsedInt))
                        output = parsedInt;
                    break;
                case ColumnDataType.Long:
                    if (long.TryParse(value, out var parsedLong))
                        output = parsedLong;
                    break;
                case ColumnDataType.Number:
                    if (decimal.TryParse(value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedNum))
                        output = parsedNum;
                    break;
                case ColumnDataType.Date:
                    if (DateTime.TryParse(value, out var parsedDate))
                        output = parsedDate;
                    break;
                case ColumnDataType.Bool:
                    if (bool.TryParse(value, out var parsedBool))
                        output = parsedBool;
                    break;
                case ColumnDataType.String:
                    output = value;
                    break;
            }

            return output;
        }


        protected static Type[] typesInt = new[] { typeof(int), typeof(short), typeof(ushort), typeof(uint) };
        protected static Type[] typesLong = new[] { typeof(long), typeof(ulong) };
        protected static Type[] typesNumber = new[] { typeof(float), typeof(double), typeof(decimal) };

        /// <summary>
        /// Works out the col data type from system type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected internal static ColumnDataType SystemTypeToColumnDataType(Type t)
        {
            if (typesInt.Any(tt => tt == t))
            {
                return ColumnDataType.Integer;
            }
            else if (typesLong.Any(tt => tt == t))
            {
                return ColumnDataType.Long;
            }
            else if (typesNumber.Any(tt => tt == t))
            {
                return ColumnDataType.Number;
            }
            else if (t == typeof(string))
            {
                return ColumnDataType.String;
            }
            else if (t == typeof(bool))
            {
                return ColumnDataType.Bool;
            }
            else if (t == typeof(DateTime))
            {
                return ColumnDataType.Date;
            }

            return ColumnDataType.Unknown;
        }

        /// <summary>
        /// returns a create schema sql
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected internal static string GetEnsureSchemaSql(DataStore ds)
        {
            return $"CREATE SCHEMA IF NOT EXISTS {ds.DataSource.Schema};";
        }

        /// <summary>
        /// returns a create table sql
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected internal static string GetCreateTableSql(DataStore ds)
        {
            return $@"CREATE TABLE {ds.DataSource.Schema}.{ds.DataSource.Table} (
                td_id serial not null,
                {string.Join(", ", ds.DataSource.Columns.Select(c => $"{c.Name} {ColumnDataTypeToDbType(c.Type)}"))},
                CONSTRAINT {ds.DataSource.Table}_pk PRIMARY KEY (td_id)
            );";
        }

        /// <summary>
        /// returns a drop table sql
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected internal static string GetDropTableSql(DataStore ds)
        {
            return $@"DROP TABLE IF EXISTS {ds.DataSource.Schema}.{ds.DataSource.Table} CASCADE;";
        }

        /// <summary>
        /// Gets base index name from table name
        /// </summary>
        /// <param name="tblName"></param>
        /// <returns></returns>
        protected internal static string GetBaseIndexName(string tblName)
        {
            //from name such as l_20190921181613_occupancy_amsterdam_noord_with_date_shp
            //should return l_20190921181613
            return string.Join("_", tblName.Split('_').Take(2));
        }

        /// <summary>
        /// returns a create geom idx sql
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected internal static string GetCreateGeomIdxSql(DataStore ds)
        {
            return $@"CREATE INDEX {GetBaseIndexName(ds.DataSource.Table)}_geom_idx
                ON {ds.DataSource.Schema}.{ds.DataSource.Table} USING gist (geom);";
        }

        /// <summary>
        /// returns a create unique idx sql
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        protected internal static string GetCreateUqIdxSql(DataStore ds, params string[] cols)
        {
            return $@"CREATE UNIQUE INDEX {GetBaseIndexName(ds.DataSource.Table)}_uq_{string.Join("_", cols)}_idx
                ON {ds.DataSource.Schema}.{ds.DataSource.Table} USING btree ({string.Join(", ", cols)});";
        }

        /// <summary>
        /// returns a create idx sql
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        protected internal static string GetCreateIdxSql(DataStore ds, params string[] cols)
        {
            return $@"CREATE INDEX {GetBaseIndexName(ds.DataSource.Table)}_{string.Join("_", cols)}_idx
                ON {ds.DataSource.Schema}.{ds.DataSource.Table} USING btree ({string.Join(", ", cols)});";
        }

        /// <summary>
        /// returns sql that calculates geom bbox
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected internal static string GetGeomBBoxSql(DataStore ds)
        {
            return $@"select min(ST_XMin(geom)) as l, min(ST_YMin(geom)) as b, max(ST_XMax(geom)) as r, max(ST_YMax(geom)) as t
from {ds.DataSource.Schema}.{ds.DataSource.Table}";
        }

        /// <summary>
        /// creates an index on a location column if it is present in the data store and of string data type
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected static async Task CreateLocationIndex(Npgsql.NpgsqlCommand cmd, DataStore ds)
        {
            //location col indexing as required
            var locationCol = ds.DataSource.Columns.FirstOrDefault(c => c.Name == "location");
            if (locationCol != null && locationCol.Type == ColumnDataType.String)
            {
                cmd.CommandText = GetCreateIdxSql(ds, "location");
                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// executes an insert batch sql against a datastore
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ds"></param>
        /// <param name="insertData"></param>
        /// <returns></returns>
        protected internal static async Task ExecuteInsertBatch(Npgsql.NpgsqlCommand cmd, DataStore ds, List<string> insertData)
        {
            cmd.CommandText = $@"INSERT INTO {ds.DataSource.Schema}.{ds.DataSource.Table}
({string.Join(",", ds.DataSource.Columns.Select(c => c.Name))})
{string.Join($"{Environment.NewLine} UNION ALL ", insertData)}
;";
            await cmd.ExecuteNonQueryAsync();

            cmd.Parameters.Clear();
            insertData.Clear();
        }

        /// <summary>
        /// executes an upsert sql against a data store
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ds"></param>
        /// <param name="insertData"></param>
        /// <param name="key"></param>
        /// <param name="skipCols"></param>
        /// <returns></returns>
        protected internal static async Task ExecuteUpsertBatch(Npgsql.NpgsqlCommand cmd, DataStore ds, List<string> insertData, IEnumerable<string> key, IEnumerable<string> skipCols)
        {
            cmd.CommandText = $@"INSERT INTO {ds.DataSource.Schema}.{ds.DataSource.Table}
({string.Join(",", ds.DataSource.Columns.Select(c => c.Name))})
{string.Join($"{Environment.NewLine} UNION ALL ", insertData)}
ON CONFLICT({string.Join(",", key)}) DO UPDATE SET
{GetDoUpdateCols(ds, key, skipCols)}
;";

            await cmd.ExecuteNonQueryAsync();

            cmd.Parameters.Clear();
            insertData.Clear();
        }

        /// <summary>
        /// returns a sql cols value set for on conflict handling
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="key"></param>
        /// <param name="skipCols"></param>
        /// <returns></returns>
        protected internal static string GetDoUpdateCols(DataStore ds, IEnumerable<string> key, IEnumerable<string> skipCols)
        {
            var updates = new List<string>();

            foreach (var c in ds.DataSource.Columns)
            {
                //need to ignore the cols that make up a key, as they should not be updated
                //and columns to skip - in upsert mode the original model is not changed, and therefore need to exclude columns that are not updated in a specified batch
                if (key.Contains(c.Name) || skipCols?.Contains(c.Name) == true)
                    continue;

                updates.Add($"{c.Name} = excluded.{c.Name}");
            }

            return string.Join(",", updates);
        }


        /// <summary>
        /// drops a table of a data set
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected internal static async Task ExecuteTableDropAsync(DataStore ds)
        {
            using (var conn = new NpgsqlConnection(ds.DataSource.DataSourceCredentials.GetConnectionString()))
            using (var cmd = new NpgsqlCommand())
            {
                await conn.OpenAsync();
                cmd.Connection = conn;

                cmd.CommandText = GetEnsureSchemaSql(ds);
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = GetDropTableSql(ds);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// creates a datastore object for given file and declared type
        /// </summary>
        /// <param name="fName"></param>
        /// <param name="type"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        protected internal static DataStore GetDataStore(string fName, string type, string schema = "imported_geodata")
        {
            if (!CheckIfObjectSafe(fName))
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("FileName", "bad_file_name",
                    "File name contains forbidden words");

            return new DataStore
            {
                Name = fName,
                DataSource = new DataSource
                {
                    DataSourceCredentials = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig().GetSection("GeoDatabase")
                        .Get<DataSourceCredentials>(),
                    Schema = schema,
                    Table = $"l_{DateTime.Now:yyyyMMddHHmmss}_{type}_{GetSafeDbObjectName(fName)}",
                    Columns = new List<Column>()
                }
            };
        }

        /// <summary>
        /// calculates a bbox of a data store
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected internal static async Task CalculateBBox(Npgsql.NpgsqlCommand cmd, DataStore ds)
        {

            cmd.CommandText = GetGeomBBoxSql(ds);
            using (var rdr = await cmd.ExecuteReaderAsync())
            {
                if (rdr.HasRows)
                {
                    //if any rows, there should be exactly one
                    await rdr.ReadAsync();

                    ds.MinX = (double)rdr[0];
                    ds.MinY = (double)rdr[1];
                    ds.MaxX = (double)rdr[2];
                    ds.MaxY = (double)rdr[3];
                }
            }
        }
    }
}
