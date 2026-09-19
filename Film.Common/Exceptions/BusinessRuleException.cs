namespace Film.Common.Exceptions
{
    /// <summary>
    /// Кидається, коли DTO не порушує модель (не 400 Bad Request через анотації),
    /// але порушує бізнес-правило (наприклад, дублікат назви фільму, некоректний рік).
    /// Application кидає цей виняток, Presentation ловить його і повертає 422/400 з повідомленням.
    /// </summary>
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message)
        {
        }
    }
}
