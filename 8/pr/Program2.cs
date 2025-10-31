using System;
using System.Collections.Generic;
using System.Globalization;

namespace LogisticsAdapterExample
{
    public interface IInternalDeliveryService
    {
        void DeliverOrder(string orderId);
        string GetDeliveryStatus(string orderId);
        decimal CalculateDeliveryCost(string orderId);
    }

    public static class Logger
    {
        public static void Info(string msg) => Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {msg}");
        public static void Error(string msg) => Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {msg}");
    }

    public class InternalDeliveryService : IInternalDeliveryService
    {
        private readonly Dictionary<string, string> _status = new();
        private readonly Dictionary<string, decimal> _cost = new();

        public void DeliverOrder(string orderId)
        {
            try
            {
                Logger.Info($"Internal: starting delivery for {orderId}");
                _status[orderId] = "Shipped (Internal)";
                _cost[orderId] = ComputeCost(orderId);
                Logger.Info($"Internal: order {orderId} shipped. Cost: {_cost[orderId].ToString("F2", CultureInfo.InvariantCulture)}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Internal: failed to deliver {orderId}. {ex.Message}");
                throw;
            }
        }

        public string GetDeliveryStatus(string orderId)
        {
            return _status.ContainsKey(orderId) ? _status[orderId] : "Unknown";
        }

        public decimal CalculateDeliveryCost(string orderId)
        {
            return _cost.ContainsKey(orderId) ? _cost[orderId] : ComputeCost(orderId);
        }

        private decimal ComputeCost(string orderId)
        {
            var hash = Math.Abs(orderId.GetHashCode());
            return 50 + (hash % 500);
        }
    }

    public class ExternalLogisticsServiceA
    {
        public int ShipItem(int itemId)
        {
            // simulate
            return itemId * 1000 + DateTime.UtcNow.Millisecond;
        }

        public string TrackShipment(int shipmentId)
        {
            return (shipmentId % 2 == 0) ? "In Transit (A)" : "Delivered (A)";
        }

        public decimal EstimateCostForItem(int itemId)
        {
            return 80 + (itemId % 200);
        }
    }

    public class ExternalLogisticsServiceB
    {
        public string SendPackage(string packageInfo)
        {
            return "B-" + Math.Abs(packageInfo.GetHashCode()).ToString();
        }

        public string CheckPackageStatus(string trackingCode)
        {
            return trackingCode.Length % 2 == 0 ? "In Transit (B)" : "Delivered (B)";
        }

        public decimal CalculatePrice(string packageInfo)
        {
            return 60 + (packageInfo.Length % 300);
        }
    }

    public class ExternalLogisticsServiceC
    {
        public (string shipmentReference, double expectedCost) CreateShipment(object details)
        {
            var refId = "CRef-" + Math.Abs(details.GetHashCode());
            var cost = 100 + (Math.Abs(details.GetHashCode()) % 400);
            return (refId, cost);
        }

        public string QueryStatus(string shipmentReference)
        {
            return shipmentReference.Contains("7") ? "Delayed (C)" : "In Transit (C)";
        }
    }

    public class LogisticsAdapterA : IInternalDeliveryService
    {
        private readonly ExternalLogisticsServiceA _serviceA;
        private readonly Dictionary<string, int> _mapOrderToShipment = new();
        private readonly Dictionary<string, decimal> _costCache = new();

        public LogisticsAdapterA(ExternalLogisticsServiceA serviceA)
        {
            _serviceA = serviceA;
        }

        public void DeliverOrder(string orderId)
        {
            try
            {
                Logger.Info($"AdapterA: delivering {orderId} via ExternalLogisticsServiceA");
                int itemId = MapOrderIdToItemId(orderId);
                int shipmentId = _serviceA.ShipItem(itemId);
                _mapOrderToShipment[orderId] = shipmentId;
                _costCache[orderId] = _serviceA.EstimateCostForItem(itemId);
                Logger.Info($"AdapterA: order {orderId} shipped as shipment {shipmentId}, estimated cost {_costCache[orderId]:F2}");
            }
            catch (Exception ex)
            {
                Logger.Error($"AdapterA: error delivering {orderId} - {ex.Message}");
                throw;
            }
        }

        public string GetDeliveryStatus(string orderId)
        {
            try
            {
                if (!_mapOrderToShipment.ContainsKey(orderId))
                    return "Unknown (A)";

                int shipmentId = _mapOrderToShipment[orderId];
                return _serviceA.TrackShipment(shipmentId);
            }
            catch (Exception ex)
            {
                Logger.Error($"AdapterA: error getting status for {orderId} - {ex.Message}");
                return "Error";
            }
        }

        public decimal CalculateDeliveryCost(string orderId)
        {
            if (_costCache.ContainsKey(orderId))
                return _costCache[orderId];

            int itemId = MapOrderIdToItemId(orderId);
            var c = _serviceA.EstimateCostForItem(itemId);
            _costCache[orderId] = c;
            return c;
        }

        private int MapOrderIdToItemId(string orderId)
        {
            if (int.TryParse(orderId, out var v)) return Math.Abs(v);
            return Math.Abs(orderId.GetHashCode()) % 10000;
        }
    }

    public class LogisticsAdapterB : IInternalDeliveryService
    {
        private readonly ExternalLogisticsServiceB _serviceB;
        private readonly Dictionary<string, string> _mapOrderToTracking = new();
        private readonly Dictionary<string, decimal> _costCache = new();

        public LogisticsAdapterB(ExternalLogisticsServiceB serviceB)
        {
            _serviceB = serviceB;
        }

        public void DeliverOrder(string orderId)
        {
            try
            {
                Logger.Info($"AdapterB: delivering {orderId} via ExternalLogisticsServiceB");
                string packageInfo = BuildPackageInfo(orderId);
                string tracking = _serviceB.SendPackage(packageInfo);
                _mapOrderToTracking[orderId] = tracking;
                _costCache[orderId] = _serviceB.CalculatePrice(packageInfo);
                Logger.Info($"AdapterB: order {orderId} sent with tracking {tracking}, cost {_costCache[orderId]:F2}");
            }
            catch (Exception ex)
            {
                Logger.Error($"AdapterB: error delivering {orderId} - {ex.Message}");
                throw;
            }
        }

        public string GetDeliveryStatus(string orderId)
        {
            try
            {
                if (!_mapOrderToTracking.ContainsKey(orderId))
                    return "Unknown (B)";

                string tracking = _mapOrderToTracking[orderId];
                return _serviceB.CheckPackageStatus(tracking);
            }
            catch (Exception ex)
            {
                Logger.Error($"AdapterB: error getting status for {orderId} - {ex.Message}");
                return "Error";
            }
        }

        public decimal CalculateDeliveryCost(string orderId)
        {
            if (_costCache.ContainsKey(orderId)) return _costCache[orderId];
            var packageInfo = BuildPackageInfo(orderId);
            var c = _serviceB.CalculatePrice(packageInfo);
            _costCache[orderId] = c;
            return c;
        }

        private string BuildPackageInfo(string orderId)
        {
            return $"order:{orderId};len:{orderId.Length}";
        }
    }

    public class LogisticsAdapterC : IInternalDeliveryService
    {
        private readonly ExternalLogisticsServiceC _serviceC;
        private readonly Dictionary<string, string> _mapOrderToRef = new();
        private readonly Dictionary<string, decimal> _costCache = new();

        public LogisticsAdapterC(ExternalLogisticsServiceC serviceC)
        {
            _serviceC = serviceC;
        }

        public void DeliverOrder(string orderId)
        {
            try
            {
                Logger.Info($"AdapterC: delivering {orderId} via ExternalLogisticsServiceC");
                var details = new { orderId, timestamp = DateTime.UtcNow.Ticks };
                var (reference, expectedCost) = _serviceC.CreateShipment(details);
                _mapOrderToRef[orderId] = reference;
                _costCache[orderId] = Convert.ToDecimal(expectedCost);
                Logger.Info($"AdapterC: order {orderId} sent, reference {reference}, expected cost {_costCache[orderId]:F2}");
            }
            catch (Exception ex)
            {
                Logger.Error($"AdapterC: error delivering {orderId} - {ex.Message}");
                throw;
            }
        }

        public string GetDeliveryStatus(string orderId)
        {
            try
            {
                if (!_mapOrderToRef.ContainsKey(orderId))
                    return "Unknown (C)";

                var reference = _mapOrderToRef[orderId];
                return _serviceC.QueryStatus(reference);
            }
            catch (Exception ex)
            {
                Logger.Error($"AdapterC: error getting status for {orderId} - {ex.Message}");
                return "Error";
            }
        }

        public decimal CalculateDeliveryCost(string orderId)
        {
            if (_costCache.ContainsKey(orderId)) return _costCache[orderId];
            var details = new { orderId, timestamp = DateTime.UtcNow.Ticks };
            var (_, expectedCost) = _serviceC.CreateShipment(details);
            _costCache[orderId] = Convert.ToDecimal(expectedCost);
            return _costCache[orderId];
        }
    }

    public enum DeliveryProvider
    {
        Internal,
        ExternalA,
        ExternalB,
        ExternalC
    }

    public static class DeliveryServiceFactory
    {
        public static IInternalDeliveryService GetService(DeliveryProvider provider)
        {
            return provider switch
            {
                DeliveryProvider.Internal => new InternalDeliveryService(),
                DeliveryProvider.ExternalA => new LogisticsAdapterA(new ExternalLogisticsServiceA()),
                DeliveryProvider.ExternalB => new LogisticsAdapterB(new ExternalLogisticsServiceB()),
                DeliveryProvider.ExternalC => new LogisticsAdapterC(new ExternalLogisticsServiceC()),
                _ => throw new ArgumentException("Unknown provider")
            };
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var orders = new[] { "1001", "ORD-200X", "3003", "special-77" };

            var internalSvc = DeliveryServiceFactory.GetService(DeliveryProvider.Internal);
            ProcessOrders(internalSvc, orders);

            var aSvc = DeliveryServiceFactory.GetService(DeliveryProvider.ExternalA);
            ProcessOrders(aSvc, orders);

            var bSvc = DeliveryServiceFactory.GetService(DeliveryProvider.ExternalB);
            ProcessOrders(bSvc, orders);

            var cSvc = DeliveryServiceFactory.GetService(DeliveryProvider.ExternalC);
            ProcessOrders(cSvc, orders);

            Console.WriteLine("All demos finished.");
        }

        static void ProcessOrders(IInternalDeliveryService svc, IEnumerable<string> orders)
        {
            foreach (var order in orders)
            {
                try
                {
                    Logger.Info($"Client: requesting delivery for {order} via {svc.GetType().Name}");
                    svc.DeliverOrder(order);
                    var status = svc.GetDeliveryStatus(order);
                    var cost = svc.CalculateDeliveryCost(order);
                    Logger.Info($"Client: order {order} status='{status}', estimated cost={cost:F2}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Client: failed to process {order} via {svc.GetType().Name} - {ex.Message}");
                }
            }
            Console.WriteLine();
        }
    }
}
