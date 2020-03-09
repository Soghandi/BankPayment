using System.Runtime.Serialization;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using System.ComponentModel;

namespace Adin.BankPayment.Parsian.Connected_Services.SaleService
{
    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [DataContractAttribute(Name = "ClientPaymentRequestDataBase", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService")]
    [KnownTypeAttribute(typeof(ClientSaleRequestData))]
    public partial class ClientPaymentRequestDataBase : object
    {

        private string LoginAccountField;

        private long AmountField;

        private long OrderIdField;

        private string CallBackUrlField;

        private string AdditionalDataField;

        private string OriginatorField;

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

        [DataMemberAttribute(IsRequired = true, Order = 2)]
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

        [DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
        public string CallBackUrl
        {
            get
            {
                return this.CallBackUrlField;
            }
            set
            {
                this.CallBackUrlField = value;
            }
        }

        [DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
        public string AdditionalData
        {
            get
            {
                return this.AdditionalDataField;
            }
            set
            {
                this.AdditionalDataField = value;
            }
        }

        [DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
        public string Originator
        {
            get
            {
                return this.OriginatorField;
            }
            set
            {
                this.OriginatorField = value;
            }
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [DataContractAttribute(Name = "ClientSaleRequestData", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService")]
    public partial class ClientSaleRequestData : ClientPaymentRequestDataBase
    {
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [DataContractAttribute(Name = "ClientPaymentResponseDataBase", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService")]
    [KnownTypeAttribute(typeof(ClientSaleResponseData))]
    public partial class ClientPaymentResponseDataBase : object
    {

        private long TokenField;

        private string MessageField;

        private short StatusField;

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

        [DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
        public string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }

        [DataMemberAttribute(IsRequired = true, Order = 2)]
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
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [DataContractAttribute(Name = "ClientSaleResponseData", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService")]
    public partial class ClientSaleResponseData : ClientPaymentResponseDataBase
    {
    }

    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [ServiceContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService", ConfigurationName = "SaleServiceSoap" + "")]
    public interface SaleServiceSoap
    {

        [OperationContractAttribute(Action = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService/SalePaymentRequest", ReplyAction = "*")]
        Task<SalePaymentRequestResponse> SalePaymentRequestAsync(SalePaymentRequestRequest request);

        [OperationContractAttribute(Action = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService/SalePaymentWithId", ReplyAction = "*")]
        Task<SalePaymentWithIdResponse> SalePaymentWithIdAsync(SalePaymentWithIdRequest request);

        [OperationContractAttribute(Action = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService/UDSalePaymentRequest", ReplyAction = "*")]
        Task<UDSalePaymentRequestResponse> UDSalePaymentRequestAsync(UDSalePaymentRequestRequest request);
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [MessageContractAttribute(IsWrapped = false)]
    public partial class SalePaymentRequestRequest
    {

        [MessageBodyMemberAttribute(Name = "SalePaymentRequest", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService", Order = 0)]
        public SalePaymentRequestRequestBody Body;

        public SalePaymentRequestRequest()
        {
        }

        public SalePaymentRequestRequest(SalePaymentRequestRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [DataContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService")]
    public partial class SalePaymentRequestRequestBody
    {

        [DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ClientSaleRequestData requestData;

        public SalePaymentRequestRequestBody()
        {
        }

        public SalePaymentRequestRequestBody(ClientSaleRequestData requestData)
        {
            this.requestData = requestData;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [MessageContractAttribute(IsWrapped = false)]
    public partial class SalePaymentRequestResponse
    {

        [MessageBodyMemberAttribute(Name = "SalePaymentRequestResponse", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService", Order = 0)]
        public SalePaymentRequestResponseBody Body;

        public SalePaymentRequestResponse()
        {
        }

        public SalePaymentRequestResponse(SalePaymentRequestResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [DataContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService")]
    public partial class SalePaymentRequestResponseBody
    {

        [DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ClientSaleResponseData SalePaymentRequestResult;

        public SalePaymentRequestResponseBody()
        {
        }

        public SalePaymentRequestResponseBody(ClientSaleResponseData SalePaymentRequestResult)
        {
            this.SalePaymentRequestResult = SalePaymentRequestResult;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [MessageContractAttribute(IsWrapped = false)]
    public partial class SalePaymentWithIdRequest
    {

        [MessageBodyMemberAttribute(Name = "SalePaymentWithId", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService", Order = 0)]
        public SalePaymentWithIdRequestBody Body;

        public SalePaymentWithIdRequest()
        {
        }

        public SalePaymentWithIdRequest(SalePaymentWithIdRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [DataContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService")]
    public partial class SalePaymentWithIdRequestBody
    {

        [DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ClientSaleRequestData requestData;

        public SalePaymentWithIdRequestBody()
        {
        }

        public SalePaymentWithIdRequestBody(ClientSaleRequestData requestData)
        {
            this.requestData = requestData;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [MessageContractAttribute(IsWrapped = false)]
    public partial class SalePaymentWithIdResponse
    {

        [MessageBodyMemberAttribute(Name = "SalePaymentWithIdResponse", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService", Order = 0)]
        public SalePaymentWithIdResponseBody Body;

        public SalePaymentWithIdResponse()
        {
        }

        public SalePaymentWithIdResponse(SalePaymentWithIdResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [DataContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService")]
    public partial class SalePaymentWithIdResponseBody
    {

        [DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ClientSaleResponseData SalePaymentWithIdResult;

        public SalePaymentWithIdResponseBody()
        {
        }

        public SalePaymentWithIdResponseBody(ClientSaleResponseData SalePaymentWithIdResult)
        {
            this.SalePaymentWithIdResult = SalePaymentWithIdResult;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [MessageContractAttribute(IsWrapped = false)]
    public partial class UDSalePaymentRequestRequest
    {

        [MessageBodyMemberAttribute(Name = "UDSalePaymentRequest", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService", Order = 0)]
        public UDSalePaymentRequestRequestBody Body;

        public UDSalePaymentRequestRequest()
        {
        }

        public UDSalePaymentRequestRequest(UDSalePaymentRequestRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [DataContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService")]
    public partial class UDSalePaymentRequestRequestBody
    {

        [DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ClientSaleRequestData requestData;

        public UDSalePaymentRequestRequestBody()
        {
        }

        public UDSalePaymentRequestRequestBody(ClientSaleRequestData requestData)
        {
            this.requestData = requestData;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [MessageContractAttribute(IsWrapped = false)]
    public partial class UDSalePaymentRequestResponse
    {

        [MessageBodyMemberAttribute(Name = "UDSalePaymentRequestResponse", Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService", Order = 0)]
        public UDSalePaymentRequestResponseBody Body;

        public UDSalePaymentRequestResponse()
        {
        }

        public UDSalePaymentRequestResponse(UDSalePaymentRequestResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
    [DataContractAttribute(Namespace = "https://pec.Shaparak.ir/NewIPGServices/Sale/SaleService")]
    public partial class UDSalePaymentRequestResponseBody
    {

        [DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ClientPaymentResponseDataBase UDSalePaymentRequestResult;

        public UDSalePaymentRequestResponseBody()
        {
        }

        public UDSalePaymentRequestResponseBody(ClientPaymentResponseDataBase UDSalePaymentRequestResult)
        {
            this.UDSalePaymentRequestResult = UDSalePaymentRequestResult;
        }
    }

    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface SaleServiceSoapChannel : SaleServiceSoap, IClientChannel
    {
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class SaleServiceSoapClient : ClientBase<SaleServiceSoap>, SaleServiceSoap
    {

        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(ServiceEndpoint serviceEndpoint, ClientCredentials clientCredentials);

        public SaleServiceSoapClient(EndpointConfiguration endpointConfiguration) :
                base(SaleServiceSoapClient.GetBindingForEndpoint(endpointConfiguration), SaleServiceSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public SaleServiceSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) :
                base(SaleServiceSoapClient.GetBindingForEndpoint(endpointConfiguration), new EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public SaleServiceSoapClient(EndpointConfiguration endpointConfiguration, EndpointAddress remoteAddress) :
                base(SaleServiceSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public SaleServiceSoapClient(Binding binding, EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        Task<SalePaymentRequestResponse> SaleServiceSoap.SalePaymentRequestAsync(SalePaymentRequestRequest request)
        {
            return base.Channel.SalePaymentRequestAsync(request);
        }

        public Task<SalePaymentRequestResponse> SalePaymentRequestAsync(ClientSaleRequestData requestData)
        {
            SalePaymentRequestRequest inValue = new SalePaymentRequestRequest();
            inValue.Body = new SalePaymentRequestRequestBody();
            inValue.Body.requestData = requestData;
            return ((SaleServiceSoap)(this)).SalePaymentRequestAsync(inValue);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        Task<SalePaymentWithIdResponse> SaleServiceSoap.SalePaymentWithIdAsync(SalePaymentWithIdRequest request)
        {
            return base.Channel.SalePaymentWithIdAsync(request);
        }

        public Task<SalePaymentWithIdResponse> SalePaymentWithIdAsync(ClientSaleRequestData requestData)
        {
            SalePaymentWithIdRequest inValue = new SalePaymentWithIdRequest();
            inValue.Body = new SalePaymentWithIdRequestBody();
            inValue.Body.requestData = requestData;
            return ((SaleServiceSoap)(this)).SalePaymentWithIdAsync(inValue);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        Task<UDSalePaymentRequestResponse> SaleServiceSoap.UDSalePaymentRequestAsync(UDSalePaymentRequestRequest request)
        {
            return base.Channel.UDSalePaymentRequestAsync(request);
        }

        public Task<UDSalePaymentRequestResponse> UDSalePaymentRequestAsync(ClientSaleRequestData requestData)
        {
            UDSalePaymentRequestRequest inValue = new UDSalePaymentRequestRequest();
            inValue.Body = new UDSalePaymentRequestRequestBody();
            inValue.Body.requestData = requestData;
            return ((SaleServiceSoap)(this)).UDSalePaymentRequestAsync(inValue);
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
            if ((endpointConfiguration == EndpointConfiguration.SaleServiceSoap))
            {
                BasicHttpBinding result = new BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = BasicHttpSecurityMode.Transport;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.SaleServiceSoap12))
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
            if ((endpointConfiguration == EndpointConfiguration.SaleServiceSoap))
            {
                return new EndpointAddress("https://pec.shaparak.ir/NewIPGServices/Sale/SaleService.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.SaleServiceSoap12))
            {
                return new EndpointAddress("https://pec.shaparak.ir/NewIPGServices/Sale/SaleService.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        public enum EndpointConfiguration
        {

            SaleServiceSoap,

            SaleServiceSoap12,
        }
    }
}