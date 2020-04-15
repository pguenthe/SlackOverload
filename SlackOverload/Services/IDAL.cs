using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SlackOverload.Models;

namespace SlackOverload.Services
{
    public interface IDAL
    {
        public int CreateQuestion(Question q);

        public IEnumerable<Answer> GetAnswersByQuestionId(int id);

        public Question GetQuestionById(int id);

        public IEnumerable<Question> GetQuestionsMostRecent();
    }
}
