using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using oEEntity;
using oEEntity.Base;
using oEEntity.Model;
using System.IO;

namespace oELib
{
    public class MasterDataFunctions
    {
        private oExEntity oeEntity = null;

        public void saveXML(oExEntity oeEntity)
        {
            oeEntity.WriteXml(@"D:\ranganathanns\Shared\New Folder\oE\oEXML.xml");
        }

        public void LoadXML()
        {
            oeEntity = new oExEntity();
            if(File.Exists(@"D:\ranganathanns\Shared\New Folder\oE\oEXML.xml"))
                oeEntity.ReadXml(@"D:\ranganathanns\Shared\New Folder\oE\oEXML.xml");
        }

        #region GROUP
        public void SaveQuestionType(SaveQuestionType QuestionType)
        {
            DataTable dtQuestionType = null;
            DataRow drNew = null;

            try
            {

                LoadXML();
                dtQuestionType = oeEntity.Tables[EntityConstents.TBL_QUESTIONTYPE];

                if (QuestionType.EntityState == EntityOperationalState.New)
                {
                    drNew = dtQuestionType.NewRow();
                    drNew[EntityConstents.COL_QUESTIONTYPE_ID] = Guid.NewGuid().ToString();
                    dtQuestionType.Rows.Add(drNew);
                }
                else
                    drNew = dtQuestionType.AsEnumerable().Where(ans => ans.Field<string>("ID") == QuestionType.ID).SingleOrDefault();


                drNew[EntityConstents.COL_QUESTIONTYPE_CODE] = QuestionType.Code;
                drNew[EntityConstents.COL_QUESTIONTYPE_DESC] = QuestionType.Description;
                drNew[EntityConstents.COL_QUESTIONTYPE_TYPE] = GetQTypeValue(QuestionType.Type);
                drNew[EntityConstents.COL_QUESTIONTYPE_FK_TYPE] = QuestionType.FkQuestionTypeID;

                saveXML(oeEntity);
            }
            catch
            {
                throw;
            }
        }
        public string GetQTypeValue(QuestionTypes QuestionTyp)
        {
            int qTypeValue = (int)QuestionTyp;
            return qTypeValue.ToString();
        }
        public QuestionTypes GetQTypeEnum(string QuestionTypValue)
        {
            QuestionTypes qTypeValue = QuestionTypes.None;

            switch (QuestionTypValue)
            {
                case "0":
                    qTypeValue = QuestionTypes.None;
                    break;
                case "1":
                    qTypeValue = QuestionTypes.Mode;
                    break;
                case "2":
                    qTypeValue = QuestionTypes.Group;
                    break;
                case "3":
                    qTypeValue = QuestionTypes.Topic;
                    break;
            }

            return qTypeValue;
        }
        private QuestionType GetQuestionTypeInfo(string parentKey, DataTable dtQuestionTYpe)
        {
            QuestionType questionTYpe = null;
            string fkQuestionTypeID = string.Empty;
            DataRow drnew = null;   

            try
            {
                drnew = dtQuestionTYpe.AsEnumerable().Where(queTyp => queTyp.Field<string>("ID") == parentKey).FirstOrDefault();

                if (drnew != null)
                {
                    questionTYpe = new QuestionType();

                    questionTYpe.ID = drnew["ID"].ToString();
                    questionTYpe.Code = drnew["Code"].ToString();
                    questionTYpe.Description = drnew["Description"].ToString();
                    questionTYpe.Type = GetQTypeEnum(drnew["Type"].ToString());
                    parentKey = drnew["Fk_QuestionType"].ToString();

                    questionTYpe.FkQuestionType = GetQuestionTypeInfo(parentKey, dtQuestionTYpe);
                }
            }
            catch
            {
                throw;
            }

            return questionTYpe;
        }
        public List<QuestionType> LoadQuestionType(QuestionTypes questionTypes)
        {
            DataTable dtQuestionTYpe = null;
            DataTable dtQuestions = null;
            DataTable dtAnswer = null;

            List<QuestionType> questionTYpeColl = null;
            List<DataRow> questionTypedrColl = null;

            try
            {
                LoadXML();
                dtQuestionTYpe = oeEntity.Tables[EntityConstents.TBL_QUESTIONTYPE];
                dtQuestions = oeEntity.Tables[EntityConstents.TBL_QUESTIONS];
                dtAnswer = oeEntity.Tables[EntityConstents.TBL_ANSWERS];

                questionTYpeColl = new List<QuestionType>();

                questionTypedrColl = (from queTyp1 in dtQuestionTYpe.AsEnumerable()
                                      where queTyp1.Field<string>("Type") == GetQTypeValue(questionTypes)
                                      select queTyp1).ToList();
                questionTYpeColl = LoadQuestionType(questionTypedrColl);

            }
            catch
            {
                throw;
            }

            return questionTYpeColl;
        }
        public List<QuestionType> LoadQuestionTypeByTopicID(string TopicID)
        {
            DataTable dtQuestionTYpe = null;
            DataTable dtQuestions = null;
            DataTable dtAnswer = null;

            List<QuestionType> questionTYpeColl = null;
            List<DataRow> questionTypedrColl = null;

            try
            {
                LoadXML();
                dtQuestionTYpe = oeEntity.Tables[EntityConstents.TBL_QUESTIONTYPE];
                dtQuestions = oeEntity.Tables[EntityConstents.TBL_QUESTIONS];
                dtAnswer = oeEntity.Tables[EntityConstents.TBL_ANSWERS];

                questionTYpeColl = new List<QuestionType>();

                questionTypedrColl = dtQuestionTYpe.AsEnumerable().Where(qr => qr.Field<string>("ID") == TopicID).ToList(); 
                questionTYpeColl = LoadQuestionType(questionTypedrColl);

            }
            catch
            {
                throw;
            }

            return questionTYpeColl;
        }

        public List<QuestionType> LoadQuestionTypeByQID(string QuestionID)
        {
            DataTable dtQuestionTYpe = null;
            DataTable dtQuestionR = null;
            DataTable dtQuestions = null;
            DataTable dtAnswer = null;

            List<string> questionRTypeIDs = null;
            List<QuestionType> questionTYpeColl = null;

            try
            {
                LoadXML();
                dtQuestionR = oeEntity.Tables[EntityConstents.TBL_QUESTIONS_REL];
                dtQuestionTYpe = oeEntity.Tables[EntityConstents.TBL_QUESTIONTYPE];
                dtQuestions = oeEntity.Tables[EntityConstents.TBL_QUESTIONS];
                dtAnswer = oeEntity.Tables[EntityConstents.TBL_ANSWERS];

                questionTYpeColl = new List<QuestionType>();

                questionRTypeIDs = dtQuestionR.AsEnumerable().Where(qr => qr.Field<string>("Fk_Que_ID") == QuestionID).Select(qr1 => qr1.Field<string>("Fk_Type_ID")).ToList();

                questionRTypeIDs.ForEach(qrid => questionTYpeColl.AddRange(LoadQuestionTypeByTopicID(qrid)));
            }
            catch
            {
                throw;
            }

            return questionTYpeColl;
        }
        private List<QuestionType> LoadQuestionType(List<DataRow> questionTypes)
        {
            DataTable dtQuestionTYpe = null;
            DataTable dtQuestions = null;
            DataTable dtAnswer = null;

            List<QuestionType> questionTYpeColl = null;
            List<DataRow> questionTypedrColl = null;

            try
            {
                LoadXML();
                dtQuestionTYpe = oeEntity.Tables[EntityConstents.TBL_QUESTIONTYPE];
                dtQuestions = oeEntity.Tables[EntityConstents.TBL_QUESTIONS];
                dtAnswer = oeEntity.Tables[EntityConstents.TBL_ANSWERS];

                questionTYpeColl = new List<QuestionType>();


                questionTYpeColl = (from queTyp in questionTypes
                                    select new QuestionType
                                    {
                                        ID = queTyp.Field<string>("ID"),
                                        Code = queTyp.Field<string>("Code"),
                                        Description = queTyp.Field<string>("Description"),
                                        Type = GetQTypeEnum(queTyp.Field<string>("Type")),
                                        FkQuestionType = GetQuestionTypeInfo(queTyp.Field<string>("Fk_QuestionType"), dtQuestionTYpe)

                                    }).ToList();

            }
            catch
            {
                throw;
            }

            return questionTYpeColl;
        }
        #endregion

        #region QUESTION NEW
        public string SaveQuestion(oEEntiti SaveQuestion)
        {
            DataTable dtQuestion = null;
            DataRow drNew = null;
            SaveQuestion saveQuestion = null;
            string questionID = string.Empty;

            try
            {
                saveQuestion = SaveQuestion as SaveQuestion;
                LoadXML();
                dtQuestion = oeEntity.Tables[EntityConstents.TBL_QUESTIONS];

                if (saveQuestion.EntityState == EntityOperationalState.New)
                {
                    drNew = dtQuestion.NewRow();
                    questionID = Guid.NewGuid().ToString();
                    drNew[EntityConstents.COL_QUESTIONS_ID] = questionID;
                    dtQuestion.Rows.Add(drNew);
                }
                else
                {
                    questionID = saveQuestion.ID;
                    drNew = dtQuestion.AsEnumerable().Where(ans => ans.Field<string>("ID") == saveQuestion.ID).SingleOrDefault();
                }

                if (drNew != null)
                {
                    drNew[EntityConstents.COL_QUESTIONS_QUESTION] = saveQuestion.Questionn;
                    drNew[EntityConstents.COL_QUESTIONS_LEVEL] = saveQuestion.ComplexLevel;
                    drNew[EntityConstents.COL_QUESTIONS_POINT] = saveQuestion.Point;

                    saveXML(oeEntity);
                }
            }
            catch
            {
                throw;
            }
            return questionID;
        }
        public void SaveQuestionTypeRelation(SaveQuestionType QuestionType)
        {
            DataRow drNew = null;
            DataRow[] drNewColl = null;
            DataTable dtQuestionRelation = null;

            try
            {
                LoadXML();
                dtQuestionRelation = oeEntity.Tables[EntityConstents.TBL_QUESTIONS_REL];

                if (QuestionType.EntityState == EntityOperationalState.Update)
                {
                    drNewColl = dtQuestionRelation.AsEnumerable().Where(ans => ans.Field<string>("ID") == QuestionType.ID).ToArray();
                    for (int cnt = drNewColl.Count() - 1; cnt >= 0; cnt--)
                        dtQuestionRelation.Rows.Remove(drNewColl[cnt]);
                }

                foreach (SaveQuestionTypes sqType in QuestionType.QuestionTypes)
                {
                    drNew = dtQuestionRelation.NewRow();
                    drNew[EntityConstents.COL_QUESTIONS_REL_ID] = Guid.NewGuid().ToString();
                    drNew[EntityConstents.COL_QUESTIONS_REL_QUSID] = QuestionType.QuestionID;
                    drNew[EntityConstents.COL_QUESTIONS_REL_TYPEID] = sqType.ID;

                    dtQuestionRelation.Rows.Add(drNew);
                }
                saveXML(oeEntity);
            }
            catch
            {
                throw;
            }

        }
        public List<QuestionRelation> LoadQuestionsRelations(string TopicID)
        {
            DataTable dtQuestionsR = null;
            List<DataRow> drQuestionR = null;
            QuestionRelation questionR = null;
            List<QuestionRelation> questionRColl = null;
            List<Question> questionColl = null;
            string groupID = string.Empty;
            string topicID = string.Empty;

            try
            {
                LoadXML();
                questionRColl = new List<QuestionRelation>();

                dtQuestionsR = oeEntity.Tables[EntityConstents.TBL_QUESTIONS_REL];

                if (dtQuestionsR != null)
                {
                    drQuestionR = dtQuestionsR.AsEnumerable().Where(qr => qr.Field<string>("Fk_Type_ID") == TopicID).ToList();

                    List<string> questionIDs = drQuestionR.AsEnumerable().Select(qr => qr.Field<string>("Fk_Que_ID")).ToList();
                    questionColl = LoadQuestion(questionIDs);

                    foreach (DataRow dr in drQuestionR)
                    {
                        //groupID = dr[EntityConstents.COL_QUESTIONS_REL_GROUPTYPE].ToString();
                        topicID = dr[EntityConstents.COL_QUESTIONS_REL_TYPEID].ToString();

                        questionR = new QuestionRelation();
                        questionR.ID = dr[EntityConstents.COL_QUESTIONS_REL_ID].ToString();
                        questionR.Questionn = questionColl.Where(que => que.ID == dr[EntityConstents.COL_QUESTIONS_REL_QUSID].ToString()).SingleOrDefault();
                        // questionR.GroupType = GroupTypeColl.Where(gc => gc.ID == groupID).SingleOrDefault();
                        questionR.QuestionTyp = LoadQuestionTypeByTopicID(TopicID);
                        questionRColl.Add(questionR);
                    }
                }
            }
            catch
            {
                throw;
            }
            return questionRColl;
        }
        #endregion

        #region QUESTION
        public List<Question> LoadQuestion(List<string> questionIDs)
        {
            DataTable dtQuestions = null;

            List<Question> questionColl = null;
            Question question = null;
            string quesModeID = string.Empty;
            string topicTypeID = string.Empty;
            List<DataRow> drs = null;

            try
            {
                LoadXML();
                dtQuestions = oeEntity.Tables[EntityConstents.TBL_QUESTIONS];

                drs = dtQuestions.AsEnumerable().Where(que => questionIDs.Contains(que.Field<string>("ID"))).ToList();
                questionColl = new List<Question>();
                foreach (DataRow dr in drs)
                {
                    question = new Question();

                    question.ID = dr[EntityConstents.COL_QUESTIONS_ID].ToString();
                    question.Questionn = dr[EntityConstents.COL_QUESTIONS_QUESTION].ToString();
                    question.ComplexLevel = dr[EntityConstents.COL_QUESTIONS_LEVEL].ToString();
                    question.Point = dr[EntityConstents.COL_QUESTIONS_POINT].ToString();
                    //topicTypeID = dr[EntityConstents.COL_QUESTIONS_TOPICTYPE].ToString();
                    //question.TopicType = topicTypeColl.Where(tt => tt.ID == topicTypeID).FirstOrDefault();
                    //question.QuestionMode = quesModeColl.Where(qm => qm.ID == quesModeID).FirstOrDefault();

                    questionColl.Add(question);
                }
            }
            catch
            {
                throw;
            }

            return questionColl;
        }
        #endregion

        #region ANSWER
        public void AddAnswers(oEEntiti AnswerEntity, string QuestionID)
        {
            DataTable dtAnswer = null;
            DataRow drNew = null;
            Answer Answer = null;

            try
            {
                Answer = AnswerEntity as Answer;

                LoadXML();
                dtAnswer = oeEntity.Tables[EntityConstents.TBL_ANSWERS];
                if (Answer.EntityState == EntityOperationalState.New)
                {
                    drNew = dtAnswer.NewRow();
                    drNew[EntityConstents.COL_ANSWERS_ID] = Guid.NewGuid().ToString();
                    dtAnswer.Rows.Add(drNew);
                }
                else
                    drNew = dtAnswer.AsEnumerable().Where(ans => ans.Field<string>("ID") == Answer.ID).SingleOrDefault();
                
                drNew[EntityConstents.COL_ANSWERS_ANSWER] = Answer.Answerr;
                drNew[EntityConstents.COL_ANSWERS_QUESTION] = QuestionID;
                drNew[EntityConstents.COL_ANSWERS_VALUE] = Answer.Value;
 
                saveXML(oeEntity);
            }
            catch
            {
                throw;
            }
        }

        public void SaveAnswers(oEEntiti SaveAnswers)
        {
            DataTable dtAnswer = null;
            DataRow drNew = null;
            SaveAnswers Answer = null;

            try
            {
                Answer = SaveAnswers as SaveAnswers;

                LoadXML();
                dtAnswer = oeEntity.Tables[EntityConstents.TBL_ANSWERS];
                if (Answer.EntityState == EntityOperationalState.New)
                {
                    drNew = dtAnswer.NewRow();
                    drNew[EntityConstents.COL_ANSWERS_ID] = Guid.NewGuid().ToString();
                    dtAnswer.Rows.Add(drNew);
                }
                else
                    drNew = dtAnswer.AsEnumerable().Where(ans => ans.Field<string>("ID") == Answer.ID).SingleOrDefault();

                drNew[EntityConstents.COL_ANSWERS_ANSWER] = Answer.Answerr;
                drNew[EntityConstents.COL_ANSWERS_QUESTION] = Answer.QuestionID;
                drNew[EntityConstents.COL_ANSWERS_ORDER] = Answer.AnswerOrder;
                drNew[EntityConstents.COL_ANSWERS_VALUE] = Answer.Value;

                saveXML(oeEntity);
            }
            catch
            {
                throw;
            }
        }

        public List<Answer> LoadAnswers()
        {
            DataTable dtAnswer = null;
            List<Answer> answerColl = null;
            Answer answer = null;

            try
            {
                LoadXML();
                dtAnswer = oeEntity.Tables[EntityConstents.TBL_ANSWERS];
                answerColl = new List<Answer>();
                foreach (DataRow dr in dtAnswer.Rows)
                {
                    answer = new Answer();

                    answer.ID = dr[EntityConstents.COL_ANSWERS_ID].ToString();
                    answer.Answerr = dr[EntityConstents.COL_ANSWERS_ANSWER].ToString();
                    answer.Value = dr[EntityConstents.COL_ANSWERS_VALUE].ToString();

                    answerColl.Add(answer);
                }
            }
            catch
            {
            }

            return answerColl;
        }

        public List<Answer> LoadAnswersByQuestion(List<string> questionIDs)
        {
            DataTable dtAnswer = null;
            List<Answer> answerColl = null;
            List<DataRow> drColl = null;
            Answer answer = null;

            try
            {
                LoadXML();
                dtAnswer = oeEntity.Tables[EntityConstents.TBL_ANSWERS];
                drColl = dtAnswer.AsEnumerable().Where(ans => questionIDs.Contains(ans.Field<string>("Fk_Question"))).ToList();
                answerColl = new List<Answer>();

                foreach (DataRow dr in drColl)
                {
                    answer = new Answer();

                    answer.ID = dr[EntityConstents.COL_ANSWERS_ID].ToString();
                    answer.Answerr = dr[EntityConstents.COL_ANSWERS_ANSWER].ToString();
                    answer.AnswerOrder = dr[EntityConstents.COL_ANSWERS_ORDER].ToString();
                    answer.Value = dr[EntityConstents.COL_ANSWERS_VALUE].ToString();

                    answerColl.Add(answer);
                }
            }
            catch
            {
            }

            return answerColl;
        }

        public List<Answer> LoadAnswersByID(List<string> answerIDs)
        {
            DataTable dtAnswer = null;
            List<Answer> answerColl = null;
            List<DataRow> drColl = null;
            Answer answer = null;

            try
            {
                LoadXML();
                dtAnswer = oeEntity.Tables[EntityConstents.TBL_ANSWERS];
                drColl = dtAnswer.AsEnumerable().Where(ans => answerIDs.Contains(ans.Field<string>("ID"))).ToList();
                answerColl = new List<Answer>();

                foreach (DataRow dr in drColl)
                {
                    answer = new Answer();

                    answer.ID = dr[EntityConstents.COL_ANSWERS_ID].ToString();
                    answer.Answerr = dr[EntityConstents.COL_ANSWERS_ANSWER].ToString();
                    answer.AnswerOrder = dr[EntityConstents.COL_ANSWERS_ORDER].ToString();
                    answer.Value = dr[EntityConstents.COL_ANSWERS_VALUE].ToString();

                    answerColl.Add(answer);
                }
            }
            catch
            {
            }

            return answerColl;
        }
        #endregion

        #region QUESTIONBANK
        public void AddQuestionBank(oEEntiti Qbinfo)
        {
            DataTable dtQuestionBank = null;
            DataTable dtQBQuestions = null;
            DataRow drNewQB = null;
            DataRow drNewQBRel = null;
            List<DataRow> drNewQBRelColl = null;
            QBInfo qbinfo = null;
            string qbID = string.Empty;

            try
            {
                qbinfo = Qbinfo as QBInfo;

                LoadXML();
                dtQuestionBank = oeEntity.Tables[EntityConstents.TBL_QUESTIONSBANK];
                dtQBQuestions = oeEntity.Tables[EntityConstents.TBL_QUESTIONSBANK_QUES];

                if (qbinfo.EntityState == EntityOperationalState.New)
                {
                    drNewQB = dtQuestionBank.NewRow();
                    drNewQB[EntityConstents.COL_QUESTIONSBANK_ID] = Guid.NewGuid().ToString();
                }
                else
                {
                    drNewQB = dtQuestionBank.AsEnumerable().Where(ans => ans.Field<string>("ID") == qbinfo.ID).SingleOrDefault();
                }

                qbID = drNewQB[EntityConstents.COL_QUESTIONSBANK_ID].ToString();
                drNewQB[EntityConstents.COL_QUESTIONSBANK_EXAMNAME] = qbinfo.QBName;
                drNewQB[EntityConstents.COL_QUESTIONSBANK_REMARKS] = qbinfo.Remarks;

                if (qbinfo.EntityState == EntityOperationalState.New)
                    dtQuestionBank.Rows.Add(drNewQB);

                if (qbinfo.QBQuestionsEntityState == EntityOperationalState.New || qbinfo.QBQuestionsEntityState == EntityOperationalState.Update)
                {
                    if (qbinfo.QBQuestionsEntityState == EntityOperationalState.Update)
                    {
                        drNewQBRelColl = dtQBQuestions.AsEnumerable().Where(ans => ans.Field<string>("Fk_Question_Bank") == qbID).ToList();
                        if (drNewQBRelColl != null && drNewQBRelColl.Count > 0)
                        {
                            for (int cnt = drNewQBRelColl.Count-1; cnt >= 0; cnt--)
                            {
                                dtQBQuestions.Rows.Remove(drNewQBRelColl[cnt]);
                            }
                        }
                    }

                    foreach (QBQuestions qbq in qbinfo.QBQuestions)
                    {
                        drNewQBRel = dtQBQuestions.NewRow();
                        drNewQBRel[EntityConstents.COL_QUESTIONSBANK_QUES_ID] = Guid.NewGuid().ToString();
                        drNewQBRel[EntityConstents.COL_QUESTIONSBANK_QUES_REL_ID] = qbq.Question_Rel_ID;
                        drNewQBRel[EntityConstents.COL_QUESTIONSBANK_QUES_BANK_ID] = qbID;
                        drNewQBRel[EntityConstents.COL_QUESTIONSBANK_QUES_ORDER] = qbq.Order;
                        dtQBQuestions.Rows.Add(drNewQBRel);
                    }
                }
                saveXML(oeEntity);
            }
            catch
            {
                throw;
            }
        }

        public List<ParticipantAssesment> LoadParticipentAssement(string ParticipentID)
        {
            DataTable dtQuestionBank = null;
            DataTable dtParticipentAssesment = null;
            DataTable dtParticipent = null;
            List<ParticipantAssesment> paColl = null;

            try
            {
                LoadXML();
                dtQuestionBank = oeEntity.Tables[EntityConstents.TBL_QUESTIONSBANK];
                dtParticipentAssesment = oeEntity.Tables[EntityConstents.TBL_PARTICIPANTASSESMENT];
                dtParticipent = oeEntity.Tables[EntityConstents.TBL_PARTICIPANT];

                paColl = (from pa in dtParticipentAssesment.AsEnumerable()
                          where pa.Field<string>("Fk_Participant") == ParticipentID
                          select new ParticipantAssesment
                          {
                              ID = pa.Field<string>("ID"),
                              QuestionBank = (from qb in dtQuestionBank.AsEnumerable()
                                              where qb.Field<string>("ID") == pa.Field<string>("Fk_QuesBank")
                                              select new QuestionBank
                                              {
                                                  ID = qb.Field<string>("ID"),
                                                  ExamName = qb.Field<string>("ExamName"),
                                                  Remarks = qb.Field<string>("Remarks"),
                                              }).FirstOrDefault(),
                              Participant = (from parti in dtParticipent.AsEnumerable()
                                             where parti.Field<string>("ID") == ParticipentID
                                             select new Participant
                                             {
                                                 ID = parti.Field<string>("ID"),
                                                 Code = parti.Field<string>("Code"),
                                                 Name = parti.Field<string>("Name"),
                                                 Gender = parti.Field<string>("Gender"),
                                                 Email = parti.Field<string>("Email"),
                                                 Active = parti.Field<string>("Active") == "True" ? true : false,
                                                 Remarks = parti.Field<string>("Remarks"),
                                             }).FirstOrDefault(),
                          }).ToList();
            }
            catch
            {
                throw;
            }

            return paColl;
        }

        public List<QuestionBank> LoadQuestionBank()
        {
            DataTable dtQuestionBank = null;
            List<QuestionBank> qbColl = null;

            try
            {
                LoadXML();
                dtQuestionBank = oeEntity.Tables[EntityConstents.TBL_QUESTIONSBANK];
                qbColl = (from qb in dtQuestionBank.AsEnumerable()
                select new QuestionBank {
                    ID =
                                 qb.Field<string>("ID"),
                    ExamName =
                                 qb.Field<string>("ExamName"),
                    Remarks =
                                 qb.Field<string>("Remarks"),
                }).ToList();
            }
            catch
            {
                throw;
            }

            return qbColl;
        }
        public List<QuestionBank> LoadQuestionBank(string QbID)
        {
            DataTable dtQuestionBank = null;
            DataTable dtQBQuestions = null;
            DataTable dtQuestionRel = null;
            DataTable dtGroupType = null;
            DataTable dtTopicType = null;
            DataTable dtQuestion = null;
            DataTable dtQuestionMode = null;
            DataTable dtAnswer = null;

            string questionBankID = string.Empty;
            string questionID = string.Empty;
            string ExamName = string.Empty;
            string Remarks = string.Empty;

            List<string> qbIdColl = null;
            List<DataRow> drs = null;
            List<QuestionBank> qbColl = null;

            try
            {
                LoadXML();
                dtQuestionBank = oeEntity.Tables[EntityConstents.TBL_QUESTIONSBANK];
                dtQBQuestions = oeEntity.Tables[EntityConstents.TBL_QUESTIONSBANK_QUES];
                dtQuestionRel = oeEntity.Tables[EntityConstents.TBL_QUESTIONS_REL];
                dtGroupType = oeEntity.Tables[EntityConstents.TBL_GROUPTYPE];
                dtTopicType = oeEntity.Tables[EntityConstents.TBL_TOPICTYPE];
                dtQuestion = oeEntity.Tables[EntityConstents.TBL_QUESTIONS];
                dtQuestionMode = oeEntity.Tables[EntityConstents.TBL_QUESTIONMODE];
                dtAnswer = oeEntity.Tables[EntityConstents.TBL_ANSWERS];

                qbIdColl = new List<string>();


                if (dtQuestionBank != null && dtQuestionBank.Rows.Count > 0)
                {
                    qbColl =
                        (from qb in dtQuestionBank.AsEnumerable()
                         where qb.Field<string>("ID") == QbID
                         select new QuestionBank
                         {
                             ID =
                                 qb.Field<string>("ID"),
                             ExamName =
                                 qb.Field<string>("ExamName"),
                             Remarks =
                                 qb.Field<string>("Remarks"),
                             Questions = (from qbqq in dtQBQuestions.AsEnumerable()
                                          where qbqq.Field<string>("Fk_Question_Bank") == qb.Field<string>("ID")
                                          select new QuestionBankQuestions
                                          {
                                              ID = qbqq.Field<string>("ID"),
                                              Order = qbqq.Field<string>("QuestionOrder"),
                                              QuestionRelation = (from qr in dtQuestionRel.AsEnumerable()
                                                                  where qr.Field<string>("ID") == qbqq.Field<string>("Fk_Question_ID")
                                                                  select new QuestionRelation
                                                                  {
                                                                      ID = qr.Field<string>("ID"),
                                                                      QuestionTyp = LoadQuestionTypeByQID(qr.Field<string>("Fk_Que_ID")),
                                                                      Questionn = (from que in dtQuestion.AsEnumerable()
                                                                                   where que.Field<string>("ID") == qr.Field<string>("Fk_Que_ID")
                                                                                   select new Question
                                                                                   {
                                                                                       ID = que.Field<string>("ID"),
                                                                                       ComplexLevel = que.Field<string>("ComplexLevel"),
                                                                                       Point = que.Field<string>("Point"),
                                                                                       Questionn = que.Field<string>("Question"),
                                                                                       Answers = (from ans in dtAnswer.AsEnumerable()
                                                                                                  where ans.Field<string>("Fk_Question") == que.Field<string>("ID")
                                                                                                  select new Answer
                                                                                                  {
                                                                                                      ID = ans.Field<string>("ID"),
                                                                                                      AnswerOrder = ans.Field<string>("AnswerOrder"),
                                                                                                      Answerr = ans.Field<string>("Answer"),
                                                                                                      Value = ans.Field<string>("Value"),
                                                                                                  }).ToList()
                                                                                   }).FirstOrDefault()
                                                                  }).FirstOrDefault()
                                          }).ToList()
                         }).ToList();
                    int i = 10;
                    //IEnumerable<QuestionBank> qbEnumerable = ii.Select(qb => qb).FirstOrDefault();
                    //qbColl = qbEnumerable.ToList();
                }
            }
            catch
            {
                throw;
            }

            return qbColl;
        }
        #endregion

        #region PARTICIPANT
        public void AddParticipant(oEEntiti Participant)
        {
            DataTable dtParticipant = null;
            DataTable dtParticipantAssesment = null;
            DataRow drNew = null;
            DataRow drNewPartiAss = null;
            List<DataRow> drParticepentColl = null;
            //Participant participant = null;
            ParticeipentInfo particeipentInfo = null;
            string participentID = string.Empty;

            try
            {
                particeipentInfo = Participant as ParticeipentInfo;
                LoadXML();
                dtParticipant = oeEntity.Tables[EntityConstents.TBL_PARTICIPANT];
                dtParticipantAssesment = oeEntity.Tables[EntityConstents.TBL_PARTICIPANTASSESMENT];

                if (particeipentInfo.EntityState == EntityOperationalState.New)
                {
                    drNew = dtParticipant.NewRow();
                    participentID = Guid.NewGuid().ToString();
                    drNew[EntityConstents.COL_PARTICIPANT_ID] = participentID;
                    dtParticipant.Rows.Add(drNew);
                }
                else
                {
                    participentID = particeipentInfo.ID;
                    drNew = dtParticipant.AsEnumerable().Where(ans => ans.Field<string>("ID") == particeipentInfo.ID).SingleOrDefault();
                }

                drNew[EntityConstents.COL_PARTICIPANT_CODE] = particeipentInfo.Code;
                drNew[EntityConstents.COL_PARTICIPANT_NAME] = particeipentInfo.Name;
                drNew[EntityConstents.COL_PARTICIPANT_ACTIVE] = particeipentInfo.Active;
                drNew[EntityConstents.COL_PARTICIPANT_EMAIL] = particeipentInfo.Email;
                drNew[EntityConstents.COL_PARTICIPANT_GENDER] = particeipentInfo.Gender;
                drNew[EntityConstents.COL_PARTICIPANT_REMARKS] = particeipentInfo.Remarks;

                if (particeipentInfo.ParticeipentAssesmentEntityState == EntityOperationalState.New || particeipentInfo.ParticeipentAssesmentEntityState == EntityOperationalState.Update)
                {
                    if (particeipentInfo.ParticeipentAssesmentEntityState == EntityOperationalState.Update)
                    {
                        drParticepentColl = dtParticipantAssesment.AsEnumerable().Where(partiass => partiass.Field<string>("Fk_Participant") == participentID).ToList();
                        if (drParticepentColl != null && drParticepentColl.Count > 0)
                        {
                            for (int cnt = drParticepentColl.Count; cnt >= 0; cnt--)
                            {
                                dtParticipantAssesment.Rows.Remove(drParticepentColl[cnt]);
                            }
                        }
                    }

                    foreach (string qbID in particeipentInfo.QBIds)
                    {
                        drNewPartiAss = dtParticipantAssesment.NewRow();
                        drNewPartiAss[EntityConstents.COL_PARTICIPANTASSESMENT_ID] = Guid.NewGuid().ToString();
                        drNewPartiAss[EntityConstents.COL_PARTICIPANTASSESMENT_QUESTIONBANK] = qbID;
                        drNewPartiAss[EntityConstents.COL_PARTICIPANTASSESMENT_PARTICIPANT] = participentID;
                        dtParticipantAssesment.Rows.Add(drNewPartiAss);
                    }
                }

                saveXML(oeEntity);
            }
            catch
            {
                throw;
            }
        }

        public List<Participant> LoadParticipents()
        {
            DataTable dtPArticipent = null;
            List<Participant> participentColl = null;
            Participant participant = null;

            try
            {
                LoadXML();
                dtPArticipent = oeEntity.Tables[EntityConstents.TBL_PARTICIPANT];
                participentColl = new List<Participant>();
                foreach (DataRow dr in dtPArticipent.Rows)
                {
                    participant = new Participant();

                    participant.ID = dr[EntityConstents.COL_PARTICIPANT_ID].ToString();
                    participant.Code = dr[EntityConstents.COL_PARTICIPANT_CODE].ToString();
                    participant.Name = dr[EntityConstents.COL_PARTICIPANT_NAME].ToString();

                    participentColl.Add(participant);
                }
            }
            catch
            {
            }

            return participentColl;
        }
        #endregion

        #region PARTICIPANTASSESMENT
        public void AddParticipantAssesment(ParticipantAssesment ParticipantAssesment)
        {
            DataTable dtParticipantAssesment = null;
            DataRow drNew = null;

            try
            {
                LoadXML();
                dtParticipantAssesment = oeEntity.Tables[EntityConstents.TBL_PARTICIPANTASSESMENT];
                drNew = dtParticipantAssesment.NewRow();
                drNew[EntityConstents.COL_PARTICIPANTASSESMENT_ID] = Guid.NewGuid().ToString();
                drNew[EntityConstents.COL_PARTICIPANTASSESMENT_PARTICIPANT] = ParticipantAssesment.Participant;
                drNew[EntityConstents.COL_PARTICIPANTASSESMENT_QUESTIONBANK] = ParticipantAssesment.QuestionBank;
                drNew[EntityConstents.COL_PARTICIPANTASSESMENT_REMARKS] = ParticipantAssesment.Remarks;

                dtParticipantAssesment.Rows.Add(drNew);
                saveXML(oeEntity);
            }
            catch
            {
            }
        }
        #endregion

        #region PARTICIPANTANSWER
        public void AddParticipantAnswer(ParticipantAnswer ParticipantAnswer)
        {
            DataTable dtParticipantAnswer = null;
            DataRow drNew = null;

            try
            {
                LoadXML();
                dtParticipantAnswer = oeEntity.Tables[EntityConstents.TBL_PARTICIPANTANSWER];
                drNew = dtParticipantAnswer.NewRow();
                drNew[EntityConstents.COL_PARTICIPANTANSWER_ID] = Guid.NewGuid().ToString();
                drNew[EntityConstents.COL_PARTICIPANTANSWER_ANSWER] = ParticipantAnswer.Answer;
                drNew[EntityConstents.COL_PARTICIPANTANSWER_PARTICIPANTASSESMENT] = ParticipantAnswer.ParticipantAssesment;
                //drNew[EntityConstents.COL_PARTICIPANTANSWER_QUESTION] = ParticipantAnswer.Question;

                dtParticipantAnswer.Rows.Add(drNew);
                saveXML(oeEntity);
            }
            catch
            {
            }
        }
        #endregion
    }
}
