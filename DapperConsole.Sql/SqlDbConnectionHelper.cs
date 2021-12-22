using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DapperConsole.Sql
{
    public class SqlDbConnectionHelper
    {
        private readonly string _connString;
        private readonly IsolationLevel _level;

        public static SqlDbConnectionHelper Create(string connString) =>
             new SqlDbConnectionHelper(connString, IsolationLevel.ReadCommitted);

        public static SqlDbConnectionHelper Create(string connString, IsolationLevel level) =>
            new SqlDbConnectionHelper(connString, level);

        private SqlDbConnectionHelper(string connString, IsolationLevel level)
        {
            _connString = connString;
            _level = level;
        }

        public T TransactionExecute<T>(Func<SqlConnection, SqlTransaction, T> action)
        {
            T result;
            using (var conn = new SqlConnection(_connString))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction(_level);
                    result = action.Invoke(conn, trans);
                    trans.Commit();
                }
                catch (Exception)
                {
                    Rollback(trans);
                    throw;
                }
                finally
                {
                    trans?.Dispose();
                }
            }
            return result;
        }

        public void TransactionExecute(Action<SqlConnection, SqlTransaction> action)
        {
            using (var conn = new SqlConnection(_connString))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction(_level);
                    action.Invoke(conn, trans);
                    trans.Commit();
                }
                catch (Exception)
                {
                    Rollback(trans);
                    throw;
                }
                finally
                {
                    trans?.Dispose();
                }
            }
        }

        private static void Rollback(IDbTransaction trans)
        {
            try
            {
                trans?.Rollback();
            }
            catch
            {
                // ignored
            }
        }

        public void Execute(Action<SqlConnection> action)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                action.Invoke(conn);
            }
        }

        public T Execute<T>(Func<SqlConnection, T> action)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                return action.Invoke(conn);
            }
        }

#if !NET40
        public async Task ExecuteAsync(Func<SqlConnection, Task> action)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                await action.Invoke(conn).ConfigureAwait(false);
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<SqlConnection, Task<T>> action)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                return await action.Invoke(conn).ConfigureAwait(false);
            }
        }

        public async Task<T> TransactionExecuteAsync<T>(Func<SqlConnection, SqlTransaction, Task<T>> action)
        {
            T result;
            using (var conn = new SqlConnection(_connString))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction(_level);
                    result = await action.Invoke(conn, trans).ConfigureAwait(false);
                    trans.Commit();
                }
                catch (Exception)
                {
                    Rollback(trans);
                    throw;
                }
                finally
                {
                    trans?.Dispose();
                }
            }
            return result;
        }

        public async Task TransactionExecuteAsync(Func<SqlConnection, SqlTransaction, Task> action)
        {
            using (var conn = new SqlConnection(_connString))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction(_level);
                    await action.Invoke(conn, trans).ConfigureAwait(false);
                    trans.Commit();
                }
                catch (Exception)
                {
                    Rollback(trans);
                    throw;
                }
                finally
                {
                    trans?.Dispose();
                }
            }
        }
#endif
    }
}

