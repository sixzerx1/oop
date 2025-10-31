using System;

namespace PaymentAdapterExample
{
    public interface IPaymentProcessor
    {
        void ProcessPayment(double amount);
    }

    public class PayPalPaymentProcessor : IPaymentProcessor
    {
        public void ProcessPayment(double amount)
        {
            Console.WriteLine($"✅ Оплата {amount} тг через PayPal успешно выполнена.");
        }
    }

    public class StripePaymentService
    {
        public void MakeTransaction(double totalAmount)
        {
            Console.WriteLine($"💳 Транзакция на {totalAmount} тг проведена через Stripe.");
        }
    }

    public class StripePaymentAdapter : IPaymentProcessor
    {
        private readonly StripePaymentService _stripeService;

        public StripePaymentAdapter(StripePaymentService stripeService)
        {
            _stripeService = stripeService;
        }

        public void ProcessPayment(double amount)
        {
            _stripeService.MakeTransaction(amount);
        }
    }

    public class QiwiService
    {
        public void Pay(double sum)
        {
            Console.WriteLine($"💰 Платёж {sum} тг успешно проведён через QIWI.");
        }
    }

    public class QiwiPaymentAdapter : IPaymentProcessor
    {
        private readonly QiwiService _qiwiService;

        public QiwiPaymentAdapter(QiwiService qiwiService)
        {
            _qiwiService = qiwiService;
        }

        public void ProcessPayment(double amount)
        {
            _qiwiService.Pay(amount);
        }
    }

    internal class Program2
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Система оплаты интернет-магазина ===\n");

            IPaymentProcessor paypal = new PayPalPaymentProcessor();
            IPaymentProcessor stripe = new StripePaymentAdapter(new StripePaymentService());
            IPaymentProcessor qiwi = new QiwiPaymentAdapter(new QiwiService());

            IPaymentProcessor[] processors = { paypal, stripe, qiwi };

            foreach (var processor in processors)
            {
                processor.ProcessPayment(1500);
            }

            Console.WriteLine("\n✅ Все транзакции успешно обработаны!");
            Console.ReadKey();
        }
    }
}
