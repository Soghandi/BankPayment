namespace Adin.BankPayment.Pasargad
{
    public static class PasargadErrors
    {
        public static (bool Success, string Message) GetResult(string result)
        {
            if (int.TryParse(result, out int resultCode))
            {
                return resultCode switch
                {
                    0 => (true, "موفق"),
                    314 => (false, "آی پی نامعتبر است"),
                    315 => (false, "نام کاربری یا رمز عبور نامعتبر است"),
                    316 => (false, "کاربر غیر فعال است"),
                    317 => (false, "به دلیل انجام تراکنش نامعتبر کاربر غیر فعال است"),
                    318 => (false, "کاربر موجود نیست"),
                    319 => (false, "مبلغ نامعتبر است"),
                    320 => (false, "شماره سرویس نامعتبر است"),
                    321 => (false, "شماره پیگیری تکراری است"),
                    322 => (false, "شماره پیگیری به کاربر تعلق ندارد"),
                    324 => (false, "شماره همراه نامعتبر است"),
                    325 => (false, "آدرس بازنشتی نامعتبر است"),
                    327 => (false, "تعداد سرویس بیشتر از حد مجاز است"),
                    328 => (false, "شماره سرویس تکراری است"),
                    329 => (false, "مجموع مبالغ سرویس ها نامعتبر است"),
                    330 => (false, "شماره پیگیری نامعتبر است"),
                    _ => (false, "خطای کلی"),
                };
            }
            return (false, "خطای کلی 2");
        }
    }
}