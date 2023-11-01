using System.Diagnostics.CodeAnalysis;

namespace Services.ViewModels
{
    public class IdNameView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class IdNameViewComparer : IEqualityComparer<IdNameView>
    {
        public bool Equals(IdNameView? x, IdNameView? y)
        {
            if (x == null || y == null)
                return false;

            return x.Id == y.Id && x.Name == y.Name;
        }

        public int GetHashCode([DisallowNull] IdNameView obj)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + obj.Id.GetHashCode();
                hash = hash * 23 + (obj.Name != null ? obj.Name.GetHashCode() : 0);
                return hash;
            }
        }
    }
}
