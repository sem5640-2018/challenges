using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using challenges.Models;
using challenges.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using YourApp.Services;

namespace challenges.Controllers.shared
{
    public class SharedFunctionality
    {
        private  IUserChallengeRepository userChallengeRepository;
        private  IApiClient apiClient;
        private  IConfigurationSection appConfig;

        public SharedFunctionality(IUserChallengeRepository userChallengeRepository, IApiClient apiClient, IConfiguration appConfig)
        {
            this.userChallengeRepository = userChallengeRepository;
            this.apiClient = apiClient;
            this.appConfig = appConfig.GetSection("Challenges");
        }

        public async Task<List<UserChallenge>> UpdatePercentageListAsync(List<UserChallenge> userChallenges)
        {

            
            var todayDate = DateTime.Now;
            foreach (var c in userChallenges)
            {
                var challengeStartDate = c.Challenge.StartDateTime;
                var challengeEndDate = c.Challenge.EndDateTime;
                var dateSelected = todayDate;

                if(DateTime.Compare(challengeEndDate, todayDate) <= 0)
                {
                    dateSelected = challengeEndDate;
                }


                if (DateTime.Compare(challengeStartDate, todayDate) <= 0)
                {
                    var userData = await apiClient.GetAsync(appConfig.GetValue<string>("HealthDataRepositoryUrl") + "api/Activities/ByUser/"
                                                          + c.UserId + "?from=" + challengeStartDate.ToString("yyyy-MM-dd") + "&to=" + dateSelected.Date.ToString("yyyy-MM-dd"));
                    if (userData.IsSuccessStatusCode)
                    {
                        var userDataResult = userData.Content.ReadAsStringAsync().Result;

                        await UpdatePercentageCompleteAsync(c, userDataResult);
                    }

                }

            }
            return userChallenges;
        }
        
        //this is also awful, please change
        public async Task<UserChallenge> UpdatePercentageCompleteAsync(UserChallenge userChallenge, string userDataString)
        {
            if(userDataString == "[]" || userDataString == null)
            {
                userChallenge.PercentageComplete = 0;

                await userChallengeRepository.UpdateAsync(userChallenge);
                return userChallenge;
            }
            dynamic dataString = JsonConvert.DeserializeObject(userDataString);
            var progress = 0;

            foreach (var d in dataString)
            {
                if (d.activityTypeId == userChallenge.Challenge.Activity.DbActivityId)
                {
                    switch (userChallenge.Challenge.GoalMetric.GoalMetricDbName)
                    {
                        case "caloriesBurnt":
                            progress += (int)d.caloriesBurnt;
                            break;
                        case "averageHeartRate":
                            progress += (int)d.averageHeartRate;
                            break;
                        case "stepsTaken":
                            progress += (int)d.stepsTaken;
                            break;
                        case "metresTravelled":
                            progress += (int)d.metresTravelled;
                            break;
                        case "metresElevationGained":
                            progress += (int)d.metresElevationGained;
                            break;
                        default:
                            break;
                    }
                }
            }

            double percentageComplete = ((double)progress / (double)userChallenge.Challenge.Goal)*100;
            
            userChallenge.PercentageComplete = Math.Min(100, (int)percentageComplete);

            await userChallengeRepository.UpdateAsync(userChallenge);

            return userChallenge;
        }

        public async Task UpdateAllPercentageComplete()
        {
            var userchallenges = await userChallengeRepository.GetAllAsync();
            await UpdatePercentageListAsync(userchallenges);

        }

        public async Task SendEmail()
        {
            var userchallenges = await userChallengeRepository.GetAllToSendEmail();

            foreach(UserChallenge uc in userchallenges)
            {
                var outcome = uc.PercentageComplete == 100 ? "Completed" : "Failed";
                var content = "You " + outcome + " your challenge to complete " + uc.Challenge.Goal + " " + uc.Challenge.GoalMetric.GoalMetricDisplay + " while doing " + uc.Challenge.Activity.ActivityName + " between the dates " + uc.Challenge.StartDateTime  + " - " + uc.Challenge.EndDateTime + ".";
                var payload = new
                {
                    Subject = "Challenge " + outcome,
                    Content = content,
                    UserId = uc.UserId
                };
                uc.EmailSent = true;
                await userChallengeRepository.UpdateAsync(uc);
                var userData = await apiClient.PostAsync(appConfig.GetValue<string>("CommsUrl") + "api/Email/ToUser/",payload);
            }

        }
    }
}