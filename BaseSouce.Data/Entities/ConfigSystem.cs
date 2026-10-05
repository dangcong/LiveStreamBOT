using BaseSource.Shared.Interfaces;

namespace BaseSource.Data.Entities
{
    public class ConfigSystem : IBotChatId
    {
        public int Id { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
        public string BankAccountName { get; set; }
        public string LinkFBAdmin { get; set; }
        public string LinkYoutube { get; set; }


        // username/userid split by ,
        public string TelegramUserIds { get; set; }



        IEnumerable<string> IBotChatId.TelegramUserNames
        {
            get
            {
                return TelegramUserIds?
                    .Split(',')?
                    .Select(x => x.Trim()) ?? Enumerable.Empty<string>();
            }
        }
    }
}
