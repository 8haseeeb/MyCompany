namespace Promotions.Domain.Participants
{
    public class PromoParticipants
    {
        public int IdAction { get; set; }
        public string CodParticipant { get; set; } = null!;
        public bool FlgInclusion { get; set; }

        public void Include() => FlgInclusion = true;
        public void Exclude() => FlgInclusion = false;
    }
}