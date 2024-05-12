using TesteBRQ.Console.ErrorHandling;
using TesteBRQ.Console.ExtensionMethods;

namespace TesteBRQ.Console.Model.Entities
{
    public sealed class Trade
    {
        private static readonly string[] ALLOWED_SECTORS = { "Private", "Public" };

        public Guid TradeID { get; set; }
        public double Value { get; set; }
        public string ClientSector { get; set; }
        public DateTime NextPaymentDate { get; set; }
        public Guid CategoryID { get; set; }
        public Category Category { get; private set; }

        private Trade(double value, string clientSector, DateTime nextPaymentDate)
        {
            TradeID = Guid.NewGuid();
            Value = value;
            ClientSector = clientSector.Capitalize();
            NextPaymentDate = nextPaymentDate;
        }

        private static Result<Trade> ValidateTrade
        (
            double value,
            string clientSector
        )
        {
            var result = new Result<Trade>();
            const string CONTEXT = "Trade.Validation";

            if (value < 0)
                result.ValidateFailure(Error.CreateError(CONTEXT,
                    "Trade value must be greater than zero"));

            if (!ALLOWED_SECTORS.Contains(clientSector, StringComparer.OrdinalIgnoreCase))
                result.ValidateFailure(Error.CreateError(CONTEXT,
                    "The informed Client Sector is not allowed."));

            return result;
        }


        public static Result<Trade> CreateTrade
        (
            double value,
            string clientSector,
            DateTime nextPaymentDate
        )
        {
            var result = ValidateTrade(value, clientSector);
            if (result.Failure)
                return result;

            return result.ValidateSuccess(new Trade(value, clientSector, nextPaymentDate));
        }

    }
}
