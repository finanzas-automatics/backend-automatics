using AutomaticsApi.Models.DTOs;

namespace AutomaticsApi.Services;

public interface ISimulatorService
{
    SimulationResponse Calculate(SimulationRequest request);
}

public class SimulatorService : ISimulatorService
{
    private const decimal Insurance = 50m;

    public SimulationResponse Calculate(SimulationRequest req)
    {
        var tem = ComputeTem(req);
        var loanAmount = req.VehiclePrice * (1 - (decimal)(req.InitialPaymentPct / 100));
        var initialPayment = req.VehiclePrice * (decimal)(req.InitialPaymentPct / 100);
        var balloonPayment = req.VehiclePrice * (decimal)(req.FinalPaymentPct / 100);

        // Ajuste por gracia total: el saldo crece si hay gracia total
        decimal effectivePv = loanAmount;
        if (req.GracePeriodType == "total" && req.GraceMonths > 0)
            effectivePv = loanAmount * (decimal)Math.Pow(1 + tem, req.GraceMonths);

        // Valor presente de la cuota final (balloon)
        var effectiveN = req.TermMonths - (req.GracePeriodType == "sin" ? 0 : req.GraceMonths);
        decimal pvBalloon = balloonPayment / (decimal)Math.Pow(1 + tem, effectiveN);
        decimal financeBase = effectivePv - pvBalloon;

        decimal monthly = tem == 0
            ? financeBase / effectiveN
            : financeBase * (decimal)tem / (1 - (decimal)Math.Pow(1 + tem, -effectiveN));

        // Generar cronograma completo
        var schedule = BuildSchedule(
            loanAmount, effectivePv, tem, monthly, balloonPayment,
            req.TermMonths, req.GracePeriodType, req.GraceMonths);

        decimal totalInterest = schedule.Sum(r => r.Interest);
        decimal totalPayment = schedule.Sum(r => r.TotalPayment) + balloonPayment;

        // TIR mensual (Newton-Raphson)
        double tirMonthly = ComputeTir(
            (double)loanAmount,
            schedule.Select(r => (double)r.TotalPayment).ToList(),
            (double)balloonPayment,
            req.TermMonths);

        double tcea = Math.Pow(1 + tirMonthly, 12) - 1;

        // VAN desde perspectiva del deudor
        double cokMonthly = Math.Pow(1 + req.Cok / 100, 1.0 / 12) - 1;
        double pvPayments = 0;
        for (int t = 1; t <= schedule.Count; t++)
            pvPayments += (double)schedule[t - 1].TotalPayment / Math.Pow(1 + cokMonthly, t);
        pvPayments += (double)balloonPayment / Math.Pow(1 + cokMonthly, req.TermMonths);
        decimal van = -(loanAmount) + (decimal)pvPayments;

        return new SimulationResponse(
            loanAmount,
            initialPayment,
            balloonPayment,
            monthly + Insurance,
            tem * 100,
            tirMonthly * 100,
            tcea * 100,
            van,
            totalInterest,
            totalPayment,
            schedule
        );
    }

    private static List<ScheduleRowResponse> BuildSchedule(
        decimal originalPv, decimal effectivePv,
        double tem, decimal monthly, decimal balloon,
        int termMonths, string graceType, int graceMonths)
    {
        var rows = new List<ScheduleRowResponse>();
        decimal balance = originalPv;

        for (int t = 1; t <= termMonths; t++)
        {
            decimal interest = balance * (decimal)tem;
            decimal amort;
            string graceLabel = "sin_gracia";
            decimal totalPayment;

            if (graceType == "parcial" && t <= graceMonths)
            {
                amort = 0;
                graceLabel = "gracia_parcial";
                totalPayment = interest + Insurance;
            }
            else if (graceType == "total" && t <= graceMonths)
            {
                amort = 0;
                interest = 0;
                graceLabel = "gracia_total";
                totalPayment = Insurance;
                balance = balance * (decimal)(1 + tem);
                rows.Add(new ScheduleRowResponse(t, balance / (decimal)(1 + tem), 0, 0, Insurance, Insurance, balance, graceLabel));
                continue;
            }
            else
            {
                amort = monthly - interest;
                totalPayment = monthly + Insurance;
            }

            var finalBalance = balance - amort;
            if (finalBalance < 0.01m) finalBalance = 0;

            rows.Add(new ScheduleRowResponse(
                t,
                Math.Round(balance, 2),
                Math.Round(interest, 2),
                Math.Round(amort, 2),
                Insurance,
                Math.Round(totalPayment, 2),
                Math.Round(finalBalance, 2),
                graceLabel
            ));

            balance = finalBalance;
        }

        return rows;
    }

    private static double ComputeTem(SimulationRequest req)
    {
        if (req.RateType == "TEA")
            return Math.Pow(1 + req.RateValue / 100, 1.0 / 12) - 1;

        // TNA
        int m = req.Capitalization switch
        {
            var c when c != null && c.Contains("Trimestral") => 4,
            var c when c != null && c.Contains("Semestral") => 2,
            _ => 12
        };
        return Math.Pow(1 + (req.RateValue / 100) / m, (double)m / 12) - 1;
    }

    private static double ComputeTir(
        double pv, List<double> payments, double balloon, int n,
        double initialGuess = 0.01, int maxIter = 1000, double tol = 1e-8)
    {
        double r = initialGuess;
        for (int i = 0; i < maxIter; i++)
        {
            double f = -pv;
            double df = 0;
            for (int t = 1; t <= payments.Count; t++)
            {
                f += payments[t - 1] / Math.Pow(1 + r, t);
                df -= t * payments[t - 1] / Math.Pow(1 + r, t + 1);
            }
            f += balloon / Math.Pow(1 + r, n);
            df -= n * balloon / Math.Pow(1 + r, n + 1);

            if (Math.Abs(df) < tol) break;
            double rNew = r - f / df;
            if (Math.Abs(rNew - r) < tol) return rNew;
            r = rNew;
        }
        return r;
    }
}
