using oEEntity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oEEntity.Model
{
    public class SaveQuestionType : oEEntiti
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public QuestionTypes Type { get; set; }
        public string FkQuestionTypeID { get; set; }
        public string QuestionID { get; set; }

        public List<SaveQuestionTypes> QuestionTypes { get; set; }
    }

    public class SaveQuestionTypes : oEEntiti
    {
        public QuestionTypes Type { get; set; }
        public string ID { get; set; }
    }

    public class SaveQuestion : oEEntiti
    {
        public string ID { get; set; }
        public string Questionn { get; set; }
        public string ComplexLevel { get; set; }
        public string Point { get; set; }

        public string QuestionID { get; set; }
    }

    public class SaveQuestionRelation : oEEntiti
    {
        public string ID { get; set; }
        public string QuestionID { get; set; }
        public string GroupTypeID { get; set; }
        public string TopicTypeID { get; set; }
    }

    public class SaveAnswers : oEEntiti
    {
        public string ID { get; set; }
        public string Answerr { get; set; }
        public string AnswerOrder { get; set; }
        public string Value { get; set; }
        public string QuestionID { get; set; }
    }
    public class QBQuestions : oEEntiti
    {
        public string ID { get; set; }
        public string Question_Rel_ID { get; set; }
        public string Question_Bank_ID { get; set; }
        public string Order { get; set; }
    }

    public class QBInfo : oEEntiti
    {
        public string ID { get; set; }
        public string QBName { get; set; }
        public string Remarks { get; set; }
        public EntityOperationalState QBQuestionsEntityState { get; set; }
        public List<QBQuestions> QBQuestions { get; set; }
    }

    public class ParticeipentInfo : oEEntiti
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Remarks { get; set; }
        public bool Active { get; set; }
        public List <string> QBIds { get; set; }
        public EntityOperationalState ParticeipentAssesmentEntityState { get; set; }
    }
}
