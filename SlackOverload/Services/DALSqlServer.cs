using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SlackOverload.Models;

namespace SlackOverload.Services
{
    public class DALSqlServer : IDAL
    {
        private string connectionString;

        public DALSqlServer(IConfiguration config)
        {
            connectionString = config.GetConnectionString("default");
        }

        public int CreateQuestion(Question q)
        {
            int newId;
            SqlConnection conn = null; 

            q.Posted = DateTime.Now;
            q.Status = 1; //always create status=1

            string addQuery = "INSERT INTO Questions (Username, Title, Detail, Posted, Category, Tags, Status) ";
            addQuery += "VALUES (@Username, @Title, @Detail, @Posted, @Category, @Tags, @Status);";
            addQuery += "SELECT SCOPE_IDENTITY();";


            //cleanup method 1:
            //SqlConnection conn = new SqlConnection(connectionString);

            ////ExecuteScalar returns the first row of the affected rows
            //int newId = conn.ExecuteScalar<int>(addQuery, q);

            //conn.Close();

            //method 2:
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    //ExecuteScalar returns the first row of the affected rows
            //    newId = conn.ExecuteScalar<int>(addQuery, q);
            //} //cleanup happens at the end of the using

            //method 3:
            try
            {
                conn = new SqlConnection(connectionString);
                newId = conn.ExecuteScalar<int>(addQuery, q);
            }
            catch (Exception e)
            {
                newId = -1;
                //log the error--get details from e
            }
            finally //cleanup!
            {
                if (conn != null)
                {
                    conn.Close(); //explicitly closing the connection
                }
            }

            return newId;
        }

        public IEnumerable<Answer> GetAnswersByQuestionId(int id)
        {
            //TODO: clean up connection
            SqlConnection conn = new SqlConnection(connectionString);
            string queryString = "SELECT * FROM Answers WHERE QuestionId = @id";
            return conn.Query<Answer>(queryString, new { id = id });
        }

        public Question GetQuestionById(int id)
        {
            //TODO: clean up connection
            SqlConnection conn = new SqlConnection(connectionString);
            string queryString = "SELECT * FROM Questions WHERE Id = @id";
            return conn.QueryFirstOrDefault<Question>(queryString, new { id = id });
        }

        public IEnumerable<Question> GetQuestionsMostRecent()
        {
            IEnumerable<Question> resultSet;
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(connectionString);
                string queryString = "SELECT TOP 20 * FROM Questions ORDER BY Posted DESC";
                resultSet = conn.Query<Question>(queryString);
            }
            catch (Exception e)
            {
                resultSet = null;
                //log the error
            } 
            finally //cleanup!
            {
                if (conn != null)
                {
                    conn.Close(); //explicitly closing the connection
                }
            }

            return resultSet;
        }
    }
}
