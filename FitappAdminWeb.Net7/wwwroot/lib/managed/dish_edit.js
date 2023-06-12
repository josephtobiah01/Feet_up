window["dish_edit"] = (function () {
    var form_elem = document.getElementById("actualDIsh_form");
    var field_calorie = document.getElementById("Data_CalorieActual");
    var field_protein = document.getElementById("Data_ProteinActual");
    var field_carbs = document.getElementById("Data_CrabsActual");
    var field_sugar = document.getElementById("Data_SugarActual");
    var field_fat = document.getElementById("Data_FatActual");
    var field_unsatfat = document.getElementById("Data_UnsaturatedFatActual");
    var field_fiber = document.getElementById("Data_FiberGramsActual");
    var field_satfat = document.getElementById("Data_SaturatedFatGramsActual");
    var field_alcohol = document.getElementById("Data_AlcoholGramsActual");

    function _fillDishNutritionDataToFields(dish) {
        if (confirm("This will overwrite the currently entered nutritional information for the current dish being transcribed. Is this ok?")) {
            field_calorie.value = dish.calorieActual;
            field_protein.value = dish.proteinActual;
            field_carbs.value = dish.crabsActual;
            field_sugar.value = dish.sugarActual;
            field_fat.value = dish.fatActual;
            field_unsatfat.value = dish.unsaturatedFatActual;
            field_fiber.value = dish.fiberGramsActual;
            field_satfat.value = dish.saturatedFatGramsActual;
            field_alcohol.value = dish.alcoholGramsActual;
        }
    }

    function _init(options) {
        options.Callback = _fillDishNutritionDataToFields;
        window["dishbrowser_modal"].Initialize(options);
    }

    return {
        Initialize: _init
    };
})();