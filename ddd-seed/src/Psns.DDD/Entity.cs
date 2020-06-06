namespace Psns.DDD
{
    public abstract class Entity
    {
        int? _requestedHashCode;
        int _Id;

        public virtual int Id
        {
            get => _Id;
            protected set => _Id = value;
        }

        public bool IsTransient() =>
            Id == default;

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var entity = (Entity)obj;

            return !(entity.IsTransient() || IsTransient()) || entity.Id == Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                {
                    _requestedHashCode = Id.GetHashCode() ^ 31;
                }

                // XOR for random distribution. See:
                // https://docs.microsoft.com/archive/blogs/ericlippert/guidelines-and-rules-for-gethashcode
                return _requestedHashCode.Value;
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public static bool operator ==(Entity left, Entity right) =>
            Equals(left, null)
                ? Equals(right, null)
                : left.Equals(right);

        public static bool operator !=(Entity left, Entity right) =>
            !(left == right);
    }
}
