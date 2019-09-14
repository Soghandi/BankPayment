namespace Adin.BankPayment.Efarda
{
    public static class EfardaErrors
    {
        public static (bool Success, string Message) GetResult(int result)
        {
            switch (result)
            {
                case 0:
                    return (true, "موفق");

                case 314:
                    return (false, "آی پی نامعتبر است");

                case 315:
                    return (false, "نام کاربری یا رمز عبور نامعتبر است");

                case 316:
                    return (false, "کاربر غیر فعال است");

                case 317:
                    return (false, "به دلیل انجام تراکنش نامعتبر کاربر غیر فعال است");

                case 318:
                    return (false, "کاربر موجود نیست");

                case 319:
                    return (false, "مبلغ نامعتبر است");

                case 320:
                    return (false, "شماره سرویس نامعتبر است");

                case 321:
                    return (false, "شماره پیگیری تکراری است");

                case 322:
                    return (false, "شماره پیگیری به کاربر تعلق ندارد");

                case 324:
                    return (false, "شماره همراه نامعتبر است");

                case 325:
                    return (false, "آدرس بازنشتی نامعتبر است");

                case 327:
                    return (false, "تعداد سرویس بیشتر از حد مجاز است");

                case 328:
                    return (false, "شماره سرویس تکراری است");

                case 329:
                    return (false, "مجموع مبالغ سرویس ها نامعتبر است");

                case 330:
                    return (false, "شماره پیگیری نامعتبر است");

                case 999:
                default:
                    return (false, "خطای کلی");
            }
        }
    }
}