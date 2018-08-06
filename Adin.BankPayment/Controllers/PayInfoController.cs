using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Adin.BankPayment.Service;
using Adin.BankPayment.Domain.Model;
using System.Linq;
using Adin.BankPayment.Connector;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Connector.Model;
using Adin.BankPayment.Extension;

namespace Adin.BankPayment.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PayInfoController : Controller
    {
        private readonly ILogger<PayController> _logger;
        private IRepository<Transaction> _transactionRepository;
        private IRepository<Application> _applicationRepository;
        private IRepository<Bank> _bankRepository;
        private IRepository<ApplicationBank> _applicationBankRepository;



        public PayInfoController(ILogger<PayController> logger,
                             IRepository<Transaction> transactionRepository,
                             IRepository<Application> applicationRepository,
                             IRepository<Bank> bankRepository,
                             IRepository<ApplicationBank> applicationBankRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _applicationRepository = applicationRepository;
            _bankRepository = bankRepository;
            _applicationBankRepository = applicationBankRepository;
        }

        [HttpPost]
        public async Task<IActionResult> RequestPay([FromBody]PayInfoModel model)
        {
            try
            {

                Application application = await _applicationRepository.GetFirstBy(x => x.PublicKey == model.PublicKey);
                if (application == null || application.Status != 0)
                {
                    return Unauthorized();
                }
                if (model.PriceUnit == PriceUnitEnum.Toman)
                {
                    model.Amount = model.Amount * 10;
                }
                if (model.PriceUnit == PriceUnitEnum.Dollar)
                {
                    throw new NotImplementedException("Dollar not supported yet");
                }

                //Todo: Find Best Bank For User
                Bank bank = await _bankRepository.GetFirstBy(x => x.Code == (byte)model.BankCode);
                var transaction = new Transaction()
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

                var applicationBank = await _applicationBankRepository.GetFirstBy(x => x.ApplicationId == application.Id && x.BankId == bank.Id);

                var currentBaseUrl = string.Format("{0}://{1}", Request.Scheme, Request.Host);

                switch (model.BankCode)
                {
                    case BankCodeEnum.Parsian:
                        var parsianRedirectUrl = string.Format("{0}/Parsian/Callback?token={1}&SecondTrackCode={2}", currentBaseUrl, model.TrackCode, transaction.Id);
                        transaction.BankRedirectUrl = parsianRedirectUrl;
                        await _transactionRepository.Update(transaction);
                        return Ok(
                            new PayRequestResponseModel
                            {
                                Code = transaction.Id,
                                Url = currentBaseUrl + "/Pay/" + transaction.Id
                            });

                    case BankCodeEnum.Mellat:
                        var mellatRedirectUrl = string.Format("{0}/Mellat/Callback?token={1}&SecondTrackCode={2}", currentBaseUrl, model.TrackCode, transaction.Id);
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
                        var redirectUrl = string.Format("{0}/Saman/Callback?token={1}&SecondTrackCode={2}", currentBaseUrl, model.TrackCode, transaction.Id);
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


        [HttpGet()]
        public async Task<IActionResult> Verify(string publicKey, Guid id)
        {
            try
            {
                Application application = await _applicationRepository.GetFirstBy(x => x.PublicKey == publicKey);
                if (application == null || application.Status != 0)
                {
                    return Unauthorized();
                }

                var transaction = await _transactionRepository.Get(id);
                if (transaction == null)
                {
                    return Unauthorized();
                }

                switch (transaction.Bank.Code)
                {
                    case ((byte)BankCodeEnum.Mellat):
                        MellatHelper mellatHelper = new MellatHelper(_logger, _transactionRepository, _applicationRepository, _bankRepository, _applicationBankRepository);
                        VerifyTransactionResponseModel mellatResult = await mellatHelper.VerifyTransaction(transaction);
                        return Ok(mellatResult);

                    case ((byte)BankCodeEnum.Saman):
                    default:
                        SamanHelper samanHelper = new SamanHelper(_logger, _transactionRepository, _applicationRepository, _bankRepository, _applicationBankRepository);
                        VerifyTransactionResponseModel result = await samanHelper.VerifyTransaction(transaction);
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


    }
}

