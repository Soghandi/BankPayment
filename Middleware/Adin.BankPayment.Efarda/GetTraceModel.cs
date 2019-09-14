using System.Collections.Generic;

namespace Adin.BankPayment.Efarda
{
    public class EfardaGetTraceModel
    {
        public EfardaGetTraceModel()
        {
            serviceAmountList = new List<EfardaServiceamountlist>();
        }
        public string username { get; set; }
        public string password { get; set; }
        public string additionalData { get; set; }
        public string callBackUrl { get; set; }
        public string amount { get; set; }
        public string mobile { get; set; }
        public List<EfardaServiceamountlist> serviceAmountList { get; set; }
    }

    public class EfardaServiceamountlist
    {
        public string serviceId { get; set; }
        public string amount { get; set; }
    }

    public class EfardaGetTraceResultModel
    {
        public string traceNumber { get; set; }
        public int result { get; set; }
        public string description { get; set; }
        public string doTime { get; set; }
    }

    public class EfardaVerifyModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string traceNumber { get; set; }
    }

    public class EfardaVerifyResultModel
    {
        public int result { get; set; }
        public string description { get; set; }
        public string doTime { get; set; }
        public string traceNumber { get; set; }
        public string amount { get; set; }
        public string rrn { get; set; }
        public string type { get; set; }
        public List<EfardaServiceamountlist> serviceAmountList { get; set; }
    }
}