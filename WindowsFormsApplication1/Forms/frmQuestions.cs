using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using oEEntity.Base;
using oEEntity.Model;
using oELib;

namespace WindowsFormsApplication1.Forms
{
    public partial class frmQuestions : Form
    {

        public EntityOperationalState QuestionOperatonState = EntityOperationalState.None;
        public EntityOperationalState AnswerOperatonState = EntityOperationalState.None;
        public Question EditedQuestion = null;

        public frmQuestions()
        {
            InitializeComponent();
        }

        #region METHODS
        private void LoadData()
        {
            List<QuestionType> QuestionTypeColl = null;
            Dictionary<string, string> comboSource = null;

            MasterDataFunctions mDataFunc = new MasterDataFunctions();

            #region LOAD GROUP

            comboSource = new Dictionary<string, string>();
            QuestionTypeColl = mDataFunc.LoadQuestionType(QuestionTypes.Group);

            if (QuestionTypeColl != null && QuestionTypeColl.Count > 0)
            {
                foreach (QuestionType gt in QuestionTypeColl)
                {
                    comboSource.Add(gt.ID, gt.Code);
                }

                cbGroupType.DataSource = new BindingSource(comboSource, null);
                cbGroupType.DisplayMember = "Value";
                cbGroupType.ValueMember = "Key";
            }
            #endregion

            #region LOADTOPIC
            /*TopicTypeColl = mDataFunc.LoadTopic();
            comboSource = new Dictionary<string, string>();
            foreach (TopicType gt in TopicTypeColl)
            {
                comboSource.Add(gt.ID, gt.Code);
            }

            cbTopicType.DataSource = new BindingSource(comboSource, null);
            cbTopicType.DisplayMember = "Value";
            cbTopicType.ValueMember = "Key";*/
            #endregion

            #region MODE
            comboSource = new Dictionary<string, string>();
            QuestionTypeColl = mDataFunc.LoadQuestionType(QuestionTypes.Mode);

            if (QuestionTypeColl != null && QuestionTypeColl.Count > 0)
            {
                foreach (QuestionType gt in QuestionTypeColl)
                {
                    comboSource.Add(gt.ID, gt.Code);
                }

                cbQuestionMode.DataSource = new BindingSource(comboSource, null);
                cbQuestionMode.DisplayMember = "Value";
                cbQuestionMode.ValueMember = "Key";
            }
            #endregion

            #region COMPLEXLEVEL
            cbComplexlevel.Items.Add("1");
            cbComplexlevel.Items.Add("2");
            cbComplexlevel.Items.Add("3");
            cbComplexlevel.Items.Add("4");
            cbComplexlevel.Items.Add("5");
            #endregion
        }
        private void LoadQuestionsGrid()
        {
            List <QuestionRelation> questionRColl = null;
            MasterDataFunctions mDataFunc = null;
            string groupTypeID = string.Empty;
            string topicTypeID = string.Empty;
            QuestionType qtype = null;

            try
            {
                mDataFunc = new MasterDataFunctions();
                if(cbGroupType.SelectedItem != null)
                    groupTypeID = ((KeyValuePair<string, string>)cbGroupType.SelectedItem).Key;
                if (cbTopicType.SelectedItem != null)
                    topicTypeID = ((KeyValuePair<string, string>)cbTopicType.SelectedItem).Key;

                questionRColl = mDataFunc.LoadQuestionsRelations(topicTypeID);

                if (questionRColl != null && questionRColl.Count > 0)
                {
                    dgQuestions.Rows.Clear();
                    //loop all row in datatable
                    for (int i = 0; i < questionRColl.Count; i++)
                    {
                        //add a row int datagridview before fill
                        dgQuestions.Rows.Add();
                        //fill the first cell value ot ith row of datagridview
                        dgQuestions.Rows[i].Cells[0].Value = questionRColl[i].Questionn.ID;
                        dgQuestions.Rows[i].Cells[1].Value = questionRColl[i].Questionn.Questionn;
                        dgQuestions.Rows[i].Cells[2].Value = questionRColl[i].Questionn.Point;
                        qtype = questionRColl[i].QuestionTyp.Where(qt => qt.Type == QuestionTypes.Topic).SingleOrDefault();
                        if (qtype != null)
                        {
                            dgQuestions.Rows[i].Cells[4].Value = qtype.Code;
                            dgQuestions.Rows[i].Cells[3].Value = qtype.FkQuestionType.Code;
                        }


                        //here for combobx column (cast the column as datagridviewcombobox column)
                        DataGridViewButtonCell cell = (DataGridViewButtonCell)dgQuestions.Rows[i].Cells[5];
                        //then assign the value of cell                
                        cell.Value = "Answers";
                        //see the top where we added the comboboxcolumn and define it's item, 
                        //so here value is on of the items of coboboxcolumn item.

                        //delete button cell value
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        private void SaveQuestionTypeForQuestion(SaveQuestion saveQuestion)
        {
            string modeID = string.Empty;
            string groupTypeID = string.Empty;
            string topicTypeID = string.Empty;
            MasterDataFunctions mDataFunc = null;
            SaveQuestionType saveQuestionType = null;
            SaveQuestionTypes questionType = null;
            List<SaveQuestionTypes> questionTypeColl = null;

            try
            {
                groupTypeID = ((KeyValuePair<string, string>)cbGroupType.SelectedItem).Key;
                topicTypeID = ((KeyValuePair<string, string>)cbTopicType.SelectedItem).Key;
                modeID = ((KeyValuePair<string, string>)cbQuestionMode.SelectedItem).Key;

                mDataFunc = new MasterDataFunctions();
                saveQuestionType = new SaveQuestionType();
                questionType = new SaveQuestionTypes();
                questionTypeColl = new List<SaveQuestionTypes>();

                questionType.Type = QuestionTypes.Topic;
                questionType.ID = topicTypeID;
                questionTypeColl.Add(questionType);
                questionType = new SaveQuestionTypes();
                questionType.Type = QuestionTypes.Mode;
                questionType.ID = modeID;
                questionTypeColl.Add(questionType);

                saveQuestionType.QuestionID = saveQuestion.QuestionID;
                saveQuestionType.Code = string.Empty;
                saveQuestionType.Description = string.Empty;
                saveQuestionType.Type = QuestionTypes.Question;
                saveQuestionType.QuestionTypes = questionTypeColl;

                mDataFunc.SaveQuestionTypeRelation(saveQuestionType);
            }
            catch
            {
                throw;
            }
        }
        private void ResetAll()
        {
            txtQuestion.Text = string.Empty;
            txtPoint.Text = string.Empty;
            cbComplexlevel.SelectedIndex = 0;
            cbQuestionMode.SelectedIndex = 0;
            txtQuestionQV.Text = string.Empty;
            dgQuestions.Rows.Clear();
            EditedQuestion = null;
        }
        #endregion

        #region EVENTS
        private void frmQuestions_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadQuestionsGrid();
            if (cbGroupType.Items.Count > 0)
            {
                cbGroupType.SelectedIndex = 0;
                cbGroupType_SelectionChangeCommitted(null, null);
            }
            QuestionOperatonState = EntityOperationalState.New;
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            txtQuestion.Text = string.Empty;
            txtPoint.Text = string.Empty;
            cbComplexlevel.SelectedIndex = 0;
            cbQuestionMode.SelectedIndex = 0;

            QuestionOperatonState = EntityOperationalState.New;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveQuestion saveQuestion = null;
            string quesGroupID = string.Empty;
            string quesTopicID = string.Empty;
            string quesModeID = string.Empty;

            string questionID = string.Empty;
            MasterDataFunctions mDataFunc = null;

            try
            {
                mDataFunc = new MasterDataFunctions();
                saveQuestion = new SaveQuestion();
                quesGroupID = ((KeyValuePair<string, string>)cbGroupType.SelectedItem).Key;
                quesTopicID = ((KeyValuePair<string, string>)cbTopicType.SelectedItem).Key;
                quesModeID = ((KeyValuePair<string, string>)cbQuestionMode.SelectedItem).Key;

                if (EditedQuestion != null)
                    saveQuestion.ID = EditedQuestion.ID;

                saveQuestion.Questionn = txtQuestion.Text;
                saveQuestion.ComplexLevel = cbComplexlevel.Text;
                saveQuestion.Point = txtPoint.Text;
                saveQuestion.EntityState = QuestionOperatonState;

                questionID = mDataFunc.SaveQuestion(saveQuestion);
                saveQuestion.QuestionID = questionID;

                if (QuestionOperatonState == EntityOperationalState.New)
                    SaveQuestionTypeForQuestion(saveQuestion);

                ResetAll();
                LoadQuestionsGrid();
                EditedQuestion = null;

                MessageBox.Show("Question saved.");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnSetRelation_Click(object sender, EventArgs e)
        {
            try
            {
                ResetAll();
                LoadQuestionsGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dgQuestions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                List<Question> questionColl = null;
                List<string> questionIDs = null;
                string quesModeID = string.Empty;
                string questionID = string.Empty;
                MasterDataFunctions mDataFunc = null;

                try
                {
                    mDataFunc = new MasterDataFunctions();
                    questionIDs = new List<string>();
                    questionIDs.Add(dgQuestions.Rows[e.RowIndex].Cells[0].Value.ToString());
                    questionColl = mDataFunc.LoadQuestion(questionIDs);
                    frmAnswers answer = new frmAnswers(questionColl[0]);
                    answer.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                txtQuestionQV.Text = dgQuestions.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
        }
        private void dgQuestions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            List<Question> questionColl = null;
            List<string> questionIDs = null;
            string quesModeID = string.Empty;
            string questionID = string.Empty;
            MasterDataFunctions mDataFunc = null;
            QuestionType qtype = null;
            List<QuestionType> questionTypeColl = null;

            try
            {
                mDataFunc = new MasterDataFunctions();
                questionIDs = new List<string>();
                questionIDs.Add(dgQuestions.Rows[e.RowIndex].Cells[0].Value.ToString());
                questionColl = mDataFunc.LoadQuestion(questionIDs);
                EditedQuestion = questionColl[0];
                txtQuestion.Text = dgQuestions.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtPoint.Text = dgQuestions.Rows[e.RowIndex].Cells[2].Value.ToString();
                cbComplexlevel.SelectedIndex = cbComplexlevel.FindString(EditedQuestion.ComplexLevel);//cbComplexlevel.FindString(dgQuestions.Rows[e.RowIndex].Cells[3].Value.ToString());

                questionTypeColl = mDataFunc.LoadQuestionTypeByQID(EditedQuestion.ID);
                qtype = questionTypeColl.Where(qt => qt.Type == QuestionTypes.Mode).SingleOrDefault();
                    cbQuestionMode.SelectedIndex = cbQuestionMode.FindString(qtype.Code);// cbQuestionMode.FindString(dgQuestions.Rows[e.RowIndex].Cells[4].Value.ToString());

                QuestionOperatonState = EntityOperationalState.Update;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void btnAddNewRelations_Click(object sender, EventArgs e)
        {
            frmMasterData masterData = new frmMasterData();
            masterData.ShowDialog();
        }

        private void cbGroupType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            MasterDataFunctions mDataFunc = null;
            string groupType = string.Empty;
            List<QuestionType> QuestionTypeColl = null;
            Dictionary<string, string> comboSource = null;

            try
            {
                mDataFunc = new MasterDataFunctions();
                groupType = ((KeyValuePair<string, string>)cbGroupType.SelectedItem).Key;
                comboSource = new Dictionary<string, string>();

                QuestionTypeColl = mDataFunc.LoadQuestionType(QuestionTypes.Topic);
                QuestionTypeColl = QuestionTypeColl.Where(qt => qt.FkQuestionType.ID == groupType).ToList();

                if (QuestionTypeColl != null && QuestionTypeColl.Count > 0)
                {
                    foreach (QuestionType gt in QuestionTypeColl)
                    {
                        comboSource.Add(gt.ID, gt.Code);
                    }

                    cbTopicType.DataSource = new BindingSource(comboSource, null);
                    cbTopicType.DisplayMember = "Value";
                    cbTopicType.ValueMember = "Key";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

