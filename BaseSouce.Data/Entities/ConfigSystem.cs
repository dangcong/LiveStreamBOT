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
        public string DiscordUserIds { get; set; }



        IEnumerable<string> IBotChatId.TelegramUserNames
        {
            get
            {
                return TelegramUserIds?
                    .Split(',')?
                    .Select(x => x.Trim()) ?? Enumerable.Empty<string>();
            }
        }
        IEnumerable<ulong> IBotChatId.DiscordUserIds
        {
            get
            {
                return DiscordUserIds?
                    .Split(',')?
                    .Select(x =>
                    {
                        ulong? result = null;
                        if (ulong.TryParse(x.Trim(), out ulong r))
                            result = r;
                        return result;
                    })?
                    .Where(x => x.HasValue)?
                    .Select(x => x.Value) ?? Enumerable.Empty<ulong>();
            }
        }
    }
}
