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
        private readonly IRepository<Application> _applicationRepository;
        private readonly IRepository<Bank> _bankRepository;
        private readonly ILogger<PayController> _logger;
        private readonly IRepository<Transaction> _transactionRepository;

        public PayInfoController(IMemoryCache memCaches,
            ILogger<PayController> logger,
            IRepository<Transaction> transactionRepository,
            IRepository<Application> applicationRepository,
            IRepository<Bank> bankRepository,
            IRepository<ApplicationBank> applicationBankRepository) : base(memCaches, logger, applicationRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _applicationRepository = applicationRepository;
            _bankRepository = bankRepository;
            _applicationBankRepository = applicationBankRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RequestPay([FromBody] PayInfoModel model)
        {
            try
            {
                var application = await GetApplicationAsync();
                if (application == null || application.Status != 0) return Unauthorized();
                if (model.PriceUnit == PriceUnitEnum.Toman) model.Amount = model.Amount * 10;
                if (model.PriceUnit == PriceUnitEnum.Dollar)
                    throw new NotImplementedException("Dollar not supported yet");

                //Todo: Find Best Bank For User
                var bank = await _bankRepository.GetFirstBy(x => x.Code == (byte) model.BankCode);
                var transaction = new Transaction
                {
                    Amount = model.Amount,
                    ApplicationId = application.Id,
                    BankId = bank.Id,
                    CreatedBy = 1,
                    Mobile = model.Mobile,
                    UserTrackCode = model.TrackCode,
                    CreationDate = DateTime.Now,
                    Status = (byte) TransactionStatusEnum.Initial,
                    CallbackUrl = model.CallbackUrl,
                    ExpirationTime = model.ExpirationTime
                };
                await _transactionRepository.Add(transaction);

                var applicationBank =
                    await _applicationBankRepository.GetFirstBy(x =>
                        x.ApplicationId == application.Id && x.BankId == bank.Id);

                var currentBaseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);

                switch (model.BankCode)
                {
                    case BankCodeEnum.Parsian:
                        var parsianRedirectUrl = string.Format("{0}/Parsian/Callback?token={1}&SecondTrackCode={2}",
                            currentBaseUrl, model.TrackCode, transaction.Id);
                        transaction.BankRedirectUrl = parsianRedirectUrl;
                        await _transactionRepository.Update(transaction);
                        return Ok(
                            new PayRequestResponseModel
                            {
                                Code = transaction.Id,
                                Url = currentBaseUrl + "/Pay/" + transaction.Id
                            });

                    case BankCodeEnum.Mellat:
                        var mellatRedirectUrl = string.Format("{0}/Mellat/Callback?token={1}&SecondTrackCode={2}",
                            currentBaseUrl, model.TrackCode, transaction.Id);
                        transaction.BankRedirectUrl = mellatRedirectUrl;
                        await _transactionRepository.Update(transaction);
                        return Ok(
                            new PayRequestResponseModel
                            {
                                Code = transaction.Id,
                                Url = currentBaseUrl + "/Pay/" + transaction.Id
                            });
                    case BankCodeEnum.Saman:
                    default:
                        var redirectUrl = string.Format("{0}/Saman/Callback?token={1}&SecondTrackCode={2}",
                            currentBaseUrl, model.TrackCode, transaction.Id);
                        transaction.BankRedirectUrl = redirectUrl;
                        await _transactionRepository.Update(transaction);
                        return Ok(
                            new PayRequestResponseModel
                            {
                                Code = transaction.Id,
                                Url = currentBaseUrl + "/Pay/" + transaction.Id
                            });
                }
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

                switch (transaction.Bank.Code)
                {
                    case (byte) BankCodeEnum.Mellat:
                        var mellatHelper =
                            new MellatHelper(_logger, _transactionRepository, _applicationBankRepository);
                        var mellatResult = await mellatHelper.VerifyTransaction(transaction);
                        return Ok(mellatResult);

                    case (byte) BankCodeEnum.Saman:
                    default:
                        var samanHelper = new SamanHelper(_logger, _transactionRepository, _applicationBankRepository);
                        var result = await samanHelper.VerifyTransaction(transaction);
                        return Ok(result);
                }
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

                if (transaction.Status == (byte) TransactionStatusEnum.BankOk ||
                    transaction.Status == (byte) TransactionStatusEnum.Success)
                    return BadRequest(new CancelPaymentResponseModel
                    {
                        ErrorCode = (byte) ErrorCodeEnum.OperationAlreadyDone,
                        Message = "پرداخت قبلا انجام شده است",
                        Status = false
                    });

                transaction.Status = (byte) TransactionStatusEnum.Cancel;
                transaction.ModifiedBy = 1;
                transaction.ModifiedOn = DateTime.Now;
                await _transactionRepository.Update(transaction);
                return Ok(new CancelPaymentResponseModel
                {
                    ErrorCode = (byte) ErrorCodeEnum.NoError,
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