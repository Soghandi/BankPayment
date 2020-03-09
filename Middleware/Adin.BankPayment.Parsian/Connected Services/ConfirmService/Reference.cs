using System.Runtime.Serialization;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using System.ComponentModel;

namespace Adin.BankPayment.Parsian.Connected_Services.ConfirmService
{
    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [DataContractAttribute(Name = "ClientConfirmRequestData", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService")]
    [KnownTypeAttribute(typeof(ClientConfirmWithAmountRequestData))]
    public partial class ClientConfirmRequestData : object
    {

        private string LoginAccountField;

        private long TokenField;

        [DataMemberAttribute(EmitDefaultValue = false)]
        public string LoginAccount
        {
            get
            {
                return this.LoginAccountField;
            }
            set
            {
                this.LoginAccountField = value;
            }
        }

        [DataMemberAttribute(IsRequired = true)]
        public long Token
        {
            get
            {
                return this.TokenField;
            }
            set
            {
                this.TokenField = value;
            }
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [DataContractAttribute(Name = "ClientConfirmWithAmountRequestData", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService")]
    public partial class ClientConfirmWithAmountRequestData : ClientConfirmRequestData
    {

        private long OrderIdField;

        private long AmountField;

        [DataMemberAttribute(IsRequired = true)]
        public long OrderId
        {
            get
            {
                return this.OrderIdField;
            }
            set
            {
                this.OrderIdField = value;
            }
        }

        [DataMemberAttribute(IsRequired = true, Order = 1)]
        public long Amount
        {
            get
            {
                return this.AmountField;
            }
            set
            {
                this.AmountField = value;
            }
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [DataContractAttribute(Name = "ClientConfirmResponseData", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService")]
    public partial class ClientConfirmResponseData : object
    {

        private short StatusField;

        private string CardNumberMaskedField;

        private long RRNField;

        private long TokenField;

        [DataMemberAttribute(IsRequired = true)]
        public short Status
        {
            get
            {
                return this.StatusField;
            }
            set
            {
                this.StatusField = value;
            }
        }

        [DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
        public string CardNumberMasked
        {
            get
            {
                return this.CardNumberMaskedField;
            }
            set
            {
                this.CardNumberMaskedField = value;
            }
        }

        [DataMemberAttribute(IsRequired = true, Order = 2)]
        public long RRN
        {
            get
            {
                return this.RRNField;
            }
            set
            {
                this.RRNField = value;
            }
        }

        [DataMemberAttribute(IsRequired = true, Order = 3)]
        public long Token
        {
            get
            {
                return this.TokenField;
            }
            set
            {
                this.TokenField = value;
            }
        }
    }

    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [ServiceContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService", ConfigurationName = "ConfirmServiceS" +
        "oap")]
    public interface ConfirmServiceSoap
    {

        [OperationContractAttribute(Action = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService/ConfirmPayment", ReplyAction = "*")]
        Task<ConfirmPaymentResponse> ConfirmPaymentAsync(ConfirmPaymentRequest request);

        [OperationContractAttribute(Action = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService/ConfirmPaymentWithA" +
            "mount", ReplyAction = "*")]
        Task<ConfirmPaymentWithAmountResponse> ConfirmPaymentWithAmountAsync(ConfirmPaymentWithAmountRequest request);
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [MessageContractAttribute(IsWrapped = false)]
    public partial class ConfirmPaymentRequest
    {

        [MessageBodyMemberAttribute(Name = "ConfirmPayment", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService", Order = 0)]
        public ConfirmPaymentRequestBody Body;

        public ConfirmPaymentRequest()
        {
        }

        public ConfirmPaymentRequest(ConfirmPaymentRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [DataContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService")]
    public partial class ConfirmPaymentRequestBody
    {

        [DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ClientConfirmRequestData requestData;

        public ConfirmPaymentRequestBody()
        {
        }

        public ConfirmPaymentRequestBody(ClientConfirmRequestData requestData)
        {
            this.requestData = requestData;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [MessageContractAttribute(IsWrapped = false)]
    public partial class ConfirmPaymentResponse
    {

        [MessageBodyMemberAttribute(Name = "ConfirmPaymentResponse", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService", Order = 0)]
        public ConfirmPaymentResponseBody Body;

        public ConfirmPaymentResponse()
        {
        }

        public ConfirmPaymentResponse(ConfirmPaymentResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [DataContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService")]
    public partial class ConfirmPaymentResponseBody
    {

        [DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ClientConfirmResponseData ConfirmPaymentResult;

        public ConfirmPaymentResponseBody()
        {
        }

        public ConfirmPaymentResponseBody(ClientConfirmResponseData ConfirmPaymentResult)
        {
            this.ConfirmPaymentResult = ConfirmPaymentResult;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [MessageContractAttribute(IsWrapped = false)]
    public partial class ConfirmPaymentWithAmountRequest
    {

        [MessageBodyMemberAttribute(Name = "ConfirmPaymentWithAmount", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService", Order = 0)]
        public ConfirmPaymentWithAmountRequestBody Body;

        public ConfirmPaymentWithAmountRequest()
        {
        }

        public ConfirmPaymentWithAmountRequest(ConfirmPaymentWithAmountRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [DataContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService")]
    public partial class ConfirmPaymentWithAmountRequestBody
    {

        [DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ClientConfirmWithAmountRequestData requestData;

        public ConfirmPaymentWithAmountRequestBody()
        {
        }

        public ConfirmPaymentWithAmountRequestBody(ClientConfirmWithAmountRequestData requestData)
        {
            this.requestData = requestData;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [MessageContractAttribute(IsWrapped = false)]
    public partial class ConfirmPaymentWithAmountResponse
    {

        [MessageBodyMemberAttribute(Name = "ConfirmPaymentWithAmountResponse", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService", Order = 0)]
        public ConfirmPaymentWithAmountResponseBody Body;

        public ConfirmPaymentWithAmountResponse()
        {
        }

        public ConfirmPaymentWithAmountResponse(ConfirmPaymentWithAmountResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [DataContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Confirm/ConfirmService")]
    public partial class ConfirmPaymentWithAmountResponseBody
    {

        [DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ClientConfirmResponseData ConfirmPaymentWithAmountResult;

        public ConfirmPaymentWithAmountResponseBody()
        {
        }

        public ConfirmPaymentWithAmountResponseBody(ClientConfirmResponseData ConfirmPaymentWithAmountResult)
        {
            this.ConfirmPaymentWithAmountResult = ConfirmPaymentWithAmountResult;
        }
    }

    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface ConfirmServiceSoapChannel : ConfirmServiceSoap, IClientChannel
    {
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class ConfirmServiceSoapClient : ClientBase<ConfirmServiceSoap>, ConfirmServiceSoap
    {

        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(ServiceEndpoint serviceEndpoint, ClientCredentials clientCredentials);

        public ConfirmServiceSoapClient(EndpointConfiguration endpointConfiguration) :
                base(ConfirmServiceSoapClient.GetBindingForEndpoint(endpointConfiguration), ConfirmServiceSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public ConfirmServiceSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) :
                base(ConfirmServiceSoapClient.GetBindingForEndpoint(endpointConfiguration), new EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public ConfirmServiceSoapClient(EndpointConfiguration endpointConfiguration, EndpointAddress remoteAddress) :
                base(ConfirmServiceSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public ConfirmServiceSoapClient(Binding binding, EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        Task<ConfirmPaymentResponse> ConfirmServiceSoap.ConfirmPaymentAsync(ConfirmPaymentRequest request)
        {
            return base.Channel.ConfirmPaymentAsync(request);
        }

        public Task<ConfirmPaymentResponse> ConfirmPaymentAsync(ClientConfirmRequestData requestData)
        {
            ConfirmPaymentRequest inValue = new ConfirmPaymentRequest();
            inValue.Body = new ConfirmPaymentRequestBody();
            inValue.Body.requestData = requestData;
            return ((ConfirmServiceSoap)(this)).ConfirmPaymentAsync(inValue);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        Task<ConfirmPaymentWithAmountResponse> ConfirmServiceSoap.ConfirmPaymentWithAmountAsync(ConfirmPaymentWithAmountRequest request)
        {
            return base.Channel.ConfirmPaymentWithAmountAsync(request);
        }

        public Task<ConfirmPaymentWithAmountResponse> ConfirmPaymentWithAmountAsync(ClientConfirmWithAmountRequestData requestData)
        {
            ConfirmPaymentWithAmountRequest inValue = new ConfirmPaymentWithAmountRequest();
            inValue.Body = new ConfirmPaymentWithAmountRequestBody();
            inValue.Body.requestData = requestData;
            return ((ConfirmServiceSoap)(this)).ConfirmPaymentWithAmountAsync(inValue);
        }

        public virtual Task OpenAsync()
        {
            return Task.Factory.FromAsync(((ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((ICommunicationObject)(this)).EndOpen));
        }

        public virtual Task CloseAsync()
        {
            return Task.Factory.FromAsync(((ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((ICommunicationObject)(this)).EndClose));
        }

        private static Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.ConfirmServiceSoap))
            {
                BasicHttpBinding result = new BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = BasicHttpSecurityMode.Transport;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.ConfirmServiceSoap12))
            {
                CustomBinding result = new CustomBinding();
                TextMessageEncodingBindingElement textBindingElement = new TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                HttpsTransportBindingElement httpsBindingElement = new HttpsTransportBindingElement();
                httpsBindingElement.AllowCookies = true;
                httpsBindingElement.MaxBufferSize = int.MaxValue;
                httpsBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpsBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        private static EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.ConfirmServiceSoap))
            {
                return new EndpointAddress("https://pec.shaparak.ir/NewIPGServices/Confirm/ConfirmService.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.ConfirmServiceSoap12))
            {
                return new EndpointAddress("https://pec.shaparak.ir/NewIPGServices/Confirm/ConfirmService.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        public enum EndpointConfiguration
        {

            ConfirmServiceSoap,

            ConfirmServiceSoap12,
        }
    }
}