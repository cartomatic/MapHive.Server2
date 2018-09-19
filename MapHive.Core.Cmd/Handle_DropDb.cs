using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using Cartomatic.Utils.Data;
using Npgsql;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler
    {
        /// <summary>
        /// Handles dropping a database
        /// </summary>
        /// <param name="args"></param>
        protected virtual void Handle_DropDb(IDictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : drops a specified database; uses the configured db credentials to connect to the db server.");
                Console.WriteLine($"syntax: {cmd} dbName");
                Console.WriteLine($"example: {cmd} someDbName");
                Console.WriteLine();

                return;
            }

            if (RemoteMode)
            {
                ConsoleEx.WriteLine($"{nameof(Handle_DropDb)} works only in direct db mode, skipping!", ConsoleColor.DarkGray);
                Console.WriteLine();
                return;
            }

            //expect the args to contain one entry only
            if (args.Count != 1)
            {
                ConsoleEx.WriteErr($"{cmd} command expects exactly one param!");
                Console.WriteLine();
                return;
            }

            var dbName = args.First().Key;

            if (PromptUser($"Are you sure you want to drop '{dbName}' database?"))
            {
                DropDb(dbName);
            }
        }

        /// <summary>
        /// Drops the specified dbs
        /// </summary>
        /// <param name="dbNames"></param>
        protected virtual void DropDb(params string[] dbNames)
        {
            //wipe out the db name for the connection as need to connect to the service db!
            var curentDbName = Dsc.DbName;
            Dsc.DbName = string.Empty;

            try
            {
                foreach (var dbName in dbNames)
                {
                    ConsoleEx.Write($"Dropping db: {dbName}... ", ConsoleColor.DarkYellow);
                    
                    //first cut the connection to the db if any
                    using (var conn = new NpgsqlConnection(Dsc.GetConnectionString(serviceDatabase: true)))
                    {
                        conn.Open();

                        var cmd = new NpgsqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText =
                            $"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '{dbName}';";

                        cmd.ExecuteNonQuery();

                        conn.CloseConnection();
                    }

                    //it should be possible to drop the db now
                    using (var conn = new NpgsqlConnection(Dsc.GetConnectionString(serviceDatabase: true)))
                    {
                        conn.Open();

                        var cmd = new NpgsqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = $"DROP DATABASE IF EXISTS {dbName}";

                        cmd.ExecuteNonQuery();

                        conn.CloseConnection();
                    }

                    ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                //restore previously set dbname
                Dsc.DbName = curentDbName;
            }
        }

        /// <summary>
        /// Drops specified schemas
        /// </summary>
        /// <param name="schemaNames">k,v: schemaname,dbname</param>
        protected virtual void DropSchema(Dictionary<string, string> schemaNames)
        {
            //cache whatever is the current db name, as it will be restored later; in order to drop schema, need to connect to a particular db!
            var curentDbName = Dsc.DbName;
            
            try
            {

                foreach (var schema in schemaNames)
                {
                    Dsc.DbName = schema.Value;

                    ConsoleEx.Write($"Dropping schema: {schema.Key}... ", ConsoleColor.DarkYellow);

                    //FIXME - need to check if db exists, oherwise will fail...

                    using (var conn = new NpgsqlConnection(Dsc.GetConnectionString()))
                    {
                        conn.Open();

                        var cmd = new NpgsqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = $"DROP SCHEMA IF EXISTS {schema.Key} CASCADE;";

                        cmd.ExecuteNonQuery();

                        conn.CloseConnection();
                    }

                    ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                //restore previously set dbname
                Dsc.DbName = curentDbName;
            }
        }
    }
}
