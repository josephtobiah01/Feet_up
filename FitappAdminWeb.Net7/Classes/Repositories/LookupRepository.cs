using AutoMapper;
using DAOLayer.Net7.Exercise;
using FitappAdminWeb.Net7.Classes.DTO;
using FitappAdminWeb.Net7.Models;
using Microsoft.EntityFrameworkCore;

namespace FitappAdminWeb.Net7.Classes.Repositories
{
    /// <summary>
    /// Repository for handling various Lookups
    /// </summary>
    public class LookupRepository
    {
        ExerciseContext _dbcontext;
        ILogger<LookupRepository> _logger;
        IMapper _mapper;
        public LookupRepository(ExerciseContext dbcontext, ILogger<LookupRepository> logger, IMapper mapper)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<EdsExerciseType>> GetExerciseTypes()
        {
            var query = _dbcontext.EdsExerciseType.Where(r => !r.IsDeleted);

            return await query.ToListAsync();
        }

        public async Task<EdsExerciseType?> GetExerciseTypeById(long id)
        {
            var query = _dbcontext.EdsExerciseType.Where(r => r.Id == id);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<EdsSetMetricTypes>> GetSetMetricTypes()
        {
            var query = _dbcontext.EdsSetMetricTypes
                .Include(m => m.EdsAvaiableSetMetrics)
                .Include(m => m.EdsSetMetricsDefault);
            return await query.ToListAsync();
        }

        public async Task<EdsExerciseType?> GetSingleExerciseTypeAllData(long id)
        {
            var query = _dbcontext.EdsExerciseType.Where(extype => extype.Id == id)
                .Include(extype => extype.FkEquipment)
                .Include(extype => extype.FkForce)
                .Include(extype => extype.FkExerciseClass)
                .Include(extype => extype.FkLevel)
                .Include(extype => extype.FkMainMuscleWorked)
                .Include(extype => extype.FkOtherMuscleWorked)
                .Include(extype => extype.FkSport)
                .Include(extype => extype.FkMechanicsType);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<EdsExerciseType>> GetExerciseTypesAllData()
        {
            var query = _dbcontext.EdsExerciseType.Where(r => !r.IsDeleted)
                .Include(extype => extype.FkEquipment)
                .Include(extype => extype.FkForce)
                .Include(extype => extype.FkExerciseClass)
                .Include(extype => extype.FkLevel)
                .Include(extype => extype.FkMainMuscleWorked)
                .Include(extype => extype.FkOtherMuscleWorked)
                .Include(extype => extype.FkSport)
                .Include(extype => extype.FkMechanicsType)
                .Include(extype => extype.EdsSetDefaults)
                    .ThenInclude(setdefaults => setdefaults.EdsSetMetricsDefault)
                        .ThenInclude(setmdefaults => setmdefaults.FkSetMetricType);

            return await query.ToListAsync();
        }

        public async Task<List<EdsSetDefaults>> GetSetDefaultsForExerciseType(long extypeid)
        {
            var query = _dbcontext.EdsSetDefaults.Where(r => r.FkExerciseTypeId == extypeid)
                            .Include(r => r.EdsSetMetricsDefault);
            return await query.ToListAsync();                     
        }

        public async Task<EdsExerciseType?> AddExerciseType(ExerciseType_DTO exType)
        {
            using (_logger.BeginScope("AddExerciseType"))
            {
                try
                {
                    if (exType == null)
                    {
                        throw new ArgumentNullException(nameof(exType));
                    }

                    if (exType.Id != 0)
                    {
                        throw new InvalidOperationException("ExerciseType already exists.");
                    }

                    EdsExerciseType eds_extype = _mapper.Map<EdsExerciseType>(exType);

                    //add dependent lookups if it doesn't exist
                    if (exType.FkEquipmentId == 0)
                    {
                        var eds_equipment = new EdsEquipment() { Name = exType.EquipmentFreeText };
                        eds_extype.FkEquipment = eds_equipment;
                    }
                    if (exType.FkForceId == 0)
                    {
                        var eds_force = new EdsForce() { Name = exType.ForceFreeText };
                        eds_extype.FkForce = eds_force;
                    }
                    if (exType.FkExerciseClassId == 0)
                    {
                        var eds_exclass = new EdsExerciseClass() { Name = exType.ExerciseClassFreeText };
                        eds_extype.FkExerciseClass = eds_exclass;
                    }
                    if (exType.FkLevelId == 0)
                    {
                        var eds_level = new EdsLevel() { Name = exType.LevelFreeText };
                        eds_extype.FkLevel = eds_level;
                    }
                    if (exType.FkSportId == 0)
                    {
                        var eds_sport = new EdsSport() { Name = exType.SportFreeText };
                        eds_extype.FkSport = eds_sport;
                    }
                    if (exType.FkMechanicsTypeId == 0)
                    {
                        var eds_mechtype = new EdsMechanicsType() { Name = exType.MechanicsTypeFreeText };
                        eds_extype.FkMechanicsType = eds_mechtype;
                    }
                    if (exType.FkMainMuscleWorkedId == 0)
                    {
                        var eds_mainmuscle = new EdsMainMuscleWorked() { Name = exType.MainMuscleWorkedFreeText };
                        eds_extype.FkMainMuscleWorked = eds_mainmuscle;
                    }
                    if (exType.FkOtherMuscleWorkedId == 0)
                    {
                        var eds_othermuscle = new EdsOtherMuscleWorked() { Name = exType.OtherMuscleFreeText };
                        eds_extype.FkOtherMuscleWorked = eds_othermuscle;
                    }

                    eds_extype.HasSetDefaultTemplate = eds_extype.EdsSetDefaults.Count > 0;

                    _dbcontext.EdsExerciseType.Add(eds_extype);

                    await _dbcontext.SaveChangesAsync();

                    return eds_extype;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add Exercise Type");
                    return null;
                }
            }
        }

        public async Task<EdsExerciseType?> EditExerciseType(ExerciseType_DTO Data)
        {
            using (_logger.BeginScope("EditExerciseType"))
            {
                try
                {
                    EdsExerciseType itemToEdit = await _dbcontext.EdsExerciseType
                        .Include(r => r.EdsSetDefaults)
                            .ThenInclude(r => r.EdsSetMetricsDefault)
                        .FirstOrDefaultAsync(r => r.Id == Data.Id);
                    if (itemToEdit == null)
                    {
                        throw new InvalidOperationException($"Edit failed. Cannot find exercise type {Data.Id} ");
                    }

                    EdsExerciseType input = _mapper.Map<EdsExerciseType>(Data);

                    //add dependent lookups if it doesn't exist
                    if (Data.FkEquipmentId == 0)
                    {
                        var eds_equipment = new EdsEquipment() { Name = Data.EquipmentFreeText };
                        itemToEdit.FkEquipment = eds_equipment;
                    }
                    if (Data.FkForceId == 0)
                    {
                        var eds_force = new EdsForce() { Name = Data.ForceFreeText };
                        itemToEdit.FkForce = eds_force;
                    }
                    if (Data.FkExerciseClassId == 0)
                    {
                        var eds_exclass = new EdsExerciseClass() { Name = Data.ExerciseClassFreeText };
                        itemToEdit.FkExerciseClass = eds_exclass;
                    }
                    if (Data.FkLevelId == 0)
                    {
                        var eds_level = new EdsLevel() { Name = Data.LevelFreeText };
                        itemToEdit.FkLevel = eds_level;
                    }
                    if (Data.FkSportId == 0)
                    {
                        var eds_sport = new EdsSport() { Name = Data.SportFreeText };
                        itemToEdit.FkSport = eds_sport;
                    }
                    if (Data.FkMechanicsTypeId == 0)
                    {
                        var eds_mechtype = new EdsMechanicsType() { Name = Data.MechanicsTypeFreeText };
                        itemToEdit.FkMechanicsType = eds_mechtype;
                    }
                    if (Data.FkMainMuscleWorkedId == 0)
                    {
                        var eds_mainmuscle = new EdsMainMuscleWorked() { Name = Data.MainMuscleWorkedFreeText };
                        itemToEdit.FkMainMuscleWorked = eds_mainmuscle;
                    }
                    if (Data.FkOtherMuscleWorkedId == 0)
                    {
                        var eds_othermuscle = new EdsOtherMuscleWorked() { Name = Data.OtherMuscleFreeText };
                        itemToEdit.FkOtherMuscleWorked = eds_othermuscle;
                    }

                    foreach (var in_set in input.EdsSetDefaults)
                    {
                        if (in_set.Id == 0)
                        {
                            itemToEdit.EdsSetDefaults.Add(in_set);
                            continue;
                        }

                        var currSet = itemToEdit.EdsSetDefaults.FirstOrDefault(r => r.Id == in_set.Id);
                        if (currSet != null)
                        {
                            currSet.SetSequenceNumber = in_set.SetSequenceNumber;
                            foreach (var in_metric in in_set.EdsSetMetricsDefault)
                            {
                                if (in_metric.Id == 0)
                                {
                                    currSet.EdsSetMetricsDefault.Add(in_metric);
                                    continue;
                                }

                                var currMetric = itemToEdit.EdsSetDefaults.First(r => r.Id == in_set.Id).EdsSetMetricsDefault
                                    .FirstOrDefault(r => r.Id == in_metric.Id);
                                if (currMetric != null)
                                {
                                    currMetric.FkSetMetricType = in_metric.FkSetMetricType;
                                    currMetric.FkSetMetricTypeId = in_metric.FkSetMetricTypeId;
                                    currMetric.DefaultCustomMetric = in_metric.DefaultCustomMetric;
                                }
                            }
                            //delete all unwanted metrics
                            List<long> metricsToRemoveId = currSet.EdsSetMetricsDefault.Select(r => r.Id).Except(in_set.EdsSetMetricsDefault.Select(r => r.Id)).ToList();
                            foreach (var id in metricsToRemoveId)
                            {
                                _dbcontext.EdsSetMetricsDefault.Remove(currSet.EdsSetMetricsDefault.First(r => r.Id == id));
                            }
                        }
                    }
                    //delete all unwanted sets
                    List<long> setsToRemoveId = itemToEdit.EdsSetDefaults.Select(r => r.Id).Except(input.EdsSetDefaults.Select(r => r.Id)).ToList();
                    itemToEdit.HasSetDefaultTemplate = itemToEdit.EdsSetDefaults.Count - setsToRemoveId.Count != 0;
                    foreach (var id in setsToRemoveId)
                    {
                        var currset = itemToEdit.EdsSetDefaults.First(r => r.Id == id);
                        foreach (var setmetric in currset.EdsSetMetricsDefault)
                        {
                            _dbcontext.EdsSetMetricsDefault.Remove(setmetric);
                        }
                        _dbcontext.EdsSetDefaults.Remove(currset);
                    }
                    
                    _dbcontext.Entry<EdsExerciseType>(itemToEdit).State = EntityState.Modified;
                    await _dbcontext.SaveChangesAsync();

                    return itemToEdit;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Edit Exercise Type Failed.");
                    return null;
                }
            }
        }

        public async Task<bool> DeleteExerciseType(long exerTypeId)
        {
            using (_logger.BeginScope("DeleteExerciseType"))
            {
                try
                {
                    var itemToDelete = await _dbcontext.EdsExerciseType.FirstOrDefaultAsync(r => r.Id == exerTypeId);
                    if (itemToDelete == null)
                        throw new InvalidOperationException($"Cannot find Exercise Type {exerTypeId} to delete");

                    itemToDelete.IsDeleted = true;

                    await _dbcontext.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete Exercise Type");
                    return false;
                }
            }
        }

        public async Task<List<EdsEquipment>> GetEdsEquipmentAll()
        {
            return await _dbcontext.EdsEquipment.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task<List<EdsForce>> GetEdsForceAll()
        {
            return await _dbcontext.EdsForce.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task<List<EdsExerciseClass>> GetEdsExerciseClassAll()
        {
            return await _dbcontext.EdsExerciseClass.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task<List<EdsLevel>> GetEdsLevelAll()
        {
            return await _dbcontext.EdsLevel.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task<List<EdsMainMuscleWorked>> GetEdsMainMuscleWorkedAll()
        {
            return await _dbcontext.EdsMainMuscleWorked.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task<List<EdsOtherMuscleWorked>> GetEdsOtherMuscleWorkedAll()
        {
            return await _dbcontext.EdsOtherMuscleWorked.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task<List<EdsSport>> GetEdsSportAll()
        {
            return await _dbcontext.EdsSport.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task<List<EdsMechanicsType>> GetMechanicsTypeAll()
        {
            return await _dbcontext.EdsMechanicsType.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task<List<EdsSetDefaults>> GetAllSetDefaults()
        {
            return await _dbcontext.EdsSetDefaults
                .Include(r => r.EdsSetMetricsDefault).ToListAsync();
        }
    }
}
