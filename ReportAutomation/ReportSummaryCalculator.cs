using System;
using System.Collections.Generic;

namespace ReportAutomation
{
    // Класс расчёта агрегированных итогов по предприятию.
    // Вычисляет суммарные и средние значения сводных показателей
    // на основе строк отчёта по подразделениям.
    public class ReportSummaryCalculator
    {
        // Суммарный объём передачи электрической энергии.
        public double TotalTransmitted { get; private set; }

        // Суммарный объём полезного отпуска электрической энергии.
        public double TotalUseful { get; private set; }

        // Суммарные потери электрической энергии.
        public double TotalLosses { get; private set; }

        // Доля потерь по предприятию в целом, процент.
        public double TotalLossesShare { get; private set; }

        // Средняя доля потерь по подразделениям, процент.
        public double AverageLossesShare { get; private set; }

        // Число подразделений, учтённых в расчёте.
        public int DepartmentCount { get; private set; }

        // Выполняет расчёт итоговых показателей по набору строк отчёта.
        public void Calculate(List<SummaryRow> rows)
        {
            TotalTransmitted = 0;
            TotalUseful = 0;
            TotalLosses = 0;
            double shareSum = 0;
            DepartmentCount = 0;

            foreach (SummaryRow row in rows)
            {
                TotalTransmitted += row.EnergyTransmitted;
                TotalUseful += row.EnergyUseful;
                TotalLosses += row.EnergyLosses;
                shareSum += row.LossesShare;
                DepartmentCount++;
            }

            // Доля потерь по предприятию в целом.
            if (TotalTransmitted > 0)
            {
                TotalLossesShare =
                    Math.Round(TotalLosses / TotalTransmitted * 100, 2);
            }
            else
            {
                TotalLossesShare = 0;
            }

            // Средняя доля потерь по подразделениям.
            if (DepartmentCount > 0)
            {
                AverageLossesShare =
                    Math.Round(shareSum / DepartmentCount, 2);
            }
            else
            {
                AverageLossesShare = 0;
            }
        }

        // Формирует итоговую строку отчёта по предприятию.
        public SummaryRow BuildTotalRow(string title)
        {
            return new SummaryRow
            {
                DepartmentName = title,
                EnergyTransmitted = TotalTransmitted,
                EnergyUseful = TotalUseful,
                EnergyLosses = TotalLosses,
                LossesShare = TotalLossesShare
            };
        }
    }
}
