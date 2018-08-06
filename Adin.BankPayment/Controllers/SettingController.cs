using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Adin.BankPayment.Service;
using Adin.BankPayment.Domain.Model;
using System.Linq;

namespace Adin.BankPayment.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SettingController : Controller
    {
        private readonly ILogger<SettingController> _logger;
        private IRepository<Transaction> _transactionRepository;
        private IRepository<Application> _applicationRepository;
        private IRepository<Bank> _bankRepository;
        private IRepository<ApplicationBank> _applicationBankRepository;
        private IRepository<ApplicationBankParam> _applicationBankParamRepository;


        public SettingController(ILogger<SettingController> logger,
                             IRepository<Transaction> transactionRepository,
                             IRepository<Application> applicationRepository,
                             IRepository<Bank> bankRepository,
                             IRepository<ApplicationBank> applicationBankRepository,
                             IRepository<ApplicationBankParam> applicationBankParamRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _applicationRepository = applicationRepository;
            _bankRepository = bankRepository;
            _applicationBankRepository = applicationBankRepository;
            _applicationBankParamRepository = applicationBankParamRepository;
        }


        [HttpPost]
        public async Task<IActionResult> AddApplication(string title, string publickey, string description)
        {
            try
            {
                Application app = await _applicationRepository.GetFirstBy(x => x.Title == title);
                if (app != null)
                {
                    return BadRequest("اپلیکیشنی با همین نام از قبل وجود دارد");
                }

                if (publickey == null || publickey .Length<16)
                {
                    return BadRequest("طول کلید باید بیشتر از 16 کاراکتر باشد");
                }
                Application application = new Application
                {
                    IsDeleted = false,
                    Status = (byte)Domain.Enum.ApplicationEnum.Normal,
                    Title = title,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    PublicKey = publickey,
                    PrivateKey = null,
                    Description = description,
                };
                await _applicationRepository.Add(application);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }


        [HttpPost]
        public async Task<IActionResult> SetSamanParams(string publicKey, string MID)
        {
            try
            {
                Application application = await _applicationRepository.GetFirstBy(x => x.PublicKey == publicKey);
                if (application == null)
                {
                    return Unauthorized();
                }
                var bank = await _bankRepository.GetFirstBy(x => x.Code == (byte)Connector.Enum.BankCodeEnum.Saman);
                var applicationBank = await _applicationBankRepository.GetFirstBy(x => x.ApplicationId == application.Id && x.BankId == bank.Id);

                if (applicationBank == null)
                {
                    applicationBank = new ApplicationBank
                    {
                        CreatedBy = 1,
                        BankId = bank.Id,
                        Status = 0,
                        ApplicationId = application.Id,
                        CreationDate = DateTime.Now
                    };
                    await _applicationBankRepository.Add(applicationBank);
                    ApplicationBankParam applicationBankParam = new ApplicationBankParam();
                    applicationBankParam.CreatedBy = 1;
                    applicationBankParam.CreationDate = DateTime.Now;
                    applicationBankParam.ParamKey = "MID";
                    applicationBankParam.ParamValue = MID;
                    applicationBankParam.Status = 0;
                    applicationBankParam.ApplicationBankId = applicationBank.Id;
                    await _applicationBankParamRepository.Add(applicationBankParam);
                    return Ok();
                }
                else
                {
                    var midParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MID");
                    midParam.ParamValue = MID;
                    await _applicationBankRepository.Update(applicationBank);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetParsianParams(string publicKey, string Pin)
        {
            try
            {
                Application application = await _applicationRepository.GetFirstBy(x => x.PublicKey == publicKey);
                if (application == null)
                {
                    return Unauthorized();
                }
                var bank = await _bankRepository.GetFirstBy(x => x.Code == (byte)Connector.Enum.BankCodeEnum.Parsian);
                var applicationBank = await _applicationBankRepository.GetFirstBy(x => x.ApplicationId == application.Id && x.BankId == bank.Id);

                if (applicationBank == null)
                {
                    applicationBank = new ApplicationBank
                    {
                        CreatedBy = 1,
                        BankId = bank.Id,
                        Status = 0,
                        ApplicationId = application.Id,
                        CreationDate = DateTime.Now
                    };
                    await _applicationBankRepository.Add(applicationBank);
                    ApplicationBankParam applicationBankParam = new ApplicationBankParam();
                    applicationBankParam.CreatedBy = 1;
                    applicationBankParam.CreationDate = DateTime.Now;
                    applicationBankParam.ParamKey = "ParsianPIN";
                    applicationBankParam.ParamValue = Pin;
                    applicationBankParam.Status = 0;
                    applicationBankParam.ApplicationBankId = applicationBank.Id;
                    await _applicationBankParamRepository.Add(applicationBankParam);
                    return Ok();
                }
                else
                {
                    var midParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "ParsianPIN");
                    midParam.ParamValue = Pin;
                    await _applicationBankRepository.Update(applicationBank);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetMellatParams(string publicKey, string terminalId, string userName, string password)
        {
            try
            {
                Application application = await _applicationRepository.GetFirstBy(x => x.PublicKey == publicKey);
                if (application == null)
                {
                    return Unauthorized();
                }
                var bank = await _bankRepository.GetFirstBy(x => x.Code == (byte)Connector.Enum.BankCodeEnum.Mellat);
                var applicationBank = await _applicationBankRepository.GetFirstBy(x => x.ApplicationId == application.Id && x.BankId == bank.Id);

                if (applicationBank == null)
                {
                    applicationBank = new ApplicationBank
                    {
                        CreatedBy = 1,
                        BankId = bank.Id,
                        Status = 0,
                        ApplicationId = application.Id,
                        CreationDate = DateTime.Now
                    };
                    await _applicationBankRepository.Add(applicationBank);
                    ApplicationBankParam terminalBankParam = new ApplicationBankParam();
                    terminalBankParam.CreatedBy = 1;
                    terminalBankParam.CreationDate = DateTime.Now;
                    terminalBankParam.ParamKey = "MellatTerminalId";
                    terminalBankParam.ParamValue = terminalId;
                    terminalBankParam.Status = 0;
                    terminalBankParam.ApplicationBankId = applicationBank.Id;
                    await _applicationBankParamRepository.Add(terminalBankParam);

                    ApplicationBankParam userNameBankParam = new ApplicationBankParam();
                    userNameBankParam.CreatedBy = 1;
                    userNameBankParam.CreationDate = DateTime.Now;
                    userNameBankParam.ParamKey = "MellatUserName";
                    userNameBankParam.ParamValue = userName;
                    userNameBankParam.Status = 0;
                    userNameBankParam.ApplicationBankId = applicationBank.Id;
                    await _applicationBankParamRepository.Add(userNameBankParam);

                    ApplicationBankParam passwordBankParam = new ApplicationBankParam();
                    passwordBankParam.CreatedBy = 1;
                    passwordBankParam.CreationDate = DateTime.Now;
                    passwordBankParam.ParamKey = "MellatPassword";
                    passwordBankParam.ParamValue = password;
                    passwordBankParam.Status = 0;
                    passwordBankParam.ApplicationBankId = applicationBank.Id;
                    await _applicationBankParamRepository.Add(passwordBankParam);
                    return Ok();
                }
                else
                {
                    var midParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatTerminalId");
                    midParam.ParamValue = terminalId;
                    await _applicationBankRepository.Update(applicationBank);

                    var userNameParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatUserName");
                    userNameParam.ParamValue = userName;
                    await _applicationBankRepository.Update(applicationBank);

                    var passwordParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatPassword");
                    passwordParam.ParamValue = password;
                    await _applicationBankRepository.Update(applicationBank);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

    }
}


