using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SlackOverload.Models;
using SlackOverload.Services;

namespace SlackOverload.Controllers
{
    public class QandAController : Controller
    {
        private IDAL dal;

        public QandAController(IDAL dalObject)
        {
            dal = dalObject;   
        }

        public IActionResult Index()
        {
            //get the most recent questions
            IEnumerable<Question> results = dal.GetQuestionsMostRecent();

            ViewData["Questions"] = results;

            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Question());
        }

        [HttpPost]
        public IActionResult Add(Question q)
        {
            //TODO: Validate before submitting
            // Then I can get the user a more detailed message: What's missing?
            int newId = dal.CreateQuestion(q);

            if (newId == -1)
            {
                ViewData["ErrorMsg"] = "There was an errror. Please check your form for missing information.";
                return View("Add", q);//pre-fill the user's info received
            }

            return RedirectToAction("Detail", new { id = newId });

            //return RedirectToAction("Index");
        }

        public IActionResult Detail(int id) {
            //first get the question detail
            Question question = dal.GetQuestionById(id);

            //then get the relevant answers
            IEnumerable<Answer> answers = dal.GetAnswersByQuestionId(id);

            ViewData["Answers"] = answers;

            return View(question);
        }
    }
}