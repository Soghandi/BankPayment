using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Domain.Enum;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Adin.BankPayment.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SettingController : BaseController
    {
        private readonly IRepository<ApplicationBankParam> _applicationBankParamRepository;
        private readonly IRepository<ApplicationBank> _applicationBankRepository;
        private readonly IRepository<Application> _applicationRepository;
        private readonly IRepository<Bank> _bankRepository;
        private readonly ILogger<SettingController> _logger;

        public SettingController(IMemoryCache memCaches,
            ILogger<SettingController> logger,
            IRepository<Application> applicationRepository,
            IRepository<Bank> bankRepository,
            IRepository<ApplicationBank> applicationBankRepository,
            IRepository<ApplicationBankParam> applicationBankParamRepository) : base(memCaches, applicationRepository)
        {
            _logger = logger;
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
                var app = await _applicationRepository.GetFirstBy(x => x.Title == title);
                if (app != null) return BadRequest("اپلیکیشنی با همین نام از قبل وجود دارد");

                if (publickey == null || publickey.Length < 16)
                    return BadRequest("طول کلید باید بیشتر از 16 کاراکتر باشد");
                var application = new Application
                {
                    IsDeleted = false,
                    Status = (byte)ApplicationEnum.Normal,
                    Title = title,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    PublicKey = publickey,
                    PrivateKey = null,
                    Description = description
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
                var application = await _applicationRepository.GetFirstBy(x => x.PublicKey == publicKey);
                if (application == null) return Unauthorized();
                var bank = await _bankRepository.GetFirstBy(x => x.Code == (byte)BankCodeEnum.Saman);
                var applicationBank =
                    await _applicationBankRepository.GetFirstBy(x =>
                        x.ApplicationId == application.Id && x.BankId == bank.Id);

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
                    var applicationBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "MID",
                        ParamValue = MID,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id
                    };
                    await _applicationBankParamRepository.Add(applicationBankParam);
                    return Ok();
                }

                var midParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MID");
                midParam.ParamValue = MID;
                await _applicationBankRepository.Update(applicationBank);
                return Ok();
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
                var application = await _applicationRepository.GetFirstBy(x => x.PublicKey == publicKey);
                if (application == null) return Unauthorized();
                var bank = await _bankRepository.GetFirstBy(x => x.Code == (byte)BankCodeEnum.Parsian);
                var applicationBank =
                    await _applicationBankRepository.GetFirstBy(x =>
                        x.ApplicationId == application.Id && x.BankId == bank.Id);

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
                    var applicationBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "ParsianPIN",
                        ParamValue = Pin,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id
                    };
                    await _applicationBankParamRepository.Add(applicationBankParam);
                    return Ok();
                }

                var midParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "ParsianPIN");
                midParam.ParamValue = Pin;
                await _applicationBankRepository.Update(applicationBank);
                return Ok();
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
                var application = await _applicationRepository.GetFirstBy(x => x.PublicKey == publicKey);
                if (application == null) return Unauthorized();
                var bank = await _bankRepository.GetFirstBy(x => x.Code == (byte)BankCodeEnum.Mellat);
                var applicationBank =
                    await _applicationBankRepository.GetFirstBy(x =>
                        x.ApplicationId == application.Id && x.BankId == bank.Id);

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
                    var terminalBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "MellatTerminalId",
                        ParamValue = terminalId,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id
                    };
                    await _applicationBankParamRepository.Add(terminalBankParam);

                    var userNameBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "MellatUserName",
                        ParamValue = userName,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id
                    };
                    await _applicationBankParamRepository.Add(userNameBankParam);

                    var passwordBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "MellatPassword",
                        ParamValue = password,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id
                    };
                    await _applicationBankParamRepository.Add(passwordBankParam);
                    return Ok();
                }

                var midParam =
                    applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatTerminalId");
                midParam.ParamValue = terminalId;
                await _applicationBankRepository.Update(applicationBank);

                var userNameParam =
                    applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatUserName");
                userNameParam.ParamValue = userName;
                await _applicationBankRepository.Update(applicationBank);

                var passwordParam =
                    applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatPassword");
                passwordParam.ParamValue = password;
                await _applicationBankRepository.Update(applicationBank);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetEfardaParams(string publicKey, string serviceId, string userName, string password)
        {
            try
            {
                var application = await _applicationRepository.GetFirstBy(x => x.PublicKey == publicKey);
                if (application == null) return Unauthorized();

                var bank = await _bankRepository.GetFirstBy(x => x.Code == (byte)BankCodeEnum.Efarda);
                var applicationBank =
                    await _applicationBankRepository.GetFirstBy(x =>
                        x.ApplicationId == application.Id && x.BankId == bank.Id);

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

                    var terminalBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "serviceId",
                        ParamValue = serviceId,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id
                    };
                    await _applicationBankParamRepository.Add(terminalBankParam);

                    var userNameBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "userName",
                        ParamValue = userName,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id
                    };
                    await _applicationBankParamRepository.Add(userNameBankParam);

                    var passwordBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "password",
                        ParamValue = password,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id
                    };
                    await _applicationBankParamRepository.Add(passwordBankParam);

                    return Ok();
                }

                var serviceIdParam =
                    applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "serviceId");
                serviceIdParam.ParamValue = serviceId;
                await _applicationBankRepository.Update(applicationBank);

                var userNameParam =
                    applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "userName");
                userNameParam.ParamValue = userName;
                await _applicationBankRepository.Update(applicationBank);

                var passwordParam =
                    applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "password");
                passwordParam.ParamValue = password;
                await _applicationBankRepository.Update(applicationBank);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetPasargadParams(string publicKey, string merchantCode, string terminalCode, string privateKey)
        {
            try
            {
                var application = await _applicationRepository.GetFirstBy(x => x.PublicKey == publicKey);
                if (application == null) return Unauthorized();

                var bank = await _bankRepository.GetFirstBy(x => x.Code == (byte)BankCodeEnum.Efarda);
                var applicationBank =
                    await _applicationBankRepository.GetFirstBy(x =>
                        x.ApplicationId == application.Id && x.BankId == bank.Id);

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

                    var terminalBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "merchantCode",
                        ParamValue = merchantCode,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id
                    };
                    await _applicationBankParamRepository.Add(terminalBankParam);

                    var userNameBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "terminalCode",
                        ParamValue = terminalCode,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id
                    };
                    await _applicationBankParamRepository.Add(userNameBankParam);

                    var passwordBankParam = new ApplicationBankParam
                    {
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        ParamKey = "privateKey",
                        ParamValue = privateKey,
                        Status = 0,
                        ApplicationBankId = applicationBank.Id,
                    };
                    await _applicationBankParamRepository.Add(passwordBankParam);

                    return Ok();
                }

                var serviceIdParam =
                    applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "merchantCode");
                serviceIdParam.ParamValue = merchantCode;
                await _applicationBankRepository.Update(applicationBank);

                var userNameParam =
                    applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "terminalCode");
                userNameParam.ParamValue = terminalCode;
                await _applicationBankRepository.Update(applicationBank);

                var passwordParam =
                    applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "privateKey");
                passwordParam.ParamValue = privateKey;
                await _applicationBankRepository.Update(applicationBank);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Protected()
        {
            var app = await GetApplicationAsync();
            return Ok(app.Title);
        }
    }
}