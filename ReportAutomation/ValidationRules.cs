namespace ReportAutomation
{
    // Правило проверки наличия кода подразделения.
    public class DepartmentCodeRule : IValidationRule
    {
        public string Name
        {
            get { return "Наличие кода подразделения"; }
        }

        public ValidationResult Check(Indicator indicator, AppConfig config)
        {
            if (indicator.DepartmentCode.Length == 0)
            {
                return ValidationResult.Fail(
                    "Не указан код подразделения");
            }
            return ValidationResult.Ok();
        }
    }

    // Правило проверки неотрицательности значений показателей.
    public class NonNegativeRule : IValidationRule
    {
        public string Name
        {
            get { return "Неотрицательность значений"; }
        }

        public ValidationResult Check(Indicator indicator, AppConfig config)
        {
            if (indicator.EnergyTransmitted < 0 ||
                indicator.EnergyUseful < 0 ||
                indicator.EnergyLosses < 0)
            {
                return ValidationResult.Fail(
                    "Значения показателей не могут быть отрицательными");
            }
            return ValidationResult.Ok();
        }
    }

    // Правило проверки минимального объёма передачи.
    public class MinTransmittedRule : IValidationRule
    {
        public string Name
        {
            get { return "Минимальный объём передачи"; }
        }

        public ValidationResult Check(Indicator indicator, AppConfig config)
        {
            if (indicator.EnergyTransmitted < config.MinEnergyTransmitted)
            {
                return ValidationResult.Fail(
                    "Объём передачи ниже установленного значения");
            }
            return ValidationResult.Ok();
        }
    }

    // Правило проверки соотношения отпуска и передачи.
    public class UsefulNotExceedsRule : IValidationRule
    {
        public string Name
        {
            get { return "Соотношение отпуска и передачи"; }
        }

        public ValidationResult Check(Indicator indicator, AppConfig config)
        {
            if (indicator.EnergyUseful > indicator.EnergyTransmitted)
            {
                return ValidationResult.Fail(
                    "Полезный отпуск превышает объём передачи");
            }
            return ValidationResult.Ok();
        }
    }

    // Правило проверки баланса передачи, отпуска и потерь.
    public class BalanceRule : IValidationRule
    {
        public string Name
        {
            get { return "Баланс показателей"; }
        }

        public ValidationResult Check(Indicator indicator, AppConfig config)
        {
            // Сумма полезного отпуска и потерь не превышает
            // объём передачи с учётом допустимого расхождения.
            double sum = indicator.EnergyUseful + indicator.EnergyLosses;
            double allowed = indicator.EnergyTransmitted * 1.01;
            if (sum > allowed)
            {
                return ValidationResult.Fail(
                    "Сумма отпуска и потерь превышает объём передачи");
            }
            return ValidationResult.Ok();
        }
    }
}
