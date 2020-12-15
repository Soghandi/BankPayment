using System;
using System.Threading.Tasks;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Connector.Model;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Extension;
using Adin.BankPayment.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Adin.BankPayment.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PayInfoController : BaseController
    {
        private readonly IRepository<ApplicationBank> _applicationBankRepository;
        private readonly IRepository<Bank> _bankRepository;
        private readonly ILogger<PayInfoController> _logger;
        private readonly IRepository<Transaction> _transactionRepository;

        public PayInfoController(IMemoryCache memCaches,
            ILogger<PayInfoController> logger,
            IRepository<Transaction> transactionRepository,
            IRepository<Application> applicationRepository,
            IRepository<Bank> bankRepository,
            IRepository<ApplicationBank> applicationBankRepository) : base(memCaches, applicationRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _bankRepository = bankRepository;
            _applicationBankRepository = applicationBankRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RequestPay([FromBody]PayInfoModel model)
        {
            try
            {
                var application = await GetApplicationAsync();
                if (application == null || application.Status != 0) return Unauthorized();
                switch (model.PriceUnit)
                {
                    case PriceUnitEnum.Toman:
                        model.Amount *= 10;
                        break;
                    case PriceUnitEnum.Dollar:
                        throw new NotImplementedException("Dollar not supported yet");
                }

                var bank = await _bankRepository.GetFirstBy(x => x.Code == (byte)model.BankCode);
                var transaction = new Transaction
                {
                    Amount = model.Amount,
                    ApplicationId = application.Id,
                    BankId = bank.Id,
                    CreatedBy = 1,
                    Mobile = model.Mobile,
                    UserTrackCode = model.TrackCode,
                    CreationDate = DateTime.Now,
                    Status = (byte)TransactionStatusEnum.Initial,
                    CallbackUrl = model.CallbackUrl,
                    ExpirationTime = model.ExpirationTime
                };
                await _transactionRepository.Add(transaction);

                var currentBaseUrl = $"{Request.Scheme}://{Request.Host}";

                switch (model.BankCode)
                {
                    case BankCodeEnum.Parsian:
                        transaction.BankRedirectUrl = $"{currentBaseUrl}/Parsian/Callback?token={model.TrackCode}&SecondTrackCode={transaction.Id}";
                        break;

                    case BankCodeEnum.Mellat:
                        transaction.BankRedirectUrl = $"{currentBaseUrl}/Mellat/Callback?token={model.TrackCode}&SecondTrackCode={transaction.Id}";
                        break;

                    case BankCodeEnum.Efarda:
                        transaction.BankRedirectUrl = $"{currentBaseUrl}/Efarda/Callback?token={model.TrackCode}&SecondTrackCode={transaction.Id}";
                        break;

                    case BankCodeEnum.Pasargad:
                        transaction.BankRedirectUrl = $"{currentBaseUrl}/Pasargad/Callback?token={model.TrackCode}&SecondTrackCode={transaction.Id}";
                        break;

                    case BankCodeEnum.Saman:
                    default:
                        transaction.BankRedirectUrl = $"{currentBaseUrl}/Saman/Callback?token={model.TrackCode}&SecondTrackCode={transaction.Id}";
                        break;
                }

                await _transactionRepository.Update(transaction);
                return Ok(
                    new PayRequestResponseModel
                    {
                        Code = transaction.Id,
                        Url = currentBaseUrl + "/Pay/" + transaction.Id
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError(ex.InnerException.Message);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Verify(Guid id)
        {
            try
            {
                var application = await GetApplicationAsync();
                if (application == null || application.Status != 0) return Unauthorized();

                var transaction = await _transactionRepository.Get(id);
                if (transaction == null) return Unauthorized();

                IBankHelper bankHelper = transaction.Bank.Code switch
                {
                    (byte)BankCodeEnum.Mellat => new MellatHelper(_logger, _transactionRepository, _applicationBankRepository),
                    (byte)BankCodeEnum.Efarda => new EfardaHelper(_logger, _transactionRepository, _applicationBankRepository),
                    (byte)BankCodeEnum.Parsian => new ParsianHelper(_logger, _transactionRepository, _applicationBankRepository),
                    _ => new SamanHelper(_logger, _transactionRepository, _applicationBankRepository),
                };
                var Result = await bankHelper.VerifyTransaction(transaction);
                return Ok(Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError(ex.InnerException.Message);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CancelPayment(Guid id)
        {
            try
            {
                var application = await GetApplicationAsync();
                if (application == null || application.Status != 0) return Unauthorized();

                var transaction = await _transactionRepository.Get(id);
                if (transaction == null) return Unauthorized();

                if (transaction.Status == (byte)TransactionStatusEnum.BankOk ||
                    transaction.Status == (byte)TransactionStatusEnum.Success)
                    return BadRequest(new CancelPaymentResponseModel
                    {
                        ErrorCode = (byte)ErrorCodeEnum.OperationAlreadyDone,
                        Message = "پرداخت قبلا انجام شده است",
                        Status = false
                    });

                transaction.Status = (byte)TransactionStatusEnum.Cancel;
                transaction.ModifiedBy = 1;
                transaction.ModifiedOn = DateTime.Now;
                await _transactionRepository.Update(transaction);
                return Ok(new CancelPaymentResponseModel
                {
                    ErrorCode = (byte)ErrorCodeEnum.NoError,
                    Message = "پرداخت با موفقیت کنسل شد ",
                    Status = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError(ex.InnerException.Message);
                throw;
            }
        }
    }
}