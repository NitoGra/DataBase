using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DataBaseButton
{
    namespace NS
    {
        static class Program
        {
            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1()); // <- вот тут
            }
        }
    }

    public partial class Form1 : Form
    {
        private static bool isrPressed = false;
        private static bool isePressed = false;
        private static bool isdPressed = false;
        private static readonly int maxLengthName = 10;
        private static readonly int maxLengthLevel = 999;

        class DataBase
        {
            private Dictionary<int, Player> _players = new Dictionary<int, Player>();
            private List<string> _names = new List<string>();
            private int _id;

            public DataBase()
            {
                _players.Add(1, new Player(
                    name: "Папич",
                    level: 1));

                _players.Add(2, new Player(
                    name: "Первый",
                    level: 2));

                _players.Add(3, new Player(
                    name: "Второй",
                    level: 33));

                for (int i = 0; i < _players.Count; i++)
                    _names.Add(_players[i + 1].Name);

                _id = _players.Count;
            }

            public Dictionary<int, Player> ShowPlayer()
            {
                if (IsFilled() == false)
                    MessageBox.Show("Она пуста!");

                return _players;
            }

            public void AddPlayer(string name, string level)
            {
                if (name.Length > maxLengthName || name.Length < 1)
                {
                    MessageBox.Show("Ошибка ввода имени!");
                    return;
                }

                if (SearchСopy(name))
                {
                    MessageBox.Show("Повторяющееся имя!");
                    return;
                }

                if (!Int32.TryParse(level, out int levelInNumber))
                {
                    MessageBox.Show("Тут должны быть цифры!");
                    return;
                }

                if (levelInNumber > maxLengthLevel || levelInNumber < 1)
                {
                    MessageBox.Show("Ошибка ввода уровня!");
                    return;
                }

                _id++;
                _names.Add(name);
                _players.Add(_id, new Player(name, levelInNumber));

                MessageBox.Show("Новый игрок успешно добавлен!");
            }

            public void BanPlayer(int id)
            {
                _players[id].SwitchBanStatus(true);
            }

            public void UnbanPlayer(int id)
            {
                _players[id].SwitchBanStatus(false);
            }

            public void DeletePlayer(int id)
            {
                _names.Remove(_players[id].Name);

                if (_players.Remove(id))
                {
                    MessageBox.Show($"\nУдаление прошло успешно!");
                    return;
                }
            }

            public bool SearchСopy(string name)
            {
                if (_names.Contains(name))
                    return true;

                return false;
            }

            private bool IsFilled()
            {
                if (_players.Count > 0)
                    return true;

                MessageBox.Show("\nБаза данных пуста!");
                return false;
            }

            public string GetBanStatus(int id) => _players[id].GetBanStatus();
            public string GetName(int id) => _players[id].Name;
            public int GetLevel(int id) => _players[id].Level;
        }

        class Player
        {
            private bool _isBanned;

            public Player(string name, int level)
            {
                Name = name;
                Level = level;
                _isBanned = false;
            }

            public string Name { get; private set; }
            public int Level { get; private set; }

            public void SwitchBanStatus(bool isBanned)
            {
                if (IsStatusSame(isBanned))
                    return;

                _isBanned = isBanned;
                string message;

                if (_isBanned)
                    message = $"\n{Name} забанен!";
                else
                    message = $"\n{Name} разбанен!";

                MessageBox.Show(message);
            }

            public string GetBanStatus()
            {
                if (_isBanned)
                    return "забанен";

                return "не забанен";
            }

            private bool IsStatusSame(bool isBanned)
            {
                if (_isBanned != isBanned)
                    return false;

                if (_isBanned)
                {
                    MessageBox.Show($"\n{Name} и так забанен!");
                    return true;
                }

                MessageBox.Show($"\n{Name} не забанен!");
                return true;
            }
        }

        DataBase dataBase = new DataBase();

        public Form1()
        {
            InitializeComponent();
            BackColor = Color.White;
            textWriter();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataBase.AddPlayer(textBox1.Text, textBox2.Text);
            textWriter();

            if (dataBase.SearchСopy(textBox1.Text))
            {
                textBox1.BackColor = Color.Red;
                return;
            }
            else
                textBox1.BackColor = Color.White;
        }

        private bool TryGetId(out int id)
        {
            id = 0;

            if (textBox3.Text == null)
                return false;

            if (!Int32.TryParse(textBox3.Text, out id))
            {
                MessageBox.Show("Некорректный ID!");
                return false;
            }

            if (dataBase.ShowPlayer().ContainsKey(id))
                return true;

            MessageBox.Show("\nТакое ID не найдено!");

            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (TryGetId(out int id))
                dataBase.BanPlayer(id);

            textWriter();
            textBox3.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (TryGetId(out int id))
                dataBase.UnbanPlayer(id);

            textWriter();
            textBox3.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (TryGetId(out int id))
                dataBase.DeletePlayer(id);

            textWriter();
            textBox3.Clear();
        }

        private void textWriter()
        {
            Dictionary<int, Player> _p = dataBase.ShowPlayer();
            listBox1.Text = "";

            foreach (KeyValuePair<int, Player> player in _p)
            {
                int personalId = player.Key;
                listBox1.Text = listBox1.Text + $"|  Id: {personalId}  " +
                                                $"|  Статус бана: {dataBase.GetBanStatus(personalId)}  " +
                                                $"|  Уровень: {dataBase.GetLevel(personalId)}  " +
                                                $"|  {dataBase.GetName(personalId)}  \n";
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            textWriter();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string name = textBox1.Text;

            if (name.Length > maxLengthName || name.Length < 1)
            {
                textBox1.BackColor = Color.Red;
                return;
            }
            else
                textBox1.BackColor = Color.White;

            if (dataBase.SearchСopy(name))
            {
                textBox1.BackColor = Color.Red;
                return;
            }
            else
                textBox1.BackColor = Color.White;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 82:
                    isrPressed = true;
                    isePressed = false;
                    isdPressed = false;
                    break;

                case 69:
                    isePressed = true;
                    isdPressed = false;
                    break;

                case 68:
                    isdPressed = true;
                    break;

                default:
                    BackColor = Color.White;
                    isrPressed = false;
                    isePressed = false;
                    isdPressed = false;
                    break;
            }

            if ((isrPressed, isePressed, isdPressed) == (true, true, true))
            {
                BackColor = Color.Red;
                isrPressed = false;
                isePressed = false;
                isdPressed = false;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int level = 1;

            if (textBox2.Text != "")
                level = Convert.ToInt32(textBox2.Text);

            if (level > maxLengthLevel || level <= 0)
            {
                textBox2.BackColor = Color.Red;
                return;
            }
            else
                textBox2.BackColor = Color.White;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) == true)
                return;

            if (e.KeyChar == Convert.ToChar(Keys.Back))
                return;

            e.Handled = true;
            textBox2.Clear();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) == true)
                return;

            if (e.KeyChar == Convert.ToChar(Keys.Back))
                return;

            e.Handled = true;
            textBox3.Clear();
        }
    }
}