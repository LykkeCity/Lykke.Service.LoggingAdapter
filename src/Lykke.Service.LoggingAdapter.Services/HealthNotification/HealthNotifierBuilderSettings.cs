namespace Lykke.Service.LoggingAdapter.Services.HealthNotification
{
    public class HealthNotifierBuilderSettings
    {
        public readonly string AppName;
        public readonly string AppVersion;
        public readonly string EnvInfo;

        public HealthNotifierBuilderSettings(string appName, string appVersion, string envInfo)
        {
            AppName = appName;
            AppVersion = appVersion;
            EnvInfo = envInfo;
        }

        protected bool Equals(HealthNotifierBuilderSettings other)
        {
            return string.Equals(AppName, other.AppName) && string.Equals(AppVersion, other.AppVersion) && string.Equals(EnvInfo, other.EnvInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((HealthNotifierBuilderSettings)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (AppName != null ? AppName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AppVersion != null ? AppVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EnvInfo != null ? EnvInfo.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
