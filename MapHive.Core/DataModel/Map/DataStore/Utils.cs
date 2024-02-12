using Cartomatic.Utils.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel.Map
{
    public partial class DataStore
    {
        private static string _idCol = "mh_id";
        /// <summary>
        /// Identifier column for the data store tables
        /// </summary>
        /// <remarks>If you do not need to modify the id, don't... it does not change anyting, but has been enabled for backwards compatibility with some older apps</remarks>
        public static string IdCol {
            get => _idCol;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value cannot be null nor empty");

                //postgres has a 63 byte limit on the domain names
                if (value.Length > 59)
                    throw new ArgumentException("Max Length of id column is 59");

                _idCol = value.ToLower();
            }
        }

        /// <summary>
        /// Extracts first zip archive found in the path
        /// </summary>
        /// <param name="path"></param>
        public static void ExtractZip(string path)
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
            input = input.Replace(" ", "_").Replace("-", "_").Replace(".", "_").ToLowerInvariant();

            //postgres does not like col names starting with digits, so need to prefix it
            if (StartsWithDigit(input))
            {
                input = $"_{input}";
            }

            return input;
        }

        private static readonly Regex StartsWithDigitRegEx = new Regex(@"^\d+.+$");

        /// <summary>
        /// whether or not a string starts with a digit
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected internal static bool StartsWithDigit(string input)
        {
            return StartsWithDigitRegEx.IsMatch(input);
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
        public static object ParseToColumnDataType(string value, ColumnDataType ct)
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
        /// returns a create schema sql
        /// </summary>
        /// <returns></returns>
        public string GetEnsureSchemaSql() => GetEnsureSchemaSql(this);

        /// <summary>
        /// returns a create table sql
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected internal static string GetCreateTableSql(DataStore ds)
        {
            return $@"CREATE TABLE {ds.DataSource.Schema}.{ds.DataSource.Table} (
                {IdCol} serial not null,
                {string.Join(", ", ds.DataSource.Columns.Where(c => c.Virtual != true).Select(c => $"{c.Name} {ColumnDataTypeToDbType(c.Type)}"))},
                CONSTRAINT {GetPkName(ds.DataSource.Table)} PRIMARY KEY ({IdCol})
            );";
        }

        /// <summary>
        /// returns a create table sql
        /// </summary>
        /// <returns></returns>
        public string GetCreateTableSql() => GetCreateTableSql(this);

        /// <summary>
        /// returns a name for a table's primary key
        /// </summary>
        /// <param name="tblName"></param>
        /// <returns></returns>
        protected internal static string GetPkName(string tblName)
        {
            //need rnd for nested datasets that are basically named the same (only suffix differs) as the parent table and other children
            var rnd = Guid.NewGuid().ToString().Split('-').Skip(1).First();
            return $"{GetBaseIndexName(tblName)}_{rnd}_pk";
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
        /// returns a create schema sql
        /// </summary>
        /// <returns></returns>
        public string GetDropTableSql() => GetDropTableSql(this);

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
        public static string GetCreateUqIdxSql(DataStore ds, params string[] cols)
        {
            var idxName = GetSafeIndexName($"{GetBaseIndexName(ds.DataSource.Table)}_uq_{string.Join("_", cols)}");
            
            return $@"CREATE UNIQUE INDEX {idxName}
                ON {ds.DataSource.Schema}.{ds.DataSource.Table} USING btree ({string.Join(", ", cols)});";
        }

        /// <summary>
        /// returns a safe index name
        /// </summary>
        /// <param name="idxName"></param>
        /// <returns></returns>
        protected internal static string GetSafeIndexName(string idxName)
        {
            if (idxName.Length > 59)
            {
                //postgres domain names are by default limited to a length of 63 bytes
                idxName = idxName.Substring(0, 59);
            }

            return $"{idxName}_idx";
        }

        /// <summary>
        /// returns a create idx sql
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static string GetCreateIdxSql(DataStore ds, params string[] cols)
        {
            var idxName = GetSafeIndexName($"{GetBaseIndexName(ds.DataSource.Table)}_{string.Join("_", cols)}");
            return $@"CREATE INDEX {idxName}
                ON {ds.DataSource.Schema}.{ds.DataSource.Table} USING btree ({string.Join(", ", cols)});";
        }

        /// <summary>
        /// returns sql that calculates bbox in the ds srid
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected internal static string GetGeomBBoxSql(DataStore ds)
        {
            return $@"select min(ST_XMin(geom)) as l, min(ST_YMin(geom)) as b, max(ST_XMax(geom)) as r, max(ST_YMax(geom)) as t
from {ds.DataSource.Schema}.{ds.DataSource.Table}";
        }

        /// <summary>
        /// returns sql that calculates ds bbox in given srid
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="outSrid"></param>
        /// <returns></returns>
        protected internal static string GetGeomBBoxSql(DataStore ds, int? outSrid)
        {
            return $@"select
	ST_XMin(geom) as l,
	ST_YMin(geom) as b,
	ST_XMax(geom) as r,
	ST_YMax(geom) as t
from (

	select
		st_union(
			st_transform(st_setsrid(st_makepoint(l,b),srid),{outSrid?.ToString() ?? "srid"}),
			st_transform(st_setsrid(st_makepoint(r,t),srid),{outSrid?.ToString() ?? "srid"})
		) as geom
	from (
		select 
			min(ST_XMin(geom)) as l,
			min(ST_YMin(geom)) as b,
			max(ST_XMax(geom)) as r,
			max(ST_YMax(geom)) as t,
			max(ST_SRID(geom)) as srid
		from {ds.DataSource.Schema}.{ds.DataSource.Table}
	) as bbox_agg
) as transformed_bbox";
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
({string.Join(",", ds.DataSource.Columns.Where(c => c.Virtual != true).Select(c => c.Name))})
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
        /// <param name="colCnt"></param>
        /// <returns></returns>
        protected internal static async Task ExecuteUpsertBatch(Npgsql.NpgsqlCommand cmd, DataStore ds, List<string> insertData, int colCnt, IEnumerable<string> key, IEnumerable<string> skipCols)
        {
            var colNameRow = string.Empty;
            var discardColNameRowData = string.Empty;
            if (key?.Any() == true)
            {
                var colsData = new string[colCnt];
                var keyCols = key.ToArray();
                for (var colIdx = 0; colIdx < colCnt; colIdx++)
                {
                    colsData[colIdx] = colIdx < keyCols.Length ? $"NULL as {keyCols[colIdx]}" : "NULL";
                }

                colNameRow = $"SELECT {string.Join(",", colsData)} UNION ALL";

                discardColNameRowData = $"WHERE {string.Join(" IS NOT NULL AND ", key)} IS NOT NULL";
            }

            cmd.CommandText = $@"INSERT INTO {ds.DataSource.Schema}.{ds.DataSource.Table}
({string.Join(",", ds.DataSource.Columns.Where(c => c.Virtual != true && (skipCols == null || !skipCols.Contains(c.Name))).Select(c => c.Name))})

{(key?.Any() == true ? $"SELECT DISTINCT ON ({string.Join(",", key)}) * FROM (" : string.Empty)}

{colNameRow}
{string.Join($"{Environment.NewLine} UNION ALL ", insertData)}

{(key?.Any() == true ? ") as data" : string.Empty)}
{discardColNameRowData}

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
                //need to ignore the cols that:
                //* virtual columns
                //* make up a key, as they should not be updated
                //* columns to skip - in upsert mode the original model is not changed, and therefore need to exclude columns that are not updated in a specified batch
                if (c.Virtual == true || key.Contains(c.Name) || skipCols?.Contains(c.Name) == true)
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
        public static DataStore GetDataStore(string fName, string type, DataSourceCredentials ds, string schema = "imported_geodata")
        {
            if (!CheckIfObjectSafe(fName))
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("FileName", "bad_file_name",
                    "File name contains forbidden words");

            //postgres has a 63 byte limit on the domain names
            var tblName = $"l_{DateTime.Now:yyyyMMddHHmmss}_{type}_{GetSafeDbObjectName(fName)}";
            if (tblName.Length > 63)
                tblName = tblName.Substring(0, 63);
            

            return new DataStore
            {
                Name = fName,
                DataSource = new DataSource
                {
                    DataSourceCredentials = ds,
                    Schema = schema,
                    Table = tblName,
                    Columns = new List<Column>()
                }
            };
        }

        /// <summary>
        /// calculates a bbox of a data store and populates the data store's minx, miny, maxx, maxy properties
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ds"></param>
        /// <param name="outSrid"></param>
        /// <returns></returns>
        protected internal static async Task CalculateAndApplyBBox(Npgsql.NpgsqlCommand cmd, DataStore ds, int? outSrid = null)
        {
            var bbox = await CalculateBBox(cmd, ds, outSrid);

            ds.MinX = bbox.l;
            ds.MinY = bbox.b;
            ds.MaxX = bbox.r;
            ds.MaxY = bbox.t;
        }

        /// <summary>
        /// Calculates 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ds"></param>
        /// <param name="outSrid"></param>
        /// <returns></returns>
        public static async Task<(double l, double b, double r, double t)> CalculateBBox(Npgsql.NpgsqlCommand cmd, DataStore ds, int? outSrid = null)
        {

            cmd.CommandText = outSrid.HasValue
                ? GetGeomBBoxSql(ds, outSrid)
                : GetGeomBBoxSql(ds);

            var output = (0d, 0d, 0d, 0d);

            using (var rdr = await cmd.ExecuteReaderAsync())
            {
                if (rdr.HasRows)
                {
                    //if any rows, there should be exactly one
                    await rdr.ReadAsync();

                    output = ((double) rdr[0], (double) rdr[1], (double) rdr[2], (double) rdr[3]);
                }
            }

            return output;
        }
    }
}
