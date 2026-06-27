using System;
using System.Windows;
using Microsoft.Win32;

namespace ReportAutomation
{
    // Класс главного окна. Реализует графический интерфейс
    // и принимает команды оператора: выбор источников данных,
    // задание отчётного периода и запуск формирования отчёта.
    public partial class MainWindow : Window
    {
        // Путь к файлу с производственными показателями.
        private string indicatorsFilePath = string.Empty;

        // Путь к файлу справочника подразделений.
        private string departmentsFilePath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Оператор выбирает файл с показателями через стандартное окно.
        private void SelectIndicatorsFile_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectExcelFile();
            if (!string.IsNullOrEmpty(path))
            {
                indicatorsFilePath = path;
                TextIndicatorsFile.Text = path;
            }
        }

        // Оператор выбирает файл справочника подразделений.
        private void SelectDepartmentsFile_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectExcelFile();
            if (!string.IsNullOrEmpty(path))
            {
                departmentsFilePath = path;
                TextDepartmentsFile.Text = path;
            }
        }

        // Общий метод выбора файла электронной таблицы.
        private string SelectExcelFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Электронные таблицы (*.xlsx)|*.xlsx";
            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }

        // Основной обработчик: запускает формирование отчёта.
        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            // Проверка того, что оператор задал все исходные параметры.
            if (DatePeriodStart.SelectedDate == null || DatePeriodEnd.SelectedDate == null)
            {
                MessageBox.Show("Необходимо задать отчётный период.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(indicatorsFilePath) ||
                string.IsNullOrEmpty(departmentsFilePath))
            {
                MessageBox.Show("Необходимо указать источники исходных данных.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime periodStart = DatePeriodStart.SelectedDate.Value;
            DateTime periodEnd = DatePeriodEnd.SelectedDate.Value;

            if (periodStart > periodEnd)
            {
                MessageBox.Show("Дата начала периода превышает дату окончания.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Формирование параметров запуска и вызов управляющего класса.
            ButtonGenerate.IsEnabled = false;
            TextStatus.Text = "Выполняется формирование отчёта...";

            ReportRequest request = new ReportRequest
            {
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                IndicatorsFilePath = indicatorsFilePath,
                DepartmentsFilePath = departmentsFilePath
            };

            ReportController controller = new ReportController();
            ReportResult result = controller.Run(request);

            // Вывод журнала операций и итогового результата оператору.
            ShowLog(result);
            ShowResult(result);

            ButtonGenerate.IsEnabled = true;
        }

        // Выводит записи журнала операций в текстовое поле.
        private void ShowLog(ReportResult result)
        {
            TextLog.Text = result.Log;
        }

        // Выводит итоговое сообщение о результате формирования отчёта.
        private void ShowResult(ReportResult result)
        {
            if (result.Success)
            {
                TextStatus.Text = "Отчёт сформирован: " + result.OutputFilePath;
                MessageBox.Show("Отчёт сформирован.",
                    "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                TextStatus.Text = "Ошибка формирования отчёта";
                MessageBox.Show(result.ErrorMessage,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
