using GeekQuiz.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeekQuiz.Controllers
{
    [Authorize]
    public class TriviaController : ApiController
    {
        TriviaContext db = new TriviaContext();

        public async Task<IHttpActionResult> Get()
        {
            string userId = User.Identity.Name;

            TriviaQuestion triviaQuestion = await NextQuestionAsync(userId);

            if (triviaQuestion == null)
            {
                return NotFound();
            }

            return Ok(triviaQuestion);
        }

        public async Task<IHttpActionResult> Post(TriviaAnswer triviaAnswer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            triviaAnswer.UserId = User.Identity.Name;

            bool isCorrect = await StoreAsync(triviaAnswer);

            return Ok<bool>(isCorrect);
        }

        private async Task<bool> StoreAsync(TriviaAnswer triviaAnswer)
        {
            db.TriviaAnswers.Add(triviaAnswer);

            await db.SaveChangesAsync();

            var selectedOption = await db.TriviaOptions.FirstOrDefaultAsync(o => o.Id == triviaAnswer.OptionId
                && o.QuestionId == triviaAnswer.QuestionId);

            return selectedOption.IsCorrect;
        }

        private async Task<TriviaQuestion> NextQuestionAsync(string userId)
        {
            var lastQuestionId = await db.TriviaAnswers
                .Where(a => a.UserId == userId)
                .GroupBy(a => a.QuestionId)
                .Select(g => new { QuestionId = g.Key, Count = g.Count() })
                .OrderByDescending(q => new { q.Count, q.QuestionId })
                .Select(q => q.QuestionId)
                .FirstOrDefaultAsync();

            var questionsCount = db.TriviaQuestions.Count();

            var nextQuestionId = (lastQuestionId % questionsCount) + 1;

            return await db.TriviaQuestions.FindAsync(nextQuestionId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
