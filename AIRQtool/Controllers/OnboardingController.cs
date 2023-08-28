using ATLogger;
using QToo.Models;
using Qtool.Air;
using QTool.Main.DAL;
using QTool.Main.Models.Helpers;
using QTool.Main.Models.QTool;
using QTool.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using QTool.Main.Internal;

namespace AIRQtool.Controllers
{
    public class OnboardingController : Controller
    {
        private static readonly string QTOOL_MODELCACHE_KEY = "QTMODELCACHE";

        #region  Methods
        protected List<string> GetList(string line)
        {
            try
            {
                Regex x = new Regex(@"\[(.*?)\]");
                var matches = x.Matches(line);
                string[] s = new string[matches.Count];
                int i = 0;
                foreach (Match match in matches)
                {
                    var a = Regex.Replace(match.Value, @"[\[\]']+", "");
                    s[i++] = a;
                }
                List<string> list = new List<string>(s);
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        //    protected BaseProductRepository GetRepository(string productcode)
        //    {
        //        try
        //        {
        //            productcode = productcode.Trim().ToUpper();
        //            BaseProductRepository _repo;
        //            switch (productcode)
        //            {
        //                case "CLO":
        //         //           _repo = new CLORepository(1, productcode, Session);
        //                    break;
        //                case "CC":
        //            //        _repo = new CCRepository(2, productcode, Session);
        //                    break;
        //                case "DEP":
        //              //      _repo = new DEPRepository(3, productcode, Session);
        //                    break;
        //                case "MOT":
        //                //    _repo = new MOTRepository(4, productcode, Session);
        //                    break;
        //                case "TRV":
        //                 //   _repo = new TRVRepository(5, productcode, Session);
        //                    break;
        //                case "HLO":
        //                 //   _repo = new HLORepository(6, productcode, Session);
        //                    break;
        //                case "VLO":
        //                //    _repo = new VLORepository(7, productcode, Session);
        //                    break;
        //                case "HEA":
        //                //    _repo = new HEARepository(8, productcode, Session);
        //                    break;
        //                case "ABL":
        //                //    _repo = new ABLRepository(10, productcode, Session);
        //                    break;
        //                default:
        //                    throw new InvalidOperationException("No Vertical Exists for input");
        //            }
        //            return _repo;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //    }

        protected LifeSyleModel InstantiateEditorFormModel(string ModelName)
        {
            try
            {
                ModelName = ModelName.Trim().ToUpper();
                LifeSyleModel _fm;
                switch (ModelName.ToLower())
                {
                    case "user":
                        _fm = new LifeSyleModel();
                        break;
                    case "survey":
                        _fm = new LifeSyleModel();
                        break;
                    case "lifestyle":
                        _fm = new LifeSyleModel();
                        break;
                    default:
                        _fm = new LifeSyleModel();
                        break;
                }
                _fm.GenericData = new List<GenericQA>();
                return _fm;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region QTool Model Store Functions
        protected Form GetModelStore()
        {
            object objModel = (object)Session[QTOOL_MODELCACHE_KEY];
            if (objModel != null)
                return (Form)objModel;
            else
                return null;
        }

        private void SetModelStore(Form obj)
        {
            Session[QTOOL_MODELCACHE_KEY] = obj;
        }

        protected void ClearModelStore()
        {
            try
            {
                Session[QTOOL_MODELCACHE_KEY] = new Form(new List<FormQuestion>());
            }
            catch (Exception ex)
            {
            }
        }

        private Form AddModelStore(Form model)
        {
            try
            {
                //  if (model.FormQuestions == null) return;
                var storedModel = GetModelStore();
                foreach (var x in model.FormQuestions)
                {
                    try
                    {
                        if (x.Answer == null) continue;
                        var y = storedModel.FormQuestions.Where(t => t.questionID == x.questionID).FirstOrDefault();
                        if (y != null)
                        {
                            y.Answer = x.Answer;
                        }
                        else
                        {

                        }
                        if (y.QuestionAnswerChoice != null && y.QuestionAnswerChoice.Count > 0)
                        {

                            var z = new SelectList(y.QuestionAnswerChoice.ToArray(), "Value", "Text", y.Answer).ToList();
                            y.QuestionAnswerChoice = new List<SerializeableSelectListItem>();
                            z.ForEach(t => y.QuestionAnswerChoice.Add(t.Convert()));
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                }
                SetModelStore(storedModel);
                return storedModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Init Function (First access to qtool)
        //protected Form getmodel(string Vertical, string Channel, int Page, string lang, Base_PersonInfo_ViewModel prefillModel = null, AnswerDictionary prefillDict = null)
        protected Form GetModel(Form formModel, int Page, string lang, BaseModel prefillModel = null)
        {
            try
            {
                formModel.CurrentPage = Page;
                formModel.Culture = lang;

                formModel.PagesHeaders = Form.GetList(formModel.PageHeaderSetup);
                if (lang != null && formModel.PagesHeaders != null && formModel.PagesHeaders.Count > 0)
                {
                    int index = formModel.PagesHeaders != null ? formModel.PagesHeaders.FindIndex(a => a.StartsWith(lang)) : -1;
                    if (index >= 0 && formModel.PagesHeaders.ElementAt(index) == lang)
                    {
                        formModel.PagesHeaders = formModel.PagesHeaders.ElementAt(index + 1).Split(';').ToList();
                    }
                }
                else
                {
                    formModel.PagesHeaders = new List<string> { };
                }

                ModelFiller.FillDisplayQuestionList(formModel, lang, prefillModel, false);
                AnswerDictionary _dict = new AnswerDictionary();
                _dict.addorUpdateEntry("Culture", lang);

                if (prefillModel != null && formModel.FormQuestions != null)
                {
                    IndigoBridge.FillQToolModel(formModel.FormQuestions, prefillModel, lang, false);
                    //   var UnsortedVals = IndigoBridge.GetUnsortedValues(prefillModel, formModel.Vertical);
                    for (int i = 0; i < formModel.FormQuestions.Count; i++)
                    {
                        // try
                        //{
                        FormQuestion _currDisplayQuestion = formModel.FormQuestions[i];
                        if (string.IsNullOrEmpty(_currDisplayQuestion.Answer) && !(_currDisplayQuestion.questionName == "Info" || _currDisplayQuestion.questionName == "Image" || _currDisplayQuestion.questionName == "HTML"))
                        {
                            _currDisplayQuestion.Answer = _currDisplayQuestion.DefaultValue;
                        }

                        bool excludeQuestion = false;
                        string cond = formModel.FormQuestions[i].conditions;

                        if (!string.IsNullOrEmpty(cond))
                        {
                            // Fill the Qtool_Answer Dictionary from editomodel-import variables (i.e questions that are *not* presented to the user)
                            // Questions that fail these conditions are permamently removed from the Question list, as the user is not able to change the result of the evaluation
                            IndigoBridge.FillPrefillConditionModel(_dict, cond, prefillModel);
                            cond = Expression.Substitute(cond, _dict.Dictionary);
                            excludeQuestion = !Expression.Evaluate(cond);
                        }

                        if (excludeQuestion)
                        {
                            if (formModel.FormQuestions[i].questionName != "HTML")
                            {
                                formModel.FormQuestions.RemoveAt(i);
                                i--;
                            }
                            else
                            {
                                formModel.FormQuestions[i].Visisble = false;
                            }
                        }
                        //  }
                        //  catch { }
                    }
                }
                SetModelStore(formModel);
                return formModel;
            }
            catch (Exception ex)
            {
                //     LogUtility.TraceMessage(ErrorLevel.Critical, ex);
                throw;
            }
        }
        #endregion

        #region New Action Methods

        //LINK
        [HttpGet]
        public ActionResult Start(string formId)
        {
            if (!IndigoBridge.IsLinKValid(formId))
            {
                return RedirectToAction("Error", "Error");
            }
            string FormName = "TEST";
            int PAGE = 1;
            bool newForm = false;
            Form formModel = null;

            ClearModelStore();

            try
            {
                formModel = FormHandler.LoadEditnContineForm(HttpUtility.UrlEncode(formId));
                if (formModel != null)
                {
                    if (formModel.CurrentPage < 1)
                    {
                        formModel.CurrentPage = 1;
                    }
                    SetModelStore(formModel);
                    return RedirectToAction("Render", "Onboarding", new { Page = formModel.CurrentPage, formModel.Culture, SkipDebug = true });
                }
            }
            catch 
            {
              //  return RedirectToAction("Error", "Error");
            }

            if (formModel == null)
            {
                formModel = new Form(FormHandler.GetForm(FormName), -1)
                {
                    FormID = HttpUtility.UrlEncode(formId)
                };
            }

            var editormodel = IndigoBridge.Fill_LifeSyleModel_ModelFromDataBase(formId);
            formModel = GetModel(formModel, PAGE, "en-us", editormodel);
            formModel.CurrentPage = PAGE;
            AddModelStore(formModel);

            if (Statics.showDebug)
            {
                Session["BMODEL"] = editormodel;
                return RedirectToAction("Preview", "Onboarding", new { FormName, formModel.Vertical, formModel.Culture, formModel.FormID, newForm, Page = 1 });
            }
            else
            {
                ViewBag.CalcerURL = Statics.CalcerURL;
                ViewBag.AZURE_STORAGE_URL = Statics.AZURE_STORAGE_URL;
                return RedirectToAction("Render", "Onboarding", new { FormName, formModel.Vertical, formModel.Culture, formModel.FormID, newForm, Page = 1 });
            }
        }

        [HttpGet]
        public ActionResult Render(string FormName, string Culture, string QToolReturnURL, bool SkipDebug = false, bool NewForm = false, bool ReloadQToolRepo = false, int Page = 0)
        {
            try
            {
                Session["QToolReturnURL"] = QToolReturnURL;
                BaseModel editormodel = new BaseModel();

                Form formModel;
                if (NewForm || ReloadQToolRepo)
                {
                    ClearModelStore();
                    formModel = new Form(FormHandler.GetForm(FormName), -1)
                    {
                        FormID = Guid.NewGuid().ToString()
                    };
                    if (!Statics.IsInternal)
                    {
                        editormodel = InstantiateEditorFormModel(formModel.Vertical);
                    }
                    else
                    {
                        Session["LiveScriptObject"] = formModel.SessionData;
                    }
                    formModel = GetModel(formModel, Page, Culture, editormodel);
                }
                else
                {
                    formModel = GetModelStore();
                }

                formModel.CurrentPage = Page;
                formModel = new Form(formModel, formModel.CurrentPage);
                // ModelFiller.FillLiveScript(formModel);

                if (Statics.showDebug && !SkipDebug && Page <= 1)
                {
                    return RedirectToAction("Preview", "Onboarding", new { FormName, formModel.Vertical, formModel.Culture, formModel.FormID });
                }
                else
                {
                    ViewBag.CalcerURL = Statics.CalcerURL;
                    ViewBag.AZURE_STORAGE_URL = Statics.AZURE_STORAGE_URL;
                    return View(formModel.z_HtmlFile, formModel);
                }
            }
            catch
            {
                string formId = Request["FormID"];
                return RedirectToAction("Start", new { formId });
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Render(string submitAction, Form model)
        {
            ViewBag.CalcerURL = Statics.CalcerURL;
            ViewBag.AZURE_STORAGE_URL = Statics.AZURE_STORAGE_URL;
            try
            {
                if (submitAction == "Back")
                {
                    if (model.CurrentPage <= 1)
                    {
                        if (Session["QToolReturnURL"] != null)
                        {
                            return Redirect(Session["QToolReturnURL"].ToString());
                        }
                        else
                        {
                            return Redirect("/");
                        }
                    }
                    else
                    {
                        model.CurrentPage -= 1;
                        return RedirectToAction("Render", "Onboarding", new { Page = model.CurrentPage, FormName = model.Name, model.Culture, SkipDebug = true, FormID = model.FormID });
                    }
                }


                //   var model2 = GetModelStore();

                var q1 = model.FormQuestions.Where(t => t.questionName == "Customer_password").FirstOrDefault();
                var q2 = model.FormQuestions.Where(t => t.questionName == "Customer_password_repeat").FirstOrDefault();

                if (q1 != null && q2 != null)
                {
                    if (q1.Answer != q2.Answer)
                    {
                        ModelState.AddModelError(String.Empty,
                        "The passwords are not he same.");
                        ViewBag.ShowPWerror = true;
                    }
                }

                if (!ModelState.IsValid)
                {
                    var model2 = GetModelStore();
                    var realModel = new Form(model2, model.CurrentPage);
                    if (Session["LiveScriptObject"] != null)
                    {
                        realModel.SessionData = Session["LiveScriptObject"].ToString();
                        ModelFiller.FillLiveScript(realModel);
                    }
                    ViewBag.CalcerURL = Statics.CalcerURL;
                    ViewBag.AZURE_STORAGE_URL = Statics.AZURE_STORAGE_URL;
                    return View(model.z_HtmlFile, realModel);
                }
                else
                {
                    //   FormHandler.PersistEditnContinueForm(model);
                }

                if (model.CurrentPage >= model.MaxPage)
                {
                    string errorList = string.Empty;
                    if (Session["ERRORLIST"] != null)
                    {
                        errorList = Session["ERRORLIST"].ToString();
                    }

                    AddModelStore(model);
                    model = GetModelStore();
                    var IndigoModel2 = InstantiateEditorFormModel(model.Vertical);

                    errorList += IndigoBridge.FillIndigoModel2(model.FormQuestions, IndigoModel2);
                    Session["ERRORLIST"] = errorList;
                    if (!Statics.IsInternal)
                    {
                        //ManualMapper.CopyProperties(Session["PrefilledCustomerCrossSellingInfo"], IndigoModel2.CustomerCrossSellingInfo, MapOptions.OverrideOnlyIfDestinationIsEmpty);
                        // Saving Final Move After Confirmation Page
                        bool saveSuccess = IndigoBridge.Save_LifeSyleModel_ModelFromDataBase(IndigoModel2, model.FormID);
                    }
                    if (Statics.showDebug)
                    {
                        Session["M2"] = IndigoModel2;
                    }

                    if (!Statics.showDebug)
                    {
                        return new RedirectResult("https://calendly.com/welcome_air/1-on-1-call-onboarding");
                       // return RedirectToAction("ThankYou", model.Vertical, new { area = "Product" });
                    }
                    else
                    {
                        Session["M3"] = errorList;
                        return RedirectToAction("Results", "Onboarding", new { Page = model.CurrentPage, FormName = model.Name, model.Culture });
                    }
                }
                AddModelStore(model);


                if (submitAction == "Next")
                {
                    model.CurrentPage += 1;
                    var model3 = GetModelStore();
                    model3.CurrentPage += 1;
                    FormHandler.PersistEditnContinueForm(model3);
                }

                return RedirectToAction("Render", "Onboarding", new { Page = model.CurrentPage, FormName = model.Name, model.Culture, FormID = model.FormID });
            }
            catch (Exception ex)
            {
                //  LogUtility.TraceMessage(ErrorLevel.Critical, ex);
                throw;
            }
        }
        #endregion


        // #region Qtool Internal Tests Methods
        [HttpGet]
        public ActionResult Preview()
        {
            bool newForm = true;
            try
            {
                newForm = bool.Parse(Request["NewForm"]);
            }
            catch { }
            string m_culture = Request["Culture"];
            string m_formname = Request["FormName"];
            Guid.TryParse(Request["FormID"], out Guid FormID);
            Form formModel = GetModelStore();
            var editormodel = (BaseModel)Session["BMODEL"];
            var dump = ObjectDumper.Dump(editormodel);
            StatsModel model = new StatsModel() { NewForm = newForm, FormName = m_formname, Culture = m_culture, FormId = FormID, ObjectDump = dump };
            return View("Preview", model);
        }


        [HttpPost]
        public ActionResult Preview(string FormName, string Culture, bool newForm)
        {
            return RedirectToAction("Render", "Onboarding", new { Page = 1, FormName = FormName, Culture, SkipDebug = true, NewForm = newForm });
        }

        [HttpGet]
        public ActionResult Results(string FormName, string Culture)
        {

            var dump = ObjectDumper.Dump(Session["M2"]);
            StatsModel model = new StatsModel() { FormName = FormName, Culture = Culture, ObjectDump = dump };
            return View(model);
        }

        [HttpPost]
        public ActionResult Results(string FormName)
        {
            return RedirectToAction("ThankYou", "Onboarding");
        }

        public ActionResult ThankYou()
        {
            return View();
        }


        //#region VIEW


        //[HttpGet]
        //public ActionResult View(long UserId)
        //{
        //    string FormName = "TEST";
        //    int PAGE = 1;
        //    bool newForm = false;
        //    Form formModel = null;

        //    ClearModelStore();


        //    if (formModel == null)
        //    {
        //        formModel = new Form(FormHandler.GetForm(FormName), -1)
        //        {
        //            FormID = UserId.ToString()
        //        };
        //    }

        //    var editormodel = IndigoBridge.Fill_LifeSyleModel_ModelFromDataBase_From_User(UserId);
        //    formModel = GetModel(formModel, PAGE, "en-us", editormodel);
        //    formModel.CurrentPage = PAGE;
        //    AddModelStore(formModel);

        //    ViewBag.CalcerURL = Statics.CalcerURL;
        //    ViewBag.AZURE_STORAGE_URL = Statics.AZURE_STORAGE_URL;
        //    return RedirectToAction("RenderVIEW", "Onboarding", new { FormName, formModel.Vertical, formModel.Culture, formModel.FormID, newForm, Page = 1 });

        //}


        //[HttpGet]
        //public ActionResult RenderVIEW(string FormName, string Culture, string QToolReturnURL, bool SkipDebug = false, bool NewForm = false, bool ReloadQToolRepo = false, int Page = 0)
        //{
        //    Session["QToolReturnURL"] = QToolReturnURL;
        //    BaseModel editormodel = new BaseModel();

        //    Form formModel;
        //    if (NewForm || ReloadQToolRepo)
        //    {
        //        ClearModelStore();
        //        formModel = new Form(FormHandler.GetForm(FormName), -1)
        //        {
        //            FormID = Guid.NewGuid().ToString()
        //        };
        //        if (!Statics.IsInternal)
        //        {
        //            editormodel = InstantiateEditorFormModel(formModel.Vertical);
        //        }
        //        else
        //        {
        //            Session["LiveScriptObject"] = formModel.SessionData;
        //        }
        //        formModel = GetModel(formModel, Page, Culture, editormodel);
        //    }
        //    else
        //    {
        //        formModel = GetModelStore();
        //    }

        //    formModel.CurrentPage = Page;
        //    formModel = new Form(formModel, formModel.CurrentPage);
        //    // ModelFiller.FillLiveScript(formModel);

        //    ViewBag.CalcerURL = Statics.CalcerURL;
        //    ViewBag.AZURE_STORAGE_URL = Statics.AZURE_STORAGE_URL;
        //    return View(formModel.z_HtmlFile, formModel);
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult RenderVIEW(string submitAction, Form model)
        //{
        //    ViewBag.CalcerURL = Statics.CalcerURL;
        //    ViewBag.AZURE_STORAGE_URL = Statics.AZURE_STORAGE_URL;
        //    try
        //    {
        //        if (submitAction == "Back")
        //        {
        //            if (model.CurrentPage <= 1)
        //            {
        //                if (Session["QToolReturnURL"] != null)
        //                {
        //                    return Redirect(Session["QToolReturnURL"].ToString());
        //                }
        //                else
        //                {
        //                    return Redirect("/");
        //                }
        //            }
        //            else
        //            {
        //                model.CurrentPage -= 1;
        //                return RedirectToAction("Render", "Onboarding", new { Page = model.CurrentPage, FormName = model.Name, model.Culture, SkipDebug = true, FormID = model.FormID });
        //            }
        //        }


        //        //   var model2 = GetModelStore();

        //        var q1 = model.FormQuestions.Where(t => t.questionName == "Customer_password").FirstOrDefault();
        //        var q2 = model.FormQuestions.Where(t => t.questionName == "Customer_password_repeat").FirstOrDefault();

        //        if (q1 != null && q2 != null)
        //        {
        //            if (q1.Answer != q2.Answer)
        //            {
        //                ModelState.AddModelError(String.Empty,
        //                "The passwords are not he same.");
        //                ViewBag.ShowPWerror = true;
        //            }
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            var model2 = GetModelStore();
        //            var realModel = new Form(model2, model.CurrentPage);
        //            if (Session["LiveScriptObject"] != null)
        //            {
        //                realModel.SessionData = Session["LiveScriptObject"].ToString();
        //                ModelFiller.FillLiveScript(realModel);
        //            }
        //            ViewBag.CalcerURL = Statics.CalcerURL;
        //            ViewBag.AZURE_STORAGE_URL = Statics.AZURE_STORAGE_URL;
        //            return View(model.z_HtmlFile, realModel);
        //        }
        //        else
        //        {
        //            //   FormHandler.PersistEditnContinueForm(model);
        //        }

        //        if (model.CurrentPage >= model.MaxPage)
        //        {
        //            string errorList = string.Empty;
        //            if (Session["ERRORLIST"] != null)
        //            {
        //                errorList = Session["ERRORLIST"].ToString();
        //            }

        //            AddModelStore(model);
        //            model = GetModelStore();
        //            var IndigoModel2 = InstantiateEditorFormModel(model.Vertical);

        //            errorList += IndigoBridge.FillIndigoModel2(model.FormQuestions, IndigoModel2);
        //            Session["ERRORLIST"] = errorList;
        //            if (!Statics.IsInternal)
        //            {
        //                //ManualMapper.CopyProperties(Session["PrefilledCustomerCrossSellingInfo"], IndigoModel2.CustomerCrossSellingInfo, MapOptions.OverrideOnlyIfDestinationIsEmpty);
        //                // Saving Final Move After Confirmation Page
        //             //   bool saveSuccess = IndigoBridge.Save_LifeSyleModel_ModelFromDataBase(IndigoModel2, model.FormID);
        //            }
        //            if (Statics.showDebug)
        //            {
        //                Session["M2"] = IndigoModel2;
        //            }

        //            if (!Statics.showDebug)
        //            {
        //                return RedirectToAction("ThankYou", model.Vertical, new { area = "Product" });
        //            }
        //            else
        //            {
        //                Session["M3"] = errorList;
        //                return RedirectToAction("Results", "Onboarding", new { Page = model.CurrentPage, FormName = model.Name, model.Culture });
        //            }
        //        }
        //        AddModelStore(model);


        //        if (submitAction == "Next")
        //        {
        //            model.CurrentPage += 1;
        //            var model3 = GetModelStore();
        //            model3.CurrentPage += 1;
        //          //  FormHandler.PersistEditnContinueForm(model3);
        //        }

        //        return RedirectToAction("Render", "Onboarding", new { Page = model.CurrentPage, FormName = model.Name, model.Culture, FormID = model.FormID });
        //    }
        //    catch (Exception ex)
        //    {
        //        //  LogUtility.TraceMessage(ErrorLevel.Critical, ex);
        //        throw;
        //    }
        //}
        //#endregion
    }
}