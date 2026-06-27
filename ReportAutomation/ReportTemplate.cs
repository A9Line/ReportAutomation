using System;

namespace ReportAutomation
{
    // Шаблон отчётного документа.
    // Содержит постоянные элементы оформления отчёта:
    // наименование предприятия, заголовки и подписи столбцов.
    // Вынесение шаблона в отдельный класс обеспечивает изменение
    // форм отчётности без правки кода формирования документов.
    public class ReportTemplate
    {
        // Наименование предприятия в заголовке отчёта.
        public string EnterpriseName { get; set; } =
            "Акционерное общество «Сетевая компания»";

        // Наименование сводного отчётного документа.
        public string SummaryTitle { get; set; } =
            "Сводный отчёт по передаче электрической энергии";

        // Наименование пояснительного документа.
        public string TextTitle { get; set; } =
            "Пояснительная часть отчёта";

        // Подписи столбцов сводной таблицы.
        public string[] ColumnHeaders { get; set; } = new string[]
        {
            "Подразделение",
            "Передача, тыс. кВт*ч",
            "Полезный отпуск, тыс. кВт*ч",
            "Потери, тыс. кВт*ч",
            "Доля потерь, %"
        };

        // Подпись итоговой строки отчёта.
        public string TotalRowTitle { get; set; } = "Итого по предприятию";

        // Формирует строку заголовка отчёта с отчётным периодом.
        public string BuildHeader(DateTime periodStart, DateTime periodEnd)
        {
            return EnterpriseName + ". " + SummaryTitle +
                " за период с " + periodStart.ToString("dd.MM.yyyy") +
                " по " + periodEnd.ToString("dd.MM.yyyy");
        }

        // Возвращает подпись столбца по его номеру.
        // При выходе за пределы набора возвращает пустую строку.
        public string GetColumnHeader(int index)
        {
            if (index >= 0 && index < ColumnHeaders.Length)
            {
                return ColumnHeaders[index];
            }
            return string.Empty;
        }

        // Возвращает число столбцов сводной таблицы.
        public int ColumnCount
        {
            get { return ColumnHeaders.Length; }
        }
    }
}
