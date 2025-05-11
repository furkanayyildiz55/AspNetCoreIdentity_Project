namespace AspNetCoreIdentity.Web.Models
{
    //Daha küçük bir alan kullanmak için byte kullanıyoruz. Default olarak int değerleri ile verisi tutuluyordu
    public enum Gender : byte
    {
        Kadın = 1,
        Erkek = 2,
    }
}
