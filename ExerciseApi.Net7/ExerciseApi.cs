using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.Models;
using System.Runtime.InteropServices;

namespace ExerciseApi.Net7
{
    public class ExerciseApi : MiddleWare
    {

        // CreateRandomDailyExercise
        public static async Task<bool> CreateRandomDailyExercise(long UserID, int offset)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Exercise/CreateRandomDailyExercise?UserID={1}&offset={2}", BaseUrl, UserID, offset), null))
                {
                    //string apiResponse = await response.Content.ReadAsStringAsync();
                    // return JsonConvert.DeserializeObject<bool>(apiResponse);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        // Start Methods
        public static async Task<bool> StartTrainingSession(long TrainingSessionID)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/{1}{2}", BaseUrl, "api/Exercise/StartTrainingSession?TrainingSessionID=", TrainingSessionID), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //PauseTrainingSession

        public static async Task<bool> PauseTrainingSession(long TrainingSessionID, int Duration)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Exercise/PauseTrainingSession?TrainingSessionID={1}&Duration={2}", BaseUrl, TrainingSessionID, Duration), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> StartExercise(long ExerciseID)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/{1}{2}", BaseUrl, "api/Exercise/StartExercise?ExerciseID=", ExerciseID), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> StartSet(long SetID)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/{1}{2}", BaseUrl, "api/Exercise/StartSet?SetID=", SetID), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Reschedule SNOOZE

        public static async Task<bool> RescheduleTrainingSession(long TrainingSessionID, int MinutesSnooze, long Reason4Rescheduling = 1)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Exercise/RescheduleTrainingSession?TrainingSessionID={1}&Reason4Rescheduling={2}&MinutesSnooze={3}", BaseUrl, TrainingSessionID, Reason4Rescheduling, MinutesSnooze), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Skip Methods

        public static async Task<bool> SkipTrainingSession(long TrainingSessionID, string Reason4Skipping = "")
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Exercise/SkipTrainingSession?TrainingSessionID={1}&Reason4Skipping={2}", BaseUrl, TrainingSessionID, Reason4Skipping), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> SkipExercise(long ExerciseID)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/{1}{2}", BaseUrl, "api/Exercise/SkipExercise?ExerciseID=", ExerciseID), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> SkipSet(long SetID)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/{1}{2}", BaseUrl, "api/Exercise/SkipSet?SetID=", SetID), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> ChangeSetMetrics(long SetID, long SetMetricsId, double newalue)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Exercise/ChangeSetMetrics?SetID={1}&SetMetricsId={2}&newalue={3}", BaseUrl, SetID, SetMetricsId, newalue), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        

        // End(Complete) Methods
        public static async Task<bool> EndTrainingSession(long TrainingSessionID, int Duration)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Exercise/EndTrainingSession?TrainingSessionID={1}&Duration={2}", BaseUrl, TrainingSessionID, Duration), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> EndExercise(long ExerciseID)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/{1}{2}", BaseUrl, "api/Exercise/EndExercise?ExerciseID=", ExerciseID), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> EndSet(long SetID)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/{1}{2}", BaseUrl, "api/Exercise/EndSet?SetID=", SetID), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> UndoEndSet(long SetID)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/{1}{2}", BaseUrl, "api/Exercise/UndoEndSet?SetID=", SetID), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static async Task<bool> UndoSkipTrainingSeseesion(long SessionId)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/{1}{2}&now={3}", BaseUrl, "api/Exercise/UndoSkipTrainingSeseesion?SessionId=", SessionId,  DateTime.Now), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //setMetricsId  = Id of the set as in <EmSetMetrics> that the user edited 
        // metricToUpdate  -- must have same Id as the original EmSetMetrics
        // newValue -- to the value the user entered. Thats the only value he can change
        public static async Task<bool> UpdateSet(long setMetricsId, double newValue)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Exercise/UpdateSet?setMetricsId={1}&newValue={2}", BaseUrl, setMetricsId, newValue), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // ExerciseId -- the Id of the EmExercise where the set is added to
        // EmSetId  -- the ID of the set to use as a template:  this is always the ID of the last set of that exercise
        public static async Task<long> AddNewSet(long ExerciseId, long EmSetId = -1)
        {
            try
            {
                var EmSet = await AddNewSetAndReturn(ExerciseId, EmSetId);
                return EmSet.Id;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static async Task<EmSet> AddNewSetAndReturn(long ExerciseId, long EmSetId = -1)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Exercise/AddNewSet?ExerciseId={1}&EmSetId={2}", BaseUrl, ExerciseId, EmSetId), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<EmSet>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<long> AddNewExercise(long TrainingSessionId)
        {
            try
            {
                var EmExercise = await AddNewExerciseAndReturn(TrainingSessionId);
                return EmExercise.Id;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static async Task<EmExercise> AddNewExerciseAndReturn(long TrainingSessionId)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Exercise/AddNewExercise?TrainingSessionId={1}", BaseUrl, TrainingSessionId), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return  JsonConvert.DeserializeObject<EmExercise>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public static async Task<EmExercise> GetExercise(long ExerciseId)
        //{
        //    try
        //    {
        //        using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetExercise?ExerciseId={1}", BaseUrl, ExerciseId)))
        //        {
        //            string apiResponse = await response.Content.ReadAsStringAsync();
        //            return JsonConvert.DeserializeObject<EmExercise>(apiResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public static async Task<EmSet> GetSetById(long SetID)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetSetById?SetID={1}", BaseUrl, SetID)))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<EmSet>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static async Task<List<EmExerciseType>> GetExerciseTypes()
        {
            try
            {
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetExerciseTypes", BaseUrl)))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<EmExerciseType>>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<List<EmEquipment>> GetEquipments()
        {
            try
            {
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetEquipments", BaseUrl)))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<EmEquipment>>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<EmExercise> GetDefaultExerciseFromExerciseType(long ExerciseTypeId, long TraningsessionID)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetDefaultExerciseFromExerciseType?ExerciseTypeId={1}&TraningsessionID={2}", BaseUrl, ExerciseTypeId, TraningsessionID)))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<EmExercise>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<List<EmMainMuscleWorked>> GetMainMuscleWorked()
        {
            try
            {
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetMainMuscleWorked", BaseUrl)))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<EmMainMuscleWorked>>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        // Sort Functions

        public static List<EmExerciseType> SortExerciseTypeByEquipment(List<EmExerciseType> ExerciseTypes, string EquipmentName)
        {
            try
            {
               return ExerciseTypes.Where(t => t.EmEquipment.Name == EquipmentName).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<EmExerciseType> SortExerciseTypeMuscleWorked(List<EmExerciseType> ExerciseTypes, string MuscleWorked)
        {
            try
            {
                return ExerciseTypes.Where(t => t.EmOtherMuscleWorked.Name == MuscleWorked).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }





        public static async Task<EmTrainingSession> GetTrainingSession(long TraningSessionId)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetTrainingSession?TraningSessionId={1}", BaseUrl, TraningSessionId)))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<EmTrainingSession>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static async Task<bool> SetFeedBack(long traningsessionId, float sliderFeedback, string Textfeedback)
        {

            return true;
            //try
            //{
            //    using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetTrainingSession?TraningSessionId={1}", BaseUrl, TraningSessionId)))
            //    {
            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        return JsonConvert.DeserializeObject<EmTrainingSession>(apiResponse);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }




        //GetDefaultSetsForExerciseType


    }
}