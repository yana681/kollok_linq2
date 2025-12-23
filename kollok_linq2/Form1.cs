using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kollok_linq2
{
    public partial class FormMain : Form
    {

        private Schedule schedule;
        private DataGridView dataGridView;
        public FormMain()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Расписание";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            LoadData();
            CreateSimpleUI();
            RefreshTable();
        }

        private void LoadData()
        {
            schedule = Schedule.LoadFromFile();
            if (schedule == null || schedule.Lessons.Count == 0)
            {
                schedule = new Schedule("Факультет");
                AddExampleData();
            }
        }

        private void AddExampleData()
        {
            schedule.Add("Матанализ", 1, 0, WeekDay.Tuesday, "21");
            schedule.Add("Алгебра", 2, 2, WeekDay.Tuesday, "21");
            schedule.Add("Физика", 3, 0, WeekDay.Monday, "22");
            schedule.SaveToFile();
        }

        private void CreateSimpleUI()
        {
            dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.ReadOnly = true;
            this.Controls.Add(dataGridView);
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 180;
            buttonPanel.BackColor = SystemColors.Control;
            this.Controls.Add(buttonPanel);
            Label title = new Label();
            title.Text = "Управление расписанием";
            title.Font = new Font("Arial", 10, FontStyle.Bold);
            title.Location = new Point(10, 10);
            title.AutoSize = true;
            buttonPanel.Controls.Add(title);

            int y = 40;
            Button btnAdd = CreateSimpleButton(" Добавить пару", 10, y, 150);
            btnAdd.Click += ShowAddDialog;
            y += 40;
            Button btnDay = CreateSimpleButton("Пары по дню", 10, y, 150);
            btnDay.Click += ShowDayQuery;

            Button btnGroup = CreateSimpleButton(" По группам", 170, y, 150);
            btnGroup.Click += ShowGroupQuery;

            Button btnSubject = CreateSimpleButton(" По предметам", 330, y, 150);
            btnSubject.Click += ShowSubjectQuery;
            y += 50;
            Button btnSave = CreateSimpleButton(" Сохранить", 10, y, 120);
            btnSave.Click += (s, e) => SaveData();

            Button btnLoad = CreateSimpleButton(" Загрузить", 140, y, 120);
            btnLoad.Click += (s, e) => LoadNewData();
            buttonPanel.Controls.AddRange(new Control[] {
                btnAdd, btnDay, btnGroup, btnSubject, btnSave, btnLoad
            });
        }

        private Button CreateSimpleButton(string text, int x, int y, int width)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 35),
                BackColor = SystemColors.ButtonFace,
                FlatStyle = FlatStyle.Standard
            };
        }

        private void RefreshTable()
        {
            dataGridView.DataSource = schedule.Lessons
                .Select(l => new
                {
                    Предмет = l.Label,
                    Пара = l.Number,
                    День = l.WeekDay.ToString(),
                    Группа = l.GroupNumber,
                    Неделя = l.GetWeekInfo()
                })
                .ToList();
        }

        private void SaveData()
        {
            schedule.SaveToFile();
            MessageBox.Show("Сохранено!", "Успех");
        }

        private void LoadNewData()
        {
            schedule = Schedule.LoadFromFile();
            if (schedule != null)
            {
                RefreshTable();
                MessageBox.Show("Загружено!", "Успех");
            }
        }
        private void ShowAddDialog(object sender, EventArgs e)
        {
            Form addForm = new Form();
            addForm.Text = "Добавить пару";
            addForm.Size = new Size(400, 350);
            addForm.StartPosition = FormStartPosition.CenterParent;
            addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addForm.MaximizeBox = false;

            int y = 20;
            Label lbl1 = new Label { Text = "Предмет:", Location = new Point(20, y), Width = 100 };
            TextBox txtSubject = new TextBox { Location = new Point(120, y), Width = 200 };
            y += 35;

            Label lbl2 = new Label { Text = "Номер пары:", Location = new Point(20, y), Width = 100 };
            NumericUpDown numPair = new NumericUpDown
            {
                Location = new Point(120, y),
                Width = 60,
                Minimum = 1,
                Maximum = 8
            };
            y += 35;

            Label lbl3 = new Label { Text = "День недели:", Location = new Point(20, y), Width = 100 };
            ComboBox cmbDay = new ComboBox
            {
                Location = new Point(120, y),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDay.Items.AddRange(Enum.GetNames(typeof(WeekDay)));
            cmbDay.SelectedIndex = 0;
            y += 35;

            Label lbl4 = new Label { Text = "Неделя:", Location = new Point(20, y), Width = 100 };
            ComboBox cmbWeek = new ComboBox
            {
                Location = new Point(120, y),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbWeek.Items.AddRange(new[] { "Каждую неделю", "Первая неделя", "Вторая неделя" });
            cmbWeek.SelectedIndex = 0;
            y += 35;

            Label lbl5 = new Label { Text = "Группа:", Location = new Point(20, y), Width = 100 };
            TextBox txtGroup = new TextBox { Location = new Point(120, y), Width = 100 };
            y += 50;
            Button btnOk = new Button
            {
                Text = "Добавить",
                Location = new Point(100, y),
                Size = new Size(100, 30),
                DialogResult = DialogResult.OK
            };

            Button btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(210, y),
                Size = new Size(100, 30),
                DialogResult = DialogResult.Cancel
            };

            btnOk.Click += (s, ev) =>
            {
                try
                {
                    schedule.Add(
                        txtSubject.Text,
                        (int)numPair.Value,
                        cmbWeek.SelectedIndex,
                        (WeekDay)Enum.Parse(typeof(WeekDay), cmbDay.Text),
                        txtGroup.Text
                    );

                    RefreshTable();
                    addForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            };

            addForm.Controls.AddRange(new Control[] {
                lbl1, txtSubject, lbl2, numPair, lbl3, cmbDay,
                lbl4, cmbWeek, lbl5, txtGroup, btnOk, btnCancel
            });

            addForm.ShowDialog();
        }


        // 6.1) Пары по дню недели
        private void ShowDayQuery(object sender, EventArgs e)
        {
            Form queryForm = new Form();
            queryForm.Text = "Пары по дню";
            queryForm.Size = new Size(400, 300);

            ComboBox cmb = new ComboBox
            {
                Location = new Point(20, 20),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmb.Items.AddRange(Enum.GetNames(typeof(WeekDay)));
            cmb.SelectedIndex = 0;

            ListBox list = new ListBox
            {
                Location = new Point(20, 60),
                Size = new Size(340, 180)
            };

            Button btn = new Button
            {
                Text = "Найти",
                Location = new Point(230, 18),
                Size = new Size(80, 25)
            };

            btn.Click += (s, ev) =>
            {
                list.Items.Clear();
                var day = (WeekDay)Enum.Parse(typeof(WeekDay), cmb.Text);
                var lessons = schedule.GetLessonsByDay(day);

                foreach (var lesson in lessons)
                {
                    list.Items.Add($"{lesson.Number} пара: {lesson.Label} (гр. {lesson.GroupNumber})");
                }
            };

            queryForm.Controls.AddRange(new Control[] { cmb, list, btn });
            queryForm.ShowDialog();
        }

        // 6.2) По группам
        private void ShowGroupQuery(object sender, EventArgs e)
        {
            Form queryForm = new Form();
            queryForm.Text = "По группам";
            queryForm.Size = new Size(500, 400);

            ListBox list = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9)
            };

            var groups = schedule.GetScheduleByGroups();

            foreach (var group in groups)
            {
                list.Items.Add($"=== Группа {group.Key} ===");

                foreach (var lesson in group.Value)
                {
                    list.Items.Add($"  {lesson.Number} пара: {lesson.Label} ({lesson.WeekDay})");
                }

                list.Items.Add("");
            }

            queryForm.Controls.Add(list);
            queryForm.ShowDialog();
        }

        // 6.3) По предметам
        private void ShowSubjectQuery(object sender, EventArgs e)
        {
            Form queryForm = new Form();
            queryForm.Text = "По предметам";
            queryForm.Size = new Size(500, 400);

            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true
            };

            var subjects = schedule.GetLessonsBySubject();
            var data = new List<object>();

            foreach (var subject in subjects)
            {
                foreach (var lesson in subject.Value)
                {
                    data.Add(new
                    {
                        Предмет = subject.Key,
                        Пара = lesson.Number,
                        День = lesson.WeekDay.ToString(),
                        Группа = lesson.GroupNumber
                    });
                }
            }

            dgv.DataSource = data;
            queryForm.Controls.Add(dgv);
            queryForm.ShowDialog();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            schedule.SaveToFile();
            base.OnFormClosing(e);
        }
    }
}
