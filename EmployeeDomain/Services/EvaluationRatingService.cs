using EmployeeDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDomain.Services
{
    public class EvaluationRatingService : IEvaluationRatingService
    {
        public string GetRating(decimal finalScore)
        {
            if (finalScore < 0 || finalScore > 5)
                throw new DomainException("Final score must be between 0 and 5.");
            return finalScore switch
            {
                >= 4.5m and <= 5m => "Excellent",
                >= 4.0m and < 4.5m => "Very Good",
                >= 3.5m and < 4.0m => "Good",
                _ => "Bad"
            };
        }
    }
}
