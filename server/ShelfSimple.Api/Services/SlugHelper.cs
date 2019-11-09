using System.Text;

namespace ShelfSimple.Api.Services
{
    public static class SlugHelper
    {
        // "Maple & Twine" -> "maple-twine". Good enough for shop names.
        public static string Slugify(string text)
        {
            var sb = new StringBuilder();
            var lastDash = true;
            foreach (var c in (text ?? "").ToLowerInvariant())
            {
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    lastDash = false;
                }
                else if (!lastDash)
                {
                    sb.Append('-');
                    lastDash = true;
                }
            }

            var slug = sb.ToString().Trim('-');
            if (slug.Length > 50)
                slug = slug.Substring(0, 50).TrimEnd('-');
            return slug.Length == 0 ? "shop" : slug;
        }
    }
}
